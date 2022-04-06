using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Respawn : IState
{
    private StateAiPlayerController player;
    private NavMeshAgent navMeshAgent;
    private TrailRenderer trailRenderer;
    private List<GameObject> respawnPoints;
    private float counter;

    public Respawn(StateAiPlayerController player, NavMeshAgent navMeshAgent, TrailRenderer trailRenderer)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.respawnPoints = this.player.ps.getRespawnPoints();
        this.trailRenderer = trailRenderer;
        this.counter = 5f;
    }

    public void OnEnter()
    {
        this.navMeshAgent.enabled = false;
        this.counter = 5f;
        this.trailRenderer.enabled = false;

        respawnCharacter();
        this.player.ps.setHealth(100f);
        this.player.ps.dead = false;
    }

    private Vector3 chooseRespawnPoint(){
        int num = Random.Range(0, this.respawnPoints.Count);
        float x = this.respawnPoints[num].transform.position.x;
        float y = this.respawnPoints[num].transform.position.y;
        float z = this.respawnPoints[num].transform.position.z;

        return new Vector3(x,y,z);
    }

    private void respawnCharacter(){
        this.player.transform.position = chooseRespawnPoint();
        this.player.transform.LookAt(GameObject.FindWithTag("CenterTarget").transform);
    }

    public void Tick()
    {
        this.counter -= Time.deltaTime;
        if (this.counter <= 0f){
            this.player.ps.setHealth(100f);
            this.player.ps.inPlay = true;
            this.counter = 1000f;
        }
    }

    public void OnExit()
    {
        this.trailRenderer.enabled = true;
        this.navMeshAgent.enabled = true;
        this.player.ps.inPlay = true;
        this.player.ps.setHealth(100f);

        this.player.decision = "continue";
        this.player.setAttackDecisionBools();

        string logMessage = (this.player.ps.team + ":" + this.player.ps.name + " respawned");
        GameStatus gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
        gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {logMessage});
        gs.addToKillScores(this.player.ps.team);
    }
}