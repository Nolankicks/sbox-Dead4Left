using Sandbox;
using System.Threading.Tasks;
public sealed class Destroyer : Component
{
	protected override void OnAwake()
	{
		_ = DestroyGameObject();
	}

	public async Task DestroyGameObject()
	{
		await Task.DelaySeconds(3);
		GameObject.Destroy();
	}
}
