using System;
using System.Linq;
using Kicks;
using Sandbox;
using Sandbox.Citizen;
using Sandbox.ModelEditor.Nodes;

public sealed class ModularWeapon : Component
{
	
	CitizenAnimationHelper animationHelper;
	[Property] public GameObject impact { get; set; }
	private PlayerController playerController;
	[Property] public SoundEvent gunSound { get; set; }
	[Property] public SoundEvent reloadSound { get; set; }
	[Property] public GameObject decalGo { get; set; }
	public float ammo;
	public float fullAmmo;
	[Property] public GameObject zombieRagdoll { get; set; }
	[Property] public GameObject zombieParticle { get; set; }
	[Property] public SkinnedModelRenderer gun { get; set; }
	[Property] public SkinnedModelRenderer arms { get; set; }
	public float weaponDamage;
	
	//[Property] public Weapon weapon { get; set; }
	public TimeSince timeSinceShoot = 0;
	public float fireRate;
	public ActiveWeapon viewmodel;
	Manager manager;
	Weapon weapon;
	float AmmoNeeded = 30;
	Vector3 startPos;
	CharacterController characterController;
	public ActiveWeapon ActiveWeapon;
	protected override void OnStart()
	{
		//networking
		animationHelper = GameObject.Components.GetInDescendantsOrSelf<CitizenAnimationHelper>();
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
		manager = GameManager.ActiveScene.GetAllComponents<Manager>().FirstOrDefault();
		viewmodel = GameManager.ActiveScene.GetAllComponents<ActiveWeapon>().FirstOrDefault(x => !x.IsProxy);
		GameObject.Transform.LocalPosition = new Vector3(3.302f, -7.1f, 63.7f);
		gun.RenderType = ModelRenderer.ShadowRenderType.Off;
		arms.RenderType = ModelRenderer.ShadowRenderType.Off;
		startPos = GameObject.Transform.LocalPosition;
		characterController = GameManager.ActiveScene.GetAllComponents<CharacterController>().FirstOrDefault(x => !x.IsProxy);
		ActiveWeapon = GameManager.ActiveScene.GetAllComponents<ActiveWeapon>().FirstOrDefault(x => !x.IsProxy);
		var item = ActiveWeapon.Item;
		ammo = item.ammo;
		fullAmmo = item.fullAmmo;
		fireRate = item.fireRate;
		weaponDamage = item.weaponDamage;
		gun.Model = item.weaponModel;
		Log.Info(weaponDamage);


		ammo = float.Parse(FileSystem.Data.ReadAllText("mp5ammo.txt"));
		fullAmmo = float.Parse(FileSystem.Data.ReadAllText("mp5maxammo.txt"));

	}
	bool ableToShoot;
	bool reloading;
	public TimeSince timeSinceReload = 3;

	protected override void OnUpdate()
	{
		FileSystem.Data.WriteAllText("mp5ammo.txt", ammo.ToString());
		FileSystem.Data.WriteAllText("mp5maxammo.txt", fullAmmo.ToString());
		Animations();
		if (IsProxy) return;
		GameObject.Transform.Rotation = Rotation.Lerp(GameObject.Transform.Rotation, playerController.eye.Transform.Rotation, Time.Delta * 100);
		if (fullAmmo < 0)
		{
			fullAmmo = 0;
		}

					var crouchVector = new Vector3(0, 0, 32);
			
		var target = startPos - (Input.Down( "duck" ) ? crouchVector : 0);
		Transform.LocalPosition = Transform.LocalPosition.LerpTo( target, Time.Delta * 10f );
		
		
		if (Input.Pressed("reload") && fullAmmo != 0 && ammo != 30)
		{
			fullAmmo -= AmmoNeeded;
			ammo = 30;
			timeSinceReload = 1;
			Sound.Play(reloadSound, GameObject.Transform.Position);
			gun.Set("b_reload", true);
		}

		if (IsProxy)
		{
			gun.GameObject.Enabled = false;
			arms.GameObject.Enabled = false;
		}
		if (playerController.Health <= 0)
		{
			fullAmmo = 60;
			ammo = 30;
		}
	}
	protected override void OnFixedUpdate()
	{
		if (IsProxy) return;
		
		if (Input.Down("attack1") && ammo > 0 && timeSinceReload > 3 && timeSinceShoot > fireRate)
		{
			Shoot();
			timeSinceShoot = 0;
			




	}
	void Shoot()
	{
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var ray = Scene.Camera.ScreenNormalToRay( 0.5f );
		var tr = Scene.Trace.Ray( eyePos, eyePos + playerController.EyeAngles.Forward * 8000).WithoutTags("player").Run();
		gun.Set("b_attack", true);
	
		if (tr.Hit)
		{
			ammo -= 1;
			AmmoNeeded -= 1;
			Log.Info(tr.GameObject);
			var impactClone = impact.Clone(tr.HitPosition);
			Sound.Play(gunSound, GameObject.Transform.Position);
			//var decal = decalGo.Clone(new Transform(tr.HitPosition + tr.Normal * 2.0f, Rotation.LookAt(-tr.Normal, Vector3.Random), Random.Shared.Float(0.8f, 1.2f)));
			//decal.SetParent( tr.GameObject );
			//decal.NetworkSpawn();
			impactClone.NetworkSpawn();
			if (tr.GameObject.Tags.Has("bad"))
			{
				var zombie = tr.GameObject.Parent.Components?.Get<Zombie>();
				zombie.TakeDamage(weaponDamage);
				if (zombie.Health <= 0) playerController.AddScore(5);
				fullAmmo += 15;
				var zombieParticleClone = zombieParticle.Clone(tr.HitPosition);
				//var zombieRagdollClone = zombieRagdoll.Clone(tr.GameObject.Transform.Position, tr.GameObject.Transform.Rotation);
				zombieParticleClone.NetworkSpawn();
				//zombieRagdollClone.NetworkSpawn();
			}
			var damage = new DamageInfo( weaponDamage, GameObject, GameObject );
			
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

void Animations()
{
	if (characterController.IsOnGround)
	{
		gun.Set("b_grounded", true);
	}
	else
	{
		gun.Set("b_grounded", false);
	}

	if (Input.Pressed("jump"))
	{
		gun.Set("b_jump", true);
	}
	
}
}
