using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAlargar : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("Alargar Settings")]
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject bottom;
    [SerializeField] private float alargarAmount = 0.05f;
    [SerializeField] private float maxRange = 9f;
    public bool isAlargarHeld = false;

    [Header("Jump Physics")]
    [SerializeField] private float jumpForceUp = 8f;
    [SerializeField] private float jumpForceForward = 3f;
    [SerializeField] private float jumpDelay = 0.1f;
    [SerializeField] private float jumpLockDuration = 0.3f;
    [SerializeField] private float jumpCooldown = 0.8f;

    [Header("Ground Check - Player 1")]
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Obstacle Check - Player 1 (Techo)")]
    [SerializeField] private float ceilingCheckDistance = 1f;
    [SerializeField] private float ceilingCheckRadius = 0.3f;
    [SerializeField] private Vector3 ceilingCheckOffset = new Vector3(0, 0, 0.5f);

    [Header("Wall Check - Player 2")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float wallCheckDistance = 2f;
    [SerializeField] private float wallCheckRadius = 0.8f;
    [SerializeField] private Vector3 wallCheckOffset = Vector3.zero;
    [SerializeField] private float player2GroundCheckDistance = 0.5f;
    [SerializeField] private float player2GroundCheckRadius = 0.3f;

    [Header("Obstacle Check - Player 2 (Muro frontal)")]
    [SerializeField] private float frontObstacleCheckDistance = 1f;
    [SerializeField] private float frontObstacleCheckRadius = 0.5f;
    [SerializeField] private Vector3 frontObstacleOffset = new Vector3(0, 0.5f, 0);

    [Header("Grabbable Objects")]
    [SerializeField] private float grabDetectionRadius = 0.6f;
    [Tooltip("Radio para detectar objetos agarrables")]

    // Estados
    private bool isRetracting = false;
    private bool isQuickRetracting = false;
    private bool isMovementLocked = false;
    private bool shouldJumpAfterRetract = false;
    public bool isJumping = false;
    private bool canInteract = true;

    // Estado de agarre
    private bool isGrabbingObject = false;
    private GrabbableObject currentGrabbedBox = null;

    private float alargarTimer = 0f;
    private const float alargarInterval = 0.1f;
    private float totalAlargar = 0f;

    // Posiciones iniciales
    private Vector3 initialBodyScale, targetBodyScale;
    private Vector3 initialBodyLocalPos, targetBodyPosition;
    private Vector3 initialHeadLocalPos, targetHeadPosition;
    private Vector3 initialBottomLocalPos, targetBottomPosition;

    private Rigidbody rb;
    private Vector3 releaseHeadWorldPos;

    // Cache para evitar cálculos repetidos
    private bool isPlayer1;
    private bool isPlayer2;
    private const float positionThreshold = 0.01f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Cache player index checks
        isPlayer1 = playerInput.playerIndex == 1;
        isPlayer2 = playerInput.playerIndex == 2;

        // Inicializar posiciones
        CacheInitialTransforms();

        // Setup level manager
        if (isPlayer1)
        {
            LevelManager.instance.player1 = this.gameObject;
        }
        else if (isPlayer2)
        {
            LevelManager.instance.player2 = this.gameObject;
        }
    }

    private void CacheInitialTransforms()
    {
        initialBodyScale = body.transform.localScale;
        targetBodyScale = initialBodyScale;
        initialBodyLocalPos = body.transform.localPosition;
        targetBodyPosition = initialBodyLocalPos;
        initialHeadLocalPos = head.transform.localPosition;
        targetHeadPosition = initialHeadLocalPos;
        initialBottomLocalPos = bottom.transform.localPosition;
        targetBottomPosition = initialBottomLocalPos;
    }

    public void Update()
    {
        // Estado de grounded - solo check para Player 2
        bool isGrounded = isPlayer2 ? IsPlayer2Grounded() : true;

        // Condiciones combinadas para claridad
        bool canExtend = !isRetracting && !isMovementLocked && !isJumping && isGrounded && canInteract;

        // EXTENSIÓN
        if (isAlargarHeld && canExtend)
        {
            // Si ya está agarrando algo, mantener el estado sin extender más
            if (isGrabbingObject)
            {
                HandleGrabbingState();
            }
            else if (totalAlargar < maxRange)
            {
                // Verificar obstáculos y objetos agarrables
                ObstacleCheckResult obstacleResult = CheckObstaclesAndGrabbables();

                if (obstacleResult.hasGrabbable)
                {
                    // Intentar agarrar el objeto
                    TryGrabObject(obstacleResult.grabbableBox);
                }
                else if (obstacleResult.hasObstacle)
                {
                    // Obstáculo normal - retracción rápida
                    Debug.Log("¡Obstáculo detectado! Cancelando extensión...");
                    ForceQuickRetract();
                }
                else
                {
                    // Extensión normal
                    HandleExtension();
                }
            }
            else
            {
                CancelAlargar();
            }
        }
        // Feedback cuando no se puede extender
        else if (isAlargarHeld && !canExtend)
        {
            HandleExtensionBlocked(isGrounded);
        }

        if (isRetracting)
        {
            HandleRetraction();
        }
    }

    #region Grabbable Object System

    private struct ObstacleCheckResult
    {
        public bool hasObstacle;
        public bool hasGrabbable;
        public GrabbableObject grabbableBox;
    }
    private ObstacleCheckResult CheckObstaclesAndGrabbables()
    {
        ObstacleCheckResult result = new ObstacleCheckResult();

        // Primero buscar objetos agarrables
        GrabbableObject grabbable = DetectGrabbableObject();
        if (grabbable != null)
        {
            result.hasGrabbable = true;
            result.grabbableBox = grabbable;
            return result; 
        }

        result.hasObstacle = HasObstacleInExtensionPath();
        return result;
    }

    private GrabbableObject DetectGrabbableObject()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(
            head.transform.position,
            grabDetectionRadius
        );

        foreach (Collider col in nearbyColliders)
        {
            if (col.transform.IsChildOf(transform)) continue;

            GrabbableObject grabbable = col.GetComponent<GrabbableObject>();
            if (grabbable != null && !grabbable.IsBeingGrabbed)
            {
                return grabbable;
            }
        }

        return null;
    }

    private void TryGrabObject(GrabbableObject grabbable)
    {
        if (grabbable.TryGrab(this, playerInput.playerIndex))
        {
            currentGrabbedBox = grabbable;
            isGrabbingObject = true;
            Debug.Log($"[Player {playerInput.playerIndex}] ¡Objeto agarrado!");
        }
    }
    private void HandleGrabbingState()
    {
        Debug.Log($"[Player {playerInput.playerIndex}] Manteniendo agarre...");
    }

    public void OnGrabbedObject(GrabbableObject box)
    {
        currentGrabbedBox = box;
        isGrabbingObject = true;
        isMovementLocked = true;
    }

    public void OnReleasedObject()
    {
        currentGrabbedBox = null;
        isGrabbingObject = false;
        isMovementLocked = false;

        if (totalAlargar > 0)
        {
            Debug.Log($"[Player {playerInput.playerIndex}] Objeto soltado - iniciando retracción");
            ForceQuickRetract();
        }
    }

    #endregion

    #region Extension Logic

    private void HandleExtension()
    {
        alargarTimer += Time.deltaTime;

        if (alargarTimer >= alargarInterval)
        {
            Alargar();
            alargarTimer = 0f;
        }

        LerpParts(targetBodyScale, targetBodyPosition, targetHeadPosition, targetBottomPosition, 0.1f);
    }

    private void HandleExtensionBlocked(bool isGrounded)
    {
        if (!isGrounded)
        {
            Debug.Log("No puedes estirarte en el aire!");
        }
        else if (!canInteract)
        {
            Debug.Log("Cooldown activo, espera...");
        }
        else if (isGrabbingObject)
        {
            Debug.Log("Manteniendo objeto agarrado...");
        }
    }

    public void Alargar()
    {
        Vector3 increment = new Vector3(0, alargarAmount * 0.5f, 0);
        targetBodyScale += increment;

        if (isPlayer1)
        {
            targetBodyPosition += new Vector3(0, 0, alargarAmount * 0.5f);
            targetHeadPosition += new Vector3(0, 0, alargarAmount);
        }
        else if (isPlayer2)
        {
            targetBodyPosition += increment;
            targetHeadPosition += new Vector3(0, alargarAmount, 0);
        }

        totalAlargar += alargarAmount;
    }

    #endregion

    #region Obstacle Detection

    private bool HasObstacleInExtensionPath()
    {
        if (isPlayer1)
        {
            return CheckCeilingAhead();
        }
        else if (isPlayer2)
        {
            return CheckFrontObstacle();
        }

        return false;
    }

    private bool CheckCeilingAhead()
    {
        Vector3 checkOrigin = head.transform.position + head.transform.TransformDirection(ceilingCheckOffset);
        Vector3 checkDirection = transform.forward;

        bool hasCeiling = Physics.SphereCast(
            checkOrigin,
            ceilingCheckRadius,
            checkDirection,
            out RaycastHit hitInfo,
            ceilingCheckDistance,
            groundLayer
        );

        if (hasCeiling)
        {
            // Verificar si es un objeto agarrable
            GrabbableObject grabbable = hitInfo.collider.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                return false; 
            }

            Debug.Log($"TECHO DETECTADO: {hitInfo.collider.name} a {hitInfo.distance:F2}m");
        }

        Debug.DrawRay(checkOrigin, checkDirection * ceilingCheckDistance, hasCeiling ? Color.red : Color.yellow, 0.1f);
        return hasCeiling;
    }

    private bool CheckFrontObstacle()
    {
        Vector3 checkOrigin = body.transform.position + body.transform.TransformDirection(frontObstacleOffset);
        Vector3 checkDirection = transform.up;

        bool hasObstacle = Physics.SphereCast(
            checkOrigin,
            frontObstacleCheckRadius,
            checkDirection,
            out RaycastHit hitInfo,
            frontObstacleCheckDistance,
            groundLayer
        );

        if (hasObstacle)
        {
            // Verificar si es un objeto agarrable
                GrabbableObject grabbable = hitInfo.collider.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                return false;
            }

            Debug.Log($"OBSTÁCULO FRONTAL DETECTADO: {hitInfo.collider.name} a {hitInfo.distance:F2}m");
        }

        Debug.DrawRay(checkOrigin, checkDirection * frontObstacleCheckDistance, hasObstacle ? Color.red : Color.yellow, 0.1f);
        return hasObstacle;
    }

    #endregion

    #region Retraction Logic

    private void HandleRetraction()
    {
        float lerpSpeed = isQuickRetracting ? 0.3f : 0.1f;
        LerpParts(initialBodyScale, initialBodyLocalPos, initialHeadLocalPos, initialBottomLocalPos, lerpSpeed);

        if (!isQuickRetracting)
        {
            MoveBodyToFollowHead();
        }

        if (IsRetractionComplete())
        {
            CompleteRetraction();
        }
    }

    private void MoveBodyToFollowHead()
    {
        Vector3 currentHeadWorld = head.transform.position;
        Vector3 delta = releaseHeadWorldPos - currentHeadWorld;

        if (delta.sqrMagnitude > 0f)
        {
            transform.position += delta;
        }
    }

    private bool IsRetractionComplete()
    {
        return Vector3.Distance(body.transform.localScale, initialBodyScale) < positionThreshold &&
               Vector3.Distance(head.transform.localPosition, initialHeadLocalPos) < positionThreshold;
    }

    private void CompleteRetraction()
    {
        ResetPositions();

        if (isPlayer2 && shouldJumpAfterRetract)
        {
            StartCoroutine(WallJumpRoutine());
        }

        isRetracting = false;
        shouldJumpAfterRetract = false;
    }

    public void CancelAlargar()
    {
        isAlargarHeld = false;

        if (totalAlargar <= 0f) return;

        // Si está agarrando algo, soltar primero
        if (isGrabbingObject && currentGrabbedBox != null)
        {
            currentGrabbedBox.ReleasePlayer();
            return;
        }

        bool canRetractNormal = DetermineRetractionType();

        isRetracting = true;
        totalAlargar = 0f;

        SetRetractionTargets();

        if (canRetractNormal)
        {
            isQuickRetracting = false;
            releaseHeadWorldPos = head.transform.position;
        }
        else
        {
            isQuickRetracting = true;
            shouldJumpAfterRetract = false;
        }
    }

    private void ForceQuickRetract()
    {
        isAlargarHeld = false;

        if (totalAlargar <= 0f) return;

        // Si está agarrando algo, soltar
        if (isGrabbingObject && currentGrabbedBox != null)
        {
            currentGrabbedBox.ReleasePlayer();
        }

        isRetracting = true;
        isQuickRetracting = true;
        shouldJumpAfterRetract = false;
        totalAlargar = 0f;

        SetRetractionTargets();
    }

    private bool DetermineRetractionType()
    {
        if (isPlayer1)
        {
            return CheckGroundUnderHead();
        }
        else if (isPlayer2)
        {
            bool hasWall = CheckWallInFront();

            if (hasWall && canInteract)
            {
                shouldJumpAfterRetract = true;
                Debug.Log("MURO DETECTADO - Salto preparado");
            }
            else
            {
                Debug.Log("Sin muro - Retracción rápida");
            }

            return hasWall;
        }

        return false;
    }

    private void SetRetractionTargets()
    {
        targetBodyScale = initialBodyScale;
        targetBodyPosition = initialBodyLocalPos;
        targetHeadPosition = initialHeadLocalPos;
        targetBottomPosition = initialBottomLocalPos;
    }

    #endregion

    #region Jump Logic

    private IEnumerator WallJumpRoutine()
    {
        Debug.Log("Iniciando salto!");

        SetJumpState(true);

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForceUp, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpDelay);

        rb.AddForce(transform.forward * jumpForceForward, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpLockDuration);

        SetJumpState(false);

        Debug.Log("Cooldown iniciado - NO puedes estirarte");
        yield return new WaitForSeconds(jumpCooldown);

        canInteract = true;
        Debug.Log("Cooldown terminado - Puedes estirarte de nuevo");
    }

    private void SetJumpState(bool jumping)
    {
        isMovementLocked = jumping;
        isJumping = jumping;
        canInteract = !jumping;
    }

    #endregion

    #region Ground and Wall Checks

    private bool CheckWallInFront()
    {
        if (!isPlayer2) return false;

        Vector3 checkOrigin = head.transform.position + head.transform.TransformDirection(wallCheckOffset);

        Collider[] hitColliders = Physics.OverlapSphere(checkOrigin, wallCheckRadius);

        foreach (Collider col in hitColliders)
        {
            if (col.transform.IsChildOf(transform)) continue;

            if (col.CompareTag(wallTag))
            {
                Debug.Log($"MURO ENCONTRADO (OverlapSphere): {col.name}");
                return true;
            }
        }

        if (Physics.SphereCast(checkOrigin, wallCheckRadius, transform.forward, out RaycastHit hitInfo, wallCheckDistance))
        {
            if (hitInfo.collider.CompareTag(wallTag))
            {
                Debug.Log($"MURO ENCONTRADO (SphereCast): {hitInfo.collider.name}");
                return true;
            }
        }

        Debug.Log("Sin muro delante");
        return false;
    }

    private bool CheckGroundUnderHead()
    {
        Vector3 rayOrigin = head.transform.position;
        bool hit = Physics.SphereCast(rayOrigin, groundCheckRadius, Vector3.down, out RaycastHit hitInfo, groundCheckDistance, groundLayer);

        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, hit ? Color.green : Color.red, 0.5f);

        return hit;
    }

    private bool IsPlayer2Grounded()
    {
        if (!isPlayer2) return true;

        Vector3 rayOrigin = bottom.transform.position;
        bool grounded = Physics.SphereCast(rayOrigin, player2GroundCheckRadius, Vector3.down, out RaycastHit hitInfo, player2GroundCheckDistance, groundLayer);

        Debug.DrawRay(rayOrigin, Vector3.down * player2GroundCheckDistance, grounded ? Color.green : Color.red, 0.1f);

        return grounded;
    }

    #endregion

    #region Transform Utilities

    private void LerpParts(Vector3 scale, Vector3 bodyPos, Vector3 headPos, Vector3 bottomPos, float t)
    {
        body.transform.localScale = Vector3.Lerp(body.transform.localScale, scale, t);
        body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, bodyPos, t);
        head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, headPos, t);
        bottom.transform.localPosition = Vector3.Lerp(bottom.transform.localPosition, bottomPos, t);
    }

    private void ResetPositions()
    {
        body.transform.localScale = initialBodyScale;
        body.transform.localPosition = initialBodyLocalPos;
        head.transform.localPosition = initialHeadLocalPos;
        bottom.transform.localPosition = initialBottomLocalPos;

        targetBodyScale = initialBodyScale;
        targetBodyPosition = initialBodyLocalPos;
        targetHeadPosition = initialHeadLocalPos;
        targetBottomPosition = initialBottomLocalPos;

        totalAlargar = 0f;
    }

    public bool IsMovementLocked()
    {
        return isMovementLocked || isRetracting || isGrabbingObject;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        if (head == null || playerInput == null) return;

        if (isPlayer1)
        {
            DrawPlayer1Gizmos();
        }
        else if (isPlayer2)
        {
            DrawPlayer2Gizmos();
        }

        // Gizmo para detección de objetos agarrables
        Gizmos.color = isGrabbingObject ? Color.green : Color.magenta;
        Gizmos.DrawWireSphere(head.transform.position, grabDetectionRadius);
    }

    private void DrawPlayer1Gizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(head.transform.position, groundCheckRadius);
        Gizmos.DrawLine(head.transform.position, head.transform.position + Vector3.down * groundCheckDistance);

        Vector3 ceilingCheckOrigin = head.transform.position + head.transform.TransformDirection(ceilingCheckOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ceilingCheckOrigin, ceilingCheckRadius);
        Gizmos.DrawLine(ceilingCheckOrigin, ceilingCheckOrigin + transform.forward * ceilingCheckDistance);
    }

    private void DrawPlayer2Gizmos()
    {
        Vector3 checkOrigin = head.transform.position + head.transform.TransformDirection(wallCheckOffset);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checkOrigin, wallCheckRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(checkOrigin, checkOrigin + transform.forward * wallCheckDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(bottom.transform.position, player2GroundCheckRadius);
        Gizmos.DrawLine(bottom.transform.position, bottom.transform.position + Vector3.down * player2GroundCheckDistance);

        Vector3 frontCheckOrigin = body.transform.position + body.transform.TransformDirection(frontObstacleOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(frontCheckOrigin, frontObstacleCheckRadius);
        Gizmos.DrawLine(frontCheckOrigin, frontCheckOrigin + transform.up * frontObstacleCheckDistance);
    }

    #endregion
}