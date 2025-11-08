using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class TEWeepingAngel : MonoBehaviour
{
    Camera playerCam;
    float angleFOV = 0f;
    [SerializeField] float inaccuracy = 0f;
    [SerializeField] AudioSource sexyback;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider collider;
    

    void Start()
    {
        playerCam = FindObjectOfType<Camera>();
        //agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Vector3 direction = (transform.position - playerCam.transform.position);
        angleFOV = playerCam.fieldOfView - inaccuracy;
        float currentAngle = Vector3.Angle(playerCam.transform.forward, direction);
        if (currentAngle < angleFOV)
        {
            if (!sexyback.isPlaying)
            {
                sexyback.Play();
            }
        }
        else
        {
            sexyback.Pause();
        }
        agent.SetDestination(playerCam.transform.position);
        agent.autoRepath = true;
        agent.autoBraking = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        print("вас заметили");
        Destroy(collision.gameObject);
    }
}
