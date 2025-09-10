using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TWeepingAngel : MonoBehaviour
{
    Camera playerCam;
    float angleFOV = 0f;
    [SerializeField] float inaccuracy = 0f;
    [SerializeField] AudioSource sexyback;

    void Start()
    {
        playerCam = FindObjectOfType<Camera>();
    }

    void Update()
    {
        //Vector3 direction = ()
        angleFOV = playerCam.fieldOfView - inaccuracy;
        float currentAngle = Vector3.Angle(playerCam.transform.forward, this.transform.position);
        if (currentAngle < angleFOV)
        {
            if (!sexyback.isPlaying)
            {
                sexyback.Play();
            }
            //print("вас заметили");
        }
        else
        {
            sexyback.Pause();
        }
    }
}
