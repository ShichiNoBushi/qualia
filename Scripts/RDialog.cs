using Godot;
using System;

[GlobalClass]
public partial class RDialog : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string speaker {get; set;} = "";
	[Export] public string text {get; set;} = "";
	
	[Export] public string nextId {get; set;} = "";
	[Export] public Godot.Collections.Array<RDialogChoice> choices {get; set;} = new();
	
	[Export] public RStoryEvent onFinish {get; set;}
}
