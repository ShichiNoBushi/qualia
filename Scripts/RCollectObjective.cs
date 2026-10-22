using Godot;
using System;

[GlobalClass]
public partial class RCollectObjective : RQuestObjective
{
	[Export] public string itemId {get; set;} = "";
	[Export] public int amount {get; set;} = 1;
	[Export] public bool isCrystal {get; set;} = false;
	
	public override bool IsSatisfied(GameSession session)
	{
		if (isCrystal)
		{
			return session.qualiaCrystals.TryGetValue(itemId, out int n) ? n >= amount : false;
		}
		
		RItemData data = session.registry.Item(itemId);
		
		if (data == null)
		{
			return false;
		}
		
		if (data.stackable)
		{
			return session.itemStacks.TryGetValue(itemId, out int m) ? m >= amount : false;
		}
		
		int l = 0;
		
		foreach (var unique in session.uniqueItems)
		{
			if (unique.data != null && unique.data.id == itemId)
			{
				l++;
			}
		}
		
		return l >= amount;
	}
	
	public override string Status(GameSession session)
	{
		int total = 0;
		RItemData data = session.registry.Item(itemId);
		
		if (isCrystal)
		{
			total = session.qualiaCrystals.TryGetValue(itemId, out int n) ? n : 0;
		}
		else if (data != null && data.stackable)
		{
			total = session.itemStacks.TryGetValue(itemId, out int m) ? m : 0;
		}
		else if (data != null)
		{
			foreach (var unique in session.uniqueItems)
			{
				if (unique.data != null && unique.data.id == itemId)
				{
					total++;
				}
			}
		}
		
		string completedText = IsSatisfied(session) ? " Complete" : "";
		
		return $"{total} / {amount}{completedText}";
	}
}
