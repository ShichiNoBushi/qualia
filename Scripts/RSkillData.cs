using Godot;
using System;

[GlobalClass]
public partial class RSkillData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public RTypeData type {get; set;}
	[Export] public string description {get; set;} = "";
	
	[Export] public TargetPattern targetPattern {get; set;} = TargetPattern.OneEnemy;
	[Export] public int cost {get; set;} = 0;
	[Export] public float speedFactor {get; set;} = 1f;
	
	[Export] public bool isMagicalAttack {get; set;} = false;
	[Export] public bool isMagicalDefense {get; set;} = false;
	[Export] public int power {get; set;} = 0;
	[Export] public float splashFactor {get; set;} = 0f;
	[Export] public bool overwhelming {get; set;} = false;
	
	[Export] public int healPower {get; set;} = 0;
	[Export] public float defenseFactorOnUser {get; set;} = 1f;
	[Export] public float defenseFactorOnTarget {get; set;} = 1f;
	
	public enum TargetPattern {
		None,
		Self,
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
		
		if (type != null && !string.IsNullOrEmpty(type.name))
		{
			lines.Add(type.name);
		}
		
		lines.Add($"Cost: {cost}");
		
		if (targetPattern != TargetPattern.None)
		{
			lines.Add($"Target: {TargetLabel()}");
		}
		
		lines.Add($"Speed Factor: {speedFactor:0.0}");
		
		if (power > 0f)
		{
			System.Collections.Generic.List<string> sublines = new();
			
			sublines.Add(isMagicalAttack ? "Attack: Magical" : "Attack: Physical");
			sublines.Add(isMagicalDefense ? "Defense: Magical\n" : "Defense: Physical");
			
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
			System.Collections.Generic.List<string> sublines = new();
			
			sublines.Add(isMagicalAttack ? "Healing: Magical\n" : "Healing: Physical");
			sublines.Add("");
			sublines.Add($"Healing: {healPower}");
			
			lines.Add(string.Join("\n", sublines));
		}
		
		if (defenseFactorOnUser != 1f || defenseFactorOnTarget != 1f)
		{
			System.Collections.Generic.List<string> sublines = new();
			
			if (defenseFactorOnUser == defenseFactorOnTarget)
			{
				sublines.Add($"Defense: {defenseFactorOnUser:0.0}\n");
			}
			else
			{
				sublines.Add($"User Defense: {defenseFactorOnUser:0.0}");
				sublines.Add($"Target Defense: {defenseFactorOnTarget:0.0}\n");
			}
			
			lines.Add(string.Join("\n", sublines));
		}
		
		if (!string.IsNullOrEmpty(description))
		{
			lines.Add(description);
		}
		
		return string.Join("\n\n", lines);
	}
	
	string TargetLabel() => targetPattern switch
	{
		TargetPattern.Self => "Self",
		TargetPattern.OneAlly => "Ally",
		TargetPattern.OneEnemy => "Enemy",
		TargetPattern.AllAllies => "All Allies",
		TargetPattern.AllEnemies => "All Enemies",
		TargetPattern.AllUnits => "All Units",
		_ => "None"
	};
}
