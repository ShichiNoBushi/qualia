using Godot;
using System;

public partial class FamiliarDisplay : Control
{
	public TextureRect portraitRect;
	public Label nameLabel;
	public ProgressBar energyProgress;
	public Label energyLabel;
	
	public bool isFamiliar {get; private set;} = false;
	public IBattleActor actor {get; private set;}
	public int slotIndex {get; private set;}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		portraitRect = GetNode<TextureRect>("PortraitRect");
		nameLabel = GetNode<Label>("NameLabel");
		energyProgress = GetNode<ProgressBar>("EnergyProgress");
		energyLabel = GetNode<Label>("EnergyLabel");
		energyLabel.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void AssignFamiliar(FamiliarActor fam)
	{
		actor = fam;
		isFamiliar = true;
		
		UpdateDisplay();
	}
	
	public void AssignSpawn(SpawnActor spark)
	{
		actor = spark;
		isFamiliar = false;
		
		UpdateDisplay();
	}
	
	public void SetSlot(int slot)
	{
		slotIndex = slot;
	}
	
	public void Clear()
	{
		actor = null;
	}
	
	public void SetVisibleEnergy(bool toggle)
	{
		energyLabel.Visible = toggle;
	}
	
	public void UpdateDisplay()
	{
		if (actor == null)
		{
			Visible = false;
			portraitRect.Texture = null;
			nameLabel.Text = "";
			energyProgress.MaxValue = 1;
			energyProgress.Value = 0;
			energyLabel.Text = "";
			return;
		}
		
		if (isFamiliar && actor is FamiliarActor familiar)
		{
			if (familiar.familiar == null)
			{
				Visible = false;
				portraitRect.Texture = null;
				nameLabel.Text = "";
				energyProgress.MaxValue = 1;
				energyProgress.Value = 0;
				energyLabel.Text = "";
				return;
			}
			
			Visible = true;
		
			RFamiliarData data = familiar.familiar.data;
			
			portraitRect.Texture = data != null && data.portrait != null ? data.portrait : null;
			
			nameLabel.Text = string.IsNullOrEmpty(familiar.name) ? "(no name)" : familiar.name;
			
			energyProgress.MaxValue = Mathf.Max(familiar.maxEnergy, 1);
			energyProgress.Value = Mathf.Clamp(familiar.currentEnergy, 0, familiar.maxEnergy);
			
			energyLabel.Text = $"{familiar.currentEnergy} / {familiar.maxEnergy}";
		}
		else if (!isFamiliar && actor is SpawnActor spark)
		{
			if (spark.familiar == null)
			{
				Visible = false;
				portraitRect.Texture = null;
				nameLabel.Text = "";
				energyProgress.MaxValue = 1;
				energyProgress.Value = 0;
				energyLabel.Text = "";
				return;
			}
			
			Visible = true;
			
			portraitRect.Texture = null; //set to default spark portrait
			
			nameLabel.Text = "Manifesting...";
			
			energyProgress.MaxValue = 1;
			energyProgress.Value = 0;
			
			energyLabel.Text = "";
		}
	}
}
