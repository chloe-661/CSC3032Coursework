using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateAiPlayerController : MonoBehaviour
{
    public PlayerStatus ps;
    private StateAiEnemyDetection enemyDetector;
    public GameObject bullet; //Prefabs
    public GameObject gun;

    public GameObject bulletSpawnPoint;

    private float bulletSpeed = 50f;
    
    private StateMachine _stateMachine;
    
    public GameObject Target { get; set; } //aka Hardpoint
    public Vector3 fleeTarget { get; set; }
    public Vector3 patrolTarget { get; set; }

    //When the target is set, make a copy of it capture/defend state
    //So that in future, we can see if it has changed...?
    public int previousRunTimeDefends = 0;
    public int previousRunTimeCaptures = 0;

    
    //Attack options
    public bool attackDecision_flee;
    public bool attackDecision_attack;
    public bool attackDecision_stayAttack;
    public bool attackDecision_chaseAttack;
    public bool attackDecision_delayDone;

    public string decision;

    private float attackCounter;
    private float fleeCounter;


    private void Awake()
    {
        ps = this.GetComponent<PlayerStatus>();
        var navMeshAgent = this.GetComponent<NavMeshAgent>();
        var animator = this.GetComponent<Animator>();
        enemyDetector = this.GetComponent<StateAiEnemyDetection>();
        var trailRenderer = this.GetComponent<TrailRenderer>();
        attackCounter = 2;
        fleeCounter = 2;
        
        _stateMachine = new StateMachine();

        
        //----States-----
        //Search for hardpoint
        //Move to hardpoint
        //Capture - Stood Still
        //Capture - Patrol
        //Defend - Stood Still
        //Defend - patrol
        //Chase
        //Attack
        //Flee

        var search = new SearchForHardpoint(this, navMeshAgent, trailRenderer);
        var moveToSelected = new MoveToHardpoint(this, navMeshAgent, animator, trailRenderer);
        var patrol = new Patrol(this, navMeshAgent, animator);
        var attackDecision = new AttackDecision(this, enemyDetector);
        var stayAttack = new StayAttack(this, navMeshAgent, enemyDetector);
        var attack = new Attack(this, navMeshAgent, enemyDetector, animator);
        var chaseAttack = new ChaseAttack(this, navMeshAgent, enemyDetector);
        var flee = new Flee(this, navMeshAgent, enemyDetector);
        // var attack = new Attack(this, navMeshAgent, enemyDetector);
        // var returnToStockpile = new ReturnToStockpile(this, navMeshAgent, animator);
        // var placeResourcesInStockpile = new PlaceResourcesInStockpile(this);
        // var flee = new Flee(this, navMeshAgent, enemyDetector, animator, fleeParticleSystem);
        
        var respawn = new Respawn(this, navMeshAgent, trailRenderer);
        
        At(search, moveToSelected, HasTarget());
        // At(search, attackDecision, sawEnemy());
        // At(search, attack, () => sawEnemy());
        At(search, attack, shouldAttack());

        
        At(moveToSelected, search, StuckForOverASecond());
        At(moveToSelected, patrol, ReachedHardpoint());
        // At(moveToSelected, attack, sawEnemy());
        At(moveToSelected, attack, shouldAttack());
        // At(moveToSelected, attack, () => this.decision == "attack");
        // At(moveToSelected, attackDecision, sawEnemy());
        At(patrol, search, hardpointStateHasUpdated());
        // At(patrol, attack, () => this.decision == "attack");
        //  At(patrol, attack, sawEnemy());
         At(patrol, attack, shouldAttack());

        At(attack, moveToSelected, noEnemyInSight());
        //At(patrol, attackDecision, sawEnemy());
        //At(attackDecision, flee, shouldFlee());
        //At(attackDecision, stayAttack, shouldStayAttack());
        //At(attackDecision, chaseAttack, shouldChaseAttack());
        //At(attackDecision, moveToSelected, shouldIgnore());
        //At(flee, search, StuckForOverASecondWhilstFleeing());
        //At(flee, search, reachedFleeTarget());
        

        _stateMachine.AddAnyTransition(respawn, () => ps.inPlay == false);
        At(respawn, search, () => ps.inPlay == true);

        _stateMachine.AddAnyTransition(flee, shouldFlee());
        At(flee, search, reachedFleeTarget());

        

        // At(flee, search, () => true);
        // At(stayAttack, moveToSelected, () => enemyDetector.getVisibleEnemyTargets().Count > 0);
        // At(chaseAttack, search, () => true);
        
        //Starting state
        _stateMachine.SetState(search);

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
        Func<bool> HasTarget() => () => Target != null;
        Func<bool> StuckForOverASecond() => () => moveToSelected.TimeStuck > 1f; 
        Func<bool> StuckForOverASecondWhilstFleeing() => () => flee.TimeStuck > 1f; 
        Func<bool> ReachedHardpoint() => () => Target != null && Target.transform.parent.gameObject.tag == "Hardpoint" && Vector3.Distance(transform.position, Target.transform.position) < 1.5f;
        Func<bool> hardpointStateHasUpdated() => () => previousRunTimeCaptures < Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeCaptures() || previousRunTimeDefends < Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeDefends();
        //Func<bool> sawEnemy() => () => enemyDetector.getVisibleEnemyTargets().Count > 0 && attackDecision_delayDone == true;
        Func<bool> sawEnemy() => () => enemyDetector.getVisibleEnemyTargets().Count > 0;
        Func<bool> noEnemyInSight() => () => enemyDetector.getVisibleEnemyTargets().Count == 0;
        Func<bool> shouldAttack() => () => {
            //Counter needed to make sure it runs, but only every second
            // attackCounter += Time.deltaTime;
            // if (attackCounter >= 1){
            //     attackWhenEnemySighted();
            //     attackCounter = 0;
            // }

            return attackDecision_attack;
        };
        
        Func<bool> shouldFlee() => () => {
            //Counter needed to make sure it runs, but only every second
            // fleeCounter += Time.deltaTime;
            // if (fleeCounter >= 1){
            //     fleeOrAttackWhenHit();
            //     fleeCounter = 0;
            // }

            return attackDecision_flee;
        };
        // Func<bool> shouldStayAttack() => () =>  attackDecision_stayAttack == true;
        // Func<bool> shouldChaseAttack() => () =>  attackDecision_chaseAttack == true;
        // Func<bool> shouldIgnore() => () =>  attackDecision_ignore == true;
        Func<bool> reachedFleeTarget() => () => fleeTarget != null && Vector3.Distance(transform.position, fleeTarget) < 1f;        // Func<bool> TargetIsDepletedAndICanCarryMore() => () => (Target == null || Target.IsDepleted) && !InventoryFull().Invoke();
        // Func<bool> InventoryFull() => () => _gathered >= _maxCarried;
        // Func<bool> ReachedStockpile() => () => StockPile != null && 
                                            //    Vector3.Distance(transform.position, StockPile.transform.position) < 1f;
    }


    private void Update() {
        _stateMachine.Tick();

        //Counter needed to make sure it runs, but only every second
        attackCounter += Time.deltaTime;
        if (attackCounter >= 1){
            attackWhenEnemySighted();
            fleeOrAttackWhenHit();
            attackCounter = 0;
        }
    }

    public void shoot(){
        GameObject b = Instantiate(this.bullet, bulletSpawnPoint.transform.position, Quaternion.Euler(new Vector3(90, 0 ,0)));
        // b.GetComponent<BulletController>().move(bulletSpawnPoint.transform.forward);
        b.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.transform.forward * this.bulletSpeed;
    }

    private void attackWhenEnemySighted(){
        Debug.Log("Entered attackWhenEnemySighted");
        var visibleList = enemyDetector.getAttackableEnemyTargets();
        var attackableList = enemyDetector.getVisibleEnemyTargets();

        if (attackableList.Count > 0){
            if (attackableList.Count == 1){
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
            else if (attackableList.Count == 2){
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
            else if (attackableList.Count == 3){
                //PROBABILITIES
                //Attack = 60%
                //Flee = 40%
                var options = new List<KeyValuePair<string, float>>() 
                { 
                    new KeyValuePair<string, float>("flee", 0.4f),
                    new KeyValuePair<string, float>("attack", 0.6f),
                };
                this.decision = makeDecision(options);
            }
            else {
                //PROBABILITIES
                //Attack = 40%
                //Flee = 60%
                var options = new List<KeyValuePair<string, float>>() 
                { 
                    new KeyValuePair<string, float>("attack", 0.4f),
                    new KeyValuePair<string, float>("flee", 0.6f),
                };
                this.decision = makeDecision(options);
            }
        }
        else if (visibleList.Count > 0){
            if (visibleList.Count == 1){
                //PROBABILITIES
                //Continue = 45%
                //Attack = 50%
                //Flee = 5%
                var options = new List<KeyValuePair<string, float>>() 
                { 
                    new KeyValuePair<string, float>("flee", 0.05f),
                    new KeyValuePair<string, float>("continue", 0.45f),
                    new KeyValuePair<string, float>("attack", 0.5f),
                };
                this.decision = makeDecision(options);
            }
            else if (visibleList.Count == 2){
                //PROBABILITIES
                //Continue = 45%
                //Attack = 45%
                //Flee = 10%
                var options = new List<KeyValuePair<string, float>>() 
                { 
                    new KeyValuePair<string, float>("flee", 0.1f),
                    new KeyValuePair<string, float>("continue", 0.45f),
                    new KeyValuePair<string, float>("attack", 0.45f),
                };
                this.decision = makeDecision(options);
            }
            else if (visibleList.Count == 3){
                //PROBABILITIES
                //Continue = 45%
                //Attack = 30%
                //Flee = 25%
                var options = new List<KeyValuePair<string, float>>() 
                { 
                    new KeyValuePair<string, float>("flee", 0.25f),
                    new KeyValuePair<string, float>("attack", 0.3f),
                    new KeyValuePair<string, float>("continue", 0.45f),
                };
                this.decision = makeDecision(options);
            }
            else {
                //PROBABILITIES
                //Continue = 40%
                //Attack = 20%
                //Flee = 40%
                var options = new List<KeyValuePair<string, float>>() 
                { 
                    new KeyValuePair<string, float>("attack", 0.2f), 
                    new KeyValuePair<string, float>("continue", 0.4f),
                    new KeyValuePair<string, float>("flee", 0.4f),
                };
                this.decision = makeDecision(options);
            }
        }
        else {
            this.decision = "continue";
        }

        setAttackDecisionBools();
    }

    public void setAttackDecisionBools(){
        switch (this.decision){
            case "attack":
                this.attackDecision_attack = true;
                this.attackDecision_flee = false;
                break;
            case "flee":
                this.attackDecision_attack = false;
                this.attackDecision_flee = true;
                break;
            default:
                this.attackDecision_attack = false;
                this.attackDecision_flee = false;
                break;
        }
    }

    private void fleeOrAttackWhenHit(){
        var visibleList = enemyDetector.getAttackableEnemyTargets();
        var attackableList = enemyDetector.getVisibleEnemyTargets();

        if (this.ps.beingAttacked == true){
            if (visibleList.Count > 0){
                this.decision = "attack";
            }
            else {
                this.decision = "flee";
            }
        }

        setAttackDecisionBools();
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

    // public void TakeFromTarget()
    // {
    //     if (Target.Take())
    //     {
    //         _gathered++;
    //         OnGatheredChanged?.Invoke(_gathered);
    //     }
    // }

    // public bool Take()
    // {
    //     if (_gathered <= 0)
    //         return false;
        
    //     _gathered--;
    //     OnGatheredChanged?.Invoke(_gathered);
    //     return true;
    // }

    // public void DropAllResources()
    // {
    //     if (_gathered > 0)
    //     {
    //         FindObjectOfType<WoodDropper>().Drop(_gathered, transform.position);
    //         _gathered = 0;
    //         OnGatheredChanged?.Invoke(_gathered);
    //     }
    // }
}