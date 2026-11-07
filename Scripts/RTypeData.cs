using Godot;
using System;

[GlobalClass]
public partial class RTypeData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public Color color {get; set;} = Colors.White;
	[Export] public Color outline {get; set;}
	
	[Export] public Godot.Collections.Array<string> strongAgainst {get; set;} = new();
	[Export] public Godot.Collections.Array<string> weakAgainst {get; set;} = new();
	[Export] public Godot.Collections.Array<string> noEffect {get; set;} = new();
	[Export] public Godot.Collections.Array<string> compatibleWith {get; set;} = new();
	[Export] public Godot.Collections.Array<string> incompatibleWith {get; set;} = new();
	
	public float DamageMultiplier(RTypeData other)
	{
		if (other == null || string.IsNullOrEmpty(other.id))
		{
			return 1f;
		}
		
		if (noEffect.Contains(other.id))
		{
			return 0f;
		}
		else if (strongAgainst.Contains(other.id))
		{
			return 1.25f;
		}
		else if (weakAgainst.Contains(other.id))
		{
			return 0.8f;
		}
		else
		{
			return 1f;
		}
	}
	
	public float HealMultiplier(RTypeData other)
	{
		if (other == null || string.IsNullOrEmpty(other.id))
		{
			return 1f;
		}
		
		if (compatibleWith.Contains(other.id))
		{
			return 1.25f;
		}
		else if (incompatibleWith.Contains(other.id))
		{
			return 0.8f;
		}
		else
		{
			return 1f;
		}
	}
	
	public Color ContrastColor()
	{
		float l = 0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B;
		return l > 0.55f ? Colors.Black : Colors.White;
	}
}
