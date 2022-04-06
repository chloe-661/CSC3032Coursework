using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAiEnemyDetection : MonoBehaviour
{

    public float VIEWRADIUS = 8f;
    public float VIEWANGLE = 150f;

    public float ATTACKRADIUS = 5f;

    private PlayerStatus ps;
    public LayerMask targetMask; //aka "Player" layer
	public LayerMask obstacleMask; //aka "unwalkable" layer
    private List<GameObject> visibleEnemyTargets = new List<GameObject>();
    private List<GameObject> attackableEnemyTargets = new List<GameObject>();

    public List<GameObject> getVisibleEnemyTargets(){ return this.visibleEnemyTargets; }
    public List<GameObject> getAttackableEnemyTargets(){ return this.attackableEnemyTargets; }

    void Start() {
        this.ps = this.GetComponent<PlayerStatus>();
        this.targetMask = LayerMask.GetMask("Player");
        this.obstacleMask = LayerMask.GetMask("Unwalkable");
		StartCoroutine ("FindTargetsWithDelay", .2f);
	}
    IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			findVisibleTargets();
            findAttackableTargets();
		}
	}
    void findVisibleTargets() {
		this.visibleEnemyTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, VIEWRADIUS, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 directionToTarget = (target.position - transform.position).normalized;

			if (Vector3.Angle (transform.forward, directionToTarget) < VIEWANGLE / 2)
            {
				float distanceToTarget = Vector3.Distance (transform.position, target.position);

				if (!Physics.Raycast (transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
					if (target.gameObject.GetComponent<PlayerStatus>().team != this.ps.team)
                    {                    
                        visibleEnemyTargets.Add(target.gameObject);
                    }
				}
			}
		}
	}

    void findAttackableTargets(){
        this.attackableEnemyTargets.Clear();
        Collider[] targetsInAttackRadius = Physics.OverlapSphere (transform.position, ATTACKRADIUS, targetMask);
        for (int i = 0; i < targetsInAttackRadius.Length; i++)
        {
			Transform target = targetsInAttackRadius [i].transform;
			Vector3 directionToTarget = (target.position - transform.position).normalized;

			if (Vector3.Angle (transform.forward, directionToTarget) < VIEWANGLE / 2)
            {
				float distanceToTarget = Vector3.Distance (transform.position, target.position);

				if (!Physics.Raycast (transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
					if (target.gameObject.GetComponent<PlayerStatus>().team != this.ps.team)
                    {                    
                        attackableEnemyTargets.Add(target.gameObject);
                    }
				}
			}
		}
    }
    public Vector3 getFirstSeenEnemy()
    {
        return this.visibleEnemyTargets[0].transform.position;
    }
    public Vector3 directionFromAngle(float angle, bool angleIsGlobal){ //Angle in dregrees
        if (!angleIsGlobal){
            angle += transform.eulerAngles.y;
        }
        
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}