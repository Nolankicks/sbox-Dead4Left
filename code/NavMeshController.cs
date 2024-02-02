using Sandbox;

public sealed class NavMeshController : Component
{
	[Property] public GameObject Target { get; set; }
	RealTimeSince timeSinceUpdate = 0;
	protected override void OnUpdate()
	{
		if ( timeSinceUpdate >  1)
		{
			timeSinceUpdate = 0;
			foreach ( var agent in Scene.GetAllComponents<NavMeshAgent>() )
			{
				agent.MoveTo( Target.Transform.Position );
			}
		}
	}
}
