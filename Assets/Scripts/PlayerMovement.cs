using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Components
    private CharacterController characterController;
    private Transform cameraTransform;


    //movement and jump configuration parameters
    [SerializeField] private float speed = 5f;
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float gravity = Physics.gravity.y;


    //Input fields for movement and look actions
    private Vector2 moveInput;
    private Vector2 lookInput;

    //Velocity and rotation variables
    private Vector2 velocity;
    private float verticalVelocity;
    private float verticalRotation = 0;

    //Is Sprinting state
    private bool isSprinting;

    //Camera look sensitivity and max angle to limit vertical rotation
    private float lookSentitivity = 1f;
    private float maxLookAngle = 80f;


    //Stamina
    public Image stamina;

    public float staminaTotal, maxStamina;

    public float sprintCost;

    public float chargeRate;

    private Coroutine recharge;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Manage Player Movement
        MovePlayer();
        // Manage Camera Rotation

        LookAroung();

        Debug.Log(staminaTotal);
        if (isSprinting )
        {
            staminaTotal -= sprintCost * Time.deltaTime;

            if (staminaTotal < 0) staminaTotal = 0;

            stamina.fillAmount = staminaTotal / maxStamina;
        }

        if (recharge != null) StopCoroutine(recharge);
        recharge = StartCoroutine(RechargeStamina());
            
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
        //if Player is touching ground
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        //when action starte or mantained
        isSprinting = context.started || context.performed;

 

    }

    private void MovePlayer()
    {
        //Jump
        if (characterController.isGrounded)
        {
            //restart vertical velocity when touch ground
            verticalVelocity = 0f;
        }
        else
        {
            // when is falling down increment velocity with gravity and time
            verticalVelocity += gravity * Time.deltaTime;

        }

        Vector3 move = new Vector3(0, verticalVelocity,0);

        characterController.Move(move * Time.deltaTime);

        //movement 
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        float targetSpeed = isSprinting ? speed * multiplier : speed;

        characterController.Move(moveDirection * targetSpeed * Time.deltaTime);

        
            

            
       


        //Apply gravity constantly
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);


    }

    private void LookAroung()
    {
        //Horizontal rotation (Y-axis) based on sensitivity and input

        float horizontalRotation = lookInput.x * lookSentitivity;
        transform.Rotate(Vector3.up * horizontalRotation);

        //Vertical rotation (X-axis) with clamping to prevent over-rotation
        verticalRotation -= lookInput.y * lookSentitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (staminaTotal < maxStamina)
        {
            staminaTotal += chargeRate / 10f;
            if (staminaTotal > maxStamina) staminaTotal = maxStamina;
            stamina.fillAmount = staminaTotal / maxStamina;
            yield return new WaitForSeconds(.1f);

        }
    }

}
