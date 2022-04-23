using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStatusT : MonoBehaviour
{
//Fields ----------------------------------------------------------------------------------------------------------------------------------------------
    [Header("GeneralInfo")]
    public string name;
    public string aiType;
    public string team;
    
    [Header("PlayerProperties")]
    public float health;
    public bool dead;
    public Vector3 previousStartLocation;

    [Header("InGameProperties")]
    public float captureScore;
    public float defendScore;
    public float killScore;
    public float totalScore;
    public bool inPlay;
    public bool inHardpoint;
    public bool beingAttacked;
    public List<GameObject> respawnPoints = new List<GameObject>();

    [Header("Constants")]
    public float bulletDamage = 25f;

    [Header("Counters/Timers")]
    public float counter;

    [Header("Scripts/Objects")]
    public GameObject centerTarget;
    public GameStatusT gs;
    public GameObject redSpawnArea;
    public GameObject blueSpawnArea;

// GETTER METHODS --------------------------------------------------------------------------

    public float getCaptureScore(){ return this.captureScore; }
    public float getDefendScore(){ return this.defendScore; }
    public float getKillScore(){ return this.killScore; }
    public float getTotalScore(){ return this.totalScore; }
    public float getHealth(){ return this.health; }
    public float getBulletDamage(){ return this.bulletDamage; }
    public List<GameObject> getRespawnPoints() { return this.respawnPoints; }
    
//Lifecycle Methods ----------------------------------------------------------------------------------------------------------------------------------------------

    public void Start()
    {
        this.inPlay = true;
        this.health = 100f;
        this.captureScore = 0f;
        this.defendScore = 0f;
        this.killScore = 0f;
        this.totalScore = 0f;
        this.inHardpoint = false;
        this.beingAttacked = false;
        this.counter = 0f;
        this.previousStartLocation = Vector3.zero;

        whichTeam();
        getRespawnPointsFromGame();
    }

    public void Update()
    {
        this.totalScore = this.captureScore + this.defendScore;

        isDead();

        if(this.inPlay){
            if (this.counter >= 2){
                healthRegen();
                this.counter = 0;
            }
            this.counter += Time.deltaTime;
        }
    }

    public void reset()
    {
        this.inPlay = true;
        this.health = 100f;
        this.captureScore = 0f;
        this.defendScore = 0f;
        this.killScore = 0f;
        this.totalScore = 0f;
        this.inHardpoint = false;
        this.beingAttacked = false;
        this.counter = 0f;
        this.transform.localPosition = this.previousStartLocation;
        this.transform.LookAt(this.centerTarget.transform);
    }

//Methods ----------------------------------------------------------------------------------------------------------------------------------------------

    public void setHealth(float health)
    { 
        if (health >= 0 && health <= 100) {
            this.health = health;
        }
        else {
            this.health = 0;
        }
    }

    public void addToHealth(float amount){ 
        if (this.health + amount <= 100) {
            this.health += amount;
        }
        else {
            this.health = 100;
        }
    }

    public void addToCaptureScore(float num){
        this.captureScore += num;
    }

    public void addToDefendScore(float num){
        this.defendScore += num;
    }

    public void addToKillScore(){
        this.killScore += 1;
    }

    private void healthRegen(){
        if (this.health < 100){
            addToHealth(10f);
        }
    }

    private string whichTeam(){
        switch(this.tag){
            case "RedPlayer":
                this.team = "red";
                break;
            case "BluePlayer":
                this.team = "blue";
                break;
            default:
                this.team = null;
                break;
        }
        return this.team;
    }

    private void isDead(){
        if (this.health <= 0 || this.dead == true){
            if (this.inPlay == true) {
                this.dead = true;
                this.inPlay = false;

                //FSM will reset these....

                string logMessage = (this.team + ":" + this.name + " respawned");
                gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {logMessage});
            }
        }
    }

    private void getRespawnPointsFromGame(){
        if (this.team == "red"){
            foreach (Transform child in redSpawnArea.transform){
                this.respawnPoints.Add(child.gameObject);
            }
        }
        else if (this.team == "blue"){
            foreach (Transform child in blueSpawnArea.transform){
                this.respawnPoints.Add(child.gameObject);
            }
        }
    }

    public void initalStartLocation(int index){
        this.GetComponent<TrailRenderer>().enabled = false;
        if (index <= respawnPoints.Count){
            float x = this.respawnPoints[index].transform.localPosition.x;
            float y = 1.3f;
            float z = this.respawnPoints[index].transform.localPosition.z;

            this.transform.localPosition = new Vector3(x, y, z);
            this.previousStartLocation = new Vector3(x, y, z);
        }
        
        this.transform.LookAt(this.centerTarget.transform.localPosition);
        this.GetComponent<TrailRenderer>().enabled = true;
    }
}
