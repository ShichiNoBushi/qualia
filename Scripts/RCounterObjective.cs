using Godot;
using System;

[GlobalClass]
public partial class RCounterObjective : RQuestObjective
{
	[Export] public string key {get; set;} = "";
	[Export] public int target {get; set;} = 1;
	[Export] public bool isBool {get; set;} = false;
	
	public override bool IsSatisfied(GameSession session) => session.GetCounter(key) >= target;
	
	public override string Status(GameSession session)
	{
		if (isBool)
		{
			return IsSatisfied(session) ? "Completed" : "Incomplete";
		}
		
		string completedText = IsSatisfied(session) ? " Completed" : "";
		
		return $"{session.GetCounter(key)} / {target}{completedText}";
	}
}
