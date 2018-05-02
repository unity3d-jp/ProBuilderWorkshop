using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Pendulum : MonoBehaviour
{
    public float speed = 0.5f;
    public bool reverse = false;
    public float initPassedTime = 0;
    public float impactForce = 5;

    private float motionDegree = 180;
    private float currentDegree;
    private float diffDegree;
    private Vector3 leftPoint = new Vector3(-5f, -7.5f, 0);
    private Vector3 rightPoint = new Vector3(5f, -7.5f, 0);

    private void OnValidate()
    {
        SetPendulamMotion(initPassedTime);
    }

    private void Start()
    {
        currentDegree = transform.eulerAngles.z;
    }

    private void Update()
    {
        float time = Time.time + initPassedTime;
        SetPendulamMotion(time);

        diffDegree = Mathf.DeltaAngle(currentDegree, transform.eulerAngles.z);
        currentDegree = transform.eulerAngles.z;

        if (diffDegree > 0)
        {
            GetComponent<SphereCollider>().center = rightPoint;
        }
        else
        {
            GetComponent<SphereCollider>().center = leftPoint;
        }
    }

    public void SetPendulamMotion(float time)
    {
        float t = reverse ? -time : time;
        float rotationZ = Mathf.Sin(t * Mathf.Deg2Rad * motionDegree * speed) * (motionDegree / 2);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotationZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager manager = GameObject.FindObjectOfType<RespawnManager>();
            manager.SetPlayerPosition(other.transform);
        }
    }
}
