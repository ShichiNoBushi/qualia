using Godot;
using System;

[GlobalClass]
public partial class RForgeRecipe : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string resultId {get; set;} = "";
	[Export] public bool makesPrism {get; set;} = true;
	[Export] public Godot.Collections.Dictionary<string, int> ingredients {get; set;} = new();
	[Export] public int cost {get; set;} = 0;
	
	public string FormatDescription(DataRegistry registry)
	{
		System.Collections.Generic.List<string> lines = new();
		
		string name = GetResultName(registry);
		
		name += makesPrism ? " Prism" : " Crystal";
		
		if (!string.IsNullOrEmpty(name))
		{
			lines.Add(name);
		}
		
		System.Collections.Generic.List<string> ingSubstring = new();
		ingSubstring.Add("Ingredients:");
		
		foreach (var ing in ingredients)
		{
			RQualiaCrystal cData = registry.Crystal(ing.Key);
			
			ingSubstring.Add($"{cData.name} x{ing.Value}");
		}
		
		lines.Add(string.Join("\n", ingSubstring));
		
		lines.Add($"Cost: {cost} G. Crystals");
		
		return string.Join("\n\n", lines);
	}
	
	public string GetResultName(DataRegistry registry)
	{
		if (registry == null || string.IsNullOrEmpty(resultId))
		{
			return "(unknown)";
		}
		
		if (makesPrism)
		{
			return registry.Familiar(resultId)?.name ?? resultId;
		}
		
		return registry.Crystal(resultId)?.name ?? resultId;
	}
}
