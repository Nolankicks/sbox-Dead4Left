using System.Linq;
using Kicks;
using Sandbox;

public sealed class Fist : Component
{
	[Property] public GameObject eye { get; set; }
	[Property] public SkinnedModelRenderer arms { get; set; }
	[Property] public Weapon weapon { get; set; }
	PlayerController playerController;

	protected override void OnStart()
	{
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		if (Input.Pressed("attack1") && weapon.ActiveSlot != 0)
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
				playerController.AddScore(5);
			}
		}
	}
}
