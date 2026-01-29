using UnityEngine;
using System.Collections.Generic;

public class PlayerSelectablePhoto : MonoBehaviour
{

    public int playerIndex = 1;
    public List<GameObject> onjectstoDisableOnDisable;
    private PlayerSelector playerSelector;

    private void Awake()
    {
        playerSelector = FindAnyObjectByType<PlayerSelector>();
        playerSelector.OnPlayerJoined.AddListener(OnPlayerJoined);
    }

    private void OnDisable()
    {
        foreach (GameObject obj in onjectstoDisableOnDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    public void OnPlayerJoined(int pIndex)
    {
        // Check if the player that joined is ours

        if (pIndex == playerIndex)
        {
            gameObject.SetActive(false);
            playerSelector.OnPlayerJoined.RemoveListener(OnPlayerJoined);
        }

       
    }
}
