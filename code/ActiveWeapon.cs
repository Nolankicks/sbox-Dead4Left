using System.Linq;
using Kicks;
using Sandbox;

public sealed class ActiveWeapon : Component
{
	[Property, ResourceType("item")] public Item Item { get; set; }
	[Property] public SkinnedModelRenderer body { get; set; }
	public GameObject weaponref;
	public GameObject viewmodelref;
	public bool NeedsChange = true;
	protected override void OnUpdate()
	{
		if (NeedsChange && Item != null)
		{
		if (Item is null) return;
		//Get Prefabs
		var weapon = SceneUtility.GetPrefabScene(Item.weaponPrefab);
		var modularWeapon = weapon?.Components.Get<ModularWeapon>();
		//Set values from Item resource

		//Clone Prefabs
		var weaponClone = weapon?.Clone();
		//Create Refs
		weaponref = weaponClone;
		
		//Set Parent
		weaponClone?.SetParent(body.GameObject);
		
		//Set Local Position
		NeedsChange = false;


		}
	}
}
