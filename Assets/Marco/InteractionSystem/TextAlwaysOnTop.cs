using UnityEngine;

public class TextOverlay : MonoBehaviour
{
    private void Start()
    {
        Renderer r = GetComponent<Renderer>();

        if (r != null)
        {
            // 1. Force it to draw LAST (Queue 4000+ is the Overlay queue)
            r.material.renderQueue = 4500;

            // 2. Force it to ignore walls (ZTest = 8 means "Always")
            r.material.SetInt("_ZTest", 8);
        }
    }
}