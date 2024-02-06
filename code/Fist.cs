using Sandbox;

public sealed class Fist : Component
{
	[Property] public GameObject eye { get; set; }
	[Property] public SkinnedModelRenderer arms { get; set; }
	protected override void OnUpdate()
	{
		if (Input.Pressed("attack1"))
		{
			Trace();
			arms.Set("b_attack", true);
		}
	}

	void Trace()
	{
		var tr = Scene.Trace.Ray( eye.Transform.Position, eye.Transform.Position + eye.Transform.Rotation.Forward * 100).WithoutTags("player").Run();

		if (tr.Hit)
		{
			Log.Info(tr.GameObject);
			if (tr.GameObject.Tags.Has("bad"))
			{
				tr.GameObject.Parent.Destroy();
			}
		}
	}
}
