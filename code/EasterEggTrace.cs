using Sandbox;

public sealed class EasterEggTrace : Component
{
	[Property] public GameObject bloodParticle { get; set; }
	[Property] public SoundEvent soundEvent { get; set; }
	protected override void OnUpdate()
	{
		var tr = Scene.Trace.Ray(Scene.Camera.ScreenPixelToRay(Mouse.Position), 10000f).Run();

		if (Input.Pressed("attack1") && tr.Hit && tr.GameObject.Tags.Has("zombie"))
		{
			Log.Info(tr.GameObject);
			bloodParticle.Clone(tr.HitPosition);
			soundEvent.UI = true;
			var sound = Sound.Play(soundEvent);
			tr.GameObject.Parent.Destroy();
		}
	}
}
