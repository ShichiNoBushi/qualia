using Godot;
using System;

public partial class Projector : Node
{
	public RProjectorData data {get; set;}
	
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
}
