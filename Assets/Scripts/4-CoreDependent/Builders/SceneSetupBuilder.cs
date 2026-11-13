using System;
using UnityEngine.SceneManagement;

// ================================================================================= //
//                                                                                   //
// Class Name: Scene Setup Builder                                                   //
// Description: The central builder for all scene and room transitions in the game.  //
//              It enforces a single, validated flow for preparing scene context,    //
//              selecting the appropriate loading UI, configuring how the load       //
//              should behave, and delegating the actual async scene request to the  //
//              scene service. This keeps scene setup consistent across the codebase //
//              and prevents invalid or partially configured transitions from being  //
//              sent to the scene system.                                            //
// Author: PsychoLucidia                                                             //
//                                                                                   //
// ================================================================================= //

public class SceneSetupBuilder
{
    #region Fields

    private readonly ISceneServiceManageable iSSM;
    private readonly IUIService iUIS;

    private SceneData sceneData;
    private UIPageSO loadingPage;
    private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;
    private bool unloadPreviousScene;

    #endregion

    #region Constructors

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

    #endregion

    #region Builder Configuration

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

    #endregion

    #region Builder Lifecycle

    /// <summary>
    /// Clears the current builder state so the same instance can be reused safely.
    /// </summary>
    /// <remarks>
    /// Only use this method if the builder instance is intentionally cached and reused.
    /// </remarks>
    public void ResetBuilder()
    {
        sceneData = null;
        loadingPage = null;
        loadSceneMode = LoadSceneMode.Additive;
        unloadPreviousScene = false;
    }

    #endregion

    #region Public Methods

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

    #endregion
}