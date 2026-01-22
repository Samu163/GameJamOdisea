using UnityEngine;

public class LevelComplete : MonoBehaviour
{

    private bool hasPlayer1Completed = false;
    private bool hasPlayer2Completed = false;

    private void LevelCompleted()
    {
        LevelManager.instance.NextLevelTransition();
        hasPlayer1Completed = false;
        hasPlayer2Completed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            hasPlayer1Completed = true;
        }
        else if (other.CompareTag("Player2"))
        {
            hasPlayer2Completed = true;
        }
        if (hasPlayer1Completed && hasPlayer2Completed)
        {
            LevelCompleted();
        }
    }



}
