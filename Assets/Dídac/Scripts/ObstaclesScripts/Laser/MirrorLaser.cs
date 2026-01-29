using UnityEngine;

public class MirrorLaser : MonoBehaviour
{
    [Header("Configuración Visual")]
    public GameObject reflectedLaserVFX; // Arrastra aquí el cilindro hijo de este espejo
    public float laserWidth = 0.1f;      // Grosor del rayo reflejado
    public float maxDistance = 100f;     // Distancia máxima

    [Header("Lógica")]
    public bool isReceivingLaser = false;
    public LayerMask mirrorMask;

    // Variables internas
    private MirrorLaser currentHittingMirror;
    private LaserReceiver currentHittingReceiver;

    private Vector3 laserDir;       // Dirección en la que sale el láser reflejado
    private Vector3 originLaser;    // Punto exacto donde el láser anterior tocó este espejo
    private Vector3 reflectionDir;  // Dirección del siguiente rebote (si lo hubiera)

    void Start()
    {
        // Asegurarnos de que el láser empiece apagado
        if (reflectedLaserVFX != null) reflectedLaserVFX.SetActive(false);
    }

    void Update()
    {
        // Solo calculamos y dibujamos si estamos recibiendo energía
        if (isReceivingLaser)
        {
            // 1. Activar el visual
            if (reflectedLaserVFX != null && !reflectedLaserVFX.activeSelf)
                reflectedLaserVFX.SetActive(true);

            Vector3 visualEndPoint; // Aquí guardaremos dónde termina el rayo para el dibujo

            // 2. Raycast (Usamos el pequeño offset epsOrigin para físicas, pero originLaser para visuales)
            Vector3 epsOrigin = originLaser + laserDir * 0.001f;

            if (Physics.Raycast(epsOrigin, laserDir, out RaycastHit hitInfo, maxDistance, mirrorMask))
            {
                // -- SI GOLPEA ALGO --
                Vector3 distanceToHit = hitInfo.point - originLaser;
                Debug.DrawRay(originLaser, distanceToHit, Color.red);

                Vector3 hitPos = hitInfo.point;
                visualEndPoint = hitPos; // El final visual es el impacto

                // Calculamos el siguiente rebote
                Vector3 incomingDir = distanceToHit.normalized;
                Vector3 surfaceNormal = hitInfo.normal;
                reflectionDir = Vector3.Reflect(incomingDir, surfaceNormal);

                // Lógica de interacción (Espejos / Receptores)
                HandleInteractions(hitInfo, reflectionDir, hitPos);
            }
            else
            {
                // -- SI NO GOLPEA NADA --
                Debug.DrawRay(originLaser, laserDir * maxDistance, Color.red);

                // El final visual es la distancia máxima
                visualEndPoint = originLaser + (laserDir * maxDistance);

                // Limpiamos referencias si dejamos de golpear objetos
                CleanUpInteractions();
            }

            // 3. Actualizar el Cilindro Visual
            if (reflectedLaserVFX != null)
            {
                ActualizarVisualLaser(originLaser, visualEndPoint);
            }
        }
        else
        {
            // Si NO estamos recibiendo láser, asegurarnos de apagar el visual
            if (reflectedLaserVFX != null && reflectedLaserVFX.activeSelf)
                reflectedLaserVFX.SetActive(false);
        }
    }

    // Método auxiliar para limpiar código en el Update
    void HandleInteractions(RaycastHit hitInfo, Vector3 refDir, Vector3 pos)
    {
        // Caso: Golpeamos otro espejo
        if (hitInfo.collider.CompareTag("Mirror"))
        {
            MirrorLaser mirror = hitInfo.collider.GetComponent<MirrorLaser>();

            // Si cambiamos de espejo objetivo, desactivamos el anterior
            if (currentHittingMirror != null && currentHittingMirror != mirror)
                currentHittingMirror.DeactivateMirror();

            currentHittingMirror = mirror;
            if (currentHittingMirror != null)
                currentHittingMirror.ActivateMirror(refDir, pos);

            // Limpiamos receiver si hubiera
            if (currentHittingReceiver != null) { currentHittingReceiver.DeactivateReceiver(); currentHittingReceiver = null; }
        }
        // Caso: Golpeamos un receptor
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

    // --- LÓGICA DEL CILINDRO (Igual que en el Emitter) ---
    void ActualizarVisualLaser(Vector3 inicio, Vector3 fin)
    {
        // Posición: Punto medio
        reflectedLaserVFX.transform.position = (inicio + fin) / 2f;
        // Rotación: Mirar al destino
        reflectedLaserVFX.transform.up = fin - inicio;
        // Escala
        float distancia = Vector3.Distance(inicio, fin);
        reflectedLaserVFX.transform.localScale = new Vector3(laserWidth, distancia / 2f, laserWidth);
    }

    // --- MÉTODOS PÚBLICOS ---

    public void ActivateMirror(Vector3 receivedDir, Vector3 hitPosReceived)
    {
        isReceivingLaser = true;
        laserDir = receivedDir;     // Dirección de salida (reflejada del anterior)
        originLaser = hitPosReceived; // Punto de inicio visual (impacto en este espejo)
    }

    public void DeactivateMirror()
    {
        isReceivingLaser = false;

        // IMPORTANTE: Al desactivar este espejo, debemos apagar inmediatamente
        // cualquier espejo o receptor que este espejo estuviera alimentando.
        CleanUpInteractions();

        // Apagar el visual inmediatamente
        if (reflectedLaserVFX != null) reflectedLaserVFX.SetActive(false);
    }
}