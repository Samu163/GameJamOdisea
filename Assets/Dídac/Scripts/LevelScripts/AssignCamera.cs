using UnityEngine;

public class AssignCamera : MonoBehaviour
{
    private void Awake()
    {
        if (LevelManager.instance == null) return;
        LevelManager.instance.AssignCamera(gameObject);
    }
}
