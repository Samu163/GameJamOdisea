using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    [Header("Configuración Láser")]
    public LayerMask mirrorMask;
    public GameObject laserVFX; // Tu cilindro
    public float laserWidth = 0.1f; // Grosor del láser
    public float maxDistance = 20f; // Distancia si no golpea nada

    // Variables internas
    private MirrorLaser currentHittingMirror;
    private LaserReceiver currentHittingReceiver;
    private Vector3 reflectionDir;
    private Vector3 hitPos;

    private void Start()
    {
        // Aseguramos que el Audio Manager exista antes de llamarlo para evitar errores
        if (AudioManager.instance != null) AudioManager.instance.PlayLaser();
    }

    void Update()
    {
        // Calculamos dónde termina el láser visualmente
        Vector3 laserEndPoint;

        // 1. Lógica del Raycast
        // Nota: Si quieres que el láser choque con PAREDES y ESPEJOS, asegúrate 
        // de que 'mirrorMask' incluya ambas capas, o quita el filtro de capa.
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, maxDistance, mirrorMask))
        {
            // SI GOLPEA ALGO
            Vector3 distanceToHit = hitInfo.point - transform.position;
            Debug.DrawRay(transform.position, distanceToHit, Color.red);

            hitPos = hitInfo.point;
            laserEndPoint = hitPos; // El final visual es el punto de impacto

            // Cálculos de reflexión
            Vector3 incomingDir = distanceToHit.normalized;
            Vector3 surfaceNormal = hitInfo.normal;
            reflectionDir = Vector3.Reflect(incomingDir, surfaceNormal);
        }
        else
        {
            // SI NO GOLPEA NADA (Láser infinito)
            Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.red);

            // El final visual es la distancia máxima hacia adelante
            laserEndPoint = transform.position + (transform.forward * maxDistance);

            // Limpiamos datos de hit antiguos
            hitPos = Vector3.zero;
        }

        // 2. Actualizamos el cilindro visual (VFX)
        if (laserVFX != null)
        {
            ActualizarVisualLaser(transform.position, laserEndPoint);
        }

        // 3. Lógica de Activación del Espejo (Sin cambios mayores)
        if (hitInfo.collider != null && (hitInfo.collider.CompareTag("Mirror")))
        {
            currentHittingMirror = hitInfo.collider.GetComponent<MirrorLaser>();
            if (currentHittingMirror != null)
            {
                currentHittingMirror.ActivateMirror(reflectionDir, hitPos);
            }
        }
        else if (hitInfo.collider.CompareTag("LaserReceiver"))
        {
            LaserReceiver receiver = hitInfo.collider.GetComponent<LaserReceiver>();

            if (currentHittingReceiver != null && currentHittingReceiver != receiver)
                currentHittingReceiver.DeactivateReceiver();

            currentHittingReceiver = receiver;
            if (currentHittingReceiver != null)
                currentHittingReceiver.ActivateReceiver();

            // Limpiamos mirror si hubiera
            if (currentHittingMirror != null) { currentHittingMirror.DeactivateMirror(); currentHittingMirror = null; }
        }
        else
        {
            CleanUpInteractions();
        }
    }

    void CleanUpInteractions()
    {
        if (currentHittingMirror != null)
        {
            currentHittingMirror.DeactivateMirror();
            currentHittingMirror = null;
        }
        if (currentHittingReceiver != null)
        {
            currentHittingReceiver.DeactivateReceiver();
            currentHittingReceiver = null;
        }
    }

    // Función encargada de estirar el cilindro
    void ActualizarVisualLaser(Vector3 inicio, Vector3 fin)
    {
        // A. Posición: Punto medio entre inicio y fin
        laserVFX.transform.position = (inicio + fin) / 2f;

        // B. Rotación: Que el eje Y (verde) mire hacia el destino
        laserVFX.transform.up = fin - inicio;

        // C. Escala:
        // X y Z = grosor
        // Y = distancia / 2 (porque el cilindro de Unity mide 2 unidades por defecto)
        float distancia = Vector3.Distance(inicio, fin);
        laserVFX.transform.localScale = new Vector3(laserWidth, distancia / 2f, laserWidth);
    }
}