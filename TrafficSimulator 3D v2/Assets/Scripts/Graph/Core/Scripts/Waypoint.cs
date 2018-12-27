using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour {
	[SerializeField]
	[HideInInspector] 
	// Parent graph of the waypoint
	protected WaypointCluster parent;
	
	// The outgoing list of edges
	public List<WaypointPercent> outs = new List<WaypointPercent>(); 
    
	[HideInInspector]
	// Incoming list of edges, hidden in the inspector
    public List<Waypoint> ins = new List<Waypoint>();				

	// Main node color
	Color color = Color.white;	

	public void setParent(WaypointCluster wc) 
		{
		 	parent = wc; 
		}

	public WaypointCluster getParent()
	 { 
	 	return parent;
	 }

	// Returns a random waypoint based on the probabilty defined in the WaypointPercent class
	public Waypoint getNextWaypoint() {
		int prob = Random.Range(0, 100);
		int sum = 0;
		for (int i = 0; i < outs.Count; ++i) {
			sum += outs[i].probability;
			if (prob <= sum) {
				return outs[i].waypoint;
			}
		}
		Debug.LogError("Last waypoint was returned on waypoint " + this.name + ". Check that the probabilities correctly sum at least 100 on that waypoint.");
		return outs[outs.Count-1].waypoint;
	}
	

	public void setSame() {
		int size = outs.Count;
		for (int i =0; i < size; ++i) {
			outs[i].probability = 100/size;
			if (i < 100%size)
				 outs[i].probability++;
		}
	}

	public void linkTo(Waypoint waypoint) {
		if (waypoint == this) {
			Debug.LogError("A waypoint cannot be linked to itself");
			return;
		}
		for (int i = 0; i < outs.Count; ++i) 
			if (waypoint == outs[i].waypoint) 
				return;

		if (waypoint.ins.Contains(this)) 
			return;

		outs.Add(new WaypointPercent(waypoint));
		waypoint.ins.Add(this);
		setSame();
		//parent.CreateWaypoint(Vector3.zero);
	}

	public void unlinkFrom(Waypoint waypoint) {
		for (int i = 0; i < outs.Count; ++i) if (outs[i].waypoint == waypoint) outs.RemoveAt(i);
		waypoint.ins.Remove(this);
	}

	public void setColor(Color color) {
		this.color = color;
	}

	private static void ForGizmo(Vector3 pos, Vector3 direction, Color c, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
		Gizmos.color = c;
		Gizmos.DrawRay(pos, direction);
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Gizmos.DrawRay(pos + direction, right * 0.5f ); // Left line of head
		Gizmos.DrawRay(pos + direction, left * 0.5f );  // Righ line of head
	}

	public virtual void OnDrawGizmos() {
		Gizmos.color = color;
		Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
		for (int i = 0; i < outs.Count; ++i) {
			Vector3 direction = outs[i].waypoint.transform.position - transform.position;
			ForGizmo(transform.position  + direction.normalized, direction - direction.normalized * 1.5f, Color.red, 2f);
		}
		
		if (color.Equals(Color.green) || color.Equals(Color.white)) 
			color = Color.white;
	}

	public virtual void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
		for (int i = 0; i < outs.Count; ++i) {
			Vector3 direction = outs[i].waypoint.transform.position - transform.position;
			ForGizmo(transform.position + direction.normalized , direction - direction.normalized * 1.5f, Color.magenta, 2f);
		}
	}

	
	[ExecuteInEditMode]
	private void OnDestroy() {
		if (parent == null) return;
		for (int i = 0; i < outs.Count; ++i) Undo.RegisterCompleteObjectUndo(outs[i].waypoint, "destroyed");
		for (int i = 0; i < ins.Count; ++i) Undo.RegisterCompleteObjectUndo(ins[i], "destroyed");
		Undo.RegisterCompleteObjectUndo(this.getParent(), "destroyed");
		for (int i = outs.Count-1; i >= 0; --i) this.unlinkFrom(outs[i].waypoint);
		for (int i = ins.Count-1; i >= 0; --i) ins[i].unlinkFrom(this);
		Undo.RegisterCompleteObjectUndo(this, "destroyed");
		this.getParent().waypoints.Remove(this);
	}

}

[System.Serializable]

public class WaypointPercent {
	[Range(0, 100)]

	public int probability = 0;
	[ReadOnly] 

	public Waypoint waypoint;

	public WaypointPercent(Waypoint waypoint) {
		this.waypoint = waypoint;
	}

}