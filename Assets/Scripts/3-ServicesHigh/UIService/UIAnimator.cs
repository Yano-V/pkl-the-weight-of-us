using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnimator : MonoBehaviour, IUIAnimator
{
    protected IUIService iUIS;
    protected UIPageHandler uIPageHandler;

    public event Action<IUIAnimator> OnAnimatorDone;

#if UNITY_EDITOR
    [SerializeField, HideInInspector] UIAnimatorGroupID prevGroupID;
#endif

    [SerializeField] protected UIAnimatorGroup uIAnimatorGroup;
    [SerializeField] protected UIAnimatorGroupID uIAnimatorGroupID;

    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected CanvasGroup canvasGroup;

    [Header("Animations")]
    [SerializeField] protected List<UIAnimationGroup> animationGroups = new();

    [Header("Instantiated Data")]
    [SerializeField] protected List<UIAnimationGroup> instantiatedAnimGroups = new();

    [Header("Debug Values")]
    [Tooltip("Number of animator groups that have completed their animations.")]
    [SerializeField] protected int doneAnimating = 0;
    [SerializeField] protected bool isAnimating = false;

    [Header("Debug Values: Tween IDs")]
    [SerializeField] protected int moveTweenID = -1;
    [SerializeField] protected int scaleTweenID = -1;
    [SerializeField] protected int rotateTweenID = -1;
    [SerializeField] protected int fadeTweenID = -1;

    #region Interface Implementation Properties
    public int MoveTweenID { get => moveTweenID; set => moveTweenID = value; }
    public int ScaleTweenID { get => scaleTweenID; set => scaleTweenID = value; }
    public int RotateTweenID { get => rotateTweenID; set => rotateTweenID = value; }
    public int FadeTweenID { get => fadeTweenID; set => fadeTweenID = value; }

    public virtual bool IsLooper => false;

    public string AnimatorGroupID => uIAnimatorGroup.UIAnimatorGroupID.AnimGroupID;
    public string animatorID => name;
    #endregion

    private bool isRefreshing = false;

    #region Unity Methods
    private void Awake()
    {
        iUIS = GameServices.Get<IUIService>();
        uIPageHandler = iUIS.UIPageHandler;

        if (uIAnimatorGroup != null)
            uIAnimatorGroup.RegisterAnimator(this);

        InstantiateAnimationGroups();
    }

    private void OnDestroy()
    {
        if (uIAnimatorGroup != null)
            uIAnimatorGroup.UnregisterAnimator(this);
    }
    #endregion

    #region Event Callbacks
    protected void HandleAnimationGroupDone(UIAnimationGroup group)
    {
        group.OnAnimationGroupDone -= HandleAnimationGroupDone;
        doneAnimating++;

        if (doneAnimating == instantiatedAnimGroups.Count)
        {
            OnAnimatorDone?.Invoke(this);
            isAnimating = false;
        }
    }

    #endregion

    #region Interface Implementation Methods
    public virtual void CloseAnimation()
    {
        if (instantiatedAnimGroups.Count == 0)
        {
            OnAnimatorDone?.Invoke(this);
            isAnimating = false;
            return;
        }

        ResetDoneCount();
        isAnimating = true;

        foreach (UIAnimationGroup animGroup in instantiatedAnimGroups)
        {
            animGroup.OnAnimationGroupDone += HandleAnimationGroupDone;
            animGroup.CloseAnimation();
        }
    }

    public virtual void OpenAnimation()
    {
        if (instantiatedAnimGroups.Count == 0)
        {
            OnAnimatorDone?.Invoke(this);
            isAnimating = false;
            return;
        }

        ResetDoneCount();
        isAnimating = true;

        foreach (UIAnimationGroup animGroup in instantiatedAnimGroups)
        {
            animGroup.OnAnimationGroupDone += HandleAnimationGroupDone;
            animGroup.OpenAnimation();
        }
    }

    public virtual void StopAnimation()
    {
        if (instantiatedAnimGroups.Count == 0) return;

        ResetDoneCount();

        if (!isRefreshing)
            isAnimating = false;

        foreach (UIAnimationGroup animGroup in instantiatedAnimGroups)
        {
            animGroup.OnAnimationGroupDone -= HandleAnimationGroupDone;
            animGroup.StopAnimation();
        }

        // for (int i = 0; i < instantiatedAnimGroups.Count; i++)
        // {
        //     instantiatedAnimGroups[i].StopAnimation();
        //     instantiatedAnimGroups[i].OnAnimationGroupDone -= HandleAnimationGroupDone;
        // }
    }
    #endregion

    #region Public Methods
    public void ReinstantiateAnimationGroups() // For Editor
    {   
        isRefreshing = true;

        if (isAnimating)
        {
            StopAnimation();
        }

        LogUtilities.Log($"Refreshing Animation", GetType().Name, gameObject);

        if (instantiatedAnimGroups.Count > 0)
        {
            for (int i = instantiatedAnimGroups.Count - 1; i >= 0; i--)
            {
                instantiatedAnimGroups[i].RemoveInstantiatedAnimations();
                Destroy(instantiatedAnimGroups[i]);
                instantiatedAnimGroups.RemoveAt(i);
            }
        }

        InstantiateAnimationGroups();
    }
    #endregion

    #region Protected Methods
    protected void InstantiateAnimationGroups()
    {
        if (animationGroups.Count == 0) return;

        for (int i = 0; i < animationGroups.Count; i++)
        {
            if (animationGroups[i] == null) continue;

            UIAnimationGroup group = Instantiate(animationGroups[i]);

            group.Initialize(this, rectTransform, canvasGroup, uIPageHandler, IsLooper);
            instantiatedAnimGroups.Add(group);
        }

        if (isAnimating)
        {
            OpenAnimation();
        }

        LogUtilities.Log("Refreshed Animation.", GetType().Name, gameObject);
        isRefreshing = false;
    }

    protected void ResetDoneCount() => doneAnimating = 0;
    #endregion

#if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        if (prevGroupID != uIAnimatorGroupID)
        {
            prevGroupID = uIAnimatorGroupID;
            uIAnimatorGroup = null;
        }

        if (uIAnimatorGroup == null)
        {
            UIAnimatorGroup[] groups = GetComponentsInParent<UIAnimatorGroup>(true);

            foreach (var group in groups)
            {
                if (group.UIAnimatorGroupID != null && uIAnimatorGroupID == group.UIAnimatorGroupID)
                {
                    uIAnimatorGroup = group;
                    break;
                }
            }
        }

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (AnimationContainsFade())
        {
            if (canvasGroup == null)
            {
                CanvasGroup group = GetComponent<CanvasGroup>();

                if (group == null)
                {
                    group = gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup = group;
            }
        }
        else
        {
            if (canvasGroup != null)
            {
                DestroyImmediate(canvasGroup);
                canvasGroup = null; 
            }
        }
    }

    bool AnimationContainsFade()
    {
        if (animationGroups.Count == 0) return false;

        foreach (UIAnimationGroup group in animationGroups)
        {
            if (group == null) continue;

            foreach (UIAnimationBase animBase in group.Animations)
            {
                if (animBase == null) continue;

                if (animBase is FadeAnimation)
                    return true;
            }
        }

        return false;
    }
#endif
}