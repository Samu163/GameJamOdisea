using UnityEngine;

public class ControlShower : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;

    private void Start()
    {
        dialogueController.enableEvent.AddListener(() => gameObject.SetActive(false));

        dialogueController.disableEvent.AddListener(() => Invoke("Activate", 0.6f));
    }

    void Activate()
    {
        gameObject.SetActive(true);
    }
}
