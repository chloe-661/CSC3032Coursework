using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    private bool gameOver;
    private float scoreToWin = 100f;
    private float redTeamScore;
    private float blueTeamScore;
    private string winner;



// Getter Methods --------------------------------------------------------------------------------------
    public float getRedTeamScore(){ return this.redTeamScore; }
    public float getBlueTeamScore(){ return this.blueTeamScore; }
    public string getWinner(){ 
        if (this.winner != null){
            return this.winner;
        }
        else {
            return "Game is still in play";
        }
    }
    public bool getGameOver(){ return this.gameOver; }



// Lifecycle Methods -----------------------------------------------------------------------------------
    void Start()
    {
        this.scoreToWin = 100f;
        this.gameOver = false;
        this.redTeamScore = 0f;
        this.blueTeamScore = 0f;
        this.winner = null;
    }
    void Update()
    {
        if (isGameOver()){
            Debug.Log(this.winner);
        }
    }



// Methods --------------------------------------------------------------------------------------------
    private bool isGameOver(){
        if (this.blueTeamScore >= this.scoreToWin){
            this.winner = "blue";
            return this.gameOver = true;
        }
        else if (this.redTeamScore >= this.scoreToWin){
            this.winner = "red";
            return this.gameOver = true;
        }
        else {
            return this.gameOver = false;
        }
    }

    public void addToScore(string team, float num){
        switch(team){
            case "red":
                this.redTeamScore += num;
                break;
            case "blue":
                this.blueTeamScore += num;
                break;
            default:
                break;
        }
    }
}
