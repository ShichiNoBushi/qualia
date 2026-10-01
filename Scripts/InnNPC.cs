using Godot;
using System;

public partial class InnNPC : NPC
{
	[Export] public Marker2D safeMarker {get; set;}
	[Export] public string safeKey {get; set;} = "";
	
	public override void _Ready()
	{
		base._Ready();
		CallDeferred(nameof(UpdateSafeMarker));
	}
	
	public override void AfterDialog(Player player)
	{
		player.projector?.Recover();
		
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		session.safePath = GetTree().CurrentScene.SceneFilePath;
		session.safeMapId = (GetTree().GetFirstNodeInGroup("map") as Map)?.id ?? "";
		session.safeMarkerId = string.IsNullOrEmpty(safeKey) ? "safe" : safeKey;
		session.safePosition = safeMarker != null ? safeMarker.GlobalPosition : GlobalPosition + new Vector2(0, 16);
		session.safeFacing = Vector2.Down;
		
		GD.Print($"InnNPC: safe position path={session.safePath}, mapId={session.safeMapId}, markerId={session.safeMarkerId}, position={session.safePosition}");
	}
	
	public void UpdateSafeMarker()
	{
		if (safeMarker == null)
		{
			string key = string.IsNullOrEmpty(safeKey) ? "default" : safeKey;
			
			Map map = GetTree().GetFirstNodeInGroup("map") as Map;
			
			safeMarker = map?.GetMarker(key);
		}
	}
}
