using System;
using System.Linq;
using Sandbox;
using Sandbox.Navigation;

public sealed class ZombieSpawner : Component
{
	[Property] public GameObject ZombiePrefab { get; set; }
	public float GetRandom() => Random.Shared.Float(1, 100);
	public Zombie[] zombies;
	public Gib[] gibs;
	protected override void OnUpdate()
	{
		zombies = Scene.GetAllComponents<Zombie>().ToArray();
		gibs = Scene.GetAllComponents<Gib>().ToArray();
		Log.Info(zombies.Length);
		foreach (var gib in gibs)
		{
			if (gib is null) return;
			gib.Break();
			gib.Components.TryGet<Rigidbody>(out var rb);
			if (rb is not null)
			{
				rb.RigidbodyFlags = RigidbodyFlags.DisableCollisionSounds;
			}
		}
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
		float time = GetRandom();
		if (zombies is null) return;
		if (nextSecond && zombies.Length <= 30 && time > 80f)
		{
			SpawnZombie();
			nextSecond = 1;
			GetRandom();
		}
		}
	}


