using Godot;
using System;

[GlobalClass]
public partial class RTypeData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public Color color {get; set;} = Colors.White;
}
