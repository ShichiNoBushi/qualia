using Godot;
using System;

[GlobalClass]
public partial class RSkillData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public RTypeData type {get; set;}
	
	[Export] public TargetPattern targetPattern {get; set;} = TargetPattern.OneEnemy;
	[Export] public int cost {get; set;} = 0;
	[Export] public float speedFactor {get; set;} = 1f;
	
	[Export] public bool isMagicalAttack {get; set;} = false;
	[Export] public bool isMagicalDefense {get; set;} = false;
	[Export] public int power {get; set;} = 0;
	[Export] public float splashFactor {get; set;} = 0f;
	[Export] public bool overwhelming {get; set;} = false;
	
	[Export] public int healPower {get; set;} = 0;
	[Export] public float defenseFactorOnUser {get; set;} = 1f;
	[Export] public float defenseFactorOnTarget {get; set;} = 1f;
	
	public enum TargetPattern {
		None,
		Self,
		OneAlly,
		OneEnemy,
		AllAllies,
		AllEnemies,
		AllUnits
	}
}
