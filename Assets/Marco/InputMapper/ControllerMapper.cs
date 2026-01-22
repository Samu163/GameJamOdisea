using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public enum ControllerType
{
    KeyboardMouse,
    Xbox,
    PlayStation,
    Switch
}

public static class ControllerMapper
{
    public static ControllerType GetControllerType(InputDevice device)
    {
        if (device == null)
            return ControllerType.KeyboardMouse;

        if (device is Keyboard || device is Mouse)
            return ControllerType.KeyboardMouse;

        if (device is DualShockGamepad)
            return ControllerType.PlayStation;

        if (device.layout != null && device.layout.Contains("Switch"))
            return ControllerType.Switch;

        return ControllerType.Xbox; // Default gamepad
    }
}

