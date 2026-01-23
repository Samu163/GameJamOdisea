using UnityEngine;

public class Hacha : MonoBehaviour
{
    public Transform pivotPoint;
    public float rotationAngle = 45f; // Maximum rotation angle (grados)
    public float period = 2f; // Tiempo (s) para un ciclo completo ida y vuelta

    private float currentAngle = 0f;
    private float elapsed = 0f;

    private void FixedUpdate()
    {
        if (period <= 0f)
        {
            // Evita división por cero y mantiene la sierra quieta si se configura mal
            return;
        }

        // Avanza el tiempo en pasos de FixedUpdate (adecuado para física)
        elapsed += Time.fixedDeltaTime;

        // Frecuencia angular para que sin() complete un ciclo ida y vuelta en 'period' segundos.
        // Sin(?t) completa un ciclo entero en 2?; queremos que la oscilación ida-vuelta use 'period' segundos,
        // así que ? = 2? / period.
        float omega = 2f * Mathf.PI / period;

        // Ángulo objetivo según movimiento armónico simple: lento en extremos, rápido en centro.
        float targetAngle = rotationAngle * Mathf.Sin(omega * elapsed);

        // Rotamos la hacha la diferencia necesaria respecto al ángulo actual
        float deltaAngle = targetAngle - currentAngle;
        transform.RotateAround(pivotPoint.position, Vector3.right, deltaAngle);
        currentAngle = targetAngle;
    }

}
