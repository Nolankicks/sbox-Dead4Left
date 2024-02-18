using System.Linq;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class Zombie : Component, IHealthComponent
{

	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	[Property] public NavMeshAgent NavMeshAgent { get; set; }
	[Property] public GameObject body { get; set; }
	[Property] public SoundEvent hitSound { get; set; }
	[Sync] public float Health { get; set; } = 100;
	[Sync] public float MaxHealth { get; set; } = 100;
	public PlayerController targetPlayer;
	protected override void OnStart()
	{
		AnimationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Auto;
		AnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
		var players = GameManager.ActiveScene.GetAllComponents<PlayerController>().ToList();
		targetPlayer = Game.Random.FromList(players);
		Log.Info( $"Targeting {targetPlayer}" );
	}
	protected override void OnUpdate()
	{
		if (Vector3.DistanceBetween(targetPlayer.Transform.Position, body.Transform.Position ) < 150f)
		{
			NavMeshAgent.Stop();
		}
		else
		{
			NavMeshAgent.MoveTo(targetPlayer.Transform.Position);
		}
		if (Vector3.DistanceBetween(targetPlayer.Transform.Position, GameObject.Transform.Position ) < 150f)
		{
			NavMeshAgent.Stop();
		}
		var target = targetPlayer.Transform.Position;
		
		UpdateAnimations();
		if (target.z > GameObject.Transform.Position.z + 50)
		{
			UpWardTrace();
		}
		else
		{
			FowardTrace();
		}
	}

	void UpdateAnimations()
	{
		var target = targetPlayer.Transform.Position;
		var bodyRot = AnimationHelper.Transform.Rotation.Angles();
		AnimationHelper.WithWishVelocity( NavMeshAgent.WishVelocity );
		AnimationHelper.WithVelocity(NavMeshAgent.WishVelocity);
		var targetRot = Rotation.LookAt(target.WithZ(Transform.Position.z) - Transform.Position);
		body.Transform.Rotation = Rotation.Slerp( body.Transform.Rotation, targetRot, Time.Delta * 5 );
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
	public void TakeDamage(float damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			Health = 0;
			//GameManager.ActiveScene.RemoveComponent(this);
			GameObject.Destroy();
		}
	}
}
