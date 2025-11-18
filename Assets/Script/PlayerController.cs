using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;
    public PlayerStats playerStats;

    

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;


    private CharacterController controller;
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
        
    }

    

    
}
