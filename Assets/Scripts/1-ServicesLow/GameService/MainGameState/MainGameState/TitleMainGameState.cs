public class TitleMainGameState : FiniteMainGameState
{
    public TitleMainGameState(IGameServiceFiniteManageable iGSFM) : base(iGSFM) { }

    public override void Enter()
    {
        iGSFM.EnterMainGameStateEnum(EnumMainGameState.Title);
    }

    public override void Exit()
    {
        iGSFM.ExitMainGameStateEnum();
    }

    public override void Update()
    {
        
    }
}