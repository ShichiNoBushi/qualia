using Godot;
using System;

[GlobalClass]
public partial class RTypeData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public Color color {get; set;} = Colors.White;
	[Export] public Color outline {get; set;}
	
	public Color ContrastColor()
	{
		float l = 0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B;
		return l > 0.55f ? Colors.Black : Colors.White;
	}
}
