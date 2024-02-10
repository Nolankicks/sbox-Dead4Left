using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.VisualBasic;
using Sandbox;
using Sandbox.UI;
using System.Linq;
using Kicks;
using Sandbox.Network;
using System;

public sealed class Manager : Component
{
	[Property] public SceneFile menuScene {get; set;}
	public bool Playing { get; private set; } = false;
	public long Score { get; private set; } = 0;
	public long HighScore { get; private set; } = 0;
	[Property] public bool testBool {get; set;}
	[Property] public bool ableToInput { get; set; } = false;
	public bool ShouldAddScore { get; set; } = false;
	[Property] public MenuUi deadMenu { get; set; }
	[Property] public PauseMenu pauseMenu { get; set; }
	[Property] public GameObject ragdoll { get; set; }
	//[Property] public PlayerController playerController { get; set; }
	PlayerController playerController;


	public Sandbox.Services.Leaderboards.Board Leaderboard;

	protected override void OnStart()
	{
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnAwake()
	{
		StartGame();
	}

	protected override void OnUpdate()
	{
		if (playerController is null) return;
		if (playerController.Health <= 0)
		{
			EndGame();
		}
		if (playerController.Health < 0)
		{
			playerController.Health = 0;
		}
		
		ProcessScore();
	}

	public void StartGame()
	{
		

		Playing = true;
		Score = 0;

		FetchLeaderboardInfo();
	}

	public void EndGame()
	{
		Playing = false;
		//deadMenu.IsDead = true;
		Sandbox.Services.Stats.SetValue( "zombieskilled", playerController.Score );
		Log.Info( "Disconnected from server" );
		Respawn();

		

	}

	
	public void ProcessScore()
	{
		
		Score = playerController.Score;
		if ( Score > HighScore ) HighScore = Score;
	}

	async void FetchLeaderboardInfo()
	{
		Leaderboard = Sandbox.Services.Leaderboards.Get( "mostzombieskill" );
		Leaderboard.MaxEntries = 10;
		await Leaderboard.Refresh();
		foreach ( var entry in Leaderboard.Entries )
		{
			if ( entry.SteamId == Game.SteamId )
			{
				HighScore = (long)entry.Value;
			}
		}
	}
	void Respawn()
	{
		playerController.Score = 0;
		playerController.Health = 100;
		var Spawns = GameManager.ActiveScene.GetAllComponents<SpawnPoint>().ToArray();
		var randomSpawnPoint = Random.Shared.FromArray(Spawns);
		playerController.Transform.Position = randomSpawnPoint.Transform.Position;
	}
}
