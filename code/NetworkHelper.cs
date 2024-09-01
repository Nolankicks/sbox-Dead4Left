using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sandbox;
using Sandbox.Network;
using Sandbox.Citizen;
using System.Linq;
using Kicks;

public sealed class NetworkHelper : Component, Component.INetworkListener
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


	public void OnActive(Connection channel)
	{
		Log.Info($"Player '{channel.DisplayName}' has joined the game");
		Connections.Add(channel);

		if (Connections.Count == 1)
		{
			Host = channel;
			HostSteamId = (long)channel.SteamId;
		}

		if (PlayerPrefab is null)
		return;

		SpawnPlayer(channel);
	}

	public void SpawnPlayer(Connection channel)
	{
		Log.Info($"Spawning player '{channel.DisplayName}'");
		var startLocation = Transform.World;
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToList();
		if (spawnPoints.Count > 0)
		{
			startLocation = spawnPoints[Random.Shared.Int(0, spawnPoints.Count - 1)].Transform.World;
		}

		startLocation.Scale = 1;

		// Spawn this object and make the client the owner
		var player = PlayerPrefab.Clone(startLocation, name: $"Player - {channel.DisplayName}");
		player.NetworkSpawn(channel);

		var playerController = player.Components.Get<PlayerController>(FindMode.EverythingInSelfAndDescendants);
		playerController.GameObject.Name = channel.DisplayName;
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

		Connections.Remove(channel);
	}

	public void OnBecameHost(Connection previousHost)
	{
		DestroyServer();
	}

	[Broadcast]
	[Obsolete]
	public void DestroyServer()
	{
		Game.ActiveScene.Load(menuScene);
	}
}
