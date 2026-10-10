using Godot;
using System;

public partial class Projector : RefCounted
{
	public RProjectorData data {get; set;}
	
	public string name {get; set;}
	
	public int level {get; set;}
	public int experience {get; set;}
	
	public int maxEnergy {get; set;}
	public int currentEnergy {get; set;}
	
	public Godot.Collections.Array<RFamiliarInstance> ownedFamiliars {get; set;}
	public Godot.Collections.Array<RSpellData> spells {get; set;}
	
	public void Initialize(RProjectorData pData)
	{
		data = pData;
		
		name = data.name;
		
		level = 1;
		experience = 0;
		
		maxEnergy = pData.energy;
		currentEnergy = maxEnergy;
		
		ownedFamiliars = new();
		spells = new();
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
	
	public void LearnSpell(RSpellData spell)
	{
		if (spell != null && !spells.Contains(spell))
		{
			spells.Add(spell);
		}
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

public class ProjectorSave
{
	public string projId;
	public string name;
	public int level;
	public int experience;
	public int currentEnergy;
	public System.Collections.Generic.List<FamiliarSave> ownedFamiliars;
	public System.Collections.Generic.List<string> spells;
	
	public ProjectorSave()
	{
		
	}
	
	public ProjectorSave(Projector proj)
	{
		RProjectorData data = proj.data;
		
		projId = data.id;
		name = proj.name;
		level = proj.level;
		experience = proj.experience;
		currentEnergy = proj.currentEnergy;
		
		ownedFamiliars = new();
		
		foreach (var fam in proj.ownedFamiliars)
		{
			ownedFamiliars.Add(new FamiliarSave(fam));
		}
		
		spells = new();
		
		foreach (var spl in proj.spells)
		{
			spells.Add(spl.id);
		}
	}
	
	public Projector ToProjector(DataRegistry registry)
	{
		RProjectorData data = registry.Projector(projId);
		
		if (data == null)
		{
			return null;
		}
		
		Projector proj = new();
		
		proj.data = data;
		proj.name = name;
		
		proj.level = Mathf.Max(level, 1);
		proj.experience = Mathf.Max(experience, 0);
		
		proj.maxEnergy = data.energy + data.levelEnergy * (level - 1);
		proj.currentEnergy = Mathf.Min(currentEnergy, proj.maxEnergy);
		
		proj.ownedFamiliars = new();
		
		foreach (var fam in ownedFamiliars)
		{
			RFamiliarInstance inst = fam.ToFamiliar(registry);
			
			if (inst != null)
			{
				proj.ownedFamiliars.Add(inst);
			}
		}
		
		proj.spells = new();
		
		foreach (var id in spells)
		{
			RSpellData spell = registry.Spell(id);
			
			if (spell != null)
			{
				proj.spells.Add(spell);
			}
		}
		
		return proj;
	}
	
	public Godot.Collections.Dictionary ToDictionary()
	{
		Godot.Collections.Array familiars = new();
		
		foreach (var fam in ownedFamiliars)
		{
			familiars.Add(fam.ToDictionary());
		}
		
		Godot.Collections.Array spellsOut = new();
		
		foreach (var id in spells)
		{
			spellsOut.Add(id);
		}
		
		return new Godot.Collections.Dictionary
		{
			{"projId", projId},
			{"name", name},
			{"level", level},
			{"experience", experience},
			{"currentEnergy", currentEnergy},
			{"ownedFamiliars", familiars},
			{"spells", spellsOut}
		};
	}
	
	public static ProjectorSave FromDictionary(Godot.Collections.Dictionary dict)
	{
		ProjectorSave projector = new();
		
		projector.projId = dict.TryGetValue("projId", out Variant pId) ? pId.AsString() : "";
		projector.name = dict.TryGetValue("name", out Variant nm) ? nm.AsString() : "";
		
		projector.level = dict.TryGetValue("level", out Variant lvl) ? lvl.AsInt32() : 1;
		projector.experience = dict.TryGetValue("experience", out Variant exp) ? exp.AsInt32() : 0;
		projector.currentEnergy = dict.TryGetValue("currentEnergy", out Variant eng) ? eng.AsInt32() : 50;
		
		projector.ownedFamiliars = new();
		
		if (dict.TryGetValue("ownedFamiliars", out Variant familiars) && familiars.VariantType == Variant.Type.Array)
		{
			foreach (Variant fam in familiars.AsGodotArray())
			{
				projector.ownedFamiliars.Add(FamiliarSave.FromDictionary(fam.AsGodotDictionary()));
			}
		}
		
		projector.spells = new();
		
		if (dict.TryGetValue("spells", out Variant spellsOut) && spellsOut.VariantType == Variant.Type.Array)
		{
			foreach (Variant id in spellsOut.AsGodotArray())
			{
				projector.spells.Add(id.AsString());
			}
		}
		
		return projector;
	}
}
