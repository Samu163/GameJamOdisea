using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneTransitionManager.instance.ChangeScene("TemplesMap");
    }

    public void GoToOptions()
    {
        SceneTransitionManager.instance.ChangeScene("Options");
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void PlayButtonConfirm()
    {
        AudioManager.instance.PlayUIConfirmSfx();
    }
}
