using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	public GameSession session;
	
	public Button closeButton;
	
	public Button quitButton;
	public ConfirmationDialog quitConfirm;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		
		closeButton = GetNode<Button>("Panel/CloseButton");
		
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
	
	public void Open()
	{
		Visible = true;
		GetTree().Paused = true;
	}
	
	public void Close()
	{
		Visible = false;
		GetTree().Paused = false;
	}
}
