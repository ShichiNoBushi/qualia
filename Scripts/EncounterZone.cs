using Godot;
using System;

public partial class EncounterZone : Area2D
{
	[Export] public Godot.Collections.Array<REncounterData> smallEncounters {get; set;}
	[Export] public Godot.Collections.Array<REncounterData> largeEncounters {get; set;}
	[Export] public PackedScene smallSparkScene {get; set;}
	[Export] public PackedScene largeSparkScene {get; set;}
	[Export] public int maxSparks {get; set;} = 3;
	[Export] public float minInterval {get; set;} = 8f;
	[Export] public float maxInterval {get; set;} = 20f;
	[Export] public float largeChance {get; set;} = 0.25f;
	
	public Godot.Collections.Array<WildSpark> liveSparks;
	
	public Timer timer;
	
	public override void _Ready()
	{
		liveSparks = new();
		
		timer = GetNode<Timer>("Timer");
		timer.Timeout += SpawnSpark;
		RestartTimer();
	}
	
	public bool TryRandomPoint(out Vector2 point, int attempts = 16)
	{
		CollisionPolygon2D poly = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		var pts = poly.Polygon;
		Rect2 rect = new(pts[0], Vector2.Zero);
		
		foreach (var p in pts)
		{
			rect = rect.Expand(p);
		}
		
		//PhysicsDirectSpaceState2D space = GetWorld2D().DirectSpaceState;
		
		for (int i = 0; i < attempts; i++)
		{
			Vector2 local = new((float)GD.RandRange(rect.Position.X, rect.End.X), (float)GD.RandRange(rect.Position.Y, rect.End.Y));
			
			if (!Geometry2D.IsPointInPolygon(local, pts))
			{
				continue;
			}
			
			point = ToGlobal(local);
			
			return true;
		}
		
		point = default;
		return false;
	}
	
	public void SpawnSpark()
	{
		if (liveSparks.Count >= maxSparks)
		{
			RestartTimer();
			return;
		}
		
		if (!TryRandomPoint(out Vector2 pos))
		{
			RestartTimer();
			return;
		}
		
		bool large = largeChance > 0f && largeEncounters.Count > 0 && GD.Randf() < largeChance;
		
		Godot.Collections.Array<REncounterData> encList = large ? largeEncounters : smallEncounters;
		
		if (encList == null || encList.Count == 0)
		{
			RestartTimer();
			return;
		}
		
		PackedScene scene = large ? largeSparkScene : smallSparkScene;
		WildSpark spark = scene.Instantiate<WildSpark>();
		
		spark.encounter = encList[(int)(GD.Randi() % encList.Count)];
		spark.zone = this;
		GetParent().AddChild(spark);
		spark.GlobalPosition = pos;
		liveSparks.Add(spark);
		
		RestartTimer();
	}
	
	public void RemoveSpark(WildSpark spark)
	{
		liveSparks.Remove(spark);
	}
	
	public void RestartTimer()
	{
		timer.Start((float)GD.RandRange(minInterval, maxInterval));
	}
}
