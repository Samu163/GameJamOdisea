using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class InteractableArea : MonoBehaviour
{

    private IInteractableObject interactableObject;

    public List<Collider> activeInteractors = new List<Collider>();

    private void Awake()
    {
        interactableObject = GetComponentInParent<IInteractableObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactor"))
        {
            print(other.gameObject.name + " entered interactable area of " + gameObject.name);
            activeInteractors.Add(other);
            OnInteractorListChanged();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactor"))
        {
            print(other.gameObject.name + " exited interactable area of " + gameObject.name);
            activeInteractors.Remove(other);
            OnInteractorListChanged();
        }
    }

    private void FixedUpdate()
    {
        // Check for interactors that may have been disabled to remove them
        for (int i = 0; i < activeInteractors.Count; i++)
        {
            if (activeInteractors[i].enabled == false)
            {
                print(activeInteractors[i].gameObject.name + " was deactivated while in interactable area of " + gameObject.name);
                activeInteractors.RemoveAt(i);
                OnInteractorListChanged();
            }
        }
    }

    void OnInteractorListChanged()
    {
        if(activeInteractors.Count > 0)
        {
            interactableObject.Activate();
        }
        else
        {
            interactableObject.Deactivate();
        }
    }

}
