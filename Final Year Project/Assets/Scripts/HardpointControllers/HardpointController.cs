using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardpointController : MonoBehaviour
{

    private string state;   // unallocated, captured, congested
    private string owner;   // red, blue, none
    
    private float counter;
    private bool counterActive;
    private float captureCounterTotal = 3f;
    private float defendCounterTotal = 10f;

    private float capturePoints = 15f;
    private float defendPoints = 5f;
    
    private string color; // red, blue, green = unallocated, purple = congested
    private List<GameObject> playersInside = new List<GameObject>();

    private GameStatus gs;



// Getter Methods --------------------------------------------------------------------------------------
    public string getState(){ return this.state; }
    public string getOwner(){ return this.owner; }
    public string getColor(){ return this.color; }



// Lifecycle Methods -----------------------------------------------------------------------------------
    void Start()
    {
        this.state = "unallocated";
        this.counterActive = false;
        this.owner = "none";
        this.color = "green";
        this.gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
    }

    void Update()
    {
        if (!gs.getGameOver()){
            updateState();

            if (this.counterActive){
                countDown();
            }
        }
    }



// Collision Detection ---------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RedPlayer" || other.tag == "BluePlayer"){ 
            this.playersInside.Add(other.gameObject);
            updateState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RedPlayer" || other.tag == "BluePlayer"){ 
            this.playersInside.Remove(other.gameObject);
            updateState();
        }
    }



// Methods --------------------------------------------------------------------------------------------
    private void updateState(){
        bool red = false;
        bool blue = false;
        for (int i = 0; i < playersInside.Count; i++){
            if (playersInside[i].GetComponent<PlayerStatus>().team == "red"){
                red = true; //At least one red in the hardpoint zone
            }
            
            if (playersInside[i].GetComponent<PlayerStatus>().team == "blue"){
                blue = true; //At least one blue in the hardpoint zone
            }
        }

        // If no one is in the hardpoint
        // No change in state of the hardpoint
        if (red == false && blue == false){     
            this.counterActive = false;
        }
        
        // If a player from each team is in the hard point
        // Neither team can capture/defend
        if (red == true && blue == true){
            congested();
        }
        
        // If only red players are in the hardpoint
        // Red can capture/defend
        if (red == true && blue == false){
            updateColor();
            if (this.owner == "none"){
                this.state = "unallocated";
                capture("red");
            }
            else if (this.owner == "red"){
                this.state = "captured";
                defend("red");
            }
            else if (this.owner == "blue"){
                this.state = "captured";
                capture("red");
            }
        }
        
        // If only blue players are in the hardpoint
        // Blue can capture/defend
        if (red == false && blue == true){
            updateColor();
            if (this.owner == "none"){
                this.state = "unallocated";
                capture("blue");
            }
            else if (this.owner == "blue"){
                this.state = "captured";
                defend("blue");
            }
            else if (this.owner == "red"){
                this.state = "captured";
                capture("blue");
            }
        }
    }

    private void congested(){
        Debug.Log("Hardpoint Congested");
        this.state = "congested";
        this.counterActive = false;
        updateColor();
    }

    private void capture(string team){
        if (this.counterActive == true && this.counter <= 0f){
            Debug.Log("Successfully Captured");
            
            gs.addToScore(team, capturePoints);
            this.state = "captured";
            this.owner = team;
            this.counter = defendCounterTotal;
            updateColor();
        }
        else if (this.counterActive == false){
            this.counterActive = true;
            this.counter = captureCounterTotal;
        }
    }

    private void defend(string team){
        if (this.counterActive == true && this.counter <= 0f){
            Debug.Log("Successfully Defended");
            
            gs.addToScore(team, defendPoints);
            this.state = "captured";
            this.counter = defendCounterTotal;
        }
        else if (this.counterActive == false){
            this.counterActive = true;
            this.counter = defendCounterTotal;
        }
    }

    private void countDown(){
        this.counter -= Time.deltaTime;
    }

    private void updateColor(){
        if (this.owner == "none" || this.state == "unallocated"){
            this.color = "green";
        }
        else if (this.state == "congested"){
            this.color = "purple";
        }
        else if (this.owner == "red" && this.state == "captured"){
            this.color = "red";
        }
        else if (this.owner == "blue" && this.state == "captured"){
            this.color = "blue";
        }
        
        switch(this.color) 
        {
            case "green":
                Debug.Log("Color should be green");
                break;
            case "purple":
                Debug.Log("Color should be purple");
                break;
            case "red":
                Debug.Log("Color should be red");
                break;
            case "blue":
                Debug.Log("Color should be blue");
                break;
            default:
                break;
            }
    }
}
