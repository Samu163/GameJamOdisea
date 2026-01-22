using UnityEngine;
using UnityEngine.InputSystem;

public class InputInteractable : MonoBehaviour , IInteractableObject
{
    public InputActionReference interactableAction;

    // Enables interaction
    public void Activate()
    {

    }

    public void Interact()
    {

    }

    // Disables interaction
    public void Deactivate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if collider is player
        if (other.CompareTag("Player"))
        {
            // Enable the input action
            //interactableAction.action.Enable();
        }
    }
}
