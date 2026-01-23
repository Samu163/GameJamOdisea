using UnityEngine;

public class LaserReceiver : MonoBehaviour
{

    public bool isReceivingLaser = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isReceivingLaser)
        {
            // Logic for when the receiver is receiving the laser
            Debug.Log("Laser Receiver is active and receiving laser.");
        }
    }

    public void ActivateReceiver()
    {
        isReceivingLaser = true;
    }

    public void DeactivateReceiver()
    {
        isReceivingLaser = false;
    }
}
