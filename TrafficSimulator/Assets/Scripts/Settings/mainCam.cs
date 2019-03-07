using UnityEngine;
using System.Collections;

public class mainCam : MonoBehaviour {

	public Transform targetPos;
	void FixedUpdate()
	{
		//transform.LookAt(targetPos.position);

		transform.LookAt(new Vector3(targetPos.position.x,0f,targetPos.position.z));
	}
}
