using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class GameStatus : MonoBehaviour
{
    //Red Team Variables
    private List<GameObject> redTeam = new List<GameObject>();
    private float redTeamScore;
    private float redTeamCaptureScore;
    private float redTeamDefendScore;
    private float redTeamKillScore;
    private string redTeamAiType;

    //Blue Team Variables
    private List<GameObject> blueTeam = new List<GameObject>();
    private float blueTeamScore;
    private float blueTeamCaptureScore;
    private float blueTeamDefendScore;
    private float blueTeamKillScore;
    private string blueTeamAiType;

    //Game Variables
    private bool gameOver;
    private float scoreToWin = 100f;
    private string winner;
    private string winnerAiType;
    private float capturePoints = 15f;
    private float defendPoints = 5f;

    //UI Variables
    private GameObject scoreUI;

    //Text File Variables

    private string matchStartDateTime;
    private string matchLogFileName;
    private string matchReportFileName;
    private string matchReportCSVFileName;
    private string matchLogPath;
    private string matchReportPath;
    private string matchReportCSVPath;

    private bool matchReportWritten;
    



// Getter Methods --------------------------------------------------------------------------------------
    public float getCapturePoints(){ return this.capturePoints; }
    public float getDefendPoints(){ return this.defendPoints; }

    public float getRedTeamScore(){ return this.redTeamScore; }
    public float getRedTeamCaptureScore(){ return this.redTeamCaptureScore; }
    public float getRedTeamDefendScore(){ return this.redTeamDefendScore; }
    public float getRedTeamKillScore(){ return this.redTeamKillScore; }
    
    public float getBlueTeamScore(){ return this.blueTeamScore; }
    public float getBlueTeamCaptureScore(){ return this.blueTeamCaptureScore; }
    public float getBlueTeamDefendScore(){ return this.blueTeamDefendScore; }
    public float getBlueTeamKillScore(){ return this.blueTeamKillScore; }

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
        getRedTeam();
        getBlueTeam();
        this.scoreToWin = 100f;
        this.gameOver = false;
        this.redTeamScore = 0f;
        this.blueTeamScore = 0f;
        this.winner = null;
        this.scoreUI = GameObject.FindWithTag("Scores");

        this.matchStartDateTime = System.DateTime.Now.ToString("ddMMyyyy-HHmmss");
        this.matchReportWritten = false;
        createTextFiles();
        writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {"Match Started"});
    }
    void Update()
    {
        if (isGameOver()){
            Debug.Log("Winner: " + this.winner);
            if (!this.matchReportWritten){
                writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {"Match Ended", ("Winner: " + this.winner)});
                writeMatchReport();
            }
        }

        updateScores();
    }



