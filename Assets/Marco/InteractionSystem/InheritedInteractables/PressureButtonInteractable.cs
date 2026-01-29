using UnityEngine;
using DG.Tweening;
using System.Collections; 
using UnityEngine.Events;

[RequireComponent(typeof(Collider))] // Asegura que tenga collider
public class PressureButtonInteractable : TriggerInteractable
{
    [Header("Pressure Settings")]
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private UnityEvent onDeactivate;
    [SerializeField] public MeshRenderer buttonMesh;

    [Tooltip("Tiempo de espera antes de desactivar. Ayuda a evitar el parpadeo si el player se mueve en el borde.")]
    [SerializeField] private float deactivationDelay = 0.2f;

    [Header("Detection Tags")]
    [SerializeField] private bool triggerByPlayerOnly = true;
    [SerializeField] private string playerTag = "Player";

    private float defaultScaleY;
    private Coroutine deactivateCoroutine;
    private int objectsOnButton = 0; // Contador por si hay múltiples objetos (ej: player + caja)

    protected void Awake()
    {
        defaultScaleY = buttonMesh.transform.localScale.z;

        // Aseguramos que el collider sea Trigger
        GetComponent<Collider>().isTrigger = true;
    }

    // ---------------------------------------------------------
    // LÓGICA DE COLISIÓN (Aquí está el arreglo de sensibilidad)
    // ---------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidObject(other)) return;

        objectsOnButton++;

        // Si entramos, cancelamos cualquier intento de apagado pendiente
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }

        // Activamos inmediatamente
        Activate();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidObject(other)) return;

        objectsOnButton--;

        // Protección: Evitar negativos si la lógica falla
        if (objectsOnButton < 0) objectsOnButton = 0;

        // Solo desactivamos si ya no queda NADIE encima
        if (objectsOnButton == 0)
        {
            // En lugar de llamar a Deactivate() directo, iniciamos el contador
            deactivateCoroutine = StartCoroutine(DeactivateWithDelay());
        }
    }

    private IEnumerator DeactivateWithDelay()
    {
        // Esperamos un momento. Si el jugador vuelve a pisar (o es un glitch de física),
        // OnTriggerEnter cancelará esta corrutina antes de que termine.
        yield return new WaitForSeconds(deactivationDelay);

        Deactivate();
        deactivateCoroutine = null;
    }

    private bool IsValidObject(Collider other)
    {
        // Ignoramos triggers para evitar que el sensor del player active el botón
        if (other.isTrigger) return false;

        if (triggerByPlayerOnly)
        {
            // Ojo: Asegúrate que tu Player tenga el tag "Player" o usa GetComponent<Interactor>()
            return other.CompareTag(playerTag) || other.GetComponent<Interactor>() != null;
        }
        return true;
    }

    // ---------------------------------------------------------
    // LÓGICA VISUAL (Tu código original)
    // ---------------------------------------------------------

    public override void Activate()
    {
        if (active_state == ACTIVE_STATE.ON) return;

        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleZ(0.2f, 0.1f).SetEase(Ease.OutExpo);

        // Solo reproducir sonido si cambiamos de estado
        if (active_state != ACTIVE_STATE.ON) AudioManager.instance.PlayPlaca();

        base.Activate();
        onActivate?.Invoke();
    }

    public override void Deactivate()
    {
        if (active_state == ACTIVE_STATE.OFF) return;

        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleZ(defaultScaleY, 0.8f).SetEase(Ease.OutElastic);

        AudioManager.instance.PlayPlaca();

        base.Deactivate();
        onDeactivate?.Invoke();
    }

    public override bool IsActive()
    {
        return active_state == ACTIVE_STATE.ON;
    }
}