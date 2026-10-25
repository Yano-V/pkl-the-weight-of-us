using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIPageHandler
{
    // Identifies the animator group, animator, and animation that completed.
    public event Action<AnimationDoneData> OnDonePlaying;

    private readonly IUIService iUIS; 
    private readonly ISceneServiceManageable iSSM;

    private readonly UIServiceSettingsSO uIServiceSettingsSO;

    // String is the UIPageSO PageID and UIPage is the actual page instance in the scene.
    private readonly Dictionary<string, UIPage> registeredPages = new();
    private readonly Dictionary<string, UIPage> loadingPages = new();

    [SerializeField] List<UIPage> openedPage = new();
    [SerializeField] UIPage currentLoadingUI;

    private UIPageSO preparedUIPageSO;


    public UIPage SceneDefaultPage => openedPage[0];
    public IReadOnlyDictionary<string, UIPage> RegisteredPages => registeredPages;
    public IReadOnlyDictionary<string, UIPage> LoadingPages => loadingPages;

    // TODO (Priority: Low):
    // Loading loading UI Pages will be loaded into memory asynchronously
    // using Addressables. For now, we can load it in Awake() since the screens doesn't
    // use that memory yet.

    public UIPageHandler(IUIService iUIS,
                         ISceneServiceManageable iSSM)
    {
        this.iUIS = iUIS;
        this.iSSM = iSSM;

        uIServiceSettingsSO = iUIS.UIServiceSettingsSO;

        this.iSSM.OnScenePreLoading += HandleScenePreLoading;
        this.iSSM.OnFrameTimeStabilized += HandleFrameTimeStabilized;
        this.iSSM.OnSceneCompleteLoad += HandleSceneCompleteLoad;
        this.iSSM.OnSceneCompleteLoadPageDone += HandleSceneCompleteLoadPageDone;
    }

    public void DestroyClass()
    {
        iSSM.OnScenePreLoading -= HandleScenePreLoading;
        iSSM.OnFrameTimeStabilized -= HandleFrameTimeStabilized;
        iSSM.OnSceneCompleteLoad -= HandleSceneCompleteLoad;
        iSSM.OnSceneCompleteLoadPageDone -= HandleSceneCompleteLoadPageDone;
    }

    #region Event Callbacks
    void HandleScenePreLoading(SceneData data)
    {
        for (int i = openedPage.Count - 1; i >= 0; i--)
        {
            if (openedPage[i] == null) continue;

            openedPage[i].SetInteractable(false);
            openedPage[i].HidePage();
        }

        int openedCount = 0;

        foreach (UIPage page in openedPage)
        {
            if (page == null) continue;

            openedCount++;
        }

        if (openedCount <= 0 && preparedUIPageSO != null)
        {
            OpenLoadingPage(preparedUIPageSO);
            preparedUIPageSO = null;
            iSSM.SetAllPagesClosed();
        }
    }

    void HandleFrameTimeStabilized(SceneData data)
    {
        CloseLoadingPage();
    }

    void HandleSceneCompleteLoad(SceneData data)
    {
        if (data.SceneDefaultPage == null) return;

        if (registeredPages.TryGetValue(data.SceneDefaultPage.PageID, out UIPage page))
        {
            if (openedPage.Count == 0)
            {
                openedPage.Add(page);
            }
            else
            {
                for (int i = openedPage.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                        openedPage[i] = page;
                    else
                        openedPage.RemoveAt(i);
                }
            }
        }
    }

    void HandleSceneCompleteLoadPageDone(SceneData data)
    {
        if (data.SceneDefaultPage == null) return;

        if (registeredPages.TryGetValue(data.SceneDefaultPage.PageID, out UIPage page))
        {
            page.DisplayPage();
        }
    }
    #endregion

    public void OpenPage(UIPageSO data)
    {
        if (openedPage.Count == 0) return;

        if (registeredPages.TryGetValue(data.PageID, out var page))
        {
            UIPage previousPage = openedPage[^1];

            previousPage.SetInteractable(false);
            if (page.OpenPageType == OpenPageType.ClosePreviousPage)
                previousPage.HidePage();

            page.DisplayPage();
            page.SetInteractable(true);
            openedPage.Add(page);
        }
        else
        {
            LogUtilities.LogError($"UIPage ID '{data.PageID}' is not registered in the handler.", GetType().Name);
        }
    }

    public void CloseTopPage()
    {
        if (openedPage.Count < 1)
        {
            LogUtilities.LogError("Cannot close the last entry in the list.", GetType().Name);
            return;
        }
            
        UIPage currentPage = openedPage[^1];
        UIPage previousPage = openedPage[^2];

        currentPage.SetInteractable(false);
        currentPage.HidePage();

        if (currentPage.OpenPageType == OpenPageType.ClosePreviousPage)
            previousPage.DisplayPage();
        previousPage.SetInteractable(true);

        openedPage.RemoveAt(openedPage.Count - 1);
    }

    public void OpenLoadingPage(UIPageSO loadingPage)
    {
        if (loadingPages.TryGetValue(loadingPage.PageID, out var uIPage))
        {
            currentLoadingUI = uIPage;
        }

        currentLoadingUI.DisplayPage();
    }

    public void CloseLoadingPage()
    {
        currentLoadingUI.HidePage();

        currentLoadingUI = null;
    }

    public void RegisterPage(UIPage page)
    {
        if (!registeredPages.ContainsKey(page.PageID))
        {
            registeredPages.Add(page.PageID, page);
            LogUtilities.Log($"Registered UIPage: {page.PageID} | Number of registered Pages: {registeredPages.Count}", GetType().Name);

            page.PageRegistered = true;

            page.EnableContentPage();
            page.DisableContentPage();

            if (page.UIPageType == UIPageType.Loading)
            {
                loadingPages.Add(page.PageID, page);
                page.transform.SetParent(iUIS.LoadingUICanvas.transform, false);
            }
        }
        else
        {
            UnityEngine.Object.Destroy(page.gameObject);
            LogUtilities.LogWarning($"Duplicate UIPage Instance: {page.PageID}. Destroying Duplicate.", GetType().Name);
        }
    }

    public void UnregisterPage(UIPage page)
    {
        if (registeredPages.TryGetValue(page.PageID, out var outPage))
        {
            if (!page.PageRegistered) return;

            outPage.PageRegistered = false;

            if (loadingPages.ContainsKey(page.PageID))
                loadingPages.Remove(page.PageID);

            registeredPages.Remove(page.PageID);
            LogUtilities.Log($"Unregistered UIPage: {page.PageID} | Number of registered Pages: {registeredPages.Count}", GetType().Name);
        }
    }

    public void PrepareUIPageSO(UIPageSO uIPageSO) => preparedUIPageSO = uIPageSO;

    public void PingAnimationDone()
    {
        int totalPages = 0;
        int closedPages = 0;

        foreach (UIPage page in openedPage)
        {
            if (page == null)
            {
                continue;
            }

            totalPages++;
            if (!page.IsOpen)
                closedPages++;
        }

        if (totalPages == closedPages || totalPages == 0 && preparedUIPageSO != null)
        {

            iSSM.SetAllPagesClosed();

            if (preparedUIPageSO != null)
            {                
                OpenLoadingPage(preparedUIPageSO);
                preparedUIPageSO = null;
            }
        }
    }

    public void SendDonePlayingEvent(AnimationDoneData animationData)
    {
        OnDonePlaying?.Invoke(animationData);

        if (animationData.SignalsSceneTransitionCloseAnimationDone &&
            animationData.UIAnimType == UIAnimType.Close && 
            !uIServiceSettingsSO.DontSendLoadingPageDoneEvent)
        {
            iSSM.SetCloseAnimationDoneFlag(true);
        }
    }
} 
