using System.Linq;
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
    }
    private float generateRandom(){
        float rndNum = Random.Range(0f, 1f);
        Debug.Log("RandomNum: " + rndNum);
        return rndNum;
    }
    private GameObject chooseHardpoint()
    {

        //IF nearest is uncaptured/captured by enemy - go to that
        //ELSE
        //      IF remaining two are uncaptured/captured by enemy
        //          Go to nearest of remaining two
        //      IF remaining one is uncaptured/captured by enemy
        //          70% capture the remaining one
        //          30% defend one of the others
        //                70% defend the closest
        //                30% defend the other one
        //      IF all are captured
        //          Defend
        //             60% Defend the closest
        //              40% Defend one of the others
        //                  50% Defend closest
        //                  50% Defend furthest away

        //ALL OF THE ABOVE SHOULD/COULD BE CHANGED BASED ON WHAT THE PLAYER SEES
        //- sees another team member in the hardpoint - go do something else
        //- Sees an enemy in one of their captured hardpoints - go attack them
      
        List<GameObject> orderedHardpoints = (GameObject.FindGameObjectsWithTag("Hardpoint") 
             .OrderBy(t=> Vector3.Distance(this.player.transform.position, t.transform.position))
             .ToList());

        Debug.Log("Nearest Hardpoint: " + orderedHardpoints[0].name);

        if (orderedHardpoints[0].GetComponent<HardpointController>().getOwner() != this.player.ps.team){
            //Probabilities:
            //Capture Nearest 80%
            //Capture Other 10%
            //Capture Other 10%

            var options = new List<KeyValuePair<GameObject, float>>() 
            { 
                new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.1f),
                new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.1f),
                new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.8f),
            };
            GameObject temp = makeDecision(options);
            Debug.Log("Chosen Hardpoint: " + temp.name);
            return temp.transform.GetChild(0).gameObject;
            //return makeDecision(options).transform.GetChild(0).gameObject;
            
        }
        else {

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
                Debug.Log("Chosen Hardpoint: " + temp.name);
                return temp.transform.GetChild(0).gameObject;
            }
            else if (orderedHardpoints[1].GetComponent<HardpointController>().getOwner() != this.player.ps.team 
                    && orderedHardpoints[2].GetComponent<HardpointController>().getOwner() == this.player.ps.team){
                // Probabilities:
                // Capture Remaining: 80%
                // Defend Nearest: 15%
                // Defend Other: 5%
                var options = new List<KeyValuePair<GameObject, float>>() 
                { 
                    new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.05f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.15f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.8f),
                };
                GameObject temp = makeDecision(options);
                Debug.Log("Chosen Hardpoint: " + temp.name);
                return temp.transform.GetChild(0).gameObject;
            }
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
                Debug.Log("Chosen Hardpoint: " + temp.name);
                return temp.transform.GetChild(0).gameObject;
            }
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
                Debug.Log("Chosen Hardpoint: " + temp.name);
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