using Godot;
using System;
using System.Linq;

public partial class BattleManager : Node
{
	public GameSession session;
	public DataRegistry registry;
	
	public TextureRect backgroundRect;
	
	public ProjectorDisplay projectorDisplayE;
	public ProjectorDisplay projectorDisplayP;
	
	public FamiliarDisplay[] famDisplaysE;
	public FamiliarDisplay[] famDisplaysP;
	
	public RichTextLabel battleLogLabel;
	public Button nextButton;
	
	public ProjectorCommands projCommandPanel;
	public FamiliarCommands[] famCommandPanels = new FamiliarCommands[4];
	
	public bool projCommandDisabled;
	public bool[] famCommandDisabled = new bool[4];
	
	public SelectionPanel selectionPanel;
	
	public bool isProjectorEncounter;
	
	public BattleState batState;
	public CommandState comState;
	
	public BattleCommand pendingCommand;
	public RSkillData pendingSkill;
	public RSpellData pendingSpell;
	public RItemData pendingItem;
	public ItemInstance pendingUnique;
	public object pendingSource;
	
	public bool projCommandSubmitted;
	public int famCommandsSubmitted;
	
	public enum BattleState
	{
		Setup,
		CommandSelect,
		Resolution,
		SpawnCheck,
		EndCheck,
		Cleanup,
		Victory,
		Defeat,
		Escape
	}
	
	public enum CommandState
	{
		None,
		SelectSummon,
		SelectDismiss,
		SelectSpell,
		SelectItem,
		SelectAllySpell,
		SelectAllySkill,
		SelectAllyItem,
		SelectEnemyAttack,
		SelectEnemySpell,
		SelectEnemySkill,
		SelectEnemyItem
	}
	
	public enum VictoryResult
	{
		None,
		PlayerWin,
		PlayerLose,
		Draw,
		PlayerEscape
	}
	
	public BattleSide playerSide;
	public BattleSide enemySide;
	
	public Godot.Collections.Array<RFamiliarInstance> spawns;
	
	public Godot.Collections.Array<BattleCommand> projectorCommands;
	public Godot.Collections.Array<BattleCommand> familiarCommands;
	public Godot.Collections.Array<BattleCommand> turnCommands;
	
	public Godot.Collections.Array<RFamiliarInstance> summonedFamiliars;
	public Godot.Collections.Array<RFamiliarInstance> defeatedFamiliars;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		GD.Print($"BattleManager: _Ready on {Name}, path={GetPath()}, id={GetInstanceId()}");
		
		backgroundRect = GetNode<TextureRect>("BackgroundRect");
		
		projectorDisplayE = GetNode<ProjectorDisplay>("ProjectorDisplayEnemy");
		projectorDisplayP = GetNode<ProjectorDisplay>("ProjectorDisplayPlayer");
		
		projectorDisplayE.battle = this;
		projectorDisplayP.battle = this;
		
		famDisplaysE = new FamiliarDisplay[4];
		famDisplaysP = new FamiliarDisplay[4];
		
		famDisplaysE[0] = GetNode<FamiliarDisplay>("EFamiliarHBox/FamiliarDisplay0");
		famDisplaysE[1] = GetNode<FamiliarDisplay>("EFamiliarHBox/FamiliarDisplay1");
		famDisplaysE[2] = GetNode<FamiliarDisplay>("EFamiliarHBox/FamiliarDisplay2");
		famDisplaysE[3] = GetNode<FamiliarDisplay>("EFamiliarHBox/FamiliarDisplay3");
		
		famDisplaysP[0] = GetNode<FamiliarDisplay>("PFamiliarHBox/FamiliarDisplay0");
		famDisplaysP[1] = GetNode<FamiliarDisplay>("PFamiliarHBox/FamiliarDisplay1");
		famDisplaysP[2] = GetNode<FamiliarDisplay>("PFamiliarHBox/FamiliarDisplay2");
		famDisplaysP[3] = GetNode<FamiliarDisplay>("PFamiliarHBox/FamiliarDisplay3");
		
		for (int i = 0; i < 4; i++)
		{
			famDisplaysE[i].slotIndex = i;
			famDisplaysE[i].isPlayerSide = false;
			famDisplaysE[i].SetVisibleEnergy(false);
			famDisplaysE[i].battle = this;
			
			famDisplaysP[i].slotIndex = i;
			famDisplaysP[i].isPlayerSide = true;
			famDisplaysP[i].battle = this;
		}
		
		battleLogLabel = GetNode<RichTextLabel>("BattleLogLabel");
		nextButton = GetNode<Button>("NextButton");
		
		nextButton.Pressed += OnNextPressed;
		
		projCommandPanel = GetNode<ProjectorCommands>("ProjectorCommands");
		famCommandPanels[0] = GetNode<FamiliarCommands>("FamiliarCommands0");
		famCommandPanels[1] = GetNode<FamiliarCommands>("FamiliarCommands1");
		famCommandPanels[2] = GetNode<FamiliarCommands>("FamiliarCommands2");
		famCommandPanels[3] = GetNode<FamiliarCommands>("FamiliarCommands3");
		
		projCommandPanel.battle = this;
		
		for (int i = 0; i < 4; i++)
		{
			famCommandPanels[i].battle = this;
		}
		
		projCommandDisabled = false;
		for (int i = 0; i < 4; i++)
		{
			famCommandDisabled[i] = false;
		}
		
		selectionPanel = GetNode<SelectionPanel>("SelectionPanel");
		
		projCommandPanel.battle = this;
		
		foreach (var panel in famCommandPanels)
		{
			panel.battle = this;
		}
		
		selectionPanel.battle = this;
		
