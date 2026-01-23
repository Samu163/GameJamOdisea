using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class StateMachine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private State _initialState;
    [SerializeField] private bool _reInitializeOnLoad = false;
    [SerializeField] private bool _reEnterIfTransitionToSameState = true;

    // State Management
    public State CurrentState { get; private set; }
    private Dictionary<string, State> _states = new Dictionary<string, State>();

    // Events (Signals)
    public event Action<string> OnChangeState;
    public event Action OnInitialize;

    // C# Property for name_current_state
    public string NameCurrentState
    {
        get
        {
            if (CurrentState != null)
            {
                return CurrentState.gameObject.name.ToLower();
            }
            return "";
        }
    }

    private void Start()
    {
        // Loop through children and add States to dictionary
        foreach (Transform child in transform)
        {
            State stateComponent = child.GetComponent<State>();

            if (stateComponent != null)
            {
                // Key is the Game Object name
                _states[child.name.ToLower()] = stateComponent;

                // Connect the function to the transition event (Signal connection)
                stateComponent.OnTransition += OnChildTransition;
            }
        }

        if (_initialState != null)
        {
            bool isLoading = false; 

            if ((CurrentState == null && !isLoading) || _reInitializeOnLoad)
            {
                EnterState(_initialState);
                OnInitialize?.Invoke();
            }
        }
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.LogicUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.PhysicsUpdate();
        }
    }

    public void ChangeState(State state, string newStateName, Dictionary<string, object> extraArgs = null)
    {
        OnChildTransition(state, newStateName, extraArgs);
    }

    private void OnChildTransition(State state, string newStateName, Dictionary<string, object> extraArgs = null)
    {
        if (state != CurrentState) return;

        // Get the new state from the dictionary
        string stateKey = newStateName.ToLower();

        if (!_states.ContainsKey(stateKey))
        {
            Debug.LogError($"StateMachine: Couldn't find state '{stateKey}'");
            return;
        }

        State newState = _states[stateKey];

        // Check re-entry logic
        if (!_reEnterIfTransitionToSameState && newState == state) return;

        // Perform the switch
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }

        CurrentState = newState;

        // Pass extra args if they exist
        CurrentState.Enter(extraArgs);

        OnChangeState?.Invoke(newStateName);
    }

    // Helper to enter state cleanly on initialization
    private void EnterState(State newState)
    {
        CurrentState = newState;
        CurrentState.Enter();
        OnChangeState?.Invoke(newState.gameObject.name);
    }


    private void OnDestroy()
    {
        // Clean up events to prevent memory leaks
        foreach (var state in _states.Values)
        {
            state.OnTransition -= OnChildTransition;
        }
    }
}