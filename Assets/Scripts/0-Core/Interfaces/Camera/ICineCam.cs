using UnityEngine;

public interface ICineCam
{
    GameObject gameObject { get; }

    Quaternion CamRotation { get; }
    Vector3 CamRotationEulerAngles { get; }

    string CamID { get; }
    bool IsSceneDefaultCamera { get; }

    void SetCamActive(bool active);
}