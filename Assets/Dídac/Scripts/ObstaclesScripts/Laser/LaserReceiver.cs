using System;
using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    [SerializeField]
    private bool isReceivingLaser = false;

    [SerializeField]
    private UnityEvent onLaserReceived;

    [SerializeField]
    private UnityEvent onLaserExit;

        // Estado previo para detectar cambios y evitar invocaciones repetidas
        private bool previousReceivingState;

        // Propiedad de solo lectura para acceder al estado desde otros scripts si es necesario
        public bool IsReceivingLaser => isReceivingLaser;

        void Awake()
        {
            // Inicializar previousReceivingState para evitar invocaciones al iniciar si no hubo cambio
            previousReceivingState = isReceivingLaser;
        }

        void Update()
        {
            // Solo reaccionar cuando exista un cambio en el estado
            if (isReceivingLaser == previousReceivingState) return;

            previousReceivingState = isReceivingLaser;

            if (isReceivingLaser)
            {
                Debug.Log("LaserReceiver: recibido láser (estado cambiado a activo).");
                onLaserReceived?.Invoke();
            }
            else
            {
                Debug.Log("LaserReceiver: láser perdido (estado cambiado a inactivo).");
                onLaserExit?.Invoke();
            }
        }

    // Métodos públicos para cambiar el estado (útiles para otros scripts)
    public void ActivateReceiver()
    {
        if (!isReceivingLaser)
            isReceivingLaser = true;
    }

    public void DeactivateReceiver()
    {
        if (isReceivingLaser)
            isReceivingLaser = false;
    }

    // Opción: permitir establecer el estado directamente (si se necesita)
    public void SetReceiving(bool receiving)
    {
        isReceivingLaser = receiving;
    }
}
