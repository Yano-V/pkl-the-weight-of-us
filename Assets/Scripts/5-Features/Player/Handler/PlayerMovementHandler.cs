using UnityEngine;
using UnityEngine.InputSystem; // Added for Input System integration
using VContainer;

// ================================================================================= //
//                                                                                   //
// Class Name: Player Movement Handler                                               //
// Description: Moves the player relative to the active camera using walk or run     //
//              speed based on the player's movement input.                         //
// Author: SenGouku                                                                  //
// Co-Author: PsychoLucidia                                                          //
//                                                                                   //
// ================================================================================= //

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementHandler : MonoBehaviour
{
    private ICameraService iCS;
    private CharacterController characterController;

    [SerializeField, Min(0)] float moveSpeed = 5;
    [SerializeField, Min(0)] float runSpeed = 10;
    [SerializeField, Min(0)] float gravity = 20;
    [Header("Character Collision")]
    [SerializeField, Min(0.01f)] float controllerHeight = 1.8f;
    [SerializeField, Min(0.01f)] float controllerRadius = 0.3f;
    [SerializeField, Min(0)] float stairStepHeight = 0.45f;
    [SerializeField, Range(0, 90)] float slopeLimit = 50;


    private float verticalVelocity;
    // Cached input states for continuous updates
    private Vector2 currentInput;
    private bool isRunning;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // Existing scene instances predate the RequireComponent attribute, so make
        // sure they also receive a controller when the scene starts.
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }

        characterController.height = controllerHeight;
        characterController.radius = controllerRadius;
        characterController.center = Vector3.zero;
        characterController.stepOffset = Mathf.Min(stairStepHeight, controllerHeight);
        characterController.slopeLimit = slopeLimit;
        characterController.skinWidth = 0.05f;
        characterController.minMoveDistance = 0;
    }

    [Inject]
    public void Construct(ICameraService iCS)
    {
        this.iCS = iCS;
    }

    // Call  method from Input System (OnMove and OnSprint events)
    public void OnMove(InputAction.CallbackContext context)
    {
        currentInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }


    // Modified by Yano: Added Update() to continuously feed inputs into MovePlayer() 
    // Ensures gravity ticks every frame so the player doesn't float in mid-air when keys are released.
    void Update()
    {
        MovePlayer(currentInput, isRunning);
    }

    // Modified by Yano: Added MovePlayer() to handle player movement based on input and camera orientation
    // Centralizes movement logic and allows for continuous updates based on input states.
    public void MovePlayer(Vector2 value, bool isRunning)
    {
        Vector3 movement = Vector3.zero;

        if (value.magnitude >= 0.1f)
        {
            Vector2 input = Vector2.ClampMagnitude(value, 1f);

            // Fallback to Main Camera if VContainer service isn't loaded in the test scene
            float camYaw = (iCS != null && iCS.CurrentActiveCam != null)
                ? iCS.CurrentActiveCam.CamRotationEulerAngles.y
                : Camera.main.transform.eulerAngles.y;

            Quaternion yawRotation = Quaternion.Euler(0, camYaw, 0);
            Vector3 inputDirection = new Vector3(input.x, 0, input.y);
            Vector3 moveDirection = yawRotation * inputDirection;

            float currentSpeed = isRunning ? runSpeed : moveSpeed;
            movement = moveDirection * currentSpeed;
        }

        if (characterController.isGrounded && verticalVelocity < 0)
        {
            // Keep a small downward force so the controller follows descending stairs.
            verticalVelocity = -2;
        }

        verticalVelocity -= gravity * Time.deltaTime;
        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }
}