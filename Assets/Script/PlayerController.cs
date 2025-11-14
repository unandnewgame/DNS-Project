using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;
    public PlayerStats playerStats;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;


    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
        cameraTransform = Camera.main.transform;

    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAnimations();
    }


    private void HandleMovement()
    {
        Vector2 input = inputHandler.moveInput;
        Vector3 move = cameraTransform.forward * input.y + cameraTransform.right * input.x;
        move.y = 0f;
        move.Normalize();

        currentSpeed = inputHandler.isSprinting ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Apply gravity
        isGrounded = isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (isGrounded && inputHandler.ConsumeJumpInput())
     {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
     }
    }

    private void HandleAnimations()
    {
        Vector2 input = inputHandler.moveInput;
        bool isMoving = input.magnitude > 0.1f;
        bool isRunning = inputHandler.isSprinting && isMoving;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", !isGrounded);
    }
}
