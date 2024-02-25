using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using Kicks;
using Sandbox;
using Sandbox.UI;

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
	}
	public void AddWeapon(WeaponData weapon, int slot)
	{
		Inventory[slot] = weapon;
		NeedsChange = true;
	}
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		if (Input.MouseWheel.y != 0)
		{
			ActiveSlot = (ActiveSlot + Math.Sign(Input.MouseWheel.y)) % Inventory.Length;
			
			if (WeaponList[ActiveSlot + Math.Sign(Input.MouseWheel.y)] is not null)
			{
				LastWeapon = WeaponList[ActiveSlot + 1];
			}
		}
		if (ActiveSlot < 0)
		{
			ActiveSlot = Inventory.Length - 1;
		}
		if (NeedsChange)
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
	
		protected override void OnStart()
		{
			if (IsProxy) return;
			playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
			gun.Model = data.WeaponModel;
			gun.Set("b_deploy", true);
			Ammo = data.MaxAmmo;
			MaxAmmo = data.MaxAmmo;
			Damage = data.Damage;
			FireRate = data.FireRate;
			ShootSound = data.ShootSound;
		}
		protected override void OnUpdate()
		{
			if (IsProxy) return;
			if (Input.Down("attack1") && Ammo > 0 && _lastFired > FireRate)
			{
				Log.Info("Shooting");
				Shoot();
				gun.Set("b_attack", true);
				_lastFired = 0;
				Sound.Play(ShootSound);
			}

			if (Ammo < 0)
			{
				Ammo = 0;
			}

			if (Input.Pressed("reload") && MaxAmmo != 0 && ShotsFired > 0)
			{
				var ammoNeeded = MaxAmmo - ShotsFired;
				Ammo = ammoNeeded;
				gun.Set("b_reload", true);
			}
		}

	void Shoot()
	{
		if (IsProxy) return;
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var tr = Scene.Trace.Ray(eyePos, eyePos + playerController.EyeAngles.Forward * 5000).WithoutTags("player").Run();
		Ammo--;
		ShotsFired++;
		if (tr.Hit)
		{
			Log.Info("Hit " + tr.GameObject.Name);
			if (tr.GameObject.Tags.Has("zombie"))
			{
				var zombie = tr.GameObject.Components.Get<Zombie>();
				zombie.Health -= Damage;
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

