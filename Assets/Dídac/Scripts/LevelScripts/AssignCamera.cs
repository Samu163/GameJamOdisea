using UnityEngine;

public class AssignCamera : MonoBehaviour
{
    private void Start()
    {
        if (LevelManager.instance == null) return;
        LevelManager.instance.AssignCamera(gameObject);
    }
}
