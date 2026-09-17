using Godot;
using System;

public partial class DialogBox : CanvasLayer
{
	[Export] public int maxCharsPerPage {get; set;} = 160;
	
	public GameSession session;
	
	public Label speakerLabel;
	public RichTextLabel bodyLabel;
	public Label promptLabel;
	
	public Action onFinished;
	
	public string[] pages;
	public int pageIndex;
	public bool waiting;
	
	public int openedOnFrame = -1;
	
	public bool IsOpen => Visible;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		
		speakerLabel = GetNode<Label>("Panel/SpeakerLabel");
		bodyLabel = GetNode<RichTextLabel>("Panel/BodyLabel");
		promptLabel = GetNode<Label>("Panel/PromptLabel");
	}
	
	public override void _UnhandledInput(InputEvent e)
	{
		if (!Visible || !e.IsActionPressed("ui_accept"))
		{
			return;
		}
		
		if (session.gameMode != GameSession.GameMode.Dialog)
		{
			return;
		}
		
		if ((int)Engine.GetProcessFrames() <= openedOnFrame)
		{
			return;
		}
		
		Advance();
		GetViewport().SetInputAsHandled();
	}
	
	public void Play(string speaker, string raw, Action finished)
	{
		onFinished = finished;
		pages = SplitPages(raw ?? "");
		pageIndex = 0;
		GetTree().Paused = true;
		session.gameMode = GameSession.GameMode.Dialog; //before this line was added, it displayed first page but didn't respond to input
		Visible = true;
		openedOnFrame = (int)Engine.GetProcessFrames();
		bodyLabel.Clear();
		UpdateBodyText();
	}
	
	public void Advance()
	{
		pageIndex++;
		
		if (pageIndex >= pages.Length)
		{
			Visible = false;
			GetTree().Paused = false;
			session.gameMode = GameSession.GameMode.World;
			
			if (onFinished != null)
			{
				Action done = onFinished;
				onFinished = null;
				done.Invoke();
			}
			
			return;
		}
		
		UpdateBodyText();
	}
	
	public void UpdateBodyText()
	{
		string text = (pageIndex > 0 ? "\n\n" : "") + pages[pageIndex];
		
		bodyLabel.AppendText(text);
		
		promptLabel.Text = pageIndex < pages.Length - 1 ? ">>>" : "X";
	}
	
	public string[] SplitPages(string raw)
	{
		string[] chunks = raw.Split("[break]");
		System.Collections.Generic.List<string> pageList = new();
		
		foreach (var chunk in chunks)
		{
			string text = chunk.Trim();
			
			if (text.Length == 0)
			{
				continue;
			}
			
			if (text.Length <= maxCharsPerPage)
			{
				pageList.Add(text);
				continue;
			}
			
			int start = 0;
			
			while (start < text.Length)
			{
				int len = Math.Min(maxCharsPerPage, text.Length - start);
				int end = start + len;
				
				if (end < text.Length)
				{
					int space = text.LastIndexOf(' ', end - 1, len);
					
					if (space > start)
					{
						end = space;
					}
				}
				
				pageList.Add(text.Substring(start, end - start).Trim());
				
				start = end;
				
				while (start < text.Length && char.IsWhiteSpace(text[start]))
				{
					start++;
				}
			}
		}
		
		if (pageList.Count == 0)
		{
			pageList.Add("");
		}
		
		return pageList.ToArray();
	}
}
