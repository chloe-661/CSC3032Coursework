using System;
using UnityEngine;
using UnityEngine.AI;

public class StateAiPlayerController : MonoBehaviour
{
    public event Action<int> OnGatheredChanged;
    
    [SerializeField] private int _maxCarried = 20;

    public PlayerStatus ps;
    
    private StateMachine _stateMachine;
    private int _gathered;
    
    public GameObject Target { get; set; } //aka Hardpoint
    public StockPile StockPile { get; set; }


    private void Awake()
    {
        ps = this.GetComponent<PlayerStatus>();
        var navMeshAgent = this.GetComponent<NavMeshAgent>();
        var animator = this.GetComponent<Animator>();
        //var enemyDetector = this.AddComponent<EnemyDetector>();
        
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
        // var returnToStockpile = new ReturnToStockpile(this, navMeshAgent, animator);
        // var placeResourcesInStockpile = new PlaceResourcesInStockpile(this);
        // var flee = new Flee(this, navMeshAgent, enemyDetector, animator, fleeParticleSystem);
        
        At(search, moveToSelected, HasTarget());
        At(moveToSelected, search, StuckForOverASecond());
        At(moveToSelected, patrol, ReachedHardpoint());
        // At(patrol, search, something());
        // At(harvest, search, TargetIsDepletedAndICanCarryMore());
        // At(harvest, returnToStockpile, InventoryFull());
        // At(returnToStockpile, placeResourcesInStockpile, ReachedStockpile());
        // At(placeResourcesInStockpile, search, () => _gathered == 0);
        
        // _stateMachine.AddAnyTransition(flee, () => enemyDetector.EnemyInRange);
        // At(flee, search, () => enemyDetector.EnemyInRange == false);
        
        _stateMachine.SetState(search);

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
        Func<bool> HasTarget() => () => Target != null;
        Func<bool> StuckForOverASecond() => () => moveToSelected.TimeStuck > 1f;
        Func<bool> ReachedHardpoint() => () => Target != null && 
                                                Vector3.Distance(transform.position, Target.transform.position) < 1f;
        // Func<bool> TargetIsDepletedAndICanCarryMore() => () => (Target == null || Target.IsDepleted) && !InventoryFull().Invoke();
        // Func<bool> InventoryFull() => () => _gathered >= _maxCarried;
        // Func<bool> ReachedStockpile() => () => StockPile != null && 
                                            //    Vector3.Distance(transform.position, StockPile.transform.position) < 1f;
    }

    private void Update() => _stateMachine.Tick();

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