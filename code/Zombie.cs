using System;
using System.Linq;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class Zombie : Component
{
	[Property] public GameObject body { get; set; }
	[Property] public GameObject eye { get; set; }
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	[Property] public GameObject ragdollGameObject { get; set; }
	[Property] public SoundEvent hitSound { get; set; }
	private NavMeshAgent agent;
	public TimeSince timeSinceHit = 0;
	private Vector3 target;
	private PlayerController targetPlayer;
	protected override void OnStart()
	{

	}
protected override void OnAwake()
    {
		agent = Components.Get<NavMeshAgent>();
        var playerControllers = Scene.GetAllComponents<PlayerController>().ToList();
        targetPlayer = Game.Random.FromList(playerControllers); //you can pick it like this
    }
	protected override void OnUpdate()
	{

		target = targetPlayer.Transform.Position;
		animationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Swing;
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		UpdateAnimtions();
		if (Vector3.DistanceBetween(targetPlayer.Transform.Position, GameObject.Transform.Position ) < 150f)
		{
			agent.Stop();
		}
		else
		{
			agent.MoveTo(target);
		}
		if (target.z > GameObject.Transform.Position.z + 50)
		{
			UpwardTrace();
		}
		else
		{
			NormalTrace();
		}
	}
	
	protected override void OnFixedUpdate()
	{

		
	}

	void UpdateAnimtions()
	{
		var bodyRot = body.Transform.Rotation.Angles();
		animationHelper.WithWishVelocity(agent.WishVelocity);
		animationHelper.WithVelocity(agent.Velocity);
		var targetRot = Rotation.LookAt(target.WithZ(Transform.Position.z) - body.Transform.Position);
		body.Transform.Rotation = Rotation.Slerp(body.Transform.Rotation, targetRot, Time.Delta * 5.0f);
	}
	void NormalTrace()
	{
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + body.Transform.Rotation.Forward * 75).WithTag("player").Run();

		if (tr.Hit && timeSinceHit > 1.0f && GameObject is not null)
		{
			targetPlayer.TakeDamage(25);
			animationHelper.Target.Set("b_attack", true);
			timeSinceHit = 0;
			Sound.Play(hitSound);
		}

	}

	void UpwardTrace()
	{
		var tr = Scene.Trace.Ray(eye.Transform.Position, eye.Transform.Position + eye.Transform.Rotation.Up * 50).Run();

		if (tr.Hit && tr.GameObject.Tags.Has("player") && timeSinceHit > 1.0f && GameObject is not null)
		{
			targetPlayer.TakeDamage(25);
			animationHelper.Target.Set("b_attack", true);
			timeSinceHit = 0;
			Sound.Play(hitSound);
			Log.Info("Hit");
		}

	}


}
