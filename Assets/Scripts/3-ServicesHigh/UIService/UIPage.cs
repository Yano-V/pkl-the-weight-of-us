using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIPage : MonoBehaviour
{
    private ISceneServiceManageable iSSM;
    private IUIService iUIS;
    private UIPageHandler pageHandler;

    private readonly List<IUIServiceSelectable> selectables = new();

    public event Action OnRefreshPage;

    [Header("Service Level Settings")]
    [SerializeField] UIServiceSettingsSO uIServiceSettingsSO;

    [Header("UI Page References")]
    [Tooltip("The information of the UI Page")]
    [SerializeField] UIPageSO uIPageSO;

    [Tooltip("The GameObject holding the contents of the UI Page")]
    [SerializeField] GameObject contentObj;

    [SerializeField] UIAnimHandler uIAnimHandler; // Add HideInInspector when deemed functional

    [Header("Page Settings")]
    [SerializeField] bool useAnimator;

    [Header("Editor Only Settings")]
    [SerializeField] bool autoSetRootContentName;

    private bool isOpen;

    #region Class Properties
    public UIPageSO UIPageSO => uIPageSO;
    public UIPageType UIPageType => uIPageSO.UIPageType;
    public OpenPageType OpenPageType => uIPageSO.OpenPageType;
    public string PageID => uIPageSO != null ? uIPageSO.PageID : "";
    public bool IsOpen => isOpen;

    public bool PageRegistered { get; set; }
    #endregion

    #region Editor Only Properties
    public bool AutoSetRootContentName => autoSetRootContentName;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        iSSM = GameServices.Get<ISceneServiceManageable>();
        iUIS = GameServices.Get<IUIService>();
        pageHandler = iUIS.UIPageHandler;
        uIServiceSettingsSO = iUIS.UIServiceSettingsSO;

        pageHandler.RegisterPage(this);
    }

    private void OnDestroy()
    {
        pageHandler.UnregisterPage(this);
    }
    #endregion

    #region Public Methods
    public void DisplayPage()
    {
        isOpen = true;

        contentObj.SetActive(true);

        if (uIAnimHandler != null &&
            uIAnimHandler.HasAnAnimator &&
            useAnimator)
        {
            uIAnimHandler.PlayOpenAnimation();
        }
    }

    public void HidePage()
    {
        isOpen = false;

        if (uIAnimHandler != null &&
            uIAnimHandler.HasAnAnimator &&
            useAnimator)
        {
            uIAnimHandler.PlayCloseAnimation();
            return;
        }

        contentObj.SetActive(false);
    }

    public void DisableContentPage()
    {
        contentObj.SetActive(false);
    }

    public void EnableContentPage()
    {
        contentObj.SetActive(true);
    }

    public void UIAnimatorFinished(UIAnimType doneType)
    {
        if (UIPageType == UIPageType.Loading && !uIServiceSettingsSO.DontSendLoadingPageDoneEvent)
        {
            iSSM.LoadingPageDone(doneType);
        }

        if (doneType == UIAnimType.Close)
        {
            DisableContentPage();
            pageHandler.PingAnimationDone();
        }
    }

    public void SetInteractable(bool value)
    {
        foreach (IUIServiceSelectable selectable in selectables)
        {
            if (selectable != null)
                selectable.Interactable = value;
        }   

        if (value)
            OnRefreshPage?.Invoke();
    }

    public void RegisterSelectable(IUIServiceSelectable serviceSelectable)
    {
        if (!selectables.Contains(serviceSelectable))
            selectables.Add(serviceSelectable);
    }

    public void UnregisterSelectable(IUIServiceSelectable serviceSelectable)
    {
        if (selectables.Contains(serviceSelectable))
            selectables.Remove(serviceSelectable);
    }
    #endregion

#if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        // Get the UIContent
        if (contentObj == null)
        {
            Transform[] transforms = GetComponentsInChildren<Transform>();

            foreach (var trans in transforms)
            {
                if (trans == transform) continue;

                if (trans.CompareTag("UIContent"))
                {
                    contentObj = trans.gameObject;
                    break;
                }
            }
        }

        if (uIAnimHandler == null &&
            useAnimator &&
            !Application.isPlaying)
        {
            UIAnimHandler handler = GetComponent<UIAnimHandler>();

            if (handler != null)
                uIAnimHandler = handler;
            else
                uIAnimHandler = gameObject.AddComponent<UIAnimHandler>();
        }

        if (autoSetRootContentName)
            SetRootContentName();
    }

    public void SetRootContentName()
    {
        if (uIPageSO != null && name != PageID + "Root")
        {
            name = PageID + "Root";
        }

        if (contentObj != null && contentObj.name != PageID + "Contents")
            contentObj.name = PageID + "Contents";
    }
#endif
} 
