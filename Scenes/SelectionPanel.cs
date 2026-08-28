using Godot;
using System;
using System.Collections.Generic;

public partial class SelectionPanel : Control
{
	public Label titleLabel;
	public VBoxContainer itemVBox;
	public Button cancelButton;
	public Button confirmButton;
	
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
	}
	
	public void HidePanel()
	{
		Visible = false;
		OnItemChosen = null;
		ClearRows();
	}
	
	public void ClearRows()
	{
		foreach (var child in itemVBox.GetChildren())
		{
			child.QueueFree();
		}
	}
}
