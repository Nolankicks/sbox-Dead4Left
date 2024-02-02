using System.Linq;
using System.Runtime.CompilerServices;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class NavMeshController : Component
{
	[Property] public GameObject body { get; set; }
	public NavMeshAgent agent;
	private Vector3 _destination;
	public CharacterController playerController;
	RealTimeSince timeSinceUpdate = 0;
	TimeSince timeSinceHit = 0;
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	protected override void OnAwake()
	{
		agent = Components.Get<NavMeshAgent>();
		playerController = Scene.GetAllComponents<CharacterController>().FirstOrDefault();
		_destination = playerController.Transform.Position;
		
	}
	protected override void OnFixedUpdate()
	{
		
		_destination = playerController.Transform.Position;
		GameObject.Transform.Rotation = Rotation.LookAt(_destination - GameObject.Transform.Position);
		if ( timeSinceUpdate > 1 && agent != null)
		{
			timeSinceUpdate = 0;
			agent.MoveTo( _destination );
			UpdateAnimations();
		}
		if (Vector3.DistanceBetween(_destination, GameObject.Transform.Position) < 100 && agent != null)
		{
			agent.Stop();
			Log.Info("Stopped");
			Trace();
			
		}
		
	}
	void Trace()
	{
		var lookDir = GameObject.Transform.Rotation;
		var tr = Scene.Trace.Ray(GameObject.Transform.Position, GameObject.Transform.Position + lookDir.Forward * 100).WithoutTags("bad").Run();
		if (tr.Hit && tr.GameObject.Tags.Has("player"))
		{
			Log.Info("Hit");
		}		
	}
	void UpdateAnimations()
	{
		animationHelper.WithWishVelocity( agent.WishVelocity );
		animationHelper.WithVelocity( agent.Velocity );
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
	}
}
