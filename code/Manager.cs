using Sandbox;

public sealed class Manager : Component
{
	public Sandbox.Services.Leaderboards.Board Leaderboard;
	public bool Playing { get; private set; } = false;
	public long Score { get; private set; } = 0;
	public long HighScore { get; private set; } = 0;
	[Property] public SceneFile endScene { get; set; }
	protected override void OnUpdate()
	{
	FetchLeaderboardInfo();

	}
	public void GetScore()
	{
		var score = 0;
		Score += 5;
		Score += score;
		if ( Score > HighScore ) HighScore = Score;
		
	}
	public void GameEnd()
	{
		GameManager.ActiveScene.Load(endScene);
		Sandbox.Services.Stats.SetValue( "zombieskilled", Score );
	}
		async void FetchLeaderboardInfo()
	{
		Leaderboard = Sandbox.Services.Leaderboards.Get( "mostzombieskill" );
		Leaderboard.MaxEntries = 10;
		await Leaderboard.Refresh();
		foreach ( var entry in Leaderboard.Entries )
		{
			if ( entry.SteamId == Game.SteamId )
			{
				HighScore = (long)entry.Value;
			}
		}
	}
}
