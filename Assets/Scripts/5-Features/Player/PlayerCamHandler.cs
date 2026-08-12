using Unity.Cinemachine;
using UnityEngine;
using VContainer;

public class PlayerCamHandler : MonoBehaviour, ICineCam
{
    private ICameraService iCS;

    [SerializeField] CinemachineCamera cam;

    [SerializeField] string iDPrefix;
    [SerializeField] string iDSuffix;
    [SerializeField] bool isSceneDefaultCamera;

    public string CamID => $"{iDPrefix}{gameObject.name}{iDSuffix}";
    public bool IsSceneDefaultCamera => isSceneDefaultCamera;

    [Inject]
    public void Construct(ICameraService cameraService)
    {
        iCS = cameraService;

        LogUtilities.Assert(iCS != null, "Camera Service is null.", GetType().Name, gameObject);

        iCS.RegisterCamera(this);
    }

    void OnDestroy()
    {
        iCS.UnregisterCamera(this);
    }

    public void SetCamActive(bool active)
    {
        cam.enabled = active;
    }
}
