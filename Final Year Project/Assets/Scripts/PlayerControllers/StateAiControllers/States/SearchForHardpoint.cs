using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchForHardpoint : IState
{
    private StateAiPlayerController player;
    private NavMeshAgent navMeshAgent;
    private TrailRenderer trailRenderer;

    public SearchForHardpoint(StateAiPlayerController player, NavMeshAgent navMeshAgent, TrailRenderer trailRenderer)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.trailRenderer = trailRenderer;
    }

    public void OnEnter() { 
        this.navMeshAgent.enabled = true;
        this.trailRenderer.enabled = true;
    }

    public void Tick()
    {
        this.player.Target = chooseHardpoint();
        this.player.previousRunTimeCaptures = this.player.Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeCaptures();
        this.player.previousRunTimeDefends = this.player.Target.transform.parent.gameObject.GetComponent<HardpointController>().getRunTimeDefends();
    }

    private GameObject chooseHardpoint()
    {      
        List<GameObject> orderedHardpoints = (GameObject.FindGameObjectsWithTag("Hardpoint") 
             .OrderBy(t=> Vector3.Distance(this.player.transform.position, t.transform.position))
             .ToList());

        //If the nearest hardpoint has not been captured by your team
        if (orderedHardpoints[0].GetComponent<HardpointController>().getOwner() != this.player.ps.team){
            //Probabilities:
            //Capture Nearest 80%
            //Capture Second Nearest 15%
            //Capture Other 5%

            var options = new List<KeyValuePair<GameObject, float>>() 
            { 
                new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.5f),
                new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.15f),
                new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.80f),
            };
            return makeDecision(options).transform.GetChild(0).gameObject;
            
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
                return makeDecision(options).transform.GetChild(0).gameObject;
            }
            //If the nearest hardpoint has been captured by your team
            //If the second nearest hardpoint has not been captured
            else if (orderedHardpoints[1].GetComponent<HardpointController>().getOwner() != this.player.ps.team 
                    && orderedHardpoints[2].GetComponent<HardpointController>().getOwner() == this.player.ps.team){
                // Probabilities:
                // Capture Remaining: 70%
                // Defend Nearest: 20%
                // Defend Other: 10%
                var options = new List<KeyValuePair<GameObject, float>>() 
                { 
                    new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.1f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.2f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.70f),
                };
                return makeDecision(options).transform.GetChild(0).gameObject;
            }
            //If the nearest hardpoint has been captured by your team
            //If the second nearest hardpoint has also been captured by your team
            else if (orderedHardpoints[1].GetComponent<HardpointController>().getOwner() == this.player.ps.team 
                    && orderedHardpoints[2].GetComponent<HardpointController>().getOwner() != this.player.ps.team){
                // Probabilities:
                // Capture Remaining: 60%
                // Defend Nearest: 25%
                // Defend Other: 15%
                var options = new List<KeyValuePair<GameObject, float>>() 
                { 
                    new KeyValuePair<GameObject, float>(orderedHardpoints[1], 0.15f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[0], 0.25f),
                    new KeyValuePair<GameObject, float>(orderedHardpoints[2], 0.6f),
                };
                return makeDecision(options).transform.GetChild(0).gameObject;
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
                return makeDecision(options).transform.GetChild(0).gameObject;
            }
        }
        return null;
    }

    private float generateRandom(){
        float rndNum = Random.Range(0f, 1f);
        return rndNum;
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
    public void OnExit() { }
}