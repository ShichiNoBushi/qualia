using Godot;
using System;

public partial class ProjectorCommands : Control
{
	public Label projectorLabel;
	public Button summonButton;
	public Button dismissButton;
	public Button spellButton;
	public Button focusButton;
	public Button itemButton;
	public Button escapeButton;
	public Button undoButton;
	
	public BattleManager battle;
	public BattleCommand activeCommand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		projectorLabel = GetNode<Label>("CommandsVBox/ProjectorLabel");
		summonButton = GetNode<Button>("CommandsVBox/SummonButton");
		dismissButton = GetNode<Button>("CommandsVBox/DismissButton");
		spellButton = GetNode<Button>("CommandsVBox/SpellButton");
		focusButton = GetNode<Button>("CommandsVBox/FocusButton");
		itemButton = GetNode<Button>("CommandsVBox/ItemButton");
		escapeButton = GetNode<Button>("CommandsVBox/EscapeButton");
		undoButton = GetNode<Button>("UndoButton");
		
		summonButton.Pressed += OnSummonPressed;
		dismissButton.Pressed += OnDismissPressed;
		spellButton.Pressed += OnSpellPressed;
		focusButton.Pressed += OnFocusPressed;
		itemButton.Pressed += OnItemPressed;
		escapeButton.Pressed += OnEscapePressed;
		undoButton.Pressed += OnUndoPressed;
		
		//Disable buttons currently without function
		summonButton.Disabled = true;
		dismissButton.Disabled = true;
		spellButton.Disabled = true;
		itemButton.Disabled = true;
		escapeButton.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnSummonPressed()
	{
		
		
		DisableCommands();
		undoButton.Visible = true;
	}
	
	public void OnDismissPressed()
	{
		
		
		DisableCommands();
		undoButton.Visible = true;
	}
	
	public void OnSpellPressed()
	{
		
		
		DisableCommands();
		undoButton.Visible = true;
	}
	
	public void OnFocusPressed()
	{
		battle.AppendBattleText("Focus button pressed.");
		GD.Print("Focus button pressed.");
		
		if (battle?.playerSide?.projector == null)
		{
			GD.Print("No projector.");
			return;
		}
		
		FocusCommand cmd = new()
		{
			sourceSide = battle.playerSide,
			source = battle.playerSide.projector
		};
		activeCommand = cmd;
		
		battle.projectorCommands.Add(cmd);
		battle.AppendBattleText("Player: Focus Command added.");
		
		DisableCommands();
		undoButton.Visible = true;
	}
	
	public void OnItemPressed()
	{
		
		
		DisableCommands();
		undoButton.Visible = true;
	}
	
	public void OnEscapePressed()
	{
		
		
		DisableCommands();
		undoButton.Visible = true;
	}
	
	public void OnUndoPressed()
	{
		if (activeCommand != null)
		{
			battle.projectorCommands.Remove(activeCommand);
			battle.AppendBattleText("Player: command undone.");
		}
		activeCommand = null;
		
		EnableCommands();
		undoButton.Visible = false;
	}
	
	public void DisableCommands()
	{
		summonButton.Disabled = true;
		dismissButton.Disabled = true;
		spellButton.Disabled = true;
		focusButton.Disabled = true;
		itemButton.Disabled = true;
		escapeButton.Disabled = true;
	}
	
	public void EnableCommands()
	{
		//summonButton.Disabled = false;
		//dismissButton.Disabled = false;
		//spellButton.Disabled = false;
		focusButton.Disabled = false;
		//itemButton.Disabled = false;
		//escapeButton.Disabled = false;
	}
}
