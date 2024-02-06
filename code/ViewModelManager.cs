using System.Numerics;
using Sandbox;

public sealed class ViewModelManager : Component
{
	[Property] SkinnedModelRenderer weaponRender {get; set;}
	[Property] public SkinnedModelRenderer armRender { get; set; }
	[Property] Model mp5Model { get; set; }
	[Property] Model shotgunModel { get; set; }
	[Property] Model fistsModel { get; set; }
	[Property] Weapon weapon { get; set; }
	private Vector3 cameraStartPos;
	[Property] public AnimationGraph punchGraph { get; set; }
	[Property] public AnimationGraph normalGraph { get; set; }
	private CameraComponent viewModelCamera;
	public Model blankModel;
	protected override void OnAwake()
	{
		viewModelCamera = Components.GetInParent<CameraComponent>();
		cameraStartPos = viewModelCamera.GameObject.Transform.LocalPosition;
	}

	protected override void OnUpdate()
	{
		if (weapon.Inventory[weapon.ActiveSlot] != "weapon_smg" && weapon.Inventory[weapon.ActiveSlot] != "weapon_shotgun")
		{
			ShowFists();
		}
		if (weapon.Inventory[weapon.ActiveSlot] == "weapon_smg")
		{
			ShowSmg();
		}
		if (weapon.Inventory[weapon.ActiveSlot] == "weapon_shotgun")
		{
			ShowShotgun();
		}


		
	}
	void ShowSmg()
	{

			weaponRender.Enabled = true;
			armRender.BoneMergeTarget = weaponRender;
			weaponRender.Model = mp5Model;
			armRender.Enabled = true;
			viewModelCamera.Transform.LocalPosition = cameraStartPos;

	}

	void ShowFists()
	{
		armRender.SceneModel.AnimationGraph = punchGraph;
			armRender.BoneMergeTarget = null;
			weaponRender.Enabled = false;
			viewModelCamera.GameObject.Transform.LocalPosition = new Vector3(15, 0, -5);
	}

	void ShowShotgun()
	{
			weaponRender.Enabled = true;
			armRender.BoneMergeTarget = weaponRender;
			weaponRender.Model = shotgunModel;
			armRender.Enabled = true;
			viewModelCamera.Transform.LocalPosition = cameraStartPos;
	}
}
