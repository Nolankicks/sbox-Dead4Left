using Sandbox;

public sealed class Grabber : Component
{
	public PhysicsBody physicsBody;
	protected override void OnUpdate()
	{
		Log.Info(physicsBody);
		if (Input.Down("attack1"))
		{
			var ray = Scene.Camera.ScreenNormalToRay(0.5f);
			var tr = Scene.Trace.Ray(ray, 5000).WithoutTags("player").Run();

			if (!tr.Hit || tr.Body is null || tr.Body.BodyType != PhysicsBodyType.Dynamic)
			{
				return;
			}
			physicsBody = tr.Body;
			physicsBody.SmoothMove(Scene.Camera.Transform.World.ToWorld(Scene.Camera.Transform.World), 0.1f, Time.Delta);
			
		}
		else
		{
			physicsBody = null;
		}
	}
}
