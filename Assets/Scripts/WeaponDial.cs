using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDial : MonoBehaviour
{

    public Transform refPlaneT;
    private Vector3 planeNormal;

    public Transform topT;
    public Transform bottomT;

    private Vector3 topProjection;
    private Vector3 bottomProjection;

    private Vector3 topRefPoint;
    private Vector3 centerRefPoint;
    private Vector3 bottomRefPoint;

    private float topAngle;
    private float bottomAngle;

    private float radius;

    void Update()
    {
        planeNormal = refPlaneT.up;

        // Calculate the projection of the two points onto the plane
        topProjection = Vector3.ProjectOnPlane(topT.position - refPlaneT.position, planeNormal) + refPlaneT.position;
        bottomProjection = Vector3.ProjectOnPlane(bottomT.position - refPlaneT.position, planeNormal) + refPlaneT.position;

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

        //print("Top: " + topAngle + " degrees");
        //print("Bottom: " + bottomAngle + " degrees");
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
