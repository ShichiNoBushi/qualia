using Godot;
using System;

public abstract partial class BattleCommand : RefCounted
{
	public BattleSide sourceSide {get; set;}
	public object source {get; set;}
	public object target {get; set;}
	
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
			slot = -1;
			
			for (int i = 0; i < BattleSide.MAX_SLOTS; i++)
			{
				if (sourceSide.IsSlotEmpty(i))
				{
					slot = i;
					break;
				}
			}
			
			if (slot == -1)
			{
				battle.AppendBattleText("No slots available");
				return;
			}
		}
		
		int cost = familiar.energy;
		
		if (projector.currentEnergy < cost)
		{
			string fName = string.IsNullOrEmpty(familiar.nickName) ? (string.IsNullOrEmpty(familiar.data.name) ? "(no name)" : familiar.data.name) : familiar.nickName;
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
			
			FamiliarDisplay[] displays = battle.GetFamiliarDisplays(sourceSide);
			displays[slot].AssignFamiliar(actor);
			
			string text = $"[b]{pName}[/b] summons [b]{aName}[/b]";
			battle.AppendBattleText(text);
		}
		else
		{
			battle.AppendBattleText("Failed to summon");
			GD.Print("SummonCommand: Failed to summon");
		}
	}
}

public partial class AttackCommand : BattleCommand
{
	public int power {get; set;} = 0;
	
	public bool isMagicAttack {get; set;} = false;
	public bool isMagicTarget {get; set;} = false;
	
	public override void Execute(BattleManager battle)
	{
		if (target == null)
		{
			Retarget(battle);
			
			if (target == null)
			{
				battle.AppendBattleText("No target available");
				return;
			}
		}
		
		int attackStat = GetAttackStat(source);
		int defenseStat = GetDefenseStat(target);
		
		float raw = (float)(attackStat * power) / Mathf.Max(1, defenseStat);
		int damage = Mathf.Max(1, Mathf.RoundToInt(raw));
		
		ApplyDamage(target, damage);
		
		string fName = "(no name)";
		
		if (source is FamiliarActor fam)
		{
			fName = string.IsNullOrEmpty(fam.name) ? "(no name)" : fam.name;
		}
		
		string tName = "(no name)";
		
		if (target is FamiliarActor tFam)
		{
			tName = string.IsNullOrEmpty(tFam.name) ? "(no name)" : tFam.name;
		}
		else if (target is Projector tProj)
		{
			tName = string.IsNullOrEmpty(tProj.name) ? "(no name)" : tProj.name;
		}
		
		string text = $"[b]{fName}[/b] deals {damage} damage to [b]{tName}[/b]";
		battle.AppendBattleText(text);
		
		if (target is FamiliarActor fam2)
		{
			BattleSide enemySide = fam2.side;
			FamiliarDisplay[] displays = battle.GetFamiliarDisplays(enemySide);
			
			int slot = enemySide.GetSlotIndex(fam2);
			
			displays[slot].UpdateDisplay();
			
			if (!fam2.isAlive)
			{
				if (slot != -1)
				{
					enemySide.ClearSlot(slot);
					
					FamiliarDisplay[] famDisplays = enemySide == battle.playerSide ? battle.famDisplaysP : battle.famDisplaysE;
					famDisplays[slot].Clear();
					
					text = $"[b]{fam2.name}[/b] was eliminated";
					battle.AppendBattleText(text, false);
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
	
	public override void Retarget(BattleManager battle)
	{
		BattleSide enemySide = sourceSide == battle.playerSide ? battle.enemySide : battle.playerSide;
		
		if (enemySide.CountActiveFamiliars() > 0)
		{
			Godot.Collections.Array<FamiliarActor> famList = enemySide.GetFamiliarList();
			
			int idx = (int)GD.Randi() % famList.Count;
			
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

public partial class FocusCommand : BattleCommand
{
	public override void Execute(BattleManager battle)
	{
		int amount = (int)GD.Randi() % 9 + 1;
		
		if (source is Projector projector)
		{
			projector.currentEnergy = Mathf.Min(projector.maxEnergy, projector.currentEnergy + amount);
		}
	}
}
