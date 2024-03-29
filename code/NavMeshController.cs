using System.Linq;
using System.Runtime.CompilerServices;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class NavMeshController : Component
{
	
	public NavMeshAgent agent;
	private Vector3 _destination;
	public PlayerController playerController;
	public Vector3 testVector = new Vector3(0,0,0);
	RealTimeSince timeSinceUpdate = 0;
	protected override void OnAwake()
	{
		agent = Components.Get<NavMeshAgent>();
		playerController = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		//_destination = playerController.Transform.Position;
	}
	
	protected override void OnUpdate()
	{
	_destination = playerController.Transform.Position;
	}

	protected override void OnFixedUpdate()
	{
		GameObject.Transform.Rotation = Rotation.LookAt(_destination - GameObject.Transform.Position);
		if ( timeSinceUpdate > 0.1 && agent != null)
		{
			timeSinceUpdate = 0;
			agent.MoveTo( _destination );
		}
	}
}
