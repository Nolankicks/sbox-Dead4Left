using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class Zombie : Component, IHealthComponent
{

	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	[Property] public NavMeshAgent NavMeshAgent { get; set; }
	[Property] public GameObject body { get; set; }
	[Property] public SkinnedModelRenderer bodyRenderer { get; set; }
	[Property] public SoundEvent hitSound { get; set; }
	[Sync] public float Health { get; set; } = 100;
	[Sync] public float MaxHealth { get; set; } = 100;
	public PlayerController targetPlayer;
	public bool NeedsToJump = false;
	private PlayerController localPlayer;
	[Property] public List<Material> materials { get; set; } = new List<Material>();

	protected override void OnStart()
	{
		var randomMaterial = Game.Random.FromList(materials);
		bodyRenderer.SetMaterial(randomMaterial);
		AnimationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Auto;
		AnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
		var players = Game.ActiveScene.GetAllComponents<PlayerController>().ToList();
		targetPlayer = Game.Random.FromList(players);
		Log.Info( $"Targeting {targetPlayer}" );
	}
	protected override void OnUpdate()
	{
		
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
		var target = targetPlayer.Transform.Position;
		var bodyRot = AnimationHelper.Transform.Rotation.Angles();
		AnimationHelper?.WithVelocity(NavMeshAgent.Velocity);
	}
	public TimeSince lastAttack = 0;
	void FowardTrace()
	{
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + Vector3.Up * 64 + body.Transform.Rotation.Forward * 150).Run();

		if (tr.Hit && tr.GameObject.Tags.Has("player") && lastAttack > 1.5f)
		{
			AnimationHelper.Target.Set("b_attack" , true);
			targetPlayer.TakeDamage(10);
			lastAttack = 0;
			Sound.Play(hitSound);
		}
	}
	void UpWardTrace()
	{
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + Vector3.Up * 64 + body.Transform.Rotation.Up * 200).Run();

		if (tr.Hit && tr.GameObject.Tags.Has("player") && lastAttack > 1.5f)
		{
			AnimationHelper.Target.Set("b_attack" , true);
			targetPlayer.TakeDamage(10);
			lastAttack = 0;
			Sound.Play(hitSound);
		}
	}
	[Broadcast]
	public void TakeDamage(float damage, PlayerController attacker)
	{
		Health -= damage;
		if (Health <= 0)
		{
			Health = 0;
			attacker.AddScore(5);
			
			GameObject.Destroy();
		}
	}

	void JumpTrace()
	{
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + Vector3.Up * 10 + body.Transform.Rotation.Forward * 5).WithoutTags("player").Run();

		if (tr.Hit)
		{
			NeedsToJump = true;
			Log.Info("Needs to jump");
		}
		else
		{
			NeedsToJump = false;
		}
	}

}
