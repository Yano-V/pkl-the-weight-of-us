using UnityEngine;
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

    public void MovePlayer(Vector2 value, bool isRunning)
    {
        Vector3 movement = Vector3.zero;

        if (iCS != null && iCS.CurrentActiveCam != null && value.magnitude >= 0.1f)
        {
            Vector2 input = Vector2.ClampMagnitude(value, 1f);
            Quaternion yawRotation = Quaternion.Euler(0, iCS.CurrentActiveCam.CamRotationEulerAngles.y, 0);
            Vector3 inputDirection = new Vector3(input.x, 0, input.y);
            Vector3 moveDirection = yawRotation * inputDirection;

            float currentSpeed = moveSpeed;

            if (isRunning)
            {
                currentSpeed = runSpeed;
            }

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
