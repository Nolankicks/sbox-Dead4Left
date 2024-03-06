using System.Linq;
using Kicks;
using Sandbox;

public sealed class PhysGun : Component
{
	PlayerController playerController;
	public bool ShouldPhys = false;
	public bool AddOutline = false;
	public GameObject PhysObject;
	[Property] public Color Color { get; set; } = Color.White;


	protected override void OnStart()
	{
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	
	}
	protected override void OnFixedUpdate()
	{
		if (IsProxy) return;
		if (Input.Down("attack1"))
		{
			Shoot();
		}
		if (Input.Released("attack1") && ShouldPhys && PhysObject is not null)
		{
			//Make bools false
			var rb = PhysObject.Components?.Get<Rigidbody>();
			rb.Gravity = true;
			PhysObject.Components.Get<HighlightOutline>().Destroy();
			PhysObject = null;
			AddOutline = false;
			ShouldPhys = false;
		}

		if (ShouldPhys && PhysObject is not null)
		{
			float lenght = 500;
			var targetPos = playerController.Crouching ? playerController.Transform.Position + Vector3.Up * 32 + playerController.EyeAngles.Forward * lenght : playerController.Transform.Position + Vector3.Up * 64 + playerController.EyeAngles.Forward * lenght;
	
			//PhysObject.Transform.Position = Transform.Position.LerpTo(targetPos, Time.Delta * 15);
			PhysObject.Transform.Position = targetPos;
			PhysObject.Transform.Rotation = playerController.EyeAngles.ToRotation();
			var rb = PhysObject.Components?.Get<Rigidbody>();
			rb.Gravity = false;
			AddOutline = true;

		}

		if (AddOutline && PhysObject.Components.Get<HighlightOutline>() is null && PhysObject is not null)
		{
			PhysObject.Components.Create<HighlightOutline>();
			AddOutline = false;
			var outLine = PhysObject.Components?.Get<HighlightOutline>();
			outLine.Color = Color;
			outLine.Width = 0.6f;
		}
	}

	void Shoot()
	{
		
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var tr = Scene.Trace.Ray(eyePos, eyePos + playerController.EyeAngles.Forward * 8000).WithoutTags("player").Run();

		if (tr.Hit && tr.GameObject.Components.Get<Prop>() is not null)
		{
			Log.Info("Hit");
			ShouldPhys = true;
			PhysObject = tr.GameObject;
			AddOutline = true;
		}
	}
}
