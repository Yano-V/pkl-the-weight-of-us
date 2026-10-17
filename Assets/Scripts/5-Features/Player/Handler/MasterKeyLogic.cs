using UnityEngine;
using UnityEngine.InputSystem;

// ================================================================================= //
//                                                                                   //
// Class Name: Master Key Logic                                                      //
// Description: Manages Miko's Master Key mechanic. Handles door detection, lock     //
//              overrides, and randomizing door states during loop resets.           //
// Author: Yano                                                                      //
// ================================================================================= //

public class MasterKeyLogic : MonoBehaviour
{
    const string DoorName = "m_single-flush 0762 x 2032mm";

    [SerializeField] private PlayerCamHandler playerCamHandler;
    [SerializeField, Min(0)] private float interactionDistance = 5f;
    [SerializeField] private bool hasMasterKey = true;

    // <AI Agents Note>: Do NOT turn to private. This is meant to display override activity for debugging
    [SerializeField] private bool isOverrideActive;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            AttemptMasterKeyOverride();
        }
    }

    public void AttemptMasterKeyOverride()
    {
        if (!hasMasterKey)
        {
            Debug.Log("[Master Key]: Miko does not possess the Master Key.");
            return;
        }

        if (playerCamHandler == null)
        {
            Debug.LogWarning("[Master Key]: PlayerCamHandler reference is missing.");
            return;
        }

        Transform cameraTransform = playerCamHandler.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 0.15f, interactionDistance);

        Transform closestDoor = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            Transform door = FindDoor(hit.transform);

            if (door != null && hit.distance < closestDistance)
            {
                closestDoor = door;
                closestDistance = hit.distance;
            }
        }

        if (closestDoor != null)
        {
            DoorInteractable door = closestDoor.GetComponent<DoorInteractable>();
            door ??= closestDoor.gameObject.AddComponent<DoorInteractable>();

            door.ToggleLockState();
            isOverrideActive = true;
            Debug.Log($"[Master Key]: Master key override applied to {closestDoor.name}.");
        }
    }

    private Transform FindDoor(Transform current)
    {
        while (current != null)
        {
            if (current.GetComponent<DoorInteractable>() != null
                || current.name.ToLowerInvariant().Contains(DoorName))
            {
                return current;
            }

            current = current.parent;
        }

        return null;
    }

    public void RandomizeDoorLocks(DoorInteractable[] roomDoors)
    {
        foreach (var door in roomDoors)
        {
            bool randomLockState = Random.value > 0.5f;
            door.SetLockState(randomLockState);
        }
    }

    public void SetMasterKeyPossession(bool state)
    {
        hasMasterKey = state;
    }
}