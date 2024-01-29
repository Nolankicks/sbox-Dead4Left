using System;
using System.Linq;
using Sandbox;

public sealed class MP5 : Component
{
	[Property] public SkinnedModelRenderer gun { get; set; }
	[Property] public GameObject impact { get; set; }
	[Property] public GameObject eye { get; set; }
	[Property] public SoundEvent gunSound { get; set; }
	[Property] public GameObject decalGo { get; set; }
	[Property] public float ammo { get; set; } = 30;
	[Property] public float fullAmmo { get; set; } = 60;
	Manager manager => Scene.GetAllComponents<Manager>().FirstOrDefault();
	bool ableToShoot;
	bool reloading;
	public TimeSince timeSinceReload = 3;
	
	protected override void OnUpdate()
	{
		
		if (Input.Pressed("reload") && fullAmmo != 0 && ammo != 30)
		{
			gun.Set("b_reload", true);
			fullAmmo -= 30;
			ammo = 30;
			timeSinceReload = 1;
		}

	}
	protected override void OnFixedUpdate()
	{
		
		if (Input.Down("attack1") && ammo > 0 && timeSinceReload > 3)
		{
			Shoot();
			gun.Set("b_attack", true);
		}

	}
	void Shoot()
	{
		var attachment = gun.GetAttachment( "muzzle" );
		
		var ray = Scene.Camera.ScreenNormalToRay( 0.5f );
		var tr = Scene.Trace.Ray( eye.Transform.Position, eye.Transform.Position + eye.Transform.Rotation.Forward * 8000).Run();
		if (tr.Hit)
		{
			ammo -= 1;
			Log.Info(tr.GameObject);
			impact.Clone(tr.HitPosition);
			Sound.Play(gunSound, tr.HitPosition);
			var decal = decalGo.Clone(new Transform(tr.HitPosition + tr.Normal * 2.0f, Rotation.LookAt(-tr.Normal, Vector3.Random), Random.Shared.Float(0.8f, 1.2f)));
			decal.SetParent( tr.GameObject );
			if (tr.GameObject.Tags.Has("bad"))
			{
				tr.GameObject.Parent.Destroy();
				manager.GetScore();
			}
		}
	}
}
