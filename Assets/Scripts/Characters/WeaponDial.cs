using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDial : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float attachAcceptanceThreshold = 15f;

    [Header("Weapon Transforms")]
    [SerializeField] private Transform topWeaponTransform;
    [SerializeField] private Transform bottomWeaponTransform;

    [Header("Weapon UI Sprites")]
    [SerializeField] private RectTransform topWeaponRect;
    [SerializeField] private RectTransform bottomWeaponRect;
    [SerializeField] private RectTransform manualPointerRect;

    [SerializeField] private RectTransform topTargetWeaponRect;
    [SerializeField] private RectTransform bottomTargetWeaponRect;

    [Header("Plane Reference")]
    public Transform referencePlaneTransform;

    // References
    private Character character;
    private InputManager inputManager;

    // Variables
    [HideInInspector] public float topAngle;
    [HideInInspector] public float bottomAngle;
    private float manualAngle;

    private float radius;

    private Vector3 planeNormal;

    private Vector3 topProjection;
    private Vector3 bottomProjection;

    private Vector3 topRefPoint;
    private Vector3 centerRefPoint;
    private Vector3 bottomRefPoint;

    private bool isUIWeaponAttached = true;

    private void Start()
    {
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        SetAngles();
        UpdateUI();
    }

    private void SetAngles()
    {
        if (!isUIWeaponAttached)
        {
            planeNormal = referencePlaneTransform.up;

            // Calculate the projection of the two points onto the plane
            topProjection = Vector3.ProjectOnPlane(topWeaponTransform.position - referencePlaneTransform.position, planeNormal) + referencePlaneTransform.position;
            bottomProjection = Vector3.ProjectOnPlane(bottomWeaponTransform.position - referencePlaneTransform.position, planeNormal) + referencePlaneTransform.position;

            // Calculate the center and radius of the circle that passes through both projection points
            centerRefPoint = (topProjection + bottomProjection) / 2.0f;
            radius = Vector3.Distance(centerRefPoint, topProjection);

            // Calculate the points to use as reference
            topRefPoint = centerRefPoint + Vector3.up * radius;
            bottomRefPoint = centerRefPoint + Vector3.down * radius;

            // Calculate the angle between the two Vectors
            Vector3 centerToRef;
            Vector3 centerToPoint;

            centerToRef = topRefPoint - centerRefPoint;
            centerToPoint = topProjection - centerRefPoint;
            topAngle = Vector3.SignedAngle(centerToPoint, centerToRef, planeNormal);
            // Convert the {-180, 180} returned by the SignedAngle function to {0, 360} for QoL -> condition ? true : false
            topAngle = topAngle < 0 ? 360f + topAngle : topAngle;

            centerToRef = bottomRefPoint - centerRefPoint;
            centerToPoint = topProjection - centerRefPoint;
            bottomAngle = Vector3.SignedAngle(centerToPoint, centerToRef, planeNormal);
            bottomAngle = bottomAngle < 0 ? 360f + bottomAngle : bottomAngle;
        }
        else
        {
            float angularTopDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, topAngle));
            float angularBottomDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, bottomAngle));

            if (angularTopDifference < angularBottomDifference)
            {
                topAngle = manualAngle;
                bottomAngle = manualAngle + 180 > 360 ? manualAngle + 180 - 360 : manualAngle + 180;
            }
            else
            {
                bottomAngle = manualAngle;
                topAngle = manualAngle + 180 > 360 ? manualAngle + 180 - 360 : manualAngle + 180;
            }

        }

        if (inputManager.inputWeaponDialVector != Vector2.zero && character.isLocking)
        {
            // Set UI Active to be rendered
            manualPointerRect.gameObject.SetActive(true);

            // Calculate manual angle
            float angleRadians = Mathf.Atan2(inputManager.inputWeaponDialVector.x, inputManager.inputWeaponDialVector.y);
            manualAngle = angleRadians * Mathf.Rad2Deg;
            manualAngle = manualAngle < 0 ? 360f + manualAngle : manualAngle;

            // Check if any of the two Weapon Angles is inside the threshold
            float angularTopDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, topAngle));
            float angularBottomDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, bottomAngle));
            if (angularTopDifference <= attachAcceptanceThreshold || angularBottomDifference <= attachAcceptanceThreshold)
                isUIWeaponAttached = true;
        }
        else
        {
            // Unnattach weapon and stop rendering UI
            isUIWeaponAttached = false;
            manualPointerRect.gameObject.SetActive(false);
        }

        if (character.isLocking && character.target != null)
        {
            topTargetWeaponRect.gameObject.SetActive(true);
            bottomTargetWeaponRect.gameObject.SetActive(true);
        }
        else
        {
            topTargetWeaponRect.gameObject.SetActive(false);
            bottomTargetWeaponRect.gameObject.SetActive(false);
        }

    }

    private void UpdateUI()
    {
        // Convert angle to radians and subtract pi/2 to make 0 degrees point up
        float radianTopAngle = (90 - topAngle) * Mathf.Deg2Rad;

        // Calculate the new position
        float x = 0.5f * Mathf.Cos(radianTopAngle);
        float y = 0.5f * Mathf.Sin(radianTopAngle);

        // Apply the new position
        topWeaponRect.localPosition = new Vector3(x, y, topWeaponRect.localPosition.z);


        // Convert angle to radians and subtract pi/2 to make 0 degrees point up
        float radianBottomAngle = (90 - bottomAngle) * Mathf.Deg2Rad;

        // Calculate the new position
        x = 0.5f * Mathf.Cos(radianBottomAngle);
        y = 0.5f * Mathf.Sin(radianBottomAngle);

        // Apply the new position
        bottomWeaponRect.localPosition = new Vector3(x, y, bottomWeaponRect.localPosition.z);

        if (manualPointerRect.gameObject.activeSelf)
        {
            // Convert angle to radians and subtract pi/2 to make 0 degrees point up
            float radianManualAngle = (90 - manualAngle) * Mathf.Deg2Rad;

            // Calculate the new position
            x = 0.5f * Mathf.Cos(radianManualAngle);
            y = 0.5f * Mathf.Sin(radianManualAngle);

            // Apply the new position
            manualPointerRect.localPosition = new Vector3(x, y, manualPointerRect.localPosition.z);
        }

        if(topTargetWeaponRect.gameObject.activeSelf || bottomTargetWeaponRect.gameObject.activeSelf)
        {
            WeaponDial targetWeaponDial = character.target.GetComponent<WeaponDial>();

            // Convert angle to radians and subtract pi/2 to make 0 degrees point up
            radianTopAngle = (90 - targetWeaponDial.topAngle) * Mathf.Deg2Rad;

            // Calculate the new position
            x = 0.6f * Mathf.Cos(radianTopAngle);
            y = 0.6f * Mathf.Sin(radianTopAngle);

            // Apply the new position
            topTargetWeaponRect.localPosition = new Vector3(x, y, topTargetWeaponRect.localPosition.z);


            // Convert angle to radians and subtract pi/2 to make 0 degrees point up
            radianBottomAngle = (90 - targetWeaponDial.bottomAngle) * Mathf.Deg2Rad;

            // Calculate the new position
            x = 0.6f * Mathf.Cos(radianBottomAngle);
            y = 0.6f * Mathf.Sin(radianBottomAngle);

            // Apply the new position
            bottomTargetWeaponRect.localPosition = new Vector3(x, y, bottomTargetWeaponRect.localPosition.z);
        }
        
    }

    void OnDrawGizmos()
    {
        // Draw the projection points and circle
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(topProjection, 0.1f);
        Gizmos.DrawSphere(bottomProjection, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(centerRefPoint, 0.1f);
        Gizmos.DrawWireSphere(centerRefPoint, radius);

        // Draw Lines For Top Point
        Gizmos.color = Color.green;
        Gizmos.DrawLine(centerRefPoint, topRefPoint);
        Gizmos.DrawLine(topProjection, centerRefPoint);

        // Draw Lines For Bottom Point
        Gizmos.color = Color.green;
        Gizmos.DrawLine(centerRefPoint, bottomRefPoint);
        Gizmos.DrawLine(bottomProjection, centerRefPoint);
    }
}
