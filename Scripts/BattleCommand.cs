using Godot;
using System;

public abstract partial class BattleCommand : RefCounted
{
	public BattleSide sourceSide {get; set;}
	public object source {get; set;}
	public object target {get; set;}
	
	public float speedFactor {get; set;} = 1f;
	
	public bool isValid {get; set;} = true;
	
	public abstract void Execute(BattleManager battle);
	
	public virtual void Retarget(BattleManager battle)
	{
		
	}
}

public partial class SummonCommand : BattleCommand
{
	public RFamiliarInstance familiar {get; set;}
	public int slot {get; set;} = -1;
	
	public override void Execute(BattleManager battle)
	{
		string fName = string.IsNullOrEmpty(familiar?.nickName) ? (string.IsNullOrEmpty(familiar?.data?.name) ? "No name" : familiar.data.name) : familiar.nickName;
		battle.AppendBattleText($"Summoning {fName} into slot {slot}.");
		
		if (source is not Projector projector)
		{
			battle.AppendBattleText("Projector does not exist");
			return;
		}
		
		if (!sourceSide.HasOpenSlot())
		{
			battle.AppendBattleText("No slots available");
			return;
		}
		
		if (slot < 0 || slot >= BattleSide.MAX_SLOTS || !sourceSide.IsSlotEmpty(slot))
		{
			/*slot = -1;
			
			for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
			{
				if (sourceSide.IsSlotEmpty(i))
				{
					slot = i;
					break;
				}
			}*/
			
			if (slot == -1)
			{
				battle.AppendBattleText("No slots available");
				return;
			}
		}
		
		int cost = familiar.energy;
		
		GD.Print($"SummonCommand: {familiar.GetPreferredName()} costs {cost} energy");
		
		if (projector.currentEnergy < cost)
		{
			//string fName = string.IsNullOrEmpty(familiar.nickName) ? (string.IsNullOrEmpty(familiar.data.name) ? "(no name)" : familiar.data.name) : familiar.nickName;
			string text = $"Not enough energy to summon {fName}";
			battle.AppendBattleText(text);
			return;
		}
		
		FamiliarActor actor = new(familiar);
		
		string pName = string.IsNullOrEmpty(projector.name) ? "(no name)" : projector.name;
		string aName = string.IsNullOrEmpty(actor.name) ? "(no name)" : actor.name;
		
		if (sourceSide.TrySummon(actor, slot))
		{
			projector.currentEnergy -= cost;
			GD.Print($"SummonCommand: {projector.name}'s Energy reduced by {cost} to {projector.currentEnergy}");
			
			FamiliarDisplay[] displays = battle.GetFamiliarDisplays(sourceSide);
			displays[slot].AssignFamiliar(actor);
			
			if (!battle.summonedFamiliars.Contains(familiar))
			{
				battle.summonedFamiliars.Add(familiar);
			}
			
			string text = $"[b]{pName}[/b] summons [b]{aName}[/b]";
			battle.AppendBattleText(text);
			battle.RefreshAllDisplays();
		}
		else
		{
			battle.AppendBattleText("Failed to summon");
			GD.Print("SummonCommand: Failed to summon");
		}
		
		GD.Print($"Execute {GetType().Name} source=projector {(source as Projector).name}");
	}
}

public partial class DismissCommand : BattleCommand
{
	public override void Execute(BattleManager battle)
	{
		if (source is not Projector proj)
		{
			return;
		}
		
		if (target is not FamiliarActor actor)
		{
			return;
		}
		
		if (actor.side != sourceSide)
		{
			return;
		}
		
		int slot = actor.slot;
		int refund = actor.currentEnergy;
		
		proj.currentEnergy = Math.Min(proj.currentEnergy + refund, proj.maxEnergy);
		
		actor.side.ClearSlot(slot);
		
		battle.InvalidateFamiliarCommands(actor, this);
		
		FamiliarDisplay[] displays = battle.GetFamiliarDisplays(actor.side);
		
		if (slot >= 0 && slot < displays.Length)
		{
			displays[slot].Clear();
		}
		
		string aName = string.IsNullOrEmpty(actor.name) ? "(No name)" : actor.name;
		string pName = string.IsNullOrEmpty(proj.name) ? "(No name)" : proj.name;
		
		battle.AppendBattleText($"[b]{pName}[/b] dismisses [b]{aName}[/b] and recovers [b]{refund}[/b] energy.");
		
		battle.RefreshAllDisplays();
		
		GD.Print($"Execute {GetType().Name} source=projector {(source as Projector).name}");
	}
}

