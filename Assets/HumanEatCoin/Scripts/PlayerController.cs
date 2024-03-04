using GoldSprite.TestSyncTemp;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Joystick joy;
    [SerializeField]
    private Joystick Joy { get { if (!joy) joy = FindObjectOfType<Joystick>(); return joy; } }
    private Rigidbody rb;
    private Transform Player => transform;

    [SerializeField]
    private float moveVel = 1;
    [SerializeField]
    private float rotForce = 4;

    private TestSyncPropsHandler syncHandler;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        syncHandler = GetComponent<TestSyncPropsHandler>();
    }

    private void Update()
    {
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        if (Joy != null)
        {
            if (v == 0) v = Joy.Vertical;
            if (h == 0) h = Joy.Horizontal;
        }

        var laterPos = GetNextPos(v);
        var laterRotation = GetNextRotation(h);

        //rb.MovePosition(laterPos);
        syncHandler.UploadMove(GetNextVec(v), rb);
        Player.rotation = laterRotation;
        //·À×²ÐÞÕý
        rb.velocity = rb.angularVelocity = Vector3.zero;
    }

    private Quaternion GetNextRotation(float h)
    {
        var rotate = Quaternion.Euler(0, h * rotForce, 0);
        var rotation = Player.rotation * rotate;
        rotation.x = rotation.z = 0;
        return rotation;
    }

    private Vector3 GetNextPos(float v)
    {
        var distance = GetNextVec(v);
        var laterPos = Player.position + distance;
        return laterPos;
    }

    private Vector3 GetNextVec(float v)
    {
        var distance = v * Player.forward * moveVel * 1/60f;
        return distance;
    }
}
