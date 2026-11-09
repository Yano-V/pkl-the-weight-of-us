using System;
using UnityEngine.SceneManagement;

public class SceneSetupBuilder
{
    private readonly ISceneServiceManageable iSSM;
    private readonly IUIService iUIS;

    private SceneData sceneData;
    private UIPageSO loadingPage;
    private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;
    private bool unloadPreviousScene;

    public SceneSetupBuilder()
    {
        iSSM = GameServices.Get<ISceneServiceManageable>();
        iUIS = GameServices.Get<IUIService>();
    }

    public SceneSetupBuilder(ISceneServiceManageable iSSM, IUIService iUIS)
    {
        this.iSSM = iSSM;
        this.iUIS = iUIS;
    }

    public SceneSetupBuilder SetSceneData(SceneData sceneData)
    {
        this.sceneData = sceneData;
        return this;
    }

    public SceneSetupBuilder SetLoadingPage(UIPageSO loadingPage)
    {
        if (loadingPage == null || loadingPage.UIPageType != UIPageType.Loading)
            throw new ArgumentException("The provided page must be a loading page.", nameof(loadingPage));

        this.loadingPage = loadingPage;
        return this;
    }

    public SceneSetupBuilder SetLoadSceneMode(LoadSceneMode loadSceneMode)
    {
        this.loadSceneMode = loadSceneMode;
        return this;
    }

    public SceneSetupBuilder SetUnloadPreviousScene()
    {
        unloadPreviousScene = true;
        return this;
    }

    public void InitiateSceneLoad()
    {
        if (sceneData == null || loadingPage == null)
        {
            string sceneDataText = sceneData != null ? sceneData.SceneName : "null";
            string loadingPageText = loadingPage != null ? loadingPage.PageID : "null";
            LogUtilities.LogError($"Builder is invalid. Scene Data is {sceneDataText} and Loading page is {loadingPageText}.", GetType().Name);
            return;
        }

        iUIS.UIPageHandler.PrepareUIPageSO(loadingPage);
        iSSM.LoadSceneAsync(sceneData, loadSceneMode, true, unloadPreviousScene);
    }
}