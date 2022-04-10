using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateAiPlayerController : MonoBehaviour
{

// FIELDS -------------------------------------------------------------------------------------------------------------------------------
    private StateMachine stateMachine;
    
    // Player Scripts
    public PlayerStatus ps;
    private StateAiEnemyDetection enemyDetector;
    
    // Prefabs
    public GameObject bullet;
    public GameObject gun;

    // Bullets
    public GameObject bulletSpawnPoint;
    private float bulletSpeed = 50f;
    
    // Targets
    public GameObject Target { get; set; } //aka Hardpoint
    public Vector3 fleeTarget { get; set; }
    public Vector3 patrolTarget { get; set; }

    // Hardpoint State monitors
        //When the target is set, make a copy of it capture/defend state
        //So that in future, we can see if it has changed
    public int previousRunTimeDefends = 0;
    public int previousRunTimeCaptures = 0;

    
    //Attack options
    public bool attackDecision_flee;
    public bool attackDecision_attack;
    public bool attackDecision_searchForEnemy;
    public string decision;

    //Timer
    private float counter = 2f;


// LIFECYCLE METHODS -------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        
        // Variables
        var navMeshAgent    = this.GetComponent<NavMeshAgent>();
        var animator        = this.GetComponent<Animator>();
        var trailRenderer   = this.GetComponent<TrailRenderer>();
        
        this.enemyDetector  = this.GetComponent<StateAiEnemyDetection>();
        this.ps             = this.GetComponent<PlayerStatus>();
        
        this.stateMachine   = new StateMachine();

        // States
        var search          = new SearchForHardpoint(this, navMeshAgent, trailRenderer);
        var moveToSelected  = new MoveToHardpoint(this, navMeshAgent, animator, trailRenderer);
        var patrol          = new Patrol(this, navMeshAgent, animator);
        var attack          = new Attack(this, navMeshAgent, enemyDetector, animator);
        var flee            = new Flee(this, navMeshAgent, enemyDetector);
        var respawn         = new Respawn(this, navMeshAgent, trailRenderer);
        var searchForEnemy  = new SearchForEnemy(this, navMeshAgent, enemyDetector);
        
        // Transitions
        this.stateMachine.AddAnyTransition(respawn, () => ps.inPlay == false);
        this.stateMachine.AddAnyTransition(flee, shouldFlee());

        At(search, moveToSelected, HasTarget());
        At(search, attack, shouldAttack());
        At(search, searchForEnemy, shouldSearchForEnemy());
        At(moveToSelected, patrol, ReachedHardpoint());
        At(moveToSelected, attack, shouldAttack());
        At(moveToSelected, searchForEnemy, shouldSearchForEnemy());
        At(moveToSelected, search, StuckForOverASecondWhenMovingToHardpoint());
        At(patrol, search, hardpointStateHasUpdated());
        At(patrol, attack, shouldAttack());
        At(patrol, searchForEnemy, shouldSearchForEnemy());
        At(patrol, moveToSelected, StuckForOverASecondWhenPatrolling());
        At(searchForEnemy, attack, sawEnemy());
        At(searchForEnemy, moveToSelected, StuckForOverASecondWhenSearchingForEnemy());
        At(attack, moveToSelected, noEnemyInSight());
        At(flee, search, reachedFleeTarget());
        At(flee, search, StuckForOverASecondWhenFleeing());
        At(respawn, search, () => ps.inPlay == true);
        
        //Starting state
        this.stateMachine.SetState(search);
        void At(IState to, IState from, Func<bool> condition) => this.stateMachine.AddTransition(to, from, condition);
        
        // Condition functions for the transitions
        Func<bool> HasTarget() => () => Target != null;
        Func<bool> ReachedHardpoint() => () => Target != null && Target.transform.parent.gameObject.tag == "Hardpoint" && Vector3.Distance(transform.position, Target.transform.position) < 1.5f;
        Func<bool> reachedFleeTarget() => () => fleeTarget != null && Vector3.Distance(transform.position, fleeTarget) < 1f;
        
        Func<bool> StuckForOverASecondWhenMovingToHardpoint() => () => moveToSelected.TimeStuck > 1f; 
        Func<bool> StuckForOverASecondWhenPatrolling() => () => patrol.TimeStuck > 1f; 
        Func<bool> StuckForOverASecondWhenFleeing() => () => flee.TimeStuck > 1f; 
        Func<bool> StuckForOverASecondWhenSearchingForEnemy() => () => searchForEnemy.TimeStuck > 3f; 
        
        Func<bool> hardpointStateHasUpdated() => () => previousRunTimeCaptures < Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeCaptures() || previousRunTimeDefends < Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeDefends();
        
        Func<bool> sawEnemy() => () => enemyDetector.getVisibleEnemyTargets().Count > 0;
        Func<bool> noEnemyInSight() => () => enemyDetector.getVisibleEnemyTargets().Count == 0;
        
        Func<bool> shouldAttack() => () => attackDecision_attack;
        Func<bool> shouldFlee() => () => attackDecision_flee;
        Func<bool> shouldSearchForEnemy() => () => attackDecision_searchForEnemy;
        
    }

    private void Update() {
        this.stateMachine.Tick();

        //Counter needed to make sure it runs, but only every second
        this.counter += Time.deltaTime;
        if (this.counter >= 2){
            attackWhenEnemySighted();
            fleeOrAttackWhenHit();
            this.counter = 0;
        }
    }

