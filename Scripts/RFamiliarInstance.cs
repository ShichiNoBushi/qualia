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
	
	public void Initialize(RFamiliarData fData, int startingLevel = 1)
	{
		data = fData;
		types = fData.types.Duplicate();
		level = Mathf.Max(startingLevel, 1);
		experience = 0;
		
		RecalculateStats();
	}
	
	public void LevelUp()
	{
		level++;
		RecalculateStats();
	}
	
	public void SetLevel(int newLevel)
	{
		level = Mathf.Max(newLevel, 1);
		RecalculateStats();
	}
	
	public void RecalculateStats()
	{
		if (data == null)
		{
			return;
		}
		
		int levelsAboveBase = level - 1;
		
		energy = data.baseEnergy + (int)(data.levelEnergy * levelsAboveBase);
		pAttack = data.basePAttack + (int)(data.levelPAttack * levelsAboveBase);
		mAttack = data.baseMAttack + (int)(data.levelMAttack * levelsAboveBase);
		pDefense = data.basePDefense + (int)(data.levelPDefense * levelsAboveBase);
		mDefense = data.baseMDefense + (int)(data.levelMDefense * levelsAboveBase);
		speed = data.baseSpeed + (int)(data.levelSpeed * levelsAboveBase);
	}
}
