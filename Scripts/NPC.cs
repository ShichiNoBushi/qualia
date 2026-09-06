using Godot;
using System;

public partial class NPC : CharacterBody2D
{
	[Export] public REncounterData encounter {get; set;}
	[Export] public bool isBattleable {get; set;} = false;

	[Export] public string name {get; set;} = "";
	[Export] public string dialog {get; set;} = "";
	
	[Export] public PackedScene battleScene {get; set;}

	public AnimatedSprite2D sprite;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		sprite.Play("walk_front");
	}
	
	public void Interact(Player player)
	{
		FaceToward(player.GlobalPosition);
		
		//Display dialog.
		
		if (isBattleable && encounter != null)
		{
			StartBattle(player);
		}
	}
	
	public void FaceToward(Vector2 target)
	{
		Vector2 d = target - GlobalPosition;
		
		string anim = Mathf.Abs(d.X) > Mathf.Abs(d.Y) ? (d.X > 0 ? "walk_right" : "walk_left") : (d.Y > 0 ? "walk_front" : "walk_back");
		
		sprite.Play(anim);
	}
	
	public void StartBattle(Player player)
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		session.pendingEncounter = encounter;
		session.returnPath = GetTree().CurrentScene.SceneFilePath;
		session.returnPosition = player.GlobalPosition;
		session.returnFacing = player.facing;
		
		if (encounter != null)
		{
			GetTree().ChangeSceneToPacked(battleScene);
		}
		else
		{
			GetTree().ChangeSceneToFile("res://Scenes/battle_scene.tscn");
		}
	}
}
