using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera camera;
    void Awake()
    {
        camera = GetComponent<Camera>();
       // camera.enabled = false;
    }


}
