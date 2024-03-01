using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowManager : MonoBehaviour
{
    public Camera Camera => Camera.main;
    public Transform Target => GameManager.Instance.PlayerTrans;
    private bool isInit;
    public Vector3 keepPoint;
    public Vector3 KeepPoint
    {
        get
        {
            if (!isInit && Target != null)
            {
                keepPoint = Camera.transform.position - Target.position;
                isInit = true;
                Debug.Log("≥ı ºªØCameraKeepPoint.");
            }
            return isInit ? keepPoint : Vector3.zero;
        }
    }

    private void Update()
    {
        if (Target != null)
        {
            Camera.transform.position = Target.position + KeepPoint;
        }
    }
}
