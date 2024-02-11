using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using Sandbox;
using Sandbox.UI;

public sealed class Weapon : Component
{
	[Property] public Image WeaponImage {get; set;}
	public bool HasViewModel;
	
	[Property] public List<Texture> InventoryImages {get; set;} = new List<Texture>()
	{

	};
	[Property] public List<string> Weapons {get; set;} = new List<string>()
	{
		"weapon_smg"
	};
	[Property] public List<Item> items {get; set;} = new List<Item>()
	{
		
	};
	
	public int ActiveSlot = 0;
	public int Slots => 9;
	public int Img => 9;
	public string[] Inventory = new string[9];
	ActiveWeapon activeWeapon;
	protected override void OnStart()
	{
		activeWeapon = GameManager.ActiveScene.GetAllComponents<ActiveWeapon>().FirstOrDefault( x => !x.IsProxy);
	}
	protected override void OnAwake()
	{
		if (IsProxy) return;
		for (int i = 0; i < Inventory.Length; i++)
		{
			Inventory[i] = "weapon_fists";
		}
		Inventory[0] = "weapon_smg";
		Inventory[1] = "";
		Inventory[2] = "";
		Inventory[3] = "";
		Inventory[4] = "";
		Inventory[5] = "";
		Inventory[6] = "";
		Inventory[7] = "";
		Inventory[8] = "";
	}
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		if (Input.MouseWheel.y != 0)
		{
			ActiveSlot = (ActiveSlot + Math.Sign(Input.MouseWheel.y)) % Inventory.Length;
			activeWeapon.weaponref.Destroy();
			activeWeapon.NeedsChange = true;
		}
		if (ActiveSlot < 0)
		{
			ActiveSlot = Slots - 1;
		}
		

		if (Inventory[ActiveSlot] == "weapon_smg")
		{
			activeWeapon.Item = items[0];
		}
		else
		{
			activeWeapon.Item = null;
		}

		if (Inventory[ActiveSlot] != "")
		{
			HasViewModel = true;
		}

		if (Inventory[ActiveSlot] == "")
		{
			activeWeapon.Item = items[1];
		}
	}
}

