using UnityEngine;
using UnityEngine.AI;

internal class MoveToHardpoint : IState
{
    private readonly StateAiPlayerController player;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    // private static readonly int Speed = Animator.StringToHash("Speed");

    private Vector3 _lastPosition = Vector3.zero;
    
    public float TimeStuck;

    public MoveToHardpoint(StateAiPlayerController player, NavMeshAgent navMeshAgent, Animator animator)
    {
        this.player = player;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }
    
    public void Tick()
    {
        if (Vector3.Distance(this.player.transform.position, _lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPosition = this.player.transform.position;
    }

    public void OnEnter()
    {
        TimeStuck = 0f;
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(this.player.Target.transform.position);
        _animator.SetBool("isRunning", true);
        _animator.SetBool("isPatrolling", false);
        _animator.SetBool("isAttacking", false);
    }

    public void OnExit()
    {
        _navMeshAgent.enabled = false;
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isPatrolling", false);
        _animator.SetBool("isAttacking", false);
    }
}