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
	private PlayerController playerController;
	public TimeSince timeSinceHit = 0;
	private Vector3 target;
	protected override void OnAwake()
	{
		agent = Components.Get<NavMeshAgent>();
		playerController = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		
	}
	protected override void OnUpdate()
	{

		target = playerController.Transform.Position;
		playerController = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		animationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Swing;
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		UpdateAnimtions();
		Trace();
		if (Vector3.DistanceBetween(target, GameObject.Transform.Position) < 175f)
		{
			agent.Stop();
			Log.Info("Stopped");
		}
		else
		{
			agent.MoveTo(playerController.Transform.Position);
		}
	}
	
	protected override void OnFixedUpdate()
	{
	 	var bodyRot = body.Transform.Rotation = Rotation.LookAt(playerController.Transform.Position - body.Transform.Position);
	}

	void UpdateAnimtions()
	{
		animationHelper.WithWishVelocity(agent.WishVelocity);
		animationHelper.WithVelocity(agent.Velocity);
	}
	void Trace()
	{
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + body.Transform.Rotation.Forward * 75).Run();

		if (tr.Hit && tr.GameObject.Tags.Has("player") && timeSinceHit > 1.0f && GameObject is not null)
		{
			playerController.TakeDamage(15);
			animationHelper.Target.Set("b_attack", true);
			timeSinceHit = 0;
			Sound.Play(hitSound);
		}

	}


}
