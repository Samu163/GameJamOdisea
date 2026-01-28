using System.Collections;
using UnityEngine;

public class Pinchos : MonoBehaviour
{
    [SerializeField] private GameObject spikes;
    [SerializeField] private float spawnDelay = 1.0f;
    [SerializeField] private float deactivateDelay = 3.0f;
    private bool isSpikesActive = false;


    private void Start()
    {
        spikes.SetActive(false);
    }

    private IEnumerator SpawnSpikes(float delay)
    {
        yield return new WaitForSeconds(delay);
        spikes.SetActive(true);
        AudioManager.instance.PlayPinchos();
        StartCoroutine(DeactivateSpikes(deactivateDelay));
    }

    private IEnumerator DeactivateSpikes(float delay)
    {
        yield return new WaitForSeconds(delay);
        spikes.SetActive(false);
        isSpikesActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player1") || other.CompareTag("Player2")) && !isSpikesActive)
        {
            StartCoroutine(SpawnSpikes(spawnDelay));
            isSpikesActive = true;
        }
    }
}
