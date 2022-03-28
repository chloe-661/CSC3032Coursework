using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameInitialiser : MonoBehaviour
{

//SettingsUI

    private GameObject settingsUI;
    public Button startButton;
    public Text numGamesTxt;

//Prefabs

    public GameObject gameUI;
    public GameObject gameStatus;
    public GameObject stateRedTeam;
    public GameObject stateBlueTeam;
    public GameObject simpleHardpoints;
    // public GameObject machineRedTeam;
    // public GameObject machineBlueTeam;


//For the new instances created

    private List<GameObject> requiredObjects = new List<GameObject>();
    private GameStatus gs;

    private string redTeamAIType;
    private string blueTeamAIType;
    private int numGames;
    private int gamesPlayed;

    private bool settingsDone;
    private bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        //Default game...
        this.redTeamAIType = "state";
        this.blueTeamAIType = "state";
        this.numGames = 1;
        this.startButton.interactable = false;
        this.gamesPlayed = 0;
        this.settingsUI = GameObject.FindWithTag("SettingsUI");
        this.gameOver = false;
    }

    // Update is called once per frame
    async void Update()
    {
        if (this.settingsDone){
            this.settingsUI.SetActive(false);

            if (gs.getGameOver() == true && this.gameOver == false){
                this.gameOver = true;
                for (int i = 0; i < requiredObjects.Count; i++){
                    Destroy(requiredObjects[i]); 
                }
                requiredObjects.Clear();
                Debug.Log("destroyed the objects");
                this.gamesPlayed++;

                if (this.gamesPlayed < this.numGames){
                    Debug.Log("Starting new game");
                    StartCoroutine(newGameWaiter());
                }
                else if (this.gamesPlayed == this.numGames){
                    Debug.Log("Finished playing all the games it was supposed to");
                    this.settingsDone = false;
                    this.gamesPlayed = 0;
                }
            }
        }
        else {
            this.settingsUI.SetActive(true);
        }
    }

    public void handleRedTeamInputData(int val){
        switch(val){
            case 0:
                this.redTeamAIType = "state";
                break;
            case 1:
                this.redTeamAIType = "machine";
                break;
            case 2:
                this.redTeamAIType = "mixed";
                break;
            default:
                this.redTeamAIType = "state";
                break;
        }
    }

    public void handleBlueTeamInputData(int val){
                switch(val){
            case 0:
                this.blueTeamAIType = "state";
                break;
            case 1:
                this.blueTeamAIType = "machine";
                break;
            case 2:
                this.blueTeamAIType = "mixed";
                break;
            default:
                this.blueTeamAIType = "state";
                break;
        }
    }

    public void handleNumGamesInputData (){
        string val = numGamesTxt.text;
        Debug.Log(val);
        bool isNumber = int.TryParse(val, out this.numGames);

        if (!isNumber) {
            this.startButton.interactable = false;
        }
        if (this.numGames > 0){
            this.startButton.interactable = true;
        }
    }

    IEnumerator newGameWaiter(){
        yield return new WaitForSeconds(3);
        startGame();
    }

    public void startGame(){
        this.settingsDone = true;
        


        switch(this.redTeamAIType){
            case "state":
                requiredObjects.Add(Instantiate(this.stateRedTeam));
                break;
            // case "machine":
            //     GameObject obj = Instantiate(this.machineRedTeam);
            //     requiredObjects.Add(obj);
            //     break;
            // case "mixed":

            //     break;
            default: //state
                requiredObjects.Add(Instantiate(this.stateRedTeam));
                break;
        }

        switch(this.blueTeamAIType){
            case "state":
                requiredObjects.Add(Instantiate(this.stateBlueTeam));
                break;
            // case "machine":
            //     requiredObjects.Add(Instantiate(this.stateBlueTeam));
            //     break;
            // case "mixed":

            //     break;
            default: //state
                requiredObjects.Add(Instantiate(this.stateBlueTeam));
                break;
        }

        requiredObjects.Add(Instantiate(gameStatus));
        requiredObjects.Add(Instantiate(gameUI));
        requiredObjects.Add(Instantiate(simpleHardpoints));

        this.gs = GameObject.FindWithTag("GameStatus").GetComponent<GameStatus>();
        this.gameOver = false;
    }
}
