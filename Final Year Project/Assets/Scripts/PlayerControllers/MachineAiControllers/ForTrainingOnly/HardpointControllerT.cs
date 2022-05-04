using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardpointControllerT : MonoBehaviour
{
    [Header("Materials")]
    public Material greenFade;
    public Material purpleFade;
    public Material redFade;
    public Material blueFade;

    [Header("Colors")]
    public Color green = new Color(0f,1f,0.1f);
    public Color purple = new Color(0.56f,0f,1f);
    public Color red = new Color(1f,0f,0f);
    public Color blue = new Color(0f,0.07f,1f);

    [Header("ProgressBar")]
    public ProgressBarCircle progressBar;

    [Header("GeneralInfo")]
    public string name;     // A, B, C
    public string state;   // unallocated, captured, congested
    public string owner;   // red, blue, none
    public string color;   // red, blue, green = unallocated, purple = congested

    [Header("RunTimeInfo")]
    public int runTimeDefends;
    public int runTimeCaptures;
    public bool counterActive;
    public float counter;

    [Header("Constants")]
    public float captureCounterTotal = 3f;
    public float defendCounterTotal = 10f;

    [Header("Objects")]
    public TrainingGameController tc;
    public GameStatusT gs;
    private List<GameObject> playersInside = new List<GameObject>();
    
    // Getter Methods --------------------------------------------------------------------------------------
    public string getState(){ return this.state; }
    public string getOwner(){ return this.owner; }
    public string getColor(){ return this.color; }
    public float getCounter(){ return this.counter; }
    public int getRunTimeDefends(){ return this.runTimeDefends; }
    public int getRunTimeCaptures(){ return this.runTimeCaptures; }
    public float getCaptureCounterTotal(){ return this.captureCounterTotal; }
    public float getDefendCounterTotal(){ return this.defendCounterTotal; }
    public List<GameObject> getPlayersInside(){ return this.playersInside; }

    // Lifecycle Methods -----------------------------------------------------------------------------------
    void Start()
    {
        this.state = "unallocated";
        this.counterActive = false;
        this.owner = "none";
        this.color = "green";
        this.progressBar.BarValue = 100;
        this.runTimeCaptures = 0;
        this.runTimeDefends = 0;
    }

    public void reset(){
        this.state = "unallocated";
        this.counterActive = false;
        this.owner = "none";
        this.color = "green";
        this.progressBar.BarValue = 100;
        this.runTimeCaptures = 0;
        this.runTimeDefends = 0;
        this.counter = 0f;
        this.playersInside.Clear();
    }

    void Update()
    {
        updateState();
        updateColor();

        if (this.counterActive){
            countDown();
        }

        // if (playersInside.Count > 0 && this.state != "congested"){
        //     for (int i = 0; i < playersInside.Count; i++){
        //         playersInside[i].GetComponent<MachineAiPlayerController>().giveReward("stayedInHardpointBonus");
        //     }
        // }
    }

    // Collision Detection ---------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RedPlayer" || other.tag == "BluePlayer"){ 
            this.playersInside.Add(other.gameObject);
            other.gameObject.GetComponent<PlayerStatusT>().inHardpoint = true;
            updateState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RedPlayer" || other.tag == "BluePlayer"){ 
            this.playersInside.Remove(other.gameObject);
            other.gameObject.GetComponent<PlayerStatusT>().inHardpoint = false;
            updateState();
        }
    }

    // Methods --------------------------------------------------------------------------------------------
    
    private void updateState(){
        bool red = false;
        bool blue = false;
        for (int i = 0; i < playersInside.Count; i++){
            if (playersInside[i].GetComponent<PlayerStatusT>().team == "red"){
                red = true; //At least one red in the hardpoint zone
            }
            
            if (playersInside[i].GetComponent<PlayerStatusT>().team == "blue"){
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
        // this.tc.congestedHardpoint();
        Debug.Log("Hardpoint Congested");
        
        this.state = "congested";
        this.counterActive = false;
        updateProgressBar();
        updateColor();
    }

    private void capture(string team){
        updateProgressBar(team);

        //If the counter has been counting down has reached 0
        if (this.counterActive == true && this.counter <= 0f){
            // this.tc.capturedHardpoint(team);
            
            Debug.Log("Successfully Captured");
            playersInside[0].GetComponent<PlayerStatusT>().addToCaptureScore(gs.getCapturePoints());

            this.state = "captured";
            this.owner = team;
            this.runTimeCaptures ++;
            this.counter = defendCounterTotal;            
            
            string logMessage = (team + ":" + playersInside[0].GetComponent<PlayerStatusT>().name + " captured hardpoint " + this.name);
            gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {logMessage});
        }
        //Otherwise start the counter
        else if (this.counterActive == false){
            this.counterActive = true;
            this.counter = captureCounterTotal;
        }
    }

    private void defend(string team){
        if (this.counterActive == true && this.counter <= 0f){
            // this.tc.defendedHardpoint(team);
            
            Debug.Log("Successfully Defended");

            this.state = "captured";
            this.runTimeDefends ++;
            this.counter = defendCounterTotal;
            string[] logMessageArray = new string[playersInside.Count];
            
            for (int i = 0; i < playersInside.Count; i++){
                playersInside[i].GetComponent<PlayerStatusT>().addToDefendScore(gs.getDefendPoints());
                logMessageArray[i] = (team + ":" + playersInside[i].GetComponent<PlayerStatusT>().name + " defended hardpoint " + this.name);
            }
            
            gs.writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), logMessageArray);
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

        updateHardpointColor();
        updateProgressBar();
    }

    private void updateHardpointColor(){
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();    
        switch(this.color) 
        {
            case "green":
                renderer.material = greenFade;
                break;
            case "purple":
                renderer.material = purpleFade;
                break;
            case "red":
                renderer.material = redFade;
                break;
            case "blue":
                renderer.material = blueFade;
                break;
            default:
                break;
        }
    }

    
    
//UI UPDATE METHODS ---------------------------------------------------------------------------

    private void updateProgressBar()
    {
        if (this.state == "congested"){
            this.progressBar.BarColor = this.purple;
            this.progressBar.BarValue = 0;
        } else {
            switch(this.color){
                case "green":
                    this.progressBar.BarColor = this.green;
                    break;
                case "red":
                    this.progressBar.BarColor = this.red;
                    break;
                case "blue":
                    this.progressBar.BarColor = this.blue;
                    break;
                // case "purple":
                //     this.progressBar.BarColor = this.purple;
                //     break;
                default:
                    this.progressBar.BarColor = this.green;
                    break;
            }
        }
    }
    private void updateProgressBar(string team)
    {
        if (team == "red"){
            this.progressBar.BarBackGroundColor = this.red;
        }
        else if (team == "blue"){
            this.progressBar.BarBackGroundColor = this.blue;
        }
        
        this.progressBar.BarValue = (this.captureCounterTotal - this.counter)/(this.captureCounterTotal/100);
    }
}
