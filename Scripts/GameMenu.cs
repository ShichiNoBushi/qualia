using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	public GameSession session;
	
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
	
	public Button quitButton;
	public ConfirmationDialog quitConfirm;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		
		closeButton = GetNode<Button>("Panel/CloseButton");
		
		projNameLabel = GetNode<Label>("Panel/TabContainer/Projector/ProjNameLabel");
		
		projLevelLabel = GetNode<Label>("Panel/TabContainer/Projector/ProjLevelLabel");
		
		projExperienceProgress = GetNode<ProgressBar>("Panel/TabContainer/Projector/ProjExperienceProgress");
		projExperienceLabel = GetNode<Label>("Panel/TabContainer/Projector/ProjExperienceLabel");
		projExperienceNextLabel = GetNode<Label>("Panel/TabContainer/Projector/ProjExperienceNextLabel");
		
		projEnergyProgress = GetNode<ProgressBar>("Panel/TabContainer/Projector/ProjEnergyProgress");
		projCurrentEnergyLabel = GetNode<Label>("Panel/TabContainer/Projector/ProjCurrentEnergyLabel");
		projMaxEnergyLabel = GetNode<Label>("Panel/TabContainer/Projector/ProjMaxEnergyLabel");
		
		projPortraitRect = GetNode<TextureRect>("Panel/TabContainer/Projector/ProjPortraitRect");
		
		familiarsList = GetNode<ItemList>("Panel/TabContainer/Familiars/FamiliarsList");
		
		statsPanel = GetNode<Panel>("Panel/TabContainer/Familiars/StatsPanel");
		
		famNameLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/FamNameLabel");
		familiarLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/FamiliarLabel");
		
		famLevelLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/FamLevelLabel");
		
		famExperienceProgress = GetNode<ProgressBar>("Panel/TabContainer/Familiars/StatsPanel/FamExperienceProgress");
		famExperienceLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/FamExperienceLabel");
		famExperienceNextLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/FamExperienceNextLabel");
		
		famEnergyLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/GridContainer/FamEnergyLabel");
		physAttackLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/GridContainer/PhysAttackLabel");
		magAttackLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/GridContainer/MagAttackLabel");
		physDefenseLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/GridContainer/PhysDefenseLabel");
		magDefenseLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/GridContainer/MagDefenseLabel");
		speedLabel = GetNode<Label>("Panel/TabContainer/Familiars/StatsPanel/GridContainer/SpeedLabel");
		
		skillsList = GetNode<ItemList>("Panel/TabContainer/Familiars/StatsPanel/SkillsList");
		
		skillDescLabel = GetNode<RichTextLabel>("Panel/TabContainer/Familiars/StatsPanel/SkillDescLabel");
		
		famPortraitRect = GetNode<TextureRect>("Panel/TabContainer/Familiars/StatsPanel/FamPortraitRect");
		
		familiarsList.ItemSelected += OnFamiliarSelect;
		
		skillsList.ItemSelected += OnSkillSelect;
		
		quitButton = GetNode<Button>("Panel/TabContainer/Options/CenterContainer/VBoxContainer/QuitButton");
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
	
	public void Open()
	{
		Visible = true;
		UpdateProjectorLabels();
		UpdateFamiliarList();
		GetTree().Paused = true;
	}
	
	public void Close()
	{
		Visible = false;
		GetTree().Paused = false;
	}
}
