using Godot;
using System;

public partial class FamiliarDisplay : Control
{
	public TextureRect portraitRect;
	public Label nameLabel;
	public ProgressBar energyProgress;
	public Label energyLabel;
	public ColorRect highlightAllyRect;
	public ColorRect highlightEnemyRect;
	
	public bool isFamiliar {get; private set;} = false;
	public bool energyVisible {get; private set;} = true;
	public IBattleActor actor {get; private set;}
	
	public bool isPlayerSide {get; set;}
	public int slotIndex {get; set;}
	public BattleManager battle {get; set;}
	public Texture2D sparkTexture {get; set;}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		portraitRect = GetNode<TextureRect>("PortraitRect");
		nameLabel = GetNode<Label>("NameLabel");
		energyProgress = GetNode<ProgressBar>("EnergyProgress");
		energyLabel = GetNode<Label>("EnergyLabel");
		highlightAllyRect = GetNode<ColorRect>("HighlightAllyRect");
		highlightEnemyRect = GetNode<ColorRect>("HighlightEnemyRect");
		energyLabel.Visible = false;
		sparkTexture = GD.Load<Texture2D>("res://Resources/Assets/Sprites/sparkle_small.png");
		
		highlightAllyRect.GuiInput += OnGuiInput;
		highlightEnemyRect.GuiInput += OnGuiInput;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnGuiInput(InputEvent e)
	{
		if (e is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
		{
			GD.Print($"FamiliarDisplay: slot {slotIndex} clicked");
			try
			{
				battle.FamiliarSlotClicked(this);
			}
			catch (Exception ex)
			{
				GD.PrintErr($"FamiliarDisplay: {ex}");
			}
		}
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
		energyVisible = toggle;
	}
	
	public void UpdateDisplay()
	{
		if (actor == null)
		{
			SetElementsVisible(false);
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
				SetElementsVisible(false);
				portraitRect.Texture = null;
				nameLabel.Text = "";
				energyProgress.MaxValue = 1;
				energyProgress.Value = 0;
				energyLabel.Text = "";
				return;
			}
			
			SetElementsVisible(true);
		
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
				SetElementsVisible(false);
				portraitRect.Texture = sparkTexture;
				nameLabel.Text = "";
				energyProgress.MaxValue = 1;
				energyProgress.Value = 0;
				energyLabel.Text = "";
				return;
			}
			
			SetElementsVisible(true);
			
			portraitRect.Texture = sparkTexture;
			
			nameLabel.Text = "Manifesting...";
			
			energyProgress.MaxValue = 1;
			energyProgress.Value = 0;
			
			energyLabel.Text = "";
		}
	}
	
	public void SetElementsVisible(bool visible)
	{
		portraitRect.Visible = visible;
		nameLabel.Visible = visible;
		energyProgress.Visible = visible;
		energyLabel.Visible = visible && energyVisible;
	}
	
	public void HighlightAlly(bool toggle)
	{
		highlightAllyRect.Visible = toggle;
		highlightEnemyRect.Visible = false;
	}
	
	public void HighlightEnemy(bool toggle)
	{
		highlightEnemyRect.Visible = toggle;
		highlightAllyRect.Visible = false;
	}
	
	public void ClearHighlights()
	{
		highlightAllyRect.Visible = false;
		highlightEnemyRect.Visible = false;
	}
}
