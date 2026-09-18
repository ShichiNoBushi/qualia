using Godot;
using System;

public partial class WildSpark : Area2D
{
	public REncounterData encounter {get; set;}
	public EncounterZone zone {get; set;}
	
	public Timer timer;
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		
		timer = GetNode<Timer>("Timer");
		timer.Timeout += Despawn;
		
		timer.Start(20f);
	}
	
	public void OnBodyEntered(Node2D body)
	{
		if (body is not Player player)
		{
			return;
		}
		
		if (encounter == null)
		{
			return;
		}
		
		zone?.RemoveSpark(this);
		QueueFree();
		
		StartBattle(player);
	}
	
	public void StartBattle(Player player)
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		session.pendingEncounter = encounter;
		session.returnPath = GetTree().CurrentScene.SceneFilePath;
		session.returnPosition = player.GlobalPosition;
		session.returnFacing = player.facing;
		GetTree().ChangeSceneToFile("res://Scenes/battle_scene.tscn");
	}
	
	public void Despawn()
	{
		zone?.RemoveSpark(this);
		QueueFree();
	}
}
