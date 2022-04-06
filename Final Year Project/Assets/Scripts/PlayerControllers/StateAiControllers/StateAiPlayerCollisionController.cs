using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAiPlayerCollisionController : MonoBehaviour
{

    private PlayerStatus ps;
    private float timePassedBullet;

    // Start is called before the first frame update
    void Start()
    {
        ps = this.GetComponent<PlayerStatus>();
        this.timePassedBullet = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timePassedSinceLastBulletHit();
        if (this.timePassedBullet > 2){
            ps.beingAttacked = false;
        }
    }

// Collision Detection ---------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet"){ 
            float remainingHealth = ps.getHealth() - ps.getBulletDamage();
            ps.setHealth(remainingHealth);
            ps.beingAttacked = true;
            this.timePassedBullet = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    private void OnCollisionEnter (Collision other) {
        if (other.gameObject.tag == "Bullet"){ 
            float remainingHealth = ps.getHealth() - ps.getBulletDamage();
            ps.setHealth(remainingHealth);
            ps.beingAttacked = true;
            this.timePassedBullet = 0;
        }
    }

// Methods --------------------------------------------------------------------------------------------

	private void timePassedSinceLastBulletHit(){
        this.timePassedBullet += Time.deltaTime;
    }
    
}
