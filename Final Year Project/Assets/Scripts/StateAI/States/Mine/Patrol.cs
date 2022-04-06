using UnityEngine;

internal class Patrol : IState
{
    private StateAiPlayerController player;
    private readonly UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private float _resourcesPerSecond = 3;

    private float _nextTakeResourceTime;
    private static readonly int Harvest = Animator.StringToHash("Harvest");

    public Patrol(StateAiPlayerController player, UnityEngine.AI.NavMeshAgent navMeshAgent, Animator animator)
    {
        this.player = player;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }

    public void Tick()
    {
        Debug.Log("PATROLLING...");
        // if (this.player.Target != null)
        // {
        //     if (_nextTakeResourceTime <= Time.time)
        //     {
        //         _nextTakeResourceTime = Time.time + (1f / _resourcesPerSecond);
        //         this.player.TakeFromTarget();
        //         _animator.SetTrigger(Harvest);
        //     }
        // }
    }

    public void OnEnter()
    {
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isPatrolling", true);
        _animator.SetBool("isAttacking", false);
    }

    public void OnExit()
    {
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isPatrolling", false);
        _animator.SetBool("isAttacking", false);
    }
}