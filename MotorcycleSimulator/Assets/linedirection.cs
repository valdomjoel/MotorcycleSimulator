using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class linedirection : MonoBehaviour
{
    public CharacterController controller;
    public SteamVR_Input_Sources handleft;
    public SteamVR_Input_Sources handright;
    public SteamVR_Action_Boolean gripThrottle;
    public SteamVR_Behaviour_Pose VRcontrollerPose;
    public LineRenderer linerenderer;



    // Update is called once per frame
    void Update()
    {
        bool throttleHeld = gripThrottle.GetState(handright);
        if(throttleHeld)
        {
            linerenderer.enabled = true;
        }
        else
        {
            linerenderer.enabled = false;
        }

        var pointlist = new List<Vector3>();

        Quaternion rot = VRcontrollerPose.transform.rotation;
        Matrix4x4 m = Matrix4x4.Rotate(rot);
        Vector3 pointdirection = m.MultiplyPoint3x4(new Vector3(0, 0, 1));
        float angle = Vector3.Dot(-controller.transform.forward, pointdirection);

        //Debug.Log(angle);

        for(float t = 0; t <= 5.0f; t += 1.0f)
        {
            pointlist.Add(VRcontrollerPose.transform.position + pointdirection * t);
        }

        Vector3 dir = pointdirection;
        dir.y = 0;
        dir = Vector3.Normalize(dir);

        linerenderer.positionCount = pointlist.Count;
        linerenderer.SetPositions(pointlist.ToArray());
    }
}
