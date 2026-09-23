using Godot;
using System;

public partial class InnNPC : NPC
{
	public override void AfterDialog(Player player)
	{
		player.projector?.Recover();
		
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		session.safePath = GetTree().CurrentScene.SceneFilePath;
		session.safePosition = GlobalPosition + new Vector2(0, 16);
		session.safeFacing = Vector2.Down;
	}
}
