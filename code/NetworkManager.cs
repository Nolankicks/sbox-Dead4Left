using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sandbox;
using Sandbox.Network;
using Sandbox.Citizen;
using System.Linq;
using Kicks;

public sealed class NetworkManager : Component, Component.INetworkListener
{
	public static NetworkManager Instance { get; private set; }

	/// <summary>
	/// Create a server (if we're not joining one)
	/// </summary>
	[Property] public bool StartServer { get; set; } = true;

	/// <summary>
	/// The prefab to spawn for the player to control.
	/// </summary>
	[Property] public GameObject PlayerPrefab { get; set; }
	[Property] public SceneFile menuScene { get; set; }


	public List<Connection> Connections = new();
	public Connection Host = null;
	[Sync] public long HostSteamId { get; set; }

	public List<PlayerController> Players => Game.ActiveScene.Components.GetAll<PlayerController>(FindMode.EnabledInSelfAndDescendants).ToList();
	private PlayerController localPlayer;
	protected override void OnStart()
	{
		localPlayer = Game.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnAwake()
	{
		base.OnAwake();

		Instance = this;
	}

	protected override async Task OnLoad()
	{
		if (Scene.IsEditor)
			return;

		if (StartServer && !GameNetworkSystem.IsActive)
		{
			LoadingScreen.Title = "Creating Lobby";
			await Task.DelayRealtimeSeconds(0.1f);
			GameNetworkSystem.CreateLobby();
		}
	}


	/// <summary>
	/// A client is fully connected to the server. This is called on the host.
	/// </summary>
	public void OnActive(Connection channel)
	{
		Log.Info($"Player '{channel.DisplayName}' has joined the game");

		Connections.Add(channel);

		if (Connections.Count == 1)
		{
			Host = channel;
			HostSteamId = (long)channel.SteamId;

			if (!string.IsNullOrEmpty(LaunchArguments.Map))
				Scene.Title = LaunchArguments.Map;
		}

		if (PlayerPrefab is null)
			return;

		SpawnPlayer(channel);


	}

	[Broadcast]
	public async void SpawnPlayer(Connection channel)
	{


		var startLocation = Transform.World;
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToList();
		if (spawnPoints.Count > 0)
		{
			startLocation = spawnPoints[Random.Shared.Int(0, spawnPoints.Count - 1)].Transform.World;
		}

		startLocation.Scale = 1;

		// Spawn this object and make the client the owner
		var player = PlayerPrefab.Clone(startLocation, name: $"Player - {channel.DisplayName}");
		player.Network.Spawn(channel);

		var playerController = player.Components.Get<PlayerController>(FindMode.EverythingInSelfAndDescendants);



	}

	public void OnDisconnected(Connection channel)
	{
		foreach (var player in Players)
		{
			if (player.SteamId == (long)channel.SteamId)
			{
				player.GameObject.Destroy();
			}
		}
	}


}
