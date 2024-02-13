using System.Linq;
using Kicks;
using Sandbox;

public sealed class ActiveWeapon : Component
{
	[Property] public Item Item { get; set; }
	[Property] public SkinnedModelRenderer body { get; set; }
	public PlayerController playerController;
	public GameObject weaponref;
	public GameObject viewmodelref;
	public Weapon weapon1;
	public bool NeedsChange = true;
	protected override void OnStart()
	{
		weapon1 = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		if (NeedsChange && Item != null)
		{
		if (Item is null) return;
		//Get Prefabs
		var weapon = SceneUtility.GetPrefabScene(Item.weaponPrefab);
		
		//Clone Prefabs
		
		var weaponClone = weapon.Clone();
		//Create Refs
		weaponref = weaponClone;
		
		//Set Parent
		weaponClone.SetParent(body.GameObject);
		
		//Set Local Position
		NeedsChange = false;
		

		}
	}


}