public partial class AttackCommand : BattleCommand
{
	public int power {get; set;} = 0;
	
	public bool isMagicAttack {get; set;} = false;
	public bool isMagicTarget {get; set;} = false;
	
	public override void Execute(BattleManager battle)
	{
		if (source is not FamiliarActor srcFam || !srcFam.isAlive)
		{
			isValid = false;
			return;
		}
		
		if (!IsUsableTarget(target))
		{
			target = null;
			Retarget(battle);
			
			if (!IsUsableTarget(target))
			{
				battle.AppendBattleText("No target available");
				return;
			}
		}
		
		int attackStat = GetAttackStat(srcFam);
		int defenseStat = GetDefenseStat(target);
		
		float defFactor = 1f;
		
		string fName = string.IsNullOrEmpty(srcFam.name) ? "(no name)" : srcFam.name;
		string fEnemyPrefix = sourceSide == battle.enemySide ? "Enemy " : "";
		
		string tName = "(no name)";
		string tEnemyPrefix = "";
		
		if (target is FamiliarActor tFam)
		{
			defFactor = tFam.defenseFactor;
			tName = string.IsNullOrEmpty(tFam.name) ? "(no name)" : tFam.name;
			tEnemyPrefix = tFam.side == battle.enemySide ? "Enemy " : "";
		}
		else if (target is Projector tProj)
		{
			tName = string.IsNullOrEmpty(tProj.name) ? "(no name)" : tProj.name;
		}
		
		float raw = (float)(attackStat * power) / Mathf.Max(1, defenseStat);
		int damage = Mathf.Max(1, Mathf.RoundToInt(raw / defFactor));
		
		ApplyDamage(target, damage);
		
		string text = $"{fEnemyPrefix}[b]{fName}[/b] deals {damage} damage to {tEnemyPrefix}[b]{tName}[/b]";
		battle.AppendBattleText(text);
		
		if (target is FamiliarActor fam2)
		{
			BattleSide enemySide = fam2.side;
			FamiliarDisplay[] displays = battle.GetFamiliarDisplays(enemySide);
			
			int slot = enemySide.GetSlotIndex(fam2);
			
			if (slot >= 0 && slot < BattleSide.MAX_SLOTS)
			{
				displays[slot].UpdateDisplay();
			}
			
			if (!fam2.isAlive)
			{
				if (slot != -1)
				{
					enemySide.ClearSlot(slot);
					
					FamiliarDisplay[] famDisplays = enemySide == battle.playerSide ? battle.famDisplaysP : battle.famDisplaysE;
					famDisplays[slot].Clear();
					
					string fEnemyPrefix2 = enemySide == battle.enemySide ? "Enemy " : "";
					text = $"{fEnemyPrefix2}[b]{fam2.name}[/b] was eliminated";
					battle.AppendBattleText(text, false);
					
					if (!battle.isProjectorEncounter && fam2.side == battle.enemySide)
					{
						battle.defeatedFamiliars.Add(fam2.familiar);
					}
				}
			}
		}
		else if (target is Projector proj)
		{
			ProjectorDisplay display = sourceSide == battle.playerSide ? battle.projectorDisplayE : battle.projectorDisplayP;
			display.UpdateDisplay();
			
			if (proj.currentEnergy <= 0)
			{
				text = $"[b]{proj.name}'s[/b] Energy was reduced to 0";
				battle.AppendBattleText(text, false);
			}
		}
		
		battle.RefreshAllDisplays();
		
		GD.Print($"Execute {GetType().Name} source={(source as FamiliarActor)?.slot} {(source as FamiliarActor).name}");
	}
	
	public override void Retarget(BattleManager battle)
	{
		GD.Print("AttackCommand: Retargeting attack");
		
		BattleSide enemySide = sourceSide == battle.playerSide ? battle.enemySide : battle.playerSide;
		
		if (enemySide.CountActiveFamiliars() > 0)
		{
			Godot.Collections.Array<FamiliarActor> famList = enemySide.GetFamiliarList();
			
			foreach (var fam in famList)
			{
				GD.Print($"   {fam.name}");
			}
			
			int idx = (int)(GD.Randi() % famList.Count);
			
			target = famList[idx];
			
			GD.Print($"AttackCommand: new target {((FamiliarActor)target).name}");
		}
		else if (enemySide.projector != null && enemySide.projector.currentEnergy > 0)
		{
			target = enemySide.projector;
		}
		else
		{
			isValid = false;
		}
	}
	
