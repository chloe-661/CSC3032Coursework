using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject[] levelCameras;
    public GameObject[] redTeamTPCameras;
    public GameObject[] redTeamFPCameras;
    public GameObject[] blueTeamTPCameras;
    public GameObject[] blueTeamFPCameras;

    public int currentLevelCamera = 0;
    public int currentRedTeamTPCamera = 0;
    public int currentRedTeamFPCamera = 0;
    public int currentBlueTeamTPCamera = 0;
    public int currentBlueTeamFPCamera = 0;

    // Start is called before the first frame update
    void Start()
    {
        levelCameras = GameObject.FindGameObjectsWithTag("LevelCamera");

        //Set all but the camera with the deapest depth to unactive
        for (int i = 0; i < levelCameras.Length; i++) 
        {
            Camera c = levelCameras[i].GetComponent<Camera>();
            if (c.depth > 0) {
                c.enabled = true;
                currentLevelCamera = i;
            }
            else {
                c.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            print("1 was pressed");
            levelCameras[currentLevelCamera].GetComponent<Camera>().enabled = false;
            if (currentLevelCamera == levelCameras.Length - 1){
                currentLevelCamera = 0;
            }
            else {
                currentLevelCamera ++;
            }
            levelCameras[currentLevelCamera].GetComponent<Camera>().enabled = true;
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            print("2 was pressed");
            redTeamTPCameras[currentRedTeamTPCamera].GetComponent<Camera>().enabled = false;
            if (currentRedTeamTPCamera == redTeamTPCameras.Length - 1){
                currentRedTeamTPCamera = 0;
            }
            else {
                currentRedTeamTPCamera ++;
            }
            redTeamTPCameras[currentRedTeamTPCamera].GetComponent<Camera>().enabled = true;
        }
    }   
}
