using Godot;
using System;

public partial class DataRegistry : Node
{
	public Godot.Collections.Dictionary<string, RQualiaCrystal> crystals = new();
	public Godot.Collections.Dictionary<string, RProjectorData> projectors = new();
	public Godot.Collections.Dictionary<string, RFamiliarData> familiars = new();
	public Godot.Collections.Dictionary<string, RSpellData> spells = new();
	public Godot.Collections.Dictionary<string, RSkillData> skills = new();
	public Godot.Collections.Dictionary<string, RTypeData> types = new();
	public Godot.Collections.Dictionary<string, RItemData> items = new();
	public Godot.Collections.Dictionary<string, RForgeRecipe> recipes = new();
	
	public override void _Ready()
	{
		LoadAll("res://Resources/QualiaCrystals/", crystals);
		LoadAll("res://Resources/Projectors", projectors);
		LoadAll("res://Resources/FamiliarData/", familiars);
		LoadAll("res://Resources/Spells/", spells);
		LoadAll("res://Resources/Skills/", skills);
		LoadAll("res://Resources/TypeData/", types);
		LoadAll("res://Resources/Items/", items);
		LoadAll("res://Resources/Recipes/", recipes);
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
			GD.Print($"DataRegistry raw: \"{name}\"");
			
			if (folder.CurrentIsDir())
			{
				continue;
			}
			
			if (name.EndsWith(".remap"))
			{
				name = name.Substring(0, name.Length - 6);
			}
			
			if (!name.EndsWith(".tres"))
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
		
		GD.Print($"DataRegistry: {dir} count={dest.Count}");
	}
	
	public string GetId(Resource res) => res switch
	{
		RQualiaCrystal crys => crys.id,
		RProjectorData proj => proj.id,
		RFamiliarData fam => fam.id,
		RSpellData spl => spl.id,
		RSkillData skl => skl.id,
		RTypeData type => type.id,
		RItemData it => it.id,
		RForgeRecipe rec => rec.id,
		_ => ""
	};
	
	public RQualiaCrystal Crystal(string id) => crystals.TryGetValue(id, out RQualiaCrystal crys) ? crys : null;
	public RProjectorData Projector(string id) => projectors.TryGetValue(id, out RProjectorData proj) ? proj : null;
	public RFamiliarData Familiar(string id) => familiars.TryGetValue(id, out RFamiliarData fam) ? fam : null;
	public RSpellData Spell(string id) => spells.TryGetValue(id, out RSpellData spl) ? spl : null;
	public RSkillData Skill(string id) => skills.TryGetValue(id, out RSkillData skl) ? skl : null;
	public RTypeData Type(string id) => types.TryGetValue(id, out RTypeData type) ? type : null;
	public RItemData Item(string id) => items.TryGetValue(id, out RItemData it) ? it : null;
	public RForgeRecipe Recipe(string id) => recipes.TryGetValue(id, out RForgeRecipe rec) ? rec : null;
}
