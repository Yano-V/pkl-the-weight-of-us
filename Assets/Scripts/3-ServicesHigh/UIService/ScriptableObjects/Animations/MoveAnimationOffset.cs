using UnityEngine;

[CreateAssetMenu(fileName = "Limbo Move Animation Offset", menuName = "Limbo/UI/UI Animations/Move Animation Offset")]
public class MoveAnimationOffset : UIAnimationBase
{
    private Vector2 initialPosition;
    private bool hasInitializedPosition = false;

    [SerializeField] Vector2 positionOffset;
    [SerializeField] Vector2 positionOffsetExit;
    [SerializeField] bool useExitPosition = false;

    public override void OpenAnimation()
    {
        uIAnimDoneType = UIAnimType.Open; 

        if (!hasInitializedPosition)
        {
            initialPosition = rectTransform.anchoredPosition;
            hasInitializedPosition = true;
        }

        rectTransform.anchoredPosition = initialPosition + positionOffset;

        if (WaitForOpenAnimationToFinish)
        {
            uIPageHandler.OnDonePlaying += HandleWaitAnimationDone;
            return;
        }

        OpenAnimationLogic();
    }

    public override void CloseAnimation()
    {
        uIAnimDoneType = UIAnimType.Close;

        if (WaitForCloseAnimationToFinish)
        {
            uIPageHandler.OnDonePlaying += HandleWaitAnimationDone;
            return;
        }

        CloseAnimationLogic();
    }

    void TweenAnchoredPosition(Vector2 value)
    {
        rectTransform.anchoredPosition = value;
    }

    protected override void OpenAnimationLogic()
    {
        float delayToUse = WaitForOpenAnimationToFinish ? 0f : delay;

        LTDescr tween = LeanTween.value
            (
                rectTransform.gameObject,
                initialPosition + positionOffset,
                initialPosition,
                duration
            )
            .setEase(leanTweenType)
            .setDelay(delayToUse)
            .setOnUpdateVector2(TweenAnchoredPosition)
            .setIgnoreTimeScale(ignoreTimeScale)
            .setOnComplete(() =>
            {
                if (!isLooper)
                {
                    MoveTweenID = -1;
                    SetAnimationDone();
                }
            });

        if (isLooper)
        {
            switch (repeatType)
            {
                case RepeatType.Repeat:
                    tween.setLoopClamp().setRepeat(-1);
                    break;
                case RepeatType.PingPong:
                    tween.setLoopPingPong(-1);
                    break;
                case RepeatType.None:
                default:
                    break;
            }
        }

        MoveTweenID = tween.id;
    }

    protected override void CloseAnimationLogic()
    {
        if (!hasInitializedPosition)
        {
            initialPosition = rectTransform.anchoredPosition;
            hasInitializedPosition = true;
        }

        float delayToUse = WaitForCloseAnimationToFinish ? 0f : delayExit;

        if (!useExitPosition)
        {
            MoveTweenID = LeanTween.value
                (
                    rectTransform.gameObject,
                    rectTransform.anchoredPosition,
                    initialPosition + positionOffset,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnUpdateVector2(TweenAnchoredPosition)
                .setOnComplete(() =>
                {
                    MoveTweenID = -1;
                    SetAnimationDone();
                })
                .setIgnoreTimeScale(ignoreTimeScale)
                .id;
        }
        else
        {
            MoveTweenID = LeanTween.value
                (
                    rectTransform.gameObject,
                    rectTransform.anchoredPosition,
                    initialPosition + positionOffsetExit,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnUpdateVector2(TweenAnchoredPosition)
                .setOnComplete(() =>
                {
                    MoveTweenID = -1;
                    SetAnimationDone();
                })
                .setIgnoreTimeScale(ignoreTimeScale)
                .id;
        }
    }

    public override void StopAnimation()
    {
        base.StopAnimation();

        uIPageHandler.OnDonePlaying -= HandleWaitAnimationDone;
    }
}