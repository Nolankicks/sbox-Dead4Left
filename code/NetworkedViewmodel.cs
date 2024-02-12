using System.Linq;
using System.Threading;
using Kicks;
using Sandbox;

public sealed class NetworkedViewmodel : Component
{
	[Property] public SkinnedModelRenderer arms	{ get; set; }
	[Property] public SkinnedModelRenderer gun { get; set; }
	[Property] public GameObject eye { get; set; }
	[Property] public Weapon weapon { get; set; }
	[Property] public AnimationGraph punchGraph { get; set; }
	[Property] public AnimationGraph normalGraph { get; set; }
	[Property] public Model smgModel { get; set; }
	PlayerController playerController;
	public Vector3 StartPos;
	public Vector3 ArmsPos;
	protected override void OnStart()
	{
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault( x => !x.IsProxy);
		StartPos = GameObject.Transform.LocalPosition;
		ArmsPos = new Vector3(3.4f, 6.3f, -2f);
	}
	protected override void OnUpdate()
	{
		var cam = GameManager.ActiveScene.GetAllComponents<CameraComponent>().FirstOrDefault(x => !x.IsProxy);
		
		//Update Viewmodel Position and Rotation
		

		arms.RenderType = ModelRenderer.ShadowRenderType.Off;
		gun.RenderType = ModelRenderer.ShadowRenderType.Off;
		if (weapon.Inventory[weapon.ActiveSlot] == "weapon_smg")
		{
			gun.Model = smgModel;
			arms.BoneMergeTarget = gun;
			arms.SceneModel.AnimationGraph = normalGraph;
			GameObject.Transform.Rotation = Rotation.Lerp(GameObject.Transform.Rotation, playerController.EyeAngles, Time.Delta * 100f);
			gun.Enabled = true;
			GameObject.Transform.LocalPosition = StartPos;
			gun.Transform.LocalPosition = new Vector3(0, 0, 0);
			arms.Transform.LocalPosition = new Vector3(0, 0, 0);
		}
		else
		{
			gun.Enabled = false;
			arms.BoneMergeTarget = null;
			arms.SceneModel.AnimationGraph = punchGraph;
			arms.Transform.LocalPosition = ArmsPos;
			GameObject.Transform.Rotation = Rotation.Lerp(GameObject.Transform.Rotation, playerController.EyeAngles, Time.Delta * 100f);
		}

		if (IsProxy)
		{
			GameObject.Enabled = false;
		}
	}
}
