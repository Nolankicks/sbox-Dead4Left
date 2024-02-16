using System;
using System.Linq;
using Sandbox;
using Sandbox.Navigation;

public sealed class ZombieSpawner : Component
{
	[Property] public GameObject ZombiePrefab { get; set; }
	public float GetRandom() => Random.Shared.Float(1, 100);
	public Zombie[] zombies;
	protected override void OnUpdate()
	{
		zombies = Scene.GetAllComponents<Zombie>().ToArray();
		
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
		if (zombies is null) return;
		if (nextSecond && zombies.Length <= 30)
		{
			SpawnZombie();
			nextSecond = 1;
		}
		}
	}


