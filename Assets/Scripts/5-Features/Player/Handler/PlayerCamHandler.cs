using ProjLimbo;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(CinemachineCamera))]
public class PlayerCamHandler : MonoBehaviour, ICineCam
{
    private ICameraService iCS;

    [SerializeField] CinemachineCamera cam;

    [SerializeField] string iDPrefix;
    [SerializeField] string iDSuffix;
    [SerializeField] bool isSceneDefaultCamera;

    [SerializeField] float lookX; // Refers to Transform's Y
    [SerializeField] float lookY; // Refers to Transform's X

    [SerializeField] float sensitivity = 1;

    #region Interface Implementation Methods
    public Quaternion CamRotation => Quaternion.Euler(new(LookX, LookY, transform.rotation.eulerAngles.z));
    public Vector3 CamRotationEulerAngles => new(LookX, LookY, transform.rotation.eulerAngles.z);

    public string CamID => $"{iDPrefix}{gameObject.name}{iDSuffix}";
    public bool IsSceneDefaultCamera => isSceneDefaultCamera;
    #endregion

    #region Private Properties
    public float LookX
    {
        get => lookX;
        private set => lookX = Mathf.Clamp(value, -90, 90);
    }

    public float LookY
    {
        get => lookY;
        private set => lookY = MathUtils.WrapAngle180(value);
    }
    #endregion


    [Inject]
    public void Construct(ICameraService cameraService)
    {
        iCS = cameraService;

        LogUtilities.Assert(iCS != null, "Camera Service is null.", GetType().Name, gameObject);

        iCS.RegisterCamera(this);
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(new(LookX, LookY, 0));
    }

    void OnDestroy()
    {
        iCS.UnregisterCamera(this);
    }

    public void SetCamActive(bool active)
    {
        cam.enabled = active;
    }

    public void MoveCamRaw(Vector2 value)
    {
        LookY += value.x * sensitivity;
        LookX -= value.y * sensitivity;
    }

    #region Unity Editor Methods
    private void OnValidate()
    {
        if (cam == null)
            cam = GetComponent<CinemachineCamera>();
    }
    #endregion
}
