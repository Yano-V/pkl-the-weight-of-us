public class InGameMainGameState : FiniteMainGameState
{
    public InGameMainGameState(IGameServiceFiniteManageable iGSFM) : base(iGSFM) { }

    public override void Enter()
    {
        iGSFM.EnterMainGameStateEnum(EnumMainGameState.InGame);
    }

    public override void Exit()
    {
        iGSFM.ExitMainGameStateEnum();
    }

    public override void Update()
    {
        
    }
}