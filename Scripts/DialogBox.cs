using Godot;
using System;
using System.Collections.Generic;

public partial class DialogBox : CanvasLayer
{
	[Export] public int maxCharsPerPage {get; set;} = 160;
	
	public GameSession session;
	
	public Panel dialogPanel;
	public Label speakerLabel;
	public RichTextLabel bodyLabel;
	public Label promptLabel;
	
	public Panel choicesPanel;
	public HBoxContainer choicesHBox;
	public List<Button> choiceButtons;
	
	public Action onFinished;
	
	public RDialog dialog;
	public Godot.Collections.Dictionary<string, RDialog> dialogTree;
	public string[] pages;
	public int pageIndex;
	public bool waiting;
	
	public int openedOnFrame = -1;
	
	public bool IsOpen => Visible;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		
		dialogPanel = GetNode<Panel>("DialogPanel");
		speakerLabel = GetNode<Label>("DialogPanel/SpeakerLabel");
		bodyLabel = GetNode<RichTextLabel>("DialogPanel/BodyLabel");
		promptLabel = GetNode<Label>("DialogPanel/PromptLabel");
		
		choicesPanel = GetNode<Panel>("DialogPanel/ChoicesPanel");
		choicesHBox = GetNode<HBoxContainer>("DialogPanel/ChoicesPanel/CenterContainer/ChoicesHBox");
		choiceButtons = new();
		
		dialogPanel.GuiInput += OnDialogClicked;
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
		
		if (choicesPanel.Visible)
		{
			return;
		}
		
		Advance();
		GetViewport().SetInputAsHandled();
	}
	
	public void OnDialogClicked(InputEvent e)
	{
		if (e is not InputEventMouseButton mb)
		{
			return;
		}
		
		if (!mb.Pressed || mb.ButtonIndex != MouseButton.Left)
		{
			return;
		}
		
		if (!Visible || session.gameMode != GameSession.GameMode.Dialog)
		{
			return;
		}
		
		if ((int)Engine.GetProcessFrames() <= openedOnFrame)
		{
			return;
		}
		
		if (choicesPanel.Visible)
		{
			return;
		}
		
		Advance();
		GetViewport().SetInputAsHandled();
	}
	
	public void Play(string speaker, string raw, Action finished)
	{
		onFinished = finished;
		dialogTree = null;
		dialog = null;
		speakerLabel.Text = SpeakerName(speaker);
		pages = SplitPages(raw ?? "");
		pageIndex = 0;
		GetTree().Paused = true;
		session.gameMode = GameSession.GameMode.Dialog;
		Visible = true;
		openedOnFrame = (int)Engine.GetProcessFrames();
		bodyLabel.Clear();
		UpdateBodyText();
	}
	
	public void Play(Godot.Collections.Dictionary<string, RDialog> tree, string start, Action finished)
	{
		if (tree == null || !tree.ContainsKey(start))
		{
			return;
		}
		
		onFinished = finished;
		dialogTree = tree;
		dialog = dialogTree[start];
		speakerLabel.Text = SpeakerName(dialog.speaker ?? "");
		pages = SplitPages(dialog.text ?? "");
		pageIndex = 0;
		GetTree().Paused = true;
		session.gameMode = GameSession.GameMode.Dialog;
		Visible = true;
		openedOnFrame = (int)Engine.GetProcessFrames();
		bodyLabel.Clear();
		UpdateBodyText();
	}
	
	public void ShowChoices(Godot.Collections.Array<RDialogChoice> choices)
	{
		choicesPanel.Visible = true;
		
		choiceButtons.Clear();
		
		foreach (var child in choicesHBox.GetChildren())
		{
			child.QueueFree();
		}
		
		foreach (var ch in choices)
		{
			bool choiceDisabled = false;
			
			foreach (var flag in ch.requireFlags)
			{
				if (!session.HasFlag(flag))
				{
					choiceDisabled = true;
					break;
				}
			}
			
			foreach (var flag in ch.forbidFlags)
			{
				if (session.HasFlag(flag))
				{
					choiceDisabled = true;
					break;
				}
			}
			
			Button btn = new()
			{
				Text = ch.label,
				Disabled = choiceDisabled
			};
			
			btn.Pressed += () => OnChoiceSelected(ch);
			
			choicesHBox.AddChild(btn);
			choiceButtons.Add(btn);
		}
	}
	
	public void OnChoiceSelected(RDialogChoice choice)
	{
		choicesPanel.Visible = false;
		
		choice.onSelect?.Execute(session);
		
		if (!string.IsNullOrEmpty(choice.nextId) && dialogTree != null && dialogTree.ContainsKey(choice.nextId))
		{
			dialog = dialogTree[choice.nextId];
			pages = SplitPages(dialog.text ?? "");
			pageIndex = 0;
			speakerLabel.Text = SpeakerName(dialog.speaker ?? "");
			bodyLabel.Clear();
			UpdateBodyText();
			return;
		}
		
		dialog?.onFinish?.Execute(session);
		
		Visible = false;
		GetTree().Paused = false;
		session.gameMode = GameSession.GameMode.World;
		
		if (onFinished != null)
		{
			Action done = onFinished;
			onFinished = null;
			done.Invoke();
		}
	}
	
	public void Advance()
	{
		pageIndex++;
		
		if (pageIndex >= pages.Length)
		{
			if (dialog != null && dialog.choices != null && dialog.choices.Count > 0)
			{
				ShowChoices(dialog.choices);
				return;
			}
			
			dialog?.onFinish?.Execute(session);
			
			if (dialog != null && !string.IsNullOrEmpty(dialog.nextId) && dialogTree != null && dialogTree.ContainsKey(dialog.nextId))
			{
				dialog = dialogTree[dialog.nextId];
				pages = SplitPages(dialog.text ?? "");
				pageIndex = 0;
				speakerLabel.Text = SpeakerName(dialog.speaker ?? "");
				bodyLabel.Clear();
				UpdateBodyText();
				return;
			}
			
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
	
	public string SpeakerName(string speaker) => speaker == "[player]" ? session.playerProjector.name : speaker;
}
