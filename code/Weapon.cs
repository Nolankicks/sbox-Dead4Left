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
	public bool AbleToSwitch = true;
	[Property] public PrefabScene weaponPrefab { get; set; }
	[Property] public PrefabScene itemPrefab { get; set; }
	[Property] public GameObject LastWeapon;
	[Property] public GameObject CurrentWeapon;
	protected override void OnStart()
	{
		if (IsProxy) return;
		var weaponList = ResourceLibrary.GetAll<WeaponData>();
		Inventory[0] = weaponList.FirstOrDefault(x => x.Name == "MP5");
		Inventory[1] = weaponList.FirstOrDefault(x => x.Name == "pistol");
		Inventory[2] = weaponList.FirstOrDefault(x => x.Name == "healthkit");
		//Inventory[3] = weaponList.FirstOrDefault(x => x.Name == "shotgun");
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
		if (Input.MouseWheel.y != 0 && !IsProxy && AbleToSwitch)
		{
			ActiveSlot = (ActiveSlot + Math.Sign(Input.MouseWheel.y)) % Inventory.Length;
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
			if (weapon.IsWeapon)
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
			if (weapon.IsItem)
			{
			var gameObj = itemPrefab.Clone();
			var HealthKit = gameObj.Components.Get<HealthKit>();
			HealthKit.PatchUptime = weapon.PatchUptime;
			gameObj.Name = weapon.Name;
			gameObj.Parent = GameObject;
			HealthKit.weaponData = weapon;
			Log.Info(weapon.Name);
			WeaponList[Array.IndexOf(Inventory, weapon)] = gameObj;
			}
			if (weapon.IsCustom)
			{
				var gameObj = weapon.CustomPrefab.Clone();
				gameObj.Name = weapon.Name;
				gameObj.Parent = GameObject;
				WeaponList[Array.IndexOf(Inventory, weapon)] = gameObj;
			}
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
	[Property] public SoundEvent ShootSound { get; set; }
	private PlayerController playerController;
	private CharacterController characterController;
	public TimeSince timeSinceReload = 1.5f;
	public Weapon weapon;
	[Property] public GameObject bloodParticle { get; set; }
	[Property] public GameObject muzzleFlash { get; set; }
		protected override void OnStart()
		{
			if (IsProxy) return;
			playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
			characterController = GameManager.ActiveScene.GetAllComponents<CharacterController>().FirstOrDefault(x => !x.IsProxy);
			weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
			gun.Model = data.WeaponModel;
			gun.Set("b_deploy", true);
			Ammo = data.Ammo;
			MaxAmmo = data.MaxAmmo;
			Damage = data.Damage;
			FireRate = data.FireRate;
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
			gun.Set("move_groundspeed", characterController.Velocity.Length);
			
			
		}
		/*protected override void OnDestroy()
		{
			if (!IsProxy)
			{
			weapon.WeaponList[Array.IndexOf(weapon.WeaponList, GameObject)] = null;
			}
		}*/
		void Shoot()
	{
		if (IsProxy) return;
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var tr = Scene.Trace.Ray(eyePos, eyePos + playerController.EyeAngles.Forward * 5000).WithoutTags("player").Run();
		Ammo--;
		ShotsFired++;
		Log.Info(ShotsFired);
		var muzzle = gun.SceneModel.GetAttachment("muzzle");
		var sound = Sound.Play(ShootSound);
		sound.SpacialBlend = 0;
		muzzleFlash.Clone(muzzle.Value.Position, new Angles(0, playerController.EyeAngles.yaw, 0));
		if (tr.Hit)
		{
			
			
			if (tr.GameObject.Tags.Has("bad"))
			{
				tr.GameObject.Destroy();
				playerController.AddScore(5);
				Log.Info(tr.GameObject.Parent);
				var blood = bloodParticle.Clone(tr.HitPosition);
				blood.NetworkSpawn();
				Ammo += 5;
			}
		}

	}
}
[GameResource( "Weapon", "weapon", "A item game resource", Icon = "track_changes") ]
public partial class WeaponData : GameResource
{
	public bool IsWeapon { get; set; }
	public bool IsItem { get; set; }
	public bool IsCustom { get; set; }
    public string Name { get; set; } = "";
	[Property, Category("Weapon"), ShowIf("IsWeapon", true)] public int Ammo { get; set; }
    [Property, Category("Weapon"), ShowIf("IsWeapon", true)] public int MaxAmmo { get; set; } = 32;
	[Property, Category("Weapon"), ShowIf("IsWeapon", true)] public int Damage { get; set; } = 50;
    [Property, Category("Weapon"), ShowIf("IsWeapon", true)] public float FireRate { get; set; } = 0.1f;
	[Property, Category("Weapon"), ShowIf("IsWeapon", true)] public Model WeaponModel { get; set; }
	[Property, Category("Weapon"), ShowIf("IsWeapon", true)] public SoundEvent ShootSound { get; set; }
	[Property, Category("Item"), ShowIf("IsItem", true)] public int PatchUptime { get; set; } = 5;
	[Property, Category("Custom"), ShowIf("IsCustom", true)] public int Count { get; set; } = 1;
	[Property, Category("Custom"), ShowIf("IsCustom", true)] public PrefabScene CustomPrefab { get; set; }
	public Texture InventoryImage { get; set; }

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

