using UnityEngine;

public interface ICineCam
{
    GameObject gameObject { get; }

    string CamID { get; }
    bool IsSceneDefaultCamera { get; }

    void SetCamActive(bool active);
}