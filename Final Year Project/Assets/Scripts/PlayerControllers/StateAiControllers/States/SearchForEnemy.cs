using UnityEngine;
using UnityEngine.AI;

public class SearchForEnemy : IState
{
    private StateAiPlayerController player;
    private NavMeshAgent navMeshAgent;
    private StateAiEnemyDetection enemyDetector;

    public float TimeStuck;
    private Vector3 lastPosition = Vector3.zero;

    public SearchForEnemy(StateAiPlayerController player, NavMeshAgent navMeshAgent, StateAiEnemyDetection enemyDetector)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.enemyDetector = enemyDetector;
    }

    public void OnEnter()
    {
        Debug.Log("ENTERED SEARCH ENEMY STATE");
        TimeStuck = 0f;
        this.navMeshAgent.enabled = true;

        this.player.attackDecision_flee = false;
        this.player.attackDecision_attack = false;
    }

    public void Tick()
    {
        rotatePlayer();
        TimeStuck += Time.deltaTime;
    }

    private void rotatePlayer(){
        this.player.transform.Rotate((Vector3.up * 5));
    }

    public void OnExit()
    {
        Debug.Log("EXITTED SEARCH ENEMY STATE");
        this.player.attackDecision_searchForEnemy = false;
    }
}