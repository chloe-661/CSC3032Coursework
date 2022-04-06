using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Attack : IState
{
    private readonly StateAiPlayerController player;
    private NavMeshAgent navMeshAgent;
    private readonly StateAiEnemyDetection enemyDetector;
    private readonly Animator animator;

    private PlayerStatus ps;
    private GameObject enemy;

    private float counter;
    private int rndNum;
    private int currentNumOfEnemies;
    private int previousNumOfEnemies;

    public Attack(StateAiPlayerController player, NavMeshAgent navMeshAgent, StateAiEnemyDetection enemyDetector, Animator animator)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.enemyDetector = enemyDetector;
        this.animator = animator; 
        this.enemy = null;
        this.counter = 0;
        this.rndNum = 0;
        this.currentNumOfEnemies = 0 ;
        this.previousNumOfEnemies = 0;
    }

    public void OnEnter()
    {
        this.navMeshAgent.enabled = true;
        this.enemy = chooseEnemyToAttack();
        this.animator.SetBool("isRunning", false);
        this.animator.SetBool("isPatrolling", false);
        this.animator.SetBool("isAttacking", true);
    }

    private GameObject chooseEnemyToAttack(){
        bool chooseNewTarget = false;
        int numOfEnemies = this.enemyDetector.getVisibleEnemyTargets().Count;

        if (this.enemy != null){
            if (this.enemy.GetComponent<PlayerStatus>().inPlay == false){
                chooseNewTarget = true;
            }
            else if (numOfEnemies > 0)
            {
                //Target goes out of range
                chooseNewTarget = true;
                for (int i = 0; i < numOfEnemies; i++){
                    if (this.enemyDetector.getVisibleEnemyTargets()[i] == this.enemy){
                        chooseNewTarget = false;
                        break;
                    }
                }
            }
        }
        else {
            chooseNewTarget = true;
        }

        if (chooseNewTarget){
            if (numOfEnemies > 0){
                int rndNum = Random.Range(0, numOfEnemies-1);
                return this.enemyDetector.getVisibleEnemyTargets()[rndNum];
            }
        }

        return null;




        // bool targetStillThere = false;
        // int numOfEnemies = this.enemyDetector.getVisibleEnemyTargets().Count;

        // if (numOfEnemies > 0){
        //     if (this.enemy != null){
        //         for (int i = 0; i < numOfEnemies; i++){
        //             if (this.enemyDetector.getVisibleEnemyTargets()[i] == this.enemy){
        //                 targetStillThere = true;
        //             }
        //         }
        //     }


        //     if (!targetStillThere){
        //         int rndNum = Random.Range(0, numOfEnemies-1);
        //         return this.enemyDetector.getVisibleEnemyTargets()[rndNum];
        //     }

        // }
        // return null;
    }

    public void Tick()
    {
        
        if (this.enemy != null) {
            this.player.transform.LookAt(enemy.transform.position);
            //Shoots once every second
            if (this.counter > 1){
                this.counter = 0;
                this.player.shoot();
                this.enemy = chooseEnemyToAttack();
            }
        
            secondCounter();
        }
        else {
            this.enemy = chooseEnemyToAttack();
        }
        
    }

    private void secondCounter(){
        this.counter += Time.deltaTime;
    }

    public void OnExit()
    {
        this.animator.SetBool("isRunning", false);
        this.animator.SetBool("isPatrolling", false);
        this.animator.SetBool("isAttacking", false);
    }
}