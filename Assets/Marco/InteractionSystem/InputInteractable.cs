using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputInteractable : MonoBehaviour , IInteractableObject
{
    public InputAction interactableAction;
    public InputActionReference TEST_interactableAction;
    private Billboard interactorBillboard;
    public ACTIVE_STATE active_state = ACTIVE_STATE.OFF;
    public UnityEvent on_interact;

    private InputPromptUI promptUI;

    protected virtual void Awake()
    {
        promptUI = GetComponentInChildren<InputPromptUI>(true); // Set the text's icon to the action we listen to
        interactorBillboard = GetComponentInChildren<Billboard>();

        // Handle initial state
        switch (active_state)
        {
            case ACTIVE_STATE.ON:
                Activate();
                break;
            case ACTIVE_STATE.OFF:
                Deactivate();
                break;
        }
    }

    // Enables interaction
    public virtual void Activate()
    {
        // Test to see if the action works when dynamically overriden
        GetComponentInChildren<InputPromptUI>(true).action = TEST_interactableAction;

        interactorBillboard.gameObject.SetActive(true);
        active_state = ACTIVE_STATE.ON;
    }
    public void OnPlayerEnterRange(PlayerInput playerInput)
    {
        // 1. DYNAMICALLY fetch the action from this specific player instance
        // "Interact" must match the name in your Input Actions file exactly
        InputAction playerSpecificAction = playerInput.actions["Interact"];

        // 2. Pass this live action to your UI
        if (promptUI != null)
        {
            // The UI will now look at P1's specific gamepad/keyboard bindings
            promptUI.action = playerSpecificAction;
        }
    }


    public virtual void Interact(Interactor interactor)
    { // Received the interactor from the player who interacted with self

        on_interact?.Invoke();
        print("You are interacting with " + gameObject.name + "!");
    }

    // Disables interaction
    public virtual void Deactivate()
    {
        interactorBillboard.gameObject.SetActive(false);
        active_state = ACTIVE_STATE.OFF;
    }
}


