using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

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

    public struct LevelData
    {
        public int levelIndex;
        public int templeIndex;
    }

    public struct TempleData
    {
        public int templeIndex;
        public int numberOfLevels;
        public List<LevelData> levels;
    }

    public List<TempleData> temples = new List<TempleData>();

    [Header("Level Settings")]
    public int currentLevel = 0;
    public int currentTemple = 1;

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
        initialCameraPosition = mainCamera.transform.position;
        targetCameraPosition = initialCameraPosition;
    }

    private void Update()
    {
        if (isChangingLevel)
        {
            ChangeLevelTransition();
        }
        
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

    public void NewLevelPositions()
    {
        player1.transform.position = new Vector3(player1InitialPosition.x + 30f * currentLevel, player1InitialPosition.y, player1InitialPosition.z);
        player2.transform.position = new Vector3(player2InitialPosition.x + 30f * currentLevel, player2InitialPosition.y, player2InitialPosition.z);
    }

    public void NextLevelTransition()
    {
        currentLevel++;
        isChangingLevel = true;

        targetCameraPosition = new Vector3(initialCameraPosition.x + 30f * currentLevel, initialCameraPosition.y, initialCameraPosition.z);
        
    }
}
