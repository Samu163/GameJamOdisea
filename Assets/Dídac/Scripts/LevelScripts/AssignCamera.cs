using UnityEngine;

public class AssignCamera : MonoBehaviour
{
    private void Awake()
    {
        LevelManager.instance.AssignCamera(gameObject);
    }
}
