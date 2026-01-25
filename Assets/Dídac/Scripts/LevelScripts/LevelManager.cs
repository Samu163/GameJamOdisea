using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    public GameObject mainCamera;
    [SerializeField] private Vector3 initialCameraPosition;
    [SerializeField] private Vector3 targetCameraPosition;

    public GameObject player1;
    public GameObject player2;
    [SerializeField] private Vector3 player1InitialPosition;
    [SerializeField] private Vector3 player2InitialPosition;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentTemple = 1;
    public int templesUnlocked = 1;
    public List<TempleData> templesData;

    private bool isChangingLevel = false;

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

    private void Start()
    {
       
    }


    private void Update()
    {
        if (isChangingLevel)
        {
            ChangeLevelTransition();
        }
        
    }

    public void AssignCamera(GameObject camera)
    {
        mainCamera = camera;
        initialCameraPosition = mainCamera.transform.position;
        targetCameraPosition = new Vector3(initialCameraPosition.x + 30f * (currentLevel - 1), initialCameraPosition.y, initialCameraPosition.z);
        mainCamera.transform.position = targetCameraPosition;

    }

    private void ChangeLevelTransition()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime);
        if (Vector3.Distance(mainCamera.transform.position, targetCameraPosition) < 0.1f)
        {
            mainCamera.transform.position = targetCameraPosition;
            NewLevelPositions();
            isChangingLevel = false;
        }
    }

    public void SpawnPlayersOnInitialPosition(int index)
    {
        if (index == 1)
        {
            player1.transform.position = player1InitialPosition;
        }
        else if (index == 2)
        {
            player2.transform.position = player2InitialPosition;
        }
    }

    public void ResetPlayerPositions()
    {
        player1.transform.position = new Vector3(player1InitialPosition.x + 30f * (currentLevel - 1), player1InitialPosition.y, player1InitialPosition.z);
        player2.transform.position = new Vector3(player2InitialPosition.x + 30f * (currentLevel - 1), player2InitialPosition.y, player2InitialPosition.z);
    }

    public void NewLevelPositions()
    {
        player1.transform.position = new Vector3(player1InitialPosition.x + 30f * (currentLevel - 1), player1InitialPosition.y, player1InitialPosition.z);
        player2.transform.position = new Vector3(player2InitialPosition.x + 30f * (currentLevel - 1), player2InitialPosition.y, player2InitialPosition.z);
    }

    public void NextLevelTransition()
    {
        currentLevel++;
        isChangingLevel = true;
        targetCameraPosition = new Vector3(initialCameraPosition.x + 30f * (currentLevel - 1), initialCameraPosition.y, initialCameraPosition.z);

        if (currentLevel > templesData[currentTemple - 1].numberOfLevels)
        {
            templesUnlocked++;
            currentLevel = 1;
            isChangingLevel = false;
            SceneManager.LoadScene("TemplesMap");
        }

        
        
    }
}
