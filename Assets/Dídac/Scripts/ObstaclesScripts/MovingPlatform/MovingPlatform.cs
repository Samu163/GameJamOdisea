using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{

    private enum MovementType
    {
        Horizontal,
        Vertical,
        Depth
    }
    [SerializeField] private MovementType movementType = MovementType.Horizontal;
    [SerializeField] private float movementDistance = 5f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float pauseDuration = 0.5f;
    private Vector3 initialPosition;

    public bool isActive = true;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 target;
    private bool isPaused = false;

    public bool startFromInitialPos = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;

        Vector3 axis = Vector3.right;
        switch (movementType)
        {
            case MovementType.Horizontal:
                axis = Vector3.right;
                break;
            case MovementType.Vertical:
                axis = Vector3.up;
                break;
            case MovementType.Depth:
                axis = Vector3.forward;
                break;
        }

        pointA = initialPosition - axis * movementDistance;
        pointB = initialPosition + axis * movementDistance;
        target = pointB;

        if (startFromInitialPos)
        {
            transform.position = pointA;
        }
    }

    private void FixedUpdate()
    {
        if (!isActive || isPaused) return;

        Move();

    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) <= 0.01f)
        {
            StartCoroutine(PauseAndSwitch());
        }
    }

    public void Activate()
    {
        isActive = !isActive;
    }

    private IEnumerator PauseAndSwitch()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        target = (target == pointA) ? pointB : pointA;
        isPaused = false;
    }

    // NEW: Parenting del jugador cuando está encima de la plataforma
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            Debug.Log("Player collided with moving platform");
            if (movementType != MovementType.Horizontal && movementType != MovementType.Depth) return;

            foreach (var contact in collision.contacts)
            {
                Debug.Log("Contact normal: " + contact.normal);
                if (Vector3.Dot(contact.normal, Vector3.up) < -0.5f)
                {
                    collision.transform.SetParent(transform, true);
                    break;
                }
            }
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            collision.transform.SetParent(null, true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 center = Application.isPlaying ? initialPosition : transform.position;

        Vector3 startPosition = center;
        Vector3 endPosition = center;
        switch (movementType)
        {
            case MovementType.Horizontal:
                startPosition.x -= movementDistance;
                endPosition.x += movementDistance;
                break;
            case MovementType.Vertical:
                startPosition.y -= movementDistance;
                endPosition.y += movementDistance;
                break;
            case MovementType.Depth:
                startPosition.z -= movementDistance;
                endPosition.z += movementDistance;
                break;
        }
        Gizmos.DrawLine(startPosition, endPosition);
        Gizmos.DrawSphere(startPosition, 0.1f);
        Gizmos.DrawSphere(endPosition, 0.1f);
    }
}
