using UnityEngine;
using VContainer;

// ================================================================================= //
//                                                                                   //
// Class Name: Player Movement Handler                                               //
// Description: Moves the player relative to the active camera using walk or run     //
//              speed based on the player's movement input.                         //
// Author: SenGouku                                                                  //
// Co-Author: PsychoLucidia                                                          //
//                                                                                   //
// ================================================================================= //

public class PlayerMovementHandler : MonoBehaviour
{
    private ICameraService iCS;

    [SerializeField, Min(0)] float moveSpeed = 5;
    [SerializeField, Min(0)] float runSpeed = 10;

    [Inject]
    public void Construct(ICameraService iCS)
    {
        this.iCS = iCS;
    }

    public void MovePlayer(Vector2 value, bool isRunning)
    {
        if (iCS == null)
        {
            return;
        }

        if (iCS.CurrentActiveCam == null)
        {
            return;
        }

        if (value.magnitude < 0.1f)
        {
            return;
        }

        Vector2 input = Vector2.ClampMagnitude(value, 1f);
        Quaternion yawRotation = Quaternion.Euler(0, iCS.CurrentActiveCam.CamRotationEulerAngles.y, 0);
        Vector3 inputDirection = new Vector3(input.x, 0, input.y);
        Vector3 moveDirection = yawRotation * inputDirection;

        float currentSpeed = moveSpeed;

        if (isRunning)
        {
            currentSpeed = runSpeed;
        }

        Vector3 movement = moveDirection * currentSpeed * Time.deltaTime;
        transform.position = transform.position + movement;
    }
}
