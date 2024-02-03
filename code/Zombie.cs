using System.Linq;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class Zombie : Component
{
	[Property] public GameObject body { get; set; }
	[Property] public GameObject eyes { get; set; }
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	private NavMeshAgent agent;
	private PlayerController playerController;
	public TimeSince timeSinceHit = 0;
	protected override void OnAwake()
	{
		agent = Components.Get<NavMeshAgent>();
		playerController = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
	}
	protected override void OnUpdate()
	{
		animationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Swing;
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		UpdateAnimtions();
		Trace();
	}
	void UpdateAnimtions()
	{
		animationHelper.WithWishVelocity(agent.WishVelocity);
		animationHelper.WithVelocity(agent.Velocity);
	}
	void Trace()
	{
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + body.Transform.Rotation.Forward * 160).Run();

		if (tr.Hit && tr.GameObject.Tags.Has("player") && timeSinceHit > 1.0f)
		{
			playerController.TakeDamage(15);
			animationHelper.Target.Set("b_attack", true);
			timeSinceHit = 0;
		}

	}
	
}
