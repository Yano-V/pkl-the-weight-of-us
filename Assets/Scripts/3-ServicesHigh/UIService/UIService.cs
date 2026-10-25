using UnityEngine;

public class UIService : MonoService, IUIService
{
    [SerializeField] UIServiceSettingsSO uIServiceSettingsSO;

    private ISceneServiceManageable iSSM;

    [SerializeField] UIPageHandler uIPageHandler;
 
    [SerializeField] Canvas loadingUICanvas;

    #region Interface Implementation Properties
    public Canvas LoadingUICanvas => loadingUICanvas;
    public UIPageHandler UIPageHandler => uIPageHandler;
    public UIServiceSettingsSO UIServiceSettingsSO => uIServiceSettingsSO;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!GameServices.IsServiceRegistered<IUIService>())
        {
            GameServices.Register<IUIService>(this);
            serviceRegistered = true;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        iSSM = GameServices.Get<ISceneServiceManageable>();

        uIPageHandler = new(this, iSSM);

        InstantiateLoadingPages();
    }

    private void OnDestroy()
    {
        if (serviceRegistered)
        {
            GameServices.Unregister<IUIService>();
            serviceRegistered = false;

            uIPageHandler.DestroyClass();
        }
    }
    #endregion

    void InstantiateLoadingPages()
    {
        foreach (GameObject obj in uIServiceSettingsSO.LoadingPagePrefabs)
        {
            if (obj == null) continue;

            Instantiate(obj);
        }
    }
}