	public bool IsUsableTarget(object target)
	{
		if (target is FamiliarActor fam)
		{
			return fam.isAlive && fam.side != null && fam.side.GetSlotIndex(fam) >= 0;
		}
		if (target is Projector proj)
		{
			return proj.currentEnergy > 0;
		}
		
		return false;
	}
	
	public int GetAttackStat(object source)
	{
		if (source is FamiliarActor fam)
		{
			return isMagicAttack ? fam.ModMAttack() : fam.ModPAttack();
		}
		
		return 10;
	}
	
	public int GetDefenseStat(object target)
	{
		if (target is FamiliarActor fam)
		{
			return isMagicTarget ? fam.ModMDefense() : fam.ModPDefense();
		}
		
		return 10;
	}
	
	public void ApplyDamage(object target, int amount)
	{
		if (target is IBattleActor actor)
		{
			actor.Damage(amount);
		}
		else if (target is Projector proj)
		{
			proj.Damage(amount);
		}
	}
}

public partial class DefendCommand : BattleCommand
{
	public DefendCommand()
	{
		speedFactor = 2f;
	}
	
	public override void Execute(BattleManager battle)
	{
		if (source is not FamiliarActor actor)
		{
			return;
		}
		
		string name = string.IsNullOrEmpty(actor.familiar?.GetPreferredName()) ? "No Name" : actor.familiar.GetPreferredName();
		string enemyPrefix = sourceSide == battle.enemySide ? "Enemy " : "";
		
		battle.AppendBattleText($"{enemyPrefix}[b]{name}[/b] defends.");
		
		actor.defenseFactor = Math.Max(2, actor.defenseFactor);
		
		GD.Print($"Execute {GetType().Name} source={(source as FamiliarActor)?.slot} {(source as FamiliarActor).name}");
	}
}

public partial class FocusCommand : BattleCommand
{
	public override void Execute(BattleManager battle)
	{
		if (source is not Projector proj)
		{
			battle.AppendBattleText("Focus command failed: source not projector");
			return;
		}
		
		int amount = (int)(GD.Randi() % 10) + 1;
		
		battle.AppendBattleText($"[b]{proj.name}[/b] focuses and recovers [b]{amount}[/b] energy.");
		
		if (source is Projector projector)
		{
			projector.currentEnergy = Mathf.Min(projector.maxEnergy, projector.currentEnergy + amount);
		}
		
		battle.RefreshAllDisplays();
		
		GD.Print($"Execute {GetType().Name} source=projector {(source as Projector).name}");
	}
}

public partial class SkillCommand : BattleCommand
{
	public RSkillData skill {get; set;}
	
	public RSkillData.TargetPattern targetPattern {get; set;}
	
	public int cost {get; set;} = 0;
	
	public int power {get; set;} = 0;
	public float splash {get; set;} = 0f;
	public bool overwhelming {get; set;} = false;
	
	public int heal {get; set;} = 0;
	
	public float defenseSelf {get; set;} = 1f;
	public float defenseTarget {get; set;} = 1f;
	
	public bool isMagicAttack {get; set;} = false;
	public bool isMagicTarget {get; set;} = false;
	
