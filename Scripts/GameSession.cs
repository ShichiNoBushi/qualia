using Godot;
using System;

public partial class GameSession : Node
{
	public Projector playerProjector {get; set;}
	public int qualiaGeneric {get; set;}
	public Godot.Collections.Dictionary<string, int> qualiaCrystals {get; set;}
	public Godot.Collections.Dictionary<string, int> itemStacks {get; set;}
	public Godot.Collections.Array<ItemInstance> uniqueItems {get; set;}
	public string returnPath {get; set;}
	public Vector2 returnPosition {get; set;}
	public Vector2 returnFacing {get; set;}
	public string safePath {get; set;}
	public Vector2 safePosition {get; set;}
	public Vector2 safeFacing {get; set;}
	public REncounterData pendingEncounter {get; set;}
	public BattleManager.VictoryResult lastResult {get; set;}
	public GameMode gameMode {get; set;}
	
	public Godot.Collections.Array<string> openedChests {get; set;}
	
	public enum GameMode
	{
		World,
		Dialog,
		Battle
	}
	
	public override void _Ready()
	{
		qualiaGeneric = 0;
		qualiaCrystals = new();
		
		RProjectorData pData = GD.Load<RProjectorData>("res://Resources/test_projector.tres");
		RFamiliarInstance gnomeInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_gnome.tres");
		RFamiliarInstance salamanderInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_salamander.tres");
		RFamiliarInstance sylphInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_sylph.tres");
		RFamiliarInstance undineInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_undine.tres");
		
		RSpellData boltSpell = GD.Load<RSpellData>("res://Resources/Spells/Bolt.tres");
		RSpellData recoverSpell = GD.Load<RSpellData>("res://Resources/Spells/Recover.tres");
		
		if (pData == null)
		{
			GD.Print("BattleManager - Failed to start: invalid projector data");
			return;
		}
		
		playerProjector = new();
		playerProjector.Initialize(pData);
		
		gnomeInst.Initialize();
		salamanderInst.Initialize();
		sylphInst.Initialize();
		undineInst.Initialize();
		
		playerProjector.GiveFamiliar(gnomeInst);
		playerProjector.GiveFamiliar(salamanderInst);
		playerProjector.GiveFamiliar(sylphInst);
		playerProjector.GiveFamiliar(undineInst);
		
		playerProjector.LearnSpell(boltSpell);
		playerProjector.LearnSpell(recoverSpell);
		
		gameMode = GameMode.World;
	}
	
	public void AddCrystal(RQualiaCrystal crystal, int amount = 1)
	{
		if (crystal == null || amount <= 0 || string.IsNullOrEmpty(crystal.id))
		{
			return;
		}
		
		string key = crystal.id;
		int current = qualiaCrystals.TryGetValue(key, out int n) ? n : 0;
		qualiaCrystals[key] = current + amount;
	}
	
	public bool RemoveCrystal(RQualiaCrystal crystal, int amount = 1)
	{
		if (crystal == null || amount <= 0 || string.IsNullOrEmpty(crystal.id))
		{
			return false;
		}
		
		string key = crystal.id;
		
		int count = 0;
		
		if (qualiaCrystals.TryGetValue(key, out count))
		{
			if (count >= amount)
			{
				qualiaCrystals[key] -= amount;
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	public void AddItem(RItemData item, int amount = 1)
	{
		if (item == null || amount <= 0 || string.IsNullOrEmpty(item.id))
		{
			return;
		}
		
		string id = item.id;
		
		if (item.stackable)
		{
			if (itemStacks == null)
			{
				itemStacks = new();
			}
			
			int current = itemStacks.TryGetValue(id, out int n) ? n : 0;
			itemStacks[id] = current + amount;
			return;
		}
		
		if (uniqueItems == null)
		{
			uniqueItems = new();
		}
		
		for (int i = 0; i < amount; i++)
		{
			uniqueItems.Add(new ItemInstance
			{
				data = item,
				maxUses = item.uses,
				usesLeft = item.uses
			});
		}
	}
	
	public bool TryUseItem(RItemData item, ItemInstance unique = null)
	{
		string id = item.id;
		
		if (item.stackable)
		{
			if (!itemStacks.TryGetValue(id, out int n) || n <= 0)
			{
				return false;
			}
			
			itemStacks[id] = n - 1;
			
			if (itemStacks[id] <= 0)
			{
				itemStacks.Remove(id);
			}
			
			return true;
		}
		
		if (unique == null || unique.usesLeft <= 0)
		{
			return false;
		}
		
		unique.usesLeft--;
		return true;
	}
	
	public bool IsChestOpened(string chestId) => !string.IsNullOrEmpty(chestId) && openedChests.Contains(chestId);
	
	public void MarkChestOpened(string chestId)
	{
		if (string.IsNullOrEmpty(chestId) || openedChests.Contains(chestId))
		{
			return;
		}
		
		openedChests.Add(chestId);
	}
	
	public void GiveLoot(Godot.Collections.Dictionary<string, int> loot)
	{
		if (loot == null || loot.Count == 0)
		{
			return;
		}
		
		DataRegistry registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		foreach (var l in loot)
		{
			if (l.Key.StartsWith("q_crystal:"))
			{
				string id = l.Key.Substring(10);
				RQualiaCrystal crystal = registry.Crystal(id);
				AddCrystal(crystal, l.Value);
			}
			else if (l.Key == "generic")
			{
				qualiaGeneric += l.Value;
			}
			else
			{
				RItemData item = registry.Item(l.Key);
				AddItem(item, l.Value);
			}
		}
	}
}
