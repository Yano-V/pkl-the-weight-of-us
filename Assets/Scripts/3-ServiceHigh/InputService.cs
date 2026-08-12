using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class InputService : MonoBehaviour, IInputService
{
    private MainGameInput input;

    [SerializeField] string initialActionMapName;

    private InputActionMap previousMap;
    private InputActionMap currentMap;

    #region Interface Implementation Properties
    public MainGameInput Input
    {
        get
        {
            if (input == null)
            {
                throw new NullReferenceException();
            }

            return input;
        }
    }

    public InputActionMap CurrentMap => currentMap;
    #endregion

    #region Interface Implementation Events
    public event Action<InputActionMap> OnActiveMapChanged;
    #endregion

    #region Dependency Injection
    [Inject]
    public void Construct(MainGameInput pInput)
    {
        input = pInput;
        ChangeActionMap(initialActionMapName);
        LogUtilities.Log("MainGameInput injected.", GetType().Name);
    }
    #endregion

    #region Interface Implementation Methods
    public void ChangeActionMap(string mapName)
    {
        InputActionMap map = input.asset.FindActionMap(mapName, true);

        if (map == null) return;

        previousMap?.Disable();

        previousMap = currentMap;
        currentMap = map;

        currentMap.Enable();

        OnActiveMapChanged?.Invoke(currentMap);
    }
    #endregion    
}