	public override void Execute(BattleManager battle)
	{
		if (source is not FamiliarActor srcFam || !srcFam.isAlive)
		{
			isValid = false;
			return;
		}
		
		if (!OpenTarget() && !IsUsableTarget(target))
		{
			target = null;
			Retarget(battle);
			
			if (!IsUsableTarget(target))
			{
				battle.AppendBattleText("No target available");
				return;
			}
		}
		
		if (cost > 0 && srcFam.currentEnergy < cost)
		{
			battle.AppendBattleText($"{srcFam.name} doesn't have enough energy.");
			return;
		}
		
		srcFam.currentEnergy -= cost;
		
		if (SingleTarget())
		{
			if (power > 0)
			{
				AttackTarget(srcFam, target, battle);
			}
			
			if (heal > 0)
			{
				HealTarget(srcFam, target, battle);
			}
			
			if (defenseSelf != 1f)
			{
				DefendTarget(srcFam, defenseSelf, battle);
			}
			
			if (defenseTarget != 1f)
			{
				DefendTarget(target, defenseTarget, battle);
			}
		}
		else if (MultiTarget())
		{
			Godot.Collections.Array<RefCounted> targets = new();
			
			if (targetPattern == RSkillData.TargetPattern.AllAllies || targetPattern == RSkillData.TargetPattern.AllUnits)
			{
				foreach (var fam in sourceSide.GetFamiliarList())
				{
					targets.Add(fam);
				}
			}
			
			if (targetPattern == RSkillData.TargetPattern.AllEnemies || targetPattern == RSkillData.TargetPattern.AllUnits)
			{
				BattleSide enemySide = sourceSide == battle.playerSide ? battle.enemySide : battle.playerSide;
				
				foreach (var fam in enemySide.GetFamiliarList())
				{
					targets.Add(fam);
				}
				
				if (enemySide.CountActiveFamiliars() == 0 && enemySide.projector != null && enemySide.projector.currentEnergy > 0)
				{
					targets.Add(enemySide.projector);
				}
			}
			
			if (power > 0)
			{
				foreach (var t in targets)
				{
					AttackTarget(srcFam, t, battle);
				}
			}
			
			if (heal > 0)
			{
				foreach (var t in targets)
				{
					HealTarget(srcFam, t, battle);
				}
			}
			
			if (defenseSelf != 1f || defenseTarget != 1f)
			{
				foreach (var t in targets)
				{
					if (ReferenceEquals(t, srcFam))
					{
						DefendTarget(t, defenseSelf, battle);
					}
					else
					{
						DefendTarget(t, defenseTarget, battle);
					}
				}
			}
		}
		
		battle.RefreshAllDisplays();
		
		GD.Print($"Execute {GetType().Name} source={(source as FamiliarActor)?.slot} {(source as FamiliarActor).name}");
	}
	
	public override void Retarget(BattleManager battle)
	{
		if (OpenTarget())
		{
			return;
		}
		
		if (targetPattern == RSkillData.TargetPattern.OneAlly)
		{
			if (sourceSide.CountActiveFamiliars() > 0)
			{
				Godot.Collections.Array<FamiliarActor> famList = sourceSide.GetFamiliarList();
				
				int idx = (int)(GD.Randi() % famList.Count);
				
				target = famList[idx];
			}
			else
			{
				isValid = false;
			}
		}
		else if (targetPattern == RSkillData.TargetPattern.OneEnemy)
		{
			BattleSide enemySide = sourceSide == battle.playerSide ? battle.enemySide : battle.playerSide;
			
			if (enemySide.CountActiveFamiliars() > 0)
			{
				Godot.Collections.Array<FamiliarActor> famList = enemySide.GetFamiliarList();
				
				int idx = (int)(GD.Randi() % famList.Count);
				
				target = famList[idx];
			}
			else if (enemySide.projector != null && enemySide.projector.currentEnergy > 0)
			{
				target = enemySide.projector;
			}
			else
			{
				isValid = false;
			}
		}
	}
	
	public bool IsUsableTarget(object target)
	{
		if (target is FamiliarActor fam)
		{
			return fam.isAlive && fam.side != null && fam.side.GetSlotIndex(fam) >= 0;
		}
		if (target is Projector proj)
		{
			return proj.currentEnergy > 0;
		}
		
		return false;
	}
	
	public int GetAttackStat(object source)
	{
		if (source is FamiliarActor fam)
		{
			return isMagicAttack ? fam.ModMAttack() : fam.ModPAttack();
		}
		
		return 10;
	}
	
	public int GetDefenseStat(object target)
	{
		if (target is FamiliarActor fam)
		{
			return isMagicTarget ? fam.ModMDefense() : fam.ModPDefense();
		}
		
		return 10;
	}
	
