using Godot;
using System;

[GlobalClass]
public partial class RQualiaCrystal : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	
	[Export] public QualiaType type {get; set;} = QualiaType.Attuned;
	
	[Export] public Godot.Collections.Array<RQualiaRequirement> recipe {get; set;}
	[Export] public int value {get; set;} = 1;
	
	[Export] public Texture2D icon {get; set;}
	
	public enum QualiaType
	{
		Attuned,
		Aspected
	}
}
