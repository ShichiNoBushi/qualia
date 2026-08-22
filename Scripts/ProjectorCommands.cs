using Godot;
using System;

public partial class ProjectorCommands : Control
{
	public Button summonButton;
	public Button dismissButton;
	public Button spellButton;
	public Button focusButton;
	public Button itemButton;
	public Button escapeButton;
	
	public BattleManager battle;
	public BattleCommand activeCommand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		summonButton = GetNode<Button>("CommandsVBox/SummonButton");
		dismissButton = GetNode<Button>("CommandsVBox/DismissButton");
		spellButton = GetNode<Button>("CommandsVBox/SpellButton");
		focusButton = GetNode<Button>("CommandsVBox/FocusButton");
		itemButton = GetNode<Button>("CommandsVBox/ItemButton");
		escapeButton = GetNode<Button>("CommandsVBox/EscapeButton");
		
		summonButton.Pressed += OnSummonPressed;
		dismissButton.Pressed += OnDismissPressed;
		spellButton.Pressed += OnSpellPressed;
		focusButton.Pressed += OnFocusPressed;
		itemButton.Pressed += OnItemPressed;
		escapeButton.Pressed += OnEscapePressed;
		
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
	}
	
	public void OnDismissPressed()
	{
		
		
		DisableCommands();
	}
	
	public void OnSpellPressed()
	{
		
		
		DisableCommands();
	}
	
	public void OnFocusPressed()
	{
		if (battle?.playerSide?.projector == null)
		{
			return;
		}
		
		FocusCommand cmd = new();
		activeCommand = cmd;
		cmd.sourceSide = battle.playerSide;
		cmd.source = battle.playerSide.projector;
		
		battle.projectorCommands.Add(cmd);
		
		DisableCommands();
	}
	
	public void OnItemPressed()
	{
		
		
		DisableCommands();
	}
	
	public void OnEscapePressed()
	{
		
		
		DisableCommands();
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
