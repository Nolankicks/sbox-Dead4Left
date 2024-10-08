using System;
using System.Linq;
using Kicks;
using Sandbox;
using static Weapon;
[Category("Custon Weapon"), Icon("track_changes"), Title("Shotgun"), Description("A shotgun.")]
public sealed class Shotgun : Component
{
	[Property, Category("ModelRenders")] public SkinnedModelRenderer gun { get; set; }
	[Property, Category("ModelRenders")] public SkinnedModelRenderer arms { get; set; }
	[Property, Category("Weapon Data")] public int Ammo { get; set; } = 16;
	[Property, Category("Weapon Data")] public int MaxAmmo { get; set; } = 32;
	[Property, Category("Weapon Data")] public int Damage { get; set; } = 10;
	[Property, Category("Particles and Decals")] public GameObject decal { get; set; }
	[Property, Category("Particles and Decals")] public GameObject muzzleFlash { get; set; }
	[Property, Category("Particles and Decals")] public GameObject impactEffect { get; set; }
	[Property, Category("Particles and Decals")] public SoundEvent shootSound { get; set; }
	public int ShotsFired = 0;
	private PlayerController playerController;
	private CharacterController characterController;
	public float FireRate { get; set; } = 0.5f;
	public int fullAmmo;
	public TimeSince timeSinceShoot;
	public TimeSince timeSinceReload = 1.5f;
	public Weapon weapon;
	protected override void OnStart()
	{
		if (IsProxy) return;
		playerController = Game.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		characterController = Game.ActiveScene.GetAllComponents<CharacterController>().FirstOrDefault(x => !x.IsProxy);
		weapon = Game.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
		timeSinceShoot = FireRate;

		if (gun.IsValid())
			gun.Set("b_deploy", true);

		fullAmmo = Ammo;
	}
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		Animations();
		Log.Info(MaxAmmo);
		if (Input.Down("attack1") && !IsProxy && timeSinceShoot >= FireRate && timeSinceReload >= 1.5f && Ammo != 0)
		{
			for (int i = 0; i < 4; i++)
			{
			Shoot();
			}
			gun.Set("b_attack", true);
			
			timeSinceShoot = 0;
		if (Input.UsingController)
		{
			Input.TriggerHaptics( 0.3f, 0.3f );
		}
		}
		if (Input.Pressed("reload") && MaxAmmo != 0 && ShotsFired != 0 && MaxAmmo >= ShotsFired && !IsProxy)
		{
				Ammo = MaxAmmo -= ShotsFired;
				Ammo = fullAmmo;
				gun.Set("b_reload", true);
				ShotsFired = 0;
				timeSinceReload = 0;
		if (Input.UsingController)
		{
			Input.TriggerHaptics( 0.1f, 0.1f );
		}
		}
	}

	void Shoot()
	{
		if (IsProxy) return;
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var ray = Scene.Camera.ScreenNormalToRay( 0.5f );
		ray.Forward += Vector3.Random * 0.05f;
		ShotsFired++;
		Ammo--;
		var tr = Scene.Trace.Ray(ray, 5000).WithoutTags("player").Run();
		var sound = Sound.Play(shootSound, tr.StartPosition);
		sound.Volume = 0.1f;

	

		if (tr.Hit)
		{
		//var decalVar = decal.Clone( tr.HitPosition + tr.Normal * 2.0f, Rotation.LookAt(-tr.Normal));
		impactEffect.Clone(tr.HitPosition, Rotation.LookAt(-tr.Normal));
		tr.GameObject.Parent.Components.TryGet<Zombie>(out var zombie);
		if (zombie is not null)
		{
			zombie.TakeDamage(Damage, GameObject.Parent.Id);
			MaxAmmo += 5;
		}
		var damage = new DamageInfo(Damage, GameObject, tr.GameObject);
		if (damage is not null)
		{
		foreach(var damageable in tr.GameObject.Components.GetAll<IDamageable>())
		{
			damageable.OnDamage(damage);
		}
		}
		if (tr.Body is not null)
		{
			tr.Body.ApplyImpulseAt(tr.HitPosition, tr.Direction * 200.0f * tr.Body.Mass.Clamp(0, 200));
		}
		//decalVar.Parent = tr.GameObject;
		}
	}

	void Animations()
	{
		if (IsProxy || !gun.IsValid() || !characterController.IsValid()) return;
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
