using UnityEngine;
using System.Collections;

public class sun : MonoBehaviour {
    public bool Night;
    private float _time = 12f;
	void Update () 
	{

		transform.RotateAround(Vector3.zero,Vector3.right,Time.deltaTime);
		transform.LookAt(Vector3.zero);
        if (transform.position.y < 0f)
            Night = true;
        else
            Night = false;
	}
}
