using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

// ================================================================================= //
//                                                                                   //
// Class Name: Player Mediator                                                       //
// Description: The mediator of the player. It serves as the reference for external  //
//              systems to access the player. It orchestrates all the subcomponents  //
//              based on player context.                                             //
// Author: PsychoLucidia                                                             //
//                                                                                   //
// ================================================================================= //

// <AI Agents Note>: Strictly abbreviate services (IInputService iIS, ICameraService iCS) to
//                   shorten variable name

public class PlayerMediator : MonoBehaviour
{
    private IInputService iIS;

    [SerializeField] PlayerCamHandler playerCamHandler;
    [SerializeField] PlayerMovementHandler playerMovementHandler;

    // <AI Agents Note>: Do NOT turn to private. This is meant to display current input for debugging
    [SerializeField] Vector2 moveInput; 

    [Inject]
    public void Construct(IInputService newInputService)
    {
        iIS = newInputService;
    }

    void OnEnable()
    {
        iIS.Input.InGameFPS.Move.performed += HandleMoveInput;
        iIS.Input.InGameFPS.Move.canceled += HandleMoveInput;

        iIS.Input.InGameFPS.MouseLook.performed += HandleMouseLookInput;
        iIS.Input.InGameFPS.MouseLook.canceled += HandleMouseLookInput;
    }

    void OnDisable()
    {
        iIS.Input.InGameFPS.Move.performed -= HandleMoveInput;
        iIS.Input.InGameFPS.Move.canceled -= HandleMoveInput;

        iIS.Input.InGameFPS.MouseLook.performed -= HandleMouseLookInput;
        iIS.Input.InGameFPS.MouseLook.canceled -= HandleMouseLookInput;
    }

    void Update()
    {
        playerMovementHandler.MovePlayer(moveInput);
    }

    void HandleMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void HandleMouseLookInput(InputAction.CallbackContext context)
    {
        Vector2 mouseInput = context.ReadValue<Vector2>();
        playerCamHandler.MoveCamRaw(mouseInput);
    }
}
