using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineAiPlayerCollisionController : MonoBehaviour
{

    private PlayerStatus ps;
    private MachineAiPlayerController pc;
    private float timePassedBullet;

// LIFECYCLE METHODS  ---------------------------------------------------------------------------------
    void Start()
    {
        ps = this.GetComponent<PlayerStatus>();
        pc = this.GetComponent<MachineAiPlayerController>();
        this.timePassedBullet = 0;
    }

    void Update()
    {
        timePassedSinceLastBulletHit();
        if (this.timePassedBullet >= 2){
            this.ps.beingAttacked = false;
        }
    }

// COLLISION DETECTION ---------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet"){
            if (other.gameObject.GetComponent<BulletController>().shotByTeam != ps.team){
                
                float remainingHealth = ps.getHealth() - ps.getBulletDamage();
                ps.setHealth(remainingHealth);

                //Rewards for the player hit 
                // pc.giveReward("beenHitBonus");
                
                //Rewards for the shooter
                var shooter = other.gameObject.GetComponent<BulletController>().shotBy.GetComponent<MachineAiPlayerController>();
                // shooter.giveReward("hitEnemyBonus");
                if (remainingHealth <= 0 ){
                    // shooter.giveReward("killedEnemyBonus");
                }

                ps.beingAttacked = true;
                this.timePassedBullet = 0;
            }
            Destroy(other.gameObject);
        }
        
    }

    private void OnCollisionEnter (Collision other) {
        if (other.gameObject.tag == "Bullet"){
            if (other.gameObject.GetComponent<BulletController>().shotByTeam != ps.team){
                
                float remainingHealth = ps.getHealth() - ps.getBulletDamage();
                ps.setHealth(remainingHealth);

                //Rewards for the player hit 
                // pc.giveReward("beenHitBonus");
                
                //Rewards for the shooter
                var shooter = other.gameObject.GetComponent<BulletController>().shotBy.GetComponent<MachineAiPlayerController>();
                // shooter.giveReward("hitEnemyBonus");
                if (remainingHealth <= 0 ){
                    // shooter.giveReward("killedEnemyBonus");
                }

                ps.beingAttacked = true;
                this.timePassedBullet = 0;
            }
            Destroy(other.gameObject);
        }
        
    }

// METHODS --------------------------------------------------------------------------------------------

	private void timePassedSinceLastBulletHit(){
        this.timePassedBullet += Time.deltaTime;
    }
    
}
