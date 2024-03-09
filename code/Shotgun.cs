using System;
using System.Linq;
using Kicks;
using Sandbox;
[Category("Custon Weapon"), Icon("track_changes"), Title("Shotgun"), Description("A shotgun.")]
public sealed class Shotgun : Component
{
	[Property, Category("ModelRenders")] public SkinnedModelRenderer gun { get; set; }
	[Property, Category("ModelRenders")] public SkinnedModelRenderer arms { get; set; }
	[Property, Category("Weapon Data")] public int Ammo { get; set; } = 16;
	[Property, Category("Weapon Data")] public int MaxAmmo { get; set; } = 32;
	[Property, Category("Particles and Decals")] public GameObject decal { get; set; }
	[Property, Category("Particles and Decals")] public GameObject muzzleFlash { get; set; }
	[Property, Category("Particles and Decals")] public GameObject impactEffect { get; set; }
	public int ShotsFired = 0;
	private PlayerController playerController;
	private CharacterController characterController;
	public float FireRate { get; set; } = 0.5f;
	public TimeSince timeSinceShoot;
	public TimeSince timeSinceReload = 1.5f;
	protected override void OnStart()
	{
		if (IsProxy) return;
		playerController = Game.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		characterController = Game.ActiveScene.GetAllComponents<CharacterController>().FirstOrDefault(x => !x.IsProxy);
		timeSinceShoot = FireRate;
	}
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		Animations();
		if (Input.Down("attack1") && !IsProxy && timeSinceShoot >= FireRate && timeSinceReload >= 1.5f && Ammo != 0)
		{
			for (int i = 0; i < 8; i++)
			{
			Shoot();
			}
			gun.Set("b_attack", true);
			timeSinceShoot = 0;
		}
		if (Input.Pressed("reload") && MaxAmmo != 0 && ShotsFired != 0 && !IsProxy && MaxAmmo >= ShotsFired)
		{
			
			gun.Set("b_reload", true);
			var finalAmmo = MaxAmmo -= ShotsFired;
			Ammo = finalAmmo;
			ShotsFired = 0;
			Log.Info(MaxAmmo);
			Log.Info(Ammo);
			timeSinceReload = 0;
		}
	}

	void Shoot()
	{
		if (IsProxy) return;
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var ray = Scene.Camera.ScreenNormalToRay( 0.5f );
		ray.Forward += Vector3.Random * 0.05f;
		ShotsFired++;
		var tr = Scene.Trace.Ray(ray, 5000).WithoutTags("player").Run();
		if (tr.Hit)
		{
		var decalVar = decal.Clone( tr.HitPosition + tr.Normal * 2.0f, Rotation.LookAt(-tr.Normal));
		impactEffect.Clone(tr.HitPosition, Rotation.LookAt(-tr.Normal));
		tr.GameObject.Parent.Components.TryGet<Zombie>(out var zombie);
		Ammo += 5;
		if (zombie is not null)
		{
			zombie.TakeDamage(10, playerController);
		}
		//decalVar.Parent = tr.GameObject;
		}
	}

	void Animations()
	{
		if (IsProxy) return;
		if (Input.Pressed("jump") && !IsProxy)
		{
			gun.Set("b_jump", true);
		}
		if (!characterController.IsOnGround && !IsProxy)
		{
			gun.Set("b_grounded", false);
		}
		else
		{
			gun.Set("b_grounded", true);
		}
	}
}
