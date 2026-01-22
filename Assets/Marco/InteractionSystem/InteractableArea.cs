using UnityEngine;

public class InteractableArea : MonoBehaviour
{

    private IInteractableObject interactableObject;

    private void Awake()
    {
        interactableObject = GetComponentInParent<IInteractableObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactor"))
        {
            interactableObject.Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactor"))
        {
            interactableObject.Deactivate();
        }
    }
    
}
