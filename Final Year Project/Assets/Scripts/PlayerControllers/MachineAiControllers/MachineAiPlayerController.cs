using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Random = UnityEngine.Random;
public class MachineAiPlayerController : Agent
{
    //Prefabs
    public GameObject GAME_STATUS;
    public GameObject RED_TEAM;
    public GameObject BLUE_TEAM;
    public GameObject BULLET;
    
    //GAME
    private GameStatus gs;
    
    //REWARDS
    public const float CAPTURED_HARDPOINT_BONUS = 15f;
    public const float DEFENDED_HARDPOINT_BONUS = 5f;
    public const float KILLED_ENEMY_BONUS = 1f;
    public const float BEEN_KILLED_BONUS = -1f;
    public const float HIT_ENEMY_BONUS = 0.25f;
    public const float BEEN_HIT_BONUS = -0.25f;
    public const float WON_BONUS = 100f;
    public const float LOST_BONUS = -50f;
    public const float STEP_BONUS = -0.01f;  

    //Player
    public PlayerStatus ps;
    public Rigidbody rb;
    public Animator animator;
    private bool isDecisionStep;
    private int stepCount;
        
        //Movement
        private float xInput;
        private float zInput;
        public float speed = 10;
        public float maxVelocity = 20;
        
        //Shooting
        public float shootInput;
        
        //Rotation
        private float rotateInput;
        public float rotationSensitivity = 1;
        public float rotationSmoothTime = 0.05f;
        private float rotationYaw;
        private float rotationSmoothYaw;
        private float rotationSmoothYawX;
        private Quaternion originalRotation;
    
    //Shooting
    public GameObject bulletSpawnPoint;
    private float bulletSpeed = 50f;
    private bool canShoot;
    private float shootTimer;

//Methods -----------------------------------------------------------------------------------------------------

    public override void Initialize(){
        
    }

    public virtual void OnEpisodeBegin() {
        this.gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
        this.ps = this.gameObject.GetComponent<PlayerStatus>();
        this.rb = this.gameObject.GetComponent<Rigidbody>();
        this.animator = this.gameObject.GetComponent<Animator>();
    }
    public override void OnActionReceived(ActionBuffers actions){
        move(actions);
    }

    public override void CollectObservations(VectorSensor sensor){
        //Player position (local)
        //All Hardpoint data
        //Player health

        // sensor.AddObservation();
    }
    void FixedUpdate()
    {
        checkIfDead();
        checkIfGameOver();

        if (this.stepCount % 5 == 0)
        {
            this.isDecisionStep = true;
            this.stepCount++;
        }

        if (this.shootTimer > 1){
            this.canShoot = false;
        }
        else {
            this.canShoot = true;
        }
        this.shootTimer += Time.fixedDeltaTime;
    }

//Methods ----------------------------------------------------------------------------------------------------
    public void move(ActionBuffers actions){
        var continuousActions = actions.ContinuousActions;
        var discreteActions = actions.DiscreteActions;
        
        if (this.ps.inPlay){
            this.xInput = continuousActions[0];
            this.zInput = continuousActions[1];
            this.rotateInput = continuousActions[2];
            this.shootInput = (int)discreteActions[0];

            //ROTATE
            this.rotationYaw += this.rotateInput * this.rotationSensitivity;
            float rotationSmoothYawOld = this.rotationSmoothYaw;
            this.rotationSmoothYaw = Mathf.SmoothDampAngle(this.rotationSmoothYaw, this.rotationYaw, ref this.rotationSmoothYawX, this.rotationSmoothTime);
            rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(Mathf.DeltaAngle(rotationSmoothYawOld, this.rotationSmoothYaw), transform.up));

            //ANIMATE
            this.animator.SetBool("isRunning", true);
            this.animator.SetBool("isPatrolling", false);
            this.animator.SetBool("isAttacking", false);
            
            //MOVE
            var moveDirection = transform.TransformDirection(new Vector3(this.zInput, 0, this.xInput));
            var velocity = this.rb.velocity.magnitude;
            float adjustedSpeed = Mathf.Clamp(this.speed - velocity, 0, this.maxVelocity);
            this.rb.AddForce(moveDirection * adjustedSpeed, ForceMode.Impulse);

            //Perform discrete actions only once between decisions
            if (this.isDecisionStep)
            {
                this.isDecisionStep = false;
                //SHOOT
                if (this.shootInput > 0)
                {
                    shoot();
                }
            }
        }
    }

    public void shoot (){
        this.animator.SetBool("isRunning", false);
        this.animator.SetBool("isPatrolling", false);
        this.animator.SetBool("isAttacking", true);

        if (canShoot){
            GameObject b = Instantiate(this.BULLET, bulletSpawnPoint.transform.position, Quaternion.Euler(new Vector3(90, 0 ,0)));
            b.GetComponent<BulletController>().shotBy = this.gameObject;
            b.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.transform.forward * this.bulletSpeed;
            this.shootTimer = 0;
        }
    }

    public void reset(){
        //Destroy the players
        //Destroy the game status
        //Destroy the hardpoints

        //NO JUST DO RESET OF VARIABLES. LOTS OF CODE BUT MUCH SIMPLIER LOGIC WISE
    }

    public void checkIfDead(){
        if (this.ps.dead == true){
            giveReward("beenKilledBonus");
            respawn();
        }
    }

    public void checkIfGameOver(){
        if (this.gs.getGameOver()){
            EndEpisode();
        }
    }

    public void respawn(){
        int num = Random.Range(0, this.ps.getRespawnPoints().Count);
        float x = this.ps.getRespawnPoints()[num].transform.position.x;
        float y = this.ps.getRespawnPoints()[num].transform.position.y;
        float z = this.ps.getRespawnPoints()[num].transform.position.z;

        this.transform.position = new Vector3(x,y,z);
        this.transform.LookAt(GameObject.FindWithTag("CenterTarget").transform);
        this.ps.dead = false;
        this.ps.setHealth(100);
        StartCoroutine(respawnWaiter());
    }

    IEnumerator respawnWaiter(){
        yield return new WaitForSeconds(5);
        this.ps.inPlay = true;
    }

    public void giveReward(string type){
        switch(type){
            case "stepBonus":
                AddReward(STEP_BONUS);
                break;
            case "hitEnemyBonus":
                AddReward(HIT_ENEMY_BONUS);
                break;
            case "beenHitBonus":
                AddReward(BEEN_HIT_BONUS);
                break;
            case "capturedHardpointBonus":
                AddReward(CAPTURED_HARDPOINT_BONUS);
                break;
            case "defendedhardpointBonus":
                AddReward(DEFENDED_HARDPOINT_BONUS);
                break;
            case "killedEnemyBonus":
                AddReward(KILLED_ENEMY_BONUS);
                break;
            case "beenKilledBonus":
                AddReward(BEEN_KILLED_BONUS);
                break;
            case "wonBonus":
                AddReward(WON_BONUS);
                break;
            case "lostBonus":
                AddReward(LOST_BONUS);
                break;
            default:
                break;
        }
    }
}
