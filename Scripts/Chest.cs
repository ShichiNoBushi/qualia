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
		GetNode<DialogBox>("/root/DialogBox").Play("Chest", FormatLoot(), null);
		session.GiveLoot(loot);
	}
	
	public string FormatLoot()
	{
		if (loot == null || loot.Count == 0)
		{
			return "Chest is empty.";
		}
		
		GameSession session = GetNode<GameSession>("/root/GameSession");
		DataRegistry registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		System.Collections.Generic.List<string> lines = new();
		
		string projectorName = string.IsNullOrEmpty(session.playerProjector?.name) ? "(no name)" : session.playerProjector.name;
		
		foreach (var lt in loot)
		{
			if (lt.Value <= 0)
			{
				continue;
			}
			
			if (lt.Key.StartsWith("q_crystal:"))
			{
				string id = lt.Key.Substring(10);
				RQualiaCrystal crystal = registry.Crystal(id);
				
				string name = crystal != null && !string.IsNullOrEmpty(crystal.name) ? crystal.name : id;
				string type = crystal == null ? "" : (crystal.type == RQualiaCrystal.QualiaType.Aspected ? "aspected " : (crystal.type == RQualiaCrystal.QualiaType.Attuned ? "attuned " : ""));
				string noun = lt.Value == 1 ? "crystal" : "crystals";
				
				lines.Add($"[b]{projectorName}[/b] acquired [b]{lt.Value}[/b] {name} {type}{noun}.");
			}
			else if (lt.Key == "generic")
			{
				string noun = lt.Value == 1 ? "shard" : "shards";
				lines.Add($"[b]{projectorName}[/b] acqured [b]{lt.Value}[/b] {noun} of generic crystals.");
			}
			else
			{
				string id = lt.Key;
				RItemData item = registry.Item(id);
				
				string name = item != null && !string.IsNullOrEmpty(item.name) ? item.name : id;
				
				string label;
				
				if (item != null && item.stackable)
				{
					string plural = lt.Value != 1 ? "s" : "";
					label = $"[b]{projectorName}[/b] acquired [b]{lt.Value}[/b] {name}{plural}.";
				}
				else
				{
					label = $"[b]{projectorName}[/b] acqured a {name}.";
				}
				
				lines.Add(label);
			}
		}
		
		return lines.Count == 0 ? "The chest is empty." : string.Join("[break]", lines);
	}
}
