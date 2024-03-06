using System;
using System.Linq;
using Kicks;
using Sandbox;
[Category("Custon Weapon"), Icon("track_changes"), Title("Shotgun"), Description("A shotgun.")]
public sealed class Shotgun : Component
{
	[Property, Category("ModelRenders")] public SkinnedModelRenderer gun { get; set; }
	[Property, Category("ModelRenders")] public SkinnedModelRenderer arms { get; set; }
	[Property, Category("Weapon Data")] public int Ammo { get; set; }
	[Property, Category("Weapon Data")] public int MaxAmmo { get; set; } = 32;
	[Property, Category("Particles and Decals")] public GameObject decal { get; set; }
	public int ShotsFired { get; set; }
	private PlayerController playerController;
	public float FireRate { get; set; } = 0.5f;
	public TimeSince timeSinceShoot;

	
	protected override void OnStart()
	{
		if (IsProxy) return;
		playerController = Game.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		timeSinceShoot = FireRate;
	}
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		if (Input.Down("attack1") && !IsProxy && timeSinceShoot >= FireRate)
		{
			Shoot();
			gun.Set("b_attack", true);
			timeSinceShoot = 0;
		}
	}

	void Shoot()
	{
		if (IsProxy) return;
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var angles = playerController.EyeAngles.Forward * Random.Shared.Float(1, 5);
		var random = Random.Shared.Float(-1, 1);
		var tr = Scene.Trace.Ray(eyePos, eyePos + playerController.EyeAngles.Forward * 500).WithoutTags("player").Run();
		var decalVar = decal.Clone( tr.HitPosition + tr.Normal * 2.0f, Rotation.LookAt(-tr.Normal));
		//decalVar.Parent = tr.GameObject;
	}
}
