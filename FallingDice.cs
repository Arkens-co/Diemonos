using System;
using UnityEngine;

public class FallingDice : MonoBehaviour
	{
    public Rigidbody rb;
    private float thrust;
    private float thrust2;
    private float thrust3;

    public Vector3 MaxValues;
    public Vector3 MinValues;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thrust = UnityEngine.Random.Range(1, 100);
        thrust2 = UnityEngine.Random.Range(1, 100);
        thrust3 = UnityEngine.Random.Range(1, 100);
        rb.AddTorque(new Vector3(thrust, thrust2, thrust3));
    }
    void OnTriggerEnter()
    {
            transform.position = new Vector3(UnityEngine.Random.Range(MinValues.x, MaxValues.x), UnityEngine.Random.Range(MinValues.y, MaxValues.y), UnityEngine.Random.Range(MinValues.z, MaxValues.z));
            //rb.AddRelativeForce(Vector3.up * thrust);
            rb.AddTorque(new Vector3(thrust, thrust2, thrust3));
            thrust = UnityEngine.Random.Range(1, 100);
            thrust2 = UnityEngine.Random.Range(1, 100);
            thrust3 = UnityEngine.Random.Range(1, 100);
    }
	}

