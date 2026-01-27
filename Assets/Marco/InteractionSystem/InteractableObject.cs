using UnityEngine;

public interface IInteractableObject
{
    // Enables interaction
    public void Activate()
    {

    }

    // Called by an interactor when it interacts with this object
    public void Interact(Interactor interactor)
    {

    }

    // Disables interaction
    public void Deactivate()
    {

    }

    // returns true or false depending on if the object if it's enabled for interaction
    public bool IsActive();

    // return if the interactable area should auto activate the interactable upon interactor getting close
    public bool AreaAutomaticActivation();

}

public enum ACTIVE_STATE
{
    ON,
    OFF
}