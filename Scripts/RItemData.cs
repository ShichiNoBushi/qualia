using Godot;
using System;

[GlobalClass]
public partial class RItemData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public string description {get; set;} = "";
	
	[Export] public bool stackable {get; set;} = true;
	[Export] public int uses {get; set;} = 1;
	
	[Export] public int value {get; set;} = 1;
	
	[Export] public bool projectorUsable {get; set;} = true;
	[Export] public bool familiarUsable {get; set;} = true;
	
	[Export] public bool fieldUsable {get; set;} = true;
	[Export] public bool battleUsable {get; set;} = true;
	
	[Export] public ItemPattern itemPattern {get; set;} = ItemPattern.OneAlly;
	
	[Export] public bool isMagicalDefense {get; set;} = false;
	[Export] public int power {get; set;} = 0;
	[Export] public float splashFactor {get; set;} = 0f;
	[Export] public bool overwhelming {get; set;} = false;
	
	[Export] public int healPower {get; set;} = 0;
	[Export] public float defenseFactorOnTarget {get; set;} = 1f;
	
	public enum ItemPattern {
		None,
		OneAlly,
		OneEnemy,
		AllAllies,
		AllEnemies,
		AllUnits
	}
	
	public string FormatDescription()
	{
		System.Collections.Generic.List<string> lines = new();
		
		if (!string.IsNullOrEmpty(name))
		{
			lines.Add(name);
		}
		
		lines.Add($"Usable on Projector: {(projectorUsable ? "Yes" : "No")}\nUsable on Familiar: {(familiarUsable ? "Yes" : "No")}");
		
		lines.Add($"Usable in field: {(fieldUsable ? "Yes" : "No")}\nUsable in battle: {(battleUsable ? "Yes" : "No")}");
		
		if (battleUsable && itemPattern != ItemPattern.None)
		{
			lines.Add($"Target: {TargetLabel()}");
		}
		
		lines.Add($"Value: {value}");
		
		if (power > 0f)
		{
			System.Collections.Generic.List<string> sublines = new();
			
			sublines.Add(isMagicalDefense ? "Defense: Magical" : "Defense: Physical");
			
			sublines.Add("");
			
			sublines.Add($"Power: {power}");
			
			if (splashFactor > 0f)
			{
				sublines.Add($"Splash: {splashFactor:0.0}");
			}
			
			if (overwhelming)
			{
				sublines.Add("Overwhelming");
			}
			
			lines.Add(string.Join("\n", sublines));
		}
		
		if (healPower > 0f)
		{
			lines.Add($"Healing: {healPower}");
		}
		
		if (defenseFactorOnTarget != 1f)
		{
			lines.Add($"Target Defense: {defenseFactorOnTarget:0.0}");
		}
		
		if (!string.IsNullOrEmpty(description))
		{
			lines.Add(description);
		}
		
		return string.Join("\n\n", lines);
	}
	
	string TargetLabel() => itemPattern switch
	{
		ItemPattern.OneAlly => "Ally",
		ItemPattern.OneEnemy => "Enemy",
		ItemPattern.AllAllies => "All Allies",
		ItemPattern.AllEnemies => "All Enemies",
		ItemPattern.AllUnits => "All Units",
		_ => "None"
	};
}
