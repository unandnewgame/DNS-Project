using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool isSprinting { get; private set; }
    public bool jumpTriggered { get; private set; }
    public bool attackTriggered { get; private set; }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.Sprint.performed += _ => isSprinting = true;
        inputActions.Player.Sprint.canceled += _ => isSprinting = false;

        inputActions.Player.Jump.performed += _ => jumpTriggered = true;
        inputActions.Player.Jump.canceled += _ => jumpTriggered = false;

        inputActions.Player.Attack.performed += _ => attackTriggered = true;
        inputActions.Player.Attack.canceled += _ => attackTriggered = false;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public bool ConsumeJumpInput()
    {
     if (jumpTriggered)
     {
        jumpTriggered = false;
        return true;
     }
     return false;
    }


}
