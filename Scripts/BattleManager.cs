using Godot;
using System;
using System.Linq;

public partial class BattleManager : Node
{
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
	public FamiliarActor pendingSource;
	
	public bool projCommandSubmitted;
	public int famCommandsSubmitted;
	
	public enum BattleState
	{
		Setup,
		CommandSelect,
		Resolution,
		SpawnCheck,
		EndCheck,
		Cleanup
	}
	
	public enum CommandState
	{
		None,
		SelectSummon,
		SelectDismiss,
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
		Draw
	}
	
	public BattleSide playerSide;
	public BattleSide enemySide;
	
	public Godot.Collections.Array<RFamiliarInstance> spawns;
	
	public Godot.Collections.Array<BattleCommand> projectorCommands;
	public Godot.Collections.Array<BattleCommand> familiarCommands;
	public Godot.Collections.Array<BattleCommand> turnCommands;
	
	public Godot.Collections.Array<RFamiliarInstance> defeatedFamiliars;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print($"BattleManager: _Ready on {Name}, path={GetPath()}, id={GetInstanceId()}");
		
		backgroundRect = GetNode<TextureRect>("BackgroundRect");
		
		projectorDisplayE = GetNode<ProjectorDisplay>("ProjectorDisplayEnemy");
		projectorDisplayP = GetNode<ProjectorDisplay>("ProjectorDisplayPlayer");
		
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
		
		StartTest();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void StartTest()
	{
		GD.Print("BattleManager - Starting test...");
		
		RProjectorData pData = GD.Load<RProjectorData>("res://Resources/test_projector.tres");
		RFamiliarInstance gnomeInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_gnome.tres");
		RFamiliarInstance salamanderInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_salamander.tres");
		RFamiliarInstance sylphInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_sylph.tres");
		RFamiliarInstance undineInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_undine.tres");
		
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
		
		REncounterData eData = GD.Load<REncounterData>("res://Resources/test_encounter.tres");
		
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
		}
		
		RefreshAllDisplays();
		BeginCommandSelect();
	}
	
	public void OnNextPressed()
	{
		AppendBattleText("Next button pressed.");
		GD.Print("Next button pressed.");
		
		if (comState != CommandState.None)
		{
			AppendBattleText("Cancel pending command");
			selectionPanel.HidePanel();
			pendingCommand = null;
			pendingSource = null;
			SetCommandState(CommandState.None);
			ClearHighlights();
			RefreshNextButton();
		}
		else
		{
			AppendBattleText("Resolving turn", true);
			try
			{
				projCommandSubmitted = false;
				famCommandsSubmitted = 0;
				SetBattleState(BattleState.Resolution);
				SetCommandState(CommandState.None);
				
				RefreshNextButton();
				
				BuildTurnOrder();
				ResolveTurn();
			}
			catch (Exception e)
			{
				GD.PrintErr($"BattleManager: Error resolving turn {e}");
				AppendBattleText("Error resolving turn");
			}
		}
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
			//CommandState.SelectAllySpell
			//CommandState.SelectAllySkill
			//CommandState.SelectAllyItem
			//CommandState.SelectEnemyAttack
			//CommandState.SelectEnemySpell
			//CommandState.SelectEnemySkill
			//CommandState.SelectEnemyItem
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
			
			SetCommandState(CommandState.None);
			ClearTargetMode();
			projCommandPanel.DisableCommands();
			projCommandSubmitted = true;
			projCommandDisabled = true;
			//projCommandPanel.undoButton.Visible = true;
			
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
		
		SetCommandState(CommandState.None);
		ClearTargetMode();
		projCommandPanel.DisableCommands();
		projCommandSubmitted = true;
		projCommandDisabled = true;
	}
	
	public void ClearTargetMode()
	{
		GD.Print("BattleManager: clearing targets");
		
		comState = CommandState.None;
		pendingCommand = null;
		pendingSource = null;
		ClearHighlights();
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
			if (!enemySide.IsSlotEmpty(slot))
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
				}
				catch (Exception e)
				{
					GD.PrintErr($"BattleManager: failed to execute command - {e}");
				}
			}
		}
	
		BeginCommandSelect();
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
			enemyDefeat = enemySide.CountActiveFamiliars() == 0 && (spawns == null || spawns.Count == 0);
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
	
	public void SpawnCheck()
	{
		if (isProjectorEncounter)
		{
			SetBattleState(BattleState.EndCheck);
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
			/*int slot = -1;
			
			for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
			{
				if (enemySide.IsSlotEmpty(i))
				{
					slot = i;
					break;
				}
			}*/
			
			int slot = enemySide.GetPreferredOpenSlot();
			
			if (slot >= 0)
			{
				RFamiliarInstance fam = spawns[0];
				
				SpawnActor spark = new(fam);
				
				if (enemySide.TrySpawn(spark, slot))
				{
					spawns.RemoveAt(0);
					famDisplaysE[slot].AssignSpawn(spark);
				}
				else if (spark == null)
				{
					spawns.RemoveAt(0);
				}
			}
		}
		
		SetBattleState(BattleState.EndCheck);
	}
	
	public void EndCheck()
	{
		VictoryResult result = CheckVictory();
		
		switch (result)
		{
			case VictoryResult.PlayerWin:
				SetBattleState(BattleState.Cleanup);
				//victory results
				break;
			case VictoryResult.PlayerLose:
			case VictoryResult.Draw:
				SetBattleState(BattleState.Cleanup);
				//game over results
				break;
			case VictoryResult.None:
				SetBattleState(BattleState.CommandSelect);
				projCommandPanel.EnableCommands();
				
				foreach (var panel in famCommandPanels)
				{
					panel.EnableCommands();
				}
				
				break;
		}
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
	
	public void Damage(int amount)
	{
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
