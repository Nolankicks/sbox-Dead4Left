using System.Linq;
using Kicks;
using Sandbox;

public sealed class Fist : Component
{
	public NetworkedViewmodel viewModel;
	PlayerController playerController;

	protected override void OnStart()
	{
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		viewModel = GameManager.ActiveScene.GetAllComponents<NetworkedViewmodel>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnFixedUpdate()
	{
		if (Input.Pressed("attack1"))
		{
			Trace();
			//viewModel.arms.Set("b_attack", true);
		}
	}

	void Trace()
	{
		var eyePos = playerController.Transform.Position + Vector3.Up * 64;
		var tr = Scene.Trace.Ray( eyePos, eyePos + playerController.EyeAngles.Forward * 100).WithoutTags("player").Run();

		if (tr.Hit)
		{
			Log.Info(tr.GameObject);
			if (tr.GameObject.Tags.Has("bad"))
			{
				tr.GameObject.Parent.Destroy();
				playerController.AddScore(5);
				Log.Info("Hit");
			}
		}
	}
}