	public void HealTarget(FamiliarActor srcFam, object target, BattleManager battle)
	{
		int healStat = GetAttackStat(srcFam);
		
		string fName = string.IsNullOrEmpty(srcFam.name) ? "(no name)" : srcFam.name;
		string fEnemyPrefix = sourceSide == battle.enemySide? "Enemy " : "";
		
		string tName = "(no name)";
		string tEnemyPrefix = "";
		
		if (target is FamiliarActor tFam)
		{
			tName = string.IsNullOrEmpty(tFam.name) ? "(no name)" : tFam.name;
			tEnemyPrefix = tFam.side == battle.enemySide ? "Enemy " : "";
		}
		else if (target is Projector tProj)
		{
			tName = string.IsNullOrEmpty(tProj.name) ? "(no name)" : tProj.name;
		}
		
		int healing = healStat * heal;
		
		ApplyHealing(target, healing);
		
		string text = $"{fEnemyPrefix}[b]{fName}[/b] heals {healing} energy for {tEnemyPrefix}[b]{tName}[/b]";
		battle.AppendBattleText(text);
		
		if (target is FamiliarActor fam2)
		{
			BattleSide targetSide = fam2.side;
			FamiliarDisplay[] displays = battle.GetFamiliarDisplays(targetSide);
			
			int slot = targetSide.GetSlotIndex(fam2);
			
			if (slot >= 0 && slot < BattleSide.MAX_SLOTS)
			{
				displays[slot].UpdateDisplay();
			}
			
			if (SingleTarget() && splash > 0f)
			{
				FamiliarActor famLeft = targetSide.GetLeftFamiliar(slot);
				
				if (famLeft != null)
				{
					healing = (int)(healStat * heal * splash);
					
					ApplyHealing(famLeft, healing);
					
					tName = string.IsNullOrEmpty(famLeft.name) ? "(no name)" : famLeft.name;
					
					text = $"Healing's splash heals {healing} energy for [b]{tName}[/b]";
					battle.AppendBattleText(text, false);
				}
				
				FamiliarActor famRight = targetSide.GetRightFamiliar(slot);
				
				if (famRight != null)
				{
					healing = (int)(healStat * heal * splash);
					
					ApplyHealing(famRight, healing);
					
					tName = string.IsNullOrEmpty(famRight.name) ? "(no name)" : famRight.name;
					
					text = $"Healing's splash heals {healing} energy for [b]{tName}[/b]";
					battle.AppendBattleText(text, false);
				}
			}
		}
	}
	
