using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour {

    public Transform ObjectToFollow;
    public Vector3 Offset;
    public float FollowSpeed = 10;
    public float LookSpeed = 10;

    private void FixedUpdate()
	{
		LookAtTarget();
		MoveToTarget();
	}

    /// <summary>
    /// Rotate camera
    /// </summary>
    public void LookAtTarget()
    {
        Vector3 _lookDirection = ObjectToFollow.position - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, LookSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Fallow the target
    /// </summary>
    public void MoveToTarget()
    {
        Vector3 _targetPos = ObjectToFollow.position +
                             ObjectToFollow.forward * Offset.z +
                             ObjectToFollow.right * Offset.x +
                             ObjectToFollow.up * Offset.y;
        transform.position = Vector3.Lerp(transform.position, _targetPos, FollowSpeed * Time.deltaTime);
    }
}
