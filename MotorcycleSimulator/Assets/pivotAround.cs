using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pivotAround : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float angley = 0.0f;
    public GameObject pivotPoint;
    public GameObject bike;

    // Update is called once per frame
    void Update()
    {
        float handleRotation = ControllerMove.handleRotation * 55.0f;
        float mom_angle = 0.0f;
        
        mom_angle = handleRotation - angley;
        Vector3 pos = pivotPoint.transform.position;
        this.transform.RotateAround(pos, bike.transform.up, mom_angle);

        angley += mom_angle;

    }
}
