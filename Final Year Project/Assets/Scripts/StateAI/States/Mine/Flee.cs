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
    private const float FLEE_DISTANCE = 5F;
    public float TimeStuck;
    private Vector3 _lastPosition = Vector3.zero;

    public Flee(StateAiPlayerController player, NavMeshAgent navMeshAgent, StateAiEnemyDetection enemyDetector)
    {
        this.player = player;
        _navMeshAgent = navMeshAgent;
        _enemyDetector = enemyDetector;
    }

    public void OnEnter()
    {
        TimeStuck = 0f;
        _navMeshAgent.enabled = true;
        this.player.decision = "continue";
        this.player.attackDecision_flee = false;

        var away = getRandomPoint();
        _navMeshAgent.SetDestination(away);
        Debug.Log("ENTERED FLEE");
    }

    public void Tick()
    {
        if (Vector3.Distance(this.player.transform.position, _lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPosition = this.player.transform.position;
    }

    private Vector3 getRandomPoint()
    {
        Vector3 direction = new Vector3();
        if (_enemyDetector.getVisibleEnemyTargets().Count > 0){
            direction = this.player.transform.position - _enemyDetector.getFirstSeenEnemy();    
        }
        else {
            direction = this.player.transform.forward;    
        }
        direction.Normalize();

        var endPoint = this.player.transform.position + (direction * FLEE_DISTANCE);
        Debug.Log(endPoint);
        if (NavMesh.SamplePosition(endPoint, out var hit, 10f, NavMesh.AllAreas))
        {
            Vector3 validPosition = new Vector3(hit.position.x, 1f, hit.position.z);
            this.player.fleeTarget = validPosition;
            return validPosition;
        }

        return this.player.transform.position;
    }

    public void OnExit()
    {
        this.player.decision = "continue";
        this.player.attackDecision_flee = false;
        Debug.Log("EXITTED FLEE");

        // _navMeshAgent.speed = _initialSpeed;
        // _navMeshAgent.enabled = false;
        // _animator.SetBool(FleeHash, false);
    }
}