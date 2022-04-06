using UnityEngine;
using UnityEngine.AI;

public class Flee : IState
{
    private StateAiPlayerController player;
    private NavMeshAgent navMeshAgent;
    private StateAiEnemyDetection enemyDetector;
    
    private const float FLEE_DISTANCE = 10f;
    private const float RUN_SPEED = 3.5f;

    public float TimeStuck;
    private Vector3 lastPosition = Vector3.zero;

    public Flee(StateAiPlayerController player, NavMeshAgent navMeshAgent, StateAiEnemyDetection enemyDetector)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.enemyDetector = enemyDetector;
    }

    public void OnEnter()
    {
        TimeStuck = 0f;
        this.navMeshAgent.enabled = true;
        this.navMeshAgent.speed = RUN_SPEED;

        this.player.decision = "continue";
        this.player.attackDecision_flee = false;

        var away = getRandomPoint();
        this.navMeshAgent.SetDestination(away);
    }

    public void Tick()
    {
        if (Vector3.Distance(this.player.transform.position, this.lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        this.lastPosition = this.player.transform.position;
    }

    private Vector3 getRandomPoint()
    {
        Vector3 direction = new Vector3();

        if (this.enemyDetector.getVisibleEnemyTargets().Count > 0){
            direction = this.player.transform.position - this.enemyDetector.getFirstSeenEnemy();    
        }
        else {
            direction = this.player.transform.forward;    
        }
       
        direction.Normalize();

        var endPoint = this.player.transform.position + (direction * FLEE_DISTANCE);

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
    }
}