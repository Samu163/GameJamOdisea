using System.Collections;
using UnityEngine;

public class Pinchos : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject spikes; // El colisionador de daño (puede ser un objeto padre o compartido)

    // CAMBIO: Ahora es un Array (lista) de animators
    [SerializeField] private Animator[] animators;

    [Header("Tiempos")]
    [SerializeField] private float spawnDelay = 1.0f;      // Tiempo advertencia
    [SerializeField] private float deactivateDelay = 3.0f; // Tiempo arriba
    [SerializeField] private float retractTime = 0.5f;     // Tiempo que tarda en bajar la animación

    private bool isSpikesActive = false;

    private void Start()
    {
        spikes.SetActive(false);
    }

    private IEnumerator SpawnSpikes(float delay)
    {
        // 1. Espera de advertencia
        yield return new WaitForSeconds(delay);

        // 2. Activa el daño
        spikes.SetActive(true);

        // 3. Activa la animación de ATAQUE en TODOS los animators de la lista
        foreach (Animator anim in animators)
        {
            if (anim != null)
            {
                anim.SetTrigger("PinchoATTACK");
            }
        }

        AudioManager.instance.PlayPinchos();

        // 4. Inicia cuenta atrás para bajar
        StartCoroutine(DeactivateSpikes(deactivateDelay));
    }

    private IEnumerator DeactivateSpikes(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 5. Activa la animación de IDLE (bajar) en TODOS los animators
        foreach (Animator anim in animators)
        {
            if (anim != null)
            {
                anim.SetTrigger("PinchoIdle");
            }
        }

        // 6. Espera a que la animación termine visualmente
        yield return new WaitForSeconds(retractTime);

        spikes.SetActive(false);
        isSpikesActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player1") || other.CompareTag("Player2")) && !isSpikesActive)
        {
            isSpikesActive = true;
            StartCoroutine(SpawnSpikes(spawnDelay));
        }
    }
}