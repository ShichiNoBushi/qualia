using Godot;
using System;

[GlobalClass]
public partial class REncounterData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	
	[Export] public bool isProjectorEncounter {get; set;} = false;
	[Export] public RProjectorData enemyProjector {get; set;}
	[Export] public Godot.Collections.Array<RFamiliarSpawn> familiarList {get; set;} = new();
	
	public bool IsValid()
	{
		if (isProjectorEncounter && enemyProjector == null)
		{
			return false;
		}
		
		if (familiarList == null || familiarList.Count == 0)
		{
			return false;
		}
		
		return true;
	}
}
