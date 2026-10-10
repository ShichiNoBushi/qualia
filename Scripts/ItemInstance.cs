using Godot;
using System;

public partial class ItemInstance : RefCounted
{
	public RItemData data;
	public int maxUses;
	public int usesLeft;
}

public class ItemSave
{
	public string itemId;
	public int maxUses;
	public int usesLeft;
	
	public ItemSave()
	{
		
	}
	
	public ItemSave(ItemInstance item)
	{
		itemId = item.data.id;
		maxUses = item.maxUses;
		usesLeft = item.usesLeft;
	}
	
	public ItemInstance ToItem(DataRegistry registry)
	{
		RItemData data = registry.Item(itemId);
		
		if (data == null)
		{
			return null;
		}
		
		ItemInstance item = new();
		
		item.data = data;
		item.maxUses = maxUses;
		item.usesLeft = usesLeft;
		
		return item;
	}
	
	public Godot.Collections.Dictionary ToDictionary()
	{
		return new Godot.Collections.Dictionary
		{
			{"itemId", itemId},
			{"maxUses", maxUses},
			{"usesLeft", usesLeft}
		};
	}
	
	public static ItemSave FromDictionary(Godot.Collections.Dictionary dict)
	{
		ItemSave item = new();
		
		item.itemId = dict.TryGetValue("itemId", out Variant id) ? id.AsString() : "";
		item.maxUses = dict.TryGetValue("maxUses", out Variant max) ? max.AsInt32() : 0;
		item.usesLeft = dict.TryGetValue("usesLeft", out Variant left) ? left.AsInt32() : 0;
		
		return item;
	}
}
