using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	public GameSession session;
	public DataRegistry registry;
	
	public RFamiliarInstance selectedFamiliar;
	public RSkillData selectedSkill;
	
	public ItemInstance selectedUnique;
	public string selectedItemId;
	
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
	
	public Label famNameLabel;  //Visibility = false
	public LineEdit famNameText;
	public Label familiarLabel;
	
	public Button renameButton;
	
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
	
	public ItemList itemList;
	public RichTextLabel itemDescLabel;
	public Button useButton;
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
		famNameText = GetNode<LineEdit>("Panel/MainTab/Familiars/StatsPanel/FamNameText");
		familiarLabel = GetNode<Label>("Panel/MainTab/Familiars/StatsPanel/FamiliarLabel");
		
		renameButton = GetNode<Button>("Panel/MainTab/Familiars/StatsPanel/RenameButton");
		
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
		
		famNameText.TextChanged += OnNameChanged;
		famNameText.TextSubmitted += OnNameSubmitted;
		renameButton.Pressed += OnRenamePressed;
		
		familiarsList.ItemSelected += OnFamiliarSelect;
		
		skillsList.ItemSelected += OnSkillSelect;
		
		itemList = GetNode<ItemList>("Panel/MainTab/Inventory/InventoryTab/Items/ItemList");
		itemDescLabel = GetNode<RichTextLabel>("Panel/MainTab/Inventory/InventoryTab/Items/ItemDescLabel");
		useButton = GetNode<Button>("Panel/MainTab/Inventory/InventoryTab/Items/UseButton");
		crystalsList = GetNode<ItemList>("Panel/MainTab/Inventory/InventoryTab/Crystals/CrystalsList");
		crystalDescLabel = GetNode<RichTextLabel>("Panel/MainTab/Inventory/InventoryTab/Crystals/CrystalDescLabel");
		
		itemList.ItemSelected += OnItemSelect;
		useButton.Pressed += OnUsePressed;
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
		
		statsPanel.Visible = true;
		
		famNameText.Text = selectedFamiliar.nickName ?? "";
		famNameText.PlaceholderText = selectedFamiliar.data?.name ?? "(no name)";
		familiarLabel.Text = string.IsNullOrEmpty(selectedFamiliar.data?.name) ? "(no name)" : selectedFamiliar.data.name;
		
		Godot.Collections.Array<RTypeData> types = selectedFamiliar.types != null && selectedFamiliar.types.Count > 0 ? selectedFamiliar.types : selectedFamiliar.data?.types;
		
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
		
		famLevelLabel.Text = $"{selectedFamiliar.level}";
		
		int next = selectedFamiliar.ExpToNextLevel();
		famExperienceProgress.Value = selectedFamiliar.experience;
		famExperienceProgress.MaxValue = next;
		famExperienceLabel.Text = $"{selectedFamiliar.experience}";
		famExperienceNextLabel.Text = $"{next}";
		
		famEnergyLabel.Text = $"{selectedFamiliar.energy}";
		physAttackLabel.Text = $"{selectedFamiliar.pAttack}";
		magAttackLabel.Text = $"{selectedFamiliar.mAttack}";
		physDefenseLabel.Text = $"{selectedFamiliar.pDefense}";
		magDefenseLabel.Text = $"{selectedFamiliar.mDefense}";
		speedLabel.Text = $"{selectedFamiliar.speed}";
		
		famPortraitRect.Texture = selectedFamiliar.data?.portrait;
		
		if (selectedFamiliar.skills != null)
		{
			GD.Print($"GameMenu: {selectedFamiliar.GetPreferredName()} has {selectedFamiliar.skills.Count} Skills");
			foreach (var skill in selectedFamiliar.skills)
			{
				GD.Print($"  {skill.name}");
			}
		}
		else
		{
			GD.Print($"GameMenu: {selectedFamiliar.GetPreferredName()}'s Skills is null");
		}
		UpdateSkillList();
	}
	
	public void OnNameChanged(string newText)
	{
		if (selectedFamiliar == null)
		{
			renameButton.Disabled = true;
			return;
		}
		
		renameButton.Disabled = newText.Trim() == selectedFamiliar.GetPreferredName();
	}
	
	public void OnNameSubmitted(string newText)
	{
		ApplyNickname(newText);
	}
	
	public void OnRenamePressed()
	{
		ApplyNickname(famNameText.Text);
	}
	
	public void OnSkillSelect(long index)
	{
		selectedSkill = skillsList.GetItemMetadata((int)index).As<RSkillData>();
		
		skillDescLabel.Text = selectedSkill != null ? selectedSkill.FormatDescription() : "";
	}
	
	public void OnItemSelect(long index)
	{
		Variant meta = itemList.GetItemMetadata((int)index);
		
		itemDescLabel.Clear();
		
		if (meta.VariantType == Variant.Type.String)
		{
			string id = meta.AsString();
			RItemData item = registry.Item(id);
			
			int stack = session.itemStacks.TryGetValue(id, out int c) ? c : 0;
			
			itemDescLabel.AppendText(item.FormatDescription());
			itemDescLabel.Newline();
			itemDescLabel.AppendText($"Held: {stack}");
			
			selectedUnique = null;
			selectedItemId = id;
			
			RefreshUseButton();
			
			return;
		}
		else if (meta.AsGodotObject() is ItemInstance inst && inst != null)
		{
			string id = inst.data.id;
			
			itemDescLabel.AppendText(inst.data.FormatDescription());
			itemDescLabel.Newline();
			itemDescLabel.AppendText($"Uses: ({inst.usesLeft} / {inst.maxUses})");
			
			selectedUnique = inst;
			selectedItemId = id;
			
			RefreshUseButton();
			
			return;
		}
		
		selectedUnique = null;
		selectedItemId = null;
	}
	
	public void OnUsePressed()
	{
		RItemData item = selectedUnique?.data ?? registry.Item(selectedItemId);
		
		if (item == null || !item.fieldUsable)
		{
			return;
		}
		
		bool canUse = selectedUnique != null ? selectedUnique.usesLeft > 0 : session.itemStacks.TryGetValue(item.id, out int n) && n > 0;
		
		if (!canUse)
		{
			return;
		}
		
		if (item.healPower > 0)
		{
			session.playerProjector.Restore(item.healPower);
		}
		
		if (!session.TryUseItem(item, selectedUnique))
		{
			return;
		}
		
		UpdateInventory();
		RefreshUseButton();
		UpdateProjectorLabels();
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
		itemList.Clear();
		
		if (session.itemStacks != null)
		{
			foreach (var it in session.itemStacks)
			{
				RItemData item = registry.Item(it.Key);
				
				if (item == null || it.Value <= 0)
				{
					continue;
				}
				
				string name = string.IsNullOrEmpty(item.name) ? item.id : item.name;
				
				itemList.AddItem($"{name} x{it.Value}");
				int idx = itemList.ItemCount - 1;
				itemList.SetItemMetadata(idx, it.Key);
			}
		}
		else
		{
			GD.PrintErr("GameMenu: item stacks is null");
		}
		
		if (session.uniqueItems != null)
		{
			foreach (var unIt in session.uniqueItems)
			{
				RItemData item = unIt.data;
				
				if (item == null)
				{
					continue;
				}
				
				string name = string.IsNullOrEmpty(item.name) ? item.id : item.name;
				
				string label = name;
				
				if ((unIt.data.fieldUsable || unIt.data.battleUsable) && unIt.data.uses > 0)
				{
					label = $"{name} ({unIt.usesLeft} / {unIt.maxUses})";
				}
				
				itemList.AddItem(label);
				int idx = itemList.ItemCount - 1;
				itemList.SetItemMetadata(idx, unIt);
			}
		}
		else
		{
			GD.PrintErr("GameMenu: unique items is null");
		}
		
		crystalsList.Clear();
		
		if (session.qualiaCrystals != null)
		{
			foreach (var qc in session.qualiaCrystals)
			{
				RQualiaCrystal crystal = registry.Crystal(qc.Key);
				
				if (crystal == null || qc.Value <= 0)
				{
					continue;
				}
				
				string name = string.IsNullOrEmpty(crystal.name) ? crystal.id : crystal.name;
				
				crystalsList.AddItem($"{name} x{qc.Value}");
				int idx = crystalsList.ItemCount - 1;
				crystalsList.SetItemMetadata(idx, qc.Key);
			}
		}
		else
		{
			GD.PrintErr("GameMenu: qualia crystals is null");
		}
		
		gCrystalsLabel.Text = $"{session.qualiaGeneric}";
	}
	
	public void RefreshUseButton()
	{
		if (string.IsNullOrEmpty(selectedItemId))
		{
			useButton.Disabled = true;
			return;
		}
		
		if (selectedUnique == null)
		{
			RItemData item = registry.Item(selectedItemId);
			
			useButton.Disabled = !(item != null && item.fieldUsable && session.itemStacks.TryGetValue(item.id, out int n) && n > 0);
		}
		else
		{
			useButton.Disabled = !(selectedUnique.data != null && selectedUnique.data.fieldUsable && selectedUnique.usesLeft > 0);
		}
	}
	
	public void ApplyNickname(string raw)
	{
		if (selectedFamiliar == null)
		{
			return;
		}
		
		selectedFamiliar.nickName = raw.Trim();
		renameButton.Disabled = true;
		famNameText.ReleaseFocus();
		UpdateFamiliarList();
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
