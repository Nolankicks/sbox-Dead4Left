using Sandbox;

public sealed class ActiveWeapon : Component
{
	[Property] public Item Item { get; set; }
	[Property] public SkinnedModelRenderer arms { get; set; }
	public GameObject weaponref;
	public bool NeedsChange = true;
	protected override void OnUpdate()
	{
		Log.Info(NeedsChange);
		if (NeedsChange)
		{
		if (Item is null) return;
		var weapon = SceneUtility.GetPrefabScene(Item.weaponPrefab);
		var weaponClone = weapon.Clone(GameObject.Transform.LocalPosition);
		weaponref = weaponClone;
		weaponClone.SetParent(arms.GameObject);
		//var modelRenderer = weaponClone.Components.Get<SkinnedModelRenderer>();
		//arms.BoneMergeTarget = modelRenderer;
		NeedsChange = false;
		}
	}


}
