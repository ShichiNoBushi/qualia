using Godot;
using System;

public partial class GameSession : Node
{
	public Projector playerProjector {get; set;}
	public int qualiaGeneric {get; set;}
	public Godot.Collections.Dictionary<string, int> qualiaCrystals {get; set;}
	public string returnPath {get; set;}
	public Vector2 returnPosition {get; set;}
	public Vector2 returnFacing {get; set;}
	public string safePath {get; set;}
	public Vector2 safePosition {get; set;}
	public Vector2 safeFacing {get; set;}
	public REncounterData pendingEncounter {get; set;}
	public BattleManager.VictoryResult lastResult {get; set;}
	public GameMode gameMode {get; set;}
	
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
		if (crystal == null || amount <= 0)
		{
			return;
		}
		
		string key = crystal.id;
		int current = qualiaCrystals.TryGetValue(key, out int n) ? n : 0;
		qualiaCrystals[key] = current + amount;
	}
	
	public bool RemoveCrystal(RQualiaCrystal crystal, int amount = 1)
	{
		if (crystal == null || amount <= 0)
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
}
