using Godot;
using System;
using System.Collections.Generic;

public partial class FamiliarCommands : Control
{
	public VBoxContainer subCommandsVBox;
	public Button attackButton;
	public Button defendButton;
	public List<Button> skillButtons = new();
	public Button undoButton;
	
	public FamiliarActor familiar;
	public BattleManager battle;
	public BattleCommand activeCommand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		subCommandsVBox = GetNode<VBoxContainer>("CommandsVBox/ScrollContainer/SubCommandsVBox");
		
		attackButton = GetNode<Button>("CommandsVBox/ScrollContainer/SubCommandsVBox/AttackButton");
		defendButton = GetNode<Button>("CommandsVBox/ScrollContainer/SubCommandsVBox/DefendButton");
		undoButton = GetNode<Button>("UndoButton");
		
		attackButton.Pressed += OnAttackPressed;
		defendButton.Pressed += OnDefendPressed;
		undoButton.Pressed += OnUndoPressed;
		
		//Disable buttons currently without function.
		defendButton.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Bind(FamiliarActor actor)
	{
		familiar = actor;
		
		ClearSkillButtons();
		
		if (actor == null || actor.familiar?.skills == null)
		{
			Visible = false;
			return;
		}
		
		Visible = true;
		
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
		
		
		DisableCommands();
	}
	
	public void OnDefendPressed()
	{
		
		
		DisableCommands();
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
		}
		activeCommand = null;
		
		EnableCommands();
		undoButton.Visible = false;
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
		attackButton.Disabled = false;
		//defendButton.Disabled = false;
		
		foreach (var btn in skillButtons)
		{
			//btn.Disabled = false;
		}
	}
}
