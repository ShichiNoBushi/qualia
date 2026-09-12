using Godot;
using System;

[GlobalClass]
public partial class RFamiliarInstance : Resource
{
	[Export] public RFamiliarData data {get; set;}
	[Export] public string nickName {get; set;}
	
	[Export] public Godot.Collections.Array<RTypeData> types {get; set;}
	
	[Export] public int level {get; set;} = 1;
	public int experience {get; set;} = 0;
	
	public int energy {get; private set;}
	public int pAttack {get; private set;}
	public int mAttack {get; private set;}
	public int pDefense {get; private set;}
	public int mDefense {get; private set;}
	public int speed {get; private set;}
	
	[Export] public int pAttackBonus {get; set;}
	[Export] public int mAttackBonus {get; set;}
	[Export] public int pDefenseBonus {get; set;}
	[Export] public int mDefenseBonus {get; set;}
	[Export] public int speedBonus {get; set;}
	
	[Export] public Godot.Collections.Array<RSkillData> skills {get; set;}
	
	public void Initialize()
	{
		RecalculateStats();
		
		foreach (var t in data.types)
		{
			if (!types.Contains(t))
			{
				types.Add(t);
			}
		}
	}
	
	public void Initialize(RFamiliarData fData, int startingLevel = 1)
	{
		data = fData;
		types = fData.types.Duplicate();
		level = Mathf.Max(startingLevel, 1);
		experience = 0;
		
		RecalculateStats();
		
		skills = new();
		
		foreach (var skill in data.learnableSkills)
		{
			skills.Add(skill);
		}
		
		foreach (var t in data.types)
		{
			if (!types.Contains(t))
			{
				types.Add(t);
			}
		}
	}
	
	public int GiveExperience(int expBonus)
	{
		if (expBonus <= 0)
		{
			return 0;
		}
		
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
		if (data == null)
		{
			return 1000;
		}
		
		return Math.Max(1, Mathf.RoundToInt(level * 1000f * data.expGrowthFactor));
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
		pAttack = data.basePAttack + (int)(data.levelPAttack * levelsAboveBase) + pAttackBonus;
		mAttack = data.baseMAttack + (int)(data.levelMAttack * levelsAboveBase) + mAttackBonus;
		pDefense = data.basePDefense + (int)(data.levelPDefense * levelsAboveBase) + pDefenseBonus;
		mDefense = data.baseMDefense + (int)(data.levelMDefense * levelsAboveBase) + mDefenseBonus;
		speed = data.baseSpeed + (int)(data.levelSpeed * levelsAboveBase) + speedBonus;
	}
	
	public string GetPreferredName()
	{
		return string.IsNullOrEmpty(nickName) ? (string.IsNullOrEmpty(data?.name) ? "(No name)" : data.name) : nickName;
	}
}
