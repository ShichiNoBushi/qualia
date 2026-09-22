using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	public GameSession session;
	public DataRegistry registry;
	
	public RFamiliarInstance selectedFamiliar;
	public RSkillData selectedSkill;
	
	public Button closeButton;
	
	public Label projNameLabel;
	
	public Label projLevelLabel;
	
	public ProgressBar projExperienceProgress;
	public Label projExperienceLabel;
	public Label projExperienceNextLabel;
	
	public ProgressBar projEnergyProgress;
	public Label projCurrentEnergyLabel;
	public Label projMaxEnergyLabel;
	
	public TextureRect projPortraitRect;
	
	public ItemList familiarsList;
	
	public Panel statsPanel;
	
	public Label famNameLabel;
	public Label familiarLabel;
	
	public RichTextLabel typesLabel;
	
	public Label famLevelLabel;
	
	public ProgressBar famExperienceProgress;
	public Label famExperienceLabel;
	public Label famExperienceNextLabel;
	
	public Label famEnergyLabel;
	public Label physAttackLabel;
	public Label magAttackLabel;
	public Label physDefenseLabel;
	public Label magDefenseLabel;
	public Label speedLabel;
	
	public TextureRect famPortraitRect;
	
	public ItemList skillsList;
	
	public RichTextLabel skillDescLabel;
	
	public ItemList crystalsList;
	public RichTextLabel crystalDescLabel;
	
	public Label gCrystalsLabel;
	
	public Button quitButton;
	public ConfirmationDialog quitConfirm;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		closeButton = GetNode<Button>("Panel/CloseButton");
		
		projNameLabel = GetNode<Label>("Panel/MainTab/Projector/ProjNameLabel");
		
		projLevelLabel = GetNode<Label>("Panel/MainTab/Projector/ProjLevelLabel");
		
		projExperienceProgress = GetNode<ProgressBar>("Panel/MainTab/Projector/ProjExperienceProgress");
		projExperienceLabel = GetNode<Label>("Panel/MainTab/Projector/ProjExperienceLabel");
		projExperienceNextLabel = GetNode<Label>("Panel/MainTab/Projector/ProjExperienceNextLabel");
		
		projEnergyProgress = GetNode<ProgressBar>("Panel/MainTab/Projector/ProjEnergyProgress");
		projCurrentEnergyLabel = GetNode<Label>("Panel/MainTab/Projector/ProjCurrentEnergyLabel");
		projMaxEnergyLabel = GetNode<Label>("Panel/MainTab/Projector/ProjMaxEnergyLabel");
		
		projPortraitRect = GetNode<TextureRect>("Panel/MainTab/Projector/ProjPortraitRect");
		
		familiarsList = GetNode<ItemList>("Panel/MainTab/Familiars/FamiliarsList");
		
		statsPanel = GetNode<Panel>("Panel/MainTab/Familiars/StatsPanel");
		
		famNameLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/FamNameLabel");
		familiarLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/FamiliarLabel");
		
		typesLabel = GetNode<RichTextLabel>("Panel/MainTab/Familiars/StatsPanel/TypesLabel");
		
		famLevelLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/FamLevelLabel");
		
		famExperienceProgress = GetNode<ProgressBar>("Panel/MainTab/Familiars/StatsPanel/FamExperienceProgress");
		famExperienceLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/FamExperienceLabel");
		famExperienceNextLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/FamExperienceNextLabel");
		
		famEnergyLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/GridContainer/FamEnergyLabel");
		physAttackLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/GridContainer/PhysAttackLabel");
		magAttackLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/GridContainer/MagAttackLabel");
		physDefenseLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/GridContainer/PhysDefenseLabel");
		magDefenseLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/GridContainer/MagDefenseLabel");
		speedLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/GridContainer/SpeedLabel");
		
		skillsList = GetNode<ItemList>("Panel/MainTab/Familiars/StatsPanel/SkillsList");
		
		skillDescLabel = GetNode<RichTextLabel>("Panel/MainTab/Familiars/StatsPanel/SkillDescLabel");
		
		famPortraitRect = GetNode<TextureRect>("Panel/MainTab/Familiars/StatsPanel/FamPortraitRect");
		
		familiarsList.ItemSelected += OnFamiliarSelect;
		
		skillsList.ItemSelected += OnSkillSelect;
		
		crystalsList = GetNode<ItemList>("Panel/MainTab/Inventory/InventoryTab/Crystals/CrystalsList");
		crystalDescLabel = GetNode<RichTextLabel>("Panel/MainTab/Inventory/InventoryTab/Crystals/CrystalDescLabel");
		
		crystalsList.ItemSelected += OnCrystalSelect;
		
		gCrystalsLabel = GetNode<Label>("Panel/MainTab/Inventory/GCrystalsLabel");
		
		quitButton = GetNode<Button>("Panel/MainTab/Options/CenterContainer/VBoxContainer/QuitButton");
		quitConfirm = GetNode<ConfirmationDialog>("QuitConfirm");
		
		closeButton.Pressed += OnClosePressed;
		
		quitButton.Pressed += OnQuitPressed;
		quitConfirm.Confirmed += OnQuitConfirmed;
	}
	
	public override void _UnhandledInput(InputEvent e)
	{
		if (session.gameMode != GameSession.GameMode.World)
		{
			return;
		}
		
		if (!e.IsActionPressed("ui_cancel"))
		{
			return;
		}
		
		GD.Print($"GameMenu: mode={session.gameMode} visible={Visible}");
		
		GD.Print("GameMenu: Esc pressed");
		
		if (Visible)
		{
			Close();
		}
		else
		{
			Open();
		}
		
		GetViewport().SetInputAsHandled();
	}
	
	public void OnClosePressed()
	{
		Close();
	}
	
	public void OnFamiliarSelect(long index)
	{
		selectedFamiliar = familiarsList.GetItemMetadata((int)index).As<RFamiliarInstance>();
		
		if (selectedFamiliar == null)
		{
			statsPanel.Visible = false;
			return;
		}
		
		RFamiliarInstance fam = selectedFamiliar;
		
		statsPanel.Visible = true;
		
		famNameLabel.Text = fam.GetPreferredName();
		familiarLabel.Text = string.IsNullOrEmpty(fam.data?.name) ? "(no name)" : fam.data.name;
		
		Godot.Collections.Array<RTypeData> types = fam.types != null && fam.types.Count > 0 ? fam.types : fam.data?.types;
		
		if (types != null && types.Count > 0)
		{
			System.Collections.Generic.List<string> typeTexts = new();
			
			foreach (var t in types)
			{
				if (t == null)
				{
					continue;
				}
				
				string name = string.IsNullOrEmpty(t.name) ? "(type)" : t.name;
				string fill = t.color.ToHtml(false);
				string edge = t.outline.A > 0 ? t.outline.ToHtml(false) : t.ContrastColor().ToHtml(false);
				
				typeTexts.Add($"[outline_size=4][outline_color=#{edge}][color=#{fill}]{name}[/color][/outline_color][/outline_size]");
			}
			
			string typeFull = string.Join("\n", typeTexts);
			typesLabel.Text = typeFull;
		}
		else
		{
			typesLabel.Text = "(untyped)";
		}
		
		famLevelLabel.Text = $"{fam.level}";
		
		int next = fam.ExpToNextLevel();
		famExperienceProgress.Value = fam.experience;
		famExperienceProgress.MaxValue = next;
		famExperienceLabel.Text = $"{fam.experience}";
		famExperienceNextLabel.Text = $"{next}";
		
		famEnergyLabel.Text = $"{fam.energy}";
		physAttackLabel.Text = $"{fam.pAttack}";
		magAttackLabel.Text = $"{fam.mAttack}";
		physDefenseLabel.Text = $"{fam.pDefense}";
		magDefenseLabel.Text = $"{fam.mDefense}";
		speedLabel.Text = $"{fam.speed}";
		
		famPortraitRect.Texture = fam.data?.portrait;
		
		if (fam.skills != null)
		{
			GD.Print($"GameMenu: {fam.GetPreferredName()} has {fam.skills.Count} Skills");
			foreach (var skill in fam.skills)
			{
				GD.Print($"  {skill.name}");
			}
		}
		else
		{
			GD.Print($"GameMenu: {fam.GetPreferredName()}'s Skills is null");
		}
		UpdateSkillList();
	}
	
	public void OnSkillSelect(long index)
	{
		selectedSkill = skillsList.GetItemMetadata((int)index).As<RSkillData>();
		
		skillDescLabel.Text = selectedSkill != null ? selectedSkill.FormatDescription() : "";
	}
	
	public void OnCrystalSelect(long index)
	{
		string crystalKey = crystalsList.GetItemMetadata((int)index).AsString();
		
		if (string.IsNullOrEmpty(crystalKey))
		{
			crystalDescLabel.Text = "";
			return;
		}
		
		RQualiaCrystal crystal = registry.Crystal(crystalKey);
		
		int amount = session.qualiaCrystals.TryGetValue(crystalKey, out int n) ? n : 0;
		
		if (crystal == null)
		{
			crystalDescLabel.Text = $"{crystalKey} x{amount}";
			return;
		}
		
		crystalDescLabel.Clear();
		crystalDescLabel.AppendText($"{crystal.name}\n\nHeld: {amount}\nValue: {crystal.value}");
	}
	
	public void OnQuitPressed()
	{
		quitConfirm.PopupCentered();
	}
	
	public void OnQuitConfirmed()
	{
		GetTree().Quit();
	}
	
	public void UpdateProjectorLabels()
	{
		Projector proj = session.playerProjector;
		
		if (proj == null)
		{
			return;
		}
		
		projNameLabel.Text = proj.name ?? "";
		
		projLevelLabel.Text = $"{proj.level}";
		
		int next = proj.ExpToNextLevel();
		projExperienceProgress.Value = proj.experience;
		projExperienceProgress.MaxValue = next;
		projExperienceLabel.Text = $"{proj.experience}";
		projExperienceNextLabel.Text = $"{next}";
		
		projEnergyProgress.Value = proj.currentEnergy;
		projEnergyProgress.MaxValue = Math.Max(proj.maxEnergy, 1);
		projCurrentEnergyLabel.Text = $"{proj.currentEnergy}";
		projMaxEnergyLabel.Text = $"{proj.maxEnergy}";
		
		projPortraitRect.Texture = proj.data?.portrait;
	}
	
	public void UpdateFamiliarList()
	{
		Godot.Collections.Array<RFamiliarInstance> familiars = session.playerProjector.ownedFamiliars;
		
		int restoreIdx = -1;
		
		familiarsList.Clear();
		
		for (int i = 0; i < familiars.Count; i++)
		{
			RFamiliarInstance fam = familiars[i];
			
			if (fam == null)
			{
				continue;
			}
			
			familiarsList.AddItem(fam.GetPreferredName());
			int idx = familiarsList.ItemCount - 1;
			familiarsList.SetItemMetadata(idx, fam);
			
			if (selectedFamiliar != null && ReferenceEquals(fam, selectedFamiliar))
			{
				restoreIdx = idx;
			}
		}
		
		if (restoreIdx >= 0)
		{
			familiarsList.Select(restoreIdx);
			OnFamiliarSelect(restoreIdx);
		}
		else
		{
			selectedFamiliar = null;
			familiarsList.DeselectAll();
			statsPanel.Visible = false;
			
			selectedSkill = null;
			skillsList.DeselectAll();
			skillDescLabel.Text = "";
		}
	}
	
	public void UpdateSkillList()
	{
		skillsList.Clear();
		
		if (selectedFamiliar == null)
		{
			selectedSkill = null;
			skillDescLabel.Text = "";
			return;
		}
		
		Godot.Collections.Array<RSkillData> skills = selectedFamiliar.skills;
		
		if (skills == null || skills.Count == 0)
		{
			selectedSkill = null;
			skillDescLabel.Text = "";
			return;
		}
		
		int restoreIdx = -1;
		
		for (int i = 0; i < skills.Count; i++)
		{
			RSkillData skill = skills[i];
			
			if (skill == null)
			{
				continue;
			}
			
			skillsList.AddItem(skill.name);
			int idx = skillsList.ItemCount - 1;
			skillsList.SetItemMetadata(idx, skill);
			
			if (skill == selectedSkill)
			{
				restoreIdx = idx;
			}
		}
		
		if (restoreIdx >= 0)
		{
			skillsList.Select(restoreIdx);
			OnSkillSelect(restoreIdx);
		}
		else
		{
			selectedSkill = null;
			skillsList.DeselectAll();
			skillDescLabel.Text = "";
		}
	}
	
	public void UpdateInventory()
	{
		crystalsList.Clear();
		
		foreach (var qc in session.qualiaCrystals)
		{
			RQualiaCrystal crystal = registry.Crystal(qc.Key);
			
			if (crystal == null || qc.Value <= 0)
			{
				continue;
			}
			
			string name = string.IsNullOrEmpty(crystal.name) ? crystal.id : crystal.name;
			
			crystalsList.AddItem(name);
			int idx = crystalsList.ItemCount - 1;
			crystalsList.SetItemMetadata(idx, qc.Key);
		}
		
		gCrystalsLabel.Text = $"{session.qualiaGeneric}";
	}
	
	public void Open()
	{
		Visible = true;
		UpdateProjectorLabels();
		UpdateFamiliarList();
		UpdateInventory();
		GetTree().Paused = true;
	}
	
	public void Close()
	{
		Visible = false;
		GetTree().Paused = false;
	}
}
