using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    
    public string name;
    public string aiType;
    public string team;
    
    private float x;
    private float y;
    
    private float health;
    public bool dead;
    
    private float captureScore;
    private float defendScore;
    private float killScore;
    private float totalScore;

    private float bulletDamage = 25f;

    public bool inPlay;
    public bool inHardpoint;

    public bool beingAttacked;

    private GameObject centerTarget;

    private List<GameObject> respawnPoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        this.centerTarget = GameObject.FindWithTag("CenterTarget");
        this.inPlay = true;
        this.health = 100f;
        this.captureScore = 0f;
        this.defendScore = 0f;
        this.killScore = 0f;
        this.totalScore = 0f;
        this.inHardpoint = false;

        whichTeam();
        getRespawnPointsFromGame();
    }

    // Update is called once per frame
    void Update()
    {
        this.totalScore = this.captureScore + this.defendScore;

        isDead();

        if(this.inPlay){
            //can move....
        }
    }

    public float getCaptureScore(){ return this.captureScore; }
    public float getDefendScore(){ return this.defendScore; }
    public float getKillScore(){ return this.killScore; }
    public float getTotalScore(){ return this.totalScore; }
    public float getHealth(){ return this.health; }
    public float getBulletDamage(){ return this.bulletDamage; }
    public List<GameObject> getRespawnPoints() { return this.respawnPoints; }

    public void setHealth(float health){ 
        if (health >= 0 && health <= 100) {
            this.health = health;
        }
        else {
            this.health = 0;
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
                GameStatus gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
                gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {logMessage});
            }
        }
    }

    private void getRespawnPointsFromGame(){
        if (this.team == "red"){
            foreach(GameObject obj in GameObject.FindGameObjectsWithTag("RedRespawnPoint")) {
                this.respawnPoints.Add(obj);
            }
        }
        else if (this.team == "blue"){
            foreach(GameObject obj in GameObject.FindGameObjectsWithTag("BlueRespawnPoint")) {
                this.respawnPoints.Add(obj);
            }
        }
    }

    public void initalStartLocation(int index){
        float x = this.respawnPoints[index].transform.position.x;
        float y = 1.5f;
        float z = this.respawnPoints[index].transform.position.z;

        this.transform.position = new Vector3(x, y, z);
        this.transform.LookAt(this.centerTarget.transform);
    }
    private void respawn(){
        StartCoroutine(respawnWaiter());
    }

    IEnumerator respawnWaiter()
    {
        int num = Random.Range(0, this.respawnPoints.Count);
        float x = this.respawnPoints[num].transform.position.x;
        float y = this.respawnPoints[num].transform.position.y;
        float z = this.respawnPoints[num].transform.position.z;

        this.transform.position = new Vector3(x, y, z);
        this.transform.LookAt(this.centerTarget.transform);

        yield return new WaitForSeconds(3);
        this.health = 100f;
        this.inPlay = true;

        string logMessage = (this.team + ":" + this.name + " respawned");

        GameStatus gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
        gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {logMessage});
    }
}
