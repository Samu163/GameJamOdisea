using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType
{
    KeyboardMouse,
    Controller
}

public class InputWatcher : MonoBehaviour
{
    public static InputControl LastUsedControl { get; private set; }

    public static InputWatcher Instance { get; private set; }

    public static event Action<InputType, InputDevice> InputTypeChanged;

    InputType lastInputType;
    InputDevice lastDevice;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }


    void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed)
            return;

        var action = obj as InputAction;
        if (action?.activeControl == null)
            return;

        LastUsedControl = action.activeControl;

        var device = LastUsedControl.device;

        InputType newType =
            device is Keyboard || device is Mouse
                ? InputType.KeyboardMouse
                : InputType.Controller;

        if (newType != lastInputType || device != lastDevice)
        {
            lastInputType = newType;
            lastDevice = device;
            InputTypeChanged?.Invoke(newType, device);
        }
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {

        if (change == InputDeviceChange.Disconnected)
        {
            if (Gamepad.all.Count == 0)
            {
                lastInputType = InputType.KeyboardMouse;
                lastDevice = null;
                InputTypeChanged?.Invoke(lastInputType, null);
            }
        }
    }

    public InputType GetLastInputType() => lastInputType;
    public InputDevice GetLastDevice() => lastDevice;
}
