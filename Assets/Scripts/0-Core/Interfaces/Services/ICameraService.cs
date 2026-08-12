using System.Collections.Generic;
using UnityEngine;

public interface ICameraService
{
    IReadOnlyDictionary<string, ICineCam> RegisteredCams { get; }

    void RegisterCamera(ICineCam cam);
    void UnregisterCamera(ICineCam cam);
    void SwapCamera(ICineCam cam);
}