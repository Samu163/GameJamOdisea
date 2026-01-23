using UnityEngine;

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
    private Vector3 initialPosition;

    public bool isActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
        }
            
    }

    private void Move()
    {
        float movementOffset = Mathf.Sin(Time.time * movementSpeed) * movementDistance;
        Vector3 newPosition = initialPosition;
        switch (movementType)
        {
            case MovementType.Horizontal:
                newPosition.x += movementOffset;
                break;
            case MovementType.Vertical:
                newPosition.y += movementOffset;
                break;
            case MovementType.Depth:
                newPosition.z += movementOffset;
                break;
        }
        transform.position = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position;
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
