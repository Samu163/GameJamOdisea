using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueController : MonoBehaviour
{

    [Header("Dialogue UI Elements")]
    [SerializeField] private Sprite npcSprite;
    [SerializeField] private Image talkingSprite;
    [SerializeField] private Sprite fatherSprite;
    [SerializeField] private Sprite motherSprite;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private float typingSpeed = 0.025f;

    private Vector2 originalScale;


    private int currentIndex = 0;
    private bool isTyping = false;
    private string[] currentDialogue;
    private int currentDialogueIndex = 0;
    private string[] orderOfTalkers;

    private void Start()
    {
        UIController.instance.dialogueController = this;
        originalScale = dialogueBox.GetComponent<RectTransform>().localScale;
    }

    public void ShowDialogueBox(Sprite NPCsprite, string[] nameTalker, string[] dialogue)
    {
        npcSprite = NPCsprite;
        currentDialogue = dialogue;
        orderOfTalkers = nameTalker;

        switch (orderOfTalkers[currentDialogueIndex])
        {
            case "NPC":
                talkingSprite.sprite = npcSprite;
                break;
            case "Father":
                talkingSprite.sprite = fatherSprite;
                break;
            case "Mother":
                talkingSprite.sprite = motherSprite;
                break;
            default:
                talkingSprite.sprite = null;
                break;
        }

        dialogueBox.SetActive(true);
        dialogueBox.GetComponent<RectTransform>().DOScale(originalScale, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        StartCoroutine(StartTyping(dialogue[0], 0.5f));

    }

    public void HideDialogueBox()
    {
        dialogueBox.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            LevelManager.instance.NextLevelTransition();
            dialogueText.text = "";
            dialogueBox.SetActive(false);
            UIController.instance.DeactivateDialogue();
            
            
        });
    }

    public void SkipTyping()
    {
        if (isTyping)
        {
            dialogueText.text = currentDialogue[currentDialogueIndex];
            isTyping = false;
        }
    }

    public void NextDialogueLine()
    {

        if (!isTyping)
        {
            currentDialogueIndex++;

            if (currentDialogueIndex >= currentDialogue.Length)
            {
                HideDialogueBox();
                currentDialogueIndex = 0;
                return;
            }

            StartCoroutine(StartTyping(currentDialogue[currentDialogueIndex], 0.25f));
        }
    }

    public IEnumerator StartTyping(string dialogue, float delayBeforeStart)
    {
        yield return new WaitForSeconds(delayBeforeStart);
        isTyping = true;
        currentIndex = 0;
        dialogueText.text = "";

        AudioManager.instance.PlayTalking();

        switch (orderOfTalkers[currentDialogueIndex])
        {
            case "NPC":
                talkingSprite.sprite = npcSprite;
                break;
            case "Father":
                talkingSprite.sprite = fatherSprite;
                break;
            case "Mother":
                talkingSprite.sprite = motherSprite;
                break;
            default:
                talkingSprite.sprite = null;
                break;
        }

        while (currentIndex < dialogue.Length && isTyping)
        {
            dialogueText.text += dialogue[currentIndex];
            currentIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        AudioManager.instance.StopTalking();
    }
}
