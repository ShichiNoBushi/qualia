using Godot;
using System;

public partial class BattleManager : Node
{
	public bool isProjectorEncounter;
	
	public BattleState state;
	
	public enum BattleState
	{
		Setup,
		CommandSelect,
		Resolution,
		SpawnCheck,
		EndCheck,
		Cleanup
	}
	
	public BattleSide playerSide;
	public BattleSide enemySide;
	
	public Godot.Collections.Array<RFamiliarInstance> spawns;
	
	public Godot.Collections.Array<string> projectorCommands;
	public Godot.Collections.Array<string> familiarCommands;
	public Godot.Collections.Array<string> turnCommands;
	
	public Godot.Collections.Array<RFamiliarInstance> defeatedFamiliars;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Initialize(Projector player, REncounterData encounter)
	{
		if (!encounter.IsValid())
		{
			return;
		}
		
		isProjectorEncounter = encounter.isProjectorEncounter;
		
		playerSide = new(player);
		
		if (isProjectorEncounter)
		{
			Projector enemy = new();
			enemy.Initialize(encounter.enemyProjector);
			
			foreach (var fam in encounter.familiarList)
			{
				RFamiliarInstance familiar = fam.CreateInstance();
				enemy.GiveFamiliar(familiar);
			}
		}
		else
		{
			foreach (var fam in encounter.familiarList)
			{
				RFamiliarInstance familiar = fam.CreateInstance();
				spawns.Add(familiar);
			}
		}
	}
}

public partial class BattleSide : RefCounted
{
	public const int MAX_SLOTS = 4;
	
	public Projector projector {get; set;}
	public object[] familiarSlots {get; set;} = new object[MAX_SLOTS];
	
	public BattleSide(Projector p)
	{
		projector = p;
		
		familiarSlots = new object[MAX_SLOTS];
	}
	
	public bool IsSlotEmpty(int index)
	{
		return index >= 0 && index < MAX_SLOTS && familiarSlots[index] == null;
	}
	
	public bool HasOpenSlot()
	{
		for (int i = 0; i < MAX_SLOTS; i++)
		{
			if (familiarSlots[i] != null)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public int GetSlotIndex(RFamiliarInstance familiar)
	{
		if (familiar == null)
		{
			return -1;
		}
		
		for (int i = 0; i < MAX_SLOTS; i ++)
		{
			if (familiarSlots[i] == familiar)
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public bool TrySummon(RFamiliarInstance familiar, int index)
	{
		if (index < 0 || index >= MAX_SLOTS || familiarSlots[index] != null)
		{
			return false;
		}
		
		familiarSlots[index] = familiar;
		
		return true;
	}
	
	public void ClearSlot(int index)
	{
		if (index >= 0 && index < MAX_SLOTS)
		{
			familiarSlots[index] = null;
		}
	}
	
	public int CountActiveFamiliars()
	{
		int count = 0;
		
		foreach (var slot in familiarSlots)
		{
			if (slot != null)
			{
				count++;
			}
		}
		
		return count;
	}
	
	public Godot.Collections.Array<RFamiliarInstance> GetFamiliarList()
	{
		Godot.Collections.Array<RFamiliarInstance> famList = new();
		
		foreach (var slot in familiarSlots)
		{
			if (slot != null)
			{
				famList.Add((RFamiliarInstance)slot);
			}
		}
		
		return famList;
	}
}

public abstract partial class BattleCommand : RefCounted
{
	public BattleSide sourceSide {get; set;}
	public object source {get; set;}
	public object target {get; set;}
	
	public bool isValid {get; set;} = true;
	
	public abstract void Execute(BattleManager battle);
	
	public virtual void Retarget(BattleManager battle)
	{
		
	}
}

public partial class SummonCommand : BattleCommand
{
	public RFamiliarInstance familiar {get; set;}
	public int slot {get; set;} = -1;
	
	public override void Execute(BattleManager battle)
	{
		if (source is not Projector projector)
		{
			return;
		}
		
		if (!sourceSide.HasOpenSlot())
		{
			return;
		}
		
		if (slot <= 0 || slot > BattleSide.MAX_SLOTS || !sourceSide.IsSlotEmpty(slot))
		{
			for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
			{
				if (sourceSide.IsSlotEmpty(i))
				{
					slot = i;
					break;
				}
			}
		}
		
		int cost = familiar.energy;
		
		if (projector.currentEnergy < cost)
		{
			return;
		}
		
		projector.currentEnergy -= cost;
		
		if (!sourceSide.TrySummon(familiar, slot))
		{
			GD.Print("SummonCommand: Failed to summon");
		}
	}
}

public partial class AttackCommand : BattleCommand
{
	public int power {get; set;} = 0;
	
	public override void Execute(BattleManager battle)
	{
		if (target == null)
		{
			Retarget(battle);
			
			if (target == null)
			{
				return;
			}
		}
		
		//calculate damage
		//damage target
	}
	
	public override void Retarget(BattleManager battle)
	{
		BattleSide enemySide = sourceSide == battle.playerSide ? battle.enemySide : battle.playerSide;
		
		if (enemySide.CountActiveFamiliars() > 0)
		{
			Godot.Collections.Array<RFamiliarInstance> famList = enemySide.GetFamiliarList();
			
			int idx = (int)GD.Randi() % famList.Count;
			
			target = famList[idx];
		}
		else if (enemySide.projector != null && enemySide.projector.currentEnergy > 0)
		{
			target = enemySide.projector;
		}
		else
		{
			isValid = false;
		}
	}
}

public partial class FocusCommand : BattleCommand
{
	public override void Execute(BattleManager battle)
	{
		int amount = (int)GD.Randi() % 10 + 1;
		
		if (source is Projector projector)
		{
			projector.currentEnergy = Mathf.Min(projector.maxEnergy, projector.currentEnergy + amount);
		}
	}
}
