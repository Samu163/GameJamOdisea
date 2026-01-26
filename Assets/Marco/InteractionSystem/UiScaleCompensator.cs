using UnityEngine;
using UnityEngine.UIElements;

// Class that ensures the ui element scale is globally 1,1 even if the parent's isn't
public class UiScaleCompensator : MonoBehaviour
{

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        Vector3 pScale = transform.parent.localScale;
        rectTransform.localScale = new Vector3(1f / pScale.x, 1f / pScale.y, 1f / pScale.z);
    }




}
