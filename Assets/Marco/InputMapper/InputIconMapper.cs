using UnityEngine.InputSystem;

public static class InputIconMapper
{

    public static string GetIconName(InputControl control)
{
    if (control == null)
        return "space";

    var controller = ControllerMapper.GetControllerType(control.device);
    var name = control.name;

    if (controller != ControllerType.KeyboardMouse)
    {
        // Controller mapping (what we already have)
        return controller switch
        {
            ControllerType.PlayStation => name switch
            {
                "buttonSouth" => "ps_cross",
                "buttonEast" => "ps_circle",
                "buttonWest" => "ps_square",
                "buttonNorth" => "ps_triangle",
                _ => "ps_unknown"
            },
            ControllerType.Xbox => name switch
            {
                "buttonSouth" => "xbox_a",
                "buttonEast" => "xbox_b",
                "buttonWest" => "xbox_x",
                "buttonNorth" => "xbox_y",
                _ => "xbox_unknown"
            },
            ControllerType.Switch => name switch
            {
                "buttonSouth" => "switch_b",
                "buttonEast" => "switch_a",
                _ => "switch_unknown"
            },
            _ => "space"
        };
    }

    // Keyboard mapping
    return name switch
    {
        "space" => "key/space",
        "enter" => "key/enter",
        "tab" => "key/tab",
        "escape" => "key/esc",
        "backspace" => "key/backspace",
        "shift" => "key/shift",
        "ctrl" => "key/ctrl",
        "alt" => "key/alt",
        "meta" => "key/meta",
        "leftArrow" => "key/arrow_left",
        "rightArrow" => "key/arrow_right",
        "upArrow" => "key/arrow_up",
        "downArrow" => "key/arrow_down",
        "a" => "key/a",
        "b" => "key/b",
        "c" => "key/c",
        "e" => "e",
        // …continue for all keys you want…
        _ => "key/unknown"
    };
}


}

