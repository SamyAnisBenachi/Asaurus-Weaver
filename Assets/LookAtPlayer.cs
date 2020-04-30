using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Camera cameraToLook;

    void Start()
    {
        cameraToLook = Camera.main;
    }

    void Update()
    {
        // UI Look at the player
        transform.LookAt(transform.position + cameraToLook.transform.rotation * Vector3.back, cameraToLook.transform.rotation * Vector3.up);
    }
}
