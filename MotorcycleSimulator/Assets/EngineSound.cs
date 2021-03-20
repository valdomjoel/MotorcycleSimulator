using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    public AudioSource mid_sound_on;
    public AudioSource mid_sound_off;
    public float pitchModifier = 1.0f;
    public float onVolumeChangeFactor = 2.0f;
    public float onPitchFactor = 0.9f;
    public float offPitchFactor = 0.9f;
    float minPitch = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        mid_sound_on.volume = 0;
        mid_sound_off.volume = 1;
    }

    // Update is called once per frame
    void Update()
    {
        float velocity = ControllerMove.velocity.magnitude;
        float gearPitchDiff = 1;

        if (velocity > 0.2f)
            gearPitchDiff = 1.2f;

        if (velocity > 0.33f)
            gearPitchDiff = 1.4f;

        if (velocity > 0.4f)
            gearPitchDiff = 1.6f;

        if (velocity > 0.45f)
            gearPitchDiff = 1.8f;

        Vector3 velocityForward = ControllerMove.velocity;
        velocityForward.y = 0;
        float throttle = ControllerMove.throttle;
        float velocityPitch = (velocityForward.magnitude * 10 / gearPitchDiff) * pitchModifier;
        mid_sound_off.volume = 1 - throttle;
        mid_sound_on.volume = throttle * onVolumeChangeFactor;
        mid_sound_on.pitch = minPitch + velocityPitch * onPitchFactor;
        mid_sound_off.pitch = minPitch + velocityPitch * offPitchFactor;

    }
}
