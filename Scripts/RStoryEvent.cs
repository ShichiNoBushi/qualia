using Godot;
using System;

[GlobalClass]
public partial class RStoryEvent : Resource
{
	[Export] public Godot.Collections.Array<string> requireFlags {get; set;} = new();
	[Export] public string onceFlag {get; set;} = "";
	[Export] public Godot.Collections.Array<string> setFlags {get; set;} = new();
	[Export] public Godot.Collections.Array<string> clearFlags {get; set;} = new();
	[Export] public Godot.Collections.Dictionary<string, int> giftItems {get; set;} = new();
	[Export] public Godot.Collections.Dictionary<string, int> takeItems {get; set;} = new();
	[Export] public Godot.Collections.Dictionary<string, int> giftCrystals {get; set;} = new();
	[Export] public Godot.Collections.Array<RFamiliarInstance> giftPrisms {get; set;} = new();
	[Export] public int giftGeneric {get; set;} = 0;
	
	public bool Execute (GameSession session)
	{
		foreach (var flag in requireFlags)
		{
			if (!session.HasFlag(flag))
			{
				return false;
			}
		}
		
		if (!string.IsNullOrEmpty(onceFlag) && session.HasFlag(onceFlag))
		{
			return false;
		}
		
		if (giftPrisms.Count > 0 && (session.playerProjector == null || session.playerProjector.ownedFamiliars.Count >= 10 - giftPrisms.Count))
		{
			return false;
		}
		
		DataRegistry registry = session.registry;
		
		foreach (var item in takeItems)
		{
			RItemData data = registry.Item(item.Key);
			
			if (data == null)
			{
				continue;
			}
			
			int have;
			
			if (data.stackable)
			{
				have = session.itemStacks.TryGetValue(data.id, out int n)? n : 0;
			}
			else
			{
				have = 0;
				
				foreach (var unq in session.uniqueItems)
				{
					if (unq?.data == data)
					{
						have++;
					}
				}
			}
			
			if (have < item.Value)
			{
				return false;
			}
		}
		
		foreach (var flag in setFlags)
		{
			session.SetFlag(flag);
		}
		
		foreach (var flag in clearFlags)
		{
			session.ClearFlag(flag);
		}
		
		foreach (var item in giftItems)
		{
			RItemData data = registry.Item(item.Key);
			
			if (data == null)
			{
				continue;
			}
			
			session.AddItem(data, item.Value);
		}
		
		foreach (var item in takeItems)
		{
			RItemData data = registry.Item(item.Key);
			
			if (data == null)
			{
				continue;
			}
			
			session.RemoveItem(data, item.Value);
		}
		
		foreach (var crystal in giftCrystals)
		{
			RQualiaCrystal data = registry.Crystal(crystal.Key);
			
			if (data == null)
			{
				continue;
			}
			
			session.AddCrystal(data, crystal.Value);
		}
		
		foreach (var prism in giftPrisms)
		{
			if (prism == null)
			{
				continue;
			}
			
			session.playerProjector.GiveFamiliar(prism);
		}
		
		if (giftGeneric > 0)
		{
			session.qualiaGeneric += giftGeneric;
		}
		
		if (!string.IsNullOrEmpty(onceFlag))
		{
			session.SetFlag(onceFlag);
		}
		
		return true;
	}
}
