using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Update()
    {
        // Performance check: ensure camera reference is valid
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        // MATCH the camera's rotation exactly
        transform.rotation = mainCamera.transform.rotation;
    }
}