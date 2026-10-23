using Godot;
using System;

public partial class TitleMenu : Control
{
	public const string START_MAP = "world";
	
	public GameSession session;
	public DataRegistry registry;
	
	public Panel titlePanel;
	
	public Button newButton;
	public Button loadButton;
	public Button quitButton;
	
	public Panel newCharacterPanel;
	
	public LineEdit nameEdit;
	public ItemList familiarList;
	
	public Button backButton;
	public Button confirmButton;
	
	public FileDialog loadDialog;
	public ConfirmationDialog quitConfirm;
	public ConfirmationDialog createConfirm;
	
	public RFamiliarData selectedFamiliar;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		titlePanel = GetNode<Panel>("TitlePanel");
		
		newButton = GetNode<Button>("TitlePanel/CenterContainer/VBoxContainer/NewButton");
		loadButton = GetNode<Button>("TitlePanel/CenterContainer/VBoxContainer/LoadButton");
		quitButton = GetNode<Button>("TitlePanel/CenterContainer/VBoxContainer/QuitButton");
		
		loadDialog = GetNode<FileDialog>("LoadDialog");
		quitConfirm = GetNode<ConfirmationDialog>("QuitConfirm");
		
		newCharacterPanel = GetNode<Panel>("NewCharacterPanel");
		
		nameEdit = GetNode<LineEdit>("NewCharacterPanel/NameEdit");
		
		familiarList = GetNode<ItemList>("NewCharacterPanel/FamiliarList");
		
		familiarList.AddItem("Gnome");
		familiarList.AddItem("Salamander");
		familiarList.AddItem("Sylph");
		familiarList.AddItem("Undine");
		
		familiarList.SetItemMetadata(0, "ex_gnome");
		familiarList.SetItemMetadata(1, "ex_salamander");
		familiarList.SetItemMetadata(2, "ex_sylph");
		familiarList.SetItemMetadata(3, "ex_undine");
		
		backButton = GetNode<Button>("NewCharacterPanel/BackButton");
		confirmButton = GetNode<Button>("NewCharacterPanel/ConfirmButton");
		
		createConfirm = GetNode<ConfirmationDialog>("CreateConfirm");
		
		newButton.Pressed += OnNewPressed;
		loadButton.Pressed += OnLoadPressed;
		quitButton.Pressed += OnQuitPressed;
		
		familiarList.ItemSelected += OnFamiliarSelected;
		
		backButton.Pressed += OnBackPressed;
		confirmButton.Pressed += OnConfirmPressed;
		
		loadDialog.FileSelected += OnLoadSelected;
		quitConfirm.Confirmed += OnQuitConfirmed;
		createConfirm.Confirmed += OnCreateConfirmed;
	}
	
	public void OnNewPressed()
	{
		titlePanel.Visible = false;
		newCharacterPanel.Visible = true;
		
		nameEdit.Text = "";
		familiarList.DeselectAll();
		selectedFamiliar = null;
	}
	
	public void OnLoadPressed()
	{
		loadDialog.PopupCentered();
	}
	
	public void OnLoadSelected(string path)
	{
		session.LoadGame(path);
	}
	
	public void OnQuitPressed()
	{
		quitConfirm.PopupCentered();
	}
	
	public void OnQuitConfirmed()
	{
		GetTree().Quit();
	}
	
	public void OnFamiliarSelected(long index)
	{
		string id = familiarList.GetItemMetadata((int)index).AsString();
		
		selectedFamiliar = registry.Familiar(id);
		
		if (selectedFamiliar == null)
		{
			GD.Print($"TitleMenu: selected familiar (id {id}) is null");
		}
	}
	
	public void OnBackPressed()
	{
		newCharacterPanel.Visible = false;
		titlePanel.Visible = true;
	}
	
	public void OnConfirmPressed()
	{
		if (selectedFamiliar == null || familiarList.GetSelectedItems().Length == 0)
		{
			GD.Print("TitleMenu: No familiar selected");
			return;
		}
		
		createConfirm.PopupCentered();
	}
	
	public void OnCreateConfirmed()
	{
		session.storyFlags.Clear();
		session.openedChests.Clear();
		session.counters.Clear();
		session.activeQuests.Clear();
		session.completedQuests.Clear();
		session.qualiaCrystals.Clear();
		session.itemStacks.Clear();
		session.uniqueItems.Clear();
		session.qualiaGeneric = 0;
		
		Projector proj = new();
		proj.Initialize(registry.Projector("player"));
		proj.name = string.IsNullOrEmpty(nameEdit.Text) ? "Nanashi" : nameEdit.Text;
		
		RFamiliarInstance starter = new();
		starter.Initialize(selectedFamiliar);
		
		proj.GiveFamiliar(starter);
		
		proj.LearnSpell(registry.Spell("bolt"));
		proj.LearnSpell(registry.Spell("recover"));
		
		session.playerProjector = proj;
		
		GetTree().ChangeSceneToFile(session.SceneForMap(START_MAP));
	}
}
