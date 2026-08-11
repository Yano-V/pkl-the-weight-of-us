using UnityEngine;
using VContainer;

public class PlayerMediator : MonoBehaviour
{
    private IInputService iIS;

    [Inject]
    public void Construct(IInputService inputService)
    {
        iIS = inputService;
    }
}
