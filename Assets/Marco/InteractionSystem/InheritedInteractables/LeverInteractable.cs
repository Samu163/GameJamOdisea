using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class LeverInteractable : InputInteractable
{
    bool isTurned = false;

    [Header("Visual Configuration")]
    public GameObject leverObject;
    public Vector3 rotationVector = new Vector3(0, 0, 30);

    [Header("Lever Events")]
    public UnityEvent onTurn;
    public UnityEvent onUnTurn;

    public override void Interact(Interactor interactor)
    { // Received the interactor from the player who interacted with self

        // 30 - -30
        if (! isTurned)
        {
            Deactivate(); // Disable interaction while turning
            leverObject.transform.DORotate(-rotationVector, 0.2f).OnComplete(() => OnChangeLeverState());
            onTurn?.Invoke();
        }
        else
        {
            Deactivate(); // Disable interaction while turning
            leverObject.transform.DORotate(rotationVector, 0.2f).OnComplete(() => OnChangeLeverState());
            onUnTurn?.Invoke();
        }
        
        base.Interact(interactor);
    }

    private void OnChangeLeverState()
    {
        isTurned = !isTurned;
        Activate(); // Re-activate interaction after turning the lever
    }

}
