using UnityEngine;

public class PressureButtonInteractable : TriggerInteractable
{
    [SerializeField] private GameObject door;


    public override void Activate()
    {
        door.gameObject.SetActive(false);
    }

    public override void Deactivate()
    {

       door.gameObject.SetActive(true);
    }
}
