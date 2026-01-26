using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputInteractable : MonoBehaviour, IInteractableObject
{
    private Billboard interactorBillboard;
    public ACTIVE_STATE active_state = ACTIVE_STATE.OFF;
    public UnityEvent on_interact;
    private InputPromptUI promptUI;

    #region Lifecycle
    protected virtual void Awake()
    {
        promptUI = GetComponentInChildren<InputPromptUI>(true);
        interactorBillboard = GetComponentInChildren<Billboard>(true);

        if (active_state == ACTIVE_STATE.ON) Activate();
        else Deactivate();
    }

    public virtual void Activate()
    {
        if (active_state == ACTIVE_STATE.ON) return;

        interactorBillboard.gameObject.SetActive(true);
        active_state = ACTIVE_STATE.ON;
    }

    public virtual void Deactivate()
    {
        if (active_state == ACTIVE_STATE.OFF) return;

        interactorBillboard.gameObject.SetActive(false);
        active_state = ACTIVE_STATE.OFF;
    }

    #endregion

    // Called by interaction area when a player enters it to setup the prompt UI
    public void OnPlayerEnterRange(PlayerInput playerInput)
    {
        InputAction playerAction = playerInput.actions["Interact"];

        // Get the player's device
        InputDevice device = null;
        if (playerInput.devices.Count > 0)
        {
            device = playerInput.devices[0];
        }

        // Send action & device to the promptUI
        if (promptUI != null && device != null)
        {
            promptUI.Setup(playerAction, device);
        }
    }

    // Called by Interactor's TryToInteract() which gets called by the player's input
    public virtual void Interact(Interactor interactor)
    {
        on_interact?.Invoke();
        // Debug.Log("Interact called on " + gameObject.name);
    }


}