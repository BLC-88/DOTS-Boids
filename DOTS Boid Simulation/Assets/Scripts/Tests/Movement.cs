using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float moveSpeed = 1f;
    Vector3 dir = Vector3.forward;

    void Update()
    {
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
