using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public void GoToTitle()
    {
        SceneTransitionManager.instance.ChangeScene("MainMenu");
    }
}
