using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

public class TMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerInput input;
    [SerializeField] Transform cumPivo;
    [SerializeField] Transform body;
    [SerializeField] Camera cum;
    //[SerializeField] GameObject bodyThird;
    [SerializeField] Animator animator;
    //[SerializeField] GameObject bodyFirst;

    [SerializeField] float sens = 2f;
    [SerializeField] float multiplierSlam = 3f;
    [SerializeField] float multiplierMove = 50f;
    [SerializeField] float JM = 50f;
    [SerializeField] float SM = 50f;
    [Header("Причина тряски?")]
    public bool jiggle = false;
    [SerializeField] float juggleForce = 2f;

    bool isGamepadLastDevice;
    float jiggling = 0;
    Vector3 _move;
    Vector2 _moveMouse;
    bool isMovementPressed = false;

    Vector3 thirdPerson = new Vector3(0.6f, 0.8f, -1.5f);

    float camY;
    float camX;

    public void OnMove(InputAction.CallbackContext contextMove)
    {
        _move = contextMove.ReadValue<Vector3>();
    }

    public void OnLook(InputAction.CallbackContext contextLook)
    {
        _moveMouse = contextLook.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext contextJump)
    {
        var _jump = contextJump.ReadValueAsButton();
    }

    public void OnSlideSlam(InputAction.CallbackContext contextSS)
    {
        var _ss = contextSS.ReadValueAsButton();
    }

    void Start()
    {
        if (input.currentControlScheme == "Gamepad")
        {
            isGamepadLastDevice = true;
        }
        else if (input.currentControlScheme == "Keyboard and Mouse")
        {
            isGamepadLastDevice = false;
        }
    }

    void Update()
    {
        CameraRotating(_moveMouse);

        if (jiggle)
        {
            jiggling = Random.Range(0f, juggleForce);
        }
        else
        {
            jiggling = 0;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * 2, ForceMode.Acceleration);
        if (isMovementPressed)
        {
            Movement(_move);
        }
        //VerticalMove(_move);
    }

    void Movement(Vector3 direction)
    {
        //print(direction);
        rb.AddRelativeForce((new Vector3(cumPivo.forward.x, 0f, cumPivo.forward.z) * direction.z + cumPivo.right * direction.x) * multiplierMove);
    }

    void CameraRotating(Vector2 moveMouse)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 currentCamTrans = cumPivo.transform.rotation.eulerAngles;
        
        if (input.currentControlScheme == "Gamepad")
        {
            if (!isGamepadLastDevice)
            {
                animator.SetTrigger("Gamepad");
            }
            cum.transform.localPosition = thirdPerson;
            cumPivo.transform.localPosition = Vector3.zero;
            camY = currentCamTrans.x + -moveMouse.y * Time.deltaTime * sens * 100;
            camX = currentCamTrans.y + moveMouse.x * Time.deltaTime * sens * 100;
            isGamepadLastDevice = true;
        }
        else if(input.currentControlScheme == "Keyboard and Mouse")
        {
            if (isGamepadLastDevice)
            {
                animator.SetTrigger("Keymouse");
            }
            cum.transform.localPosition = Vector3.zero;
            cumPivo.transform.localPosition = new Vector3(0, 0.5f, 0);
            camY = currentCamTrans.x + -moveMouse.y * Time.deltaTime * sens * 3;
            camX = currentCamTrans.y + moveMouse.x * Time.deltaTime * sens * 3;
            isGamepadLastDevice = false;
        }
        cumPivo.transform.rotation = Quaternion.Euler(camY, camX, jiggling);
    }
    
    /*public void VerticalMove(Vector3 moveJump)
    {
        Vector3 currentVelocity = rb.velocity;
        //print(rb.velocity);

        switch (moveJump.y >= 0, Physics.Raycast(rb.transform.position, Vector3.down, 1.5f))
        {
            case (true, true):  //прыжок
                rb.velocity = currentVelocity + new Vector3(0f, moveJump.y * 15 * JM, 0f);
                break;

            case (true, false):
                rb.velocity = currentVelocity + new Vector3(0f, moveJump.y * 15 * JM, 0f);
                break;

            case (false, true): //слайд
                rb.velocity = new Vector3(cumPivo.forward.x, 0f, cumPivo.forward.z) * SM;

                break;

            case (false, false): //слэм
                rb.velocity = new Vector3(0f, (moveJump.y * multiplierSlam), 0f);
                break;
        };
    }
    */
}
