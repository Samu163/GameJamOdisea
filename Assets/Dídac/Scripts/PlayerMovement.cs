using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    public Vector3 inputDir;
    [SerializeField] private float rotationSmooth = 10f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (inputDir.sqrMagnitude > 0.001f)
        {

            Vector3 move = inputDir;


            Vector3 velocity = rb.linearVelocity;
            Vector3 targetHorizontal = move * moveSpeed;
            Vector3 newVelocity = Vector3.Lerp(velocity, new Vector3(targetHorizontal.x, velocity.y, targetHorizontal.z), 0.9f);

            rb.linearVelocity = newVelocity;


            Quaternion targetRot = Quaternion.LookRotation(new Vector3(move.x, 0f, move.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmooth * Time.fixedDeltaTime);
        }
        else
        {

            Vector3 v = rb.linearVelocity;
            Vector3 dec = Vector3.Lerp(new Vector3(v.x, 0f, v.z), Vector3.zero, 8f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(dec.x, v.y, dec.z);
        }
    }


}
