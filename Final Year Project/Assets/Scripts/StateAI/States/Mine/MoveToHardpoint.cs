using UnityEngine;
using UnityEngine.AI;

internal class MoveToHardpoint : IState
{
    private readonly StateAiPlayerController player;
    private readonly NavMeshAgent navMeshAgent;
    private readonly Animator animator;
    private TrailRenderer trailRenderer;
    
    private Vector3 lastPosition = Vector3.zero;
    public float TimeStuck;

    public MoveToHardpoint(StateAiPlayerController player, NavMeshAgent navMeshAgent, Animator animator, TrailRenderer trailRenderer)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.animator = animator;
        this.trailRenderer = trailRenderer;
    }
    
    public void Tick()
    {
        if (Vector3.Distance(this.player.transform.position, this.lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        this.lastPosition = this.player.transform.position;
    }

    public void OnEnter()
    {
        TimeStuck = 0f;
        this.navMeshAgent.enabled = true;
         this.trailRenderer.enabled = true;
        this.navMeshAgent.SetDestination(this.player.Target.transform.position);
        this.animator.SetBool("isRunning", true);
        this.animator.SetBool("isPatrolling", false);
        this.animator.SetBool("isAttacking", false);
    }

    public void OnExit()
    {
        this.navMeshAgent.enabled = false;
        this.animator.SetBool("isRunning", false);
        this.animator.SetBool("isPatrolling", false);
        this.animator.SetBool("isAttacking", false);
    }
}