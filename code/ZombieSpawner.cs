using System;
using System.Linq;
using Sandbox;
using Sandbox.Navigation;

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
		var randomSpawnPoint = Scene.NavMesh.GetRandomPoint().GetValueOrDefault();
		var zombie = ZombiePrefab.Clone( randomSpawnPoint );
		zombie.NetworkSpawn();

	}

	TimeUntil nextSecond = 0f;
	protected override void OnFixedUpdate()
	{
		if (nextSecond)
		{
			var random = GetRandom();
			GetRandom();

			if (random >= 80f)
			{
				SpawnZombie();
			}
			nextSecond = 1;
			Log.Info(random);
		}
		
		
	}

}
