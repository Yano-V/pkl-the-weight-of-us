using UnityEngine;

public class WaitUntilLoadingPageDone : CustomYieldInstruction
{
    public UIAnimType TypeToCompare { get; private set; }
    public UIAnimType CurrentType { get; set; }

    public override bool keepWaiting
    {
        get
        {
            LogUtilities.Log($"Waiting until Current Type is: {TypeToCompare}. (Currently {CurrentType}) ", GetType().Name);
            return TypeToCompare != CurrentType;
        }
    }

    public void PrepareConditions(UIAnimType uIAnimType)
    {
        TypeToCompare = uIAnimType;
        CurrentType = UIAnimType.None;
    }

    public override void Reset()
    {
        // do nothing
    }
}