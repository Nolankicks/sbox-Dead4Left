using System;
using System.Linq;
using Kicks;
using Sandbox;

public sealed class Shotgun : Component
{
	[Property] public GameObject eye {get; set;}
	[Property] public GameObject impact { get; set; }
	[Property] public SoundEvent gunSound { get; set; }
	[Property] public GameObject decalGo { get; set; }
	[Property] public Manager manager { get; set; }
	[Property] public GameObject zombieRagdoll { get; set; }
	[Property] public GameObject zombieParticle { get; set; }
	[Property] public float ammo { get; set; } = 6;
	[Property] public float fullAmmo { get; set; } = 12;
	[Property] public Weapon weapon { get; set; }
	[Property] public SkinnedModelRenderer gun { get; set; }
	[Property] public SoundEvent reloadSound { get; set; }
	public TimeSince timeSinceShoot = 0;
	public int timesShoot = 0;
	public TimeSince timeSinceReload = 3;
	public PlayerController playerController;
	protected override void OnStart()
	{
		playerController = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		if (weapon.Inventory[weapon.ActiveSlot] == "weapon_shotgun" && Input.Pressed("attack1") && timeSinceReload > 1 && ammo != 0)
		{
			for (int i = 0; i < 3; i++)
			{
				Trace();
			}
			gun.Set("b_attack", true);
			ammo -= 1;
			timesShoot += 1;
		}

		if (Input.Pressed("reload") && fullAmmo != 0 && ammo != 30 && weapon.Inventory[weapon.ActiveSlot] == "weapon_shotgun")
		{
			gun.Set("b_reload", true);
			fullAmmo -= timesShoot;
			ammo = 4;
			timeSinceReload = 1;
			Sound.Play(reloadSound, GameObject.Transform.Position);
		}
		if (fullAmmo < 0)
		{
			fullAmmo = 0;
		}
		if (ammo < 0)
		{
			ammo = 0;
		}

	}

	void Trace()
	{
		var tr = Scene.Trace.Ray(eye.Transform.Position, eye.Transform.Position * Random.Shared.Float(0.4f, 1.4f) + eye.Transform.Rotation.Forward * 8000)
			.WithoutTags("player")
			.Run();

			if (tr.Hit)
			{
				
			Log.Info(tr.GameObject);
			impact.Clone(tr.HitPosition);
			Sound.Play(gunSound, GameObject.Transform.Position);
			var decal = decalGo.Clone(new Transform(tr.HitPosition + tr.Normal * 2.0f, Rotation.LookAt(-tr.Normal, Vector3.Random), Random.Shared.Float(0.8f, 1.2f)));
			decal.SetParent( tr.GameObject );
			if (tr.GameObject.Tags.Has("bad"))
			{
				tr.GameObject.Parent.Destroy();
				playerController.AddScore(5);
				zombieParticle.Clone(tr.HitPosition);
				zombieRagdoll.Clone(tr.GameObject.Transform.Position, tr.GameObject.Transform.Rotation);
			}
			}
	}
}
