using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class InputPromptUI : MonoBehaviour
{
    public TMP_Text text;
    public InputActionReference action;

    private void OnEnable()
    {
        if (InputWatcher.Instance != null)
            InputWatcher.Instance.OnInputTypeChanged += OnInputTypeChanged;

        UpdateText(); // initial update
    }

    private void OnDisable()
    {
        if (InputWatcher.Instance != null)
            InputWatcher.Instance.OnInputTypeChanged -= OnInputTypeChanged;
    }

    private void OnInputTypeChanged(InputType type, InputDevice device)
    {
        UpdateText();
    }

    private void Update()
    {
        // Optional: constantly update in editor/play mode in case bindings change
        if (action != null && text != null)
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (action == null || text == null || InputWatcher.Instance == null)
            return;

        var device = InputWatcher.Instance.LastDevice;

        // Get the correct icon for this action on the current device
        string iconName = InputIconMapper.GetIconForAction(action.action, device);

        // Set TMP text with sprite tag
        text.text = $"Press <sprite name=\"{iconName}\">";
    }
}
