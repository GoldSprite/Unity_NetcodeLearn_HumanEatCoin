using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotationWindmill : MonoBehaviour
{
    private Rigidbody rb;
    public float rotVel = 10;
    public Vector3 moveVel;
    private System.Random rand = new System.Random();
    [Tooltip("持续时间")]
    public float existsTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DelTask());
    }

    private IEnumerator DelTask()
    {
        yield return new WaitForSeconds(existsTime/2f);

        var materials = GetComponentsInChildren<MeshRenderer>()?.Select(p => p.materials)?.ToList();
        if (materials != null && materials.Count > 0)
        {
            var fadeTime = existsTime/2f;
            var fadeRate = 30f;
            for (int i = 0; i < fadeRate; i++)
            {
                foreach (var ii in materials)
                    foreach (var material in ii)
                    {
                        var c = material.color;
                        c.a -= 1 / fadeRate;
                        material.color = c;
                    }
                yield return new WaitForSeconds(fadeTime / fadeRate);
            }
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        rb.angularVelocity = Vector3.zero;
        transform.rotation *= Quaternion.Euler(0, rotVel * rand.NextDouble() > 0.5f ? 1 : -1 * UnityEngine.Random.Range(0.3f, 1), 0);
        rb.velocity = moveVel;
    }
}
