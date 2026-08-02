using Godot;
using System;

[GlobalClass]
public partial class RSkillData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public RTypeData type {get; set;}
	[Export] public int cost {get; set;} = 0;
	[Export] public int power {get; set;} = 0;
}
