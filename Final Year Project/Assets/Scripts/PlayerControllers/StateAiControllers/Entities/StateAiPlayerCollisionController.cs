using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAiPlayerCollisionController : MonoBehaviour
{

    private PlayerStatus ps;
    private float timePassedBullet;

// LIFECYCLE METHODS  ---------------------------------------------------------------------------------
    void Start()
    {
        ps = this.GetComponent<PlayerStatus>();
        this.timePassedBullet = 0;
    }

    void Update()
    {
        timePassedSinceLastBulletHit();
        if (this.timePassedBullet >= 2){
            ps.beingAttacked = false;
        }
    }

// COLLISION DETECTION ---------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet"){
            if (other.gameObject.GetComponent<BulletController>().shotByTeam != ps.team){
                float remainingHealth = ps.getHealth() - ps.getBulletDamage();
                ps.setHealth(remainingHealth);

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
