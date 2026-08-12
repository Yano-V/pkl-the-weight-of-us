using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class PlayerMediator : MonoBehaviour
{
    private IInputService iIS;

    [SerializeField] PlayerCamHandler playerCamHandler;

    [SerializeField] Vector2 currentInput;

    #region Dependency Injection
    [Inject]
    public void Construct(IInputService inputService)
    {
        iIS = inputService;
    }
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        SubInputEvents();
    }

    void OnDisable()
    {
        UnsubInputEvents();        
    }
    #endregion

    #region Event Subscriptions
    void SubInputEvents()
    {
        iIS.Input.InGameFPS.Move.performed += HandleMoveInput;
        iIS.Input.InGameFPS.Move.canceled += HandleMoveInput;

        iIS.Input.InGameFPS.MouseLook.performed += HandleMouseLookInput;
        iIS.Input.InGameFPS.MouseLook.canceled += HandleMouseLookInput;
    }

    void UnsubInputEvents()
    {
        iIS.Input.InGameFPS.Move.performed -= HandleMoveInput;
        iIS.Input.InGameFPS.Move.canceled -= HandleMoveInput;

        iIS.Input.InGameFPS.MouseLook.performed -= HandleMouseLookInput;
        iIS.Input.InGameFPS.MouseLook.canceled -= HandleMouseLookInput;
    }

    #endregion

    #region Input Event Callbacks
    void HandleMoveInput(InputAction.CallbackContext context)
    {
        currentInput = context.ReadValue<Vector2>();
    }

    void HandleMouseLookInput(InputAction.CallbackContext context)
    {
        playerCamHandler.MoveCamRaw(context.ReadValue<Vector2>());
    }
    #endregion
}
