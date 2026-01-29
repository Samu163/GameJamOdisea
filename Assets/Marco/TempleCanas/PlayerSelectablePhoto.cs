using UnityEngine;
using System.Collections.Generic;

public class PlayerSelectablePhoto : MonoBehaviour
{
    public List<GameObject> onjectstoDisableOnDisable;

    private void OnDisable()
    {
        foreach (GameObject obj in onjectstoDisableOnDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
