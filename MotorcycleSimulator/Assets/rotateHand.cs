using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateHand : MonoBehaviour
{
    float currentRotation = 0.0f;
    public GameObject pivotPoint;
    public GameObject bike;
    public GameObject rightHandle;
    public GameObject leftHandle;

    // Update is called once per frame
    void Update()
    {
        float throttleRotation = ControllerMove.throttleHandRotation * 90.0f;
        float mom_angle = throttleRotation - currentRotation;
        currentRotation += mom_angle;
        Vector3 pos = pivotPoint.transform.position;
        Vector3 barDir = leftHandle.transform.position - rightHandle.transform.position;

        this.transform.RotateAround(pos, barDir, mom_angle);
    }
}
