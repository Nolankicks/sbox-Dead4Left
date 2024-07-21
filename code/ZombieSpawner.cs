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
			if ( Game.ActiveScene is null )
			{
				break;
			}
			var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToList();
			var randomSpawnPoint = Game.Random.FromList( spawnPoints );
			var zombie = ZombiePrefab.Clone();
			if ( randomSpawnPoint is not null )
			{
				zombie.Transform.Position = randomSpawnPoint.Transform.Position;
			}
			else
			{
				zombie.Transform.Position = Vector3.Zero;
			}

			zombie.NetworkSpawn( null );
			await GameTask.DelaySeconds( 5 );
		}
	}
}


