﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchForHardpoint : IState
{
    private StateAiPlayerController player;

    public SearchForHardpoint(StateAiPlayerController player)
    {
        this.player = player;
    }

    public void Tick()
    {
        this.player.Target = chooseHardpoint();
        this.player.previousRunTimeCaptures = this.player.Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeCaptures();
        this.player.previousRunTimeDefends = this.player.Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeDefends();
    }
    private float generateRandom(){
        float rndNum = Random.Range(0f, 1f);
        return rndNum;
    }
    private GameObject chooseHardpoint()
    {      
        List<GameObject> orderedHardpoints = (GameObject.FindGameObjectsWithTag("Hardpoint") 
             .OrderBy(t=> Vector3.Distance(this.player.transform.position, t.transform.position))
             .ToList());

        //If the nearest hardpoint has not been captured by your team
        if (orderedHardpoints[0].GetComponent<HardpointController>().getOwner() != this.player.ps.team){
            //Probabilities:
            //Capture Nearest 95%
            //Capture Other 2.5%
            //Capture Other 2.5%

            var options = new List<KeyValuePair<GameObject, float>>() 
            { 
                new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.025f),
                new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.025f),
                new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.95f),
            };
            GameObject temp = makeDecision(options);
            // Debug.Log(this.player.ps.name + "has chosen hardpoint: " + temp.name);
            return temp.transform.GetChild(0).gameObject;
            //return makeDecision(options).transform.GetChild(0).gameObject;
            
        }
        else {
            //If the nearest hardpoint has been captured by your team
            //But the other two are uncaptured
            if (orderedHardpoints[1].GetComponent<HardpointController>().getOwner() != this.player.ps.team 
                && orderedHardpoints[2].GetComponent<HardpointController>().getOwner() != this.player.ps.team){
                // Probabilities:
                // Capture Nearest: 85%
                // Capture Other: 10%
                // Defend Other: 5%
                var options = new List<KeyValuePair<GameObject, float>>() 
                { 
                    new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.05f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.1f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.85f),
                };
                GameObject temp = makeDecision(options);
                // Debug.Log(this.player.ps.name + "has chosen hardpoint: " + temp.name);
                return temp.transform.GetChild(0).gameObject;
            }
            //If the nearest hardpoint has been captured by your team
            //If the second nearest hardpoint has not been captured
            else if (orderedHardpoints[1].GetComponent<HardpointController>().getOwner() != this.player.ps.team 
                    && orderedHardpoints[2].GetComponent<HardpointController>().getOwner() == this.player.ps.team){
                // Probabilities:
                // Capture Remaining: 85%
                // Defend Nearest: 10%
                // Defend Other: 5%
                var options = new List<KeyValuePair<GameObject, float>>() 
                { 
                    new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.05f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.1f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.85f),
                };
                GameObject temp = makeDecision(options);
                // Debug.Log(this.player.ps.name + "has chosen hardpoint: " + temp.name);
                return temp.transform.GetChild(0).gameObject;
            }
            //If the nearest hardpoint has been captured by your team
            //If the second nearest hardpoint has also been captured by your team
            else if (orderedHardpoints[1].GetComponent<HardpointController>().getOwner() == this.player.ps.team 
                    && orderedHardpoints[2].GetComponent<HardpointController>().getOwner() != this.player.ps.team){
                // Probabilities:
                // Capture Remaining: 80%
                // Defend Nearest: 15%
                // Defend Other: 5%
                var options = new List<KeyValuePair<GameObject, float>>() 
                { 
                    new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.05f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.15f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.8f),
                };
                GameObject temp = makeDecision(options);
                // Debug.Log(this.player.ps.name + "has chosen hardpoint: " + temp.name);
                return temp.transform.GetChild(0).gameObject;
            }
            //If all hardpoint have been captured by your team
            else {
                // Probabilities:
                // Defend Nearest: 60%
                // Defend Second Nearest: 30%
                // Defend Other: 10%
                var options = new List<KeyValuePair<GameObject, float>>() 
                { 
                    new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.1f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.3f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.6f),
                };
                GameObject temp = makeDecision(options);
                // Debug.Log(this.player.ps.name + "has chosen hardpoint: " + temp.name);
                return temp.transform.GetChild(0).gameObject;
            }
        }
        return null;
    }
    private GameObject makeDecision(List<KeyValuePair<GameObject, float>> options){
        float rndNum = generateRandom();
            
        float cumulative = 0f;
        for (int i = 0; i < options.Count; i++)
        {
            cumulative += options[i].Value;
            if (rndNum < cumulative)
            {
                return options[i].Key;
            }
        }

        return null;
    }

    public void OnEnter() { }
    public void OnExit() { }
}