using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDial : MonoBehaviour
{

    public Transform topPos;
    public Transform bottomPos;

    private Vector3 topPoint;
    private Vector3 centerPoint;
    private Vector3 bottomPoint;
    private Vector3 referencePoint;

    private Vector2 topProjection;
    private Vector2 centerProjection;
    private Vector2 bottomProjection;
    private Vector2 referenceProjection;

    private float topAngle;
    private float bottomAngle;

    private float radius;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //ToDo: Adapt X to -> Forward Vector.x
        
        topPoint = topPos.position;
        bottomPoint = bottomPos.position;

        centerPoint = (topPoint + bottomPoint) / 2;

        topProjection = new Vector2(topPoint.x, topPoint.y);
        bottomProjection = new Vector2(bottomPoint.x, bottomPoint.y);
        centerProjection = new Vector2(centerPoint.x, centerPoint.y);

        Vector2 temp = topProjection - centerProjection;
        radius = temp.magnitude;

        referencePoint = centerPoint + radius * Vector3.up;
        referenceProjection = new Vector2(referencePoint.x, referencePoint.y);

        Vector2 referenceVector = referencePoint - centerPoint;
        Vector2 topVector = topPoint - centerPoint;
        Vector2 bottomVector = bottomPoint - centerPoint;

        topAngle = Vector2.Angle(topVector, referenceVector);
        bottomAngle = Vector2.Angle(bottomVector, referenceVector);

        //print(topPoint + " - " + bottomPoint + " - " + centerPoint + " - " + referencePoint);

        print(topAngle + " - " + bottomAngle);
    }
}
