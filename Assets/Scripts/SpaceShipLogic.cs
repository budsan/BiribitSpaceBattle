using UnityEngine;
using System.Collections;
using System.IO;

public class SpaceShipLogic : BiribitBehaviour
{
	private class State
	{
		public Vector3 position = Vector3.zero;
		public Quaternion rotation = Quaternion.identity;
		public float speed = 1.0f;
		public float time = 0.0f;
	}

	State m_currentState;
	State m_tempState;
	float time = 0.0f;
	Quaternion gyroscope = Quaternion.identity;

	public override void Start()
	{
		base.Start();
		m_currentState = new State();
		m_currentState.position = transform.position;
		m_currentState.rotation = transform.rotation;
		m_currentState.time = Time.time;
	}

	public void Update()
	{
		float delta = Time.time - m_currentState.time;
		m_tempState = UpdateStep(m_currentState, delta);
		
	}

	public void FixedUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, m_tempState.position, 0.1f);
		transform.rotation = Quaternion.Slerp(transform.rotation, m_tempState.rotation, 0.1f);
	}

	public override void NetworkUpdate()
	{
		m_currentState = UpdateStep(m_currentState, manager.NetworkDeltaTime);

		MemoryStream stream = new MemoryStream();
		BinaryWriter writer = new BinaryWriter(stream);
		writer.Write(m_currentState.position.x);
		writer.Write(m_currentState.position.y);
		writer.Write(m_currentState.position.z);
		writer.Write(m_currentState.rotation.x);
		writer.Write(m_currentState.rotation.y);
		writer.Write(m_currentState.rotation.z);
		writer.Write(m_currentState.rotation.w);
		writer.Write(m_currentState.speed);

		manager.Send(stream.ToArray());
	}

	public override void NewIncomingData(byte[] data)
	{
		MemoryStream stream = new MemoryStream(data);
		BinaryReader reader = new BinaryReader(stream);
		State state = new State();
		state.position.x = reader.ReadSingle();
		state.position.y = reader.ReadSingle();
		state.position.z = reader.ReadSingle();
		state.rotation.x = reader.ReadSingle();
		state.rotation.y = reader.ReadSingle();
		state.rotation.z = reader.ReadSingle();
		state.rotation.w = reader.ReadSingle();
		state.speed = reader.ReadSingle();
		state.time = Time.time;
		time = state.time;
		m_currentState = state;
	}

	private State UpdateStep(State currentState, float deltaTime)
	{
		State nextState = new State();
		nextState.position = currentState.position + (currentState.rotation * Vector3.forward) * deltaTime;
		nextState.rotation = Quaternion.Slerp(currentState.rotation, gyroscope, 0.5f);
		nextState.speed = currentState.speed;
		nextState.time = currentState.time + deltaTime;
		return nextState;
	}

	public void OnGUI()
	{
		gyroscope = GyroscopeDevice.Instance.GetRotation();
	}
}
