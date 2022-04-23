using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class GameStatusT : MonoBehaviour
{
    //Fields ----------------------------------------------------------------------------------------------------------------------------------------------
    [Header("RedTeamVariables")]
    public List<GameObject> redTeam = new List<GameObject>();
    public float redTeamScore;
    public float redTeamCaptureScore;
    public float redTeamDefendScore;
    public float redTeamKillScore;
    public string redTeamAiType;       //state, machine, mixed

    [Header("BlueTeamVariables")]
    public List<GameObject> blueTeam = new List<GameObject>();
    public float blueTeamScore;
    public float blueTeamCaptureScore;
    public float blueTeamDefendScore;
    public float blueTeamKillScore; 
    public string blueTeamAiType;      //state, machine, mixed

    [Header("GameVariables")]
    public bool gameOver;
    public bool init;
    public float scoreToWin = 100f;
    public string winner;
    public string winnerAiType;
    public float capturePoints = 15f;
    public float defendPoints = 5f;

    [Header("OtherControllers")]
    public TrainingGameController tc;
    
    [Header("UI")]
    public GameObject scoreUI;

    [Header("TextFileVariables")]
    private string matchStartDateTime;
    private string matchLogFileName;
    private string matchReportFileName;
    private string matchReportCSVFileName;
    private string totalDetailedReportFileName;
    private string totalSimpleReportFileName;
    private string matchLogPath;
    private string matchReportPath;
    private string matchReportCSVPath;
    private string totalDetailedReportPath;
    private string totalSimpleReportPath;
    private bool matchReportWritten;

    // Getter Methods --------------------------------------------------------------------------------------
    public float getCapturePoints(){ return this.capturePoints; }
    public float getDefendPoints(){ return this.defendPoints; }

    public float getRedTeamScore(){ return this.redTeamScore; }
    public float getRedTeamCaptureScore(){ return this.redTeamCaptureScore; }
    public float getRedTeamDefendScore(){ return this.redTeamDefendScore; }
    public float getRedTeamKillScore(){ return this.redTeamKillScore; }
    public string getRedTeamAiType(){ return this.redTeamAiType; }
    
    public float getBlueTeamScore(){ return this.blueTeamScore; }
    public float getBlueTeamCaptureScore(){ return this.blueTeamCaptureScore; }
    public float getBlueTeamDefendScore(){ return this.blueTeamDefendScore; }
    public float getBlueTeamKillScore(){ return this.blueTeamKillScore; }
    public string getBlueTeamAiType(){ return this.blueTeamAiType; }

    public string getWinner(){ 
        if (this.winner != null){
            return this.winner;
        }
        else {
            return "Game is still in play";
        }
    }
    public bool getGameOver(){ return this.gameOver; }
    
    //Lifecycle Methods ----------------------------------------------------------------------------------------------------------------------------------------------

    public void Start()
    {
        getRedTeam();
        getBlueTeam();

        this.init = false;
        this.scoreToWin = 100f;
        this.gameOver = false;
        this.redTeamScore = 0f;
        this.blueTeamScore = 0f;
        this.winner = null;

        this.matchStartDateTime = System.DateTime.Now.ToString("ddMMyyyy-HHmmss");
        this.matchReportWritten = false;
        createTextFiles();
        writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {"Match Started"});
    }

    public void Update()
    {   
        if (!this.init){
            initalStartLocation();
        }
        if (isGameOver() && this.gameOver == false){
            Debug.Log("WINNER: " + this.winner);
            this.gameOver = true;
            if (!this.matchReportWritten){
                writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {"Match Ended", ("Winner: " + this.winner)});
                writeMatchReport();
            }
        }

        updateScores();
    }

    public void reset()
    {
        this.init = false;
        this.scoreToWin = 100f;
        this.gameOver = false;
        this.redTeamScore = 0f;
        this.blueTeamScore = 0f;
        this.winner = null;
        this.winner = "";
        this.winnerAiType = "";

        this.blueTeamScore = 0f;
        this.blueTeamCaptureScore = 0f;
        this.blueTeamDefendScore = 0f;
        this.blueTeamKillScore = 0f; 

        this.redTeamScore = 0f;
        this.redTeamCaptureScore = 0f;
        this.redTeamDefendScore = 0f;
        this.redTeamKillScore = 0f;

        this.matchStartDateTime = System.DateTime.Now.ToString("ddMMyyyy-HHmmss");
        this.matchReportWritten = false;
        createTextFiles();
        writeMatchLogRecord(System.DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"), new string[] {"Match Started"});

        updateScores();
        initalStartLocation();
    }

    //Methods ----------------------------------------------------------------------------------------------------------------------------------------------
    
    private void getRedTeam(){
        string aiTypeTemp = "";
        foreach (Transform child in this.tc.transform)
        {
            foreach (Transform grandchild in child){
                if (grandchild.tag == "RedPlayer"){
                    this.redTeam.Add(grandchild.gameObject);
                    if (aiTypeTemp == ""){
                        aiTypeTemp = grandchild.GetComponent<PlayerStatusT>().aiType;
                    }
                    else if (aiTypeTemp == grandchild.GetComponent<PlayerStatusT>().aiType){
                        aiTypeTemp = grandchild.GetComponent<PlayerStatusT>().aiType;
                    }
                    else {
                        aiTypeTemp = "mixed";
                    }
                }   
            }
            this.redTeamAiType = aiTypeTemp;
        }
    }

    private void getBlueTeam(){
        string aiTypeTemp = "";
        foreach (Transform child in this.tc.transform)
        {
            foreach (Transform grandchild in child){
                if (grandchild.tag == "BluePlayer"){
                    this.blueTeam.Add(grandchild.gameObject);
                    if (aiTypeTemp == ""){
                        aiTypeTemp = grandchild.GetComponent<PlayerStatus>().aiType;
                    }
                    else if (aiTypeTemp == grandchild.GetComponent<PlayerStatus>().aiType){
                        aiTypeTemp = grandchild.GetComponent<PlayerStatus>().aiType;
                    }
                    else {
                        aiTypeTemp = "mixed";
                    }
                }   
            }
            this.blueTeamAiType = aiTypeTemp;
        }
    }

    private void initalStartLocation(){
        Debug.Log("ENTERED LOCATION THING");
        this.init = true;
        //Sets the player to a respawn point
        //Done this way so that there doesn't end up being 2 players on one respawn point
        //Maps player in list to respawn point in list by index value
        //aka Player[0] maps to Respawn[0] and so on
        for (int i = 0; i < this.redTeam.Count; i++){
            this.redTeam[i].GetComponent<PlayerStatusT>().initalStartLocation(i);
        }
        for (int j = 0; j < this.blueTeam.Count; j++){
            this.blueTeam[j].GetComponent<PlayerStatusT>().initalStartLocation(j);
        }
        
    }

    public void addToKillScores(string teamThatDied){
        //Takes in team of player that died
        if (teamThatDied == "red"){
            this.blueTeamKillScore += 1;
        }
        else if (teamThatDied == "blue"){
            this.redTeamKillScore += 1;
        }
    }
    
    private bool isGameOver(){
        if (this.blueTeamScore >= this.scoreToWin){
            this.winner = "blue";
            this.winnerAiType = this.blueTeamAiType;
            return true;
        }
        else if (this.redTeamScore >= this.scoreToWin){
            this.winner = "red";
            this.winnerAiType = this.redTeamAiType;
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
            PlayerStatusT r = redTeam[i].GetComponent<PlayerStatusT>();
            captureScore += r.getCaptureScore();
            defendScore += r.getDefendScore();
            killScore += r.getKillScore();
        }
        this.redTeamCaptureScore = captureScore;
        this.redTeamDefendScore = defendScore;
        // this.redTeamKillScore = killScore;
        this.redTeamScore = this.redTeamCaptureScore + this.redTeamDefendScore;

        //Reset
        captureScore = 0f;
        defendScore = 0f;
        killScore = 0f;

        //Blue Team
        for (int i = 0; i < blueTeam.Count; i++){
            PlayerStatusT b = blueTeam[i].GetComponent<PlayerStatusT>();
            captureScore += b.getCaptureScore();
            defendScore += b.getDefendScore();
            killScore += b.getKillScore();
        }
        this.blueTeamCaptureScore = captureScore;
        this.blueTeamDefendScore = defendScore;
        // this.blueTeamKillScore = killScore;
        this.blueTeamScore = this.blueTeamCaptureScore + this.blueTeamDefendScore;
        
        //updateScoreUI();
    }

    private void updateScoreUI(){
        Transform r = this.scoreUI.transform.Find("redTeam");
        Transform b = this.scoreUI.transform.Find("blueTeam");

        r.transform.Find("redTotalScore").gameObject.GetComponent<Text>().text = this.redTeamScore.ToString();
        b.transform.Find("blueTotalScore").gameObject.GetComponent<Text>().text = this.blueTeamScore.ToString();

        r.transform.Find("redAiType").gameObject.GetComponent<Text>().text = ("AI Type: " + this.redTeamAiType);
        b.transform.Find("blueAiType").gameObject.GetComponent<Text>().text = ("AI Type: " + this.blueTeamAiType);

        r.transform.Find("redCaptureScore").gameObject.GetComponent<Text>().text = ("Capture Score: " + this.redTeamCaptureScore.ToString());
        b.transform.Find("blueCaptureScore").gameObject.GetComponent<Text>().text = ("Capture Score: " + this.blueTeamCaptureScore.ToString());

        r.transform.Find("redDefendScore").gameObject.GetComponent<Text>().text = ("Defend Score: " + this.redTeamDefendScore.ToString());
        b.transform.Find("blueDefendScore").gameObject.GetComponent<Text>().text = ("Defend Score: " + this.blueTeamDefendScore.ToString());

        r.transform.Find("redKillScore").gameObject.GetComponent<Text>().text = ("Total Kills: " + this.redTeamKillScore.ToString());
        b.transform.Find("blueKillScore").gameObject.GetComponent<Text>().text = ("Total Kills: " + this.blueTeamKillScore.ToString());
    }

    public void createTextFiles()
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
        this.totalDetailedReportFileName = ("Cumulative-Match-Reports-Detailed");
        this.totalSimpleReportFileName = ("Cumulative-Match-Reports-Simple");
        this.matchReportPath = (Application.dataPath + "/GameLogs/MatchReports/" + this.matchReportFileName + ".txt");
        this.matchReportCSVPath = (Application.dataPath + "/GameLogs/MatchReports/" + this.matchReportCSVFileName + ".csv");
        this.totalDetailedReportPath = (Application.dataPath + "/GameLogs/" + this.totalDetailedReportFileName + ".csv");
        this.totalSimpleReportPath = (Application.dataPath + "/GameLogs/" + this.totalSimpleReportFileName + ".csv");

        if(!File.Exists(this.matchReportCSVPath)){
            string content = "TEAM,AI TYPE,NAME,TOTAL,CAPTURES,DEFENDS,KILLS,DATE/TIME" + "\n==============================================" + "\n";
            File.WriteAllText(this.matchReportCSVPath, content);
        }
        
        if(!File.Exists(this.matchReportPath)){
            string content = "MATCH REPORT: " + dateTimeFormat + "\n" + "---------------------------------";
            File.WriteAllText(this.matchReportPath, content + "\n\n");
        }

        if(!File.Exists(this.totalDetailedReportPath)){
            string content = "TEAM,AI TYPE,NAME,TOTAL,CAPTURES,DEFENDS,KILLS,DATE/TIME" + "\n==============================================" + "\n";
            File.WriteAllText(this.totalDetailedReportPath, content);
        }

        if(!File.Exists(this.totalSimpleReportPath)){
            string content = "TEAM,AI TYPE,TOTAL,CAPTURES,DEFENDS,KILLS,WON,DATE/TIME" + "\n==============================================" + "\n";
            File.WriteAllText(this.totalSimpleReportPath, content);
        }

        //Match Log
        this.matchLogFileName = (this.matchStartDateTime + "-ML-Formatted");
        this.matchLogPath = (Application.dataPath + "/GameLogs/MatchLogs/" + this.matchLogFileName + ".txt");

        if(!File.Exists(this.matchLogPath)){
            string content = "MATCH LOG: " + dateTimeFormat + "\n" + "-----------------------------------";
            File.WriteAllText(this.matchLogPath, content + "\n");
        }
    }

    public async void writeMatchReport()
    {   
        if(File.Exists(this.matchReportPath)){
            string csvContent = "";           
            
            string totalReportContent = "";
            totalReportContent = totalReportContent + "red," + this.redTeamAiType + "," + this.redTeamScore + "," + this.redTeamCaptureScore + "," + this.redTeamDefendScore + "," + this.redTeamKillScore + ",";
            if(this.winner == "red"){
                totalReportContent = totalReportContent + "yes,";
            }
            else {
                totalReportContent = totalReportContent + "no,";
            } 
            totalReportContent = totalReportContent + this.matchStartDateTime + "\n";
            totalReportContent = totalReportContent + "blue," + this.blueTeamAiType + "," + this.blueTeamScore + "," + this.blueTeamCaptureScore + "," + this.blueTeamDefendScore + "," + this.blueTeamKillScore + ",";
            if(this.winner == "blue"){
                totalReportContent = totalReportContent + "yes,";
            }
            else {
                totalReportContent = totalReportContent + "no,";
            } 
            totalReportContent = totalReportContent + this.matchStartDateTime + "\n";
            
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
                PlayerStatusT r = redTeam[i].GetComponent<PlayerStatusT>();
                content = content + 
                    String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-10}|", r.team, r.aiType, r.name, r.getTotalScore(), r.getCaptureScore(), r.getDefendScore(), r.getKillScore()) + "\n" +
                    string.Format("{0,-56}","------------------------------------------------------------------------------") + "\n";
                csvContent = csvContent + (r.team + "," + r.aiType + "," + r.name + "," + r.getTotalScore() + "," + r.getCaptureScore() + "," + r.getDefendScore() + "," + r.getKillScore() + "," + this.matchStartDateTime + "\n");
            }                

            content = content + "\nBLUE TEAM OVERVIEW" + "\n" +
                string.Format("{0,-56}","==============================================================================") + "\n" +
                String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-10}|", "TEAM", "AI TYPE", "NAME", "TOTAL", "CAPTURES", "DEFENDS", "KILLS") + "\n" +
                string.Format("{0,-56}","==============================================================================") + "\n";
                
            for (int i = 0; i < blueTeam.Count; i++){
                PlayerStatusT b = blueTeam[i].GetComponent<PlayerStatusT>();
                content = content + 
                    String.Format("|{0,-10}|{1,-10}|{2,-10}|{3,-10}|{4,-10}|{5,-10}|{6,-10}|", b.team, b.aiType, b.name, b.getTotalScore(), b.getCaptureScore(), b.getDefendScore(), b.getKillScore()) + "\n" +
                    string.Format("{0,-56}","------------------------------------------------------------------------------") + "\n";
                csvContent = csvContent + (b.team + "," + b.aiType + "," + b.name + "," + b.getTotalScore() + "," + b.getCaptureScore() + "," + b.getDefendScore() + "," + b.getKillScore() + "," + this.matchStartDateTime + "\n");
            }            

            File.AppendAllText(this.matchReportPath, content);
            File.AppendAllText(this.matchReportCSVPath, csvContent);
            File.AppendAllText(this.totalSimpleReportPath, totalReportContent);
            File.AppendAllText(this.totalDetailedReportPath, csvContent);
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
