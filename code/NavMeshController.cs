using Sandbox;

public sealed class NavMeshController : Component
{
	RealTimeSince timeSinceUpdate = 0;
	
	protected override void OnUpdate()
	{
		if (timeSinceUpdate > 0.5f)
		{
			timeSinceUpdate = 0f;

			foreach (var agent in Scene.GetAllComponents<NavMeshAgent>())
			{
				
			}
		}
	}
}
