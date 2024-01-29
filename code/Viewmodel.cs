using Sandbox;

public sealed class Viewmodel : Component
{
	protected override void OnUpdate()
	{
			if (Input.Pressed("Duck"))
			{
				GameObject.Transform.Position -= Vector3.Down * -32;
			}
			if (Input.Released("Duck"))
			{
				GameObject.Transform.Position += Vector3.Up * 32;
			}
	}
}
