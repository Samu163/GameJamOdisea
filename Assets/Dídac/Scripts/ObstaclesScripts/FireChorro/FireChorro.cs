using UnityEngine;

public class FireChorro : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject fireObject; // El objeto que tiene el ParticleSystem
    private ParticleSystem fireParticles; // Referencia interna para controlar la emisión
    [SerializeField] private BoxCollider damageCollider;

    [Header("Configuración")]
    [SerializeField] private float activationTimer = 0f;
    [SerializeField] private float activeDuration = 3.0f;
    [SerializeField] private float inactiveDuration = 5.0f;

    private bool isActive = false;

    private void Start()
    {
        // 1. Obtenemos el sistema de partículas del objeto
        // Si el script está en el mismo objeto que el fuego, usa GetComponent<ParticleSystem>()
        // Si 'fireObject' es un hijo asignado en el inspector:
        if (fireObject != null)
        {
            fireParticles = fireObject.GetComponent<ParticleSystem>();
        }

        // 2. Estado inicial: El objeto debe estar ACTIVO (true) para que las partículas existan,
        // pero le decimos que pare de emitir (Stop).
        if (fireObject != null) fireObject.SetActive(true);
        if (fireParticles != null) fireParticles.Stop(); // "Cierra el grifo"

        if (damageCollider != null) damageCollider.enabled = false;

        isActive = false;
        activationTimer = 0f;
    }

    void Update()
    {
        if (isActive)
        {
            // --- FASE ACTIVA (Fuego saliendo) ---
            activationTimer += Time.deltaTime;

            if (activationTimer >= activeDuration)
            {
                // CAMBIO CLAVE: No desactivamos el objeto, solo paramos la emisión
                if (fireParticles != null) fireParticles.Stop();

                // El daño sí se corta de inmediato
                if (damageCollider != null) damageCollider.enabled = false;

                AudioManager.instance.StopParrillaFuego();

                isActive = false;
                activationTimer = 0f;
            }
        }
        else
        {
            // --- FASE INACTIVA (Esperando) ---
            activationTimer += Time.deltaTime;

            if (activationTimer >= inactiveDuration)
            {
                // CAMBIO CLAVE: Arrancamos el sistema de partículas
                if (fireParticles != null) fireParticles.Play();

                if (damageCollider != null) damageCollider.enabled = true;

                AudioManager.instance.PlayParrillaFuego();

                isActive = true;
                activationTimer = 0f;
            }
        }
    }
}