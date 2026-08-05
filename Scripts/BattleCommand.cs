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
			return;
		}
		
		if (!sourceSide.HasOpenSlot())
		{
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
				return;
			}
		}
		
		int cost = familiar.energy;
		
		if (projector.currentEnergy < cost)
		{
			return;
		}
		
		FamiliarActor actor = new(familiar);
		
		if (sourceSide.TrySummon(actor, slot))
		{
			projector.currentEnergy -= cost;
		}
		else
		{
			GD.Print("SummonCommand: Failed to summon");
		}
	}
}

public partial class AttackCommand : BattleCommand
{
	public int power {get; set;} = 0;
	
	public override void Execute(BattleManager battle)
	{
		if (target == null)
		{
			Retarget(battle);
			
			if (target == null)
			{
				return;
			}
		}
		
		//calculate damage
		//damage target
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
}

public partial class FocusCommand : BattleCommand
{
	public override void Execute(BattleManager battle)
	{
		int amount = (int)GD.Randi() % 10 + 1;
		
		if (source is Projector projector)
		{
			projector.currentEnergy = Mathf.Min(projector.maxEnergy, projector.currentEnergy + amount);
		}
	}
}
