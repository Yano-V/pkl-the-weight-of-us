public class IntroMainGameState : FiniteMainGameState
{
    public IntroMainGameState(IGameServiceFiniteManageable iGSFM) : base(iGSFM) { }

    public override void Enter()
    {
        iGSFM.EnterMainGameStateEnum(EnumMainGameState.Intro);
    }

    public override void Exit()
    {
        iGSFM.ExitMainGameStateEnum();
    }

    public override void Update()
    {
        
    }
}