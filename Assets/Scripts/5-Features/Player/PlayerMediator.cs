using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class PlayerMediator : MonoBehaviour
{
    private IInputService inputService;

    [SerializeField] PlayerCamHandler playerCamHandler;
    private Vector2 moveInput;

    [Inject]
    public void Construct(IInputService newInputService)
    {
        inputService = newInputService;
    }

    void OnEnable()
    {
        inputService.Input.InGameFPS.Move.performed += GetMoveInput;
        inputService.Input.InGameFPS.Move.canceled += GetMoveInput;

        inputService.Input.InGameFPS.MouseLook.performed += GetMouseInput;
        inputService.Input.InGameFPS.MouseLook.canceled += GetMouseInput;
    }

    void OnDisable()
    {
        inputService.Input.InGameFPS.Move.performed -= GetMoveInput;
        inputService.Input.InGameFPS.Move.canceled -= GetMoveInput;

        inputService.Input.InGameFPS.MouseLook.performed -= GetMouseInput;
        inputService.Input.InGameFPS.MouseLook.canceled -= GetMouseInput;
    }

    void Update()
    {
        playerCamHandler.MoveCam(moveInput);
    }

    void GetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void GetMouseInput(InputAction.CallbackContext context)
    {
        Vector2 mouseInput = context.ReadValue<Vector2>();
        playerCamHandler.MoveCamRaw(mouseInput);
    }
}
