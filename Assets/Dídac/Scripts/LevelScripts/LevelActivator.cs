using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LevelActivator : MonoBehaviour
{
    public List<GameObject> levels;

    private void Start()
    {
        LevelManager.instance.levelActivator = this;
        ActivateNextLevel(LevelManager.instance.currentLevel);
    }

    public void ActivateNextLevel(int index)
    {

        foreach (GameObject level in levels)
        {
            level.SetActive(false);
        }

        levels[index - 1].SetActive(true);

        
    }
}
