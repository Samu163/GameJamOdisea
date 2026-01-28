using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    [Header("Camera Settings")]
    public GameObject mainCamera;
    [SerializeField] private Vector3 initialCameraPosition;
    [SerializeField] private Vector3 targetCameraPosition;
    [SerializeField] private float cameraTransitionDuration = 2f;
    [SerializeField] private float currentCameraTransitionTime = 0f;
    public CameraFollow cameraFollow;

    public GameObject player1;
    public GameObject player2;
    [SerializeField] private Vector3 player1InitialPosition;
    [SerializeField] private Vector3 player2InitialPosition;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentTemple = 1;
    public int templesUnlocked = 1;
    public List<TempleData> templesData;

    public LayerMask ignoreLayer;

    public LevelActivator levelActivator;

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

    public void AssignCamera(GameObject camera)
    {
        mainCamera = camera;
        initialCameraPosition = mainCamera.transform.position;
        targetCameraPosition = new Vector3(initialCameraPosition.x + 30f * (currentLevel - 1), initialCameraPosition.y, initialCameraPosition.z);
        mainCamera.transform.position = targetCameraPosition;

    }

    public void ResetPlayerPositions()
    {
        player1.GetComponent<Rigidbody>().MovePosition(new Vector3(player1InitialPosition.x + 30f * (currentLevel - 1), player1InitialPosition.y, player1InitialPosition.z));
        player1.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player1.GetComponent<Rigidbody>().useGravity = true;
        player1.GetComponent<CapsuleCollider>().excludeLayers = ignoreLayer;
        player2.GetComponent<Rigidbody>().MovePosition(new Vector3(player2InitialPosition.x + 30f * (currentLevel - 1), player2InitialPosition.y, player2InitialPosition.z));
        player2.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player2.GetComponent<Rigidbody>().useGravity = true;
        player2.GetComponent<CapsuleCollider>().excludeLayers = ignoreLayer;
    }

    public void DeactivatePlayers()
    {
        player1.SetActive(false);
        player2.SetActive(false);
    }

    public IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(0.2f);
        ResetPlayerPositions();
        cameraFollow.ResetCameraLevel();
    }

    public void SkipLevel()
    {
        Debug.Log("Skip Level");
        currentLevel++;
        cameraFollow.SkipNextLevel();
        ResetPlayerPositions();
    }

    public IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1f);
        cameraFollow.NextCameraPosition();
        ResetPlayerPositions();
        levelActivator.ActivateNextLevel(currentLevel);
    }

    public void NextLevelTransition()
    {
        Debug.Log("Next Level Transition");
        currentLevel++;
        SceneTransitionManager.instance.NextLevelTransition();
        StartCoroutine(NextLevel());
        

        if (currentLevel > templesData[currentTemple - 1].numberOfLevels)
        {
            templesUnlocked++;
            currentLevel = 1;
            SceneTransitionManager.instance.ChangeScene("TemplesMap");
        }

        
        
    }
}
