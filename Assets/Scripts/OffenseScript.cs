using UnityEngine;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    
    [Header("Weapon Transforms")]
    [SerializeField] private Transform topWeaponTransform;
    [SerializeField] private Transform bottomWeaponTransform;

    [Header("Weapon UI Sprites")]
    [SerializeField] private RectTransform topWeaponRect;
    [SerializeField] private RectTransform bottomWeaponRect;

    [Header("Plane Reference")]
    public Transform referencePlaneTransform;

    // References
    private Character character;
    private InputManager inputManager;
    private Animator characterAnimator;

    // Variables
    private float topAngle;
    private float bottomAngle;

    private float radius;

    private Vector3 planeNormal;

    private Vector3 topProjection;
    private Vector3 bottomProjection;

    private Vector3 topRefPoint;
    private Vector3 centerRefPoint;
    private Vector3 bottomRefPoint;

    // Bools
    private bool ableToAttack = false;

    // UI Parameters
    private Vector3 positionInDial;

    private void Start()
    {
        characterAnimator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        SetAngles();

        UpdatePossibleActions();

        UpdateStatesAndAnimations();

        UpdateUI();
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
    }

    private void UpdatePossibleActions()
    {
        // Can The Player Attack?
        if(character.isGrounded)
            ableToAttack = true;
        else
            ableToAttack = false;
    }

    private void UpdateStatesAndAnimations()
    {
        // Attack
        if (inputManager.tryingToAttack && ableToAttack)
        {
            inputManager.tryingToAttack = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetTrigger(character.animKeys.attackTriggerKey);
        }
        else
            inputManager.tryingToAttack = false;
    }

    private void SetAngles()
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

        Vector3 centerToRef;
        Vector3 centerToPoint;

        centerToRef = topRefPoint - centerRefPoint;
        centerToPoint = topProjection - centerRefPoint;
        topAngle = Vector3.SignedAngle(centerToPoint, centerToRef, planeNormal);

        centerToRef = bottomRefPoint - centerRefPoint;
        centerToPoint = topProjection - centerRefPoint;
        bottomAngle = Vector3.SignedAngle(centerToPoint, centerToRef, planeNormal);

        print(topAngle);
        print(bottomAngle);

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
