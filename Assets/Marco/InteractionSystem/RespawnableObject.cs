using UnityEngine;

public class RespawnableObject : MonoBehaviour
{

    Vector3 respawnPosition;

    private void Awake()
    {
        respawnPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            Respawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            Respawn();
        }
    }


    void Respawn()
    {
        transform.position = respawnPosition;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

}
