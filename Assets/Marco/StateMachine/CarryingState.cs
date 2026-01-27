using UnityEngine;

public class CarryingState : State
{
    BoxInteractable currentBox;
    public Transform carryPoint;

    // Store the original local position of the carry point
    private Vector3 defaultCarryPosition;

    void Awake()
    {
        if (carryPoint != null)
        {
            defaultCarryPosition = carryPoint.localPosition;
        }
    }

    // The box/ carryable object changes to this state
    public override void Enter(System.Collections.Generic.Dictionary<string, object> extraArgs = null)
    {
        if (extraArgs != null && extraArgs.ContainsKey("box"))
        {
            currentBox = extraArgs["box"] as BoxInteractable;
            if (currentBox != null)
            {
                Collider boxCollider = currentBox.meshCollider;

                float forwardOffset = 0f;
                if (boxCollider != null)
                {
                    forwardOffset = boxCollider.bounds.extents.z;
                }

                // Move the carry point forward away from the player, Idk why 1.5 offset
                carryPoint.localPosition = defaultCarryPosition + (Vector3.forward * forwardOffset * 2.8f);

                //Parent the box to the carry point
                currentBox.Grab(carryPoint);
                AudioManager.instance.PlayCogerBox();


            }
        }
    }

    public override void Exit()
    {
        //  Reset the carry position when dropping
        if (carryPoint != null)
        {
            carryPoint.localPosition = defaultCarryPosition;
        }
        if (currentBox != null)
        {
            //Unparent the box
            currentBox.Drop(null);
            currentBox = null;
        }
    }
}