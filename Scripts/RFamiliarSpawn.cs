using Godot;
using System;

[GlobalClass]
public partial class RFamiliarSpawn : Resource
{
	[Export] public RFamiliarData familiar {get; set;}
	[Export] public int minLevel {get; set;} = 1;
	[Export] public int maxLevel {get; set;} = 1;
	
	public RFamiliarInstance CreateInstance()
	{
		if (familiar == null)
		{
			GD.PrintErr("RFamiliarSpawn.CreateInstance: familiar data is null");
			return null;
		}
		
		RFamiliarInstance instance = new();
		
		int level = maxLevel > minLevel ? (int)(GD.Randi() % (maxLevel - minLevel + 1) + minLevel) : minLevel;
		instance.Initialize(familiar, level);
		
		return instance;
	}
}
