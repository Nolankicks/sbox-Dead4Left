using System;
using System.Collections.Generic;
using System.Linq;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class Zombie : Component
{
	[Property] public List<Model> hairList { get; set; } = new List<Model>()
	{

	};
	[Property] public GameObject body { get; set; }
	[Property] public GameObject eye { get; set; }
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	[Property] public GameObject ragdollGameObject { get; set; }
	[Property] public SoundEvent hitSound { get; set; }
	[Property] public SkinnedModelRenderer hair { get; set; }
	private NavMeshAgent agent;
	public TimeSince timeSinceHit = 0;
	private Vector3 target;
	private PlayerController targetPlayer;
	
	protected override void OnStart()
	{
		agent = Components.Get<NavMeshAgent>();
        var playerControllers = Scene.GetAllComponents<PlayerController>().ToList();
        targetPlayer = Game.Random.FromList(playerControllers); //you can pick it like this

		hair.Model = hairList[Random.Shared.Int(0, hairList.Count)];
		
		if (hair.Model == null)
		{
			hair.GameObject.Destroy();
		}
	}
	protected override void OnUpdate()
	{
		if (targetPlayer is not null)
		{
		target = targetPlayer.Transform.Position;
		animationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Swing;
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		
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
	UpdateAnimtions();
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
