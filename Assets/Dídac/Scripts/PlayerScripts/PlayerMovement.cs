using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float turnSpeed = 900f;

    [Header("Advanced Settings")]
    [SerializeField] private float inputDeadzone = 0.01f;
    [SerializeField] private float velocitySmoothing = 0.1f;
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float minSpeedForRotation = 0.5f;

    [SerializeField] private float fallMultiplier = 2.5f;

    [HideInInspector] public Vector3 inputDir;

    private Rigidbody rb;
    private PlayerAlargar playerAlargar;

    // Estados de movimiento
    private bool hasPlayerAlargar;
    private Vector3 currentVelocity;
    private Vector3 velocityDampRef;

    // Constantes
    private const float SPAWN_HEIGHT = 1.5f;
    private static readonly Vector3 UP_VECTOR = Vector3.up;

    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        InitializeRigidbody();
        SetInitialPosition();
    }

    private void FixedUpdate()
    {
        ProcessMovement();
        ApplyExtraGravity();
    }

    #region Initialization

    private void CacheComponents()
    {
        rb = GetComponent<Rigidbody>();
        playerAlargar = GetComponent<PlayerAlargar>();
        hasPlayerAlargar = playerAlargar != null;
    }

    private void InitializeRigidbody()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void SetInitialPosition()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, SPAWN_HEIGHT, pos.z);
    }

    #endregion

    #region Movement Processing

    private void ProcessMovement()
    {
        if (IsStretching())
        {
            StopHorizontalMovement();
            return;
        }
        if (IsJumping())
        {
            return;
        }

        Vector3 horizontalVelocity = GetHorizontalVelocity();

        if (IsMovementLocked())
        {
            ApplyDeceleration(horizontalVelocity);
            return;
        }
        if (HasMovementInput())
        {
            HandleMovement(horizontalVelocity);
        }
        else
        {
            ApplyDeceleration(horizontalVelocity);
        }
    }

    private void HandleMovement(Vector3 currentHorizontalVelocity)
    {
        Vector3 moveDirection = inputDir.normalized;
        Vector3 targetVelocity = moveDirection * maxSpeed;
        Vector3 newVelocity = CalculateNewVelocity(currentHorizontalVelocity, targetVelocity);
        SetHorizontalVelocity(newVelocity);

        // Rotar hacia la dirección de movimiento
        RotateTowardsDirection(moveDirection, newVelocity.magnitude);
    }

    #endregion

    #region Velocity Calculations

    private Vector3 CalculateNewVelocity(Vector3 current, Vector3 target)
    {
        if (velocitySmoothing > 0f)
        {
            return Vector3.SmoothDamp(current, target, ref velocityDampRef, velocitySmoothing);
        }
        else
        {
            return Vector3.MoveTowards(current, target, acceleration * Time.fixedDeltaTime);
        }
    }

    private void ApplyDeceleration(Vector3 currentHorizontalVelocity)
    {
        Vector3 newVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            Vector3.zero,
            deceleration * Time.fixedDeltaTime
        );

        SetHorizontalVelocity(newVelocity);
    }

    #endregion

    private void ApplyExtraGravity()
    {
        if (rb.linearVelocity.y < -0.5f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    #region Rotation

    private void RotateTowardsDirection(Vector3 direction, float currentSpeed)
    {
        if (currentSpeed < minSpeedForRotation)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (smoothRotation)
        {
            // Rotación suave y progresiva
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );
        }
        else
        {
            // Rotación instantánea (más arcade)
            transform.rotation = targetRotation;
        }
    }

    #endregion

    #region State Checks

    private bool IsStretching()
    {
        return hasPlayerAlargar && playerAlargar.isAlargarHeld;
    }

    private bool IsJumping()
    {
        return hasPlayerAlargar && playerAlargar.isJumping;
    }

    private bool IsMovementLocked()
    {
        return hasPlayerAlargar && playerAlargar.IsMovementLocked();
    }

    private bool HasMovementInput()
    {
        return inputDir.sqrMagnitude > inputDeadzone;
    }

    #endregion

    #region Velocity Helpers

    private Vector3 GetHorizontalVelocity()
    {
        return new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }

    private void SetHorizontalVelocity(Vector3 horizontalVelocity)
    {
        rb.linearVelocity = new Vector3(
            horizontalVelocity.x,
            rb.linearVelocity.y,
            horizontalVelocity.z
        );
    }

    private void StopHorizontalMovement()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        velocityDampRef = Vector3.zero; // Reset smoothdamp
    }

    #endregion

    #region Public API

    public void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
        inputDir = Vector3.zero;
        velocityDampRef = Vector3.zero;
    }
    public void AddExternalForce(Vector3 force, ForceMode mode = ForceMode.Impulse)
    {
        rb.AddForce(force, mode);
    }
    public float GetCurrentSpeed()
    {
        return GetHorizontalVelocity().magnitude;
    }
    public bool IsMoving()
    {
        return GetCurrentSpeed() > 0.1f;
    }

    #endregion
}