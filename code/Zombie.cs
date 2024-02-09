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
	private PlayerController playerController;
	protected override void OnStart()
	{
		agent = Components.Get<NavMeshAgent>();
		
	}
	protected override void OnUpdate()
	{

		var playerControllers = Scene.GetAllComponents<PlayerController>().ToList();
		var random = Game.Random.FromList(playerControllers);
		if (random is not null)
		{
			target = random.Transform.Position;
		}
		else
		{
			return;
		}
		
		

		
	


		playerController = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		animationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Swing;
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		UpdateAnimtions();
		if (Vector3.DistanceBetween(target, GameObject.Transform.Position ) < 150f)
		{
			agent.Stop();

		}
		else
		{
			agent.MoveTo(playerController.Transform.Position);
		}

		if (playerController.Transform.Position.z > GameObject.Transform.Position.z + 50)
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
		var targetRot = Rotation.LookAt(playerController.GameObject.Transform.Position.WithZ(Transform.Position.z) - body.Transform.Position);
		body.Transform.Rotation = Rotation.Slerp(body.Transform.Rotation, targetRot, Time.Delta * 5.0f);
	}
	void NormalTrace()
	{
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + body.Transform.Rotation.Forward * 75).WithTag("player").Run();

		if (tr.Hit && timeSinceHit > 1.0f && GameObject is not null)
		{
			playerController.TakeDamage(25);
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
			playerController.TakeDamage(25);
			animationHelper.Target.Set("b_attack", true);
			timeSinceHit = 0;
			Sound.Play(hitSound);
			Log.Info("Hit");
		}

	}


}
