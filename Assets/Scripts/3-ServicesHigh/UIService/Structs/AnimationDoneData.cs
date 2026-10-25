public readonly struct AnimationDoneData
{
    public string AnimatorGroupID { get; }
    public string AnimatorID { get; }
    public string AnimationGroupID { get; }
    public string AnimationID { get; }

    public bool SendCloseAnimationDoneFlag { get; }

    public UIAnimType UIAnimType { get; }

    public AnimationDoneData(string animatorGroupID,
                             string animatorID,
                             string animationID,
                             string animationGroupID,
                             bool sendCloseAnimationDoneFlag,
                             UIAnimType uIAnimType)
    {
        AnimatorGroupID = animatorGroupID;
        AnimatorID = animatorID;
        AnimationGroupID = animationGroupID;
        AnimationID = animationID;
        SendCloseAnimationDoneFlag = sendCloseAnimationDoneFlag;
        UIAnimType = uIAnimType;
    }
}