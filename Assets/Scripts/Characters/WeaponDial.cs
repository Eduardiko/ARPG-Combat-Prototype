using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDial : MonoBehaviour
{
    [Header("Weapon Transforms")]
    [SerializeField] private Transform topRWeaponTransform;
    [SerializeField] private Transform bottomRWeaponTransform;
    [SerializeField] private Transform topLWeaponTransform;
    [SerializeField] private Transform bottomLWeaponTransform;

    [Header("Weapon UI Sprites")]
    [SerializeField] private RectTransform topWeaponRect;
    [SerializeField] private RectTransform bottomWeaponRect;
    [SerializeField] private RectTransform manualPointerRect;

    [SerializeField] private RectTransform topTargetWeaponRect;
    [SerializeField] private RectTransform bottomTargetWeaponRect;

    [Header("Plane Reference")]
    [SerializeField] private Transform referencePlaneTransform;

    // References
    private Character character;
    private InputManager inputManager;

    // Variables
    [HideInInspector] public float topAngle;
    [HideInInspector] public float bottomAngle;
    [HideInInspector] public float manualAngle;
    [HideInInspector] public bool isUIWeaponAttached = true;

    private float radius;

    private Vector3 planeNormal;

    private Vector3 topProjection;
    private Vector3 bottomProjection;

    private Vector3 topRefPoint;
    private Vector3 centerRefPoint;
    private Vector3 bottomRefPoint;

    private Transform topWeaponTransform;
    private Transform bottomWeaponTransform;


    private void Start()
    {
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        if(!character.isDead)
        {
            UpdateWeapon();

            SetAngles();
            UpdateUI();
        }
    }

    #region ANGLES

    private void SetAngles()
    {
        if (!isUIWeaponAttached)
            SetTopBottomAngles();
        else
            SetAttachedTopBottomAngles();

        ManageManualAngle();
    }

    private void SetTopBottomAngles()
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

        // Calculate the angles
        Vector3 centerToRef;
        Vector3 centerToPoint;

        // Convert the {-180, 180} returned by the SignedAngle function to {0, 360} for QoL -> condition ? true : false
        centerToRef = topRefPoint - centerRefPoint;
        centerToPoint = topProjection - centerRefPoint;
        topAngle = Vector3.SignedAngle(centerToPoint, centerToRef, planeNormal);
        topAngle = topAngle < 0 ? 360f + topAngle : topAngle;

        centerToRef = bottomRefPoint - centerRefPoint;
        centerToPoint = topProjection - centerRefPoint;
        bottomAngle = Vector3.SignedAngle(centerToPoint, centerToRef, planeNormal);
        bottomAngle = bottomAngle < 0 ? 360f + bottomAngle : bottomAngle;
    }

    private void SetAttachedTopBottomAngles()
    {
        // Attach Top Angle and set Bottom Angle to + 180º
        topAngle = manualAngle;
        bottomAngle = manualAngle + 180 > 360 ? manualAngle + 180 - 360 : manualAngle + 180;
    }

    private void ManageManualAngle()
    {
        if (inputManager.inputWeaponDialVector != Vector2.zero && character.isLocking)
        {
            // Set UI Active to be rendered
            manualPointerRect.gameObject.SetActive(true);
            isUIWeaponAttached = true;

            // Calculate manual angle
            float angleRadians = Mathf.Atan2(inputManager.inputWeaponDialVector.x, inputManager.inputWeaponDialVector.y);
            manualAngle = angleRadians * Mathf.Rad2Deg;
            manualAngle = manualAngle < 0 ? 360f + manualAngle : manualAngle;
        }
        else
        {
            // Unnattach weapon and stop rendering UI
            isUIWeaponAttached = false;
            manualPointerRect.gameObject.SetActive(false);
        }
    }

    // ---- Uncomment To Use Threshold System - Add at the beggining ----

    //[Header("Parameters")]
    //[SerializeField] private float attachAcceptanceThreshold = 15f;

    // ---- Uncomment To Use Threshold System - Replace SetAttachedTopBottomAngles() code ----

    //float angularTopDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, topAngle));
    //float angularBottomDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, bottomAngle));

    //// Attach the nearest part to the manual angle
    //if (angularTopDifference < angularBottomDifference)
    //{
    //    topAngle = manualAngle;
    //    bottomAngle = manualAngle + 180 > 360 ? manualAngle + 180 - 360 : manualAngle + 180;
    //}
    //else
    //{
    //    bottomAngle = manualAngle;
    //    topAngle = manualAngle + 180 > 360 ? manualAngle + 180 - 360 : manualAngle + 180;
    //}

    // ---- Uncomment To Use Threshold System - Add code in ManageManualAngle() ----

    //float angularTopDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, topAngle));
    //float angularBottomDifference = Mathf.Abs(Mathf.DeltaAngle(manualAngle, bottomAngle));
    //if (angularTopDifference <= attachAcceptanceThreshold || angularBottomDifference <= attachAcceptanceThreshold)
    //    isUIWeaponAttached = true;

    #endregion

    #region UI
    private void UpdateUI()
    {
        if(isUIWeaponAttached || !character.isUILocked)
            UpdateAnglesUI();

        if (character.isLocking && character.target != null)
            UpdateTargetAnglesUI();
    }

    private void UpdateAnglesUI()
    {
        // Top Angle
        // Convert angle to radians
        float radianTopAngle = (90 - topAngle) * Mathf.Deg2Rad;

        // Calculate the new position
        float x = 0.5f * Mathf.Cos(radianTopAngle);
        float y = 0.5f * Mathf.Sin(radianTopAngle);

        // Apply the new position
        topWeaponRect.localPosition = new Vector3(x, y, topWeaponRect.localPosition.z);

        // Bottom Angle
        float radianBottomAngle = (90 - bottomAngle) * Mathf.Deg2Rad;
        x = 0.5f * Mathf.Cos(radianBottomAngle);
        y = 0.5f * Mathf.Sin(radianBottomAngle);
        bottomWeaponRect.localPosition = new Vector3(x, y, bottomWeaponRect.localPosition.z);

        // Manual Angle
        if (manualPointerRect.gameObject.activeSelf)
        {
            float radianManualAngle = (90 - manualAngle) * Mathf.Deg2Rad;
            x = 0.5f * Mathf.Cos(radianManualAngle);
            y = 0.5f * Mathf.Sin(radianManualAngle);
            manualPointerRect.localPosition = new Vector3(x, y, manualPointerRect.localPosition.z);
        }
    }

    private void UpdateTargetAnglesUI()
    {
        Character targetCharacter = character.target.GetComponent<Character>();

        // Activate/Deactivate render
        if (targetCharacter.isUILocked)
        {
            topTargetWeaponRect.gameObject.SetActive(true);
            bottomTargetWeaponRect.gameObject.SetActive(true);
        }
        else
        {
            topTargetWeaponRect.gameObject.SetActive(false);
            bottomTargetWeaponRect.gameObject.SetActive(false);
        }

        // Target Angles (Enemy Angles)
        if (topTargetWeaponRect.gameObject.activeSelf || bottomTargetWeaponRect.gameObject.activeSelf)
        {

            float radianTopAngle = (90 - targetCharacter.attackInfo.topAngle) * Mathf.Deg2Rad;
            float x = 0.6f * Mathf.Cos(radianTopAngle);
            float y = 0.6f * Mathf.Sin(radianTopAngle);
            topTargetWeaponRect.localPosition = new Vector3(-x, y, topTargetWeaponRect.localPosition.z);

            float radianBottomAngle = (90 - targetCharacter.attackInfo.bottomAngle) * Mathf.Deg2Rad;
            x = 0.6f * Mathf.Cos(radianBottomAngle);
            y = 0.6f * Mathf.Sin(radianBottomAngle);
            bottomTargetWeaponRect.localPosition = new Vector3(-x, y, bottomTargetWeaponRect.localPosition.z);
        }
    }

    #endregion

    #region HELPERS

    private void UpdateWeapon()
    {
        // Update Top/Bottom Transforms to match active weapon (Left/Right)
        if(character.RWeapon.activeSelf)
        {
            topWeaponTransform = topRWeaponTransform;
            bottomWeaponTransform = bottomRWeaponTransform;
        } else
        {
            topWeaponTransform = topLWeaponTransform;
            bottomWeaponTransform = bottomLWeaponTransform;
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


    private void LockUI()
    {
        character.isUILocked = true;
    }

    private void UnlockUI()
    {
        character.isUILocked = false;
    }


    #endregion
}
