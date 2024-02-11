using System;
using System.Linq;
using Sandbox;

public sealed class ZombieSpawner : Component
{
	[Property] public GameObject ZombiePrefab { get; set; }
	public float GetRandom() => Random.Shared.Float(1, 100);
	protected override void OnUpdate()
	{
		
	}
	void SpawnZombie()
	{
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();
		var randomSpawnPoint = Random.Shared.FromArray( spawnPoints ).Transform.World;
		var zombie = ZombiePrefab.Clone( randomSpawnPoint );
		zombie.Network.Spawn();

	}

	TimeUntil nextSecond = 0f;
	protected override void OnFixedUpdate()
	{
		if (nextSecond)
		{
			var random = GetRandom();
			GetRandom();

			if (random >= 70f)
			{
				SpawnZombie();
			}
			nextSecond = 1;
			Log.Info(random);
		}
		
		
	}

}
