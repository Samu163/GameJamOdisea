using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class InputPromptUI : MonoBehaviour
{
    public TMP_Text text;
    public InputActionReference action;

    void OnEnable()
    {
        InputWatcher.InputTypeChanged += OnInputTypeChanged;
    }

    void OnDisable()
    {
        InputWatcher.InputTypeChanged -= OnInputTypeChanged;
    }

    void OnInputTypeChanged(InputType type, InputDevice device)
    {
        string icon = InputIconMapper.GetIconName(InputWatcher.LastUsedControl);
        text.text = $"Press <sprite name=\"{icon}\">";
    }

}
