using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Kicks;
using Sandbox;

public sealed class ViewModelManager : Component
{
	[Property] SkinnedModelRenderer weaponRender {get; set;}
	[Property] public SkinnedModelRenderer armRender { get; set; }
	[Property] Model mp5Model { get; set; }
	[Property] Model shotgunModel { get; set; }
	[Property] Model fistsModel { get; set; }
	[Property] public GameObject eye { get; set; }
	private Weapon weapon;
	private Vector3 StartPos;
	[Property] public AnimationGraph punchGraph { get; set; }
	[Property] public AnimationGraph normalGraph { get; set; }
	PlayerController playerController;
	public Model blankModel;
	protected override void OnAwake()
	{
		StartPos = GameObject.Transform.LocalPosition;
		
	}
	protected override void OnStart()
	{
		weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		
		var eyeRot = eye.Transform.LocalRotation;
		GameObject.Transform.Rotation = playerController.EyeAngles;

		if (weapon.Inventory[weapon.ActiveSlot] != "weapon_smg")
		{
			ShowFists();
		}
		else
		{
			ShowSmg();
		}
		foreach(var arms in GameObject.Components.GetAll<ModelRenderer>(FindMode.EverythingInSelfAndDescendants))
		{
			if (IsProxy)
			{
				GameObject.Enabled = false;
			}
			else
			{
				GameObject.Enabled = true;
			}
		}
	}
	void ShowSmg()
	{

			weaponRender.Enabled = true;
			armRender.BoneMergeTarget = weaponRender;
			weaponRender.Model = mp5Model;
			armRender.Enabled = true;
			GameObject.Transform.LocalPosition = StartPos;
	}

	void ShowFists()
	{
		armRender.SceneModel.AnimationGraph = punchGraph;
		armRender.BoneMergeTarget = null;
		weaponRender.Enabled = false;
		GameObject.Transform.LocalPosition = new Vector3(StartPos.x, 0, StartPos.z - 2);
	}

	
}
