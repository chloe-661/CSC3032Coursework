using UnityEngine;
using System.Collections;
using System.Collections.Generic;

internal class Patrol : IState
{
    private StateAiPlayerController player;
    private readonly UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private float _resourcesPerSecond = 3;
    private float PATROL_RADIUS = 5f;
    private Vector3 away;

    private float _nextTakeResourceTime;
    private static readonly int Harvest = Animator.StringToHash("Harvest");

    public Patrol(StateAiPlayerController player, UnityEngine.AI.NavMeshAgent navMeshAgent, Animator animator)
    {
        this.player = player;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }



    public void OnEnter()
    {
        Debug.Log("ENTERED PATROL");
        _navMeshAgent.enabled = true;
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isPatrolling", true);
        _animator.SetBool("isAttacking", false);
        away = getRandomPoint();
        _navMeshAgent.SetDestination(away);
    }

    public void Tick()
    {
        if (Vector3.Distance(this.player.transform.position, away) < 0.5f){
            away = getRandomPoint();
            _navMeshAgent.SetDestination(away);
        }
    }

    private Vector3 getRandomPoint()
    {
        List<GameObject> patrolPoints = new List<GameObject>();
        Transform parent = this.player.Target.transform.parent;

        foreach (Transform child in parent)
        {
            patrolPoints.Add(child.gameObject);
        }

        int rndNum = Random.Range(0, patrolPoints.Count);
        return patrolPoints[rndNum].transform.position;
    }

    public void OnExit()
    {
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isPatrolling", false);
        _animator.SetBool("isAttacking", false);
        Debug.Log("EXIT PATROL");
    }
}