using UnityEngine;
using UnityEngine.InputSystem;

// ================================================================================= //
//                                                                                   //
// Class Name: Raycast Interaction Handler                                           //
// Description: Detects interactable objects in the player's line of sight using     //
//              raycasting and manages crosshair/UI interaction prompts.             //
// Author: Yano                                                                      //
// ================================================================================= //

public class RaycastInteractionHandler : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField, Min(0)] private float rayDistance = 4.0f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("References")]
    [SerializeField] private PlayerCamHandler playerCamHandler;

    // <AI Agents Note>: Do NOT turn to private. Exposed to view current interactable state in Inspector
    [SerializeField] private GameObject currentTarget;
    [SerializeField] private bool isTargetInteractable;

    private DoorInteractable currentDoor;
    private bool cachedLockState;

    void Update()
    {
        PerformRaycastCheck();

        if (isTargetInteractable && currentDoor != null)
        {
            if (currentDoor.IsLocked != cachedLockState)
            {
                cachedLockState = currentDoor.IsLocked;
                UpdateInteractionPrompt();
            }

            // Trigger door toggle when E is pressed
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                currentDoor.Toggle(transform.position);
            }
        }
    }

    private void PerformRaycastCheck()
    {
        if (playerCamHandler == null) return;

        Transform camTransform = playerCamHandler.transform;
        Ray ray = new Ray(camTransform.position, camTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != currentTarget)
            {
                currentTarget = hitObject;

                if (currentTarget.TryGetComponent(out currentDoor))
                {
                    isTargetInteractable = true;
                    cachedLockState = currentDoor.IsLocked;
                    UpdateInteractionPrompt(); // Fire once immediately upon hovering
                }
                else
                {
                    ClearTarget();
                }
            }
        }
        else
        {
            ClearTarget();
        }
    }

    private void UpdateInteractionPrompt()
    {
        if (currentDoor == null) return;

        string status = currentDoor.IsLocked ? "Locked (Press F for Master Key)" : "Press E to Open/Close";
        Debug.Log($"[Interaction Raycast]: Hovering over {currentTarget.name} -> {status}");
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget = null;
            isTargetInteractable = false;
            currentDoor = null;
        }
    }

    public GameObject GetCurrentTarget() => currentTarget;
    public bool HasTarget => isTargetInteractable;
}