using UnityEngine;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    
    [Header("Weapon Transforms")]
    [SerializeField] private Transform topWeaponTransform;
    [SerializeField] private Transform bottomWeaponTransform;

    [Header("Weapon UI Sprites")]
    [SerializeField] private GameObject topWeaponSprite;
    [SerializeField] private GameObject bottomWeaponSprite;

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

        // Calculate the angle of inclination of the two projection points
        Vector3 centerToRef;
        Vector3 centerToPoint;

        centerToRef = topRefPoint - centerRefPoint;
        centerToPoint = topProjection - centerRefPoint;
        topAngle = Vector3.SignedAngle(centerToPoint, centerToRef, centerRefPoint);

        centerToRef = bottomRefPoint - centerRefPoint;
        centerToPoint = topProjection - centerRefPoint;
        bottomAngle = Vector3.SignedAngle(centerToPoint, centerToRef, centerRefPoint);
    }
    void OnDrawGizmos()
    {
        // Draw the projection points and circle
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(topProjection, 0.1f);
        Gizmos.DrawSphere(bottomProjection, 0.1f);
        Gizmos.color = Color.blue;
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
