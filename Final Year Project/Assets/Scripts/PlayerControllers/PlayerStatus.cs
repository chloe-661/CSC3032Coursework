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

    //private Random rnd = new Random();
    private bool inPlay;

    private GameStatus gs;

    private List<GameObject> respawnPoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        this.gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
        this.inPlay = true;
        this.health = 100f;
        this.captureScore = 0f;
        this.defendScore = 0f;
        this.killScore = 0f;
        this.totalScore = 0f;

        whichTeam();
        getRespawnPoints();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gs.getGameOver()){
            this.totalScore = this.captureScore + this.defendScore;

            isDead();

            if(this.inPlay){
                //can move....
            }
        }
    }

    public float getCaptureScore(){ return this.captureScore; }
    public float getDefendScore(){ return this.defendScore; }
    public float getKillScore(){ return this.killScore; }
    public float getTotalScore(){ return this.totalScore; }

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
                this.inPlay = false;
                respawn();
            }
        }
    }

    private void getRespawnPoints(){
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
    private void respawn(){
        StartCoroutine(respawnWaiter());
    }

    IEnumerator respawnWaiter()
    {
        int num = Random.Range(0, this.respawnPoints.Count);
        float x = this.respawnPoints[num].transform.position.x;
        float y = this.respawnPoints[num].transform.position.y;
        float z = this.respawnPoints[num].transform.position.z;
        
        //TO DO
        //Rotate the player to look at a target in the center of the map
        
        this.transform.position = new Vector3(x, y, z);

        yield return new WaitForSeconds(3);
        this.health = 100f;
        this.inPlay = true;

        string logMessage = (this.team + ":" + this.name + " respawned");
        gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {logMessage});
    }
}
