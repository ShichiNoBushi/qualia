using Godot;
using System;

[GlobalClass]
public partial class RQuestData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public string description {get; set;} = "";
	
	[Export] public Godot.Collections.Array<RQuestObjective> objectives {get; set;}
	
	[Export] public RStoryEvent completionEvent {get; set;}
	
	public bool completed {get; set;} = false;
}
