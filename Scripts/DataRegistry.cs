using Godot;
using System;

public partial class DataRegistry : Node
{
	public Godot.Collections.Dictionary<string, RQualiaCrystal> crystals = new();
	public Godot.Collections.Dictionary<string, RFamiliarData> familiars = new();
	public Godot.Collections.Dictionary<string, RSpellData> spells = new();
	public Godot.Collections.Dictionary<string, RSkillData> skills = new();
	public Godot.Collections.Dictionary<string, RTypeData> types = new();
	public Godot.Collections.Dictionary<string, RItemData> items = new();
	
	public override void _Ready()
	{
		LoadAll("res://Resources/QualiaCrystals/", crystals);
		LoadAll("res://Resources/FamiliarData/", familiars);
		LoadAll("res://Resources/Spells/", spells);
		LoadAll("res://Resources/Skills/", skills);
		LoadAll("res://Resources/TypeData/", types);
		LoadAll("res://Resources/Items/", items);
	}
	
	public void LoadAll<[MustBeVariant] T>(string dir, Godot.Collections.Dictionary<string, T> dest) where T : Resource
	{
		using var folder = DirAccess.Open(dir);
		
		if (folder == null)
		{
			return;
		}
		
		folder.ListDirBegin();
		
		for (string name = folder.GetNext(); name != ""; name = folder.GetNext())
		{
			if (folder.CurrentIsDir() || !name.EndsWith(".tres"))
			{
				continue;
			}
			
			string path = dir.TrimEnd('/') + "/" + name;
			
			var res = GD.Load<T>(path);
			
			if (res == null)
			{
				continue;
			}
			
			string id = GetId(res);
			
			if (string.IsNullOrEmpty(id))
			{
				GD.PrintErr($"DataRegistry: no id on {dir}{name}");
				continue;
			}
			
			dest[id] = res;
			GD.Print($"DataRegistry: loaded from {path} as {res.GetType().Name}");
		}
	}
	
	public string GetId(Resource res) => res switch
	{
		RQualiaCrystal crys => crys.id,
		RFamiliarData fam => fam.id,
		RSpellData spl => spl.id,
		RSkillData skl => skl.id,
		RTypeData type => type.id,
		RItemData it => it.id,
		_ => ""
	};
	
	public RQualiaCrystal Crystal(string id) => crystals.TryGetValue(id, out RQualiaCrystal crys) ? crys : null;
	public RFamiliarData Familiar(string id) => familiars.TryGetValue(id, out RFamiliarData fam) ? fam : null;
	public RSpellData Spell(string id) => spells.TryGetValue(id, out RSpellData spl) ? spl : null;
	public RSkillData Skill(string id) => skills.TryGetValue(id, out RSkillData skl) ? skl : null;
	public RTypeData Type(string id) => types.TryGetValue(id, out RTypeData type) ? type : null;
	public RItemData Item(string id) => items.TryGetValue(id, out RItemData it) ? it : null;
}
