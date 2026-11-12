using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : MonoService, IInputService
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

    void Awake()
    {
        if (GameServices.IsServiceRegistered<IInputService>())
        {
            Destroy(gameObject);
            return;
        }

        GameServices.Register<IInputService>(this);
        serviceRegistered = true;

        input = new MainGameInput();
        ChangeActionMap(initialActionMapName);
    }

    void OnDestroy()
    {
        if (serviceRegistered)
        {
            GameServices.Unregister<IInputService>();
            serviceRegistered = false;
        }
    }

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