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

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var v = Input.GetAxis("Vertical");
        if (v == 0) v = Joy.Vertical;
        var h = Input.GetAxis("Horizontal");
        if (h == 0) h = Joy.Horizontal;

        var laterPos = GetNextPos(v);
        var laterRotation = GetNextRotation(h);

        rb.MovePosition(laterPos);
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
        var distance = v * Player.forward * moveVel * 1/60f;
        var laterPos = Player.position + distance;
        return laterPos;
    }
}
