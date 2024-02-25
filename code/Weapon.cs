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
	public WeaponData[] Inventory = new WeaponData[9];
	[Property] public PrefabScene WeaponPrefab { get; set; }
	[Property] public GameObject WeaponHolder { get; set; }
	public bool NeedsChange = true;
	protected override void OnStart()
	{
		var weaponList = ResourceLibrary.GetAll<WeaponData>();
		Inventory[0] = weaponList.FirstOrDefault(x => x.Name == "MP5");
		
		foreach(WeaponData weapon in Inventory)
		{
			if (weapon is not null)
			{
			var weaponData = WeaponPrefab.Clone().Components.Get<WeaponFunction>();
			weaponData.data = weapon;
			weaponData.GameObject.Parent = WeaponHolder;
			weaponData.GameObject.Name = weapon.Name;
			}
		}
	}
	protected override void OnUpdate()
	{

		if (IsProxy) return;
		if (Input.MouseWheel.y != 0)
		{
			ActiveSlot = (ActiveSlot + Math.Sign(Input.MouseWheel.y)) % Inventory.Length;
			NeedsChange = true;
			Log.Info(Inventory[ActiveSlot]);
		}
		if (ActiveSlot < 0)
		{
			ActiveSlot = Slots - 1;
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
		}
		protected override void OnUpdate()
		{
			if (IsProxy) return;
			if (Input.Down("attack1") && Ammo > 0)
			{
				Log.Info("Shooting");
				Shoot();
				gun.Set("b_attack", true);
				_lastFired = 0;
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
			if (weapon.NeedsChange)
			{

			}
		}
	}
}

