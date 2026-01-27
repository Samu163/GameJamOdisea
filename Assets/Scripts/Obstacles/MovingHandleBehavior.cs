using UnityEngine;

[RequireComponent(typeof(GrabbableObject))]
public class MovingHandleBehavior : MonoBehaviour
{
    [Header("Movimiento del Asa")]
    [SerializeField] private bool moveOnXAxis = true;
    [SerializeField] private bool moveOnYAxis = false;
    [SerializeField] private bool moveOnZAxis = false;
    [SerializeField] private bool loopMovement = true;
    [Tooltip("Si es true, hace efecto boomerang (oscila). Si es false, va en una dirección hasta el límite y se detiene")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 5f;
    [Tooltip("Distancia que recorre el asa automáticamente")]

    [Header("Comportamiento al Soltar")]
    [SerializeField] private bool goBackToStart = true;
    [Tooltip("Si es false, el asa se queda donde fue soltada y continúa desde ahí")]
    [SerializeField] private float returnSpeed = 2f;
    [Tooltip("Velocidad de retorno a la posición inicial")]

    [Header("Límites")]
    [SerializeField] private bool useLimits = true;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;
    [SerializeField] private float minZ = -5f;
    [SerializeField] private float maxZ = 5f;

    private GrabbableObject grabbableObject;
    private Vector3 initialPosition;
    private Vector3 grabOffset;
    private float moveTimer = 0f;
    private bool isReturning = false;
    private Vector3 returnStartPosition;
    private float linearProgress = 0f; // Para movimiento lineal sin loop

    private void Awake()
    {
        grabbableObject = GetComponent<GrabbableObject>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (grabbableObject.IsBeingGrabbed)
        {
            // Si está siendo agarrada, cancelar retorno y mover normalmente
            isReturning = false;
            MoveHandle();
            MovePlayerWithHandle();
        }
        else if (isReturning && goBackToStart)
        {
            // Si debe volver, animar el retorno
            ReturnToInitialPosition();
        }
    }

    private void MoveHandle()
    {
        Vector3 newPosition = initialPosition;
        float offset;
        if (loopMovement)
        {
            // Movimiento oscilante (boomerang)
            moveTimer += Time.deltaTime * moveSpeed;
            offset = Mathf.Sin(moveTimer) * Mathf.Abs(moveDistance);
            if (moveDistance < 0) offset *= -1;
        }
        else
        {
            // Movimiento lineal en una dirección
            linearProgress += Time.deltaTime * moveSpeed;
            linearProgress = Mathf.Clamp01(linearProgress / Mathf.Abs(moveDistance)); // Normalizar entre 0 y 1
            offset = linearProgress * Mathf.Abs(moveDistance);
            if (moveDistance < 0) offset *= -1;
        }

        // Aplicar movimiento en los ejes habilitados
        if (moveOnXAxis)
        {
            newPosition.x += offset;
        }
        if (moveOnYAxis)
        {
            newPosition.y += offset;
        }
        if (moveOnZAxis)
        {
            newPosition.z += offset;
        }

        // Aplicar límites
        if (useLimits)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        }

        transform.position = newPosition;
    }

    private void ReturnToInitialPosition()
    {
        // Mover suavemente hacia la posición inicial
        transform.position = Vector3.MoveTowards(
            transform.position,
            initialPosition,
            returnSpeed * Time.deltaTime
        );

        // Si llegó a la posición inicial, detener el retorno
        if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
        {
            transform.position = initialPosition;
            isReturning = false;
            moveTimer = 0f; // Resetear el timer para que empiece desde el inicio
            linearProgress = 0f; // Resetear progreso lineal
            Debug.Log("[MovingHandle] Asa ha vuelto a su posición inicial");
        }
    }

    private void MovePlayerWithHandle()
    {
        PlayerAlargar player = grabbableObject.GrabbingPlayer;
        if (player == null) return;

        Transform playerHead = GetPlayerHead(player);
        if (playerHead == null) return;

        // Mover al JUGADOR para que siga al asa
        Vector3 targetPlayerPos = transform.position - grabOffset;
        player.transform.position = targetPlayerPos;
    }

    public void OnGrabbed()
    {
        PlayerAlargar player = grabbableObject.GrabbingPlayer;
        if (player == null) return;

        Transform playerHead = GetPlayerHead(player);
        if (playerHead == null) return;

        // Cancelar cualquier retorno en progreso
        isReturning = false;

        // Calcular offset: posición del asa - posición del jugador
        grabOffset = transform.position - player.transform.position;

        Debug.Log($"[MovingHandle] Asa agarrada! Offset: {grabOffset}, Timer actual: {moveTimer}");
    }

    public void OnReleased()
    {
        if (goBackToStart)
        {
            // Iniciar el retorno animado a la posición inicial
            isReturning = true;
            returnStartPosition = transform.position;
            Debug.Log("[MovingHandle] Asa soltada! Iniciando retorno a posición inicial");
        }
        else
        {
            // Se queda donde está, el timer mantiene su valor
            // y continuará desde ahí cuando se vuelva a agarrar
            Debug.Log($"[MovingHandle] Asa soltada! Se queda en posición actual. Timer: {moveTimer}");
        }
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

    private void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? initialPosition : transform.position;

        // Dibujar límites en X si está habilitado
        if (useLimits && moveOnXAxis)
        {
            Gizmos.color = Color.red;
            Vector3 minPoint = new Vector3(minX, start.y, start.z);
            Vector3 maxPoint = new Vector3(maxX, start.y, start.z);
            Gizmos.DrawLine(minPoint, maxPoint);
            Gizmos.DrawWireSphere(minPoint, 0.2f);
            Gizmos.DrawWireSphere(maxPoint, 0.2f);
        }

        // Dibujar límites en Y si está habilitado
        if (useLimits && moveOnYAxis)
        {
            Gizmos.color = Color.green;
            Vector3 minPoint = new Vector3(start.x, minY, start.z);
            Vector3 maxPoint = new Vector3(start.x, maxY, start.z);
            Gizmos.DrawLine(minPoint, maxPoint);
            Gizmos.DrawWireSphere(minPoint, 0.2f);
            Gizmos.DrawWireSphere(maxPoint, 0.2f);
        }

        // Dibujar límites en Z si está habilitado
        if (useLimits && moveOnZAxis)
        {
            Gizmos.color = Color.blue;
            Vector3 minPoint = new Vector3(start.x, start.y, minZ);
            Vector3 maxPoint = new Vector3(start.x, start.y, maxZ);
            Gizmos.DrawLine(minPoint, maxPoint);
            Gizmos.DrawWireSphere(minPoint, 0.2f);
            Gizmos.DrawWireSphere(maxPoint, 0.2f);
        }

        // Dibujar posición inicial
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(start, 0.3f);

        // Dibujar posición actual si está en ejecución
        if (Application.isPlaying)
        {
            Gizmos.color = isReturning ? Color.magenta : Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }
    }
}