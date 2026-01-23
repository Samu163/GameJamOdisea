using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float turnSpeed = 900f;

    [HideInInspector] public Vector3 inputDir;
    private Rigidbody rb;
    private PlayerAlargar playerAlargar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAlargar = GetComponent<PlayerAlargar>();
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        //mecanicas salto 
        if (playerAlargar != null && playerAlargar.isAlargarHeld)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }
        if (playerAlargar != null && playerAlargar.isJumping)
        {
            return;
        }

        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (playerAlargar != null && playerAlargar.IsMovementLocked())
        {
            ApplyDeceleration(currentHorizontalVelocity);
            return;
        }

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = inputDir.normalized;
            Vector3 targetVelocity = moveDirection * maxSpeed;
            Vector3 newVelocity = Vector3.MoveTowards(currentHorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

            rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);

            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
        }
        else
        {
            ApplyDeceleration(currentHorizontalVelocity);
        }
    }

    private void ApplyDeceleration(Vector3 currentHorizontalVelocity)
    {
        Vector3 newVelocity = Vector3.MoveTowards(currentHorizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
    }
}