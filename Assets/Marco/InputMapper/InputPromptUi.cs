using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// A class for displaying input prompts with the correct icon for the current device
public class InputPromptUI : MonoBehaviour
{
    public TMP_Text text;

    private InputAction action;
    private InputDevice currentDevice;

    // Called from Interactable to set the action of the current player that entered it's interaction area
    public void Setup(InputAction newAction, InputDevice device)
    {
        action = newAction;
        currentDevice = device;
        UpdateText();
    }

    private void UpdateText()
    {
        if (action == null || text == null || currentDevice == null) return;

        // Pass the specific device (e.g. "Player 1's Gamepad"), so the Mapper 
        string iconName = InputIconMapper.GetIconForAction(action, currentDevice);
        // knows exactly which dictionary (Xbox vs PS vs Switch) to check.

        text.text = $"Pulsa <sprite name=\"{iconName}\">";
    }
}