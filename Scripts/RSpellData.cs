using Godot;
using System;

[GlobalClass]
public partial class RSpellData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public RTypeData type {get; set;}
	[Export] public string description {get; set;} = "";
	
	[Export] public SpellPattern spellPattern {get; set;} = SpellPattern.OneEnemy;
	[Export] public int cost {get; set;} = 0;
	
	[Export] public bool isMagicalDefense {get; set;} = false;
	[Export] public int power {get; set;} = 0;
	[Export] public float splashFactor {get; set;} = 0f;
	
	[Export] public int healPower {get; set;} = 0;
	[Export] public float defenseFactorOnTarget {get; set;} = 1f;
	
	public enum SpellPattern {
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
		
		if (type != null && !string.IsNullOrEmpty(type.name))
		{
			lines.Add(type.name);
		}
		
		lines.Add($"Cost: {cost}\n");
		
		if (spellPattern != SpellPattern.None)
		{
			lines.Add($"Target: {TargetLabel()}");
		}
		
		if (power > 0f)
		{
			System.Collections.Generic.List<string> sublines = new();
			
			sublines.Add(isMagicalDefense ? "Defense: Magical" : "Defense: Physical");
			
			sublines.Add("\n");
			
			sublines.Add($"Power: {power}");
			
			if (splashFactor > 0f)
			{
				sublines.Add($"Splash: {splashFactor:0.0}");
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
	
	string TargetLabel() => spellPattern switch
	{
		SpellPattern.OneAlly => "Ally",
		SpellPattern.OneEnemy => "Enemy",
		SpellPattern.AllAllies => "All Allies",
		SpellPattern.AllEnemies => "All Enemies",
		SpellPattern.AllUnits => "All Units",
		_ => "None"
	};
}
