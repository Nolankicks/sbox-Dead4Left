using System.Linq;
using Kicks;
using Sandbox;

public sealed class Fist : Component
{
	public ActiveWeapon viewModel;
	PlayerController playerController;
	[Property] public AnimationGraph punchGraph { get; set; }
	[Property] public SkinnedModelRenderer arms { get; set; }
	public bool PushViewModel;
	protected override void OnStart()
	{
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		arms.SceneModel.AnimationGraph = punchGraph;
		GameObject.Transform.LocalPosition = new Vector3(8f, 0f, 63.7f);
	}
	protected override void OnUpdate()
	{
		GameObject.Transform.Rotation = Rotation.Lerp(GameObject.Transform.Rotation, playerController.eye.Transform.Rotation, Time.Delta * 20);
		
		

		


	}
	protected override void OnFixedUpdate()
	{
		if (Input.Pressed("attack1"))
		{
			Trace();
			arms.Set("b_attack", true);
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
