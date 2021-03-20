using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leanPivot : MonoBehaviour
{
    float anglex = 0.0f;
    public float leanMagnitude = 1.0f;
    public CharacterController player;
    public GameObject bike;

    // Update is called once per frame
    void Update()
    {
        float leanRotation = ControllerMove.leanRotation * 90.0f * leanMagnitude;
        float mom_angle = 0.0f;
        mom_angle = leanRotation - anglex;
        bike.transform.Rotate(new Vector3(1,0,0), mom_angle);
        anglex += mom_angle;
    }
}
