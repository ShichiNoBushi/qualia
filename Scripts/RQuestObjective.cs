using Godot;
using System;

[GlobalClass]
public partial class RQuestObjective : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string description {get; set;} = "";
	
	public virtual bool IsSatisfied(GameSession session) => false;
	public virtual string Status(GameSession session) => IsSatisfied(session) ? "Completed" : "Incomplete";
}
