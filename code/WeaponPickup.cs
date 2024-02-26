using Sandbox;
using Kicks;
using static Weapon;
using System.Linq;
public sealed class WeaponPickup : Component
{
	public Weapon weapon;
	[Property] public WeaponData data { get; set;}
	protected override void OnStart()
	{
		weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
		weapon.AddWeapon(data, 2);
	}
	protected override void OnUpdate()
	{

	}
}
