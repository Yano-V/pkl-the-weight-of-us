using UnityEngine;

public abstract class FiniteMainGameState
{
    protected IGameServiceFiniteManageable iGSFM;

    public FiniteMainGameState(IGameServiceFiniteManageable iGSFM)
    {
        this.iGSFM = iGSFM;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}
