using System;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Navigation;

public sealed class ZombieSpawner : Component
{
	[Property] public GameObject ZombiePrefab { get; set; }
	public Zombie[] zombies;

	protected override void OnStart()
	{
		_ = SpawnZombie();
	}
	protected override void OnUpdate()
	{
		foreach ( var gib in Scene.GetAllComponents<Gib>() )
		{
			if ( gib is null ) return;
			//gib.Break();
			gib.Components.TryGet<Rigidbody>( out var rb );
			if ( rb is not null )
			{
				rb.RigidbodyFlags = RigidbodyFlags.DisableCollisionSounds;
			}
		}
	}

	TimeUntil nextSecond = 0f;

	public async Task SpawnZombie()
	{
		while ( true )
		{
			var zombies = Scene.GetAllComponents<Zombie>().ToList();
			if ( zombies.Count > 30 )
			{
				await Task.Frame();
				continue;
			}

			var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToList();
			var randomSpawnPoint = Game.Random.FromList( spawnPoints );
			var zombie = ZombiePrefab.Clone( randomSpawnPoint.Transform.Position );
			zombie.NetworkSpawn( null );
			await GameTask.DelaySeconds( 5 );
		}
	}
}


