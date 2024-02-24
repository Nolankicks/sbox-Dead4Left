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
			
		}
		if (ActiveSlot < 0)
		{
			ActiveSlot = Slots - 1;
		}
}

public partial class WeaponFunction : Component
{
	 public int Ammo { get; set; }

    private WeaponData _data;

    public void Init( WeaponData data )
    {
        _data = data;

        Ammo = _data.MaxAmmo;
    }
	private PlayerController playerController;
		protected override void OnStart()
		{
			playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		}
		protected override void OnUpdate()
		{
			if (Input.Pressed("attack1"))
			{
				Shoot();
				Log.Info(Ammo);
			}
		}

	void Shoot()
	{
		var eyePos = Input.Down("duck") ? playerController.Transform.Position + Vector3.Up * 32 : playerController.Transform.Position + Vector3.Up * 64;
		var tr = Scene.Trace.Ray(eyePos, eyePos + playerController.EyeAngles.Forward * 5000).WithoutTags("player").Run();
		Ammo--;
		if (tr.Hit)
		{
			Log.Info("Hit " + tr.GameObject.Name);
			if (tr.GameObject.Tags.Has("zombie"))
			{

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
    public float FireRate { get; set; } = 0.1f;
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
			if (weapon.NeedsChange && weapon.Inventory[weapon.ActiveSlot] != null)
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

