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
            print(other.gameObject.name + " entered interactable area of " + gameObject.name);
            interactableObject.Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactor"))
        {
            print(other.gameObject.name + " exited interactable area of " + gameObject.name);
            interactableObject.Deactivate();
        }
    }
    
}
