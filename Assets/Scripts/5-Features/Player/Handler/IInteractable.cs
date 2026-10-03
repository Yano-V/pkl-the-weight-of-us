using UnityEngine;

// ================================================================================= //
//                                                                                   //
// Class Name: IInteractable                                                         //
// Description: Contract interface for all interactable objects within the game      //
//              world (Doors, Skateboard, Tarot Cards, Kitchen Stations).            //
// Author: Yano                                                                      //
//                                                                                   //
// ================================================================================= //

public interface IInteractable
{
    /// <summary>
    /// Executes primary interaction (e.g., Press 'E')
    /// </summary>
    void Interact(Transform interactorTransform);

    /// <summary>
    /// Returns current UI interaction prompt string (e.g., "Press E to Open")
    /// </summary>
    string GetInteractionPrompt();

    /// <summary>
    /// Returns whether object can currently be interacted with
    /// </summary>
    bool CanInteract();
}