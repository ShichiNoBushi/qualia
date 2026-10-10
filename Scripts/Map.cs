using Godot;
using System;

public partial class Map : Node2D
{
	[Export] public string id {get; set;}
	[Export] public string name {get; set;}
	[Export] public Godot.Collections.Dictionary<string, Marker2D> spawnMarkers {get; set;}
	
	public override async void _Ready()
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		session.gameMode = GameSession.GameMode.World;
		
		Player player = GetNode<Player>("YSort/Player");
		session.player = player;
		
		if (session.pendingLoadPosition != Vector2.Zero)
		{
			player.GlobalPosition = session.pendingLoadPosition;
			
			player.facing = session.pendingLoadFacing != Vector2.Zero ? session.pendingLoadFacing : Vector2.Down;
			player.PlayWalkAnim(player.facing);
			
			session.pendingLoadPosition = Vector2.Zero;
			session.pendingLoadFacing = Vector2.Zero;
		}
		else if (WarpDest.TryParse(session.pendingWarp, out WarpDest dest))
		{
			player.warping = true;
			ApplyMarker(player, dest.markerKey, dest.facing);
			session.pendingWarp = "";
		}
		else if (session.lastResult == BattleManager.VictoryResult.PlayerLose)
		{
			if (session.safePath != null)
			{
				player.GlobalPosition = session.safePosition;
				player.PlayWalkAnim(session.safeFacing);
			}
			else
			{
				ApplyMarker(player, "default", Vector2.Down);
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
		
		if (string.IsNullOrEmpty(session.safePath))
		{
			session.safePath = GetTree().CurrentScene.SceneFilePath;
			session.safePosition = GetNode<Marker2D>("Markers/default").GlobalPosition;
			session.safeFacing = Vector2.Down;
		}
		
		bool seen = false;
		
		for (int i = 0; i < 12; i++)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			
			if (IsOverlappingTransition(player))
			{
				seen = true;
				break;
			}
		}
		
		if (!seen)
		{
			player.warping = false;
		}
	}
	
	public Marker2D GetMarker(string key)
	{
		return GetNodeOrNull<Marker2D>($"Markers/{key}");
	}
	
	public bool IsOverlappingTransition(Player player)
	{
		foreach (var n in GetTree().GetNodesInGroup("map_transitions"))
		{
			if (n is MapTransition t && t.GetOverlappingBodies().Contains(player))
			{
				return true;
			}
		}
		
		return false;
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
