using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class rMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerInput input;
    [SerializeField] Transform camPivot;
    [SerializeField] Transform body;
    [SerializeField] Camera cam;
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
        _move = contextMove.ReadValue<Vector2>();
        //print($"[MOVE] {contextMove}");
    }

    public void OnLook(InputAction.CallbackContext contextLook)
    {
        _moveMouse = contextLook.ReadValue<Vector2>();
        print($"[LOOK] {contextLook}");
    }

    public void OnJump(InputAction.CallbackContext contextJump)
    {
        var _jump = contextJump.ReadValueAsButton();
        //print($"[JUMP] {contextJump}");

        //добавить проверку на состояние
        //как делать стейты
        /*
        if (contextJump.performed)
        {
            print("double jump");
        }
        else if(contextJump.canceled)
        {
            print("jump");
        }
        */
    }

    public void OnSlideSlam(InputAction.CallbackContext contextSl)
    {
        //var _ss = contextSl.ReadValueAsButton();
        print($"[SLSL] {contextSl}");
    }

    public void OnDEBUG(InputAction.CallbackContext contextDeb)
    {
        var _deb = contextDeb.ReadValueAsButton();
        print($"[DEBUG] {contextDeb}");
    }


    void Start()
    {
        
    }

    void Update()
    {
        rb.AddRelativeForce(new Vector3(_move.x, 0, _move.y) * multiplierMove);
    }
}