		if (session.playerProjector != null && session.pendingEncounter != null)
		{
			Initialize(session.playerProjector, session.pendingEncounter);
		}
		else
		{
			StartTest();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void StartTest()
	{
		GD.Print("BattleManager - Starting test...");
		
		RProjectorData pData = GD.Load<RProjectorData>("res://Resources/test_projector.tres");
		RFamiliarInstance gnomeInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_gnome.tres");
		RFamiliarInstance salamanderInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_salamander.tres");
		RFamiliarInstance sylphInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_sylph.tres");
		RFamiliarInstance undineInst = GD.Load<RFamiliarInstance>("res://Resources/FamiliarInstance/ex_undine.tres");
		
		if (pData == null)
		{
			GD.Print("BattleManager - Failed to start: invalid projector data");
			return;
		}
		
		Projector player = new();
		player.Initialize(pData);
		
		gnomeInst.Initialize();
		salamanderInst.Initialize();
		sylphInst.Initialize();
		undineInst.Initialize();
		
		player.GiveFamiliar(gnomeInst);
		player.GiveFamiliar(salamanderInst);
		player.GiveFamiliar(sylphInst);
		player.GiveFamiliar(undineInst);
		
		REncounterData eData = GD.Load<REncounterData>("res://Resources/encounters/test_encounter.tres");
		
		if (eData == null)
		{
			GD.Print("BattleManager - Failed to start: invalid encounter data");
			return;
		}
		
		Initialize(player, eData);
	}
	
	public void Initialize(Projector player, REncounterData encounter)
	{
		SetBattleState(BattleState.Setup);
		SetCommandState(CommandState.None);
		
		if (encounter == null || !encounter.IsValid())
		{
			return;
		}
		
		spawns = new();
		
		projectorCommands = new();
		familiarCommands = new();
		turnCommands = new();
		
		summonedFamiliars = new();
		defeatedFamiliars = new();
		
		isProjectorEncounter = encounter.isProjectorEncounter;
		
		playerSide = new(player);
		
		if (isProjectorEncounter)
		{
			Projector enemy = new();
			enemy.Initialize(encounter.enemyProjector);
			
			foreach (var fam in encounter.familiarList)
			{
				RFamiliarInstance familiar = fam.CreateInstance();
				enemy.GiveFamiliar(familiar);
			}
			
			enemySide = new(enemy);
		}
		else
		{
			enemySide = new(null);
			
			foreach (var fam in encounter.familiarList)
			{
				RFamiliarInstance familiar = fam.CreateInstance();
				
				if (familiar != null)
				{
					spawns.Add(familiar);
				}
			}
			
			SpawnSpark();
		}
		
		RefreshAllDisplays();
		BeginCommandSelect();
	}
	
	public void OnNextPressed()
	{
		if (batState == BattleState.Victory)
		{
			GD.Print("BattleManager: Victory!");
			FinishVictory();
			return;
		}
		
		if (batState == BattleState.Defeat)
		{
			GD.Print("BattleManager: Defeat.");
			FinishDefeat();
			return;
		}
		
		if (batState == BattleState.Escape)
		{
			GD.Print("BattleManager: Escape.");
			FinishEscape();
			return;
		}
		
		if (comState != CommandState.None)
		{
			CancelPendingCommand();
			return;
		}
		
		if (batState != BattleState.CommandSelect || !PlayerCommandsSubmitted())
		{
			return;
		}
		
		CommitTurn();
	}
	
	public void FamiliarSlotClicked(FamiliarDisplay display)
	{
		GD.Print($"BattleManager: battle state {batState}, command state {comState}");
		
		if (batState != BattleState.CommandSelect)
		{
			return;
		}
		
		switch (comState)
		{
			case CommandState.SelectSummon:
				try
				{
					TryFinishSummon(display);
				}
				catch (Exception e)
				{
					GD.PrintErr($"BattleManager: Summon failed - {e}");
				}
				break;
			case CommandState.SelectDismiss:
				TryFinishDismiss(display);
				break;
			case CommandState.SelectAllySpell:
			case CommandState.SelectEnemySpell:
				TryFinishSpell(display);
				break;
			case CommandState.SelectAllySkill:
			case CommandState.SelectEnemySkill:
				TryFinishSkill(display);
				break;
			case CommandState.SelectAllyItem:
			case CommandState.SelectEnemyItem:
				TryFinishItem(display);
				break;
			case CommandState.SelectEnemyAttack:
				TryFinishAttack(display);
				break;
		}
	}
	
	public void ProjectorClicked(ProjectorDisplay display)
	{
		GD.Print($"BattleManager: battle state {batState}, command state {comState}");
		
		if (batState != BattleState.CommandSelect)
		{
			return;
		}
		
		switch (comState)
		{
			case CommandState.SelectAllySpell:
			case CommandState.SelectEnemySpell:
				TryFinishSpell(display);
				break;
			case CommandState.SelectAllySkill:
			case CommandState.SelectEnemySkill:
				TryFinishSkill(display);
				break;
			case CommandState.SelectAllyItem:
			case CommandState.SelectEnemyItem:
				TryFinishItem(display);
				break;
			case CommandState.SelectEnemyAttack:
				TryFinishAttack(display);
				break;
		}
	}
	
	public void RefreshNextButton()
	{
		if (batState == BattleState.CommandSelect)
		{
			bool canCommit = PlayerCommandsSubmitted();
			bool canCancel = comState != CommandState.None;
			nextButton.Text = canCommit ? "Commit" : (canCancel ? "Cancel" : "Next");
			nextButton.Disabled = !canCommit && !canCancel;
		}
		else if (batState == BattleState.Victory || batState == BattleState.Defeat || batState == BattleState.Escape)
		{
			nextButton.Text = "Finish";
			nextButton.Disabled = false;
		}
		else
		{
			nextButton.Text = "Next";
			nextButton.Disabled = false;
		}
	}
	
	public void RefreshCommandPanels()
	{
		projCommandDisabled = false;
		
		projCommandPanel.CheckValidCommands();
		
		for (int i = 0; i < 4; i++)
		{
			FamiliarActor actor = playerSide?.familiarSlots[i] as FamiliarActor;
			bool alive = actor != null && actor.isAlive;
			famCommandPanels[i].SetElementsVisible(alive);
			famCommandPanels[i].Bind(alive ? actor : null);
			famCommandPanels[i].CheckValidCommands();
			famCommandDisabled[i] = !alive;
		}
	}
	
	public void SetBattleState(BattleState newState)
	{
		batState = newState;
	}
	
	public void SetCommandState(CommandState newState)
	{
		comState = newState;
	}
	
	public void BeginCommandSelect()
	{
		SetBattleState(BattleState.CommandSelect);
		SetCommandState(CommandState.None);
		
		projectorCommands.Clear();
		familiarCommands.Clear();
		turnCommands.Clear();
		
		ResetSideModifiers(playerSide);
		ResetSideModifiers(enemySide);
		
		RefreshCommandPanels();
		
		projCommandPanel.EnableCommands();
		projCommandPanel.undoButton.Visible = false;
		
		for (int i = 0; i < 4; i++)
		{
			FamiliarCommands panel = famCommandPanels[i];
			if (!famCommandDisabled[i])
			{
				panel.EnableCommands();
			}
			panel.undoButton.Visible = false;
		}
		
		for (int i = 0; i < 4; i++)
		{
			famCommandPanels[i].SetElementsVisible(!playerSide.IsSlotEmpty(i));
		}
		
		projCommandSubmitted = false;
		famCommandsSubmitted = 0;
		RefreshNextButton();
	}
	
	public void ResetSideModifiers(BattleSide side)
	{
		foreach (var slot in side.familiarSlots)
		{
			if (slot is FamiliarActor fam)
			{
				fam.ResetTurnModifiers();
			}
		}
	}
	
	public void TryFinishSummon(FamiliarDisplay display)
	{
		int slot = display.slotIndex;
		
		if (slot < 0 || !playerSide.IsSlotEmpty(slot))
		{
			return;
		}
		
		if (pendingCommand is SummonCommand summon)
		{
			summon.slot = slot;
			
			string projName = "(No name)";
			string famName = "(No name)";
			
			try
			{
				projName = ((Projector)summon.source).name;
			}
			catch (Exception e)
			{
				GD.PrintErr($"BattleManager: failed to get porjector name - {e}");
			}
			
			try
			{
				famName = summon.familiar.GetPreferredName();
			}
			catch (Exception e)
			{
				GD.PrintErr($"BattleManager: failed to get familiar name - {e}");
			}
			
			GD.Print($"BattleManager: Adding summon command source {projName}, familiar {famName}, slot {slot}");
			projectorCommands.Add(summon);
			projCommandPanel.SetActiveCommand(summon);
			
			ClearTargetMode();
			projCommandPanel.DisableCommands();
			projCommandSubmitted = true;
			projCommandDisabled = true;
			
			RefreshNextButton();
		}
	}
	
	public void TryFinishDismiss(FamiliarDisplay display)
	{
		int slot = display.slotIndex;
		
		if (slot < 0)
		{
			return;
		}
		
		if (playerSide.familiarSlots[slot] is not FamiliarActor actor || !actor.isAlive)
		{
			return;
		}
		
		DismissCommand cmd = new DismissCommand {
			sourceSide = playerSide,
			source = playerSide.projector,
			target = actor
		};
		
		projectorCommands.Add(cmd);
		projCommandPanel.SetActiveCommand(cmd);
		
		ClearTargetMode();
		projCommandPanel.DisableCommands();
		projCommandSubmitted = true;
		projCommandDisabled = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishSpell(FamiliarDisplay display)
	{
		if (pendingSource is not Projector proj || proj.currentEnergy == 0)
		{
			return;
		}
		
		if (pendingSpell == null)
		{
			return;
		}
		
		if (pendingSpell.spellPattern == RSpellData.SpellPattern.OneEnemy && display.isPlayerSide)
		{
			return;
		}
		
		if (pendingSpell.spellPattern == RSpellData.SpellPattern.OneAlly && !display.isPlayerSide)
		{
			return;
		}
		
		BattleSide targetSide = display.isPlayerSide ? playerSide : enemySide;
		
		int slot = display.slotIndex;
		
		if (slot < 0 || slot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		if (targetSide.familiarSlots[slot] is not FamiliarActor actor || !actor.isAlive)
		{
			return;
		}
		
		SpellCommand cmd = new SpellCommand {
			sourceSide = playerSide,
			source = pendingSource,
			target = actor,
		};
		
		cmd.AssignSpell(pendingSpell);
		
		projectorCommands.Add(cmd);
		projCommandPanel.SetActiveCommand(cmd);
		
		ClearTargetMode();
		projCommandSubmitted = true;
		projCommandDisabled = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishSpell(ProjectorDisplay display)
	{
		if (pendingSource is not Projector proj || proj.currentEnergy == 0)
		{
			return;
		}
		
		if (pendingSpell == null)
		{
			return;
		}
		
		if (pendingSpell.spellPattern == RSpellData.SpellPattern.OneEnemy)
		{
			if (display == projectorDisplayP)
			{
				return;
			}
			
			if (enemySide.CountActiveFamiliars() > 0)
			{
				return;
			}
		}
		
		if (pendingSpell.spellPattern == RSpellData.SpellPattern.OneAlly)
		{
			return;
		}
		
		BattleSide targetSide = display == projectorDisplayP ? playerSide : enemySide;
		
		Projector projector = targetSide.projector;
		
		if (projector == null || projector.currentEnergy == 0)
		{
			return;
		}
		
		SpellCommand cmd = new SpellCommand {
			sourceSide = playerSide,
			source = pendingSource,
			target = projector,
		};
		
		cmd.AssignSpell(pendingSpell);
		
		projectorCommands.Add(cmd);
		projCommandPanel.SetActiveCommand(cmd);
		
		ClearTargetMode();
		projCommandSubmitted = true;
		projCommandDisabled = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishItem(FamiliarDisplay display)
	{
		bool enough;
		
		if (pendingItem != null && pendingUnique == null)
		{
			enough = session.itemStacks.TryGetValue(pendingItem.id, out int n) && n > 0;
		}
		else if (pendingItem != null && pendingUnique != null)
		{
			enough = pendingUnique.usesLeft > 0;
		}
		else
		{
			enough = false;
		}
		
		if (pendingSource is not Projector proj || !enough)
		{
			return;
		}
		
		if (!pendingItem.familiarUsable)
		{
			return;
		}
		
		if (pendingItem.itemPattern == RItemData.ItemPattern.OneEnemy && display.isPlayerSide)
		{
			return;
		}
		
		if (pendingItem.itemPattern == RItemData.ItemPattern.OneAlly && !display.isPlayerSide)
		{
			return;
		}
		
		BattleSide targetSide = display.isPlayerSide ? playerSide : enemySide;
		
		int slot = display.slotIndex;
		
		if (slot < 0 || slot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		if (targetSide.familiarSlots[slot] is not FamiliarActor actor || !actor.isAlive)
		{
			return;
		}
		
		ItemCommand cmd = new ItemCommand {
			sourceSide = playerSide,
			source = pendingSource,
			target = actor,
		};
		
		cmd.AssignItem(pendingItem, pendingUnique);
		
		projectorCommands.Add(cmd);
		projCommandPanel.SetActiveCommand(cmd);
		
		ClearTargetMode();
		projCommandSubmitted = true;
		projCommandDisabled = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishItem(ProjectorDisplay display)
	{
		bool enough;
		
		if (pendingItem != null && pendingUnique == null)
		{
			enough = session.itemStacks.TryGetValue(pendingItem.id, out int n) && n > 0;
		}
		else if (pendingItem != null && pendingUnique != null)
		{
			enough = pendingUnique.usesLeft > 0;
		}
		else
		{
			enough = false;
		}
		
		if (pendingSource is not Projector proj || !enough)
		{
			return;
		}
		
		if (!pendingItem.projectorUsable)
		{
			return;
		}
		
		if (pendingItem.itemPattern == RItemData.ItemPattern.OneEnemy)
		{
			if (display == projectorDisplayP)
			{
				return;
			}
			
			if (enemySide.CountActiveFamiliars() > 0)
			{
				return;
			}
		}
		
		if (pendingItem.itemPattern == RItemData.ItemPattern.OneAlly && display == projectorDisplayE)
		{
			return;
		}
		
		BattleSide targetSide = display == projectorDisplayP ? playerSide : enemySide;
		
		Projector projector = targetSide.projector;
		
		if (projector == null || projector.currentEnergy == 0)
		{
			return;
		}
		
		ItemCommand cmd = new ItemCommand {
			sourceSide = playerSide,
			source = pendingSource,
			target = projector,
		};
		
		cmd.AssignItem(pendingItem, pendingUnique);
		
		projectorCommands.Add(cmd);
		projCommandPanel.SetActiveCommand(cmd);
		
		ClearTargetMode();
		projCommandSubmitted = true;
		projCommandDisabled = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishAttack(FamiliarDisplay display)
	{
		if (pendingSource is not FamiliarActor srcFam || !srcFam.isAlive)
		{
			return;
		}
		
		if (display.isPlayerSide)
		{
			return;
		}
		
		int slot = display.slotIndex;
		
		if (slot < 0 || slot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		if (enemySide.familiarSlots[slot] is not FamiliarActor actor || !actor.isAlive)
		{
			return;
		}
		
		int sourceSlot = playerSide.GetSlotIndex(srcFam);
		
		if (sourceSlot < 0 || sourceSlot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		AttackCommand cmd = new AttackCommand {
			sourceSide = playerSide,
			source = srcFam,
			target = actor,
			power = 5
		};
		
		familiarCommands.Add(cmd);
		famCommandPanels[sourceSlot].SetActiveCommand(cmd);
		
		ClearTargetMode();
		famCommandsSubmitted++;
		famCommandDisabled[sourceSlot] = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishAttack(ProjectorDisplay display)
	{
		if (pendingSource is not FamiliarActor srcFam || !srcFam.isAlive)
		{
			return;
		}
		
		if (display == projectorDisplayP)
		{
			return;
		}
		
		if (enemySide.CountActiveFamiliars() > 0)
		{
			return;
		}
		
		Projector projector = enemySide.projector;
		
		if (projector == null || projector.currentEnergy == 0)
		{
			return;
		}
		
		int sourceSlot = playerSide.GetSlotIndex(srcFam);
		
		if (sourceSlot < 0 || sourceSlot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		AttackCommand cmd = new AttackCommand {
			sourceSide = playerSide,
			source = srcFam,
			target = projector,
			power = 5
		};
		
		familiarCommands.Add(cmd);
		famCommandPanels[sourceSlot].SetActiveCommand(cmd);
		
		ClearTargetMode();
		famCommandsSubmitted++;
		famCommandDisabled[sourceSlot] = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishSkill(FamiliarDisplay display)
	{
		if (pendingSource is not FamiliarActor srcFam || !srcFam.isAlive)
		{
			return;
		}
		
		if (pendingSkill == null)
		{
			return;
		}
		
		if (pendingSkill.targetPattern == RSkillData.TargetPattern.OneEnemy && display.isPlayerSide)
		{
			return;
		}
		
		if (pendingSkill.targetPattern == RSkillData.TargetPattern.OneAlly && !display.isPlayerSide)
		{
			return;
		}
		
		BattleSide targetSide = display.isPlayerSide ? playerSide : enemySide;
		
		int slot = display.slotIndex;
		
		if (slot < 0 || slot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		if (targetSide.familiarSlots[slot] is not FamiliarActor actor || !actor.isAlive)
		{
			return;
		}
		
		int sourceSlot = playerSide.GetSlotIndex(srcFam);
		
		if (sourceSlot < 0 || sourceSlot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		SkillCommand cmd = new SkillCommand {
			sourceSide = playerSide,
			source = srcFam,
			target = actor,
		};
		
		cmd.AssignSkill(pendingSkill);
		
		familiarCommands.Add(cmd);
		famCommandPanels[sourceSlot].SetActiveCommand(cmd);
		
		ClearTargetMode();
		famCommandsSubmitted++;
		famCommandDisabled[sourceSlot] = true;
		
		RefreshNextButton();
	}
	
	public void TryFinishSkill(ProjectorDisplay display)
	{
		if (pendingSource is not FamiliarActor srcFam || !srcFam.isAlive)
		{
			return;
		}
		
		if (pendingSkill == null)
		{
			return;
		}
		
		if (pendingSkill.targetPattern == RSkillData.TargetPattern.OneEnemy)
		{
			if (display == projectorDisplayP)
			{
				return;
			}
			
			if (enemySide.CountActiveFamiliars() > 0)
			{
				return;
			}
		}
		
		if (pendingSkill.targetPattern == RSkillData.TargetPattern.OneAlly)
		{
			return;
		}
		
		BattleSide targetSide = display == projectorDisplayP ? playerSide : enemySide;
		
		Projector projector = targetSide.projector;
		
		if (projector == null || projector.currentEnergy == 0)
		{
			return;
		}
		
		int sourceSlot = playerSide.GetSlotIndex(srcFam);
		
		if (sourceSlot < 0 || sourceSlot >= BattleSide.MAX_SLOTS)
		{
			return;
		}
		
		SkillCommand cmd = new SkillCommand {
			sourceSide = playerSide,
			source = srcFam,
			target = projector,
		};
		
		cmd.AssignSkill(pendingSkill);
		
		familiarCommands.Add(cmd);
		famCommandPanels[sourceSlot].SetActiveCommand(cmd);
		
		ClearTargetMode();
		famCommandsSubmitted++;
		famCommandDisabled[sourceSlot] = true;
		
		RefreshNextButton();
	}
	
	public void ClearTargetMode()
	{
		GD.Print("BattleManager: clearing targets");
		
		SetCommandState(CommandState.None);
		pendingCommand = null;
		pendingSkill = null;
		pendingSpell = null;
		pendingItem = null;
		pendingUnique = null;
		pendingSource = null;
		ClearHighlights();
		UnblockCommands();
	}
	
	public void HighlightAllySlots()
	{
		foreach (var panel in famDisplaysP)
		{
			int slot = panel.slotIndex;
			if (playerSide.IsSlotEmpty(slot))
			{
				panel.HighlightAlly(true);
			}
		}
	}
	
	public void HighlightAllies()
	{
		foreach (var panel in famDisplaysP)
		{
			int slot = panel.slotIndex;
			if (!playerSide.IsSlotEmpty(slot))
			{
				panel.HighlightAlly(true);
			}
		}
	}
	
	public void HighlightEnemies()
	{
		foreach (var panel in famDisplaysE)
		{
			int slot = panel.slotIndex;
			if (!enemySide.IsSlotEmpty(slot) && enemySide.familiarSlots[slot].isAlive)
			{
				panel.HighlightEnemy(true);
			}
		}
	}
	
	public void ClearHighlights()
	{
		GD.Print("BattleManager: clearing highlights");
		
		projectorDisplayE.Highlight(false);
		projectorDisplayP.Highlight(false);
		
		foreach (var panel in famDisplaysE)
		{
			panel.ClearHighlights();
		}
		
		foreach (var panel in famDisplaysP)
		{
			panel.ClearHighlights();
		}
	}
	
	public void BlockCommands()
	{
		projCommandPanel.DisableCommands();
		
		for (int i = 0; i < 4; i++)
		{
			famCommandPanels[i].DisableCommands();
		}
	}
	
	public void UnblockCommands()
	{
		if (!projCommandDisabled)
		{
			projCommandPanel.EnableCommands();
		}
		
		for (int i = 0; i < 4; i++)
		{
			if (!famCommandDisabled[i])
			{
				famCommandPanels[i].EnableCommands();
			}
		}
	}
	
	public void FinishVictory()
	{
		DismissAllAndRefund(playerSide);
		
		ReturnToField(VictoryResult.PlayerWin);
	}
	
	public void FinishDefeat()
	{
		playerSide.projector.Recover();
		
		ReturnToField(VictoryResult.PlayerLose);
	}
	
	public void FinishEscape()
	{
		DismissAllAndRefund(playerSide);
		
		ReturnToField(VictoryResult.PlayerEscape);
	}
	
	public void CancelPendingCommand()
	{
		AppendBattleText("Cancel pending command");
		selectionPanel.HidePanel();
		pendingCommand = null;
		pendingSkill = null;
		pendingSpell = null;
		pendingSource = null;
		SetCommandState(CommandState.None);
		
		UnblockCommands();
		ClearHighlights();
		RefreshNextButton();
	}
	
	public void CommitTurn()
	{
		AppendBattleText("Resolving turn");
		try
		{
			projCommandSubmitted = false;
			famCommandsSubmitted = 0;
			SetBattleState(BattleState.Resolution);
			SetCommandState(CommandState.None);
			
			RefreshNextButton();
			
			AssignComCommands();
			BuildTurnOrder();
			ResolveTurn();
		}
		catch (Exception e)
		{
			GD.PrintErr($"BattleManager: Error resolving turn {e}");
			AppendBattleText("Error resolving turn");
		}
	}
	
	public void DismissAllAndRefund(BattleSide side)
	{
		if (side?.projector == null)
		{
			return;
		}
		
		foreach (var fam in side.GetFamiliarList())
		{
			side.projector.currentEnergy = Math.Min(side.projector.currentEnergy + fam.currentEnergy, side.projector.maxEnergy);
		}
	}
	
	public void AssignComCommands()
	{
		IEncounterAI ai;
		
		if (isProjectorEncounter && enemySide.projector != null)
		{
			ai = new RandomProjectorAI();
		}
		else
		{
			ai = new RandomWildAI();
		}
		
		if (isProjectorEncounter && enemySide.projector != null)
		{
			BattleCommand pCmd = ai.PickProjectorAction(this, enemySide);
			if (pCmd != null)
			{
				projectorCommands.Add(pCmd);
			}
		}
		
		foreach (var fam in enemySide.GetFamiliarList())
		{
			BattleCommand fCmd = ai.PickFamiliarAction(this, fam);
			if (fCmd != null)
			{
				GD.Print($"AI add slot={fam.slot} {fam.name} -> {fCmd.GetType().Name}");
				familiarCommands.Add(fCmd);
			}
		}
	}
	
	public void BuildTurnOrder()
	{
		turnCommands.Clear();
		
		var playerProjCmds = projectorCommands.Where(c => c.sourceSide == playerSide);
		var enemyProjCmds = projectorCommands.Where(c => c.sourceSide == enemySide);
		
		if (GD.Randi() % 2 == 0)
		{
			foreach (var cmd in playerProjCmds)
			{
				turnCommands.Add(cmd);
			}
			
			foreach (var cmd in enemyProjCmds)
			{
				turnCommands.Add(cmd);
			}
		}
		else
		{
			foreach (var cmd in enemyProjCmds)
			{
				turnCommands.Add(cmd);
			}
			
			foreach (var cmd in playerProjCmds)
			{
				turnCommands.Add(cmd);
			}
		}
		
		var sortedFamiliarCmds = familiarCommands.OrderByDescending(cmd =>
		{
			float speed = 0f;
			
			if (cmd.source is FamiliarActor fam)
			{
				speed = fam.ModSpeed() * cmd.speedFactor;
			}
			
			return speed * 100f + (int)GD.Randi() % 20;
		}).ToList();
		
		foreach (var cmd in sortedFamiliarCmds)
		{
			turnCommands.Add(cmd);
		}
	}
	
	public void InvalidateFamiliarCommands(FamiliarActor actor, BattleCommand except = null)
	{
		foreach (var cmd in turnCommands)
		{
			if (cmd == null || ReferenceEquals(cmd, except))
			{
				continue;
			}
			
			if (ReferenceEquals(cmd.source, actor))
			{
				cmd.isValid = false;
			}
			
			if (ReferenceEquals(cmd.target, actor))
			{
				cmd.Retarget(this);
			}
		}
	}
	
	public void ResolveTurn()
	{
		foreach (var cmd in turnCommands)
		{
			if (!cmd.isValid)
			{
				continue;
			}
			
			//cmd.Retarget(this);
			
			if (cmd.isValid)
			{
				try
				{
					cmd.Execute(this);
					
					if (batState == BattleState.Escape)
					{
						EndCheck();
						return;
					}
				}
				catch (Exception e)
				{
					GD.PrintErr($"BattleManager: failed to execute command - {e}");
				}
			}
		}
		
		SetBattleState(BattleState.SpawnCheck);
		SpawnCheck();
	}
	
	public void SpawnSpark()
	{
		if (isProjectorEncounter || spawns.Count == 0)
		{
			return;
		}
		
		int slot = enemySide.GetPreferredOpenSlot();
		
		if (slot >= 0)
		{
			RFamiliarInstance fam = spawns[0];
			
			SpawnActor spark = new(fam);
			
			if (enemySide.TrySpawn(spark, slot))
			{
				spawns.RemoveAt(0);
				famDisplaysE[slot].AssignSpawn(spark);
				AppendBattleText("A familiar starts to manifest...");
			}
			else if (spark == null)
			{
				spawns.RemoveAt(0);
			}
		}
	}
	
	public VictoryResult CheckVictory()
	{
		bool playerDefeat = playerSide.projector != null && playerSide.projector.currentEnergy <= 0 && playerSide.CountActiveFamiliars() == 0;
		bool enemyDefeat = false;
		
		if (isProjectorEncounter)
		{
			enemyDefeat = enemySide.projector != null && enemySide.projector.currentEnergy <= 0 && enemySide.CountActiveFamiliars() == 0;
		}
		else
		{
			enemyDefeat = enemySide.CountActiveFamiliars() == 0 && PendingSparks() == 0 && (spawns == null || spawns.Count == 0);
		}
		
		if (enemyDefeat && !playerDefeat)
		{
			AppendBattleText($"[b]{playerSide.projector.name}[/b] wins!!");
			return VictoryResult.PlayerWin;
		}
		else if (playerDefeat && !enemyDefeat)
		{
			AppendBattleText($"[b]{playerSide.projector.name}[/b] loses.");
			return VictoryResult.PlayerLose;
		}
		else if (playerDefeat && enemyDefeat)
		{
			AppendBattleText($"Both sides defeated.");
			return VictoryResult.Draw;
		}
		
		return VictoryResult.None;
	}
	
	public int PendingSparks()
	{
		int n = 0;
		
		foreach (var slot in enemySide.familiarSlots)
		{
			if (slot is SpawnActor)
			{
				n++;
			}
		}
		
		return n;
	}
	
	public void SpawnCheck()
	{
		if (isProjectorEncounter)
		{
			SetBattleState(BattleState.EndCheck);
			EndCheck();
			return;
		}
		
		for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
		{
			if (enemySide.familiarSlots[i] is SpawnActor spark)
			{
				FamiliarActor actor = enemySide.SpawnFamiliar(spark);
				
				if (actor != null)
				{
					AppendBattleText($"A [b]{actor.name}[/b] manifests.");
				}
				else
				{
					AppendBattleText("Spark fails to manifest.");
				}
			}
		}
		
		if (spawns.Count > 0 && enemySide.HasOpenSlot())
		{
			SpawnSpark();
		}
		
		RefreshAllDisplays();
		
		SetBattleState(BattleState.EndCheck);
		EndCheck();
	}
	
	public void EndCheck()
	{
		if (batState == BattleState.Escape)
		{
			RefreshNextButton();
			return;
		}
		
		VictoryResult result = CheckVictory();
		
		switch (result)
		{
			case VictoryResult.PlayerWin:
				SetBattleState(BattleState.Victory);
				GrantRewards();
				RefreshNextButton();
				break;
			case VictoryResult.PlayerLose:
			case VictoryResult.Draw:
				SetBattleState(BattleState.Defeat);
				RefreshNextButton();
				break;
			case VictoryResult.None:
				BeginCommandSelect();
				break;
		}
	}
	
	public void GrantRewards()
	{
		int totalExp = 0;
		int totalCrystals = 0;
		Godot.Collections.Dictionary<string, int> qualiaCrystals = new();
		
		foreach (var fam in defeatedFamiliars)
		{
			if (fam == null)
			{
				continue;
			}
			
			float growthFactor = fam.data != null ? fam.data.expGrowthFactor : 1f;
			totalExp += Mathf.RoundToInt(fam.level * 100 * growthFactor);
			totalCrystals += fam.level * fam.data.crystals;
			
			if (fam.data.crystalDrops == null)
			{
				continue;
			}
			
			foreach (var qc in fam.data.crystalDrops)
			{
				if (string.IsNullOrEmpty(qc.Key) || qc.Value <= 0)
				{
					continue;
				}
				
				int amt = qc.Value * Math.Max(fam.level, 1);
				qualiaCrystals[qc.Key] = (qualiaCrystals.TryGetValue(qc.Key, out int c) ? c : 0) + amt;
			}
		}
		
		int sharedExp = Math.Max(totalExp / Math.Max(summonedFamiliars.Count, 1), 1);
		
		AppendBattleText($"[b]{playerSide.projector.name}[/b] gains {totalExp} experience.");
		AppendBattleText($"Familiars gain {sharedExp} experience.", false);
		
		int projLevelIncrease = playerSide.projector.GiveExperience(totalExp);
		
		if (projLevelIncrease == 1)
		{
			AppendBattleText($"[b]{playerSide.projector.name}[/b] leveled up!", true);
		}
		else if (projLevelIncrease >= 2)
		{
			AppendBattleText($"[b]{playerSide.projector.name}[/b] gained {projLevelIncrease} levels!", true);
		}
		
		foreach (var fam in summonedFamiliars)
		{
			int levelIncrease = fam.GiveExperience(sharedExp);
			
			if (levelIncrease == 1)
			{
				AppendBattleText($"[b]{fam.GetPreferredName()}[/b] leveled up!", true);
			}
			else if (levelIncrease >= 2)
			{
				AppendBattleText($"[b]{fam.GetPreferredName()}[/b] gained {levelIncrease} levels!", true);
			}
		}
		
		DataRegistry registry = GetNode<DataRegistry>("/root/DataRegistry");
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		bool firstLine = true;
		
		foreach (var drop in qualiaCrystals)
		{
			RQualiaCrystal crystal = registry.Crystal(drop.Key);
			
			if (crystal == null || drop.Value <= 0)
			{
				continue;
			}
			
			string name = string.IsNullOrEmpty(crystal.name) ? crystal.id : crystal.name;
			
			session.AddCrystal(crystal, drop.Value);
			
			AppendBattleText($"[b]{playerSide.projector.name}[/b] acquired [b]{drop.Value}[/b] {name} crystals (total: {session.qualiaCrystals[drop.Key]}).", firstLine);
			firstLine = false;
		}
		
		if (totalCrystals > 0)
		{
			session.qualiaGeneric += totalCrystals;
			AppendBattleText($"[b]{playerSide.projector.name}[/b] acquired [b]{totalCrystals}[/b] generic qualia crystals.");
		}
	}
	
	public void ReturnToField(VictoryResult result)
	{
		GameSession session = GetNode<GameSession>("/root/GameSession");
		session.lastResult = result;
		session.pendingEncounter = null;
		session.gameMode = GameSession.GameMode.World;
		
		string path = result == VictoryResult.PlayerLose ? session.safePath : session.returnPath;
		
		if (string.IsNullOrEmpty(path))
		{
			GD.PrintErr($"BattleManager: no return path for {result}");
			GetTree().Quit();
			return;
		}
		
		GetTree().ChangeSceneToFile(path);
	}
	
	public FamiliarDisplay[] GetFamiliarDisplays(BattleSide side)
	{
		return side == playerSide ? famDisplaysP : famDisplaysE;
	}
	
	public int IndexOfDisplay(FamiliarDisplay display, FamiliarDisplay[] panels)
	{
		for (int i = 0; i < panels.Length; i++)
		{
			if (display == panels[i])
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public bool PlayerCommandsSubmitted()
	{
		return projCommandSubmitted && famCommandsSubmitted >= playerSide.CountActiveFamiliars();
	}
	
	public void AppendBattleText(string text, bool doubleSpace = true)
	{
		if (battleLogLabel.GetLineCount() > 0)
		{
			battleLogLabel.Newline();
			
			if (doubleSpace)
			{
				battleLogLabel.Newline();
			}
		}
		
		battleLogLabel.AppendText(text);
	}
	
	public void RefreshAllDisplays()
	{
		GD.Print("BattleManager - Refreshing displays...");
		
		if (playerSide?.projector != null)
		{
			projectorDisplayP.AssignProjector(playerSide.projector);
		}
		else
		{
			GD.Print("BattleManager - null player projector");
			projectorDisplayP.Clear();
		}
		
		projectorDisplayP.SetVisibleEnergy(true);
		projectorDisplayP.UpdateDisplay();
		
		if (enemySide?.projector != null)
		{
			projectorDisplayE.AssignProjector(enemySide.projector);
		}
		else
		{
			GD.Print("BattleManager - null enemy projector");
			projectorDisplayE.Clear();
		}
		
		projectorDisplayE.UpdateDisplay();
		
		for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
		{
			BindSlotDisplay(famDisplaysP[i], playerSide.familiarSlots[i]);
			famDisplaysP[i].UpdateDisplay();
			BindSlotDisplay(famDisplaysE[i], enemySide.familiarSlots[i]);
			famDisplaysE[i].UpdateDisplay();
		}
		
		/*for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
		{
			IBattleActor actor = playerSide.familiarSlots[i];
			FamiliarDisplay display = famDisplaysP[i];
			
			if (actor is FamiliarActor f)
			{
				display.AssignFamiliar(f);
			}
			else if (actor is SpawnActor s)
			{
				display.AssignSpawn(s);
			}
			else
			{
				display.Clear();
			}
			
			display.UpdateDisplay();
		}
		
		for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
		{
			IBattleActor actor = enemySide.familiarSlots[i];
			FamiliarDisplay display = famDisplaysE[i];
			
			if (actor is FamiliarActor f)
			{
				display.AssignFamiliar(f);
			}
			else if (actor is SpawnActor s)
			{
				display.AssignSpawn(s);
			}
			else
			{
				display.Clear();
			}
			
			display.UpdateDisplay();
		}*/
	}
	
	private void BindSlotDisplay(FamiliarDisplay display, IBattleActor actor)
	{
		if (actor is FamiliarActor f)
		{
			display.AssignFamiliar(f);
		}
		else if (actor is SpawnActor s)
		{
			display.AssignSpawn(s);
		}
		else
		{
			display.Clear();
		}
	}
}

public partial class BattleSide : RefCounted
{
	public const int MAX_SLOTS = 4;
	
	public static readonly int[] slotPriority = {1, 2, 0, 3};
	
	public Projector projector {get; set;}
	public IBattleActor[] familiarSlots {get; set;} = new IBattleActor[MAX_SLOTS];
	
	public BattleSide(Projector p)
	{
		projector = p;
		
		familiarSlots = new IBattleActor[MAX_SLOTS];
	}
	
	public bool IsSlotEmpty(int index)
	{
		return index >= 0 && index < MAX_SLOTS && familiarSlots[index] == null;
	}
	
	public bool HasOpenSlot()
	{
		for (int i = 0; i < MAX_SLOTS; i++)
		{
			if (familiarSlots[i] == null)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public int GetPreferredOpenSlot()
	{
		foreach (int idx in slotPriority)
		{
			if (IsSlotEmpty(idx))
			{
				return idx;
			}
		}
		
		return -1;
	}
	
	public int GetSlotIndex(IBattleActor actor)
	{
		if (actor == null)
		{
			return -1;
		}
		
		for (int i = 0; i < MAX_SLOTS; i ++)
		{
			if (familiarSlots[i] == actor)
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public bool TrySummon(FamiliarActor familiar, int index)
	{
		if (familiar == null || index < 0 || index >= MAX_SLOTS || familiarSlots[index] != null)
		{
			return false;
		}
		
		familiarSlots[index] = familiar;
		familiar.side = this;
		familiar.slot = index;
		
		return true;
	}
	
	public bool TrySpawn(SpawnActor spark, int index)
	{
		if (spark == null || index < 0 || index >= MAX_SLOTS || familiarSlots[index] != null)
		{
			return false;
		}
		
		familiarSlots[index] = spark;
		spark.side = this;
		spark.slot = index;
		
		return true;
	}
	
	public FamiliarActor SpawnFamiliar(SpawnActor spark)
	{
		if (spark == null || spark.side != this || spark.slot < 0 || spark.slot >= MAX_SLOTS)
		{
			return null;
		}
		
		int slot = spark.slot;
		ClearSlot(slot);
		
		FamiliarActor newActor = new(spark.familiar);
		
		if (TrySummon(newActor, slot))
		{
			return newActor;
		}
		
		return null;
	}
	
	public void ClearSlot(int index)
	{
		if (index >= 0 && index < MAX_SLOTS)
		{
			familiarSlots[index] = null;
		}
	}
	
	public int CountActiveFamiliars()
	{
		int count = 0;
		
		foreach (var slot in familiarSlots)
		{
			if (slot is FamiliarActor fam && fam.isAlive)
			{
				count++;
			}
		}
		
		return count;
	}
	
	public Godot.Collections.Array<FamiliarActor> GetFamiliarList()
	{
		Godot.Collections.Array<FamiliarActor> famList = new();
		
		foreach (var slot in familiarSlots)
		{
			if (slot != null && slot is FamiliarActor fam)
			{
				famList.Add(fam);
			}
		}
		
		return famList;
	}
	
	public FamiliarActor GetLeftFamiliar(int slot)
	{
		if (slot >= 1 && slot < MAX_SLOTS)
		{
			if (familiarSlots[slot - 1] is FamiliarActor fam)
			{
				return fam;
			}
		}
		
		return null;
	}
	
	public FamiliarActor GetRightFamiliar(int slot)
	{
		if (slot >= 0 && slot < MAX_SLOTS - 1)
		{
			if (familiarSlots[slot + 1] is FamiliarActor fam)
			{
				return fam;
			}
		}
		
		return null;
	}
}

public interface IBattleActor
{
	string name {get;}
	int maxEnergy {get;}
	int currentEnergy {get; set;}
	int speed {get;}
	BattleSide side {get; set;}
	int slot {get; set;}
	
	bool isAlive {get;}
	
	void Damage(int amount);
}

public partial class FamiliarActor : RefCounted, IBattleActor
{
	public RFamiliarInstance familiar {get; private set;}
	public string name {get; private set;}
	
	public BattleSide side {get; set;}
	public int slot {get; set;} = -1;
	
	public int currentEnergy {get; set;} = 1;
	public int maxEnergy => familiar.energy;
	
	public int pAttack => familiar.pAttack;
	public int mAttack => familiar.mAttack;
	public int pDefense => familiar.pDefense;
	public int mDefense => familiar.mDefense;
	public int speed => familiar.speed;
	
	public int pAttackBonus {get; set;} = 0;
	public int mAttackBonus {get; set;} = 0;
	public int pDefenseBonus {get; set;} = 0;
	public int mDefenseBonus {get; set;} = 0;
	public int speedBonus {get; set;} = 0;
	
	public bool isAlive => currentEnergy > 0;
	
	public float defenseFactor {get; set;} = 1f;
	
	public FamiliarActor(RFamiliarInstance fam)
	{
		familiar = fam;
		name = familiar.GetPreferredName();
		currentEnergy = maxEnergy;
	}
	
	public void Heal(int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		
		currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
	}
	
	public void Damage(int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		
		currentEnergy = Mathf.Max(currentEnergy - amount, 0);
	}
	
	public void ResetTurnModifiers()
	{
		defenseFactor = 1f;
	}
	
	public int ModPAttack()
	{
		return pAttack + pAttackBonus;
	}
	
	public int ModMAttack()
	{
		return mAttack + mAttackBonus;
	}
	
	public int ModPDefense()
	{
		return pDefense + pDefenseBonus;
	}
	
	public int ModMDefense()
	{
		return mDefense + mDefenseBonus;
	}
	
	public int ModSpeed()
	{
		return speed + speedBonus;
	}
	
	public BattleCommand GenerateCommand(BattleManager battle)
	{
		
		DefendCommand cmd = new DefendCommand {
			source = this,
			sourceSide = side
		};
		
		return cmd;
	}
}

public partial class SpawnActor : RefCounted, IBattleActor
{
	public RFamiliarInstance familiar {get; private set;}
	public string name {get; private set;}
	
	public BattleSide side {get; set;}
	public int slot {get; set;} = 0;
	
	public int maxEnergy {get; set;} = 1;
	public int currentEnergy {get; set;} = 0;
	public int speed {get; set;} = 0;
	public bool isAlive {get; set;} = false;
	
	public SpawnActor(RFamiliarInstance fam)
	{
		familiar = fam;
		name = "Manifesting Familiar";
	}
	
	public void Damage(int amount)
	{
		
	}
}

public interface IEncounterAI
{
	BattleCommand PickProjectorAction(BattleManager battle, BattleSide side);
	BattleCommand PickFamiliarAction(BattleManager battle, FamiliarActor fam);
}

public class RandomWildAI : IEncounterAI
{
	public BattleCommand PickProjectorAction(BattleManager battle, BattleSide side) => null;
	
	public BattleCommand PickFamiliarAction(BattleManager battle, FamiliarActor fam)
	{
		int roll = (int)(GD.Randi() % 5);
		
		GD.Print($"AI {fam.name} rolls {roll} to select action...");
		
		switch (roll)
		{
			case 0:
			case 1:
				return (BattleCommand)CommandFactory.AttackRandom(battle, fam)
					?? CommandFactory.Defend(fam);
			case 2:
			case 3:
				return (BattleCommand)CommandFactory.SkillRandom(battle, fam)
					?? (BattleCommand)CommandFactory.AttackRandom(battle, fam)
					?? CommandFactory.Defend(fam);
		}
		
		return CommandFactory.Defend(fam);
	}
}

public class RandomProjectorAI : IEncounterAI
{
	public BattleCommand PickProjectorAction(BattleManager battle, BattleSide side)
	{
		int roll = (int)(GD.Randi() % 10);
		
		switch (roll)
		{
			case 0:
			case 1:
			case 2:
			case 3:
				return (BattleCommand)CommandFactory.SummonRandom(battle, side)
					?? CommandFactory.Focus(side);
			case 4:
				return (BattleCommand)CommandFactory.DismissRandom(battle, side)
					?? (BattleCommand)CommandFactory.SummonRandom(battle, side)
					?? CommandFactory.Focus(side);
			case 5:
			case 6:
			case 7:
				return (BattleCommand)CommandFactory.SpellRandom(battle, side)
					?? CommandFactory.Focus(side);
		}
		
		return CommandFactory.Focus(side);
	}
	
	public BattleCommand PickFamiliarAction(BattleManager battle, FamiliarActor fam)
	{
		int roll = (int)(GD.Randi() % 5);
		
		GD.Print($"AI {fam.name} rolls {roll} to select action...");
		
		switch (roll)
		{
			case 0:
			case 1:
				return (BattleCommand)CommandFactory.AttackRandom(battle, fam)
					?? CommandFactory.Defend(fam);
			case 2:
			case 3:
				return (BattleCommand)CommandFactory.SkillRandom(battle, fam)
					?? (BattleCommand)CommandFactory.AttackRandom(battle, fam)
					?? CommandFactory.Defend(fam);
		}
		
		return CommandFactory.Defend(fam);
	}
}

public class CommandFactory
{
	public static SummonCommand SummonRandom(BattleManager battle, BattleSide side)
	{
		if (side == null)
		{
			return null;
		}
		
		if (!side.HasOpenSlot())
		{
			return null;
		}
		
		Projector projector = side.projector;
		
		if (projector == null)
		{
			return null;
		}
		
		Godot.Collections.Array<RFamiliarInstance> summonable = new();
		Godot.Collections.Array<RFamiliarInstance> alreadyOut = new();
		
		foreach (var actor in side.GetFamiliarList())
		{
			alreadyOut.Add(actor.familiar);
		}
		
		foreach (var fam in projector.ownedFamiliars)
		{
			bool summoned = false;
			
			foreach (var fam2 in alreadyOut)
			{
				if (ReferenceEquals(fam, fam2))
				{
					summoned = true;
					break;
				}
			}
			
			if (!summoned && projector.currentEnergy >= fam.energy)
			{
				summonable.Add(fam);
			}
		}
		
		if (summonable.Count == 0)
		{
			return null;
		}
		
		int roll = (int)(GD.Randi() % summonable.Count);
		
		SummonCommand cmd = new SummonCommand
		{
			sourceSide = side,
			source = projector,
			familiar = summonable[roll],
			slot = side.GetPreferredOpenSlot()
		};
		
		return cmd;
	}
	
	public static DismissCommand DismissRandom(BattleManager battle, BattleSide side)
	{
		if (side == null)
		{
			return null;
		}
		
		if (side.CountActiveFamiliars() == 0)
		{
			return null;
		}
		
		Projector projector = side.projector;
		
		if (projector == null)
		{
			return null;
		}
		
		Godot.Collections.Array<FamiliarActor> familiars = side.GetFamiliarList();
		
		int roll = (int)(GD.Randi() % familiars.Count);
		
		DismissCommand cmd = new DismissCommand
		{
			sourceSide = side,
			source = projector,
			target = familiars[roll]
		};
		
		return cmd;
	}
	
	public static SpellCommand SpellRandom(BattleManager battle, BattleSide side)
	{
		if (side == null)
		{
			return null;
		}
		
		BattleSide enemySide = side == battle.playerSide ? battle.enemySide : battle.playerSide;
		
		Projector projector = side.projector;
		
		if (projector == null)
		{
			return null;
		}
		
		if (projector.spells.Count == 0)
		{
			return null;
		}
		
		Godot.Collections.Array<RSpellData> castable = new();
		
		foreach (var spell in projector.spells)
		{
			if (projector.currentEnergy >= spell.cost)
			{
				bool ok = spell.spellPattern switch
				{
					RSpellData.SpellPattern.OneAlly => side.CountActiveFamiliars() > 0,
					RSpellData.SpellPattern.OneEnemy => enemySide.CountActiveFamiliars() > 0 || (enemySide.projector != null && enemySide.projector.currentEnergy > 0),
					_ => true
				};
				
				if (ok)
				{
					castable.Add(spell);
				}
			}
		}
		
		if (castable.Count == 0)
		{
			return null;
		}
		
		int roll = (int)(GD.Randi() % castable.Count);
		
		RSpellData selectedSpell = castable[roll];
		
		SpellCommand cmd = new SpellCommand
		{
			sourceSide = side,
			source = projector
		};
		
		cmd.AssignSpell(selectedSpell);
		
		if (selectedSpell.spellPattern == RSpellData.SpellPattern.OneAlly)
		{
			Godot.Collections.Array<FamiliarActor> allies = side.GetFamiliarList();
			
			roll = (int)(GD.Randi() % allies.Count);
			
			cmd.target = allies[roll];
		}
		else if (selectedSpell.spellPattern == RSpellData.SpellPattern.OneEnemy)
		{
			if (enemySide.CountActiveFamiliars() > 0)
			{
				Godot.Collections.Array<FamiliarActor> enemies = enemySide.GetFamiliarList();
				
				roll = (int)(GD.Randi() % enemies.Count);
				
				cmd.target = enemies[roll];
			}
			else
			{
				cmd.target = enemySide.projector;
			}
		}
		
		return cmd;
	}
	
	public static FocusCommand Focus(BattleSide side)
	{
		if (side == null)
		{
			return null;
		}
		
		Projector projector = side.projector;
		
		if (projector == null)
		{
			return null;
		}
		
		return new FocusCommand
		{
			sourceSide = side,
			source = projector
		};
	}
	
	public static DefendCommand Defend(FamiliarActor fam)
	{
		return new DefendCommand {
			sourceSide = fam.side,
			source = fam
		};
	}
	
	//public static AttackCommand Attack(FamiliarActor fam, object target)
	
	public static AttackCommand AttackRandom(BattleManager battle, FamiliarActor fam)
	{
		GD.Print($"CommandFactory: {fam.name} attempts to attack");
		
		if (battle == null || fam == null)
		{
			GD.Print("  invalid due to null battle or familiar");
			return null;
		}
		
		BattleSide otherSide = fam.side == battle.playerSide ? battle.enemySide : battle.playerSide;
		
		object randTarget = PickHostileTarget(otherSide);
		
		if (randTarget == null)
		{
			GD.Print("  invalud due to null target");
			return null;
		}
		
		return new AttackCommand {
			sourceSide = fam.side,
			source = fam,
			target = randTarget,
			power = 5
		};
	}
	
	public static SkillCommand SkillRandom(BattleManager battle, FamiliarActor fam)
	{
		GD.Print($"CommandFactory: {fam.name} attempts to pick a skill");
		
		if (battle == null || fam?.familiar?.skills == null)
		{
			GD.Print($"  invalid due to null battle, familiar, or skills");
			GD.Print($"  battle null={battle == null}, familiar null={fam?.familiar == null}, skills null={fam?.familiar?.skills == null}");
			return null;
		}
		
		BattleSide otherSide = fam.side == battle.playerSide ? battle.enemySide : battle.playerSide;
		
		Godot.Collections.Array<RSkillData> validSkills = new();
		
		foreach (var skl in fam.familiar.skills)
		{
			if (skl == null || fam.currentEnergy < skl.cost)
			{
				continue;
			}
			
			bool ok = skl.targetPattern switch
			{
				RSkillData.TargetPattern.OneAlly =>
					fam.side.CountActiveFamiliars() > 0,
				RSkillData.TargetPattern.OneEnemy =>
					otherSide.CountActiveFamiliars() > 0 || (otherSide.projector != null && otherSide.projector.currentEnergy > 0),
				_ => true
			};
			
			if (ok)
			{
				validSkills.Add(skl);
			}
		}
		
		if (validSkills.Count == 0)
		{
			GD.Print("  no valid skills");
			return null;
		}
		
		RSkillData skill = validSkills[(int)(GD.Randi() % validSkills.Count)];
		
		SkillCommand cmd = new SkillCommand
		{
			sourceSide = fam.side,
			source = fam
		};
		
		cmd.target = skill.targetPattern switch
		{
			RSkillData.TargetPattern.Self or RSkillData.TargetPattern.None => fam,
			RSkillData.TargetPattern.OneAlly => PickFriendlyTarget(fam.side),
			RSkillData.TargetPattern.OneEnemy => PickHostileTarget(otherSide),
			_ => null
		};
		
		if (skill.targetPattern is RSkillData.TargetPattern.OneAlly or RSkillData.TargetPattern.OneEnemy && cmd.target == null)
		{
			GD.Print("  single target with null target");
			return null;
		}
		
		cmd.AssignSkill(skill);
		
		return cmd;
	}
	
	//public static SummonCommand Summon(Projector proj, BattleSide side, RFamiliarInstance fam, int slot)
	
	//public static FocusCommand Focus(Projector proj)
	
	public static object PickFriendlyTarget(BattleSide side)
	{
		if (side == null)
		{
			return null;
		}
		
		Godot.Collections.Array<FamiliarActor> famList = side.GetFamiliarList();
		
		if (famList != null && famList.Count > 0)
		{
			int randIdx = (int)(GD.Randi() % famList.Count);
			return famList[randIdx];
		}
		
		return null;
	}
	
	public static object PickHostileTarget(BattleSide side)
	{
		if (side == null)
		{
			return null;
		}
		
		Godot.Collections.Array<FamiliarActor> famList = side.GetFamiliarList();
		
		if (famList != null && famList.Count > 0)
		{
			int randIdx = (int)(GD.Randi() % famList.Count);
			return famList[randIdx];
		}
		
		if (side.projector != null && side.projector.currentEnergy > 0)
		{
			return side.projector;
		}
		
		return null;
	}
}
