using System.Collections.Generic;
using UnityEngine;

public class CameraService : MonoBehaviour, ICameraService
{
    private readonly Dictionary<string, ICineCam> registeredCams = new();

    private ICineCam previousCamera;
    private ICineCam activeCamera;

    #region Interface Implementation Properties
    public IReadOnlyDictionary<string, ICineCam> RegisteredCams => registeredCams;

    public ICineCam CurrentActiveCam => activeCamera;
    #endregion

    #region Interface Implementation Methods
    public void RegisterCamera(ICineCam cam)
    {
        if (!registeredCams.ContainsKey(cam.CamID))
        {
            registeredCams.Add(cam.CamID, cam);

            if (cam.IsSceneDefaultCamera)
            {
                SwapCamera(cam);
            }

            LogUtilities.Log($"Camera ID {cam.CamID} registered", GetType().Name);
        }
    }

    public void UnregisterCamera(ICineCam cam)
    {
        if (registeredCams.ContainsKey(cam.CamID))
        {
            registeredCams.Remove(cam.CamID);
        }
    }

    public void SwapCamera(ICineCam cam)
    {
        if (!registeredCams.ContainsKey(cam.CamID)) return;

        previousCamera?.SetCamActive(false);
        previousCamera = activeCamera;

        activeCamera = cam;
        activeCamera.SetCamActive(true);
    }
    #endregion
}
