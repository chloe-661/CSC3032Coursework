using System;
using UnityEngine;
using UnityEngine.AI;

public class StateAiPlayerController : MonoBehaviour
{
    public PlayerStatus ps;
    public GameObject bullet; //Prefabs
    public GameObject gun;

    public GameObject bulletSpawnPoint;

    private float bulletSpeed = 50f;
    
    private StateMachine _stateMachine;
    
    public GameObject Target { get; set; } //aka Hardpoint
    public Vector3 fleeTarget { get; set; }

    //When the target is set, make a copy of it capture/defend state
    //So that in future, we can see if it has changed...?
    public int previousRunTimeDefends = 0;
    public int previousRunTimeCaptures = 0;

    
    //Attack options
    public bool attackDecision_flee;
    public bool attackDecision_ignore;
    public bool attackDecision_stayAttack;
    public bool attackDecision_chaseAttack;
    public bool attackDecision_delayDone;


    private void Awake()
    {
        ps = this.GetComponent<PlayerStatus>();
        var navMeshAgent = this.GetComponent<NavMeshAgent>();
        var animator = this.GetComponent<Animator>();
        var enemyDetector = this.GetComponent<StateAiEnemyDetection>();
        
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

        var search = new SearchForHardpoint(this);
        var moveToSelected = new MoveToHardpoint(this, navMeshAgent, animator);
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
        
        var respawn = new Respawn(this);
        
        At(search, moveToSelected, HasTarget());
        //At(search, attackDecision, sawEnemy());
        At(search, attack, sawEnemy());
        
        At(moveToSelected, search, StuckForOverASecond());
        At(moveToSelected, patrol, ReachedHardpoint());
        At(moveToSelected, attack, sawEnemy());
        //At(moveToSelected, attackDecision, sawEnemy());
        At(patrol, search, hardpointStateHasUpdated());
        At(patrol, attack, sawEnemy());

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

        _stateMachine.AddAnyTransition(flee, () => ps.beingAttacked == true);
        At(flee, search, () => ps.beingAttacked == false);

        

        // At(flee, search, () => true);
        // At(stayAttack, moveToSelected, () => enemyDetector.getVisibleEnemyTargets().Count > 0);
        // At(chaseAttack, search, () => true);
        
        //Starting state
        _stateMachine.SetState(search);

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
        Func<bool> HasTarget() => () => Target != null;
        Func<bool> StuckForOverASecond() => () => moveToSelected.TimeStuck > 1f; 
        Func<bool> StuckForOverASecondWhilstFleeing() => () => flee.TimeStuck > 1f; 
        Func<bool> ReachedHardpoint() => () => Target != null && Target.tag == "Hardpoint" && Vector3.Distance(transform.position, Target.transform.position) < 1.5f;
        Func<bool> hardpointStateHasUpdated() => () => previousRunTimeCaptures < Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeCaptures() || previousRunTimeDefends < Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeDefends();
        //Func<bool> sawEnemy() => () => enemyDetector.getVisibleEnemyTargets().Count > 0 && attackDecision_delayDone == true;
        Func<bool> sawEnemy() => () => enemyDetector.getVisibleEnemyTargets().Count > 0;
        Func<bool> noEnemyInSight() => () => enemyDetector.getVisibleEnemyTargets().Count == 0;
        Func<bool> shouldFlee() => () => {
            return attackDecision_flee;
        };
        Func<bool> shouldStayAttack() => () =>  attackDecision_stayAttack == true;
        Func<bool> shouldChaseAttack() => () =>  attackDecision_chaseAttack == true;
        Func<bool> shouldIgnore() => () =>  attackDecision_ignore == true;
        Func<bool> reachedFleeTarget() => () => fleeTarget != null && Vector3.Distance(transform.position, fleeTarget) < 1f;
        // Func<bool> TargetIsDepletedAndICanCarryMore() => () => (Target == null || Target.IsDepleted) && !InventoryFull().Invoke();
        // Func<bool> InventoryFull() => () => _gathered >= _maxCarried;
        // Func<bool> ReachedStockpile() => () => StockPile != null && 
                                            //    Vector3.Distance(transform.position, StockPile.transform.position) < 1f;
    }

    private void Update() => _stateMachine.Tick();

    public void shoot(){
        GameObject b = Instantiate(this.bullet, bulletSpawnPoint.transform.position, Quaternion.Euler(new Vector3(90, 0 ,0)));
        // b.GetComponent<BulletController>().move(bulletSpawnPoint.transform.forward);
        b.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.transform.forward * this.bulletSpeed;
    }

    public string fleeOrAttack(){

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