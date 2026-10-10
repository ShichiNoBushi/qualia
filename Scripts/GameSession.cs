using Godot;
using System;

public partial class GameSession : Node
{
	public DataRegistry registry;
	
	[Export] public Godot.Collections.Dictionary<string, string> mapScenes {get; set;}
	
	public Projector playerProjector {get; set;}
	public Player player {get; set;}
	public int qualiaGeneric {get; set;}
	public Godot.Collections.Dictionary<string, int> qualiaCrystals {get; set;}
	public Godot.Collections.Dictionary<string, int> itemStacks {get; set;}
	public Godot.Collections.Array<ItemInstance> uniqueItems {get; set;}
	
	public string returnPath {get; set;}
	public Vector2 returnPosition {get; set;}
	public Vector2 returnFacing {get; set;}
	public string safePath {get; set;}
	public string safeMapId {get; set;}
	public string safeMarkerId {get; set;}
	public Vector2 safePosition {get; set;}
	public Vector2 safeFacing {get; set;}
	
	public REncounterData pendingEncounter {get; set;}
	public string pendingWarp {get; set;}
	
	public Vector2 pendingLoadPosition {get; set;}
	public Vector2 pendingLoadFacing {get; set;}
	
	public BattleManager.VictoryResult lastResult {get; set;}
	
	public GameMode gameMode {get; set;}
	
	public Godot.Collections.Array<string> openedChests {get; set;}
	
	public enum GameMode
	{
		World,
		Dialog,
		Shop,
		Battle
	}
	
	public override void _Ready()
	{
		registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		qualiaGeneric = 0;
		itemStacks = new();
		uniqueItems = new();
		qualiaCrystals = new();
		
		openedChests = new();
		
		RProjectorData pData = registry.Projector("player");
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
	
	public string SceneForMap(string mapId) =>
		mapScenes != null && mapScenes.TryGetValue(mapId, out string p) ? p : null;
	
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
	
	public void RemoveItem(RItemData item, int amount = 1)
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
				return;
			}
			
			int current = itemStacks.TryGetValue(id, out int n) ? n : 0;
			itemStacks[id] = Mathf.Max(current - amount, 0);
			
			if (itemStacks[id] == 0)
			{
				itemStacks.Remove(id);
			}
			
			return;
		}
		
		if (uniqueItems == null)
		{
			return;
		}
		
		for (int i = 0; i < amount; i++)
		{
			//remove instance of item from uniqueItems
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
	
	public void SaveGame(string path)
	{
		GameSave save = new(this, player);
		
		var dict = save.ToDictionary();
		string json = Json.Stringify(dict, "\t");
		
		using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		
		if (file == null)
		{
			GD.PrintErr(FileAccess.GetOpenError());
			return;
		}
		
		file.StoreString(json);
	}
	
	public void LoadGame(string path)
	{
		using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		
		if (file == null)
		{
			GD.PrintErr(FileAccess.GetOpenError());
			return;
		}
		
		Variant parsed = Json.ParseString(file.GetAsText());
		
		if (parsed.VariantType != Variant.Type.Dictionary)
		{
			return;
		}
		
		var dict = parsed.AsGodotDictionary();
		
		GameSave save = GameSave.FromDictionary(dict);
		
		Projector proj = save.playerProjector.ToProjector(registry);
		
		if (proj == null)
		{
			GD.PrintErr("GameSession: failure loading projector");
			return;
		}
		
		playerProjector = proj;
		qualiaGeneric = save.qualiaGeneric;
		qualiaCrystals = save.qualiaCrystals ?? new();
		itemStacks = save.itemStacks ?? new();
		
		uniqueItems = new();
		
		foreach (var item in save.uniqueItems)
		{
			ItemInstance inst = item.ToItem(registry);
			
			if (inst != null)
			{
				uniqueItems.Add(inst);
			}
		}
		
		openedChests = new();
		
		foreach (var id in save.openedChests)
		{
			openedChests.Add(id);
		}
		
		returnPath = save.returnPath;
		returnPosition = new Vector2(save.returnPositionX, save.returnPositionY);
		returnFacing = new Vector2(save.returnFacingX, save.returnFacingY);
		
		safePath = save.safePath;
		safeMapId = save.safeMapId;
		safeMarkerId = save.safeMarkerId;
		safePosition = new Vector2(save.safePositionX, save.safePositionY);
		safeFacing = new Vector2(save.safeFacingX, save.safeFacingY);
		
		pendingLoadPosition = new Vector2(save.savePositionX, save.savePositionY);
		pendingLoadFacing = new Vector2(save.saveFacingX, save.saveFacingY);
		
		gameMode = GameMode.World;
		pendingEncounter = null;
		pendingWarp = "";
		
		if (string.IsNullOrEmpty(save.savePath))
		{
			GD.PrintErr("GameSession: no map path");
			return;
		}
		
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile(save.savePath);
	}
}

