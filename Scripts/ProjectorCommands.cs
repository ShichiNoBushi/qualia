using Godot;
using System;
using System.Collections.Generic;

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
	
	public BattleManager battle {get; set;}
	public BattleCommand activeCommand {get; set;}
	
	public bool disableSummon {get; set;} = false;
	public bool disableDismiss {get; set;} = false;
	
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
		battle.SetCommandState(BattleManager.CommandState.SelectSummon);
		
		Projector projector = battle.playerSide.projector;
		
		if (projector == null)
		{
			battle.AppendBattleText("ProjectorCommand: null projector");
			GD.Print("ProjectorCommand: null projector");
			return;
		}
		
		Godot.Collections.Array<RFamiliarInstance> alreadyOut = new();
		
		foreach (var actor in battle.playerSide.GetFamiliarList())
		{
			if (actor.familiar != null)
			{
				alreadyOut.Add(actor.familiar);
			}
		}
		
		GD.Print($"ProjectorCommand: {projector.ownedFamiliars.Count} Owned Familiars, {alreadyOut.Count} Familiars Out");
		
		List<(object, string, bool)> entries = new();
		
		foreach (var inst in projector.ownedFamiliars)
		{
			bool alreadySummoned = alreadyOut.Contains(inst);
			string label = string.IsNullOrEmpty(inst.nickName) ? inst.data.name : inst.nickName;
			label += $"  (E {inst.energy})";
			
			entries.Add((inst, label, !alreadySummoned && projector.currentEnergy >= inst.energy));
		}
		
		GD.Print($"ProjectorCommand: {entries.Count} entries in summon list");
		
		battle.selectionPanel.OnItemChosen = OnFamiliarPicked;
		battle.selectionPanel.Open("Summon", entries);
		
		/*battle.HighlightAllySlots();
		battle.pendingCommand = new SummonCommand {
			sourceSide = battle.playerSide,
			source = battle.playerSide.projector
		};
		
		DisableCommands();
		undoButton.Visible = true;*/
		battle.BlockCommands();
		
		battle.RefreshNextButton();
	}
	
	public void OnDismissPressed()
	{
		if (battle.playerSide.CountActiveFamiliars() == 0)
		{
			return;
		}
		
		battle.SetCommandState(BattleManager.CommandState.SelectDismiss);
		battle.pendingCommand = null;
		
		battle.HighlightAllies();
		
		battle.BlockCommands();
		battle.RefreshNextButton();
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
		
		battle.projectorCommands.Add(cmd);
		battle.projCommandSubmitted = true;
		battle.AppendBattleText("Player: Focus Command added.");
		
		SetActiveCommand(cmd);
		
		battle.RefreshNextButton();
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
		battle.projCommandSubmitted = false;
		battle.projCommandDisabled = false;
		undoButton.Visible = false;
		
		battle.RefreshNextButton();
	}
	
	public void OnFamiliarPicked(object data)
	{
		if (data is not RFamiliarInstance inst)
		{
			GD.Print("ProjectorCommands: assigned data is not familiar");
			return;
		}
		
		battle.pendingCommand = new SummonCommand {
			sourceSide = battle.playerSide,
			source = battle.playerSide.projector,
			familiar = inst
		};
		
		try
		{
			GD.Print($"ProjectorCommands: familiar {inst.GetPreferredName()} picked");
		}
		catch (Exception e)
		{
			GD.PrintErr($"ProjectorCommands: failed to identify familiar - {e}");
		}
		
		battle.HighlightAllySlots();
		battle.pendingCommand = new SummonCommand {
			sourceSide = battle.playerSide,
			source = battle.playerSide.projector,
			familiar = inst
		};
		
		//battle.selectionPanel.UnblockCommands();
		DisableCommands();
		//undoButton.Visible = true;
	}
	
	public void CheckValidCommands()
	{
		disableSummon = !battle.playerSide.HasOpenSlot();
		disableDismiss = battle.playerSide.CountActiveFamiliars() == 0;
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
		summonButton.Disabled = disableSummon;
		dismissButton.Disabled = disableDismiss;
		//spellButton.Disabled = false;
		focusButton.Disabled = false;
		//itemButton.Disabled = false;
		//escapeButton.Disabled = false;
	}
	
	public void SetActiveCommand(BattleCommand command)
	{
		DisableCommands();
		battle.projCommandDisabled = true;
		activeCommand = command;
		undoButton.Visible = true;
	}
}
