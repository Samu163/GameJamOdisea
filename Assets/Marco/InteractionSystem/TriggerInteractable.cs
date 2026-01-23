using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// A class for interactors that activate automatically when a player enters an area (Trigger)
// and may also deactivate automatically when leaving said area
public class TriggerInteractable : MonoBehaviour, IInteractableObject
{
    public ACTIVE_STATE active_state = ACTIVE_STATE.OFF;
    public UnityEvent on_interact;

    public virtual void Activate()
    {

    }

    public virtual void Interact(Interactor interactor)
    {

    }

    // Disables interaction
    public virtual void Deactivate()
    {

    }

}
