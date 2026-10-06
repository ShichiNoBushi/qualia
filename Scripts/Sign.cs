using Godot;
using System;

public partial class Sign : StaticBody2D
{
	[Export] public string label {get; set;} = "Sign";
	[Export] public string textNorth {get; set;} = "Nothing written here.";
	[Export] public string textSouth {get; set;} = "Nothing written here.";
	[Export] public string textWest {get; set;} = "Nothing written here.";
	[Export] public string textEast {get; set;} = "Nothing written here.";
	
	public void Interact(Player player)
	{
		Vector2 d = player.GlobalPosition - GlobalPosition;
		
		string readText;
		
		if (Mathf.Abs(d.X) > Mathf.Abs(d.Y))
		{
			readText = d.X > 0 ? textEast : textWest;
		}
		else
		{
			readText = d.Y > 0 ? textSouth : textNorth;
		}
		
		if (string.IsNullOrEmpty(readText))
		{
			readText = "Nothing written here.";
		}
		
		GetNode<DialogBox>("/root/DialogBox").Play(label, readText, null);
	}
}
