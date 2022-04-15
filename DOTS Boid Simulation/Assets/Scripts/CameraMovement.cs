using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 1f;
    float moveSpeedMultiplier = 1f;
    [SerializeField] float moveSpeedMultiplierMin = 0.1f;
    [SerializeField] float moveSpeedMultiplierMax = 10f;

    [Header("Sensitivity")]
    [SerializeField] float lookSensitivityX = 1f;
    [SerializeField] float lookSensitivityY = 1f;

    [Header("FOV")]
    [SerializeField] float FOVChangeRate = 10f;
    [SerializeField] float minFOV = 10f;
    [SerializeField] float maxFOV = 100f;

    bool mouseInput;
    Vector3 moveDir;
    float rotationX, rotationY;
    float scrollWheel;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        rotationX = transform.localEulerAngles.x;
        rotationY = transform.localEulerAngles.y;
    }

    void Update()
    {
        moveDir = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal")).normalized;
        transform.position += moveDir * moveSpeed * moveSpeedMultiplier * Time.deltaTime;
        transform.position += Vector3.up * Input.GetAxis("Jump") * moveSpeed * moveSpeedMultiplier * Time.deltaTime;

        mouseInput = Input.GetKey(KeyCode.Mouse1);
        scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseInput)
        {
            Cursor.lockState = CursorLockMode.Locked;

            rotationY += Input.GetAxis("Mouse X") * lookSensitivityX * Time.timeScale;
            rotationX -= Input.GetAxis("Mouse Y") * lookSensitivityY * Time.timeScale;

            moveSpeedMultiplier += scrollWheel;
            moveSpeedMultiplier = Mathf.Clamp(moveSpeedMultiplier, moveSpeedMultiplierMin, moveSpeedMultiplierMax);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

            float fov = cam.fieldOfView;
            fov += -scrollWheel * FOVChangeRate;
            fov = Mathf.Clamp(fov, minFOV, maxFOV);
            cam.fieldOfView = fov;
        }
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }
}
