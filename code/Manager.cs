using Sandbox;
using System.Linq;
using Kicks;
using Sandbox.Network;
using System;
using Sandbox.Navigation;
using static Weapon;

public sealed class Manager : Component
{
	[Property] public SceneFile menuScene {get; set;}
	public bool Playing { get; private set; } = false;
	public long Score { get; private set; } = 0;
	public long HighScore { get; private set; } = 0;
	[Property] public bool testBool {get; set;}
	[Property] public bool ableToInput { get; set; } = false;
	public bool ShouldAddScore { get; set; } = false;
	//[Property] public PlayerController playerController { get; set; }
	PlayerController playerController;
	Weapon weapon;


	public Sandbox.Services.Leaderboards.Board Leaderboard;

	protected override void OnStart()
	{
		playerController = Game.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault();
		weapon = Game.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault();
		Scene.NavMesh.SetDirty();
	}
	protected override void OnAwake()
	{
		StartGame();
	}

	protected override void OnUpdate()
	{
		playerController = Game.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault();
		weapon = Game.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault();

		if ( !playerController.IsValid() ) return;

		ProcessScore();
	}
	protected override void OnFixedUpdate()
	{
		AddDestroyer();
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
		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( player.IsValid() )
		{
			Sandbox.Services.Stats.SetValue( "zombieskilled", player.Score );
		}
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
		var player = Game.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( !player.IsValid() )
			return;
		player.Score = 0;
		player.Health = 100;
		var Spawns = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToArray();
		if (Spawns.Length == 0)
		{
			player.Transform.Position = new Vector3(0, 0, 0);
			return;
		}
		var randomSpawnPoint = Random.Shared.FromArray(Spawns);
		if (randomSpawnPoint is not null)
		{
			player.Transform.Position = randomSpawnPoint.Transform.Position;
		}
		else
		{
			player.Transform.Position = new Vector3(0, 0, 0);
		}
	}

	void AddDestroyer()
	{
		foreach (var gib in Game.ActiveScene.GetAllComponents<Gib>())
		{
			if (gib is not null)
			{
				//gib.Components.Create<Destroyer>();
			}
		}
	}
}
