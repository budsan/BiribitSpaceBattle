using UnityEngine;
using System.Collections;

public class FollowSmooth : MonoBehaviour
{
	public Transform target;
	private Quaternion rotation = Quaternion.identity;

	void Start()
	{
		Input.gyro.updateInterval = 0.01f;
	}

	void FixedUpdate ()
	{
		if (target != null)
			transform.position = Vector3.Lerp(transform.position, target.position, 0.05f);

		transform.rotation = rotation;
	}

	void OnGUI()
	{
		rotation = GyroscopeDevice.Instance.GetRotation();
	}
}
