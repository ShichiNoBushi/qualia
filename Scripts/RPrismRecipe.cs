using Godot;
using System;

[GlobalClass]
public partial class RPrismRecipe : Resource
{
	[Export] public Godot.Collections.Array<RQualiaRequirement> requirements {get; set;} = new();
	
	/*public PrismRecipe()
	{
		requirements = new();
	}*/
}
