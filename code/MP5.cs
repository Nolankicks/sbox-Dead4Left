using System;
using System.Linq;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class MP5 : Component
{
	
	CitizenAnimationHelper animationHelper;
	[Property] public GameObject impact { get; set; }
	private PlayerController playerController;
	[Property] public SoundEvent gunSound { get; set; }
	[Property] public SoundEvent reloadSound { get; set; }
	[Property] public GameObject decalGo { get; set; }
	[Property] public float ammo { get; set; } = 30;
	[Property] public float fullAmmo { get; set; } = 60;
	[Property] public float ShootDamage { get; set; } = 10;
	[Property] public GameObject zombieRagdoll { get; set; }
	[Property] public GameObject zombieParticle { get; set; }
	//[Property] public Weapon weapon { get; set; }
	public TimeSince timeSinceShoot = 0;
	public NetworkedViewmodel viewmodel;
	Manager manager;
	Weapon weapon;
	PlayerController player;
	float AmmoNeeded = 30;
	protected override void OnStart()
	{
		//networking
		animationHelper = GameObject.Components.GetInDescendantsOrSelf<CitizenAnimationHelper>();
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
		manager = GameManager.ActiveScene.GetAllComponents<Manager>().FirstOrDefault();
		player = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		viewmodel = GameManager.ActiveScene.GetAllComponents<NetworkedViewmodel>().FirstOrDefault(x => !x.IsProxy);
	}
	bool ableToShoot;
	bool reloading;
	public TimeSince timeSinceReload = 3;

	protected override void OnUpdate()
	{

		if (IsProxy) return;
		if (fullAmmo < 0)
		{
			fullAmmo = 0;
		}
		
		if (Input.Pressed("reload") && fullAmmo != 0 && ammo != 30)
		{
			fullAmmo -= AmmoNeeded;
			ammo = 30;
			timeSinceReload = 1;
			Sound.Play(reloadSound, GameObject.Transform.Position);
			//var gun = viewmodel.gun;
			//gun.Set("b_reload", true);
		}

	}
	protected override void OnFixedUpdate()
	{
		if (IsProxy) return;
		if (Input.Down("attack1") && ammo > 0 && timeSinceReload > 3 && timeSinceShoot > 0.1)
		{
			//var gun = viewmodel.gun;
			Shoot();
			timeSinceShoot = 0;
			//gun.Set("b_attack", true);
		}
	}
	void Shoot()
	{
		var eyePos = playerController.Transform.Position + Vector3.Up * 64;
		var ray = Scene.Camera.ScreenNormalToRay( 0.5f );
		var tr = Scene.Trace.Ray( eyePos, eyePos + playerController.EyeAngles.Forward * 8000).WithoutTags("player").Run();
		
		if (tr.Hit)
		{
			
			ammo -= 1;
			AmmoNeeded -= 1;
			Log.Info(tr.GameObject);
			var impactClone = impact.Clone(tr.HitPosition);
			Sound.Play(gunSound, GameObject.Transform.Position);
			var decal = decalGo.Clone(new Transform(tr.HitPosition + tr.Normal * 2.0f, Rotation.LookAt(-tr.Normal, Vector3.Random), Random.Shared.Float(0.8f, 1.2f)));
			decal.SetParent( tr.GameObject );
			decal.NetworkSpawn();
			impactClone.NetworkSpawn();
			if (tr.GameObject.Tags.Has("bad"))
			{
				tr.GameObject.Parent.Destroy();
				player.AddScore(5);
				fullAmmo += 15;
				var zombieParticleClone = zombieParticle.Clone(tr.HitPosition);
				var zombieRagdollClone = zombieRagdoll.Clone(tr.GameObject.Transform.Position, tr.GameObject.Transform.Rotation);
				zombieParticleClone.NetworkSpawn();
				zombieRagdollClone.NetworkSpawn();
			}
			var damage = new DamageInfo( ShootDamage, GameObject, GameObject );
			
		if ( tr.Body is not null )
		{
			tr.Body.ApplyImpulseAt( tr.HitPosition, tr.Direction * 200.0f * tr.Body.Mass.Clamp( 0,200 ) );
		}

		foreach ( var damageable in tr.GameObject.Components.GetAll<IDamageable>() )
		{
			damageable.OnDamage( damage );
		}
		}
	}
}
