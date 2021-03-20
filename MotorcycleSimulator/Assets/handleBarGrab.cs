using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class handleBarGrab : MonoBehaviour
{
    public GameObject handleBarHand;
    public GameObject realHandObject;
    public SteamVR_Input_Sources handSource;
    public SteamVR_Behaviour_Pose realHandPose;
    public SteamVR_Action_Boolean grabGrip;
    public bool barGripped = false;

    // Update is called once per frame
    void Update()
    {
        bool handGripped = grabGrip.GetState(handSource);

        if (handGripped && (barGripped || ((realHandPose.transform.position - this.transform.position).magnitude < 0.2f)))
        {
            handleBarHand.SetActive(true);
            realHandObject.SetActive(false);
            barGripped = true;
        }
        else
        {
            handleBarHand.SetActive(false);
            realHandObject.SetActive(true);
            barGripped = false;
        }
    }
}
