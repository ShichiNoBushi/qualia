using Godot;
using System;

[GlobalClass]
public partial class RQualiaCrystal : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public string description {get; set;} = "";
	
	[Export] public QualiaType type {get; set;} = QualiaType.Aspected;
	
	[Export] public Godot.Collections.Array<RQualiaRequirement> recipe {get; set;}
	[Export] public int value {get; set;} = 5;
	
	[Export] public Texture2D icon {get; set;}
	
	public enum QualiaType
	{
		Attuned,
		Aspected
	}
	
	public string FormatDescription()
	{
		System.Collections.Generic.List<string> lines = new();
		
		if (!string.IsNullOrEmpty(name))
		{
			if (type == QualiaType.Aspected)
			{
				lines.Add($"{name} aspected crystal");
			}
			else if (type == QualiaType.Attuned)
			{
				lines.Add($"{name} attuned crystal");
			}
			else
			{
				lines.Add(name);
			}
		}
		
		lines.Add($"Value: {value}");
		
		if (!string.IsNullOrEmpty(description))
		{
			lines.Add(description);
		}
		
		return string.Join("\n\n", lines);
	}
}
