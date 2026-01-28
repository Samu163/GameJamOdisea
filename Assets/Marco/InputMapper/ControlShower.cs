using UnityEngine;
using DG.Tweening;

public class ControlShower : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private RectTransform momUI;
    [SerializeField] private RectTransform dadtUI;

    [Header("Animation Settings")]
    [SerializeField] private float slideOffDuration = 0.2f;
    [SerializeField] private float slideOnDuration = 0.5f;
    [SerializeField] private float slideOffset = 1000f; // How far they fly off-screen

    private float momOriginalX;
    private float dadOriginalX;

    private void Start()
    {
        //Cache the starting positions
        momOriginalX = momUI.anchoredPosition.x;
        dadOriginalX = dadtUI.anchoredPosition.x;

        // Hook up the listeners with your requested delays
        if (dialogueController != null)
        {
            dialogueController.enableEvent.AddListener(Deactivate);
            dialogueController.disableEvent.AddListener(() => Invoke("Activate", 0.6f));
        }
    }

    void Activate()
    {
        // Ensure object is active so we can see the tween
        gameObject.SetActive(true);

        // Slide back to original positions
        momUI.DOAnchorPosX(momOriginalX, slideOnDuration).SetEase(Ease.OutBack);
        dadtUI.DOAnchorPosX(dadOriginalX, slideOnDuration).SetEase(Ease.OutBack);
    }

    void Deactivate()
    {
        // Dad goes Left (-), Mom goes Right (+)
        dadtUI.DOAnchorPosX(dadOriginalX - slideOffset, slideOffDuration).SetEase(Ease.InBack);
        momUI.DOAnchorPosX(momOriginalX + slideOffset, slideOffDuration).SetEase(Ease.InBack);
    }
}