using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DELETEME : MonoBehaviour
{
    [SerializeField] GameObject ui;
    [SerializeField] float moveSpeed = 0.6f;

    void Start()
    {
        ui.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        transform.position += -transform.right * moveSpeed * Time.unscaledDeltaTime;
    }
}
