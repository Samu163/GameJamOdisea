using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
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
            playerStateMachine.ChangeState( playerStateMachine.CurrentState ,"state");
            // Drop interactable
        }
        else if (currentIntreactable != null)
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
