using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerMove : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck1;
    public Transform groundCheck2;
    public float collisionRadius = 0.02f;
    public LayerMask groundMask;
    public SteamVR_Input_Sources handLeft;
    public SteamVR_Input_Sources handRight;
    public SteamVR_Action_Single brakePull;
    public SteamVR_Action_Boolean grabGrip;
    public SteamVR_Action_Boolean triggerPull;
    public SteamVR_Behaviour_Pose controllerPoseLeft;
    public SteamVR_Behaviour_Pose controllerPoseRight;
    public SteamVR_Action_Vibration vibration;
    public SteamVR_Action_Boolean resetButton;
    public GameObject rightHandle;
    public GameObject leftHandle;

    public TextMesh speedmeter;
    
    bool onGround;
    static public Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    float velmag = 0.0f;
    static public float aoa = 0;
    public bool stopMovement = false;
    Vector3 startpos;
    bool throttleGripped = false;

    public float forwardForceFactor = 10.0f;
    public float liftFactor = 70.0f;
    public float dragFactor = 10.0f;
    public float brakeFactor = 1.0f;
    public float turnspeed = 2.0f;
    public float jumpFactor = 0.5f;
    public float freeHandleRotationFactor = 0.1f;

    public float horsepower = 10.0f;

    static public float throttleHandRotation = 0.0f;
    static public float throttle = 0.0f;
    static public float handleRotation = 0.0f;
    static public float leanRotation = 0.0f;
    float startHandleRotation = 0.0f;
    float brake = 0.0f;
    float ySpeed = 0.0f;
    float lastYPosition = 0.0f;

    bool onAir = false;

    // Start is called before the first frame update
    void Start()
    {
        startpos = controller.transform.position;
        lastYPosition = controller.transform.position.y;
    }

    bool fwdcheck = false;
    bool bwdcheck = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(resetButton.GetState(handLeft) || resetButton.GetState(handRight))
        {
            resetGame();
            return;
        }

        bool onGround1 = Physics.CheckSphere(groundCheck1.position, collisionRadius, groundMask);
        bool onGround2 = Physics.CheckSphere(groundCheck2.position, collisionRadius, groundMask);
        onGround = onGround1 || onGround2;
        
        Quaternion rot = controllerPoseRight.transform.rotation;
        Matrix4x4 m = Matrix4x4.Rotate(rot);
        Vector3 throttleDirection = m.MultiplyPoint3x4(new Vector3(1, 0, 0));
        throttleHandRotation = Vector3.Dot(-controller.transform.forward, throttleDirection);

        Vector3 gripDirection = m.MultiplyPoint3x4(new Vector3(0, 0, 1));
        float gripHAlignment = Vector3.Dot(-controller.transform.right, gripDirection);
        float gripVAlignment = Vector3.Dot(controller.transform.up, throttleDirection);
        bool rightHandleGripped = rightHandle.GetComponent<handleBarGrab>().barGripped;

        if (rightHandleGripped && !throttleGripped && (gripHAlignment > 0.55) && (gripVAlignment > 0.75))
        {
            throttleGripped = true;
            startHandleRotation = throttleHandRotation;
        }
        else if (rightHandleGripped && throttleGripped)
        {
            throttle = throttleHandRotation - startHandleRotation;
        }
        else
        {
            throttle = 0;
            throttleGripped = false;
        }

        if (throttle < 0) throttle = 0;

        brake = brakeFactor * GetBrake();

        Vector3 velocityForward = velocity;
        velocityForward.y = 0;

        Vector3 actualVelocityForward = controller.velocity;
        actualVelocityForward.y = 0;

        ySpeed = controller.velocity.y * Time.deltaTime;
        //ySpeed = controller.transform.position.y - lastYPosition;
        lastYPosition = controller.transform.position.y;

        if (Mathf.Abs(actualVelocityForward.magnitude * Time.deltaTime - velocityForward.magnitude) > 0.01f)
            velocity = Vector3.Normalize(velocity) * controller.velocity.magnitude * Time.deltaTime;
        else
            calc_forces();

        if (onGround && (actualVelocityForward.magnitude > 0.3))
        {
            if (ySpeed >= 0)
                velocity.y = ySpeed;// - 20.81f * Time.deltaTime * Time.deltaTime;// - 9.81f * Time.deltaTime * Time.deltaTime;// * jumpFactor;
        }

        bool rightHandleGrabbed = rightHandle.GetComponent<handleBarGrab>().barGripped;
        bool leftHandleGrabbed = leftHandle.GetComponent<handleBarGrab>().barGripped;

        if (velocity.magnitude > 0.002)
        {
            if(rightHandleGrabbed)
                vibration.Execute(0, .01f, velocityForward.magnitude / 0.60f * 30, velocityForward.magnitude / 0.60f * 0.3f, handRight);
            if(leftHandleGrabbed)
                vibration.Execute(0, .01f, velocityForward.magnitude / 0.60f * 30, velocityForward.magnitude / 0.60f * 0.3f, handLeft);
            Vector3 lookDirection = (Vector3.Dot(controller.transform.forward, velocityForward) < 0) ? -velocityForward : velocityForward;
            controller.transform.forward = Vector3.RotateTowards(controller.transform.forward, lookDirection, 1.0f, 1);
            controller.Move(velocity);
        }
        speedmeter.text = Mathf.RoundToInt(velocityForward.magnitude/Time.deltaTime * 2.23694f) + " mph";
    }

    public void resetGame()
    {
        controller.enabled = false;
        controller.transform.position = startpos;
        controller.enabled = true;
        velocity = Vector3.zero;
        lastYPosition = startpos.y;
        controller.Move(velocity);
    }

    void calc_forces()
    {
        Vector3 forward = controller.transform.forward;

        Vector3 velocityForward = velocity;
        velocityForward.y = 0;

        velmag = Vector3.Magnitude(velocityForward);

        Vector3 movedirection = Vector3.Normalize(controller.velocity);
        if (velocityForward.magnitude < 0.002f)
        {
            movedirection = Vector3.zero;
            velmag = 0;
        }

        //Gravity
        Vector3 FG = new Vector3(0, -9.81f * Time.deltaTime, 0);
        //Vector3 FG = new Vector3(0, -9.81f, 0);

        //Drag
        Vector3 FD = -movedirection * velmag * velmag * dragFactor;

        bool rightHandleGrabbed = rightHandle.GetComponent<handleBarGrab>().barGripped;
        bool leftHandleGrabbed = leftHandle.GetComponent<handleBarGrab>().barGripped;

        //Brakes
        Vector3 FB = Vector3.zero;
        if(rightHandleGrabbed || ((controllerPoseRight.transform.position - rightHandle.transform.position).magnitude < 0.2f))
            FB = -movedirection * brake;

        //Forward or reverse
        Vector3 Ff;
        bool leftTriggerPulled = triggerPull.GetState(handLeft);
        if(leftTriggerPulled && (leftHandleGrabbed || ((controllerPoseLeft.transform.position - leftHandle.transform.position).magnitude < 0.2f)))
        {
            Ff = -forward * throttle * horsepower;
        }
        else
        {
            Ff = forward * throttle * horsepower;
        }

        //Leaning
        leanRotation = Vector3.Dot(Camera.main.transform.position - controller.transform.position, controller.transform.right);
        Vector3 FL = turnspeed * velmag * leanRotation * controller.transform.right;

        //Handlebar turning
        Vector3 FT;
        if (rightHandleGrabbed || leftHandleGrabbed)
        {
            Vector3 leftPoint = leftHandleGrabbed ? controllerPoseLeft.transform.position : leftHandle.transform.position;
            Vector3 rightPoint = rightHandleGrabbed ? controllerPoseRight.transform.position : rightHandle.transform.position;

            Vector3 lhs = Vector3.Normalize(leftPoint - rightPoint);
            Vector3 groundlevelleft = leftPoint;
            groundlevelleft.y = 0;

            Vector3 rhs = Vector3.Normalize(groundlevelleft - rightPoint);

            Vector3 barforward = Vector3.Normalize(Vector3.Cross(lhs, rhs));

            handleRotation = Vector3.Dot(controller.transform.right, barforward);
            FT = turnspeed * velmag * handleRotation * controller.transform.right;
        }
        else
        {
            handleRotation -= handleRotation * velmag * freeHandleRotationFactor;
            FT = turnspeed * velmag * handleRotation * controller.transform.right;
        }

        if (onGround)
            velocity += (FD + Ff + FB + FT + FG + FL) * Time.deltaTime;
        else
            velocity += (FD * .2f + FG) * Time.deltaTime;
    }

    public float GetBrake()
    {
        return brakePull.GetAxis(handRight);
    }
}
