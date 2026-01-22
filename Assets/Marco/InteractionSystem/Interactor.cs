using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public IInteractableObject currentIntreactable = null;

    private void OnTriggerEnter(Collider other)
    {
        IInteractableObject interactable = other.GetComponent<IInteractableObject>();
        if (interactable != null)
        {
            currentIntreactable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractableObject interactable = other.GetComponent<IInteractableObject>();
        if (interactable != null && interactable == currentIntreactable)
        {
            currentIntreactable = null;
        }
    }

    // Connected thru the PlayerInput component
    public void TryToInteract(InputAction.CallbackContext context)
    {
        if (context.performed && currentIntreactable != null)
        {
            currentIntreactable.Interact();
        }
    }

}
