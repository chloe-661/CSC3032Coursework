using UnityEngine;
using System.Collections;
using System.Collections.Generic;

internal class Patrol : IState
{
    private StateAiPlayerController player;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private Animator animator;

    private const float PATROL_RADIUS = 5f;
    private const float PATROL_SPEED = 1f;
    private const float RUN_SPEED = 3.5f;

    private Vector3 away;

    public float TimeStuck;
    private Vector3 lastPosition = Vector3.zero;

    public Patrol(StateAiPlayerController player, UnityEngine.AI.NavMeshAgent navMeshAgent, Animator animator)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.animator = animator;
    }

    public void OnEnter()
    {
        this.navMeshAgent.enabled = true;
        this.navMeshAgent.speed = PATROL_SPEED;

        away = getRandomPoint();
        this.navMeshAgent.SetDestination(away);

        this.animator.SetBool("isRunning", false);
        this.animator.SetBool("isPatrolling", true);
        this.animator.SetBool("isAttacking", false);
    }

    public void Tick()
    {
        if (Vector3.Distance(this.player.transform.position, away) < 0.5f){
            away = getRandomPoint();
            this.navMeshAgent.SetDestination(away);
        }

        if (Vector3.Distance(this.player.transform.position, this.lastPosition) <= 0f){
            TimeStuck += Time.deltaTime;
        }
        this.lastPosition = this.player.transform.position;
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
        float x = patrolPoints[rndNum].transform.position.x;
        float y = 1f;
        float z = patrolPoints[rndNum].transform.position.z;
        Vector3 point = new Vector3(x,y,z);

        this.player.patrolTarget = point;

        return point;
    }

    public void OnExit()
    {
        this.animator.SetBool("isRunning", false);
        this.animator.SetBool("isPatrolling", false);
        this.animator.SetBool("isAttacking", false);
    }
}