using Godot;
using System;

public partial class MapDisplay : CanvasLayer
{
	public Label regionLabel {get; set;}
	public AnimationPlayer animation {get; set;}
	public Timer timer {get; set;}
	
	public bool isDisplayed = false;
	
	public override void _Ready()
	{
		regionLabel = GetNode<Label>("Panel/RegionLabel");
		animation = GetNode<AnimationPlayer>("AnimationPlayer");
		timer = GetNode<Timer>("Timer");
		
		timer.Timeout += OnTimeout;
	}
	
	public void OnTimeout()
	{
		HideRegion();
	}
	
	public void DisplayRegion(string regName)
	{
		regionLabel.Text = regName;
		
		if (animation.CurrentAnimation == "Hide Region" && animation.IsPlaying())
		{
			double hidePos = animation.CurrentAnimationPosition;
			animation.Play("Display Region");
			animation.Seek(0.5 - hidePos, true);
			isDisplayed = true;
		}
		else if (!isDisplayed)
		{
			animation.Play("Display Region");
			isDisplayed = true;
		}
		
		timer.Start();
	}
	
	public void HideRegion()
	{
		animation.Play("Hide Region");
		isDisplayed = false;
	}
}
