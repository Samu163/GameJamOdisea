using UnityEngine;

public class UIController : MonoBehaviour
{

    public static UIController instance;

    public DialogueController dialogueController;

    public bool isDialogueActive = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLineDialogue()
    {
        dialogueController.NextDialogueLine();
    }

    public void SkipDialogue()
    {
        dialogueController.SkipTyping();
    }

    public void ActivateDialogue(Sprite NPCsprite, string[] nameTalker, string[] dialogue)
    {
        dialogueController.ShowDialogueBox(NPCsprite, nameTalker, dialogue);
        isDialogueActive = true;
    }

    public void DeactivateDialogue()
    {
        isDialogueActive = false;
    }

}
