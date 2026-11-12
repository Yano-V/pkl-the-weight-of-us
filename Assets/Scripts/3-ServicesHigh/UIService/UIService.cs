using UnityEngine;
using UnityEngine.EventSystems;

public class UIService : MonoService, IUIService
{
    [SerializeField] UIServiceSettingsSO uIServiceSettingsSO;

    private ISceneServiceManageable iSSM;

    [SerializeField] UIPageHandler uIPageHandler;
 
    [SerializeField] Canvas loadingUICanvas;
    [SerializeField] EventSystem eventSystem;

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
        iSSM.OnSceneLoading += HandleOnSceneLoading;

        InstantiateLoadingPages();
    }

    private void OnDestroy()
    {
        if (serviceRegistered)
        {
            iSSM.OnSceneLoading -= HandleOnSceneLoading;
            GameServices.Unregister<IUIService>();
            serviceRegistered = false;

            uIPageHandler.DestroyClass();
        }
    }
    #endregion

    private void HandleOnSceneLoading(SceneLoadingEventData data)
    {
        if (data.LoadState != LoadState.PostLoading) return;

        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (EventSystem sceneEventSystem in eventSystems)
        {
            if (sceneEventSystem == eventSystem) continue;

            Destroy(sceneEventSystem.gameObject);
        }
    }

    void InstantiateLoadingPages()
    {
        foreach (GameObject obj in uIServiceSettingsSO.LoadingPagePrefabs)
        {
            if (obj == null) continue;

            Instantiate(obj);
        }
    }
}