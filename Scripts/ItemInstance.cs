using Godot;
using System;

public partial class ItemInstance : RefCounted
{
	public RItemData data;
	public int maxUses;
	public int usesLeft;
}
