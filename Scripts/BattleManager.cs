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

public class BattleSide
{
	public Projector projector;
	public Godot.Collections.Array<string> familiarSlots;
	
	public BattleSide(Projector p)
	{
		projector = p;
		
		familiarSlots = new();
	}
}
