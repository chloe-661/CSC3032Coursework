using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class StayAttack : IState
{
    private readonly StateAiPlayerController player;
    private PlayerStatus ps;
    private NavMeshAgent _navMeshAgent;
    private readonly StateAiEnemyDetection _enemyDetector;

    public StayAttack(StateAiPlayerController player, NavMeshAgent navMeshAgent, StateAiEnemyDetection enemyDetector)
    {
        this.player = player;
        _navMeshAgent = navMeshAgent;
        _enemyDetector = enemyDetector;
    }

    public void OnEnter()
    {

    }

    public void Tick()
    {
        // if (_enemyDetector.Count > 0)
        // {
        //     //Attack nearest

        // }
    }

    public void OnExit()
    {

    }
}