// Methods --------------------------------------------------------------------------------------------

    private void getRedTeam(){
        string aiTypeTemp = "";
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("RedPlayer")) {
            this.redTeam.Add(obj);
            if (aiTypeTemp == ""){
                aiTypeTemp = obj.GetComponent<PlayerStatus>().aiType;
            }
            else if (aiTypeTemp == obj.GetComponent<PlayerStatus>().aiType){
                aiTypeTemp = obj.GetComponent<PlayerStatus>().aiType;
            }
            else {
                aiTypeTemp = "mixed";
            }
        }
        this.redTeamAiType = aiTypeTemp;
    }

    private void getBlueTeam(){
        string aiTypeTemp = "";
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("BluePlayer")) {
            this.blueTeam.Add(obj);
            if (aiTypeTemp == ""){
                aiTypeTemp = obj.GetComponent<PlayerStatus>().aiType;
            }
            else if (aiTypeTemp == obj.GetComponent<PlayerStatus>().aiType){
                aiTypeTemp = obj.GetComponent<PlayerStatus>().aiType;
            }
            else {
                aiTypeTemp = "mixed";
            }
        }
        this.blueTeamAiType = aiTypeTemp;
    }
    
    private bool isGameOver(){
        if (this.blueTeamScore >= this.scoreToWin){
            this.winner = "blue";
            this.winnerAiType = this.blueTeamAiType;
            this.gameOver = true;
            return true;
        }
        else if (this.redTeamScore >= this.scoreToWin){
            this.winner = "red";
            this.winnerAiType = this.redTeamAiType;
            this.gameOver = true;
            return true;
        }
        else {
            this.gameOver = false;
            return false;
        }
    }

    private async void updateScores()
    {
        //Reset
        float captureScore = 0f;
        float defendScore = 0f;
        float killScore = 0f;

        //Red Team
        for (int i = 0; i < redTeam.Count; i++){
            PlayerStatus r = redTeam[i].GetComponent<PlayerStatus>();
            captureScore += r.getCaptureScore();
            defendScore += r.getDefendScore();
            killScore += r.getKillScore();
        }
        this.redTeamCaptureScore = captureScore;
        this.redTeamDefendScore = defendScore;
        this.redTeamKillScore = killScore;
        this.redTeamScore = this.redTeamCaptureScore + this.redTeamDefendScore;

        //Reset
        captureScore = 0f;
        defendScore = 0f;
        killScore = 0f;

        //Blue Team
        for (int i = 0; i < blueTeam.Count; i++){
            PlayerStatus b = blueTeam[i].GetComponent<PlayerStatus>();
            captureScore += b.getCaptureScore();
            defendScore += b.getDefendScore();
            killScore += b.getKillScore();
        }
        this.blueTeamCaptureScore = captureScore;
        this.blueTeamDefendScore = defendScore;
        this.blueTeamKillScore = killScore;
        this.blueTeamScore = this.blueTeamCaptureScore + this.blueTeamDefendScore;
        
        updateScoreUI();
    }

    private void updateScoreUI(){
        Transform r = this.scoreUI.transform.Find("redTeam");
        Transform b = this.scoreUI.transform.Find("blueTeam");

        r.transform.Find("redTotalScore").gameObject.GetComponent<Text>().text = this.redTeamScore.ToString();
        b.transform.Find("blueTotalScore").gameObject.GetComponent<Text>().text = this.blueTeamScore.ToString();

        r.transform.Find("redCaptureScore").gameObject.GetComponent<Text>().text = ("Capture Score: " + this.redTeamCaptureScore.ToString());
        b.transform.Find("blueCaptureScore").gameObject.GetComponent<Text>().text = ("Capture Score: " + this.blueTeamCaptureScore.ToString());

        r.transform.Find("redDefendScore").gameObject.GetComponent<Text>().text = ("Defend Score: " + this.redTeamDefendScore.ToString());
        b.transform.Find("blueDefendScore").gameObject.GetComponent<Text>().text = ("Defend Score: " + this.blueTeamDefendScore.ToString());

        r.transform.Find("redKillScore").gameObject.GetComponent<Text>().text = ("Total Kills: " + this.redTeamKillScore.ToString());
        b.transform.Find("blueKillScore").gameObject.GetComponent<Text>().text = ("Total Kills: " + this.blueTeamKillScore.ToString());
    }

    private void createTextFiles()
    {
        string dateTimeFormat = 
            this.matchStartDateTime.Substring(0, 2)  + "/" +
            this.matchStartDateTime.Substring(2, 2)  + "/" +
            this.matchStartDateTime.Substring(4, 4)  + " " +
            this.matchStartDateTime.Substring(9, 2)  + ":" +
            this.matchStartDateTime.Substring(11, 2)  + ":" +
            this.matchStartDateTime.Substring(13, 2);

        //Match Report
        this.matchReportFileName = (this.matchStartDateTime + "-MR-Formatted");
        this.matchReportCSVFileName = (this.matchStartDateTime + "-MR");
        this.matchReportPath = (Application.dataPath + "/GameLogs/MatchReports/" + this.matchReportFileName + ".txt");
        this.matchReportCSVPath = (Application.dataPath + "/GameLogs/MatchReports/" + this.matchReportCSVFileName + ".csv");

        if(!File.Exists(this.matchReportCSVPath)){
            string content = "TEAM,AI TYPE,NAME,TOTAL,CAPTURES,DEFENDS,KILLS" + "\n==============================================" + "\n";
            File.WriteAllText(this.matchReportCSVPath, content);
        }
        
        if(!File.Exists(this.matchReportPath)){
            string content = "MATCH REPORT: " + dateTimeFormat + "\n" + "---------------------------------";
            File.WriteAllText(this.matchReportPath, content + "\n\n");
        }

        //Match Log
        this.matchLogFileName = (this.matchStartDateTime + "-ML-Formatted");
        this.matchLogPath = (Application.dataPath + "/GameLogs/MatchLogs/" + this.matchLogFileName + ".txt");

        if(!File.Exists(this.matchLogPath)){
            string content = "MATCH LOG: " + dateTimeFormat + "\n" + "-----------------------------------";
            File.WriteAllText(this.matchLogPath, content + "\n");
        }
    }

    private async void writeMatchReport()
    {   
        if(File.Exists(this.matchReportPath)){
            string csvContent = "";
            string content = 
                "WINNER" + "\n" +
                string.Format("{0,-56}","=======================") + "\n" +
                String.Format("|{0,-10}|{1,-10}|", "TEAM", "AI TYPE") + "\n" +
                string.Format("{0,-56}","=======================") + "\n" +
                String.Format("|{0,-10}|{1,-10}|", this.winner, this.winnerAiType) + "\n" +
                string.Format("{0,-56}","-----------------------") + "\n\n" +
                
                "OVERVIEW" + "\n" +
                string.Format("{0,-56}","========================================================") + "\n" +
                String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|", "TEAM", "TOTAL", "CAPTURES", "DEFENDS", "KILLS") + "\n" +
                string.Format("{0,-56}","========================================================") + "\n" +
                String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|", "red", this.redTeamScore, this.redTeamCaptureScore, this.redTeamDefendScore, this.redTeamKillScore) + "\n" +
                string.Format("{0,-56}","--------------------------------------------------------") + "\n" +
                String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|", "blue", this.blueTeamScore, this.blueTeamCaptureScore, this.blueTeamDefendScore, this.blueTeamKillScore) + "\n" +
                string.Format("{0,-56}","--------------------------------------------------------") + "\n\n" +
                
                "RED TEAM OVERVIEW" + "\n" +
                string.Format("{0,-56}","==============================================================================") + "\n" +
                String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-10}|", "TEAM", "AI TYPE", "NAME", "TOTAL", "CAPTURES", "DEFENDS", "KILLS") + "\n" +
                string.Format("{0,-56}","==============================================================================") + "\n";
                
            for (int i = 0; i < redTeam.Count; i++){
                PlayerStatus r = redTeam[i].GetComponent<PlayerStatus>();
                content = content + 
                    String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-10}|", r.team, r.aiType, r.name, r.getTotalScore(), r.getCaptureScore(), r.getDefendScore(), r.getKillScore()) + "\n" +
                    string.Format("{0,-56}","------------------------------------------------------------------------------") + "\n";
                csvContent = csvContent + (r.team + "," + r.aiType + "," + r.name + "," + r.getTotalScore() + "," + r.getCaptureScore() + "," + r.getDefendScore() + "," + r.getKillScore() + "\n");
            }                

            content = content + "\nBLUE TEAM OVERVIEW" + "\n" +
                string.Format("{0,-56}","==============================================================================") + "\n" +
                String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-10}|", "TEAM", "AI TYPE", "NAME", "TOTAL", "CAPTURES", "DEFENDS", "KILLS") + "\n" +
                string.Format("{0,-56}","==============================================================================") + "\n";
                
            for (int i = 0; i < blueTeam.Count; i++){
                PlayerStatus b = blueTeam[i].GetComponent<PlayerStatus>();
                content = content + 
                    String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-10}|", b.team, b.aiType, b.name, b.getTotalScore(), b.getCaptureScore(), b.getDefendScore(), b.getKillScore()) + "\n" +
                    string.Format("{0,-56}","------------------------------------------------------------------------------") + "\n";
                csvContent = csvContent + (b.team + "," + b.aiType + "," + b.name + "," + b.getTotalScore() + "," + b.getCaptureScore() + "," + b.getDefendScore() + "," + b.getKillScore() + "\n");
            }            

            File.AppendAllText(this.matchReportPath, content);
            File.AppendAllText(this.matchReportCSVPath, csvContent);
            this.matchReportWritten = true;
        }
    }

    public async void writeMatchLogRecord(string dateTime, string[] message)
    {
        if(File.Exists(this.matchLogPath))
        {
            string content = "";
            for (int i = 0; i < message.Length; i++){
                content =  content + (dateTime + " | " + message[i] + "\n");
            }
            content = content + "------\n";
            File.AppendAllText(this.matchLogPath, content);
        }
    }
}
