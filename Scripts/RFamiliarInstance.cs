using Godot;
using System;

[GlobalClass]
public partial class RFamiliarInstance : Resource
{
	[Export] public RFamiliarData data {get; set;}
	[Export] public string nickname {get; set;}
	
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
		
		for (int i = 1; i <= level; i++)
		{
			RSkillData skill = data.SkillAt(i);
			
			if (skill != null && !skills.Contains(skill))
			{
				skills.Add(skill);
			}
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
		
		RSkillData skill = data.SkillAt(level);
		
		if (skill != null && !skills.Contains(skill))
		{
			skills.Add(skill);
		}
		
		RecalculateStats();
	}
	
	public void SetLevel(int newLevel)
	{
		int oldLevel = level;
		level = Mathf.Max(newLevel, 1);
		
		for (int i = oldLevel + 1; i <= level; i++)
		{
			RSkillData skill = data.SkillAt(i);
			
			if (skill != null && !skills.Contains(skill))
			{
				skills.Add(skill);
			}
		}
		
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
		return string.IsNullOrEmpty(nickname) ? (string.IsNullOrEmpty(data?.name) ? "(No name)" : data.name) : nickname;
	}
}

public class FamiliarSave
{
	public string familiarId;
	public string nickname;
	
	public System.Collections.Generic.List<string> types;
	
	public int level;
	public int experience;
	
	public int pAttackBonus;
	public int mAttackBonus;
	public int pDefenseBonus;
	public int mDefenseBonus;
	public int speedBonus;
	
	public System.Collections.Generic.List<string> skills;
	
	public FamiliarSave()
	{
		
	}
	
	public FamiliarSave(RFamiliarInstance fam)
	{
		RFamiliarData data = fam.data;
		familiarId = data.id;
		nickname = fam.nickname;
		
		types = new();
		
		foreach (var tp in fam.types)
		{
			types.Add(tp.id);
		}
		
		level = fam.level;
		experience = fam.experience;
		
		pAttackBonus = fam.pAttackBonus;
		mAttackBonus = fam.mAttackBonus;
		pDefenseBonus = fam.pDefenseBonus;
		mDefenseBonus = fam.mDefenseBonus;
		speedBonus = fam.speedBonus;
		
		skills = new();
		
		foreach (var skl in fam.skills)
		{
			skills.Add(skl.id);
		}
	}
	
	public RFamiliarInstance ToFamiliar(DataRegistry registry)
	{
		RFamiliarData data = registry.Familiar(familiarId);
		
		if (data == null)
		{
			return null;
		}
		
		RFamiliarInstance fam = new();
		
		fam.data = data;
		fam.nickname = nickname;
		
		fam.types = new();
		
		foreach (var id in types)
		{
			RTypeData type = registry.Type(id);
			
			if (type != null)
			{
				fam.types.Add(type);
			}
		}
		
		fam.level = Mathf.Max(level, 1);
		fam.experience = Mathf.Max(experience, 0);
		
		fam.pAttackBonus = pAttackBonus;
		fam.mAttackBonus = mAttackBonus;
		fam.pDefenseBonus = pDefenseBonus;
		fam.mDefenseBonus = mDefenseBonus;
		fam.speedBonus = speedBonus;
		
		fam.skills = new();
		
		foreach (var id in skills)
		{
			RSkillData skill = registry.Skill(id);
			
			if (skill != null)
			{
				fam.skills.Add(skill);
			}
		}
		
		fam.RecalculateStats();
		
		return fam;
	}
	
	public Godot.Collections.Dictionary ToDictionary()
	{
		Godot.Collections.Array typesOut = new();
		
		foreach (var id in types)
		{
			typesOut.Add(id);
		}
		
		Godot.Collections.Array skillsOut = new();
		
		foreach (var id in skills)
		{
			skillsOut.Add(id);
		}
		
		return new Godot.Collections.Dictionary
		{
			{"familiarId", familiarId},
			{"nickname", nickname},
			{"types", typesOut},
			{"level", level},
			{"experience", experience},
			{"pAttackBonus", pAttackBonus},
			{"mAttackBonus", mAttackBonus},
			{"pDefenseBonus", pDefenseBonus},
			{"mDefenseBonus", mDefenseBonus},
			{"speedBonus", speedBonus},
			{"skills", skillsOut}
		};
	}
	
	public static FamiliarSave FromDictionary(Godot.Collections.Dictionary dict)
	{
		FamiliarSave familiar = new();
		
		familiar.familiarId = dict.TryGetValue("familiarId", out Variant id) ? id.AsString() : "";
		familiar.nickname = dict.TryGetValue("nickname", out Variant nick) ? nick.AsString() : "";
		
		familiar.types = new();
		
		Godot.Collections.Array typesIn = dict.TryGetValue("types", out Variant tIn) ? tIn.AsGodotArray() : new();
		
		foreach (Variant tId in typesIn)
		{
			familiar.types.Add(id.AsString());
		}
		
		familiar.level = dict.TryGetValue("level", out Variant lvl) ? lvl.AsInt32() : 1;
		familiar.experience = dict.TryGetValue("experience", out Variant exp) ? exp.AsInt32() : 0;
		
		familiar.pAttackBonus = dict.TryGetValue("pAttackBonus", out Variant pAtk) ? pAtk.AsInt32() : 0;
		familiar.mAttackBonus = dict.TryGetValue("mAttackBonus", out Variant mAtk) ? mAtk.AsInt32() : 0;
		familiar.pDefenseBonus = dict.TryGetValue("pDefenseBonus", out Variant pDef) ? pDef.AsInt32() : 0;
		familiar.mDefenseBonus = dict.TryGetValue("mDefenseBonus", out Variant mDef) ? mDef.AsInt32() : 0;
		familiar.speedBonus = dict.TryGetValue("speedBonus", out Variant spd) ? spd.AsInt32() : 0;
		
		familiar.skills = new();
		
		Godot.Collections.Array skillsIn = dict.TryGetValue("skills", out Variant skIn) ? skIn.AsGodotArray() : new();
		
		foreach (Variant sId in skillsIn)
		{
			familiar.skills.Add(id.AsString());
		}
		
		return familiar;
	}
}
