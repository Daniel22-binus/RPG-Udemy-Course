using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }

    public void initialize (EnemyState _startState)
    {
        this.currentState = _startState;
        currentState.Enter();
    }

    public void changeState (EnemyState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
    
}
