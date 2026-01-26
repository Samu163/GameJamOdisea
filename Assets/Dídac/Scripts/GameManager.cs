using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public PlayerSelector playerSelector;
    public InputDevice player1Device;
    public InputDevice player2Device;

    public bool hasGameStarted = false;

    public string currentSceneName = "";

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
        SceneTransitionManager.instance.ChangeScene(currentSceneName);
        StartCoroutine(ResetPlayersCoroutine());
    }

    public IEnumerator ResetPlayersCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        playerSelector.ResetLevelPlayers();
    }

    public void StartGame()
    {
        SceneTransitionManager.instance.ChangeScene("TemplesMap");
    }

    public void LoadTemple1()
    {
        LevelManager.instance.currentTemple = 1;
        SceneTransitionManager.instance.ChangeScene("Temple 1");
    }

    public void LoadTemple2()
    {
        LevelManager.instance.currentTemple = 2;
        SceneTransitionManager.instance.ChangeScene("Temple 2");
    }

    public void LoadTemple3()
    {
        LevelManager.instance.currentTemple = 3;
        SceneTransitionManager.instance.ChangeScene("Temple 3");
    }
}
