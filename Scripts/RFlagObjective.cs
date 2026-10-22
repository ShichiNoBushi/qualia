using Godot;
using System;

[GlobalClass]
public partial class RFlagObjective : RQuestObjective
{
	[Export] public string flag {get; set;} = "";
	[Export] public bool mustBeSet {get; set;} = true;
	
	public override bool IsSatisfied(GameSession session) => mustBeSet ? session.HasFlag(flag) : !session.HasFlag(flag);
}
