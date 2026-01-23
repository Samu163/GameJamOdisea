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

    public override void Enter(System.Collections.Generic.Dictionary<string, object> extraArgs = null)
    {
        if (extraArgs != null && extraArgs.ContainsKey("box"))
        {
            currentBox = extraArgs["box"] as BoxInteractable;
            if (currentBox != null)
            {
                Collider boxCollider = currentBox.GetComponent<Collider>();

                float forwardOffset = 0f;
                if (boxCollider != null)
                {
                    forwardOffset = boxCollider.bounds.extents.z;
                }

                // Move the carry point forward away from the player
                // We add a little base offset (forwardOffset)
                carryPoint.localPosition = defaultCarryPosition + (Vector3.forward * forwardOffset);
                //Parent the box to the carry point
                currentBox.transform.SetParent(carryPoint);
                // Reset box position to 0,0,0 relative to the new carry point
                currentBox.transform.localPosition = Vector3.zero;
                currentBox.transform.localRotation = Quaternion.identity; // Reset rotation
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
    }
}