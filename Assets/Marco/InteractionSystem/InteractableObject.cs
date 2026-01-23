using UnityEngine;

public interface IInteractableObject
{
    // Enables interaction
    public void Activate()
    {

    }

    public void Interact(Interactor interactor)
    {

    }

    // Disables interaction
    public void Deactivate()
    {

    }

}

public enum ACTIVE_STATE
{
    ON,
    OFF
}