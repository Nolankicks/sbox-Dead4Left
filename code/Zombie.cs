using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class Zombie : Component
{
	[Property] public NavMeshAgent NavMeshAgent { get; set; }
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	[Property] public SoundEvent hitSound { get; set; }
	[Property] public GameObject gibs { get; set; }
	[Sync] public float Health { get; set; } = 100;
	[Sync] public float MaxHealth { get; set; } = 100;
	public PlayerController targetPlayer;
	public bool NeedsToJump = false;
	private PlayerController localPlayer;
	[Property] public List<Model> materials { get; set; } = new List<Model>();

	protected override void OnStart()
	{
		var players = Game.ActiveScene.GetAllComponents<PlayerController>().ToList();
		targetPlayer = Game.Random.FromList(players);
		Log.Info( $"Targeting {targetPlayer}" );
	}
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		if (targetPlayer is null) return;
		if (Vector3.DistanceBetween(targetPlayer.Transform.Position, NavMeshAgent.Transform.Position) < 150f && targetPlayer is not null)
		{
			NavMeshAgent?.Stop();
		}
		else
		{
			MoveToTarget();
		}
		if (Vector3.DistanceBetween(targetPlayer.Transform.Position, NavMeshAgent.Transform.Position ) < 150f && targetPlayer is not null)
		{
			NavMeshAgent?.Stop();
		}
		
		var target = targetPlayer.Transform.Position;
		if (target.z > GameObject.Transform.Position.z + 50 && targetPlayer is not null)
		{
			UpWardTrace();
		}
		else
		{
			FowardTrace();
		}
		
		//JumpTrace();
		if (targetPlayer is not null)
		{
		UpdateAnimations();
		}
	}
	void MoveToTarget()
	{
		NavMeshAgent?.MoveTo(targetPlayer.Transform.Position);
	}
	void UpdateAnimations()
	{
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		animationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
		animationHelper.WithVelocity(NavMeshAgent.Velocity);
		animationHelper.WithWishVelocity(NavMeshAgent.WishVelocity);
	}
	public TimeSince lastAttack = 0;
	void FowardTrace()
	{
		var tr = Scene.Trace.Ray(GameObject.Transform.Position, GameObject.Transform.Position + Vector3.Up * 64 + GameObject.Transform.Rotation.Forward * 150).Run();
		if (tr.Hit && tr.GameObject.Tags.Has("player") && lastAttack > 1.5f)
		{
		
			tr.GameObject.Parent.Components.TryGet<PlayerController>(out var player);
			player.TakeDamage(10);
			lastAttack = 0;
			Sound.Play(hitSound);
		}
	}
	void UpWardTrace()
	{
		var tr = Scene.Trace.Ray(GameObject.Transform.Position, GameObject.Transform.Position + Vector3.Up * 64 + GameObject.Transform.Rotation.Up * 200).Run();

		if (tr.Hit && tr.GameObject.Tags.Has("player") && lastAttack > 1.5f)
		{
	
			tr.GameObject.Parent.Components.TryGet<PlayerController>(out var player);
			player.TakeDamage(10);
			lastAttack = 0;
			Sound.Play(hitSound);
		}
	}
	[Broadcast]
	public void TakeDamage(float damage, Guid AttackerId)
	{
		if (IsProxy) return;
		Health -= damage;
		if (Health <= 0)
		{
			Kill(AttackerId);
		}
	}
	void Kill(Guid guid)
	{
		var gib = gibs.Clone(GameObject.Transform.World).Components.Get<Prop>();
		AddScore( guid );
		GameObject.Destroy();
		
		
	}
	[Broadcast]
	public void NewTakeDamage( float damage, PlayerController attacker, GameObject zombie )
	{
		if (IsProxy) return;
		if (zombie is null) return;
		var zombiecomponent = zombie.Components.Get<Zombie>();
		zombiecomponent.Health -= damage;
		if (zombiecomponent.Health <= 0)
		{
			zombiecomponent.Health = 0;
			attacker.AddScore(5);
			var gib = gibs.Clone(GameObject.Transform.World).Components.Get<Prop>();
			zombie.Destroy();
		}

	}
	[Broadcast]
	public void DealDamage(float damage, PlayerController attacker)
	{
		//if (IsProxy) return;
		attacker.TakeDamage(damage, attacker);
		Log.Info("Dealt Damage to" + attacker.GameObject.Parent.Name);
	}
	[Broadcast]
	void AddScore( Guid guid )
	{
		var attacker = Scene.Directory.FindByGuid(guid);
		var player = attacker.Components.Get<PlayerController>();
		player.Score += 5;
	}
}
