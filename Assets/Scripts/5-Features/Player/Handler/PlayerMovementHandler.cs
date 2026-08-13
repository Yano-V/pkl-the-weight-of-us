using UnityEngine;
using VContainer;

// ================================================================================= //
//                                                                                   //
// Class Name: Player Movement Handler                                               //
// Description:                                                                      //
// Author: SenGouku                                                                  //
// Co-Author: PsychoLucidia                                                          //
//                                                                                   //
// ================================================================================= //

public class PlayerMovementHandler : MonoBehaviour
{
    private ICameraService iCS;

    [SerializeField, Min(0)] float moveSpeed = 5;

    [Inject]
    public void Construct(ICameraService cameraService)
    {
        iCS = cameraService;
    }

    public void MovePlayer(Vector2 value)
    {
        if (iCS == null || 
            iCS.CurrentActiveCam == null ||
            value.magnitude < 0.1f) return;

        Vector2 input = Vector2.ClampMagnitude(value, 1f);
        Quaternion yawRotation = Quaternion.Euler(0, iCS.CurrentActiveCam.CamRotationEulerAngles.y, 0);
        Vector3 direction = yawRotation * new Vector3(input.x, 0, input.y);

        transform.position += moveSpeed * Time.deltaTime * direction;
    }
}
