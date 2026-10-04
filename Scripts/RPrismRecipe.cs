using Godot;
using System;

[GlobalClass]
public partial class RPrismRecipe : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string resultId {get; set;} = "";
	[Export] public bool makesPrism {get; set;} = false;
	[Export] public Godot.Collections.Array<string> ingredients {get; set;} = new();
	[Export] public int cost {get; set;} = 0;
}
