using Godot;
using System;

public partial class Map : Node2D
{
	[Export] public string id {get; set;}
	[Export] public string name {get; set;}
	[Export] public Godot.Collections.Dictionary<string, Marker2D> spawnMarkers {get; set;}
	
	public override void _Ready()
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		session.gameMode = GameSession.GameMode.World;
		
		Player player = GetNode<Player>("YSort/Player");
		
		if (WarpDest.TryParse(session.pendingWarp, out WarpDest dest))
		{
			ApplyMarker(player, dest.markerKey, dest.facing);
			session.pendingWarp = "";
		}
		else if (session.lastResult == BattleManager.VictoryResult.PlayerLose)
		{
			if (session.safePosition != Vector2.Zero)
			{
				player.GlobalPosition = session.safePosition;
				player.PlayWalkAnim(session.safeFacing);
			}
			
			session.lastResult = BattleManager.VictoryResult.None;
		}
		else if (session.lastResult == BattleManager.VictoryResult.PlayerWin || session.lastResult == BattleManager.VictoryResult.PlayerEscape)
		{
			if (session.returnPosition != Vector2.Zero)
			{
				player.GlobalPosition = session.returnPosition;
				player.PlayWalkAnim(session.returnFacing);
			}
			
			player.SetInvulnerable();
			session.lastResult = BattleManager.VictoryResult.None;
		}
		else
		{
			ApplyMarker(player, "default", Vector2.Down);
		}
		
		session.pendingEncounter = null;
	}
	
	public Marker2D GetMarker(string key)
	{
		return GetNodeOrNull<Marker2D>($"Markers/{key}");
	}
	
	public void ApplyMarker(Player player, string key, Vector2 facing)
	{
		Marker2D m = GetMarker(key);
		
		if (m != null)
		{
			player.GlobalPosition = m.GlobalPosition;
			player.PlayWalkAnim(facing);
		}
	}
}
