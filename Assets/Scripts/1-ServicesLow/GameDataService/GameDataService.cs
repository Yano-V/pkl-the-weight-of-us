using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameDataService : MonoService, IDataService
{
    [SerializeField] AssetLabelReference sceneDataLabel;

    [SerializeField] List<SceneData> sceneDatas = new();

    #region Unity Methods
    private void Awake()
    {
        if (!GameServices.IsServiceRegistered<IDataService>())
        {
            GameServices.Register<IDataService>(this);
            serviceRegistered = true;
        }

        LoadSceneData();
    }

    private void OnDestroy()
    {
        if (serviceRegistered)
        {
            GameServices.Unregister<IDataService>();
            serviceRegistered = false;
        }
    }
    #endregion

    #region Interface Implementation Methods
    public SceneData GetSceneDataByName(string sceneName)
    {
        SceneData data = sceneDatas.FirstOrDefault(sceneData => sceneData.SceneName == sceneName);

        if (data == null)
        {
            // TODO: Create an exception that will be thrown when an asset is not found.
            // Temporary. Will use an Exception later on.

            LogUtilities.LogError($"Scene Data asset not found: {sceneName}", GetType().Name);
            return null;
        }

        return data;
    }
    #endregion

    #region Addressables Methods
    void LoadSceneData()
    {
        AsyncOperationHandle<IList<SceneData>> sceneLoadHandle = Addressables.LoadAssetsAsync<SceneData>(sceneDataLabel, null);

        var loadedData = sceneLoadHandle.WaitForCompletion();

        if (sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sceneDatas.Clear();

            foreach (SceneData data in loadedData)
            {
                if (data != null)
                    sceneDatas.Add(data);
            }
        }
    }
    #endregion
}