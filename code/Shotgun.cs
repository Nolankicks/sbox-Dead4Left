using Sandbox;

public sealed class Shotgun : Component
{
	[Property] public GameObject eye {get; set;}
	protected override void OnUpdate()
	{

	}

	void Trace()
	{
		var tr = Scene.Trace.Ray(eye.Transform.Position, eye.Transform.Position + eye.Transform.Rotation.Forward * 1000)
			.WithoutTags("player")
			.Run();

			if (tr.Hit)
	}
}
