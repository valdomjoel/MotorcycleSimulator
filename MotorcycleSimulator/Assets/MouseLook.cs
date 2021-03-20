using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public GameObject player;
    float x_mouse=0.0f, y_mouse=0.0f;
    float x_keyboard = 0.0f, y_keyboard = 0.0f;
    Vector3 startCameraAngle;
    Vector3 startBodyAngle;
    public float sensitivity = 100.0f;
    public float keyboard_sensitivity = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        startCameraAngle = transform.localEulerAngles;
        startBodyAngle = player.transform.localEulerAngles;
    }


    // Update is called once per frame
    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        float kx = Input.GetAxis("Vertical");
        float ky = Input.GetAxis("Horizontal");

        x_mouse += mx * sensitivity * Time.deltaTime;
        y_mouse -= my * sensitivity * Time.deltaTime;
        x_keyboard += kx * keyboard_sensitivity * Time.deltaTime;
        y_keyboard += ky * keyboard_sensitivity * Time.deltaTime;

        y_mouse = Mathf.Clamp(y_mouse, -90, 90);


        transform.localEulerAngles = startCameraAngle + new Vector3(y_mouse, x_mouse, 0.0f);
        player.transform.localEulerAngles = startBodyAngle + new Vector3(x_keyboard, y_keyboard, 0.0f);
    }

    public void testFunction()
    {
        Debug.Log("Test Function");
    }
}
