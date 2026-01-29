using UnityEngine;
using System.Collections.Generic;

public class MovingParent: MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("1 = Se mueve exactamente igual que la plataforma. Mayor a 1 = Se mueve más rápido.")]
    [Range(0f, 2f)]
    [SerializeField] private float movementFactor = 1f; // Este es el parámetro que pediste

    private List<Transform> playersOnPlatform = new List<Transform>();
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    // Usamos FixedUpdate porque tu plataforma se mueve en FixedUpdate
    void FixedUpdate()
    {
        // 1. Calculamos cuánto se ha movido la plataforma desde el último frame
        Vector3 movementDelta = transform.position - lastPosition;

        // 2. Aplicamos ese movimiento a todos los jugadores que estén encima
        if (playersOnPlatform.Count > 0)
        {
            foreach (Transform player in playersOnPlatform)
            {
                if (player != null)
                {
                    // Opción A: Mover transform directamente (funciona con todo)
                    player.position += movementDelta * movementFactor;

                    // Nota: Si usas Rigidbody en el player y sigue temblando, 
                    // podrías necesitar usar playerRB.MovePosition(), pero esto suele bastar.
                }
            }
        }

        // 3. Actualizamos la última posición para el siguiente frame
        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            // Verificamos que esté pisando la plataforma (y no chocando de lado)
            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) < -0.5f)
                {
                    if (!playersOnPlatform.Contains(collision.transform))
                    {
                        Debug.Log("Player anclado a la plataforma");
                        playersOnPlatform.Add(collision.transform);
                    }
                    break;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            if (playersOnPlatform.Contains(collision.transform))
            {
                Debug.Log("Player desanclado");
                playersOnPlatform.Remove(collision.transform);
            }
        }
    }

    // Función auxiliar para detectar jugadores
    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player1") || obj.CompareTag("Player2");
    }
}