using Sandbox;
using Sandbox.Citizen;

public sealed class Zombie : Component
{
	[Property] public GameObject body { get; set; }
	[Property] public GameObject eyes { get; set; }
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	private NavMeshAgent agent;
	protected override void OnAwake()
	{
		agent = Components.Get<NavMeshAgent>();
	}
	protected override void OnUpdate()
	{
		animationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Swing;
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		UpdateAnimtions();
	}
	void UpdateAnimtions()
	{
		animationHelper.WithWishVelocity(agent.WishVelocity);
		animationHelper.WithVelocity(agent.Velocity);
	}
}
