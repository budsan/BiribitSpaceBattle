using UnityEngine;
using System.Collections.Generic;
using System;

public class ShipBattleManager : MonoBehaviour, BiribitListener
{
	public FollowSmooth mainCamera;
	public GameObject playerToInstantiate;
	public string SceneToLoadOnDisconnection;
	public UnityEngine.UI.Text UILog;

	class LogEntry
	{
		public string Text = "";
		public float LifeTime = 5;

		public LogEntry(string content)
		{
			Text = content;
			LifeTime = Time.time;
		}
	}

	private List<LogEntry> log = new List<LogEntry>();

	private BiribitManager manager;
	private SpaceShipLogic[] players = new SpaceShipLogic[0];

	void AddLog(string message)
	{
		log.Reverse();
		log.Add(new LogEntry(message));
		log.Reverse();
	}

	void Start ()
	{
		manager = BiribitManager.Instance;
		manager.AddListener(this);

		bool available = SystemInfo.supportsGyroscope;
		if (!available)
			Debug.LogWarning("[GyroscopeDevice] Gyroscope not available in this device");

		AddLog("Welcome!");
	}
	
	void OnDestroy()
	{
		manager.RemoveListener(this);
	}

	void Update()
	{
		if (UILog != null)
			UILog.text = "";

		int i = 0;
		float ThresholdTime = Time.time - 10;
		while (i < log.Count && log[i].LifeTime > ThresholdTime)
		{
			if (UILog != null)
				UILog.text += log[i].Text + "\n";
			i++;
		}

		if (i < log.Count)
			log.RemoveRange(i, log.Count - i);
	}

	public void PlayerJoined(uint id)
	{
		GameObject newship = Instantiate(playerToInstantiate);
		SpaceShipLogic logic = newship.GetComponent<SpaceShipLogic>();
		logic.PlayerSlot = id;

		if (id >= players.Length)
		{
			SpaceShipLogic[] newplayers = new SpaceShipLogic[id + 1];
			if (players.Length > 0)
				Array.Copy(players, newplayers, players.Length);

			players = newplayers;
		}

		players[id] = logic;

		if (id == manager.GetLocalPlayer())
		{
			Transform transform = logic.transform.Find("CameraPosition");
			if (mainCamera != null)
				mainCamera.target = transform;
		}

		AddLog("Player " + id + " " + manager.GetPlayerName(id) + " joined!");
	}

	public void PlayerLeaved(uint id)
	{
		if (id < players.Length)
		{
			SpaceShipLogic logic = players[id];
			Destroy(logic.gameObject);
			players[id] = null;
		}

		AddLog("Player " + id + " " + manager.GetPlayerName(id) + " leaved!");
	}

	public void Connected()
	{

	}

	public void Disconnected()
	{
		if (!string.IsNullOrEmpty(SceneToLoadOnDisconnection))
		{
			Application.LoadLevel(SceneToLoadOnDisconnection);
		}
	}
}
