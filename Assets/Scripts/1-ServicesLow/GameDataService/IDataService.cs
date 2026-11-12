public interface IDataService : IBaseService
{
    SceneData GetSceneDataByName(string sceneName);
}