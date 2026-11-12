using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneService : MonoService, ISceneServiceManageable
{
    private IDataService iDS;
    private IGameServiceManageable iGSM;

    [Header("Scene Settings")]
    [SerializeField] SceneServiceSettingsSO sceneServiceSettingsSO;

    [Header("Current Scene Data")]
    [SerializeField] SceneData previousSceneData;
    [SerializeField] SceneData currentSceneData;
    [SerializeField] SceneData loadingSceneData;

    #region Interface Implementation Events
    public event Action<SceneLoadingEventData> OnSceneLoading;
    public event Action<SceneData> OnFrameTimeStabilized;
    public event Action<SceneData> OnSceneCompleteLoad;
    public event Action<SceneData> OnSceneCompleteLoadPageDone;

    public event Action<float> OnUpdateLoadProgress;
    
    public event Action<UIPageSO> OnOpenLoadingPage;
    public event Action<UIPageSO> OnCloseLoadingPage;
    #endregion

    #region Interface Implementation Properties
    public SceneServiceSettingsSO SceneServiceSettingsSO => sceneServiceSettingsSO;
    #endregion

    private WaitUntilFrameTimeStabilize waitUntilFrameTimeStabilize;
    private WaitUntilLoadingPageDone waitUntilLoadingPageDone;
    private Coroutine loadCoroutine;

    private bool allPagesClosed;
    private bool closeAnimationDoneFlag;

    #region Unity Methods
    private void Awake()
    {
        if (!GameServices.IsServiceRegistered<ISceneService>())
        {
            GameServices.Register<ISceneService>(this);
            GameServices.Register<ISceneServiceManageable>(this);
            serviceRegistered = true;
        }

        iDS = GameServices.Get<IDataService>();
        iGSM = GameServices.Get<IGameServiceManageable>();

        waitUntilFrameTimeStabilize = new(iGSM, this);
        waitUntilLoadingPageDone = new();

        SubEvents();
    }

    private void Update()
    {
        FetchAVGFrameTime();
    }

    private void OnDestroy()
    {
        if (serviceRegistered)
        {
            GameServices.Unregister<ISceneService>();
            GameServices.Unregister<ISceneServiceManageable>();
            serviceRegistered = false;

            UnsubEvents();
        }
    }
    #endregion

    #region Event Subscription
    void SubEvents()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    void UnsubEvents()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }
    #endregion

    #region Event Callbacks
    void HandleSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
    {
        if (!serviceInitialized)
        {
            currentSceneData = iDS.GetSceneDataByName(loadedScene.name);

            if (currentSceneData != null)
            {
                iGSM.ChangeMainGameState(currentSceneData.DefaultMainGameState);
                OnSceneLoading?.Invoke(new(currentSceneData, LoadState.PostLoading));
                OnSceneCompleteLoad?.Invoke(currentSceneData);
                OnSceneCompleteLoadPageDone?.Invoke(currentSceneData);
                serviceInitialized = true;
            } 
        }
    }
    #endregion

    #region Interface Implementation Methods
    public void LoadSceneAsync(SceneData sceneToLoad,
                               LoadSceneMode loadSceneMode,
                               bool isAsync = true,
                               bool unloadPreviousScene = false)
    {
        if (!isAsync)
        {
            throw new NotImplementedException("Synchronous Loading is not implemented yet");
        }

        if (loadSceneMode != LoadSceneMode.Single && loadSceneMode != LoadSceneMode.Additive)
        {
            LogUtilities.LogError($"Unsupported scene load mode: {loadSceneMode}.", GetType().Name);
            return;
        }

        SceneServiceLoadData loadData = new(sceneToLoad, loadSceneMode, unloadPreviousScene);
        StartSceneLoad(loadData);
    }

    public void LoadingPageDone(UIAnimType uIAnimType) => waitUntilLoadingPageDone.CurrentType = uIAnimType;

    public void SetAllPagesClosed() => allPagesClosed = true;
    public void SetCloseAnimationDoneFlag(bool value) => closeAnimationDoneFlag = value;
    #endregion

    #region Private Methods
    void StartSceneLoad(SceneServiceLoadData loadData)
    {
        if (loadCoroutine != null)
            StopCoroutine(loadCoroutine);

        loadCoroutine = StartCoroutine(LoadSceneAsyncCoroutine(loadData));
    }

    #endregion

    #region Private Update Methods
    void FetchAVGFrameTime()
    {
        waitUntilFrameTimeStabilize.UpdateFrameTime(iGSM.PerformanceTracker.AverageFrameTime);
        waitUntilFrameTimeStabilize.UpdateTimeoutTimer(Time.unscaledDeltaTime);
    } 
    #endregion

    #region Coroutines
    IEnumerator LoadSceneAsyncCoroutine(SceneServiceLoadData loadData)
    {
        SceneData sceneData = loadData.SceneData;

        if (sceneData.IsSceneLoaded)
        {
            LogUtilities.LogWarning($"Cannot load {sceneData.SceneName}. Scene is already loaded.", GetType().Name);
            loadCoroutine = null;
            yield break;
        }

        closeAnimationDoneFlag = false;
        allPagesClosed = false;

        waitUntilFrameTimeStabilize.PrepareYieldInstruction(iGSM.PerformanceTracker.AverageFrameTime);

        loadingSceneData = sceneData;
        iGSM.ChangeLoadState(LoadState.PreLoading);
        OnSceneLoading?.Invoke(new(loadingSceneData, LoadState.PreLoading));
        waitUntilLoadingPageDone.PrepareConditions(UIAnimType.Open);
        
        while (!allPagesClosed)
        {
            LogUtilities.Log("Waiting for all pages to close.", GetType().Name);
            yield return null;
        }
            
        yield return waitUntilLoadingPageDone;

        AsyncOperation sceneToLoad = loadData.LoadSceneMode switch
        {
            LoadSceneMode.Single => SceneManager.LoadSceneAsync(sceneData.SceneName),
            LoadSceneMode.Additive => SceneManager.LoadSceneAsync(sceneData.SceneName, LoadSceneMode.Additive),
            _ => null
        };

        if (sceneToLoad == null)
        {
            LogUtilities.LogError($"Unable to start loading scene {sceneData.SceneName}.", GetType().Name);
            loadCoroutine = null;
            yield break;
        }

        iGSM.ChangeLoadState(LoadState.Loading);
        OnSceneLoading?.Invoke(new(loadingSceneData, LoadState.Loading));
        
        sceneToLoad.allowSceneActivation = false;

        bool startLoadingLogged = false;
        while (sceneToLoad.progress < 0.9f)
        {
            if (!startLoadingLogged)
            {
                LogUtilities.Log($"Now Loading Scene: {loadingSceneData.SceneName}", GetType().Name);
                startLoadingLogged = true;
            }

            OnUpdateLoadProgress?.Invoke(sceneToLoad.progress);
            yield return null;
        }

        if (currentSceneData != null)
            previousSceneData = currentSceneData;

        sceneToLoad.allowSceneActivation = true;

        while (!sceneToLoad.isDone) yield return null;

        if (loadData.UnloadPreviousScene &&
            loadData.LoadSceneMode == LoadSceneMode.Additive &&
            previousSceneData != null &&
            SceneManager.GetSceneByName(previousSceneData.SceneName).isLoaded)
        {
            AsyncOperation sceneToUnload = SceneManager.UnloadSceneAsync(previousSceneData.SceneName);
            if (sceneToUnload != null)
            {
                LogUtilities.Log($"Unloading Scene: {previousSceneData.SceneName}", GetType().Name);
                while (!sceneToUnload.isDone) yield return null;
            }
        }

        iGSM.ChangeLoadState(LoadState.PostLoading);
        OnSceneLoading?.Invoke(new(loadingSceneData, LoadState.PostLoading));
        waitUntilFrameTimeStabilize.ResetTimeoutTimer();

        yield return waitUntilFrameTimeStabilize;

        waitUntilLoadingPageDone.PrepareConditions(UIAnimType.Close);
        OnFrameTimeStabilized?.Invoke(loadingSceneData);

        while (!closeAnimationDoneFlag) yield return null;
        currentSceneData = loadingSceneData;
        loadingSceneData = null;

        iGSM.ChangeMainGameState(currentSceneData.DefaultMainGameState);
        iGSM.ChangeLoadState(LoadState.Done);
        OnSceneCompleteLoad?.Invoke(currentSceneData);
        
        yield return waitUntilLoadingPageDone;

        OnSceneCompleteLoadPageDone?.Invoke(currentSceneData);

        loadCoroutine = null;
    }
    #endregion
}