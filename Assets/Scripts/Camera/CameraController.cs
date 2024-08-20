using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//https://gamedevbeginner.com/input-in-unity-made-easy-complete-guide-to-the-new-system/

//This script needs to be attached to an empty GameObject which acts as a camera rig and 
//***HAS A CAMERA CHILD OBJECT***
//The child objects' moves on the rigs y-axis. This makes a zoom effect
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [Header("Movement Settings")]
    [SerializeField]
    [Range(1,10000)]
    private float cameraSpeed;
    [Tooltip("The first position is the minimum height the second the maximum height")]
    [SerializeField]
    private float[] cameraHeightBorders = new float[2];
    [SerializeField]
    private float movementLerp;
    private Vector3 newPosition;

    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationSpeed;
    private Quaternion newRotation;

    [Header("Zoom Settings")]
    [SerializeField]
    private float zoomAmmount;
    [SerializeField]
    private int scrollZoomDamper;
    [SerializeField]
    private float power = 0.25f;
    [Tooltip("The first position is the minimum zoom the second the maximum zoom. The first value must be greater than 0")]
    [SerializeField]
    private float[] zoomBorders = new float[2];
    private Vector3 newZoom;
    [Tooltip("The first position is the minimum rotation the second the maximum rotation")]
    [SerializeField]
    private float[] zoomRotationBorders = new float[2];
    private Quaternion newCamRotation;


    private Vector3 rotateStart;
    private Vector3 rotateCurrent;

    private Vector2 dirInput;
    private float zoom;
    private float rotation;
    private bool mouseRotationTriggeredThisFrame;
    private bool mouseRotation;

    // Start is called before the first frame update
    private void Awake()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;
        //Sets the camera height between the camera height borders
        if (transform.position.y < cameraHeightBorders[0]) transform.position = new Vector3(transform.position.x, cameraHeightBorders[0], transform.position.z);
        if (transform.position.y > cameraHeightBorders[1]) transform.position = new Vector3(transform.position.x, cameraHeightBorders[1], transform.position.z);

        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        newCamRotation = cameraTransform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        Rotate();
        Zoom();
    }

    public void GetMoveDirection(InputAction.CallbackContext ctx)
    {
        dirInput = ctx.ReadValue<Vector2>();
    }

    public void GetRotation(InputAction.CallbackContext ctx)
    {
        rotation = ctx.ReadValue<float>();
    }

    public void GetZoom(InputAction.CallbackContext ctx)
    {
        zoom = ctx.ReadValue<float>();
    }

    //NOT WORKING
    public void GetToggleMouseRotationButton(InputAction.CallbackContext ctx)
    {
        mouseRotation = ctx.action.WasPressedThisFrame();
        if (ctx.started && ctx.action.WasReleasedThisFrame())
        {
            mouseRotationTriggeredThisFrame = !mouseRotationTriggeredThisFrame; 
        }
    }

    void Move()
    {
        //Calculates the movementSpeed based on the zoomAmmount
        float zoomModifier = newZoom.y;
        float realCameraSpeed = Mathf.Sqrt(Mathf.Clamp(zoomModifier, 1, float.MaxValue / cameraSpeed) * cameraSpeed) / 100;
        
        newPosition += transform.TransformDirection(new Vector3(dirInput.x, 0, dirInput.y)) * realCameraSpeed;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementLerp);
    }

    void Rotate()
    {
        newRotation *= Quaternion.Euler(transform.up * rotationSpeed * rotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementLerp * 2);

        //Mouse Input
        //TODO: Change to InputSystem Inputs
        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            rotateStart = Mouse.current.position.ReadValue();
        }
        if (Mouse.current.middleButton.isPressed)
        {
            rotateCurrent = Mouse.current.position.ReadValue();
            Vector3 difference = rotateStart - rotateCurrent;
            rotateStart = rotateCurrent; //resets start position

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 2f));
        }
    }

    void Zoom()
    {
        newZoom += new Vector3(0, zoomAmmount * -zoom, 0);
        newZoom = new Vector3(newZoom.x, Mathf.Clamp(newZoom.y, zoomBorders[0], zoomBorders[1]), newZoom.z);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementLerp);

        //ZOOM ROTATION
        //Based on how far the camera is away from the minimum zoom discance the angle becomes flatter
        newCamRotation = Quaternion.Euler(Mathf.Clamp(Mathf.Pow(newZoom.y / zoomBorders[1], power) * (zoomRotationBorders[1] - zoomRotationBorders[0]) + zoomRotationBorders[0], zoomRotationBorders[0], zoomRotationBorders[1]), cameraTransform.rotation.y, cameraTransform.localRotation.z);
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, newCamRotation, Time.deltaTime * movementLerp);
    }


}
