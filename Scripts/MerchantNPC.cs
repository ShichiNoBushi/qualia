using Godot;
using System;

public partial class MerchantNPC : NPC
{
	[Export] public Godot.Collections.Dictionary<string, int> itemStock;
	
	public override void AfterDialog(Player player)
	{
		GetNode<MerchantMenu>("/root/MerchantMenu").Open(this);
	}
}
