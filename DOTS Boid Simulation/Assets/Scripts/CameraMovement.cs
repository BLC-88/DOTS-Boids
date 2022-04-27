using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 1f;
    float moveSpeedMultiplier = 1f;
    [SerializeField] float moveSpeedMultiplierMin = 0.1f;
    [SerializeField] float moveSpeedMultiplierMax = 10f;
    [SerializeField] Text moveSpeedText;

    [Header("Sensitivity")]
    [SerializeField] float lookSensitivityX = 1f;
    [SerializeField] float lookSensitivityY = 1f;

    [Header("FOV")]
    [SerializeField] float FOVChangeRate = 10f;
    [SerializeField] float minFOV = 10f;
    [SerializeField] float maxFOV = 100f;
    [SerializeField] Text FOVText;

    bool mouseInput;
    Vector3 moveDir;
    Vector3 moveDirVer;
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
        transform.position += moveDir * moveSpeed * moveSpeedMultiplier * Time.unscaledDeltaTime;
        moveDirVer = Vector3.up * Input.GetAxisRaw("Jump");
        transform.position += moveDirVer * moveSpeed * moveSpeedMultiplier * Time.unscaledDeltaTime;

        mouseInput = Input.GetKey(KeyCode.Mouse1);
        scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseInput)
        {
            Cursor.lockState = CursorLockMode.Locked;

            rotationY += Input.GetAxis("Mouse X") * lookSensitivityX;
            rotationX -= Input.GetAxis("Mouse Y") * lookSensitivityY;

            moveSpeedMultiplier += scrollWheel;
            moveSpeedMultiplier = Mathf.Clamp(moveSpeedMultiplier, moveSpeedMultiplierMin, moveSpeedMultiplierMax);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

            transform.position += transform.forward * scrollWheel * moveSpeed * moveSpeedMultiplier;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                float fov = cam.fieldOfView;
                fov += -scrollWheel * 10 * FOVChangeRate;
                fov = Mathf.Clamp(fov, minFOV, maxFOV);
                cam.fieldOfView = fov;
            }
        }
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        moveSpeedText.text = "SPD: " + moveSpeedMultiplier.ToString("f1") + "x";
        FOVText.text = "FOV: " + cam.fieldOfView;
    }
}
