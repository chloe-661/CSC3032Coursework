using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    
    public string name;
    private string aiType;
    public string team;
    
    private float x;
    private float y;
    
    private float health;
    private bool dead;
    
    private float captureScore;
    private float defendScore;
    private float killScore;
    private float totalScore;

    private GameStatus gs;

    // Start is called before the first frame update
    void Start()
    {
        this.dead = false;
        this.health = 100f;
        this.captureScore = 0f;
        this.defendScore = 0f;
        this.killScore = 0f;
        this.totalScore = 0f;

        whichTeam();
        this.gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gs.getGameOver()){
            this.totalScore = this.captureScore + this.defendScore;
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
            //do countdown first
            respawn();
        }
    }

    private void respawn(){
        this.dead = false;
        this.health = 100f;

        string logMessage = (this.team + ":" + this.name + " respawned");
        gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {logMessage});
    }
}
