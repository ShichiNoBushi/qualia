using Godot;
using System;

[GlobalClass]
public partial class RFamiliarData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public string description {get; set;} = "";
	
	[Export] public int baseEnergy {get; set;} = 50;
	[Export] public int basePAttack {get; set;} = 10;
	[Export] public int baseMAttack {get; set;} = 10;
	[Export] public int basePDefense {get; set;} = 10;
	[Export] public int baseMDefense {get; set;} = 10;
	[Export] public int baseSpeed {get; set;} = 10;
	
	[Export] public float levelEnergy {get; set;} = 1f;
	[Export] public float levelPAttack {get; set;} = 0.5f;
	[Export] public float levelMAttack {get; set;} = 0.5f;
	[Export] public float levelPDefense {get; set;} = 0.5f;
	[Export] public float levelMDefense {get; set;} = 0.5f;
	[Export] public float levelSpeed {get; set;} = 0.5f;
	
	[Export] public Godot.Collections.Array<RTypeData> types {get; set;} = new();
	[Export] public Godot.Collections.Array<RSkillData> learnableSkills {get; set;} = new();
	
	[Export] public RPrismRecipe recipe {get; set;} = new();
	
	[Export] public Texture2D portrait {get; set;}
	
	/*public FamiliarData()
	{
		id = "";
		name = "";
		description = "";
		
		baseEnergy = 50;
		basePAttack = 10;
		baseMAttack = 10;
		basePDefense = 10;
		baseMDefense = 10;
		baseSpeed = 10;
		
		types = new();
		learnableSkills = new();
		
		recipe = new();
		
		portrait = null;
	}*/
}