	public void AttackTarget(FamiliarActor srcFam, object target, BattleManager battle)
	{
		int attackStat = GetAttackStat(srcFam);
		int defenseStat = GetDefenseStat(target);
		
		float defFactor = 1f;
		
		string fName = string.IsNullOrEmpty(srcFam.name) ? "(no name)" : srcFam.name;
		string fEnemyPrefix = sourceSide == battle.enemySide ? "Enemy " : "";
		
		string tName = "(no name)";
		string tEnemyPrefix = "";
		
		if (target is FamiliarActor tFam)
		{
			defFactor = tFam.defenseFactor;
			tName = string.IsNullOrEmpty(tFam.name) ? "(no name)" : tFam.name;
			tEnemyPrefix = tFam.side == battle.enemySide ? "Enemy " : "";
		}
		else if (target is Projector tProj)
		{
			tName = string.IsNullOrEmpty(tProj.name) ? "(no name)" : tProj.name;
		}
		
		float raw = (float)(attackStat * power) / Mathf.Max(defenseStat, 1);
		int damage = Mathf.Max(1, Mathf.RoundToInt(raw / defFactor));
		
		ApplyDamage(target, damage, overwhelming, battle);
		
		string text = $"{fEnemyPrefix}[b]{fName}[/b] deals {damage} damage to {tEnemyPrefix}[b]{tName}[/b].";
		battle.AppendBattleText(text);
		
		if (target is FamiliarActor fam2)
		{
			BattleSide targetSide = fam2.side;
			FamiliarDisplay[] displays = battle.GetFamiliarDisplays(targetSide);
			
			int slot = targetSide.GetSlotIndex(fam2);
			
			if (slot >= 0 && slot < BattleSide.MAX_SLOTS)
			{
				displays[slot].UpdateDisplay();
			}
			
			if (!fam2.isAlive)
			{
				if (slot != -1)
				{
					targetSide.ClearSlot(slot);
					battle.InvalidateFamiliarCommands(fam2, this);
					
					FamiliarDisplay[] famDisplays = targetSide == battle.playerSide ? battle.famDisplaysP : battle.famDisplaysE;
					famDisplays[slot].Clear();
					
					string fEnemyPrefix2 = targetSide == battle.enemySide ? "Enemy " : "";
					text = $"{fEnemyPrefix2}[b]{fam2.name}[/b] was eliminated";
					battle.AppendBattleText(text, false);
					
					if (!battle.isProjectorEncounter && fam2.side == battle.enemySide)
					{
						battle.defeatedFamiliars.Add(fam2.familiar);
					}
				}
			}
			
			if (SingleTarget() && splash > 0f)
			{
				FamiliarActor famLeft = fam2.side.GetLeftFamiliar(slot);
				
				if (famLeft != null)
				{
					int defenseStatLeft = GetDefenseStat(famLeft);
					defFactor = famLeft.defenseFactor;
					
					raw = (float)(attackStat * power * splash) / Mathf.Max(defenseStatLeft, 1);
					damage = Mathf.Max(1, Mathf.RoundToInt(raw / defFactor));
					
					ApplyDamage(famLeft, damage, false, battle);
					
					tName = string.IsNullOrEmpty(famLeft.name) ? "(no name)" : famLeft.name;
					
					text = $"Attack's splash deals {damage} damage to {tEnemyPrefix}[b]{tName}[/b].";
					battle.AppendBattleText(text, false);
					
					int slotL = slot - 1;
					
					if (slotL >= 0 && slotL < BattleSide.MAX_SLOTS)
					{
						displays[slotL].UpdateDisplay();
					}
					
					if (!famLeft.isAlive)
					{
						if (slotL != -1)
						{
							targetSide.ClearSlot(slotL);
							battle.InvalidateFamiliarCommands(famLeft, this);
							
							FamiliarDisplay[] famDisplays = targetSide == battle.playerSide ? battle.famDisplaysP : battle.famDisplaysE;
							famDisplays[slotL].Clear();
							
							string fEnemyPrefix2 = targetSide == battle.enemySide ? "Enemy " : "";
							text = $"{fEnemyPrefix2}[b]{famLeft.name}[/b] was eliminated";
							battle.AppendBattleText(text, false);
							
							if (!battle.isProjectorEncounter && famLeft.side == battle.enemySide)
							{
								battle.defeatedFamiliars.Add(famLeft.familiar);
							}
						}
					}
				}
				
				FamiliarActor famRight = fam2.side.GetRightFamiliar(slot);
				
				if (famRight != null)
				{
					int defenseStatRight = GetDefenseStat(famRight);
					defFactor = famRight.defenseFactor;
					
					raw = (float)(attackStat * power * splash) / Mathf.Max(defenseStatRight, 1);
					damage = Mathf.Max(1, Mathf.RoundToInt(raw / defFactor));
					
					ApplyDamage(famRight, damage, false, battle);
					
					tName = string.IsNullOrEmpty(famRight.name) ? "(no name)" : famRight.name;
					
					text = $"Attack's splash deals {damage} damage to {tEnemyPrefix}[b]{tName}[/b].";
					battle.AppendBattleText(text, false);
					
					int slotR = slot + 1;
					
					if (slotR >= 0 && slotR < BattleSide.MAX_SLOTS)
					{
						displays[slotR].UpdateDisplay();
					}
					
					if (!famRight.isAlive)
					{
						if (slotR != -1)
						{
							targetSide.ClearSlot(slotR);
							battle.InvalidateFamiliarCommands(famRight, this);
							
							FamiliarDisplay[] famDisplays = targetSide == battle.playerSide ? battle.famDisplaysP : battle.famDisplaysE;
							famDisplays[slotR].Clear();
							
							string fEnemyPrefix2 = targetSide == battle.enemySide ? "Enemy " : "";
							text = $"{fEnemyPrefix2}[b]{famRight.name}[/b] was eliminated";
							battle.AppendBattleText(text, false);
							
							if (!battle.isProjectorEncounter && famRight.side == battle.enemySide)
							{
								battle.defeatedFamiliars.Add(famRight.familiar);
							}
						}
					}
				}
			}
		}
		else if (target is Projector proj)
		{
			ProjectorDisplay display = sourceSide == battle.playerSide ? battle.projectorDisplayE : battle.projectorDisplayP;
			display.UpdateDisplay();
			
			if (proj.currentEnergy <= 0)
			{
				text = $"[b]{proj.name}'s[/b] Energy was reduced to 0";
				battle.AppendBattleText(text, false);
			}
		}
	}
	
