using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform handleTransform;

    [Header("Configuración de Agarre")]
    [SerializeField] private bool canBeGrabbedByPlayer1 = true;
    [SerializeField] private bool canBeGrabbedByPlayer2 = true;

    [Header("Detección")]
    [SerializeField] private float detectionRadius = 0.5f;
    [Tooltip("Radio para detectar cuando el jugador toca el asa")]

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = Color.cyan;

    // Estado actual
    private PlayerAlargar currentGrabbingPlayer;
    private bool isBeingGrabbed;

    public bool IsBeingGrabbed => isBeingGrabbed;
    public PlayerAlargar GrabbingPlayer => currentGrabbingPlayer;

    private void Awake()
    {
        if (handleTransform == null)
        {
            handleTransform = transform;
        }
    }

    public bool TryGrab(PlayerAlargar player, int playerIndex)
    {
        if (playerIndex == 1 && !canBeGrabbedByPlayer1) return false;
        if (playerIndex == 2 && !canBeGrabbedByPlayer2) return false;

        if (isBeingGrabbed) return false;

        if (!IsPlayerCloseEnough(player))
        {
            return false;
        }

        GrabPlayer(player);
        return true;
    }
    private bool IsPlayerCloseEnough(PlayerAlargar player)
    {
        Transform playerHead = GetPlayerHead(player);
        if (playerHead == null) return false;

        float distance = Vector3.Distance(playerHead.position, handleTransform.position);
        return distance <= detectionRadius;
    }

    private void GrabPlayer(PlayerAlargar player)
    {
        currentGrabbingPlayer = player;
        isBeingGrabbed = true;
        player.OnGrabbedObject(this);
        Debug.Log($"[GrabbableBox] {player.gameObject.name} ha agarrado {gameObject.name}");
    }
    public void ReleasePlayer()
    {
        if (currentGrabbingPlayer != null)
        {
            Debug.Log($"[GrabbableBox] {currentGrabbingPlayer.gameObject.name} ha soltado {gameObject.name}");

            // Notificar al jugador que ha sido soltado
            currentGrabbingPlayer.OnReleasedObject();
            currentGrabbingPlayer = null;
        }

        isBeingGrabbed = false;
    }
    private Transform GetPlayerHead(PlayerAlargar player)
    {
        var headField = typeof(PlayerAlargar).GetField("head",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (headField != null)
        {
            GameObject headObj = headField.GetValue(player) as GameObject;
            return headObj?.transform;
        }

        return null;
    }
    public Vector3 GetHandlePosition()
    {
        return handleTransform.position;
    }
}