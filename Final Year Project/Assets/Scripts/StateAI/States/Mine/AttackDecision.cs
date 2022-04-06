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
    public void OnEnter()
    {
    //    this.player.attackDecision_delayDone = false;
    }

    public void Tick()
    {
        // decisions();
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