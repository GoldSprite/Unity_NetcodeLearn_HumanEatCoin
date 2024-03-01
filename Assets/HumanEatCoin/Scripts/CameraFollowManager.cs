using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowManager : MonoBehaviour
{
    public Camera Camera => Camera.main;
    public Transform Target => GameManager.Instance.PlayerTrans;
    private bool isInit;
    public Vector3 keepPoint;
    public float keepDistance = 3;

    private void Update()
    {
        if (Target != null)
        {
            if (!isInit) Init();
            Camera.transform.position = Target.position + keepPoint;
        }
    }

    private void Init()
    {
        keepPoint = Target.InverseTransformPoint(Camera.transform.position - Target.position);
        keepDistance = keepPoint.magnitude;
        isInit = true;
    }
}
