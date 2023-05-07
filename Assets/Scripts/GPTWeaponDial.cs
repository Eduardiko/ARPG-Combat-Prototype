using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTWeaponDial : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public float radius = 1.0f;

    void Update()
    {
        // Calculate the center point between the two 3D points
        Vector3 center = (point1.position + point2.position) / 2.0f;

        // Calculate the distance between the two 3D points
        float distance = Vector3.Distance(point1.position, point2.position);

        // Project each point onto a plane that is perpendicular to the line connecting the two 3D points
        Vector3 line = point2.position - point1.position;
        Vector3 planeNormal = line.normalized;
        Vector3 projection1 = Vector3.ProjectOnPlane(point1.position - center, point3.position);
        Vector3 projection2 = Vector3.ProjectOnPlane(point2.position - center, point3.position);

        // Convert the projected 3D points to 2D points on the circumference
        float angle1 = Mathf.Atan2(projection1.z, projection1.x) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan2(projection2.z, projection2.x) * Mathf.Rad2Deg;

        print("Angle 1: " + angle1 + " || Angle2: " + angle2);

        float angleDiff = angle2 - angle1;
        if (angleDiff < 0.0f)
        {
            angleDiff += 360.0f;
        }
        float angleMid = angle1 + angleDiff / 2.0f;
        float angleMidRadians = angleMid * Mathf.Deg2Rad;
        float x = center.x + radius * Mathf.Cos(angleMidRadians);
        float z = center.z + radius * Mathf.Sin(angleMidRadians);
        Vector3 positionOnCircumference = new Vector3(x, center.y, z);

        // Use the position on the circumference for whatever purpose you need
        //Debug.Log("Position on circumference: " + positionOnCircumference);
    }
}
