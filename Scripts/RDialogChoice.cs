using Godot;
using System;

[GlobalClass]
public partial class RDialogChoice : Resource
{
	[Export] public string label {get; set;} = "";
	[Export] public string nextId {get; set;} = "";
	[Export] public Godot.Collections.Array<string> requireFlags {get; set;} = new();
	[Export] public Godot.Collections.Array<string> forbidFlags {get; set;} = new();
	[Export] public RStoryEvent onSelect {get; set;}
}
