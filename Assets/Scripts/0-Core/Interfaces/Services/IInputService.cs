using System;
using UnityEngine.InputSystem;

public interface IInputService : IBaseService
{
    event Action<InputActionMap> OnActiveMapChanged;

    MainGameInput Input { get; }
    InputActionMap CurrentMap { get; }

    void ChangeActionMap(string mapName);
}