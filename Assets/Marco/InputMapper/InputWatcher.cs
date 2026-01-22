using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public enum InputType
{
    KeyboardMouse,
    Controller
}

public class InputWatcher : MonoBehaviour
{
    public static InputWatcher Instance { get; private set; }

    public InputDevice LastDevice { get; private set; }
    public InputType LastInputType { get; private set; }

    public event Action<InputType, InputDevice> OnInputTypeChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to any input
        InputSystem.onAnyButtonPress.Call(OnAnyInput);
    }

    private void OnAnyInput(InputControl control)
    {
        if (control == null) return;

        var device = control.device;
        var type = (device is Keyboard || device is Mouse) ? InputType.KeyboardMouse : InputType.Controller;

        if (device != LastDevice || type != LastInputType)
        {
            LastDevice = device;
            LastInputType = type;
            OnInputTypeChanged?.Invoke(type, device);
        }
    }
}
