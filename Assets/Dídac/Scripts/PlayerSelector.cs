using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelector : MonoBehaviour
{
    [Header("Configuración")]
    public PlayerInputManager playerInputManager;
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public InputAction joinAction;

    private int numPlayers = 0;

    private void OnEnable()
    {
        if (joinAction.bindings.Count == 0)
            joinAction.AddBinding("<Gamepad>/buttonSouth");

        joinAction.performed += OnJoinActionPerformed;
        joinAction.Enable();
    }

    private void OnDisable()
    {
        joinAction.performed -= OnJoinActionPerformed;
        joinAction.Disable();
    }

    private void OnJoinActionPerformed(InputAction.CallbackContext context)
    {

        InputDevice device = context.control.device;

        if (numPlayers == 0)
        {
            numPlayers++;
            playerInputManager.playerPrefab = player1Prefab;
            playerInputManager.JoinPlayer(numPlayers, -1, "Gamepad", device);
        }
        else if (numPlayers == 1)
        {
            numPlayers++;
            playerInputManager.playerPrefab = player2Prefab;
            playerInputManager.JoinPlayer(numPlayers, -1, "Gamepad", device);
        }


    }
}
