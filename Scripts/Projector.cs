using Godot;
using System;

public partial class Projector : Node
{
	public RProjectorData data {get; set;}
	
	public string name {get; set;}
	
	public int level {get; set;}
	public int experience {get; set;}
	
	public int maxEnergy {get; set;}
	public int currentEnergy {get; set;}
	
	public Godot.Collections.Array<RFamiliarInstance> ownedFamiliars {get; set;}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Initialize(RProjectorData pData)
	{
		data = pData;
		
		name = data.name;
		
		level = 1;
		experience = 0;
		
		maxEnergy = pData.energy;
		currentEnergy = maxEnergy;
		
		ownedFamiliars = new();
	}
	
	public bool GiveFamiliar(RFamiliarInstance familiar)
	{
		if (familiar == null || ownedFamiliars.Count >= 10)
		{
			return false;
		}
		
		ownedFamiliars.Add(familiar);
		return true;
	}
	
	public RFamiliarInstance RemoveFamiliar(RFamiliarInstance familiar)
	{
		if (familiar == null || !ownedFamiliars.Contains(familiar))
		{
			return null;
		}
		
		ownedFamiliars.Remove(familiar);
		return familiar;
	}
	
	public void Damage(int amount)
	{
		currentEnergy = Mathf.Max(currentEnergy - amount, 0);
	}
	
	public void Restore(int amount)
	{
		currentEnergy = Math.Min(currentEnergy + amount, maxEnergy);
	}
	
	public void Recover()
	{
		currentEnergy = maxEnergy;
	}
	
	public int GiveExperience(int expBonus)
	{
		int oldLevel = level;
		
		experience += expBonus;
		
		while (experience >= ExpToNextLevel())
		{
			experience -= ExpToNextLevel();
			LevelUp();
		}
		
		return level - oldLevel;
	}
	
	public int ExpToNextLevel()
	{
		return Math.Max(1, level * 1000);
	}
	
	public void LevelUp()
	{
		level++;
		RecalculateEnergy();
		currentEnergy += data.levelEnergy;
	}
	
	public void SetLevel(int newLevel)
	{
		level = newLevel;
		RecalculateEnergy();
		currentEnergy = maxEnergy;
	}
	
	public void RecalculateEnergy()
	{
		int baseEnergy = data != null ? data.energy : 100;
		int growth = data != null ? data.levelEnergy : 10;
		maxEnergy = baseEnergy + growth * (level - 1);
	}
}
