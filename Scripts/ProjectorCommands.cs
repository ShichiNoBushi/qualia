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
	public bool disableSpell {get; set;} = false;
	public bool disableItem {get; set;} = false;
	public bool disableEscape {get; set;} = false;
	
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
		
		CheckValidCommands();
		EnableCommands();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnSummonPressed()
	{
		Projector projector = battle.playerSide.projector;
		
		if (projector == null)
		{
			battle.AppendBattleText("ProjectorCommand: null projector");
			GD.Print("ProjectorCommand: null projector");
			return;
		}
		
		battle.SetCommandState(BattleManager.CommandState.SelectSummon);
		
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
			string label = inst.GetPreferredName();
			label += $"  (E {inst.energy})";
			
			entries.Add((inst, label, !alreadySummoned && projector.currentEnergy >= inst.energy));
		}
		
		GD.Print($"ProjectorCommand: {entries.Count} entries in summon list");
		
		battle.selectionPanel.OnItemChosen = OnFamiliarPicked;
		battle.selectionPanel.Open("Summon", entries);
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
		Projector projector = battle.playerSide.projector;
		
		if (projector == null)
		{
			battle.AppendBattleText("ProjectorCommand: null projector");
			GD.Print("ProjectorCommand: null projector");
			return;
		}
		
		battle.SetCommandState(BattleManager.CommandState.SelectSpell);
		
		List<(object, string, bool)> entries = new();
		
		foreach (var spell in projector.spells)
		{
			bool validTargets = true;
			
			if (spell.spellPattern == RSpellData.SpellPattern.OneAlly && battle.playerSide.CountActiveFamiliars() == 0)
			{
				validTargets = false;
			}
			else if (spell.spellPattern == RSpellData.SpellPattern.OneEnemy && battle.enemySide.CountActiveFamiliars() == 0 && battle.enemySide.projector == null)
			{
				validTargets = false;
			}
			
			string label = string.IsNullOrEmpty(spell.name) ? "(no name)" : spell.name;
			label += $"  (E {spell.cost})";
			
			entries.Add((spell, label, validTargets && projector.currentEnergy > spell.cost));
		}
		
		GD.Print($"ProjectorCommand: {entries.Count} entries in spell list");
		
		battle.selectionPanel.OnItemChosen = OnSpellPicked;
		battle.selectionPanel.Open("Spell", entries);
		battle.BlockCommands();
		
		battle.RefreshNextButton();
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
		Projector projector = battle.playerSide.projector;
		
		if (projector == null)
		{
			battle.AppendBattleText("ProjectorCommand: null projector");
			GD.Print("ProjectorCommand: null projector");
			return;
		}
		
		battle.SetCommandState(BattleManager.CommandState.SelectItem);
		
		List<(object, string, bool)> entries = new();
		
		List<(RItemData item, ItemInstance unique)> totalInventory = new();
		
		foreach (var it in battle.session.itemStacks.Keys)
		{
			RItemData item = battle.registry.Item(it);
			
			if (item != null && item.battleUsable)
			{
				totalInventory.Add((item, null));
			}
		}
		
		foreach (var unq in battle.session.uniqueItems)
		{
			if (unq.data != null && unq.data.battleUsable)
			{
				totalInventory.Add((unq.data, unq));
			}
		}
		
		foreach (var itTup in totalInventory)
		{
			RItemData item = itTup.item;
			ItemInstance unique = itTup.unique;
			
			bool canAllyFam = item.familiarUsable && battle.playerSide.CountActiveFamiliars() > 0;
			bool canAllyProj = item.projectorUsable && battle.playerSide.projector != null;
			bool canEnemyFam = item.familiarUsable && battle.enemySide.CountActiveFamiliars() > 0;
			bool canEnemyProj = item.projectorUsable && battle.enemySide.projector != null && battle.enemySide.projector.currentEnergy > 0;
			
			bool validTargets = item.itemPattern switch
			{
				RItemData.ItemPattern.OneAlly => canAllyFam || canAllyProj,
				RItemData.ItemPattern.OneEnemy => canEnemyFam || canEnemyProj,
				RItemData.ItemPattern.AllAllies => canAllyFam || canAllyProj,
				RItemData.ItemPattern.AllEnemies => canEnemyFam || canEnemyProj,
				_ => true
			};
			
			string label = string.IsNullOrEmpty(item.name) ? "(no name)" : item.name;
			
			if (unique == null)
			{
				int count = battle.session.itemStacks.TryGetValue(item.id, out int n) ? n : 0;
				label += $"  (x{count})";
			}
			else
			{
				label += $"  ({unique.usesLeft} / {unique.maxUses})";
			}
			
			bool enough;
			
			if (unique == null)
			{
				enough = battle.session.itemStacks.TryGetValue(item.id, out int n) && n > 0;
				entries.Add((item, label, enough));
			}
			else
			{
				enough = unique.usesLeft > 0;
				entries.Add((unique, label, enough));
			}
		}
		
		GD.Print($"ProjectorCommand: {entries.Count} entries in item list");
		
		battle.selectionPanel.OnItemChosen = OnItemPicked;
		battle.selectionPanel.Open("Item", entries);
		battle.BlockCommands();
		
		battle.RefreshNextButton();
	}
	
	public void OnEscapePressed()
	{
		if (battle?.playerSide?.projector == null)
		{
			GD.Print("No projector.");
			return;
		}
		
		EscapeCommand cmd = new()
		{
			sourceSide = battle.playerSide,
			source = battle.playerSide.projector
		};
		
		battle.projectorCommands.Add(cmd);
		battle.projCommandSubmitted = true;
		battle.AppendBattleText("Player: Escape Command added.");
		
		SetActiveCommand(cmd);
		
		battle.RefreshNextButton();
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
		battle.AppendBattleText($"Player command submitted {battle.projCommandSubmitted}, {battle.famCommandsSubmitted} familiar commands submitted.");
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
		
		try
		{
			GD.Print($"ProjectorCommands: familiar {inst.GetPreferredName()} picked");
		}
		catch (Exception e)
		{
			GD.PrintErr($"ProjectorCommands: failed to identify familiar - {e}");
		}
		
		battle.HighlightAllySlots();
		battle.pendingCommand = new SummonCommand
		{
			sourceSide = battle.playerSide,
			source = battle.playerSide.projector,
			familiar = inst
		};
		
		DisableCommands();
	}
	
	public void OnSpellPicked(object data)
	{
		if (data is not RSpellData spell)
		{
			GD.Print("ProjectorCommands: assigned data is not a spell");
			return;
		}
		
		battle.pendingSource = battle.playerSide.projector;
		battle.pendingSpell = spell;
		
		try
		{
			GD.Print($"ProjectorCommands: spell {spell.name} picked");
		}
		catch (Exception e)
		{
			GD.PrintErr($"ProjectorCommands: failed to identify spell - {e}");
		}
		
		if (spell.spellPattern == RSpellData.SpellPattern.OneAlly)
		{
			battle.HighlightAllies();
			battle.SetCommandState(BattleManager.CommandState.SelectAllySpell);
		}
		else if (spell.spellPattern == RSpellData.SpellPattern.OneEnemy)
		{
			if (battle.enemySide.CountActiveFamiliars() > 0)
			{
				battle.HighlightEnemies();
			}
			
			battle.SetCommandState(BattleManager.CommandState.SelectEnemySpell);
		}
		else
		{
			SpellCommand cmd = new SpellCommand
			{
				sourceSide = battle.playerSide,
				source = battle.playerSide.projector
			};
			
			cmd.AssignSpell(spell);
			
			battle.projectorCommands.Add(cmd);
			battle.projCommandSubmitted = true;
			
			SetActiveCommand(cmd);
			
			battle.ClearTargetMode();
			battle.RefreshNextButton();
		}
	}
	
	public void OnItemPicked(object data)
	{
		RItemData item;
		ItemInstance unique = data as ItemInstance;
		
		if (unique != null)
		{
			item = unique.data;
		}
		else if (data is RItemData it)
		{
			item = it;
		}
		else
		{
			GD.Print("ProjectorCommands: assigned data is not an item");
			return;
		}
		
		battle.pendingSource = battle.playerSide.projector;
		battle.pendingItem = item;
		battle.pendingUnique = unique;
		
		try
		{
			GD.Print($"ProjectorCommands: spell {item.name} picked");
		}
		catch (Exception e)
		{
			GD.PrintErr($"ProjectorCommands: failed to identify item - {e}");
		}
		
		if (item.itemPattern == RItemData.ItemPattern.OneAlly)
		{
			if (item.familiarUsable && battle.playerSide.CountActiveFamiliars() > 0)
			{
				battle.HighlightAllies();
			}
			
			if (item.projectorUsable)
			{
				battle.projectorDisplayP.Highlight(true);
			}
			
			battle.SetCommandState(BattleManager.CommandState.SelectAllyItem);
		}
		else if (item.itemPattern == RItemData.ItemPattern.OneEnemy)
		{
			if (item.familiarUsable && battle.enemySide.CountActiveFamiliars() > 0)
			{
				battle.HighlightEnemies();
			}
			
			if (item.projectorUsable && battle.enemySide.CountActiveFamiliars() == 0 && battle.enemySide.projector != null && battle.enemySide.projector.currentEnergy > 0)
			{
				battle.projectorDisplayE.Highlight(true);
			}
			
			battle.SetCommandState(BattleManager.CommandState.SelectEnemyItem);
		}
		else
		{
			ItemCommand cmd = new ItemCommand
			{
				sourceSide = battle.playerSide,
				source = battle.playerSide.projector
			};
			
			cmd.AssignItem(item, unique);
			
			battle.projectorCommands.Add(cmd);
			battle.projCommandSubmitted = true;
			
			SetActiveCommand(cmd);
			
			battle.ClearTargetMode();
			battle.RefreshNextButton();
		}
	}
	
	public void CheckValidCommands()
	{
		disableSummon = battle.playerSide.projector.currentEnergy == 0 || !battle.playerSide.HasOpenSlot();
		disableDismiss = battle.playerSide.CountActiveFamiliars() == 0;
		disableSpell = battle.playerSide.projector.currentEnergy == 0;
		disableItem = battle.session.itemStacks.Count == 0 && battle.session.uniqueItems.Count == 0;
		disableEscape = battle.isProjectorEncounter;
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
		spellButton.Disabled = disableSpell;
		focusButton.Disabled = false;
		itemButton.Disabled = disableItem;
		escapeButton.Disabled = disableEscape;
	}
	
	public void SetActiveCommand(BattleCommand command)
	{
		DisableCommands();
		battle.projCommandDisabled = true;
		activeCommand = command;
		undoButton.Visible = true;
	}
}
