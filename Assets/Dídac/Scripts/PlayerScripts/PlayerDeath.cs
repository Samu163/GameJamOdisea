using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{

    public GameObject rbHead;
    public GameObject rbBody;
    public GameObject rbBottom;
    public LayerMask deathLayers;

    private void ExplodePlayer()
    {
        rbHead.AddComponent<Rigidbody>();
        rbHead.transform.parent = null;
        rbHead.GetComponent<Rigidbody>().AddForce(Vector3.up * 2f, ForceMode.Impulse);
        rbBody.AddComponent<Rigidbody>();
        rbBody.transform.parent = null;
        rbBody.GetComponent<Rigidbody>().AddForce(Vector3.right * 1f, ForceMode.Impulse);
        rbBottom.AddComponent<Rigidbody>();
        rbBottom.transform.parent = null;
        rbBottom.GetComponent<Rigidbody>().AddForce(Vector3.left * 1.5f, ForceMode.Impulse);
        GetComponent<Collider>().excludeLayers = deathLayers;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            Debug.Log("Player has died collision.");
            StartCoroutine(RespawnPlayer());
            ExplodePlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            Debug.Log("Player has died trigger.");
            StartCoroutine(RespawnPlayer());
            ExplodePlayer();
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.ResetLevel();
    }
}
