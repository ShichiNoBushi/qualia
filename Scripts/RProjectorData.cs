using Godot;
using System;

[GlobalClass]
public partial class RProjectorData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public int energy {get; set;} = 100;
	[Export] public Texture2D portrait {get; set;}
}
