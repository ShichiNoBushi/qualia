using Godot;
using System;

public partial class GameSession : Node
{
	public Projector playerProjector {get; set;}
	public string returnPath {get; set;}
	public Vector2 returnPosition {get; set;}
	public Vector2 returnFacing {get; set;}
	public REncounterData pendingEncounter {get; set;}
	public BattleManager.VictoryResult lastResult {get; set;}
	public GameMode gameMode {get; set;}
	
	public enum GameMode
	{
		World,
		Battle
	}
	
	public override void _Ready()
	{
		RProjectorData pData = GD.Load<RProjectorData>("res://Resources/test_projector.tres");
		RFamiliarInstance gnomeInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_gnome.tres");
		RFamiliarInstance salamanderInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_salamander.tres");
		RFamiliarInstance sylphInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_sylph.tres");
		RFamiliarInstance undineInst = GD.Load<RFamiliarInstance>("res://Resources/familiar_instance/ex_undine.tres");
		
		if (pData == null)
		{
			GD.Print("BattleManager - Failed to start: invalid projector data");
			return;
		}
		
		playerProjector = new();
		playerProjector.Initialize(pData);
		
		gnomeInst.Initialize();
		salamanderInst.Initialize();
		sylphInst.Initialize();
		undineInst.Initialize();
		
		playerProjector.GiveFamiliar(gnomeInst);
		playerProjector.GiveFamiliar(salamanderInst);
		playerProjector.GiveFamiliar(sylphInst);
		playerProjector.GiveFamiliar(undineInst);
		
		gameMode = GameMode.World;
	}
}
