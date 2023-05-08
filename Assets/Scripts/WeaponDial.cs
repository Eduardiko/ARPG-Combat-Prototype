using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDial : MonoBehaviour
{

    public Transform plane;
    public Transform point1;
    public Transform point2;

    private Vector3 planeNormal;

    private Vector3 topPoint;
    private Vector3 bottomPoint;

    void Update()
    {
        planeNormal = plane.up;

        // Calculate the projection of the two points onto the plane
        Vector3 projection1 = Vector3.ProjectOnPlane(point1.position - plane.position, planeNormal) + plane.position;
        Vector3 projection2 = Vector3.ProjectOnPlane(point2.position - plane.position, planeNormal) + plane.position;

        // Calculate the center and radius of the circle that passes through both projection points
        Vector3 center = (projection1 + projection2) / 2.0f;
        float radius = Vector3.Distance(center, projection1);

        // Calculate the angle of inclination of the two projection points
        topPoint = center + Vector3.up * radius;
        bottomPoint = center + Vector3.down * radius;

        Vector3 dir1 = topPoint - center;
        Vector3 dir2 = projection1 - center;
        float angle = Vector3.SignedAngle(dir2, dir1, center);

        dir1 = bottomPoint - center;
        dir2 = projection1 - center;
        float angle2 = Vector3.SignedAngle(dir2, dir1, center);

        // Output the angle to the console
        Debug.Log("Top: " + angle + " degrees");
        Debug.Log("Bottom: " + angle2 + " degrees");
    }

    void OnDrawGizmos()
    {
        // Calculate the projection of the two points onto the plane
        Vector3 projection1 = Vector3.ProjectOnPlane(point1.position - plane.position, planeNormal) + plane.position;
        Vector3 projection2 = Vector3.ProjectOnPlane(point2.position - plane.position, planeNormal) + plane.position;

        // Calculate the center and radius of the circle that passes through both projection points
        Vector3 center = (projection1 + projection2) / 2.0f;
        float radius = Vector3.Distance(center, projection1);

        // Draw the projection points and circle using Gizmos
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(projection1, 0.1f);
        Gizmos.DrawSphere(projection2, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, radius);

        // Calculate the top point of the circle and draw a line to it from each projection point
        Gizmos.color = Color.green;
        Gizmos.DrawLine(center, topPoint);
        Gizmos.DrawLine(projection1, center);

        // Calculate the top point of the circle and draw a line to it from each projection point
        Gizmos.color = Color.green;
        Gizmos.DrawLine(center, bottomPoint);
        Gizmos.DrawLine(projection2, center);
    }

}
