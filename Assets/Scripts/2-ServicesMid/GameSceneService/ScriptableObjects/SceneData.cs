using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Limbo Scene Data", menuName = "Limbo/Scenes/Scene Data")]
public class SceneData : ScriptableObject
{
    [Tooltip("The actual scene name of the scene.")]
    [SerializeField] string sceneName;
    [SerializeField] UIPageSO sceneDefaultPage;
    [SerializeField] EnumMainGameState defaultMainGameState;

    public string SceneName => sceneName;
    public UIPageSO SceneDefaultPage => sceneDefaultPage;
    public EnumMainGameState DefaultMainGameState => defaultMainGameState;
    public bool IsSceneLoaded => SceneManager.GetSceneByName(sceneName).isLoaded;
}