using UnityEngine;

public class FireChorro : MonoBehaviour
{

    public GameObject fire;
    [SerializeField] private float activationTimer = 0f;
    [SerializeField] private float activeDuration = 3.0f;
    [SerializeField] private float inactiveDuration = 5.0f;
    private bool isActive = false;

    private void Start()
    {
        fire.SetActive(false);
        isActive = false;
        activationTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            activationTimer += Time.deltaTime;
            if (activationTimer >= activeDuration)
            {
                fire.SetActive(false);
                isActive = false;
                activationTimer = 0f;
            }
        }
        else
        {
            activationTimer += Time.deltaTime;
            if (activationTimer >= inactiveDuration)
            {
                fire.SetActive(true);
                isActive = true;
                activationTimer = 0f;
            }
        }
    }
}
