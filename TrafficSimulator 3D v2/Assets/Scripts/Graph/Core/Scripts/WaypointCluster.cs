using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointCluster : MonoBehaviour {

	[HideInInspector] 
	public List<Waypoint> waypoints = new List<Waypoint>();
    
    [HideInInspector]
    public GameObject cluster = null;

    private uint currentID = 0;

	public Waypoint CreateWaypoint(Vector3 point) {

		GameObject waypointAux = Resources.Load("Waypoint")  as GameObject;
		GameObject waypointInstance = Instantiate(waypointAux) as GameObject;

		waypointInstance.transform.position = point;
		waypointInstance.transform.parent = cluster.transform;
        waypointInstance.name = currentID.ToString();
        ++currentID;
		waypoints.Add(waypointInstance.GetComponent<Waypoint>());
		waypointInstance.GetComponent<Waypoint>().setParent(this);

		return waypointInstance.GetComponent<Waypoint>();
	}
	
}
