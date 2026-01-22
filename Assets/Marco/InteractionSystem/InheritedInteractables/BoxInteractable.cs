using UnityEngine;

public class BoxInteractable : InputInteractable
{

    public void Interact()
    {
        print("You are interacting with " + gameObject.name + "!");
    }

}
