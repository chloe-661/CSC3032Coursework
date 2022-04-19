using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float SPEED = 50f;

    public GameObject shotBy;
    public string shotByTeam;
    public GameObject hit;

    private Rigidbody rb;
    void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.shotByTeam = this.shotBy.GetComponent<PlayerStatus>().team;
    }

//METHODS --------------------------------------------------------------------------------------------------------

    public void move(Vector3 direction){
        this.rb.velocity = direction * this.SPEED;
    }

//COLLISION METHODS -----------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        this.hit = other.gameObject;

        if (   this.hit.tag == "Wall" || this.hit.tag == "Ground")
        {    
            Debug.Log("T: Hit a wall/ground");
            Destroy(this.gameObject);
        }
        // else if (   (this.hit.tag == "RedPlayer" && shotBy.GetComponent<PlayerStatus>().team == "blue") 
        //          || (this.hit.tag == "BluePlayer"&& shotBy.GetComponent<PlayerStatus>().team == "red"))
        // {
        //     Debug.Log("T: Hit a enemy player");
        //     if (this.shotBy.GetComponent<PlayerStatus>().aiType == "machine"){
        //         this.shotBy.GetComponent<MachineAiPlayerController>().giveReward("hitEnemyBonus");
        //     }

        //     if (this.hit.GetComponent<PlayerStatus>().aiType == "machine"){
        //         this.hit.GetComponent<MachineAiPlayerController>().takeHealth();
        //         this.hit.GetComponent<MachineAiPlayerController>().giveReward("beenHitBonus");
        //     }
        //     Destroy(this.gameObject);
        // }
        // else {
        //     Destroy(this.gameObject);
        // }
    }

    private void OnCollisionEnter (Collision other) {

        this.hit = other.gameObject;

        if (   this.hit.tag == "Wall" || this.hit.tag == "Ground")
        {   
            Debug.Log("C: Hit a wall/ground"); 
            Destroy(this.gameObject);
        }
        // else if (   (this.hit.tag == "RedPlayer" && shotBy.GetComponent<PlayerStatus>().team == "blue") 
        //          || (this.hit.tag == "BluePlayer"&& shotBy.GetComponent<PlayerStatus>().team == "red"))
        // {
        //     Debug.Log("C: Hit a enemy player");
        //     if (this.shotBy.GetComponent<PlayerStatus>().aiType == "machine"){
        //         this.shotBy.GetComponent<MachineAiPlayerController>().giveReward("hitEnemyBonus");
        //     }

        //     if (this.hit.GetComponent<PlayerStatus>().aiType == "machine"){
        //         this.hit.GetComponent<MachineAiPlayerController>().takeHealth();
        //         this.hit.GetComponent<MachineAiPlayerController>().giveReward("beenHitBonus");
        //     }
        //     Destroy(this.gameObject);
        // }
        // else {
        //     Destroy(this.gameObject);
        // }
    }
}