public class GameSave
{
	public ProjectorSave playerProjector;
	public int qualiaGeneric;
	public Godot.Collections.Dictionary<string, int> qualiaCrystals;
	public Godot.Collections.Dictionary<string, int> itemStacks;
	public System.Collections.Generic.List<ItemSave> uniqueItems;
	
	public string savePath;
	public float savePositionX;
	public float savePositionY;
	public float saveFacingX;
	public float saveFacingY;
	
	public string returnPath;
	public float returnPositionX;
	public float returnPositionY;
	public float returnFacingX;
	public float returnFacingY;
	
	public string safePath;
	public string safeMapId;
	public string safeMarkerId;
	public float safePositionX;
	public float safePositionY;
	public float safeFacingX;
	public float safeFacingY;
	
	public Godot.Collections.Array<string> openedChests;
	
	public GameSave()
	{
		
	}
	
	public GameSave(GameSession session, Player player)
	{
		playerProjector = new(session.playerProjector);
		qualiaGeneric = session.qualiaGeneric;
		qualiaCrystals = session.qualiaCrystals.Duplicate();
		itemStacks = session.itemStacks.Duplicate();
		
		uniqueItems = new();
		
		foreach (var it in session.uniqueItems)
		{
			ItemSave item = new(it);
			
			uniqueItems.Add(item);
		}
		
		savePath = player.GetTree().CurrentScene.SceneFilePath;
		savePositionX = player.GlobalPosition.X;
		savePositionY = player.GlobalPosition.Y;
		saveFacingX = player.facing.X;
		saveFacingY = player.facing.Y;
		
		returnPath = session.returnPath;
		returnPositionX = session.returnPosition.X;
		returnPositionY = session.returnPosition.Y;
		returnFacingX = session.returnFacing.X;
		returnFacingY = session.returnFacing.Y;
		
		safePath = session.safePath;
		safeMapId = session.safeMapId;
		safeMarkerId = session.safeMarkerId;
		safePositionX = session.safePosition.X;
		safePositionY = session.safePosition.Y;
		safeFacingX = session.safeFacing.X;
		safeFacingY = session.safeFacing.Y;
		
		openedChests = session.openedChests.Duplicate();
	}
	
	public Godot.Collections.Dictionary ToDictionary()
	{
		Godot.Collections.Dictionary crystals = new();
		
		foreach (var pair in qualiaCrystals)
		{
			crystals[pair.Key] = pair.Value;
		}
		
		Godot.Collections.Dictionary stacks = new();
		
		foreach (var pair in itemStacks)
		{
			stacks[pair.Key] = pair.Value;
		}
		
		Godot.Collections.Array uniques = new();
		
		foreach (var item in uniqueItems)
		{
			uniques.Add(item.ToDictionary());
		}
		
		Godot.Collections.Array chests = new();
		
		foreach (var id in openedChests)
		{
			chests.Add(id);
		}
		
		return new Godot.Collections.Dictionary
		{
			{"version", 1},
			{"playerProjector", playerProjector.ToDictionary()},
			{"qualiaGeneric", qualiaGeneric},
			{"qualiaCrystals", crystals},
			{"itemStacks", stacks},
			{"uniqueItems", uniques},
			{"savePath", savePath},
			{"savePositionX", savePositionX}, {"savePositionY", savePositionY},
			{"saveFacingX", saveFacingX}, {"saveFacingY", saveFacingY},
			{"returnPath", returnPath},
			{"returnPositionX", returnPositionX}, {"returnPositionY", returnPositionY},
			{"returnFacingX", returnFacingX}, {"returnFacingY", returnFacingY},
			{"safePath", safePath},
			{"safeMapId", safeMapId},
			{"safeMarkerId", safeMarkerId},
			{"safePositionX", safePositionX}, {"safePositionY", safePositionY},
			{"safeFacingX", safeFacingX}, {"safeFacingY", safeFacingY},
			{"openedChests", chests}
		};
	}
	
