using Godot;
using System;

public partial class MapRegion : Area2D
{
	[Export] public string regionName {get; set;} = "";
	
	public MapDisplay display;
	
	public override void _Ready()
	{
		display = GetNode<MapDisplay>("/root/MapDisplay");
		
		BodyEntered += OnBodyEntered;
		
		foreach (var body in GetOverlappingBodies())
		{
			if (body is Player)
			{
				display.DisplayRegion(regionName);
			}
		}
	}
	
	public void OnBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			display.DisplayRegion(regionName);
		}
	}
}
