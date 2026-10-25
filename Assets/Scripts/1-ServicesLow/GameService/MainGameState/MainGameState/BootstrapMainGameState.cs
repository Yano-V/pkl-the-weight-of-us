public class BootstrapMainGameState : FiniteMainGameState
{
    public BootstrapMainGameState(IGameServiceFiniteManageable iGSFM) : base(iGSFM) { }

    public override void Enter()
    {
        iGSFM.EnterMainGameStateEnum(EnumMainGameState.Bootstrap);
    }

    public override void Exit()
    {
        iGSFM.ExitMainGameStateEnum();
    }

    public override void Update()
    {
        
    }
}