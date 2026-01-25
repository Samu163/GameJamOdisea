using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public bool hasGameStarted = false;

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

    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneTransitionManager.instance.ChangeScene(currentScene.name);
        LevelManager.instance.ResetPlayerPositions();
    }

    public void StartGame()
    {
        SceneTransitionManager.instance.ChangeScene("TemplesMap");
    }

    public void LoadTemple1()
    {
        LevelManager.instance.currentTemple = 1;
        SceneTransitionManager.instance.ChangeScene("GameLoopTesting");
    }

    public void LoadTemple2()
    {
        LevelManager.instance.currentTemple = 2;
        SceneTransitionManager.instance.ChangeScene("Temple_2");
    }

    public void LoadTemple3()
    {
        LevelManager.instance.currentTemple = 3;
        SceneTransitionManager.instance.ChangeScene("Temple_3");
    }
}
