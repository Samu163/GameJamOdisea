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

    [Header("Wall Check - Player 2")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float wallCheckDistance = 2f;
    [SerializeField] private float wallCheckRadius = 0.8f;
    [SerializeField] private Vector3 wallCheckOffset = Vector3.zero;
    [SerializeField] private float player2GroundCheckDistance = 0.5f;
    [SerializeField] private float player2GroundCheckRadius = 0.3f;

    // Estados
    private bool isRetracting = false;
    private bool isQuickRetracting = false;
    private bool isMovementLocked = false;
    private bool shouldJumpAfterRetract = false;
    public bool isJumping = false;
    private bool canInteract = true; 

    private float alargarTimer = 0f;
    private float alargarInterval = 0.1f;
    private float totalAlargar = 0f;

    // Posiciones iniciales
    private Vector3 initialBodyScale, targetBodyScale;
    private Vector3 initialBodyLocalPos, targetBodyPosition;
    private Vector3 initialHeadLocalPos, targetHeadPosition;
    private Vector3 initialBottomLocalPos, targetBottomPosition;

    private Rigidbody rb;
    private Vector3 releaseHeadWorldPos;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        initialBodyScale = body.transform.localScale;
        targetBodyScale = initialBodyScale;
        initialBodyLocalPos = body.transform.localPosition;
        targetBodyPosition = initialBodyLocalPos;
        initialHeadLocalPos = head.transform.localPosition;
        targetHeadPosition = initialHeadLocalPos;
        initialBottomLocalPos = bottom.transform.localPosition;
        targetBottomPosition = initialBottomLocalPos;

        if (playerInput.playerIndex == 1)
        {
            LevelManager.instance.player1 = this.gameObject;
            LevelManager.instance.SpawnPlayersOnInitialPosition(1);
        }
        else if (playerInput.playerIndex == 2)
        {
            LevelManager.instance.player2 = this.gameObject;
            LevelManager.instance.SpawnPlayersOnInitialPosition(2);
        }
    }

    public void Update()
    {
        bool isGrounded = playerInput.playerIndex == 2 ? IsPlayer2Grounded() : true;

        bool canExtend = !isRetracting && !isMovementLocked && !isJumping && isGrounded && canInteract;

        if (isAlargarHeld && canExtend && totalAlargar < maxRange)
        {
            alargarTimer += Time.deltaTime;
            if (alargarTimer >= alargarInterval)
            {
                Alargar();
                alargarTimer = 0f;
            }
            LerpParts(targetBodyScale, targetBodyPosition, targetHeadPosition, targetBottomPosition, 0.1f);
        }
        else if (isAlargarHeld && canExtend && totalAlargar >= maxRange)
        {
            CancelAlargar();
        }
        // SI INTENTA ESTIRARSE EN EL AIRE O EN COOLDOWN -> IGNORAR
        else if (isAlargarHeld && !canExtend)
        {
            if (!isGrounded)
            {
                Debug.Log("No puedes estirarte en el aire!");
            }
            else if (!canInteract)
            {
                Debug.Log("Cooldown activo, espera...");
            }
        }

        // RETRACCIÓN
        if (isRetracting)
        {
            float lerpSpeed = isQuickRetracting ? 0.3f : 0.1f;
            LerpParts(initialBodyScale, initialBodyLocalPos, initialHeadLocalPos, initialBottomLocalPos, lerpSpeed);

            if (!isQuickRetracting)
            {
                Vector3 currentHeadWorld = head.transform.position;
                Vector3 delta = releaseHeadWorldPos - currentHeadWorld;
                if (delta.sqrMagnitude > 0f) transform.position += delta;
            }

            const float eps = 0.01f;
            if (Vector3.Distance(body.transform.localScale, initialBodyScale) < eps &&
                Vector3.Distance(head.transform.localPosition, initialHeadLocalPos) < eps)
            {
                ResetPositions();

                if (playerInput.playerIndex == 2 && shouldJumpAfterRetract)
                {
                    StartCoroutine(WallJumpRoutine());
                }

                isRetracting = false;
                shouldJumpAfterRetract = false;
            }
        }
    }

    public void Alargar()
    {
        Vector3 increment = new Vector3(0, alargarAmount / 2, 0);
        targetBodyScale += increment;

        if (playerInput.playerIndex == 1)
        {
            targetBodyPosition += new Vector3(0, 0, alargarAmount / 2);
            targetHeadPosition += new Vector3(0, 0, alargarAmount);
        }
        else if (playerInput.playerIndex == 2)
        {
            targetBodyPosition += increment;
            targetHeadPosition += new Vector3(0, alargarAmount, 0);
        }

        totalAlargar += alargarAmount;
    }

    public void CancelAlargar()
    {
        isAlargarHeld = false;

        if (totalAlargar <= 0f)
        {
            return;
        }

        bool canRetractNormal = false;

        if (playerInput.playerIndex == 1)
        {
            canRetractNormal = CheckGroundUnderHead();
        }
        else if (playerInput.playerIndex == 2)
        {
            bool hasWall = CheckWallInFront();
            canRetractNormal = hasWall;

            if (hasWall && canInteract)
            {
                shouldJumpAfterRetract = true;
                Debug.Log("MURO DETECTADO - Salto preparado");
            }
            else
            {
                Debug.Log("Sin muro - Retracción rápida");
            }
        }

        isRetracting = true;
        totalAlargar = 0f;

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

        targetBodyScale = initialBodyScale;
        targetBodyPosition = initialBodyLocalPos;
        targetHeadPosition = initialHeadLocalPos;
        targetBottomPosition = initialBottomLocalPos;
    }

    private IEnumerator WallJumpRoutine()
    {
        Debug.Log("Iniciando salto!");

        isMovementLocked = true;
        isJumping = true;
        canInteract = false;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForceUp, ForceMode.Impulse);
        yield return new WaitForSeconds(jumpDelay);

        rb.AddForce(transform.forward * jumpForceForward, ForceMode.Impulse);
        yield return new WaitForSeconds(jumpLockDuration);

        isMovementLocked = false;
        isJumping = false;

        Debug.Log("Cooldown iniciado - NO puedes estirarte");
        yield return new WaitForSeconds(jumpCooldown);

        canInteract = true;
        Debug.Log("Cooldown terminado - Puedes estirarte de nuevo");
    }

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
        return isMovementLocked || isRetracting;
    }

    private bool CheckWallInFront()
    {
        if (playerInput.playerIndex != 2) return false;

        Vector3 checkOrigin = head.transform.position + head.transform.TransformDirection(wallCheckOffset);
        Vector3 direction = transform.forward;

        // MÉTODO 1: OverlapSphere 
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

        // MÉTODO 2: SphereCast 
        if (Physics.SphereCast(checkOrigin, wallCheckRadius, direction, out RaycastHit hitInfo, wallCheckDistance))
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
        if (playerInput.playerIndex != 2) return true;

        Vector3 rayOrigin = bottom.transform.position;
        bool grounded = Physics.SphereCast(rayOrigin, player2GroundCheckRadius, Vector3.down, out RaycastHit hitInfo, player2GroundCheckDistance, groundLayer);
        Debug.DrawRay(rayOrigin, Vector3.down * player2GroundCheckDistance, grounded ? Color.green : Color.red, 0.1f);
        return grounded;
    }

    private void OnDrawGizmosSelected()
    {
        if (head != null && playerInput != null)
        {
            if (playerInput.playerIndex == 1)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(head.transform.position, groundCheckRadius);
                Gizmos.DrawLine(head.transform.position, head.transform.position + Vector3.down * groundCheckDistance);
            }
            else if (playerInput.playerIndex == 2)
            {
                Vector3 checkOrigin = head.transform.position + head.transform.TransformDirection(wallCheckOffset);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(checkOrigin, wallCheckRadius);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(checkOrigin, checkOrigin + transform.forward * wallCheckDistance);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(bottom.transform.position, player2GroundCheckRadius);
                Gizmos.DrawLine(bottom.transform.position, bottom.transform.position + Vector3.down * player2GroundCheckDistance);
            }
        }
    }
}