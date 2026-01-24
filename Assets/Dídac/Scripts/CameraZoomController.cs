using Unity.Cinemachine;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Cámaras")]
    public CinemachineCamera mainCamera;
    public CinemachineCamera dialogueCamera;

    private void Start()
    {
        // Asegúrate de que la cámara de diálogo tenga una prioridad menor al inicio
        mainCamera.Priority = 10;
    }

    // Llama a esto cuando empiece el diálogo
    public void ActivarZoomDialogo()
    {
        // Al poner una prioridad mayor, Cinemachine hace la transición automática
        dialogueCamera.Priority = 20;
    }

    // Llama a esto cuando termine el diálogo
    public void DesactivarZoomDialogo()
    {
        // Al bajar la prioridad, vuelve a la cámara principal (que tiene 10)
        dialogueCamera.Priority = 5;
    }
}
