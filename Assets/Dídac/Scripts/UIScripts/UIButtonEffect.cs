using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Si se deja vacío, se usará el transform del GameObject donde está este script")]
    public Button button;

    [SerializeField] private Vector3 hoverScale;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private float augmentScale = 1.1f;

    private Transform targetTransform;
    private Vector3 originalScale;

    void Awake()
    {
        targetTransform = (button != null) ? button.transform : transform;
        originalScale = targetTransform.localScale;
        hoverScale = originalScale * augmentScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Evitar que se acumulen tweens anteriores
        targetTransform.DOKill();
        targetTransform.DOScale(hoverScale, duration).SetEase(ease);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetTransform.DOKill();
        targetTransform.DOScale(originalScale, duration).SetEase(ease);
    }

    void OnDisable()
    {
        // Asegura restaurar la escala y limpiar tweens si se desactiva el objeto
        targetTransform.DOKill();
        targetTransform.localScale = originalScale;
    }
}
