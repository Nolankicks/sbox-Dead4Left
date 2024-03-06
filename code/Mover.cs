using Sandbox;
using Kicks;
using System.Linq;
using Microsoft.VisualBasic;
public sealed class Mover : Component
{
	public PlayerController playerController;
	public bool ShouldTrace = false;
	public GameObject traceObject;
	[Property] public Vector3 testvector { get; set; }

	protected override void OnStart()
	{
		if (IsProxy) return;
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		if (Input.Pressed("use") && !IsProxy)
		{
			ShouldTrace = !ShouldTrace;
		}
		if (ShouldTrace && !IsProxy)
		{
			Trace();
		}
		if (traceObject is not null)
		{
			traceObject.Transform.Position = Vector3.Lerp(traceObject.Transform.Position, testvector, 10000f);
			Log.Info("lerping");
		}
		if (!ShouldTrace)
		{
			traceObject = null;
		}
	}

	void Trace()
	{
		if (IsProxy) return;
		
		var tr = Scene.Trace.Ray(playerController.Transform.Position + Vector3.Up * 64, playerController.Transform.Position + Vector3.Up * 64 + playerController.EyeAngles.Forward * 100).WithoutTags("player").Run();
		if (tr.Hit && tr.GameObject.Components.Get<Prop>() is not null)
		{
			traceObject = tr.GameObject;
		}
		

		
	}
}
