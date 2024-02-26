using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using Kicks;
using Sandbox;
using Sandbox.UI;
using Sandbox.Citizen;
public sealed class Weapon : Component
{	
	[Property] public List<Texture> InventoryImages {get; set;} = new List<Texture>()
	{

	};
	
	public int ActiveSlot = 0;
	public int Slots => 9;
	public int Img => 9;
	public bool NeedsChange = true;
	public WeaponData[] Inventory = new WeaponData[9];
	public GameObject[] WeaponList = new GameObject[9];
	[Property] public PrefabScene weaponPrefab { get; set; }
	[Property] public GameObject LastWeapon;
	[Property] public GameObject CurrentWeapon;
	protected override void OnStart()
	{
		if (IsProxy) return;
		var weaponList = ResourceLibrary.GetAll<WeaponData>();
		Inventory[0] = weaponList.FirstOrDefault(x => x.Name == "MP5");
		Inventory[1] = weaponList.FirstOrDefault(x => x.Name == "pistol");
	}
	public void AddWeapon(WeaponData weapon, int slot)
	{
		Inventory[slot] = weapon;
		NeedsChange = true;
	}
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		CurrentWeapon = WeaponList[ActiveSlot];
		if (Input.MouseWheel.y != 0 && !IsProxy)
		{
			ActiveSlot = (ActiveSlot + Math.Sign(Input.MouseWheel.y)) % Inventory.Length;
			LastWeapon = WeaponList[ActiveSlot + 1];
		}
		if (CurrentWeapon == LastWeapon && !IsProxy)
		{
			LastWeapon = null;
		}
		if (ActiveSlot < 0 && !IsProxy)
		{
			ActiveSlot = 8;
		}
		if (NeedsChange && !IsProxy)
		{
		foreach (WeaponData weapon in Inventory)
		{
			if (weapon is not null)
			{
			var gameObj = weaponPrefab.Clone();
			var weaponData = gameObj.Components.Get<WeaponFunction>();
			weaponData.data = weapon;
			gameObj.Name = weapon.Name;
			Log.Info(weapon.Name);
			WeaponList[Array.IndexOf(Inventory, weapon)] = gameObj;
			Log.Info(WeaponList.ToString());
			gameObj.Parent = GameObject;
			}
		}
		NeedsChange = false;
		}
		foreach (GameObject weapon in WeaponList)
		{
			if (weapon is not null && weapon == WeaponList[ActiveSlot] && !IsProxy)
			{
				weapon.Enabled = true;
			}
			if (weapon is not null && weapon != WeaponList[ActiveSlot] && !IsProxy)
			{
				weapon.Enabled = false;
			}

		}
		
}


public partial class WeaponFunction : Component
{
	
	[Property] public SkinnedModelRenderer gun { get; set; }
    [Property] public WeaponData data;
	private TimeSince _lastFired = 0;
	public int ShotsFired = 0;
	[Property] public int Ammo;
	[Property] public int MaxAmmo;
	public int Damage;
	public float FireRate;
	public SoundEvent ShootSound;
	private PlayerController playerController;
	private CharacterController characterController;
	public TimeSince timeSinceReload = 1.5f;
	[Property] public GameObject bloodParticle { get; set; }
	[Property] public GameObject muzzleFlash { get; set; }
		protected override void OnStart()
		{
			if (IsProxy) return;
			playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
			characterController = GameManager.ActiveScene.GetAllComponents<CharacterController>().FirstOrDefault(x => !x.IsProxy);
			gun.Model = data.WeaponModel;
			gun.Set("b_deploy", true);
			Ammo = data.Ammo;
			MaxAmmo = data.MaxAmmo;
			Damage = data.Damage;
			FireRate = data.FireRate;
			ShootSound = data.ShootSound;
		}
		protected override void OnUpdate()
		{
			if (IsProxy) return;
			if (Input.Down("attack1") && Ammo > 0 && _lastFired > FireRate && timeSinceReload > 1.5f && !IsProxy)
			{
				Shoot();
				gun.Set("b_attack", true);
				_lastFired = 0;
			}

			if (Ammo < 0)
			{
				Ammo = 0;
			}
			if (MaxAmmo < 0)
			{
				MaxAmmo = 0;
			}

			if (Input.Pressed("reload") && MaxAmmo != 0 && ShotsFired != 0 && !IsProxy)
			{
				
				Ammo = MaxAmmo -= ShotsFired;
				Ammo = data.Ammo;
				gun.Set("b_reload", true);
				ShotsFired = 0;
				timeSinceReload = 0;
			}
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

	void Shoot()
	{
		if (IsProxy) return;
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var tr = Scene.Trace.Ray(eyePos, eyePos + playerController.EyeAngles.Forward * 5000).WithoutTags("player").Run();
		Ammo--;
		ShotsFired++;
		Log.Info(ShotsFired);
		var muzzle = gun.SceneModel.GetAttachment("muzzle");

		muzzleFlash.Clone(muzzle.Value.Position + Vector3.Up * 62, playerController.EyeAngles);
		if (tr.Hit)
		{
			Sound.Play(ShootSound, tr.EndPosition);
			if (tr.GameObject.Tags.Has("bad"))
			{
				var zombie = tr.GameObject.Parent.Components.Get<Zombie>();
				zombie.TakeDamage(Damage);
				Log.Info(tr.GameObject.Parent);
				var blood = bloodParticle.Clone(tr.HitPosition);
				blood.NetworkSpawn();
				Ammo += 5;
			}
		}

	}
}
[GameResource( "Weapon", "weapon", "A item game resource", Icon = "track_changes" ) ]
public partial class WeaponData : GameResource
{
    public string Name { get; private set; } = "MP5";
    public int MaxAmmo { get; private set; } = 32;
    public int Ammo { get; set; }
	public int Damage { get; set; } = 50;
    public float FireRate { get; set; } = 0.1f;
	public Model WeaponModel { get; set; }
	public SoundEvent ShootSound { get; set; }
}

public partial class Switcher : Component
{
	[Property] public GameObject weaponPrefab { get; set; }
	WeaponData[] _weapons;
	GameObject _currentlyEquiped;
	public Weapon weapon;
		protected override void OnStart()
		{

		}

		protected override void OnUpdate()
		{

		}
	}
}

