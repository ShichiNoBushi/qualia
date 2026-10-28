using Godot;
using System;

[GlobalClass]
public partial class RFamiliarData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public string description {get; set;} = "";
	
	[Export] public int baseEnergy {get; set;} = 10;
	[Export] public int basePAttack {get; set;} = 3;
	[Export] public int baseMAttack {get; set;} = 3;
	[Export] public int basePDefense {get; set;} = 3;
	[Export] public int baseMDefense {get; set;} = 3;
	[Export] public int baseSpeed {get; set;} = 3;
	
	[Export] public float levelEnergy {get; set;} = 1f;
	[Export] public float levelPAttack {get; set;} = 0.5f;
	[Export] public float levelMAttack {get; set;} = 0.5f;
	[Export] public float levelPDefense {get; set;} = 0.5f;
	[Export] public float levelMDefense {get; set;} = 0.5f;
	[Export] public float levelSpeed {get; set;} = 0.5f;
	
	[Export] public float expGrowthFactor {get; set;} = 1f;
	
	[Export] public Godot.Collections.Array<RTypeData> types {get; set;} = new();
	[Export] public Godot.Collections.Array<RSkillData> learnableSkills {get; set;} = new();
	[Export] public Godot.Collections.Dictionary<int, RSkillData> levelUpSkills {get; set;} = new();
	[Export] public Godot.Collections.Dictionary<string, int> fortifyRecipe {get; set;} = new();
	[Export] public int fortifyCost {get; set;} = 0;
	[Export] public Godot.Collections.Array<string> enhanceCrystals {get; set;} = new();
	
	[Export] public int crystals {get; set;} = 0;
	[Export] public Godot.Collections.Array<RCrystalDrop> crystalDrops {get; set;} = new();
	
	[Export] public Texture2D portrait {get; set;}
	
	public RSkillData SkillAt(int at)
	{
		if (levelUpSkills == null)
		{
			return null;
		}
		
		return levelUpSkills.TryGetValue(at, out RSkillData skill) ? skill : null;
	}
	
	public string FormatDescription()
	{
		System.Collections.Generic.List<string> lines = new();
		
		lines.Add(string.IsNullOrEmpty(name) ? "(no name)" : name);
		
		System.Collections.Generic.List<string> sublines = new();
		
		sublines.Add("Types:");
		
		if (types != null && types.Count > 0)
		{
			foreach (var t in types)
			{
				string typeName = string.IsNullOrEmpty(t.name) ? "(type)" : t.name;
				string fill = t.color.ToHtml(false);
				string edge = t.outline.A > 0 ? t.outline.ToHtml(false) : t.ContrastColor().ToHtml(false);
				
				sublines.Add($"[outline_size=4][outline_color=#{edge}][color=#{fill}]{typeName}[/color][/outline_color][/outline_size]");
			}
		}
		else
		{
			sublines.Add("(untyped)");
		}
		
		lines.Add(string.Join("\n", sublines));
		
		sublines.Clear();
		
		sublines.Add("Base Stats:");
		
		sublines.Add($"  Energy     - {StatSummary(baseEnergy, 5f)}");
		sublines.Add($"  P. Attack  - {StatSummary(basePAttack, 1f)}");
		sublines.Add($"  M. Attack  - {StatSummary(baseMAttack, 1f)}");
		sublines.Add($"  P. Defense - {StatSummary(basePDefense, 1f)}");
		sublines.Add($"  M. Defense - {StatSummary(baseMDefense, 1f)}");
		sublines.Add($"  Speed      - {StatSummary(baseSpeed, 1f)}");
		
		lines.Add(string.Join("\n", sublines));
		
		sublines.Clear();
		
		sublines.Add("Stat Growth:");
		
		sublines.Add($"  Energy     - {StatSummary(levelEnergy, 2f)}");
		sublines.Add($"  P. Attack  - {StatSummary(levelPAttack, 0.5f)}");
		sublines.Add($"  M. Attack  - {StatSummary(levelMAttack, 0.5f)}");
		sublines.Add($"  P. Defense - {StatSummary(levelPDefense, 0.5f)}");
		sublines.Add($"  M. Defense - {StatSummary(levelMDefense, 0.5f)}");
		sublines.Add($"  Speed - {StatSummary(levelSpeed, 0.5f)}");
		
		lines.Add(string.Join("\n", sublines));
		
		sublines.Clear();
		
		sublines.Add("Growth Rate:");
		sublines.Add(ExpGrowthSummary());
		lines.Add(string.Join("\n", sublines));
		
		lines.Add(description);
		
		return string.Join("\n\n", lines);
	}
	
	public string StatSummary(float statValue, float interval)
	{
		int ranking = Mathf.Max((int)(statValue / interval), 1);
		
		if (ranking <= 5)
		{
			string stars = "";
			
			for (int i = 0; i < ranking; i++)
			{
				stars += "•";
			}
			
			return stars;
		}
		else
		{
			return $"{ranking}•";
		}
	}
	
	public string ExpGrowthSummary()
	{
		if (expGrowthFactor <= 0.25f)
		{
			return "Very Slow";
		}
		else if (expGrowthFactor > 0.25f && expGrowthFactor <= 0.5f)
		{
			return "Slow";
		}
		else if (expGrowthFactor > 0.5f && expGrowthFactor <= 0.85f)
		{
			return "Somewhat Slow";
		}
		else if (expGrowthFactor > 0.85f && expGrowthFactor < 1.15f)
		{
			return "Average";
		}
		else if (expGrowthFactor >= 1.15f && expGrowthFactor < 1.5f)
		{
			return "Somewhat Fast";
		}
		else if (expGrowthFactor >= 1.5f && expGrowthFactor < 1.75f)
		{
			return "Fast";
		}
		else
		{
			return "Very Fast";
		}
	}
}
