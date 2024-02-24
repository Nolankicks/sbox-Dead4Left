using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
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
	public bool NeedsChange = true;
	protected override void OnStart()
	{
		var weaponList = ResourceLibrary.GetAll<WeaponData>();
		Inventory[0] = weaponList.FirstOrDefault(x => x.Name == "MP5");
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
	
	public SkinnedModelRenderer gun { get; set; }
    private WeaponData _data;
	private TimeSince _lastFired = 0;
	public int ShotsFired = 0;
    public void Init( WeaponData data )
    {
        _data = data;


    }
	private PlayerController playerController;
		protected override void OnStart()
		{
			playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
			gun.Set("b_deploy", true);
			gun.Model = _data.WeaponModel;
		}
		protected override void OnUpdate()
		{
			Log.Info(_data.Ammo);
			if (Input.Pressed("attack1") &&_lastFired > _data.FireRate && _data.Ammo > 0)
			{
				Shoot();
				gun.Set("b_attack", true);
			}

			if (_data.Ammo <= 0)
			{
				_data.Ammo = 0;
			}

			if (Input.Pressed("reload") && _data.Ammo < _data.MaxAmmo && ShotsFired > 0 && _data.Ammo != 0)
			{
				var ammoNeeded = _data.MaxAmmo - ShotsFired;
				_data.Ammo = ammoNeeded;
				gun.Set("b_reload", true);
			}
		}

	void Shoot()
	{
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var tr = Scene.Trace.Ray(eyePos, eyePos + playerController.EyeAngles.Forward * 5000).WithoutTags("player").Run();
		_data.Ammo--;
		ShotsFired++;
		if (tr.Hit)
		{
			Log.Info("Hit " + tr.GameObject.Name);
			if (tr.GameObject.Tags.Has("zombie"))
			{
				var zombie = tr.GameObject.Components.Get<Zombie>();
				zombie.Health -= _data.Damage;
			}
		}

	}
}
[GameResource( "Weapon", "weapon", "A item game resource", Icon = "track_changes" ) ]
public partial class WeaponData : GameResource
{
	
    public string Name { get; set; } = "MP5";
    public int MaxAmmo { get; set; } = 32;
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
			var weapons = ResourceLibrary.GetAll<WeaponData>();
			_weapons = weapons.ToArray();
			weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
			_currentlyEquiped = weaponPrefab.Clone();
			_currentlyEquiped.Parent = GameObject;
		}

		protected override void OnUpdate()
		{
			if (weapon.NeedsChange)
			{
				_currentlyEquiped.Destroy();
				_currentlyEquiped = weaponPrefab.Clone();
				var weaponFunction = _currentlyEquiped.Components.Get<WeaponFunction>();
				weaponFunction.Init(_weapons[weapon.ActiveSlot]);
				_currentlyEquiped.Parent = GameObject;
				weapon.NeedsChange = false;
			}
		}
	}
}

