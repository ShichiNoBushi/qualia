using Godot;
using System;
using System.Collections.Generic;

public partial class FamiliarCommands : Control
{
	public Label familiarLabel;
	public VBoxContainer subCommandsVBox;
	public Button attackButton;
	public Button defendButton;
	public List<Button> skillButtons = new();
	public Button undoButton;
	
	public FamiliarActor familiar {get; set;}
	public BattleManager battle {get; set;}
	public int slot {get; set;}
	public BattleCommand activeCommand {get; set;}
	
	public bool disableAttack {get; set;} = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		familiarLabel = GetNode<Label>("CommandsVBox/FamiliarLabel");
		
		subCommandsVBox = GetNode<VBoxContainer>("CommandsVBox/ScrollContainer/SubCommandsVBox");
		
		attackButton = GetNode<Button>("CommandsVBox/ScrollContainer/SubCommandsVBox/AttackButton");
		defendButton = GetNode<Button>("CommandsVBox/ScrollContainer/SubCommandsVBox/DefendButton");
		undoButton = GetNode<Button>("UndoButton");
		
		attackButton.Pressed += OnAttackPressed;
		defendButton.Pressed += OnDefendPressed;
		undoButton.Pressed += OnUndoPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Bind(FamiliarActor actor)
	{
		familiar = actor;
		slot = battle.playerSide.GetSlotIndex(actor);
		
		ClearSkillButtons();
		
		if (actor == null || actor.familiar?.skills == null)
		{
			familiarLabel.Text = "";
			SetElementsVisible(false);
			return;
		}
		
		familiarLabel.Text = string.IsNullOrEmpty(actor.name) ? "(no name)" : actor.name;
		SetElementsVisible(true);
		
		foreach (var skill in actor.familiar.skills)
		{
			Button btn = new();
			btn.Text = skill.name;
			btn.Pressed += () => OnSkillPressed(skill);
			skillButtons.Add(btn);
			subCommandsVBox.AddChild(btn);
		}
	}
	
	public void ClearSkillButtons()
	{
		foreach (var btn in skillButtons)
		{
			btn.QueueFree();
		}
		
		skillButtons.Clear();
	}
	
	public void OnAttackPressed()
	{
		if (battle.enemySide.CountActiveFamiliars() == 0 && !battle.isProjectorEncounter)
		{
			return;
		}
		
		bool canAttackFamiliars = battle.enemySide.CountActiveFamiliars() > 0;
		bool canAttackProjector = battle.isProjectorEncounter && !canAttackFamiliars && battle.enemySide.projector != null && battle.enemySide.projector.currentEnergy > 0;
		
		if (!canAttackFamiliars && !canAttackProjector)
		{
			return;
		}
		
		if (familiar == null)
		{
			battle.AppendBattleText("FamiliarCommand: familiar null");
			GD.Print("FamiliarCommand: familiar null");
			return;
		}
		
		battle.SetCommandState(BattleManager.CommandState.SelectEnemyAttack);
		battle.pendingCommand = null;
		battle.pendingSource = familiar;
		
		if (canAttackFamiliars)
		{
			battle.HighlightEnemies();
		}
		else if (canAttackProjector)
		{
			battle.projectorDisplayE.Highlight(true);
		}
		
		battle.BlockCommands();
		battle.RefreshNextButton();
	}
	
	public void OnDefendPressed()
	{
		if (familiar == null)
		{
			return;
		}
		
		DefendCommand cmd = new()
		{
			sourceSide = battle.playerSide,
			source = familiar
		};
		
		battle.familiarCommands.Add(cmd);
		battle.famCommandsSubmitted++;
		battle.AppendBattleText("Player: Defend Command added.");
		
		SetActiveCommand(cmd);
		
		battle.RefreshNextButton();
	}
	
	public void OnSkillPressed(RSkillData skill)
	{
		
		
		DisableCommands();
	}
	
	public void OnUndoPressed()
	{
		if (activeCommand != null)
		{
			battle.familiarCommands.Remove(activeCommand);
			battle.famCommandsSubmitted = Math.Min(battle.famCommandsSubmitted - 1, 0);
		}
		activeCommand = null;
		
		battle.famCommandDisabled[slot] = false;
		EnableCommands();
		undoButton.Visible = false;
		
		battle.RefreshNextButton();
	}
	
	public void CheckValidCommands()
	{
		bool canAttackFamiliars = battle.enemySide.CountActiveFamiliars() > 0;
		bool canAttackProjector = battle.isProjectorEncounter && !canAttackFamiliars && battle.enemySide.projector != null && battle.enemySide.projector.currentEnergy > 0;
		
		disableAttack = !canAttackFamiliars && !canAttackProjector;
	}
	
	public void DisableCommands()
	{
		attackButton.Disabled = true;
		defendButton.Disabled = true;
		
		foreach (var btn in skillButtons)
		{
			btn.Disabled = true;
		}
	}
	
	public void EnableCommands()
	{
		attackButton.Disabled = disableAttack;
		defendButton.Disabled = false;
		
		foreach (var btn in skillButtons)
		{
			//btn.Disabled = false;
		}
	}
	
	public void SetActiveCommand(BattleCommand command)
	{
		DisableCommands();
		battle.famCommandDisabled[slot] = true;
		activeCommand = command;
		undoButton.Visible = true;
	}
	
	public void SetElementsVisible(bool visible)
	{
		familiarLabel.Visible = visible;
		subCommandsVBox.Visible = visible;
	}
}
