using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public UnityEvent onInteractionLockMovement;
    public UnityEvent onInteractionUnlockMovement;

    public IInteractableObject currentIntreactable = null;
    public StateMachine playerStateMachine;
    private void OnTriggerEnter(Collider other)
    {
         IInteractableObject interactable = other.GetComponentInParent<IInteractableObject>();
        if (interactable != null)
        {
            currentIntreactable = interactable;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        IInteractableObject interactable = other.GetComponentInParent<IInteractableObject>();
        if (interactable != null)
        {
            currentIntreactable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractableObject interactable = other.GetComponentInParent<IInteractableObject>();
        if (interactable != null && interactable == currentIntreactable)
        {
            currentIntreactable = null;
        }
    }

    // Connected thru the PlayerInput component
    public void TryToInteract()
    {
        if (IsCarryingInteractable())
        {
            playerStateMachine.ChangeState(playerStateMachine.CurrentState ,"state");
            // Drop interactable
        }
        else if (currentIntreactable != null) // Lever is failing sometimes here? currentInteractable = null??
        {
            currentIntreactable.Interact(this);
        }
    }

    public bool IsCarryingInteractable()
    {
        return playerStateMachine.CurrentState is CarryingState;
    }

    public void Deactivate()
    {
        
    }

}
