using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	public GameSession session;
	
	public Button closeButton;
	
	public Label nameLabel;
	
	public Label levelLabel;
	
	public ProgressBar experienceProgress;
	public Label experienceLabel;
	public Label experienceNextLabel;
	
	public ProgressBar energyProgress;
	public Label currentEnergyLabel;
	public Label maxEnergyLabel;
	
	public TextureRect portraitRect;
	
	public Button quitButton;
	public ConfirmationDialog quitConfirm;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		
		closeButton = GetNode<Button>("Panel/CloseButton");
		
		nameLabel = GetNode<Label>("Panel/TabContainer/Projector/NameLabel");
		
		levelLabel = GetNode<Label>("Panel/TabContainer/Projector/LevelLabel");
		
		experienceProgress = GetNode<ProgressBar>("Panel/TabContainer/Projector/ExperienceProgress");
		experienceLabel = GetNode<Label>("Panel/TabContainer/Projector/ExperienceLabel");
		experienceNextLabel = GetNode<Label>("Panel/TabContainer/Projector/ExperienceNextLabel");
		
		energyProgress = GetNode<ProgressBar>("Panel/TabContainer/Projector/EnergyProgress");
		currentEnergyLabel = GetNode<Label>("Panel/TabContainer/Projector/CurrentEnergyLabel");
		maxEnergyLabel = GetNode<Label>("Panel/TabContainer/Projector/MaxEnergyLabel");
		
		portraitRect = GetNode<TextureRect>("Panel/TabContainer/Projector/PortraitRect");
		
		quitButton = GetNode<Button>("Panel/TabContainer/Options/CenterContainer/VBoxContainer/QuitButton");
		quitConfirm = GetNode<ConfirmationDialog>("QuitConfirm");
		
		closeButton.Pressed += OnClosePressed;
		
		quitButton.Pressed += OnQuitPressed;
		quitConfirm.Confirmed += OnQuitConfirmed;
	}
	
	public override void _UnhandledInput(InputEvent e)
	{
		GD.Print($"GameMenu: mode={session.gameMode} visible={Visible}");
		
		if (session.gameMode != GameSession.GameMode.World)
		{
			return;
		}
		
		if (!e.IsActionPressed("ui_cancel"))
		{
			return;
		}
		
		GD.Print("GameMenu: Esc pressed");
		
		if (Visible)
		{
			Close();
		}
		else
		{
			Open();
		}
		
		GetViewport().SetInputAsHandled();
	}
	
	public void OnClosePressed()
	{
		Close();
	}
	
	public void OnQuitPressed()
	{
		quitConfirm.PopupCentered();
	}
	
	public void OnQuitConfirmed()
	{
		GetTree().Quit();
	}
	
	public void UpdateProjectorLabels()
	{
		Projector proj = session.playerProjector;
		
		nameLabel.Text = proj.name;
		
		levelLabel.Text = $"{proj.level}";
		
		experienceProgress.Value = proj.experience;
		experienceProgress.MaxValue = proj.ExpToNextLevel();
		experienceLabel.Text = $"{proj.experience}";
		experienceNextLabel.Text = $"{proj.ExpToNextLevel()}";
		
		energyProgress.Value = proj.currentEnergy;
		energyProgress.MaxValue = proj.maxEnergy;
		currentEnergyLabel.Text = $"{proj.currentEnergy}";
		maxEnergyLabel.Text = $"{proj.maxEnergy}";
		
		portraitRect.Texture = proj.data?.portrait;
	}
	
	public void Open()
	{
		Visible = true;
		UpdateProjectorLabels();
		GetTree().Paused = true;
	}
	
	public void Close()
	{
		Visible = false;
		GetTree().Paused = false;
	}
}
