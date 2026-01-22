using System.Collections.Generic;
using UnityEngine.InputSystem;

public static class InputIconMapper
{
    #region Input Texture Mappings

    // ===== Keyboard & Mouse icons =====
    private static readonly Dictionary<string, string> keyboardIcons = new()
    {
        {"space", "key/space"},
        {"enter", "key/enter"},
        {"tab", "key/tab"},
        {"escape", "key/esc"},
        {"backspace", "key/backspace"},
        {"shift", "key/shift"},
        {"ctrl", "key/ctrl"},
        {"alt", "key/alt"},
        {"meta", "key/meta"},
        {"leftArrow", "key/arrow_left"},
        {"rightArrow", "key/arrow_right"},
        {"upArrow", "key/arrow_up"},
        {"downArrow", "key/arrow_down"},
    };

    // Add letters A-Z and numbers 0-9
    static InputIconMapper()
    {
        // map all letter and number keys to values 'a' = 'a'

        for (char c = 'a'; c <= 'z'; c++)
            keyboardIcons.Add(c.ToString(),  c.ToString());
        for (int i = 0; i <= 9; i++)
            keyboardIcons.Add(i.ToString(),"key/" + i.ToString());
    }

    // ===== Xbox icons =====
    private static readonly Dictionary<string, string> xboxIcons = new()
    {
        {"buttonSouth", "xa"},
        {"buttonEast", "xb"},
        {"buttonWest", "xx"},
        {"buttonNorth", "xy"},
        {"leftShoulder", "lb"},
        {"rightShoulder", "rb"},
        {"leftTrigger", "lt"},
        {"rightTrigger", "rt"},
        {"leftStick", "xbox_l_stick"},
        {"rightStick", "xbox_r_stick"},
        {"start", "xbox_start"},
        {"select", "xbox_back"},
        {"dpad/up", "xbox_dpad_up"},
        {"dpad/down", "xbox_dpad_down"},
        {"dpad/left", "xbox_dpad_left"},
        {"dpad/right", "xbox_dpad_right"},
    };

    // ===== PlayStation icons =====
    private static readonly Dictionary<string, string> psIcons = new()
    {
        {"buttonSouth", "px"},
        {"buttonEast", "po"},
        {"buttonWest", "ps"},
        {"buttonNorth", "pt"},
        {"leftShoulder", "ps_lb"},
        {"rightShoulder", "ps_rb"},
        {"leftTrigger", "ps_lt"},
        {"rightTrigger", "ps_rt"},
        {"leftStick", "ps_l_stick"},
        {"rightStick", "ps_r_stick"},
        {"start", "ps_options"},
        {"select", "ps_share"},
        {"dpad/up", "ps_dpad_up"},
        {"dpad/down", "ps_dpad_down"},
        {"dpad/left", "ps_dpad_left"},
        {"dpad/right", "ps_dpad_right"},
    };

    // ===== Switch icons =====
    private static readonly Dictionary<string, string> switchIcons = new()
    {
        {"buttonSouth", "nb"},
        {"buttonEast", "na"},
        {"buttonWest", "nx"},
        {"buttonNorth", "ny"},
        {"leftShoulder", "switch_l"},
        {"rightShoulder", "switch_r"},
        {"leftTrigger", "switch_zl"},
        {"rightTrigger", "switch_zr"},
        {"leftStick", "switch_l_stick"},
        {"rightStick", "switch_r_stick"},
        {"start", "switch_plus"},
        {"select", "switch_minus"},
        {"dpad/up", "switch_dpad_up"},
        {"dpad/down", "switch_dpad_down"},
        {"dpad/left", "switch_dpad_left"},
        {"dpad/right", "switch_dpad_right"},
    };

#endregion

    // ----- Public API ------
    public static string GetIconForAction(UnityEngine.InputSystem.InputAction action, InputDevice device)
    {
        if (action == null || device == null) return "key/unknown";

        foreach (var binding in action.bindings)
        {
            if (binding.isPartOfComposite) continue;

            if (device is Keyboard || device is Mouse)
            {
                if (binding.path.Contains("Keyboard") || binding.path.Contains("Mouse"))
                    return MapPathToIcon(binding.path, InputType.KeyboardMouse, device);
            }
            else
            {
                if (binding.path.Contains("Gamepad"))
                    return MapPathToIcon(binding.path, InputType.Controller, device);
            }
        }

        return "key/unknown";
    }

    private static string MapPathToIcon(string bindingPath, InputType type, InputDevice device)
    {
        if (type == InputType.KeyboardMouse)
        {
            foreach (var kv in keyboardIcons)
            {

                string sanitizedKey = bindingPath.Replace("<Keyboard>/", "").Replace("<Mouse>/","");

                if (sanitizedKey.ToLower().Contains(kv.Key.ToLower()))
                    return kv.Value;
            }
            return "key/unknown";
        }
        else
        {
            var controller = ControllerMapper.GetControllerType(device);
            var icons = controller switch
            {
                ControllerType.Xbox => xboxIcons,
                ControllerType.PlayStation => psIcons,
                ControllerType.Switch => switchIcons,
                _ => null
            };
            if (icons != null)
            {
                foreach (var kv in icons)
                    if (bindingPath.ToLower().Contains(kv.Key.ToLower()))
                        return kv.Value;
            }
            return "key/unknown";
        }
    }
}
