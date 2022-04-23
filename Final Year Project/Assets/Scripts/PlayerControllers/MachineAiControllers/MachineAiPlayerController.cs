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
    public GameStatusT gs;
    public TrainingGameController tc;
    public BufferSensorComponent otherPlayersBuffer;
    public BufferSensorComponent hardpointsBuffer;
    private const float LOCATION_NORMALIZATION_FACTOR = 80.0f;
    
    //INDIVIDUAL REWARDS        
    public const float STAYED_IN_HARDPOINT_BONUS = 0.1f;
    public const float KILLED_ENEMY_BONUS = 1f;
    public const float BEEN_KILLED_BONUS = -1f;
    public const float HIT_ENEMY_BONUS = 0.25f;
    public const float BEEN_HIT_BONUS = -0.25f;
    public const float STEP_BONUS = -0.001f;  

    //Player
    public PlayerStatusT ps;
    public Rigidbody rb;
    public Animator animator;
    public BehaviorParameters behaviorParameters;
    public bool isDecisionStep;
    public int stepCount;
        
        //Movement
        public float xInput;
        public float zInput;
        public float speed = 10;
        public float maxVelocity = 20;
        
        //Shooting
        public float shootInput;
        
        //Rotation
        public float rotateInput;
        public float rotationSensitivity = 1;
        public float rotationSmoothTime = 0.05f;
        public float rotationYaw;
        public float rotationSmoothYaw;
        public float rotationSmoothYawX;
        public Quaternion originalRotation;
    
    //Shooting
    public GameObject bulletSpawnPoint;
    public float bulletSpeed = 50f;
    public bool canShoot;
    public float shootTimer;

