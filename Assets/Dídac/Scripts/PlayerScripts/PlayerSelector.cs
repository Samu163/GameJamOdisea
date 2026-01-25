using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
    [Header("Configuración")]
    public PlayerInputManager playerInputManager;
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player2KeyboardPrefab;
    public InputAction joinAction;
    

    private int numPlayers = 0;
    private readonly HashSet<int> pairedDeviceIds = new HashSet<int>();
    private bool bothPlayersKeyboard = false;

    [SerializeField] private Image player1Selected;
    [SerializeField] private Image player2Selected;

    private void Start()
    {
        GameManager.instance.playerSelector = this;
    }


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

        if (IsDeviceAlreadyPaired(device))
        {
            if (IsDeviceGamepad(device))
            {
                return;
            }
            else
            {
                bothPlayersKeyboard = true;
            }
        }

        pairedDeviceIds.Add(device.deviceId);

        if (numPlayers == 0)
        {
            numPlayers++;
            GameManager.instance.player1Device = device;
            player1Selected.color = Color.green;
            playerInputManager.playerPrefab = player1Prefab;
            playerInputManager.JoinPlayer(numPlayers, -1, "Gamepad", device);
        }
        else if (numPlayers == 1 && !bothPlayersKeyboard)
        {
            numPlayers++;
            GameManager.instance.player2Device = device;
            player2Selected.color = Color.green;
            playerInputManager.playerPrefab = player2Prefab;
            playerInputManager.JoinPlayer(numPlayers, -1, "Gamepad", device);
        }
        else if (numPlayers == 1 && bothPlayersKeyboard)
        {
            numPlayers++;
            GameManager.instance.player2Device = device;
            player2Selected.color = Color.green;
            playerInputManager.playerPrefab = player2KeyboardPrefab;
            playerInputManager.JoinPlayer(numPlayers, -1, "Keyboard", device);
        }

        if (numPlayers >= 2)
        {
            joinAction.Disable();
            StartCoroutine(StartGame());
        }

    }

    public void ResetLevelPlayers()
    {
        playerInputManager.playerPrefab = player1Prefab;
        playerInputManager.JoinPlayer(1, -1, "Gamepad", GameManager.instance.player1Device);

        if (IsDeviceGamepad(GameManager.instance.player2Device))
        {
            playerInputManager.playerPrefab = player2Prefab;
            playerInputManager.JoinPlayer(2, -1, "Gamepad", GameManager.instance.player2Device);
        }
        else
        {
            playerInputManager.playerPrefab = player2KeyboardPrefab;
            playerInputManager.JoinPlayer(2, -1, "Keyboard", GameManager.instance.player2Device);
        }
        StartCoroutine(LevelManager.instance.ResetLevel());
    }

    private bool IsDeviceAlreadyPaired(InputDevice device)
    {
        if (device == null) return false;
        return pairedDeviceIds.Contains(device.deviceId);
    }

    private bool IsDeviceGamepad(InputDevice device)
    {
        return device is Gamepad;
    }

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.25f);
        player1Selected.gameObject.SetActive(false);
        player2Selected.gameObject.SetActive(false);
        GameManager.instance.hasGameStarted = true;
    }
}
