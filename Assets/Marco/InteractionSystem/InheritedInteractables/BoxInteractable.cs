using UnityEngine;

public class BoxInteractable : InputInteractable
{

    public override void Interact(Interactor interactor)
    { // Received the interactor from the player who interacted with self


        // Change player's state to carrying state passing this box
        interactor.playerStateMachine.ChangeState(interactor.playerStateMachine.CurrentState ,
            "CarryingState", new System.Collections.Generic.Dictionary<string, object> { { "box", this } });

        base.Interact(interactor);
        Deactivate(); // Interactability is disabled because the player is carrying it
        // TODO: should also disable the interactor so that you can't interact with other things while carrying
    }

    //public void Drop(Interactor interactor)
    //{
    //    // Re-enable interaction when dropped
    //    Activate();
    //}

}
