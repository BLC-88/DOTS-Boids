using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] GameObject UI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            UI.SetActive(!UI.activeSelf);
        }
    }
}
