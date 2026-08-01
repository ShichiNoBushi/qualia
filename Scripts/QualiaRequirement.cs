using Godot;
using System;

[GlobalClass]
public partial class QualiaRequirement : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public int amount {get; set;} = 1;
	
	public QualiaRequirement()
	{
		id = "";
		amount = 1;
	}
}