	public void DefendTarget(object target, float defFactor, BattleManager battle)
	{
		if (defFactor <= 0)
		{
			GD.Print($"BattleCommand: invalid defense factor ({defFactor})");
			battle.AppendBattleText($"Skill Error: invalid defense factor ({defFactor})");
			return;
		}
		
		if (target is Projector proj)
		{
			GD.Print($"BattleCommand: invalid defense target ({proj.name})");
			battle.AppendBattleText($"Skill Error: invalid defense target ({proj.name})");
			return;
		}
		
		string fName = "(no name)";
		string fEnemyPrefix = "";
		
		if (source is FamiliarActor fam)
		{
			fName = string.IsNullOrEmpty(fam.name) ? "(no name)" : fam.name;
			fEnemyPrefix = sourceSide == battle.enemySide? "Enemy " : "";
		}
		
		string tName = "(no name)";
		string tEnemyPrefix = "";
		
		if (target is FamiliarActor tFam)
		{
			tName = string.IsNullOrEmpty(tFam.name) ? "(no name)" : tFam.name;
			tEnemyPrefix = tFam.side == battle.enemySide ? "Enemy " : "";
			
			if (defFactor > 1f)
			{
				tFam.defenseFactor = Mathf.Max(defFactor, tFam.defenseFactor);
			}
			else if (defFactor < 1f && defFactor > 0f)
			{
				tFam.defenseFactor = Mathf.Min(defFactor, tFam.defenseFactor);
			}
		}
		
		string defDescription = "";
		
		if (defFactor >= 2.25f)
		{
			defDescription = "greatly defended";
		}
		else if (defFactor >= 1.75f && defFactor < 2.25f)
		{
			defDescription = "defended";
		}
		else if (defFactor > 1f && defFactor < 1.75f)
		{
			defDescription = "moderately defended";
		}
		else if (defFactor >= 0.5f && defFactor < 1f)
		{
			defDescription = "vulnerable";
		}
		else if (defFactor > 0f && defFactor < 0.5f)
		{
			defDescription = "greatly vulnerable";
		}
		
		if (defFactor != 1f)
		{
			string tText = ReferenceEquals(target, source) ? "itself" : $"{tEnemyPrefix}[b]{tName}[/b]";
			string text = $"{fEnemyPrefix}[b]{fName}[/b] makes {tText} {defDescription}.";
			battle.AppendBattleText(text);
		}
	}
	
	public void ApplyHealing(object target, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		
		if (target is not FamiliarActor fam)
		{
			return;
		}
		
		fam.Heal(amount);
	}
	
	public void ApplyDamage(object target, int amount, bool ow, BattleManager battle)
	{
		if (amount <= 0)
		{
			return;
		}
		
		if (target is FamiliarActor fam)
		{
			int before = fam.currentEnergy;
			
			fam.Damage(amount);
			
			if (ow && fam.currentEnergy <= 0 && fam.side?.projector != null)
			{
				int overflow = amount - before;
				
				if (overflow > 0)
				{
					fam.side.projector.Damage(overflow);
					Projector proj = fam.side.projector;
					string pName = string.IsNullOrEmpty(proj.name) ? "(no name)" : proj.name;
					battle.AppendBattleText($"Overflow damaged [b]{pName}[/b] for {overflow}.", false);
				}
			}
		}
		else if (target is Projector proj)
		{
			proj.Damage(amount);
		}
	}
	
	public void AssignSkill(RSkillData skl)
	{
		skill = skl;
		
		targetPattern = skill.targetPattern;
		
		cost = skill.cost;
		
		speedFactor = skill.speedFactor;
		
		isMagicAttack = skill.isMagicalAttack;
		isMagicTarget = skill.isMagicalDefense;
		
		power = skill.power;
		splash = skill.splashFactor;
		overwhelming = skill.overwhelming;
		
		heal = skill.healPower;
		
		defenseSelf = skill.defenseFactorOnUser;
		defenseTarget = skill.defenseFactorOnTarget;
	}
	
	public bool SingleTarget()
	{
		return targetPattern == RSkillData.TargetPattern.OneAlly || targetPattern == RSkillData.TargetPattern.OneEnemy;
	}
	
	public bool MultiTarget()
	{
		return targetPattern == RSkillData.TargetPattern.AllAllies || targetPattern == RSkillData.TargetPattern.AllEnemies;
	}
	
	public bool OpenTarget()
	{
		return targetPattern != RSkillData.TargetPattern.OneAlly && targetPattern != RSkillData.TargetPattern.OneEnemy;
	}
}
