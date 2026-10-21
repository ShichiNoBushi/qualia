using Godot;
using System;

public partial class NPC : CharacterBody2D
{
	[Export] public Godot.Collections.Array<string> requireFlags {get; set;} = new();
	[Export] public Godot.Collections.Array<string> hideFlags {get; set;} = new();
	
	[Export] public REncounterData encounter {get; set;}
	[Export] public bool isBattleable {get; set;} = false;
	[Export] public string battleFlag {get; set;} = "";

	[Export] public string name {get; set;} = "";
	[Export] public Godot.Collections.Dictionary<string, RDialog> dialogs {get; set;}
	[Export] public string startDialogId {get; set;} = "";
	
	[Export] public RStoryEvent dialogEvent {get; set;}
	
	[Export] public PackedScene battleScene {get; set;}

	public AnimatedSprite2D sprite;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		bool show = true;
		
		foreach (var flag in requireFlags)
		{
			if (!session.HasFlag(flag))
			{
				show = false;
				break;
			}
		}
		
		foreach (var flag in hideFlags)
		{
			if (session.HasFlag(flag))
			{
				show = false;
				break;
			}
		}
		
		Visible = show;
		
		if (!show)
		{
			ProcessMode = ProcessModeEnum.Disabled;
		}
		
		sprite.Play("walk_front");
	}
	
	public void Interact(Player player)
	{
		FaceToward(player.GlobalPosition);
		
		DialogBox box = GetNode<DialogBox>("/root/DialogBox");
		
		if (box.IsOpen)
		{
			return;
		}
		
		box.Play(dialogs, startDialogId, () => AfterDialog(player));
	}
	
	public void FaceToward(Vector2 target)
	{
		Vector2 d = target - GlobalPosition;
		
		string anim = Mathf.Abs(d.X) > Mathf.Abs(d.Y) ? (d.X > 0 ? "walk_right" : "walk_left") : (d.Y > 0 ? "walk_front" : "walk_back");
		
		sprite.Play(anim);
	}
	
	public virtual void AfterDialog(Player player)
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		if (isBattleable && encounter != null && (string.IsNullOrEmpty(battleFlag) || session.HasFlag(battleFlag)))
		{
			session.ClearFlag(battleFlag);
			StartBattle(player);
		}
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
			session.gameMode = GameSession.GameMode.Battle;
			GetTree().ChangeSceneToPacked(battleScene);
		}
		else
		{
			GetTree().ChangeSceneToFile("res://Scenes/battle_scene.tscn");
		}
	}
}
