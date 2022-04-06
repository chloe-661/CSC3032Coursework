using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class AttackDecision : IState
{
    private readonly StateAiPlayerController player;
    private PlayerStatus ps;
    private readonly StateAiEnemyDetection _enemyDetector;

    public bool ignore;
    public bool stayAttack;
    public bool chaseAttack;
    public bool flee;

    public AttackDecision(StateAiPlayerController player, StateAiEnemyDetection enemyDetector)
    {
        this.player = player;
        ps = this.player.GetComponent<PlayerStatus>();
        _enemyDetector = enemyDetector;
        this.player.attackDecision_delayDone = true;
    }

    public void Tick()
    {
        decisions();
    }
        //IF NOT BEING ATTACKED/CHASED
            //IF IN HARDPOINT

            //IF NOT IN HARDPOINT
                //IF 1 PLAYER
                    //IF MORE THAN HALF WAY TO TARGET
                        //FLEE = 10%
                        //STAY&ATTACK = 40%
                        //CHASE%ATTACK = 10%
                        //INGORE = 40%
                    //IF LESS THAN HALF WAY TO TARGET
                        //FLEE = 10%
                        //STAY&ATTACK = 35%
                        //CHASE%ATTACK = 45%
                        //INGORE = 10%
                //IF 2 PLAYER
                    //IF MORE THAN HALF WAY TO TARGET
                        //FLEE = 15%
                        //STAY&ATTACK = 40%
                        //CHASE%ATTACK = 25%
                        //INGORE = 20%
                    //IF LESS THAN HALF WAY TO TARGET
                        //FLEE = 40%
                        //STAY&ATTACK = 20%
                        //CHASE%ATTACK = 30%
                        //INGORE = 10%                    
                //IF 3 PLAYER
                    //IF MORE THAN HALF WAY TO TARGET
                        //FLEE = 50%
                        //STAY&ATTACK = 30%
                        //CHASE%ATTACK = 10%
                        //INGORE = 10%
                    //IF LESS THAN HALF WAY TO TARGET
                        //FLEE = 75%
                        //STAY&ATTACK = 10%
                        //CHASE%ATTACK = 10%
                        //INGORE = 5%
                //IF 4 PLAYER
                    //IF MORE THAN HALF WAY TO TARGET
                        //FLEE = 75%
                        //STAY&ATTACK = 10%
                        //CHASE%ATTACK = 10%
                        //INGORE = 5%
                    //IF LESS THAN HALF WAY TO TARGET
                        //FLEE = 85%
                        //STAY&ATTACK = 6%
                        //CHASE%ATTACK = 6%
                        //INGORE = 3%
                //IF BEING ATTACKED
                    //FLEE = 40%
                    //STAY&ATTACK = 25%
                    //CHASE%ATTACK = 30%
                    //INGORE = 5%


    private void decisions(){
        string decision = "";

        if (ps.inHardpoint == false){
            if (ps.beingAttacked == true){
                //FLEE = 40%
                //STAY&ATTACK = 25%
                //CHASE%ATTACK = 30%
                //INGORE = 5%
                var options = new List<KeyValuePair<string, float>>() 
                { 
                    new KeyValuePair<string, float>("flee", 0.4f),
                    new KeyValuePair<string, float>("stayAttack", 0.25f),
                    new KeyValuePair<string, float>("chaseAttack", 0.3f),
                    new KeyValuePair<string, float>("ignore", 0.05f),
                };
                decision = makeDecision(options);
            }
            else {
                var totalTargets = _enemyDetector.getVisibleEnemyTargets().Count;

                if (totalTargets == 1){                      
                    if (Vector3.Distance(this.player.transform.position, this.player.Target.transform.position) <= 15f){
                        //FLEE = 10%
                        //STAY&ATTACK = 40%
                        //CHASE%ATTACK = 10%
                        //INGORE = 40%
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("flee", 0.1f),
                            new KeyValuePair<string, float>("chaseAttack", 0.1f),
                            new KeyValuePair<string, float>("stayAttack", 0.4f),
                            new KeyValuePair<string, float>("ignore", 0.4f),
                        };
                        decision = makeDecision(options);
                    }
                    else {
                        //FLEE = 10%
                        //STAY&ATTACK = 35%
                        //CHASE%ATTACK = 45%
                        //INGORE = 10%
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("flee", 0.1f),
                            new KeyValuePair<string, float>("ignore", 0.1f),
                            new KeyValuePair<string, float>("stayAttack", 0.35f),
                            new KeyValuePair<string, float>("chaseAttack", 0.45f),
                        };
                        decision = makeDecision(options);
                    }
                }
                else if (totalTargets == 2){                         
                    if (Vector3.Distance(this.player.transform.position, this.player.Target.transform.position) <= 15f){
                        //FLEE = 15%
                        //STAY&ATTACK = 40%
                        //CHASE%ATTACK = 25%
                        //INGORE = 20%
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("flee", 0.15f),
                            new KeyValuePair<string, float>("ignore", 0.2f),
                            new KeyValuePair<string, float>("chaseAttack", 0.25f),
                            new KeyValuePair<string, float>("stayAttack", 0.4f),
                        };
                        decision = makeDecision(options);
                    }
                    else {
                        //FLEE = 40%
                        //STAY&ATTACK = 20%
                        //CHASE%ATTACK = 30%
                        //INGORE = 10%   
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("ignore", 0.1f),   
                            new KeyValuePair<string, float>("stayAttack", 0.2f),
                            new KeyValuePair<string, float>("chaseAttack", 0.3f),
                            new KeyValuePair<string, float>("flee", 0.4f), 
                        };
                        decision = makeDecision(options);
                    }
                }
                else if (totalTargets == 3){                       
                    if (Vector3.Distance(this.player.transform.position, this.player.Target.transform.position) <= 15f){
                         //FLEE = 50%
                        //STAY&ATTACK = 30%
                        //CHASE%ATTACK = 10%
                        //INGORE = 10%
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("chaseAttack", 0.1f),
                            new KeyValuePair<string, float>("ignore", 0.1f),
                            new KeyValuePair<string, float>("stayAttack", 0.3f),
                            new KeyValuePair<string, float>("flee", 0.5f),
                        };
                        decision = makeDecision(options);
                    }
                    else {
                        //FLEE = 75%
                        //STAY&ATTACK = 10%
                        //CHASE%ATTACK = 10%
                        //INGORE = 5%
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("ignore", 0.05f),
                            new KeyValuePair<string, float>("stayAttack", 0.1f),
                            new KeyValuePair<string, float>("chaseAttack", 0.1f),
                            new KeyValuePair<string, float>("flee", 0.75f),
                        };
                        decision = makeDecision(options);
                    }
                }
                else if (totalTargets == 4){                       
                    if (Vector3.Distance(this.player.transform.position, this.player.Target.transform.position) <= 15f){
                        //FLEE = 75%
                        //STAY&ATTACK = 10%
                        //CHASE%ATTACK = 10%
                        //INGORE = 5%
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("ignore", 0.05f),
                            new KeyValuePair<string, float>("stayAttack", 0.1f),
                            new KeyValuePair<string, float>("chaseAttack", 0.1f),
                            new KeyValuePair<string, float>("flee", 0.75f),
                        };
                        decision = makeDecision(options);
                    }
                    else {
                        //FLEE = 85%
                        //STAY&ATTACK = 6%
                        //CHASE%ATTACK = 6%
                        //INGORE = 3%
                        var options = new List<KeyValuePair<string, float>>() 
                        { 
                            new KeyValuePair<string, float>("ignore", 0.03f),
                            new KeyValuePair<string, float>("stayAttack", 0.06f),
                            new KeyValuePair<string, float>("chaseAttack", 0.06f),
                            new KeyValuePair<string, float>("flee", 0.85f),
                        };
                        decision = makeDecision(options);
                    }
                }
                else {
                    //FLEE = 0%
                    //STAY&ATTACK = 0%
                    //CHASE%ATTACK = 0%
                    //INGORE = 100%
                    var options = new List<KeyValuePair<string, float>>() 
                    { 
                        new KeyValuePair<string, float>("flee", 0f),
                        new KeyValuePair<string, float>("stayAttack", 0f),
                        new KeyValuePair<string, float>("chaseAttack", 0f),
                        new KeyValuePair<string, float>("ignore", 1f),
                    };
                    decision = makeDecision(options);
                }
            }
        }
        Debug.Log(decision);

        switch(decision){
            case "flee":
                this.flee = true;
                this.stayAttack = false;
                this.chaseAttack = false;
                this.ignore = false;
                break;
            case "stayAttack":
                this.flee = false;
                this.stayAttack = true;
                this.chaseAttack = false;
                this.ignore = false;
                break;
            case "chaseAttack":
                this.flee = false;
                this.stayAttack = false;
                this.chaseAttack = true;
                this.ignore = false;
                break;
            case "ignore":
                this.flee = false;
                this.stayAttack = false;
                this.chaseAttack = false;
                this.ignore = true;
                break;
            default:
                this.flee = false;
                this.stayAttack = false;
                this.chaseAttack = false;
                this.ignore = true;
                break;
        }

        this.player.attackDecision_flee = this.flee;
        this.player.attackDecision_stayAttack = this.stayAttack;
        this.player.attackDecision_chaseAttack = this.chaseAttack;
        this.player.attackDecision_ignore = this.ignore;
    }

    private float generateRandom(){
        float rndNum = Random.Range(0f, 1f);
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

    public void OnEnter()
    {
       this.player.attackDecision_delayDone = false;
    }
    public void OnExit()
    {
        decisionResetDelay();
    }

    IEnumerator decisionResetDelay(){
        Debug.Log("DelayResetter is running1");
        yield return new WaitForSeconds(2);
        Debug.Log("DelayResetter is running2");
        this.player.attackDecision_delayDone = true;
    }
}