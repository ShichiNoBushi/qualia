using Godot;
using System;

public partial class ProjectorDisplay : Control
{
	public TextureRect portraitRect;
	public Label nameLabel;
	public ProgressBar energyProgress;
	public Label energyLabel;
	
	public Projector projector;
	
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
	
	public void AssignProjector(Projector proj)
	{
		projector = proj;
		
		UpdateDisplay();
	}
	
	public void Clear()
	{
		projector = null;
		
		UpdateDisplay();
	}
	
	public void SetVisibleEnergy(bool toggle)
	{
		energyLabel.Visible = toggle;
	}
	
	public void UpdateDisplay()
	{
		if (projector == null)
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
		
		RProjectorData data = projector.data;
		
		portraitRect.Texture = data != null && data.portrait != null ? data.portrait : null;
		
		nameLabel.Text = string.IsNullOrEmpty(projector.name) ? "(no name)" : projector.name;
		
		energyProgress.MaxValue = Mathf.Max(projector.maxEnergy, 1);
		energyProgress.Value = Mathf.Clamp(projector.currentEnergy, 0, projector.maxEnergy);
		
		energyLabel.Text = $"{projector.currentEnergy} / {projector.maxEnergy}";
	}
}
