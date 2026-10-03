using Godot;
using System;

public partial class ForgeNPC : NPC
{
	[Export] public Godot.Collections.Dictionary<string, int> itemStock;
	[Export] public Godot.Collections.Array<string> compatibleRecipes;
	[Export] public Godot.Collections.Array<string> knownRecipes;
	
	public override void AfterDialog(Player player)
	{
		GetNode<ForgeMenu>("/root/ForgeMenu").Open(this);
	}
}
