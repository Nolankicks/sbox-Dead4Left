using Sandbox;
namespace Kicks;

public interface IScoreComponent
{
	public long Score { get; }
	public long HighScore { get; }
	public void AddScore(long score);
	
}
