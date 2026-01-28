using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class CaminoAnimation : MonoBehaviour
{

    public List<GameObject> camino1;
    public List<GameObject> camino2;
    public List<GameObject> camino3;

    public Vector3 originalScale;

    public int toAnimateIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = camino1[0].transform.localScale;

        switch (LevelManager.instance.templesUnlocked)
        {
            case 1:
                foreach (GameObject go in camino1)
                {
                    go.SetActive(false);
                }
                foreach (GameObject go in camino2)
                {
                    go.SetActive(false);
                }
                foreach (GameObject go in camino3)
                {
                    go.SetActive(false);
                }
                break;
            case 2:
                foreach (GameObject go in camino1)
                {
                    go.SetActive(true);
                }
                foreach (GameObject go in camino2)
                {
                    go.SetActive(false);
                }
                foreach (GameObject go in camino3)
                {
                    go.SetActive(false);
                }
                break;
            case 3:
                foreach (GameObject go in camino1)
                {
                    go.SetActive(true);
                }
                foreach (GameObject go in camino2)
                {
                    go.SetActive(true);
                }
                foreach (GameObject go in camino3)
                {
                    go.SetActive(false);
                }
                break;
            default:
                break;
        }

        StartCoroutine(StartAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartAnimation()
    {
        toAnimateIndex = 0;
        yield return new WaitForSeconds(1f);
        AnimateUnlockedCamino(toAnimateIndex);
    }

    public IEnumerator PlaySFX()
    {
        yield return new WaitForSeconds(0.2f);
        AudioManager.instance.PlayCaminoSFX();
    }

    public void AnimateUnlockedCamino(int index)
    {
        switch (LevelManager.instance.templesUnlocked)
            {
            case 1:
                if (index >= camino1.Count) return;
                camino1[toAnimateIndex].SetActive(true);
                StartCoroutine(PlaySFX());
                camino1[toAnimateIndex].transform.DOScale(originalScale, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    toAnimateIndex++;
                    AnimateUnlockedCamino(toAnimateIndex);
                    
                });
                break;
            case 2:
                if (index >= camino2.Count) return;
                camino2[toAnimateIndex].SetActive(true);
                StartCoroutine(PlaySFX());
                camino2[toAnimateIndex].transform.DOScale(originalScale, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    toAnimateIndex++;
                    AnimateUnlockedCamino(toAnimateIndex);
                    
                });
                break;
            case 3:
                if (index >= camino3.Count) return;
                camino3[toAnimateIndex].SetActive(true);
                StartCoroutine(PlaySFX());
                camino3[toAnimateIndex].transform.DOScale(originalScale, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    toAnimateIndex++;
                    AnimateUnlockedCamino(toAnimateIndex);
                    
                });
                break;
            default:
                break;
        }
    }
}
