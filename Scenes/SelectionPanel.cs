using Godot;
using System;
using System.Collections.Generic;

public partial class SelectionPanel : Control
{
	public Label titleLabel;
	public VBoxContainer itemVBox;
	public Button cancelButton;
	public Button confirmButton;
	
	public BattleManager battle;
	
	public Action<object> OnItemChosen;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		titleLabel = GetNode<Label>("TitleLabel");
		itemVBox = GetNode<VBoxContainer>("ScrollContainer/ItemVBox");
		cancelButton = GetNode<Button>("CancelButton");
		confirmButton = GetNode<Button>("ConfirmButton");
		
		cancelButton.Pressed += HidePanel;
		confirmButton.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Open(string title, IEnumerable<(object data, string label, bool enabled)> entries)
	{
		GD.Print($"SelectionPanel: Opened with title \"{title}\" and {((List<(object, string, bool)>)entries).Count} entries");
		
		titleLabel.Text = title;
		ClearRows();
		
		foreach (var (data, label, enabled) in entries)
		{
			Button btn = new Button {
				Text = label,
				Disabled = !enabled
			};
			
			var captured = data;
			
			btn.Pressed += () => {
				OnItemChosen?.Invoke(captured);
				HidePanel();
			};
			
			itemVBox.AddChild(btn);
		}
		
		Visible = true;
		BlockCommands();
	}
	
	public void HidePanel()
	{
		Visible = false;
		OnItemChosen = null;
		ClearRows();
		UnblockCommands();
	}
	
	public void ClearRows()
	{
		foreach (var child in itemVBox.GetChildren())
		{
			child.QueueFree();
		}
	}
	
	public void BlockCommands()
	{
		battle.projCommandPanel.DisableCommands();
		
		for (int i = 0; i < 4; i++)
		{
			battle.famCommandPanels[i].DisableCommands();
		}
	}
	
	public void UnblockCommands()
	{
		if (!battle.projCommandDisabled)
		{
			battle.projCommandPanel.EnableCommands();
		}
		
		for (int i = 0; i < 4; i++)
		{
			if (!battle.famCommandDisabled[i])
			{
				battle.famCommandPanels[i].EnableCommands();
			}
		}
	}
}
