using Godot;
using System;

public partial class MapTransition : Area2D
{
	[Export] public string warp {get; set;} = "";
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	
	public void OnBodyEntered(Node2D body)
	{
		if (body is not Player player)
		{
			return;
		}
		
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		if (session.gameMode != GameSession.GameMode.World)
		{
			return;
		}
		
		if (!WarpDest.TryParse(warp, out WarpDest dest))
		{
			return;
		}
		
		CallDeferred(nameof(GoTo), dest.mapId, dest.markerKey, dest.facing);
	}
	
	public void GoTo(string mapId, string markerKey, Vector2 facing)
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		session.pendingWarp = $"{mapId}:{markerKey}:{FacingName(facing)}";
		
		string scene = session.SceneForMap(mapId);
		
		if (string.IsNullOrEmpty(scene))
		{
			return;
		}
		
		GetTree().ChangeSceneToFile(scene);
	}
	
	public string FacingName(Vector2 facing)
	{
		if (facing == Vector2.Up)
		{
			return "up";
		}
		
		if (facing == Vector2.Left)
		{
			return "left";
		}
		
		if (facing == Vector2.Right)
		{
			return "right";
		}
		
		return "down";
	}
}

public struct WarpDest
{
	public string mapId;
	public string markerKey;
	public Vector2 facing;
	
	public static bool TryParse(string raw, out WarpDest dest)
	{
		dest = default;
		
		if (string.IsNullOrEmpty(raw))
		{
			return false;
		}
		
		string[] p = raw.Split(':');
		
		if (p.Length != 3)
		{
			return false;
		}
		
		dest.mapId = p[0];
		dest.markerKey = p[1];
		dest.facing = p[2].ToLower() switch
		{
			"up" or "back" or "north" => Vector2.Up,
			"down" or "front" or "south" => Vector2.Down,
			"left" or "west" => Vector2.Left,
			"right" or "east" => Vector2.Right,
			_ => Vector2.Down
		};
		
		return true;
	}
}
