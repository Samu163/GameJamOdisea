using UnityEngine;
using UnityEngine.Events;

public class GrabbableObject : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform handleTransform;

    [Header("Configuración de Agarre")]
    [SerializeField] private bool canBeGrabbedByPlayer1 = true;
    [SerializeField] private bool canBeGrabbedByPlayer2 = true;

    [Header("Detección")]
    [SerializeField] private float detectionRadius = 0.5f;

    [Header("Física al Agarrar")]
    [SerializeField] private bool disableGravityWhenGrabbed = true;
    [SerializeField] private bool freezeRotationWhenGrabbed = true;

    [Header("Eventos")]
    [SerializeField] private UnityEvent onGrabbed;
    [SerializeField] private UnityEvent onReleased;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = Color.cyan;

    private PlayerAlargar currentGrabbingPlayer;
    private bool isBeingGrabbed;
    private Rigidbody rb;
    private bool originalGravity;
    private RigidbodyConstraints originalConstraints;

    public bool IsBeingGrabbed => isBeingGrabbed;
    public PlayerAlargar GrabbingPlayer => currentGrabbingPlayer;

    private void Awake()
    {
        if (handleTransform == null)
        {
            handleTransform = transform;
        }

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalGravity = rb.useGravity;
            originalConstraints = rb.constraints;
        }
    }

    public bool TryGrab(PlayerAlargar player, int playerIndex)
    {
        if (playerIndex == 1 && !canBeGrabbedByPlayer1) return false;
        if (playerIndex == 2 && !canBeGrabbedByPlayer2) return false;
        if (isBeingGrabbed) return false;

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

        // Aplicar configuración de física
        if (rb != null)
        {
            if (disableGravityWhenGrabbed)
            {
                rb.useGravity = false;
            }
            if (freezeRotationWhenGrabbed)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        onGrabbed?.Invoke();

        Debug.Log($"[GrabbableObject] {player.gameObject.name} ha agarrado {gameObject.name}");

        // NO LLAMAR A OnGrabbedObject aquí - el jugador ya maneja su estado en TryGrabObject
    }
    public void ReleasePlayer()
    {
        if (currentGrabbingPlayer != null)
        {
            Debug.Log($"[GrabbableObject] {currentGrabbingPlayer.gameObject.name} ha soltado {gameObject.name}");

            // Restaurar física
            if (rb != null)
            {
                rb.useGravity = originalGravity;
                rb.constraints = originalConstraints;
            }

            currentGrabbingPlayer.OnReleasedObject();
            currentGrabbingPlayer = null;
        }

        isBeingGrabbed = false;
        onReleased?.Invoke();
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

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Transform handle = handleTransform != null ? handleTransform : transform;
        Gizmos.color = isBeingGrabbed ? Color.green : gizmoColor;
        Gizmos.DrawWireSphere(handle.position, detectionRadius);
    }
}