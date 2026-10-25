public readonly struct AnimationDoneData
{
    public string AnimatorGroupID { get; }
    public string AnimatorID { get; }
    public string AnimationGroupID { get; }
    public string AnimationID { get; }

    public bool SignalsSceneTransitionCloseAnimationDone { get; }

    public UIAnimType UIAnimType { get; }

    public AnimationDoneData(string animatorGroupID,
                             string animatorID,
                             string animationID,
                             string animationGroupID,
                             bool signalsSceneTransitionCloseAnimationDone,
                             UIAnimType uIAnimType)
    {
        AnimatorGroupID = animatorGroupID;
        AnimatorID = animatorID;
        AnimationGroupID = animationGroupID;
        AnimationID = animationID;
        SignalsSceneTransitionCloseAnimationDone = signalsSceneTransitionCloseAnimationDone;
        UIAnimType = uIAnimType;
    }
}