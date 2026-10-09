using Godot;
using System;

[GlobalClass]
public partial class RCrystalDrop : Resource
{
	[Export] public string crystalId {get; set;} = "";
	[Export] public int amount {get; set;} = 0;
	[Export] public float chance {get; set;} = 0f;
}
