using UnityEngine;
using UnityEngine.AI;

public class Flee : IState
{
    private StateAiPlayerController player;
    private NavMeshAgent _navMeshAgent;
    private readonly StateAiEnemyDetection _enemyDetector;
    // private Animator _animator;
    // private readonly ParticleSystem _particleSystem;
    // private static readonly int FleeHash = Animator.StringToHash("Flee");

    // private float _initialSpeed;
    // private const float FLEE_SPEED = 6F;
    private const float FLEE_DISTANCE = 15F;
    public float TimeStuck;
    private Vector3 _lastPosition = Vector3.zero;

    public Flee(StateAiPlayerController player, NavMeshAgent navMeshAgent, StateAiEnemyDetection enemyDetector)
    {
        Debug.Log("Flee Constructor");
        this.player = player;
        _navMeshAgent = navMeshAgent;
        _enemyDetector = enemyDetector;
    }

    public void OnEnter()
    {
        Debug.Log("Flee OnEnter");
        TimeStuck = 0f;
        _navMeshAgent.enabled = true;

        var away = getRandomPoint();
        _navMeshAgent.SetDestination(away);
    }

    public void Tick()
    {
        if (Vector3.Distance(this.player.transform.position, _lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPosition = this.player.transform.position;
    }

    private Vector3 getRandomPoint()
    {
        Debug.Log("Entered getRandomPoint");
        var directionFromEnemy = this.player.transform.position - _enemyDetector.getFirstSeenEnemy();
        directionFromEnemy.Normalize();

        var endPoint = this.player.transform.position + (directionFromEnemy * FLEE_DISTANCE);
        Debug.Log(endPoint);
        if (NavMesh.SamplePosition(endPoint, out var hit, 10f, NavMesh.AllAreas))
        {
            Debug.Log("Found valid point");
            Vector3 validPosition = new Vector3(hit.position.x, 1.3f, hit.position.z);
            this.player.fleeTarget = validPosition;
            return validPosition;
        }

        return this.player.transform.position;
    }

    public void OnExit()
    {
        Debug.Log("Flee OnExit");
        this.player.attackDecision_flee = false;
        // _navMeshAgent.speed = _initialSpeed;
        // _navMeshAgent.enabled = false;
        // _animator.SetBool(FleeHash, false);
    }
}