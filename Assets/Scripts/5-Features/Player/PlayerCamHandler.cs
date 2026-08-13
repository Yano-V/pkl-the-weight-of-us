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
    [SerializeField, Min(0)] float moveSpeed = 5;

    public string CamID => $"{iDPrefix}{gameObject.name}{iDSuffix}";
    public bool IsSceneDefaultCamera => isSceneDefaultCamera;

    #region Private Properties
    private float LookX
    {
        get => lookX;
        set => lookX = Mathf.Clamp(value, -90, 90);
    }

    private float LookY
    {
        get => lookY;
        set => lookY = MathUtils.WrapAngle180(value);
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

    public void MoveCam(Vector2 value)
    {
        Vector2 input = Vector2.ClampMagnitude(value, 1f);
        Quaternion yawRotation = Quaternion.Euler(0, LookY, 0);
        Vector3 direction = yawRotation * new Vector3(input.x, 0, input.y);

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    #region Unity Editor Methods
    private void OnValidate()
    {
        if (cam == null)
            cam = GetComponent<CinemachineCamera>();
    }
    #endregion
}
