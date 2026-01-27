using System.Collections.Generic;
using UnityEngine;

public class RudderRotatable : MonoBehaviour
{
    [Header("Configuración de Rotación (Grados)")]
    [Tooltip("La rotación del objeto cuando la barra de progreso está al 100%")]
    [SerializeField] Vector3 maxRotation = new Vector3(0, 180, 0); // Ejemplo: 180 grados

    [Tooltip("La rotación del objeto cuando la barra de progreso está al 0%")]
    [SerializeField] Vector3 minRotation = new Vector3(0, 0, 0);   // Ejemplo: 0 grados

    [Header("Configuración Inicial")]
    [Tooltip("0 = Empieza en Min, 0.5 = Empieza en el Centro, 1 = Empieza en Max")]
    [Range(0f, 1f)]
    [SerializeField] float initialProgress = 0.5f; // <--- NUEVO: Para empezar centrado

    [Header("Conexiones")]
    public List<RudderInteractable> myRudder;

    [Tooltip("¿Cuántos grados debe girar el TIMÓN físico para completar todo el recorrido del objeto?")]
    [SerializeField] float rangeInDegrees = 150f;

    private float _currentProgress = 0f;

    private void Awake()
    {
        // 1. Establecemos el progreso inicial al valor que configures (ej: 0.5)
        _currentProgress = initialProgress;

        foreach (var rudder in myRudder)
            rudder.onRudderTurned.AddListener(OnRudderTurned);

        // 2. Calculamos la rotación inicial basada en ese progreso
        UpdateRotation();
    }

    public void OnRudderTurned(float angle_difference)
    {
        float percentChange = angle_difference / rangeInDegrees;
        _currentProgress += percentChange;

        _currentProgress = Mathf.Clamp01(_currentProgress);

        UpdateRotation();
    }

    // Saqué la lógica a una función aparte para usarla en Awake y al girar
    private void UpdateRotation()
    {
        Quaternion minRot = Quaternion.Euler(minRotation);
        Quaternion maxRot = Quaternion.Euler(maxRotation);
        transform.localRotation = Quaternion.Lerp(minRot, maxRot, _currentProgress);
    }
}