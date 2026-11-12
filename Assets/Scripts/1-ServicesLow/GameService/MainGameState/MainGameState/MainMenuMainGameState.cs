public class MainMenuMainGameState : FiniteMainGameState
{
    public MainMenuMainGameState(IGameServiceFiniteManageable iGSFM) : base(iGSFM) { }

    public override void Enter()
    {
        iGSFM.EnterMainGameStateEnum(EnumMainGameState.MainMenu);
    }

    public override void Exit()
    {
        iGSFM.ExitMainGameStateEnum();
    }

    public override void Update()
    {
        
    }
}