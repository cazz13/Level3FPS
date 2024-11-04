using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    // Components
    private CharacterController characterController;
    private Transform cameraTransform;

    // Movement and jump configuration parameters
    [SerializeField] private float speed = 5f;
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float gravity = Physics.gravity.y;

    // Input fields for movement and look actions
    private Vector2 moveInput;
    private Vector2 lookInput;

    // Velocity and rotation variables
    private Vector2 velocity;
    private float verticalVelocity;
    private float verticalRotation = 0;

    // Sprint stamina parameters
    [SerializeField] private float maxStamina = 100f; // Max stamina value
    [SerializeField] private float sprintStaminaCost = 50f; // Stamina cost per second while sprinting
    [SerializeField] private float staminaRegenRate = 5f; // Stamina regenerated per second
    private float currentStamina;
    private bool isSprinting;

    // UI Slider for stamina
    [SerializeField] private Slider staminaSlider; // Referencia al slider de estamina

    // Camera look sensitivity and max angle to limit vertical rotation
    private float lookSentitivity = 1f;
    private float maxLookAngle = 80f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        // Initialize stamina to max
        currentStamina = maxStamina;

        // Initialize the stamina slider
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Manage Player Movement
        MovePlayer();

        // Manage Camera Rotation
        LookAroung();

        // Regenerate stamina when not sprinting
        RegenerateStamina();

        // Update the stamina slider
        UpdateStaminaSlider();
    }

    /// <summary>
    /// Receives movement input from Input System
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log(moveInput);
    }

    /// <summary>
    /// Receives Look input from Input System
    /// </summary>
    /// <param name="context"></param>
    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Receives jump input from Input System
    /// </summary>
    /// <param name="context"></param>
    public void Jump(InputAction.CallbackContext context)
    {
        // If player is touching ground
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    /// <summary>
    /// Receives sprint input from Input System
    /// </summary>
    /// <param name="context"></param>
    public void Sprint(InputAction.CallbackContext context)
    {
        // Check if action is started or maintained and if there's enough stamina
        isSprinting = (context.started || context.performed) && currentStamina > 0;
    }

    private void MovePlayer()
    {
        // Jump
        if (characterController.isGrounded)
        {
            // Reset vertical velocity when touching the ground
            verticalVelocity = 0f;
        }
        else
        {
            // Apply gravity
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = new Vector3(0, verticalVelocity, 0);
        characterController.Move(move * Time.deltaTime);

        // Movement
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);

        // Adjust speed based on sprinting and stamina
        float targetSpeed = isSprinting && currentStamina > 0 ? speed * multiplier : speed;
        characterController.Move(moveDirection * targetSpeed * Time.deltaTime);

        // Apply gravity constantly
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Drain stamina if sprinting
        if (isSprinting && currentStamina > 0)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    /// <summary>
    /// Regenerates stamina when player is not sprinting
    /// </summary>
    private void RegenerateStamina()
    {
        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    /// <summary>
    /// Updates the stamina slider based on the current stamina
    /// </summary>
    private void UpdateStaminaSlider()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }

    private void LookAroung()
    {
        // Horizontal rotation (Y-axis) based on sensitivity and input
        float horizontalRotation = lookInput.x * lookSentitivity;
        transform.Rotate(Vector3.up * horizontalRotation);

        // Vertical rotation (X-axis) with clamping to prevent over-rotation
        verticalRotation -= lookInput.y * lookSentitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