	public static GameSave FromDictionary(Godot.Collections.Dictionary dict)
	{
		GameSave save = new();
		
		if (dict.TryGetValue("playerProjector", out Variant proj) && proj.VariantType == Variant.Type.Dictionary)
		{
			save.playerProjector = ProjectorSave.FromDictionary(proj.AsGodotDictionary());
		}
		
		save.qualiaGeneric = dict.TryGetValue("qualiaGeneric", out Variant qg) ? qg.AsInt32() : 0;
		
		save.qualiaCrystals = new();
		
		if (dict.TryGetValue("qualiaCrystals", out Variant crystals) && crystals.VariantType == Variant.Type.Dictionary)
		{
			foreach (var pair in crystals.AsGodotDictionary())
			{
				save.qualiaCrystals[pair.Key.AsString()] = pair.Value.AsInt32();
			}
		}
		
		save.itemStacks = new();
		
		if (dict.TryGetValue("itemStacks", out Variant stacks) && stacks.VariantType == Variant.Type.Dictionary)
		{
			foreach (var pair in stacks.AsGodotDictionary())
			{
				save.itemStacks[pair.Key.AsString()] = pair.Value.AsInt32();
			}
		}
		
		save.uniqueItems = new();
		
		if (dict.TryGetValue("uniqueItems", out Variant uniques) && uniques.VariantType == Variant.Type.Array)
		{
			foreach (Variant item in uniques.AsGodotArray())
			{
				if (item.VariantType == Variant.Type.Dictionary)
				{
					save.uniqueItems.Add(ItemSave.FromDictionary(item.AsGodotDictionary()));
				}
			}
		}
		
		save.savePath = dict.TryGetValue("savePath", out Variant svp) ? svp.AsString() : "";
		save.savePositionX = dict.TryGetValue("savePositionX", out Variant svpx) ? svpx.AsSingle() : 0f;
		save.savePositionY = dict.TryGetValue("savePositionY", out Variant svpy) ? svpy.AsSingle() : 0f;
		save.saveFacingX = dict.TryGetValue("saveFacingX", out Variant svfx) ? svfx.AsSingle() : 0f;
		save.saveFacingY = dict.TryGetValue("saveFacingY", out Variant svfy) ? svfy.AsSingle() : 1f;
		
		save.returnPath = dict.TryGetValue("returnPath", out Variant rtp) ? rtp.AsString() : "";
		save.returnPositionX = dict.TryGetValue("returnPositionX", out Variant rtpx) ? rtpx.AsSingle() : 0f;
		save.returnPositionY = dict.TryGetValue("returnPositionY", out Variant rtpy) ? rtpy.AsSingle() : 0f;
		save.returnFacingX = dict.TryGetValue("returnFacingX", out Variant rtfx) ? rtfx.AsSingle() : 0f;
		save.returnFacingY = dict.TryGetValue("returnFacingY", out Variant rtfy) ? rtfy.AsSingle() : 1f;
		
		save.safePath = dict.TryGetValue("safePath", out Variant sfp) ? sfp.AsString() : "";
		save.safeMapId = dict.TryGetValue("safeMapId", out Variant sfmpid) ? sfmpid.AsString() : "";
		save.safeMarkerId = dict.TryGetValue("safeMarkerId", out Variant sfmkid) ? sfmkid.AsString() : "";
		save.safePositionX = dict.TryGetValue("safePositionX", out Variant sfpx) ? sfpx.AsSingle() : 0f;
		save.safePositionY = dict.TryGetValue("safePositionY", out Variant sfpy) ? sfpy.AsSingle() : 0f;
		save.safeFacingX = dict.TryGetValue("safeFacingX", out Variant sffx) ? sffx.AsSingle() : 0f;
		save.safeFacingY = dict.TryGetValue("safeFacingY", out Variant sffy) ? sffy.AsSingle() : 1f;
		
		save.openedChests = new();
		
		if (dict.TryGetValue("openedChests", out Variant chests) && chests.VariantType == Variant.Type.Array)
		{
			foreach (Variant id in chests.AsGodotArray())
			{
				save.openedChests.Add(id.AsString());
			}
		}
		
		return save;
	}
}
