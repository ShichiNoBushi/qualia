using Godot;
using System;

[GlobalClass]
public partial class RItemData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public string description {get; set;} = "";
	
	[Export] public bool stackable {get; set;} = true;
	[Export] public int uses {get; set;} = 1;
	
	[Export] public bool projectorUsable {get; set;} = true;
	[Export] public bool familiarUsable {get; set;} = true;
	
	[Export] public bool fieldUsable {get; set;} = true;
	[Export] public bool battleUsable {get; set;} = true;
	
	[Export] public ItemPattern itemPattern {get; set;} = ItemPattern.OneAlly;
	
	[Export] public bool isMagicalDefense {get; set;} = false;
	[Export] public int power {get; set;} = 0;
	[Export] public float splashFactor {get; set;} = 0f;
	[Export] public bool overwhelming {get; set;} = false;
	
	[Export] public int healPower {get; set;} = 0;
	[Export] public float defenseFactorOnTarget {get; set;} = 1f;
	
	public enum ItemPattern {
		None,
		OneAlly,
		OneEnemy,
		AllAllies,
		AllEnemies,
		AllUnits
	}
}
