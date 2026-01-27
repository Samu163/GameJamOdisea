using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInputsHUD : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text player1Text;
    [SerializeField] private TMP_Text player2Text;

    [Header("Configuration")]
    [SerializeField] private string actionName = "Alargar"; 
    [SerializeField] private string label = "Estirar"; // The text shown next to icon


    private void Update()
    {
        UpdatePlayerPrompts();
    }

    private void UpdatePlayerPrompts()
    {
        // find everyone currently joined
        foreach (var player in PlayerInput.all)
        {
            // index1 = p1 , index2 = p2 
            TMP_Text targetText = (player.playerIndex == 1) ? player1Text : player2Text;

            if (targetText == null) continue;

            // Get the Action
            InputAction action = player.actions[actionName];

            // Get the Device
            InputDevice device = null;
            if (player.devices.Count > 0) device = player.devices[0];

            // Get the Icon Name 
            string iconName = "key/unknown";
            if (action != null && device != null)
            {
                iconName = InputIconMapper.GetIconForAction(action, device);
            }

            //  Update the Text
            targetText.text = $"<sprite name=\"{iconName}\"> {label}";
        }
    }
}