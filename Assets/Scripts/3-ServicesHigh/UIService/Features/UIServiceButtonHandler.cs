using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIServiceButtonHandler : MonoBehaviour, IUIServiceSelectable
{
    private IUIService iUIS;

    [SerializeField, HideInInspector] UIPage uIPage;

    [SerializeField] UIPageSO pageToOpen;
    [SerializeField] Selectable button;

    public bool Interactable { get => button.interactable; set => button.interactable = value; }

    #region Unity Methods
    private void Awake()
    {
        iUIS = GameServices.Get<IUIService>();

        uIPage.RegisterSelectable(this);
    }

    private void OnDestroy()
    {
        uIPage.UnregisterSelectable(this);
    }
    #endregion

    public void B_OpenPage()
    {
        iUIS.UIPageHandler.OpenPage(pageToOpen);
    }

    public void B_CloseCurrentPage()
    {
        iUIS.UIPageHandler.CloseTopPage();
    }

#if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        if (uIPage == null)
            uIPage = GetComponentInParent<UIPage>();

        if (button == null)
            button = GetComponent<Button>();
    }
#endif
}