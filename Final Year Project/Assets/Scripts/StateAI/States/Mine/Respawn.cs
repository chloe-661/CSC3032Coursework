using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Respawn : IState
{
    private StateAiPlayerController player;
    private TrailRenderer trail;
    private List<GameObject> respawnPoints;


    private NavMeshAgent _navMeshAgent;
    private readonly EnemyDetector _enemyDetector;
    private Animator _animator;
    private readonly ParticleSystem _particleSystem;
    private static readonly int FleeHash = Animator.StringToHash("Flee");

    private float _initialSpeed;
    private const float FLEE_SPEED = 6F;
    private const float FLEE_DISTANCE = 5F;

    private float counter;

    public Respawn(StateAiPlayerController player)
    {
        this.player = player;
        this.respawnPoints = this.player.ps.getRespawnPoints();
        this.trail = this.player.GetComponent<TrailRenderer>();
        this.counter = 3f;
    }

    public void OnEnter()
    {
        Debug.Log("RESPAWN ENTERED");
        this.counter = 5f;
        this.trail.enabled = false;

        int num = Random.Range(0, this.respawnPoints.Count);
        float x = this.respawnPoints[num].transform.position.x;
        float y = this.respawnPoints[num].transform.position.y;
        float z = this.respawnPoints[num].transform.position.z;

        this.player.transform.position = new Vector3(x, y, z);
        this.player.transform.LookAt(GameObject.FindWithTag("CenterTarget").transform);
        this.player.ps.setHealth(100f);
        this.player.ps.dead = false;
        Debug.Log("DEAD RESET");
    }

    public void Tick()
    {
        this.counter -= Time.deltaTime;
        if (this.counter <= 0f){
            this.player.ps.setHealth(100f);
            this.player.ps.inPlay = true;
            this.counter = 1000f;
            Debug.Log("RESPAWN TICK RESET");
        }
    }

    public void OnExit()
    {
        this.trail.enabled = true;
        this.player.ps.inPlay = true;
        this.player.ps.setHealth(100f);
        Debug.Log("RESPAWN EXITED");
    }
}