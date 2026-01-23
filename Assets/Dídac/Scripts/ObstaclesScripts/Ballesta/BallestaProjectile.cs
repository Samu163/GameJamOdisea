using UnityEngine;

public class BallestaProjectile : MonoBehaviour
{

    private bool isShooting = false;
    private Rigidbody rb;
    private Vector3 initialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isShooting)
        {
            rb.MovePosition(transform.position + transform.forward * 0.5f);
        }
    }

    public void Shoot()
    {
        isShooting = true;
        gameObject.SetActive(true);
    }

    public void ResetProjectile()
    {
        isShooting = false;
        gameObject.SetActive(false);
        transform.position = initialPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ResetProjectile();
    }
}
