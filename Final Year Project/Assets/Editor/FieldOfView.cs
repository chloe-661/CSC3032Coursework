using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (StateAiEnemyDetection))]
public class FieldOfView : Editor
{
    void OnSceneGUI(){
        Debug.Log("Running OnSceneGUI");
        StateAiEnemyDetection fov = (StateAiEnemyDetection)target;
        Handles.color = Color.black;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.VIEWRADIUS);
        Vector3 viewAngleA = fov.directionFromAngle (-fov.VIEWANGLE / 2, false);
		Vector3 viewAngleB = fov.directionFromAngle (fov.VIEWANGLE / 2, false);

        Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngleA * fov.VIEWRADIUS);
		Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngleB * fov.VIEWRADIUS);

        Handles.color = Color.yellow;
        foreach (GameObject visibleTarget in fov.getVisibleEnemyTargets()) {
			Handles.DrawLine (fov.transform.position, visibleTarget.transform.position);
		}

        Handles.color = Color.magenta;
        Handles.DrawLine (fov.transform.position, fov.transform.position + fov.transform.forward * 15);
    }
}
