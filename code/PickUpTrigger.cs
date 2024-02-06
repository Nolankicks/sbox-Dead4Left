using System.Linq;
using Sandbox;

public sealed class PickUpTrigger : Component, Component.ITriggerListener
{
	[Property] public Weapon weapon {get; set;}
	[Property] public SoundEvent pickupSound {get; set;}
	[Property, ToggleGroup("Item")] public bool SpawnShotgun {get; set;} = false;
	[Property, ToggleGroup("Item")] public bool SpawnSmg {get; set;} = false;
	private string itemToPlace;
	protected override void OnAwake()
	{
		if (SpawnShotgun)
		{
			itemToPlace = "weapon_shotgun";
		}
		if (SpawnSmg)
		{
			itemToPlace = "weapon_smg";
		}
	}

	void ITriggerListener.OnTriggerEnter(Sandbox.Collider other)
	{
		if (other.Tags.Has("player") && !weapon.Inventory.Contains(itemToPlace))
		{
		for (int i = 0; i < weapon.Inventory.Length; i++) {
    	if (string.IsNullOrEmpty(weapon.Inventory[i])) {
        weapon.Inventory[i] = itemToPlace;
		Sound.Play(pickupSound, GameObject.Transform.Position);
        break;
    }
	}
	}
	}

	void ITriggerListener.OnTriggerExit(Sandbox.Collider other)
	{
		return;
	}
}
