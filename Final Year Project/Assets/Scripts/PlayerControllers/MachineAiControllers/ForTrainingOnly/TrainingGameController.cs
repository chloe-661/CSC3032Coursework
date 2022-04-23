using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;
using UnityEngine.UI;

public class TrainingGameController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject machineRedTeam;
    public GameObject machineBlueTeam;
    public GameObject hardpoints;
    public GameObject gameStatus;

    [Header("Teams")]
    public SimpleMultiAgentGroup redTeamAgentGroup; //TeamID: 0
    public SimpleMultiAgentGroup blueTeamAgentGroup; // TeamID: 1
    public List<PlayerStatusT> redTeamPlayers;
    public List<PlayerStatusT> blueTeamPlayers;

    [Header("GroupRewards")]
    public const float CAPTURED_HARDPOINT_BONUS = 10f;
    public const float DEFENDED_HARDPOINT_BONUS = 2f;       
    public const float WON_BONUS = 50f;
    public const float LOST_BONUS = -25f;

    [Header("Other")]
    public bool initialized;
    public int currentSteps;
    public const int MAX_STEPS = 25000;

    [Header("Objects")]
    public GameStatusT gs;
    public List<GameObject> requiredObjects = new List<GameObject>();
    public List<HardpointControllerT> hardpointsList = new List<HardpointControllerT>();

//LIFECYCLE METHODS --------------------------------------------------------------------------------------
    void Start(){
        this.initialized = false;
        redTeamAgentGroup = new SimpleMultiAgentGroup();
        blueTeamAgentGroup = new SimpleMultiAgentGroup();

        StartCoroutine(startDelay());
    }

    IEnumerator startDelay(){
        yield return new WaitForSeconds(1f);
        foreach (Transform child in this.transform)
        {
            foreach (Transform grandchild in child){
                if (grandchild.tag == "BluePlayer" && grandchild.gameObject.active){
                    this.blueTeamPlayers.Add(grandchild.GetComponent<PlayerStatusT>());
                    blueTeamAgentGroup.RegisterAgent(grandchild.GetComponent<MachineAiPlayerController>());
                    grandchild.GetComponent<MachineAiPlayerController>().behaviorParameters.TeamId = 1;
                }
                if (grandchild.tag == "RedPlayer" && grandchild.gameObject.active){
                    this.redTeamPlayers.Add(grandchild.GetComponent<PlayerStatusT>());
                    redTeamAgentGroup.RegisterAgent(grandchild.GetComponent<MachineAiPlayerController>());
                    grandchild.GetComponent<MachineAiPlayerController>().behaviorParameters.TeamId = 0;
                }   
                if (grandchild.tag == "Hardpoint" && grandchild.gameObject.active){
                    this.hardpointsList.Add(grandchild.GetComponent<HardpointControllerT>());
                }
            }
            if (child.tag == "GameStatus"){
                    this.gs = child.GetComponent<GameStatusT>();
                }
        }

        startNewGame();
    }

    void Update()
    {
        // if (!this.initialized)
        // {
        //     Initialize();
        // }
    }

    void FixedUpdate()
    {
        if (this.initialized){
            
            //RESET SCENE IF WE REACH MAX_STEPS
            this.currentSteps += 1;
            if (this.currentSteps >= MAX_STEPS)
            {
                this.redTeamAgentGroup.GroupEpisodeInterrupted();
                this.blueTeamAgentGroup.GroupEpisodeInterrupted();
                this.initialized = false;
                this.currentSteps = 0;
                startNewGame();
            }

            //RESET SCENE IF GAME IS WON
            //ADD GROUP REWARDS
            if (this.gs.getGameOver()){
                var winningGroup = gs.getWinner() == "red" ? redTeamAgentGroup : blueTeamAgentGroup;
                var loosingGroup = gs.getWinner() == "red" ? blueTeamAgentGroup : redTeamAgentGroup;
                // Debug.Log("5 before");
                // winningGroup.AddGroupReward(WON_BONUS -(float)this.currentSteps / MAX_STEPS);
                // Debug.Log("5 after");
                // Debug.Log("6 before");
                // loosingGroup.AddGroupReward(LOST_BONUS);
                // Debug.Log("6 after");

                this.redTeamAgentGroup.EndGroupEpisode();
                this.blueTeamAgentGroup.EndGroupEpisode();
                this.initialized = false;
                this.currentSteps = 0;
                startNewGame();
            }   
        }
    }

