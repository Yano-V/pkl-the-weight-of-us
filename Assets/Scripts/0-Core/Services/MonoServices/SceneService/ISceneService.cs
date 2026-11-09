using System;
using UnityEngine.SceneManagement;

public interface ISceneService : IBaseService
{
    SceneServiceSettingsSO SceneServiceSettingsSO { get; }

    event Action<SceneLoadingEventData> OnSceneLoading;
    event Action<SceneData> OnFrameTimeStabilized;
    event Action<SceneData> OnSceneCompleteLoad;
    event Action<SceneData> OnSceneCompleteLoadPageDone;

    event Action<UIPageSO> OnOpenLoadingPage;
    event Action<UIPageSO> OnCloseLoadingPage;

    event Action<float> OnUpdateLoadProgress;
}

public interface ISceneServiceManageable : ISceneService
{
    void LoadingPageDone(UIAnimType uIAnimType);
    void SetAllPagesClosed();
    void SetCloseAnimationDoneFlag(bool value);

    void LoadSceneAsync(SceneData sceneToLoad,
                        LoadSceneMode loadSceneMode,
                        bool isAsync = true,
                        bool unloadPreviousScene = false);
}