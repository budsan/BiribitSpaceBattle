using UnityEngine;
using System.Collections;

public class GyroscopeDevice : MonoBehaviour
{
	static private GyroscopeDevice instance = null;
	static public GyroscopeDevice Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject obj = new GameObject("GyroDevice");
				instance = obj.AddComponent<GyroscopeDevice>();
				//obj.hideFlags = HideFlags.HideInHierarchy;
			}

			return instance;
		}
	}

	protected Gyroscope gyroscope;
	protected bool available = false;
	protected Quaternion orientationOffset = Quaternion.Euler(90, 0, 0);

	void Start()
	{
		available = SystemInfo.supportsGyroscope;
		if (!available)
			Debug.LogWarning("[GyroscopeDevice] Gyroscope not available in this device");

		if (available)
		{
			Input.compass.enabled = true;
			gyroscope = Input.gyro;
			gyroscope.enabled = true;
		}
	}

	private Quaternion ConvertRotation(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}

	public Quaternion GetRotation()
	{
		if (available)
		{
			Quaternion rotation = orientationOffset * ConvertRotation(gyroscope.attitude);
			return rotation;
		}

		return Quaternion.identity;
	}

	public void OnDestroy()
	{
		if (available)
		{
			gyroscope.enabled = false;
		}
	}
}