//METHODS -----------------------------------------------------------------------------------------------
    public void startNewGame()
    {
        Debug.Log("STARTING NEW GAME");
        this.initialized = false;
        StopAllCoroutines();

        //If there is anything from the previous game still on the scene, destroy it
        // if (requiredObjects.Count > 0){
        //     for (int i = 0; i < requiredObjects.Count; i++){
        //         Destroy(requiredObjects[i]); 
        //     }
        //     requiredObjects.Clear();
        // }

        //Everything needed for a new game
        // GameObject hardpoints = Instantiate(this.hardpoints);
        // hardpoints.transform.parent = this.gameObject.transform;
        
        // GameObject redTeam = Instantiate(this.machineRedTeam);
        // redTeam.transform.parent = this.gameObject.transform;
        
        // GameObject blueTeam = Instantiate(this.machineBlueTeam);
        // blueTeam.transform.parent = this.gameObject.transform;

        // GameObject gameStatus = Instantiate(this.gameStatus);
        // gameStatus.transform.parent = this.gameObject.transform;
        // this.gs = gameStatus.GetComponent<GameStatus>();

        // foreach (Transform child in redTeam.transform){
        //     this.redTeamPlayers.Add(child.GetComponent<PlayerStatus>());
        // }
        
        // foreach (Transform child in blueTeam.transform){
        //     this.blueTeamPlayers.Add(child.GetComponent<PlayerStatus>());
        // }
        
        // foreach (Transform child in hardpoints.transform){
        //     this.hardpointsList.Add(child.GetComponent<HardpointController>());
        // }
        
        // requiredObjects.Add(hardpoints);
        // requiredObjects.Add(redTeam);
        // requiredObjects.Add(blueTeam);
        // requiredObjects.Add(gameStatus); 

        foreach (Transform child in this.transform)
        {
            if (child.tag == "Bullet")
                Destroy(child.gameObject);
        } 
        
        foreach(var h in this.hardpointsList){
            h.reset();
        }

        foreach (var r in this.redTeamPlayers){
            r.GetComponent<MachineAiPlayerController>().reset();
        }

        foreach (var b in this.blueTeamPlayers){
            b.GetComponent<MachineAiPlayerController>().reset();
        }

        // //Initialize the agents
        // for (int i = 0; i < redTeamPlayers.Count; i++)
        // {
        //     redTeamPlayers[i].reset();
        //     redTeamPlayers[i].gameObject.GetComponent<MachineAiPlayerController>().reset(); //Also resets gs
        //     // r.Initialize();
        //     // r.reset();
        //     // r.behaviorParameters.TeamId = 0;
        //     // redTeamAgentGroup.UnregisterAgent(r);
        //     // redTeamAgentGroup.RegisterAgent(r);
        //     // redTeamPlayers[i].initalStartLocation(0);   
        // }

        // for (int j = 0; j < blueTeamPlayers.Count; j++)
        // {
        //     blueTeamPlayers[j].reset();
        //     blueTeamPlayers[j].gameObject.GetComponent<MachineAiPlayerController>().reset();
        //     // b.Initialize();
        //     // b.reset();
        //     // b.behaviorParameters.TeamId = 1;
        //     // blueTeamAgentGroup.UnregisterAgent(b);
        //     // blueTeamAgentGroup.RegisterAgent(b);
        //     // blueTeamPlayers[j].initalStartLocation(0);  
        // }
        this.initialized = true;
    }

//TRAINING METHODS -----------------------------------------------------------------------------------
    public void reachedTarget(string playerTeam){
        Debug.Log("ENTERED FUNCTION");
        var teamToReward = playerTeam == "red" ? this.redTeamAgentGroup : this.blueTeamAgentGroup;
        var teamThatLost = playerTeam == "red" ? this.blueTeamAgentGroup : this.redTeamAgentGroup;
        Debug.Log("W: " + teamToReward + " L: " + teamThatLost);
        teamToReward.AddGroupReward(10);
        teamThatLost.AddGroupReward(-10);
        this.redTeamAgentGroup.EndGroupEpisode();
        this.blueTeamAgentGroup.EndGroupEpisode();
        startNewGame();
    }
}
