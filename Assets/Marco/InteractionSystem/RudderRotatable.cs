using System.Collections.Generic;
using UnityEngine;

public class RudderRotatable : MonoBehaviour
{
    [Header("Configuración de Rotación (Grados)")]
    [Tooltip("La rotación final en grados (Euler Angles) cuando el timón está al máximo")]
    [SerializeField] Vector3 maxRotation = new Vector3(0, 90, 0);

    [Tooltip("La rotación inicial en grados (Euler Angles) cuando el timón está al mínimo")]
    [SerializeField] Vector3 minRotation = new Vector3(0, 0, 0);

    [Header("Conexiones")]
    [Tooltip("Selecciona el rudder interactable que controla este objeto")]
    public List<RudderInteractable> myRudder;

    [Tooltip("¿Cuántos grados debe girar el TIMÓN (el control) para completar la animación?")]
    [SerializeField] float rangeInDegrees = 150f;

    // 0.0 significa que estamos en Min, 1.0 significa que estamos en Max
    private float _currentProgress = 0f;

    private void Awake()
    {
        // Suscribirse a los eventos del timón
        foreach (var rudder in myRudder)
            rudder.onRudderTurned.AddListener(OnRudderTurned);

        // Establecer la rotación inicial
        transform.localRotation = Quaternion.Euler(minRotation);
    }

    public void OnRudderTurned(float angle_difference)
    {
        // Convertimos el cambio de ángulo del timón en un porcentaje de progreso
        float percentChange = angle_difference / rangeInDegrees;
        _currentProgress += percentChange;

        // Limitamos el progreso entre 0 y 1
        _currentProgress = Mathf.Clamp01(_currentProgress);

        // --- CAMBIO PRINCIPAL ---
        // Convertimos los vectores (Vector3) a Cuaterniones (Quaternion) para rotar suavemente
        Quaternion minRot = Quaternion.Euler(minRotation);
        Quaternion maxRot = Quaternion.Euler(maxRotation);

        // Usamos Lerp para interpolar entre las dos rotaciones
        transform.localRotation = Quaternion.Lerp(minRot, maxRot, _currentProgress);
    }
}