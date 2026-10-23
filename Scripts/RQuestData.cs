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
	
	public bool Check(GameSession session)
	{
		if (completed)
		{
			return true;
		}
		
		foreach (var obj in objectives)
		{
			if (obj == null || !obj.IsSatisfied(session))
			{
				return false;
			}
		}
		
		completed = true;
		completionEvent?.Execute(session);
		return true;
	}
	
	public string FormatDescription(GameSession session)
	{
		System.Collections.Generic.List<string> lines = new();
		
		lines.Add(name);
		
		lines.Add(description);
		
		if (objectives.Count > 0)
		{
			System.Collections.Generic.List<string> objLines = new();
			
			foreach (var obj in objectives)
			{
				objLines.Add($"{obj.description} - {obj.Status(session)}");
			}
			
			lines.Add(string.Join("\n", objLines));
		}
		
		return string.Join("\n\n", lines);
	}
}
