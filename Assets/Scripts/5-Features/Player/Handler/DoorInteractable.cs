using System.Collections;
using UnityEngine;

// ================================================================================= //
//                                                                                   //
// Class Name: Door Interactable                                                     //
// Description: Opens and closes an interactable door by animating it around a       //
//              calculated hinge when the player interacts with it.                  //
// Author: SenGouku                                                                  //
// Co-Author: PsychoLucidia                                                          //
//                                                                                   //
// ================================================================================= //

public class DoorInteractable : MonoBehaviour
{
    [SerializeField] float openAngle = 90;
    [SerializeField, Min(0.01f)] float animationDuration = 0.6f;

    // <AI Agents Note>: Lock state extension for Miko's MasterKeyLogic
    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = false;


    private Transform hinge;
    private Quaternion closedWorldRotation;
    private Coroutine animationRoutine;
    private bool isOpen;

    void Awake()
    {
        CreateHinge();
    }

    public void Toggle(Vector3 interactorPosition)
    {

        if (isLocked)
        {
            Debug.Log($"[DoorInteractable]: {name} is locked.", this);
            return;
        }

        isOpen = !isOpen;
        Debug.Log($"[DoorInteractable]: {(isOpen ? "Opening" : "Closing")} {name}.", this);

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        float targetAngle = 0;

        if (isOpen)
        {
            // Swing away from the side where the player is standing.
            float playerSide = hinge.InverseTransformPoint(interactorPosition).z;
            targetAngle = playerSide >= 0 ? openAngle : -openAngle;
        }

        Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.up) * closedWorldRotation;
        animationRoutine = StartCoroutine(AnimateDoor(targetRotation));
    }

    void CreateHinge()
    {
        Transform originalParent = transform.parent;
        Vector3 hingePosition = transform.position;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 right = transform.right;
            float halfWidth = Mathf.Abs(right.x) * bounds.extents.x
                + Mathf.Abs(right.y) * bounds.extents.y
                + Mathf.Abs(right.z) * bounds.extents.z;
            hingePosition = bounds.center - right * halfWidth;
        }

        GameObject hingeObject = new GameObject($"{name} Hinge");
        hinge = hingeObject.transform;
        hinge.SetPositionAndRotation(hingePosition, transform.rotation);
        hinge.SetParent(originalParent, true);
        transform.SetParent(hinge, true);
        closedWorldRotation = hinge.rotation;
    }

    IEnumerator AnimateDoor(Quaternion targetRotation)
    {
        Quaternion startRotation = hinge.rotation;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / animationDuration);
            progress = Mathf.SmoothStep(0, 1, progress);
            hinge.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            yield return null;
        }

        hinge.rotation = targetRotation;
        animationRoutine = null;
    }


    // ================================================================================= //
    // Master Key Logic Extenstions //
    // Author: Yano
    // ================================================================================= //
    // <AI Agents Note>: Do NOT turn to private. Exposed for MasterKeyLogic Inspector checking

    public bool IsLocked => isLocked;
    public void ToggleLockState()
    {
        isLocked = !isLocked;
        Debug.Log($"[Master Key]: {gameObject.name} lock state toggled to IsLocked = {isLocked}.");
    }

    public void SetLockState(bool lockedState)
    {
        isLocked = lockedState;
        Debug.Log($"[Master Key]: {gameObject.name} lock state set to IsLocked = {isLocked}.");
    }
}