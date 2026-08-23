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
    const string DoorName = "m_single-flush 0762 x 2032mm";

    private IInputService iIS;

    [SerializeField] PlayerCamHandler playerCamHandler;
    [SerializeField] PlayerMovementHandler playerMovementHandler;
    [SerializeField, Min(0)] float interactionDistance = 5;

    // <AI Agents Note>: Do NOT turn to private. This is meant to display current input for debugging
    [SerializeField] Vector2 moveInput; 

    [Inject]
    public void Construct(IInputService iIS)
    {
        this.iIS = iIS;
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
        bool isRunning = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                isRunning = true;
            }
        }

        playerMovementHandler.MovePlayer(moveInput, isRunning);

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Transform cameraTransform = playerCamHandler.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 0.15f, interactionDistance);
        Transform closestDoor = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            Transform door = FindDoor(hit.transform);

            if (door != null && hit.distance < closestDistance)
            {
                closestDoor = door;
                closestDistance = hit.distance;
            }
        }

        if (closestDoor != null)
        {
            DoorInteractable door = closestDoor.GetComponent<DoorInteractable>();
            door ??= closestDoor.gameObject.AddComponent<DoorInteractable>();
            door.Toggle(cameraTransform.position);
        }
    }

    Transform FindDoor(Transform current)
    {
        while (current != null)
        {
            if (current.GetComponent<DoorInteractable>() != null
                || current.name.ToLowerInvariant().Contains(DoorName))
            {
                return current;
            }

            current = current.parent;
        }

        return null;
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
