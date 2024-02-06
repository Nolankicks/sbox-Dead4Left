using System;
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
	[Property] public MP5 mp5 { get; set; }
	[Property] public float ammo { get; set; } = 30;
	[Property] public float fullAmmo { get; set; } = 60;
	[Property] public Weapon weapon { get; set; }
	[Property] public SkinnedModelRenderer gun { get; set; }
	[Property] public SoundEvent reloadSound { get; set; }
	public TimeSince timeSinceShoot = 0;
	public TimeSince timeSinceReload = 3;
	protected override void OnUpdate()
	{
		if (weapon.Inventory[weapon.ActiveSlot] == "weapon_shotgun" && Input.Pressed("attack1"))
		{
			Trace();
			gun.Set("b_attack", true);
		}
		if (Input.Pressed("reload") && fullAmmo != 0 && ammo != 30)
		{
			gun.Set("b_reload", true);
			fullAmmo -= 30;
			ammo = 30;
			timeSinceReload = 1;
			Sound.Play(reloadSound, GameObject.Transform.Position);
		}

	}

	void Trace()
	{
		var tr = Scene.Trace.Ray(eye.Transform.Position, eye.Transform.Position + eye.Transform.Rotation.Forward * 1000)
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
				manager.AddScore();
				zombieParticle.Clone(tr.HitPosition);
				zombieRagdoll.Clone(tr.GameObject.Transform.Position, tr.GameObject.Transform.Rotation);
			}
			}
	}
}