// METHODS -------------------------------------------------------------------------------------------------------------------------------
    public void shoot(){
        GameObject b = Instantiate(this.bullet, bulletSpawnPoint.transform.position, Quaternion.Euler(new Vector3(90, 0 ,0)));
        b.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.transform.forward * this.bulletSpeed;
    }

    // Weighted decision maker
    private void attackWhenEnemySighted(){
        var visibleList = enemyDetector.getAttackableEnemyTargets();
        var attackableList = enemyDetector.getVisibleEnemyTargets();

        if (attackableList.Count > 0){
            if (attackableList.Count == 1){
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Attack = 99%
                    //Flee = 1%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.01f),
                        new KeyValuePair<string, float>("attack", 0.99f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Attack = 95%
                    //Flee = 5%

                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.05f),
                        new KeyValuePair<string, float>("attack", 0.95f),
                    };
                    this.decision = makeDecision(options);
                }
            }
            else if (attackableList.Count == 2){
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Attack = 98%
                    //Flee = 2%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.02f),
                        new KeyValuePair<string, float>("attack", 0.98f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Attack = 95%
                    //Flee = 5%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.05f),
                        new KeyValuePair<string, float>("attack", 0.95f),
                    };
                    this.decision = makeDecision(options);
                }
            }
            else if (attackableList.Count == 3){
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Attack = 95%
                    //Flee = 5%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.05f),
                        new KeyValuePair<string, float>("attack", 0.95f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Attack = 80%
                    //Flee = 20%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.2f),
                        new KeyValuePair<string, float>("attack", 0.8f),
                    };
                    this.decision = makeDecision(options);
                }
            }
            else {
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Attack = 90%
                    //Flee = 10%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.1f),
                        new KeyValuePair<string, float>("attack", 0.9f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Attack = 50%
                    //Flee = 50%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("attack", 0.5f),
                        new KeyValuePair<string, float>("flee", 0.5f),
                    };
                    this.decision = makeDecision(options);
                }
            }
        }
        else if (visibleList.Count > 0){
            if (visibleList.Count == 1){
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Continue = 32%
                    //Attack = 67%
                    //Flee = 1%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.01f),
                        new KeyValuePair<string, float>("continue", 0.32f),
                        new KeyValuePair<string, float>("attack", 0.67f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Continue = 30%
                    //Attack = 65%
                    //Flee = 5%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.05f),
                        new KeyValuePair<string, float>("continue", 0.3f),
                        new KeyValuePair<string, float>("attack", 0.65f),
                    };
                    this.decision = makeDecision(options);
                }
            }
            else if (visibleList.Count == 2){
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Continue = 46%
                    //Attack = 52%
                    //Flee = 2%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.02f),
                        new KeyValuePair<string, float>("continue", 0.46f),
                        new KeyValuePair<string, float>("attack", 0.52f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Continue = 45%
                    //Attack = 50%
                    //Flee = 5%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.1f),
                        new KeyValuePair<string, float>("continue", 0.45f),
                        new KeyValuePair<string, float>("attack", 0.45f),
                    };
                    this.decision = makeDecision(options);
                }
            }
            else if (visibleList.Count == 3){
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Continue = 47%
                    //Attack = 50%
                    //Flee = 3%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.03f),
                        new KeyValuePair<string, float>("continue", 0.47f),
                        new KeyValuePair<string, float>("attack", 0.50f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Continue = 40%
                    //Attack = 50%
                    //Flee = 10%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.1f),
                        new KeyValuePair<string, float>("continue", 0.4f),
                        new KeyValuePair<string, float>("attack", 0.5f),
                        
                    };
                    this.decision = makeDecision(options);
                }
            }
            else {
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //Continue = 66%
                    //Attack = 30%
                    //Flee = 4%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.04f),
                        new KeyValuePair<string, float>("attack", 0.3f),
                        new KeyValuePair<string, float>("continue", 0.66f),
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //Continue = 50%
                    //Attack = 30%
                    //Flee = 20%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.2f),
                        new KeyValuePair<string, float>("attack", 0.3f), 
                        new KeyValuePair<string, float>("continue", 0.5f),
                    };
                    this.decision = makeDecision(options);
                }
            }
        }
        else {
            this.decision = "continue";
        }

        setAttackDecisionBools();
    }

    // Weighted decision maker
    private void fleeOrAttackWhenHit(){
        var visibleList = enemyDetector.getAttackableEnemyTargets();

        if (this.ps.beingAttacked == true){
            if (visibleList.Count > 0){
                this.decision = "attack";
            }
            else {
                if (Vector3.Distance(transform.position, Target.transform.position) < 5f){
                    //PROBABILITIES
                    //SearchForEnemy = 90%
                    //Flee = 10%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.1f),
                        new KeyValuePair<string, float>("seasearchForEnemyrch", 0.9f), 
                    };
                    this.decision = makeDecision(options);
                }
                else {
                    //PROBABILITIES
                    //SearchForEnemy = 70%
                    //Flee = 30%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0.3f),
                        new KeyValuePair<string, float>("searchForEnemy", 0.7f), 
                    };
                    this.decision = makeDecision(options);
                }
            }
        }

        setAttackDecisionBools();
    }

    public void setAttackDecisionBools(){
        switch (this.decision){
            case "attack":
                this.attackDecision_attack = true;
                this.attackDecision_flee = false;
                this.attackDecision_searchForEnemy = false;
                break;
            case "flee":
                this.attackDecision_attack = false;
                this.attackDecision_flee = true;
                this.attackDecision_searchForEnemy = false;
                break;
            case "searchForEnemy":
                this.attackDecision_attack = false;
                this.attackDecision_flee = false;
                this.attackDecision_searchForEnemy = true;
                break;
            default:
                this.attackDecision_attack = false;
                this.attackDecision_flee = false;
                this.attackDecision_searchForEnemy = false;
                break;
        }
    }

    private float generateRandom(){
        float rndNum = UnityEngine.Random.Range(0f, 1f);
        return rndNum;
    }

    private string makeDecision(List<KeyValuePair<string, float>> options){
        float rndNum = generateRandom();
            
        float cumulative = 0f;
        for (int i = 0; i < options.Count; i++)
        {
            cumulative += options[i].Value;
            if (rndNum < cumulative)
            {
                return options[i].Key;
            }
        }

        return null;
    }
}