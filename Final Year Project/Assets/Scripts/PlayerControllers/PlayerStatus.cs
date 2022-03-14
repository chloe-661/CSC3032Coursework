using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{

    private float x;
    private float y;
    public string team;
    private float health;
    private bool dead;

    private GameStatus gs;

    // Start is called before the first frame update
    void Start()
    {
        this.dead = false;
        this.health = 100f;
        whichTeam();
        this.gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gs.getGameOver()){

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
            //do countdown first
            respawn();
        }
    }

    private void respawn(){
        this.dead = false;
        this.health = 100f;
    }
}
