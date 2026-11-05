using Godot;
using System;

[GlobalClass]
public partial class RTypeData : Resource
{
	[Export] public string id {get; set;} = "";
	[Export] public string name {get; set;} = "";
	[Export] public Color color {get; set;} = Colors.White;
	[Export] public Color outline {get; set;}
	
	[Export] public Godot.Collections.Array<RTypeData> strongAgainst {get; set;}
	[Export] public Godot.Collections.Array<RTypeData> weakAgainst {get; set;}
	[Export] public Godot.Collections.Array<RTypeData> noEffect {get; set;}
	[Export] public Godot.Collections.Array<RTypeData> compatibleWith {get; set;}
	[Export] public Godot.Collections.Array<RTypeData> incompatibleWith {get; set;}
	
	public float DamageMultiplier(RTypeData other)
	{
		if (other == null)
		{
			return 1f;
		}
		
		if (noEffect.Contains(other))
		{
			return 0f;
		}
		else if (strongAgainst.Contains(other))
		{
			return 1.25f;
		}
		else if (weakAgainst.Contains(other))
		{
			return 0.75f;
		}
		else
		{
			return 1f;
		}
	}
	
	public float HealMultiplier(RTypeData other)
	{
		if (other == null)
		{
			return 1f;
		}
		
		if (compatibleWith.Contains(other))
		{
			return 1.25f;
		}
		else if (incompatibleWith.Contains(other))
		{
			return 0.75f;
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
