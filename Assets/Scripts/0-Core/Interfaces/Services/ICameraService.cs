using System.Collections.Generic;
using UnityEngine;

public interface ICameraService : IBaseService
{
    IReadOnlyDictionary<string, ICineCam> RegisteredCams { get; }

    ICineCam CurrentActiveCam { get; }

    void RegisterCamera(ICineCam cam);
    void UnregisterCamera(ICineCam cam);
    void SwapCamera(ICineCam cam);
}