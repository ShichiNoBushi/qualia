using Godot;
using System;

public partial class FamiliarDisplay : Control
{
	public TextureRect portraitRect;
	public Label nameLabel;
	public ProgressBar energyProgress;
	public Label energyLabel;
	
	public FamiliarActor familiar {get; private set;}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		portraitRect = GetNode<TextureRect>("PortraitRect");
		nameLabel = GetNode<Label>("NameLabel");
		energyProgress = GetNode<ProgressBar>("EnergyProgress");
		energyLabel = GetNode<Label>("EnergyLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void AssignFamiliar(FamiliarActor fam)
	{
		familiar = fam;
		
		UpdateDisplay();
	}
	
	public void Clear()
	{
		familiar = null;
	}
	
	public void SetVisibleEnergy(bool toggle)
	{
		energyLabel.Visible = toggle;
	}
	
	public void UpdateDisplay()
	{
		if (familiar == null || familiar.familiar == null)
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
}
