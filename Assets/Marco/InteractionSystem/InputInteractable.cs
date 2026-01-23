using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputInteractable : MonoBehaviour , IInteractableObject
{
    public InputActionReference interactableAction;
    private Billboard interactorBillboard;
    public ACTIVE_STATE active_state = ACTIVE_STATE.OFF;
    public UnityEvent on_interact;

    protected virtual void Awake()
    {
        GetComponentInChildren<InputPromptUI>().action = interactableAction; // Set the text's icon to the action we listen to

        //interactableAction.action.Enable(); // Enable the input action
        //interactableAction.action.performed += _ => TryTointeract(); // when this input is pressed TryTointeract is called
        interactorBillboard = GetComponentInChildren<Billboard>();
    }

    // Enables interaction
    public void Activate()
    {
        interactorBillboard.gameObject.SetActive(true);
        active_state = ACTIVE_STATE.ON;
    }

    public virtual void Interact(Interactor interactor)
    { // Received the interactor from the player who interacted with self

        on_interact?.Invoke();
        print("You are interacting with " + gameObject.name + "!");
    }

    //private void TryTointeract()
    //{
    //    if (active_state == ACTIVE_STATE.OFF) return;
    //}

    // Disables interaction
    public void Deactivate()
    {
        interactorBillboard.gameObject.SetActive(false);
        active_state = ACTIVE_STATE.OFF;
    }
}


