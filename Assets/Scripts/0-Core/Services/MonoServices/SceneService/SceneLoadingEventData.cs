public readonly struct SceneLoadingEventData
{
    public SceneData SceneData { get; }
    public LoadState LoadState { get; }

    public SceneLoadingEventData(SceneData sceneData,
                                 LoadState loadState)
    {
        SceneData = sceneData;
        LoadState = loadState;
    }
}