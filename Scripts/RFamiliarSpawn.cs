using Godot;
using System;

[GlobalClass]
public partial class RFamiliarSpawn : Resource
{
	[Export] public RFamiliarData familiar {get; set;}
	[Export] public int level {get; set;} = 1;
	
	public RFamiliarInstance CreateInstance()
	{
		if (familiar == null)
		{
			GD.PrintErr("RFamiliarSpawn.CreateInstance: familiar data is null");
			return null;
		}
		
		RFamiliarInstance instance = new();
		instance.Initialize(familiar);
		
		instance.level = level;
		
		return instance;
	}
}
