using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFacingCamera : MonoBehaviour
{
    private void Update()
    {
        var cam = Camera.main;
        transform.LookAt(cam.transform);
    }
}
