using Godot;
using System;
using System.Linq;

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
	
	public Godot.Collections.Array<BattleCommand> projectorCommands;
	public Godot.Collections.Array<BattleCommand> familiarCommands;
	public Godot.Collections.Array<BattleCommand> turnCommands;
	
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
		if (encounter == null || !encounter.IsValid())
		{
			return;
		}
		
		spawns = new();
		
		projectorCommands = new();
		familiarCommands = new();
		turnCommands = new();
		
		defeatedFamiliars = new();
		
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
			
			enemySide = new(enemy);
		}
		else
		{
			enemySide = new(null);
			
			foreach (var fam in encounter.familiarList)
			{
				RFamiliarInstance familiar = fam.CreateInstance();
				
				if (familiar != null)
				{
					spawns.Add(familiar);
				}
			}
		}
		
		state = BattleState.Setup;
	}
	
	public void SetState(BattleState newState)
	{
		state = newState;
	}
	
	public void BuildTurnOrder()
	{
		turnCommands.Clear();
		
		var playerProjCmds = projectorCommands.Where(c => c.sourceSide == playerSide);
		var enemyProjCmds = projectorCommands.Where(c => c.sourceSide == enemySide);
		
		if (GD.Randi() % 2 == 0)
		{
			turnCommands.AddRange(playerProjCmds);
			turnCommands.AddRange(enemyProjCmds);
		}
		else
		{
			turnCommands.AddRange(enemyProjCmds);
			turnCommands.AddRange(playerProjCmds);
		}
		
		var sortedFamiliarCmds = familiarCommands.OrderByDescending(cmd =>
		{
			int speed = 0;
			
			if (cmd.source is FamiliarActor fam)
			{
				speed = fam.speed; //change to modified speed
			}
			
			return speed * 100 + (int)GD.Randi() % 20;
		}).ToList();
		
		foreach (var cmd in sortedFamiliarCmds)
		{
			turnCommands.Add(cmd);
		}
	}
	
	public void ResolveTurn()
	{
		foreach (var cmd in turnCommands)
		{
			if (!cmd.isValid)
			{
				continue;
			}
			
			cmd.Retarget(this);
			
			if (cmd.isValid)
			{
				cmd.Execute(this);
			}
		}
	
		SetState(BattleState.EndCheck);
	}
}

public partial class BattleSide : RefCounted
{
	public const int MAX_SLOTS = 4;
	
	public Projector projector {get; set;}
	public FamiliarActor[] familiarSlots {get; set;} = new FamiliarActor[MAX_SLOTS];
	
	public BattleSide(Projector p)
	{
		projector = p;
		
		familiarSlots = new FamiliarActor[MAX_SLOTS];
	}
	
	public bool IsSlotEmpty(int index)
	{
		return index >= 0 && index < MAX_SLOTS && familiarSlots[index] == null;
	}
	
	public bool HasOpenSlot()
	{
		for (int i = 0; i < MAX_SLOTS; i++)
		{
			if (familiarSlots[i] == null)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public int GetSlotIndex(FamiliarActor familiar)
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
	
	public bool TrySummon(FamiliarActor familiar, int index)
	{
		if (index < 0 || index >= MAX_SLOTS || familiarSlots[index] != null)
		{
			return false;
		}
		
		familiarSlots[index] = familiar;
		familiar.side = this;
		
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
	
	public Godot.Collections.Array<FamiliarActor> GetFamiliarList()
	{
		Godot.Collections.Array<FamiliarActor> famList = new();
		
		foreach (var slot in familiarSlots)
		{
			if (slot != null)
			{
				famList.Add(slot);
			}
		}
		
		return famList;
	}
}

public interface IBattleActor
{
	string name {get;}
	int maxEnergy {get;}
	int currentEnergy {get; set;}
	int speed {get;}
	BattleSide side {get; set;}
	
	bool isAlive {get;}
	
	void Damage(int amount);
}

public partial class FamiliarActor : RefCounted, IBattleActor
{
	public RFamiliarInstance familiar {get; private set;}
	public string name {get; private set;}
	
	public BattleSide side {get; set;}
	
	public int currentEnergy {get; set;} = 1;
	public int maxEnergy => familiar.energy;
	
	public int pAttack => familiar.pAttack;
	public int mAttack => familiar.mAttack;
	public int pDefense => familiar.pDefense;
	public int mDefense => familiar.mDefense;
	public int speed => familiar.speed;
	
	public int pAttackBonus {get; set;} = 0;
	public int mAttackBonus {get; set;} = 0;
	public int pDefenseBonus {get; set;} = 0;
	public int mDefenseBonus {get; set;} = 0;
	public int speedBonus {get; set;} = 0;
	
	public bool isAlive => currentEnergy > 0;
	
	public FamiliarActor(RFamiliarInstance fam)
	{
		familiar = fam;
		name = string.IsNullOrEmpty(familiar.nickName) ? familiar.data.name : familiar.nickName;
		currentEnergy = maxEnergy;
	}
	
	public void Damage(int amount)
	{
		currentEnergy = Mathf.Max(currentEnergy - amount, 0);
	}
}
