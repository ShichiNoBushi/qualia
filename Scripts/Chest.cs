using Godot;
using System;

public partial class Chest : StaticBody2D
{
	[Export] public string id {get; set;} = "";
	[Export] public Godot.Collections.Dictionary<string, int> loot {get; set;}
	
	[Export] public bool startOpened {get; set;} = false;
	
	public bool opened;
	
	public AnimatedSprite2D sprite;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		opened = startOpened || session.IsChestOpened(id);
		sprite.Play(opened ? "opened" : "closed");
	}
	
	public void Interact(Player player)
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		if (opened || session.IsChestOpened(id))
		{
			GetNode<DialogBox>("/root/DialogBox").Play("", "It's empty.", null);
			return;
		}
		
		opened = true;
		sprite.Play("opened");
		session.MarkChestOpened(id);
		session.GiveLoot(loot);
	}
}
