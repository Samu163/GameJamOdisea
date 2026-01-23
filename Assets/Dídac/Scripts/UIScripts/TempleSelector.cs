using UnityEngine;
using UnityEngine.UI;

public class TempleSelector : MonoBehaviour
{

    public Button Temple1;
    public Button Temple2;
    public Button Temple3;

    private void Awake()
    {
        Temple1.onClick.AddListener(GameManager.instance.LoadTemple1);
        Temple2.onClick.AddListener(GameManager.instance.LoadTemple2);
        Temple3.onClick.AddListener(GameManager.instance.LoadTemple3);
    }

    private void Update()
    {
        if (LevelManager.instance.templesUnlocked == 1)
        {
            Temple1.interactable = true;
            Temple2.interactable = false;
            Temple3.interactable = false;
        }
        else if (LevelManager.instance.templesUnlocked == 2)
        {
            Temple1.interactable = true;
            Temple2.interactable = true;
            Temple3.interactable = false;
        }
        else if (LevelManager.instance.templesUnlocked == 3)
        {
            Temple1.interactable = true;
            Temple2.interactable = true;
            Temple3.interactable = true;
        }
    }
}
