using UnityEngine.SceneManagement;

public readonly struct SceneServiceLoadData
{
    public SceneData SceneData { get; }
    public LoadSceneMode LoadSceneMode { get; }
    public bool UnloadPreviousScene { get; }

    public SceneServiceLoadData(SceneData sceneData,
                                LoadSceneMode loadSceneMode,
                                bool unloadPreviousScene = false)
    {
        SceneData = sceneData;
        LoadSceneMode = loadSceneMode;
        UnloadPreviousScene = unloadPreviousScene;
    }
}