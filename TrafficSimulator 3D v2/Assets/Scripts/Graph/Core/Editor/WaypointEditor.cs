using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waypoint), true)]
[CanEditMultipleObjects]
class WaypointEditor : Editor {

	private Waypoint waypoint;

	void OnEnable() {
		waypoint = (Waypoint)target;
	}

	public override void OnInspectorGUI(){
		if(GUILayout.Button("Same probabilities for all edges")) waypoint.setSame();
		DrawDefaultInspector ();
    }

}

