using UnityEngine;
using System;
using System.Collections.Generic;

public class State : MonoBehaviour
{

    public event Action<State, string, Dictionary<string, object>> OnTransition;

    // Virtual methods for the State Machine to call
    public virtual void Enter(Dictionary<string, object> extraArgs = null) { }
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }  
    public virtual void PhysicsUpdate() { } 

    // Helper method to trigger the transition
    protected void TransitionTo(string newStateName, Dictionary<string, object> extraArgs = null)
    {
        OnTransition?.Invoke(this, newStateName, extraArgs);
    }
}