using Godot;
using System;

[GlobalClass]
public partial class PrismRecipe : Resource
{
	[Export] public Godot.Collections.Array<QualiaRequirement> requirements {get; set;} = new();
	
	public PrismRecipe()
	{
		requirements = new();
	}
}
