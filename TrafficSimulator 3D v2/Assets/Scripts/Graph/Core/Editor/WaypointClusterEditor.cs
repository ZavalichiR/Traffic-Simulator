using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(WaypointCluster))]
public class WaypointClusterEditor : Editor {

	// Editing state, changed with buttons
	enum EditingState 	{

		None,

		Adding,

        Removing
    };

	///Editing state, changed with buttons
	private WaypointCluster clusterobject;			//Object being edited.

	// Editing state, changed with buttons
	private bool dragging = false;					//True if the user is dragging the mouse.

	// Editing state, changed with buttons
	private Waypoint waypointClicked;				//waypoint clicked, if any.

	// Editing state, changed with buttons
	private Waypoint waypointDestiny;				//waypoint the mouse is over, if any.

	// Editing state, changed with buttons
	private EditingState state = EditingState.None;	//Current editing state.

	// Editing state, changed with buttons
	private string currentState = "Not editing.";	//Text shown in the inspector label.

	// Editing state, changed with buttons
	private GameObject previewSphere;				//The small white preview sphere.

	// Editing state, changed with buttons
	int hash = "ClusterControl".GetHashCode();		//hash used to keep the focus on the current object
	
	// Editing state, changed with buttons
	void OnEnable() {
		clusterobject = (WaypointCluster)target;
	}

	// GUI showed on the inspector, some buttons were added to change the editing state and a label to show the current state.
	public override void OnInspectorGUI(){

		DrawDefaultInspector ();
		if(GUILayout.Button("Add waypoints / edges")){
			state = EditingState.Adding;
			currentState = "Adding principal links.";
        } else if (GUILayout.Button("Remove edges"))
        {
            state = EditingState.Removing;
            currentState = "Removing links";
        }
        else if(GUILayout.Button("Turn off edit mode")){
			state = EditingState.None;
			currentState = "Not editing.";
		}
		EditorGUILayout.LabelField("Current state: " + currentState);
	}

	// Scene event handling
	void OnSceneGUI() {
		Event current = Event.current;
		//note: current.button == 0 = leftClick.
		if (state != EditingState.None) {
			if (dragging) movePreview();
			int ID = GUIUtility.GetControlID(hash, FocusType.Passive);

			switch(current.type) 
			{
				//For focus purposes
				case EventType.Layout:
					HandleUtility.AddDefaultControl(ID);
					break;

				//White sphere creation and dragging = true
				case EventType.MouseDown:
                    if (current.button == 1) //ignore right clicks, use them to cancel the dragging
                    {
                        dragging = false;
                        if(previewSphere != null) 
                        	DestroyImmediate(previewSphere);
                    } 
                    else if (current.button == 0) 
                    	createPreview();
					break;

				//Point creation if necessary
				case EventType.MouseUp:
                    if (current.button != 0 ) 
                    	break;
                    if (!dragging) 
                    	break;

                    dragging = false;
					if (previewSphere != null) 
						DestroyImmediate(previewSphere);

					Ray worldray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
					RaycastHit hitinfo;

					if (Physics.Raycast(worldray, out hitinfo)) {
                        if (state == EditingState.Adding) 
                        	createLink(hitinfo);
                        else if (state == EditingState.Removing) 
                        	removeLink(hitinfo);
					}

					waypointClicked = null;
					waypointDestiny = null;

					break;
			}
		}
	}


	private Waypoint createPoint(RaycastHit hitinfo) {
		if (clusterobject.cluster == null) clusterobject.cluster = new GameObject("Waypoint Cluster");
		GameObject waypointAux;
		Undo.RecordObject(clusterobject, "Created waypoint");
		waypointAux = Resources.Load("Waypoint")  as GameObject;
		GameObject waypointInstance = Instantiate(waypointAux) as GameObject;
		waypointInstance.transform.position = hitinfo.point;
		waypointInstance.transform.parent = clusterobject.cluster.transform;
		waypointInstance.name = (clusterobject.waypoints.Count + 1).ToString();
		clusterobject.waypoints.Add(waypointInstance.GetComponent<Waypoint>());
		waypointInstance.GetComponent<Waypoint>().setParent(clusterobject);
		Undo.RegisterCreatedObjectUndo (waypointInstance, "Created waypoint");
		return waypointInstance.GetComponent<Waypoint>();
	}

	private void createLink(RaycastHit hitinfo) {
		if (waypointClicked != null && waypointDestiny != null && waypointClicked != waypointDestiny)
			 link (waypointClicked, waypointDestiny);
			 
		else if (waypointClicked != null && waypointDestiny == null) 
			link (waypointClicked, createPoint(hitinfo));

		else if (waypointClicked == null && waypointDestiny == null) 
			createPoint(hitinfo);
	}

    private void removeLink(RaycastHit hitinfo)
    {
        if (waypointClicked != null && waypointDestiny != null && waypointClicked != waypointDestiny) unLink(waypointClicked, waypointDestiny);
    }

    private void link(Waypoint source, Waypoint destiny) {
		Undo.RecordObject(source, "waypointadd");
		Undo.RecordObject(destiny, "waypointadd");
		source.linkTo(destiny);
	}

    private void unLink(Waypoint source, Waypoint destiny)
    {
        Undo.RecordObject(source, "waypointremove");
        Undo.RecordObject(destiny, "waypointremove");
        source.unlinkFrom(destiny);
    }

    private void createPreview() {

		Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast(worldRay, out hitInfo)) {
			dragging = true;
			previewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			DestroyImmediate(previewSphere.GetComponent<SphereCollider>());
			previewSphere.transform.position = hitInfo.point;
			waypointClicked = hitInfo.transform.gameObject.GetComponent<Waypoint>();
		}
	}

	private void movePreview() {

		Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast(worldRay, out hitInfo)) {
			if (previewSphere == null) {
				previewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				DestroyImmediate(previewSphere.GetComponent<SphereCollider>());
			}

			previewSphere.transform.position = hitInfo.point;
			waypointDestiny = hitInfo.transform.gameObject.GetComponent<Waypoint>();

			if (waypointDestiny != null) waypointDestiny.setColor(Color.green);
			if (waypointClicked != null && waypointClicked != waypointDestiny) {
				if (waypointDestiny != null) 
					DrawArrow.ForDebug(waypointClicked.transform.position, waypointDestiny.transform.position - waypointClicked.transform.position, (state == EditingState.Adding ? Color.red : Color.blue));
				else  if (previewSphere != null && waypointClicked != null) 
					DrawArrow.ForDebug(waypointClicked.transform.position, previewSphere.transform.position - waypointClicked.transform.position, (state == EditingState.Adding ? Color.red : Color.blue)); 
			}
		} else if (previewSphere != null) DestroyImmediate(previewSphere);

	}
}
