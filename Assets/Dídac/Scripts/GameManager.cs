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

    public void StartGame()
    {
        SceneManager.LoadScene("TemplesMap");
    }

    public void LoadTemple1()
    {
        LevelManager.instance.currentTemple = 1;
        SceneManager.LoadScene("GameLoopTesting");
    }

    public void LoadTemple2()
    {
        LevelManager.instance.currentTemple = 2;
        SceneManager.LoadScene("Temple_2");
    }

    public void LoadTemple3()
    {
        LevelManager.instance.currentTemple = 3;
        SceneManager.LoadScene("Temple_3");
    }
}