//Methods -----------------------------------------------------------------------------------------------------

    public override void Initialize(){
        
        this.ps = this.gameObject.GetComponent<PlayerStatusT>();
        this.rb = this.gameObject.GetComponent<Rigidbody>();
        this.animator = this.gameObject.GetComponent<Animator>();
        this.behaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        
        var bufferSensors = GetComponentsInChildren<BufferSensorComponent>();
        this.otherPlayersBuffer = bufferSensors[0];
        this.hardpointsBuffer = bufferSensors[1];
    }

    public void OnEpisodeBegin() {
        Debug.Log("OnEpisodeBegin has run...");
        reset();
    }

    public void reset(){
        this.rb.velocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
        this.canShoot = true;
        this.shootTimer = 0;
        this.rb.drag = 4;
        this.rb.angularDrag = 1;
        this.isDecisionStep = true;
        this.stepCount = 0;
        this.ps.reset();
        this.gs.reset();
    }

    public override void OnActionReceived(ActionBuffers actions){
        move(actions);
    }

    public override void CollectObservations(VectorSensor sensor){
        foreach (var h in this.tc.hardpointsList){
            this.hardpointsBuffer.AppendObservation(getHardpointData(h));
        }
        
        List<PlayerStatusT> teamList;
        List<PlayerStatusT> opponentsList;
        if (this.behaviorParameters.TeamId == 0)
        {
            teamList = this.tc.redTeamPlayers;
            opponentsList = this.tc.blueTeamPlayers;
        }
        else
        {
            teamList = this.tc.blueTeamPlayers;
            opponentsList = this.tc.redTeamPlayers;
        }

        foreach (var player in teamList)
        {
            MachineAiPlayerController p = player.gameObject.GetComponent<MachineAiPlayerController>();
            if (p != this && p.gameObject.activeInHierarchy)
            {
                otherPlayersBuffer.AppendObservation(getOtherPlayerData(player));
            }
        }
    }
    void FixedUpdate()
    {
        this.tc = this.transform.root.GetComponent<TrainingGameController>();
        checkIfDead();
        // checkIfGameOver();

        if (this.ps.inHardpoint == true){
            Debug.Log("Hit Hardpoint");
            this.tc.reachedTarget(this.ps.team);
        }

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
    private float[] getHardpointData(HardpointControllerT hardpoint)
    {
        var hardpointData = new float[4];

        //Location     
        var relativePosition = transform.InverseTransformPoint(hardpoint.transform.GetChild(0).position); //Gets child "centerTarget", sets it position as local position
        hardpointData[0] = relativePosition.x / LOCATION_NORMALIZATION_FACTOR;
        hardpointData[1] = relativePosition.z / LOCATION_NORMALIZATION_FACTOR;

        //Owner
        if (hardpoint.getOwner() == "none" || hardpoint.getOwner() != this.ps.team){
            hardpointData[2] = 1.0f;
        }
        else {
            hardpointData[2] = 0.0f;
        }

        //State
        hardpointData[3] = hardpoint.getState() == "congested" ? 1.0f : 0.0f; 
        
        return hardpointData;
    }
    private float[] getOtherPlayerData(PlayerStatusT player)
    {
        var otherPlayerdata = new float[9];
        var p = player.gameObject.GetComponent<MachineAiPlayerController>();

        otherPlayerdata[0] = player.getHealth();
        
        var relativePosition = transform.InverseTransformPoint(p.transform.localPosition);
        otherPlayerdata[1] = relativePosition.x / LOCATION_NORMALIZATION_FACTOR;
        otherPlayerdata[2] = relativePosition.z / LOCATION_NORMALIZATION_FACTOR;

        otherPlayerdata[3] = player.dead ? 1.0f : 0.0f;             //1 = Dead              0 = Alive
        otherPlayerdata[4] = player.inPlay ? 1.0f : 0.0f;           //1 = InPlay            0 = Respawning
        otherPlayerdata[5] = player.inHardpoint ? 1.0f : 0.0f;      //1 = InHardpoint       0 = Elsewhere
        otherPlayerdata[6] = player.beingAttacked ? 1.0f : 0.0f;    //1 = BeingAttacked     0 = Havent been hit for a while
        
        var relativeVelocity = transform.InverseTransformDirection(p.rb.velocity);
        otherPlayerdata[7] = relativeVelocity.x / 30.0f;
        otherPlayerdata[8] = relativeVelocity.z / 30.0f;
        
        return otherPlayerdata;
    }
    
    public void move(ActionBuffers actions){
        Debug.Log("2 before");
        giveReward("stepBonus");
        Debug.Log("2 after");
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
                // if (this.shootInput > 0)
                // {
                //     shoot();
                // }
            }
        }
    }

    // public void shoot (){
    //     this.animator.SetBool("isRunning", false);
    //     this.animator.SetBool("isPatrolling", false);
    //     this.animator.SetBool("isAttacking", true);

    //     if (canShoot){
    //         GameObject b = Instantiate(this.BULLET, bulletSpawnPoint.transform.position, Quaternion.Euler(new Vector3(90, 0 ,0)));
    //         b.transform.parent = this.transform.root;
    //         b.GetComponent<BulletController>().shotBy = this.gameObject;
    //         b.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.transform.forward * this.bulletSpeed;
    //         this.shootTimer = 0;
    //     }
    // }

    public void checkIfDead(){
        if (this.ps.dead == true){
            // giveReward("beenKilledBonus");
            respawn();
        }
    }



    public void respawn(){
        int num = Random.Range(0, this.ps.getRespawnPoints().Count);
        float x = this.ps.getRespawnPoints()[num].transform.localPosition .x;
        float y = this.ps.getRespawnPoints()[num].transform.localPosition .y;
        float z = this.ps.getRespawnPoints()[num].transform.localPosition .z;

        this.transform.localPosition = new Vector3(x,y,z);
        foreach (Transform child in this.tc.transform){
            if (child.tag == "BluePlayer"){
                this.transform.LookAt(child);
            }
        }
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
            case "stayedInHardpointBonus":
                AddReward(STAYED_IN_HARDPOINT_BONUS);
                break;
            case "killedEnemyBonus":
                AddReward(KILLED_ENEMY_BONUS);
                break;
            case "beenKilledBonus":
                AddReward(BEEN_KILLED_BONUS);
                break;
            default:
                break;
        }
    }

    
//FOR TRAINING ONLY --------------------------------------------------------------------------------
    // public void checkIfGameOver(){
    //     if (this.gs.getGameOver()){
    //         if (this.gs.getWinner() == this.ps.team){
    //             giveReward("wonBonus");
    //         }
    //         else {
    //             giveReward("lostBonus");
    //         }
    //     }
    // }

    // public override float[] Heuristic()
    // {
    //     var action = new float[2];
    //     action[0] = Input.GetAxis("Horizontal");
    //     action[1] = Input.GetAxis("Vertical");
    //     return action;
    // }
}
