using Godot;
using System;

[GlobalClass]
public partial class RFamiliarInstance : Resource
{
	[Export] public RFamiliarData data {get; set;}
	[Export] public string nickName {get; set;}
	
	[Export] public Godot.Collections.Array<RTypeData> types {get; set;}
	
	[Export] public int level {get; set;} = 1;
	[Export] public int experience {get; set;} = 0;
	
	[Export] public int energy {get; set;}
	[Export] public int pAttack {get; set;}
	[Export] public int mAttack {get; set;}
	[Export] public int pDefense {get; set;}
	[Export] public int mDefense {get; set;}
	[Export] public int speed {get; set;}
	
	[Export] public Godot.Collections.Array<RSkillData> skills {get; set;}
	
	public void Initialize(RFamiliarData fData)
	{
		data = fData;
		types = fData.types.Duplicate();
		
		energy = fData.baseEnergy;
		pAttack = fData.basePAttack;
		mAttack = fData.baseMAttack;
		pDefense = fData.basePDefense;
		mDefense = fData.baseMDefense;
		speed = fData.baseSpeed;
	}
}
