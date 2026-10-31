using Godot;
using System;

public partial class ForgeMenu : CanvasLayer
{
	public GameSession session;
	public DataRegistry registry;
	
	public ItemList shopList;
	public Label crystalBuyLabel;
	public SpinBox buySpin;
	public Button buyButton;
	public RichTextLabel shopItemLabel;
	
	public ItemList inventoryList;
	public Label crystalSellLabel;
	public SpinBox sellSpin;
	public Button sellButton;
	public RichTextLabel inventoryItemLabel;
	
	public ItemList recipeCrList;
	public Label crystalCraftCrLabel;
	public SpinBox craftCrSpin;
	public Button craftCrButton;
	public RichTextLabel craftCrResultLabel;
	
	public ItemList recipePrList;
	public Label crystalCraftPrLabel;
	public Button craftPrButton;
	public RichTextLabel craftPrResultLabel;
	
	public ItemList breakCrList;
	public SpinBox breakCrSpin;
	public Button breakCrButton;
	public RichTextLabel breakCrResultLabel;
	
	public ItemList breakPrList;
	public Button breakPrButton;
	public RichTextLabel breakPrResultLabel;
	
	public ItemList fortInvFamList;
	
	public Panel fortStatsPanel;
	
	public Label fortNickNameLabel;
	public Label fortFamNameLabel;
	
	public RichTextLabel fortTypesLabel;
	
	public Label fortLevelLabel;
	public ProgressBar fortExperienceProgress;
	public Label fortExperienceLabel;
	public Label fortExperienceNextLabel;
	
	public Label fortEnergyLabel;
	public Label fortPhysAttackLabel;
	public Label fortMagAttackLabel;
	public Label fortPhysDefenseLabel;
	public Label fortMagDefenseLabel;
	public Label fortSpeedLabel;
	
	public RichTextLabel fortCostLabel;
	public SpinBox fortifySpin;
	public Button fortifyButton;
	public TextureRect fortFamPortraitRect;
	
	public ItemList enhInvFamList;
	
	public Panel enhStatsPanel;
	
	public Label enhNickNameLabel;
	public Label enhFamNameLabel;
	
	public RichTextLabel enhTypesLabel;
	
	public Label enhLevelLabel;
	
	public ProgressBar totalBonusProgress;
	public Label totalBonusLabel;
	public Label totalBonusMaxLabel;
	
	public Label enhEnergyLabel;
	public Label enhPhysAttackLabel;
	public Label enhPhysAttackBonusLabel;
	public ProgressBar enhPhysAttackProgress;
	public Label enhMagAttackLabel;
	public Label enhMagAttackBonusLabel;
	public ProgressBar enhMagAttackProgress;
	public Label enhPhysDefenseLabel;
	public Label enhPhysDefenseBonusLabel;
	public ProgressBar enhPhysDefenseProgress;
	public Label enhMagDefenseLabel;
	public Label enhMagDefenseBonusLabel;
	public ProgressBar enhMagDefenseProgress;
	public Label enhSpeedLabel;
	public Label enhSpeedBonusLabel;
	public ProgressBar enhSpeedProgress;
	
	public OptionButton enhCrystalMenu;
	public SpinBox enhanceSpin;
	public OptionButton enhStatMenu;
	public Button enhanceButton;
	
	public TextureRect enhFamPortraitRect;
	
	public Button closeButton;
	
	public ConfirmationDialog breakConfirm;
	
	public ForgeNPC selectedForge;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		shopList = GetNode<ItemList>("Panel/MainTab/Trade/TradeTab/Buy/ShopList");
		crystalBuyLabel = GetNode<Label>("Panel/MainTab/Trade/TradeTab/Buy/CrystalsBuyLabel");
		buySpin = GetNode<SpinBox>("Panel/MainTab/Trade/TradeTab/Buy/BuySpin");
		buyButton = GetNode<Button>("Panel/MainTab/Trade/TradeTab/Buy/BuyButton");
		shopItemLabel = GetNode<RichTextLabel>("Panel/MainTab/Trade/TradeTab/Buy/ShopItemLabel");
		
		inventoryList = GetNode<ItemList>("Panel/MainTab/Trade/TradeTab/Sell/InventoryList");
		crystalSellLabel = GetNode<Label>("Panel/MainTab/Trade/TradeTab/Sell/CrystalsSellLabel");
		sellSpin = GetNode<SpinBox>("Panel/MainTab/Trade/TradeTab/Sell/SellSpin");
		sellButton = GetNode<Button>("Panel/MainTab/Trade/TradeTab/Sell/SellButton");
		inventoryItemLabel = GetNode<RichTextLabel>("Panel/MainTab/Trade/TradeTab/Sell/InventoryItemLabel");
		
		recipeCrList = GetNode<ItemList>("Panel/MainTab/Craft/CraftTab/Crystals/RecipeCrList");
		crystalCraftCrLabel = GetNode<Label>("Panel/MainTab/Craft/CraftTab/Crystals/CrystalsCraftCrLabel");
		craftCrSpin = GetNode<SpinBox>("Panel/MainTab/Craft/CraftTab/Crystals/CraftCrSpin");
		craftCrButton = GetNode<Button>("Panel/MainTab/Craft/CraftTab/Crystals/CraftCrButton");
		craftCrResultLabel = GetNode<RichTextLabel>("Panel/MainTab/Craft/CraftTab/Crystals/CraftCrResultLabel");
		
		recipePrList = GetNode<ItemList>("Panel/MainTab/Craft/CraftTab/Prisms/RecipePrList");
		crystalCraftPrLabel = GetNode<Label>("Panel/MainTab/Craft/CraftTab/Prisms/CrystalsCraftPrLabel");
		craftPrButton = GetNode<Button>("Panel/MainTab/Craft/CraftTab/Prisms/CraftPrButton");
		craftPrResultLabel = GetNode<RichTextLabel>("Panel/MainTab/Craft/CraftTab/Prisms/CraftPrResultLabel");
		
		breakCrList = GetNode<ItemList>("Panel/MainTab/Refine/RefineTab/Crystals/BreakCrList");
		breakCrSpin = GetNode<SpinBox>("Panel/MainTab/Refine/RefineTab/Crystals/BreakCrSpin");
		breakCrButton = GetNode<Button>("Panel/MainTab/Refine/RefineTab/Crystals/BreakCrButton");
		breakCrResultLabel = GetNode<RichTextLabel>("Panel/MainTab/Refine/RefineTab/Crystals/BreakCrResultLabel");
		
		breakPrList = GetNode<ItemList>("Panel/MainTab/Refine/RefineTab/Prisms/BreakPrList");
		breakPrButton = GetNode<Button>("Panel/MainTab/Refine/RefineTab/Prisms/BreakPrButton");
		breakPrResultLabel = GetNode<RichTextLabel>("Panel/MainTab/Refine/RefineTab/Prisms/BreakPrResultLabel");
		
		fortInvFamList = GetNode<ItemList>("Panel/MainTab/Modify/ModifyTab/Fortify/FortFamTab/Inventory/FortInvFamList");
		
		fortStatsPanel = GetNode<Panel>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel");
		
		fortNickNameLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortNickNameLabel");
		fortFamNameLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortFamNameLabel");
		
		fortTypesLabel = GetNode<RichTextLabel>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortTypesLabel");
		
		fortLevelLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortLevelLabel");
		fortExperienceProgress = GetNode<ProgressBar>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortExperienceProgress");
		fortExperienceLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortExperienceLabel");
		fortExperienceNextLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortExperienceNextLabel");
		
		fortEnergyLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortGridContainer/FortEnergyLabel");
		fortPhysAttackLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortGridContainer/FortPhysAttackLabel");
		fortMagAttackLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortGridContainer/FortMagAttackLabel");
		fortPhysDefenseLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortGridContainer/FortPhysDefenseLabel");
		fortMagDefenseLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortGridContainer/FortMagDefenseLabel");
		fortSpeedLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortGridContainer/FortSpeedLabel");
		
		fortCostLabel = GetNode<RichTextLabel>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortCostLabel");
		fortifySpin = GetNode<SpinBox>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortifySpin");
		fortifyButton = GetNode<Button>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortifyButton");
		fortFamPortraitRect = GetNode<TextureRect>("Panel/MainTab/Modify/ModifyTab/Fortify/FortStatsPanel/FortFamPortraitRect");
		
		enhInvFamList = GetNode<ItemList>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhFamTab/Inventory/EnhInvFamList");
		
		enhStatsPanel = GetNode<Panel>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel");
		
		enhNickNameLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhNickNameLabel");
		enhFamNameLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhFamNameLabel");
		
		enhTypesLabel = GetNode<RichTextLabel>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhTypesLabel");
		
		enhLevelLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhLevelLabel");
		
		totalBonusProgress = GetNode<ProgressBar>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/TotalBonusProgress");
		totalBonusLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/TotalBonusLabel");
		totalBonusMaxLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/TotalBonusMaxLabel");
		
		enhEnergyLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhEnergyLabel");
		enhPhysAttackLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhPhysAttackLabel");
		enhPhysAttackBonusLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhPhysAttackBonusLabel");
		enhPhysAttackProgress = GetNode<ProgressBar>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhPhysAttackProgress");
		enhMagAttackLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhMagAttackLabel");
		enhMagAttackBonusLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhMagAttackBonusLabel");
		enhMagAttackProgress = GetNode<ProgressBar>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhMagAttackProgress");
		enhPhysDefenseLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhPhysDefenseLabel");
		enhPhysDefenseBonusLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhPhysDefenseBonusLabel");
		enhPhysDefenseProgress = GetNode<ProgressBar>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhPhysDefenseProgress");
		enhMagDefenseLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhMagDefenseLabel");
		enhMagDefenseBonusLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhMagDefenseBonusLabel");
		enhMagDefenseProgress = GetNode<ProgressBar>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhMagDefenseProgress");
		enhSpeedLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhSpeedLabel");
		enhSpeedBonusLabel = GetNode<Label>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhSpeedBonusLabel");
		enhSpeedProgress = GetNode<ProgressBar>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhGridContainer/EnhSpeedProgress");
		
		enhCrystalMenu = GetNode<OptionButton>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhCrystalMenu");
		enhanceSpin = GetNode<SpinBox>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhanceSpin");
		enhStatMenu = GetNode<OptionButton>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhStatMenu");
		enhanceButton = GetNode<Button>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhanceButton");
		
		enhFamPortraitRect = GetNode<TextureRect>("Panel/MainTab/Modify/ModifyTab/Enhance/EnhStatsPanel/EnhFamPortraitRect");
		
		closeButton = GetNode<Button>("Panel/CloseButton");
		
		breakConfirm = GetNode<ConfirmationDialog>("BreakConfirm");
		
		shopList.ItemSelected += OnShopSelected;
		buySpin.GuiInput += OnBuySpinGui;
		buyButton.Pressed += OnBuyPressed;
		
		inventoryList.ItemSelected += OnInventorySelected;
		sellSpin.GuiInput += OnSellSpinGui;
		sellButton.Pressed += OnSellPressed;
		
		recipeCrList.ItemSelected += OnRecipeCrSelected;
		craftCrSpin.GuiInput += OnCraftCrSpinGui;
		craftCrButton.Pressed += OnCraftCrPressed;
		
		recipePrList.ItemSelected += OnRecipePrSelected;
		craftPrButton.Pressed += OnCraftPrPressed;
		
		breakCrList.ItemSelected += OnBreakCrSelected;
		breakCrSpin.GuiInput += OnBreakCrSpinGui;
		breakCrButton.Pressed += OnBreakCrPressed;
		
		breakPrList.ItemSelected += OnBreakPrSelected;
		breakPrButton.Pressed += OnBreakPrPressed;
		
		fortInvFamList.ItemSelected += OnFortInvFamSelected;
		fortifySpin.GuiInput += OnFortifySpinGui;
		fortifySpin.ValueChanged += OnFortifyChanged;
		fortifyButton.Pressed += OnFortifyPressed;
		
		enhInvFamList.ItemSelected += OnEnhInvFamSelected;
		enhCrystalMenu.ItemSelected += OnEnhCrystalSelected;
		enhStatMenu.ItemSelected += OnEnhStatSelected;
		enhanceButton.Pressed += OnEnhancePressed;
		
		closeButton.Pressed += OnClosePressed;
		
		breakConfirm.Confirmed += OnBreakConfirmed;
	}
	
	public void UpdateInventory()
	{
		shopList.Clear();
		inventoryList.Clear();
		recipeCrList.Clear();
		recipePrList.Clear();
		breakCrList.Clear();
		breakPrList.Clear();
		fortInvFamList.Clear();
		enhInvFamList.Clear();
		
		foreach (var crystal in session.qualiaCrystals)
		{
			RQualiaCrystal data = registry.Crystal(crystal.Key);
			
			string label = $"{data.name} ({data.value} qc) x{crystal.Value}";
			
			inventoryList.AddItem(label);
			int idx = inventoryList.ItemCount - 1;
			inventoryList.SetItemMetadata(idx, crystal.Key);
			
			if (HasRecipe(data))
			{
				label = $"{data.name} x{crystal.Value}";
				
				breakCrList.AddItem(label);
				idx = breakCrList.ItemCount - 1;
				breakCrList.SetItemMetadata(idx, crystal.Key);
			}
		}
		
		crystalBuyLabel.Text = $"{session.qualiaGeneric}";
		crystalSellLabel.Text = $"{session.qualiaGeneric}";
		crystalCraftCrLabel.Text = $"{session.qualiaGeneric}";
		crystalCraftPrLabel.Text = $"{session.qualiaGeneric}";
		
		if (selectedForge != null)
		{
			if (selectedForge.itemStock != null)
			{
				foreach (var crystal in selectedForge.itemStock)
				{
					RQualiaCrystal data = registry.Crystal(crystal.Key);
					
					if (data == null)
					{
						continue;
					}
					
					string label = $"{data.name} ({data.value} qc)";
					
					if (crystal.Value > 0)
					{
						
						label += $" x{crystal.Value}";
					}
					else if (crystal.Value == 0)
					{
						label += $" (Out of Stock)";
					}
					
					shopList.AddItem(label);
					int idx = shopList.ItemCount - 1;
					shopList.SetItemMetadata(idx, crystal.Key);
				}
			}
			
			if (selectedForge.knownRecipes != null)
			{
				foreach (var recipe in selectedForge.knownRecipes)
				{
					RForgeRecipe data = registry.Recipe(recipe);
					
					if (data == null)
					{
						continue;
					}
					
					if (data.makesPrism)
					{
						string label = data.GetResultName(registry);
						
						recipePrList.AddItem(label);
						int idx = recipePrList.ItemCount - 1;
						recipePrList.SetItemMetadata(idx, recipe);
					}
					else
					{
						string label = data.GetResultName(registry);
						
						recipeCrList.AddItem(label);
						int idx = recipeCrList.ItemCount - 1;
						recipeCrList.SetItemMetadata(idx, recipe);
					}
				}
			}
			
			if (session.playerProjector?.ownedFamiliars != null)
			{
				for (int i = 0; i < session.playerProjector.ownedFamiliars.Count; i++)
				{
					RFamiliarInstance familiar = session.playerProjector.ownedFamiliars[i];
					
					if (familiar?.data == null || !HasRecipe(familiar.data))
					{
						continue;
					}
					
					string label = familiar.GetPreferredName();
					
					breakPrList.AddItem(label);
					int idx = breakPrList.ItemCount - 1;
					breakPrList.SetItemMetadata(idx, i);
				}
			}
		}
		
		Godot.Collections.Array<RFamiliarInstance> oFamiliars = session.playerProjector.ownedFamiliars;
		
		for (int i = 0; i < oFamiliars.Count; i++)
		{
			RFamiliarInstance fam = oFamiliars[i];
			
			if (fam == null)
			{
				continue;
			}
			
			fortInvFamList.AddItem(fam.GetPreferredName());
			int idx = fortInvFamList.ItemCount - 1;
			fortInvFamList.SetItemMetadata(idx, fam);
			
			enhInvFamList.AddItem(fam.GetPreferredName());
			idx = enhInvFamList.ItemCount - 1;
			enhInvFamList.SetItemMetadata(idx, fam);
		}
		
		buySpin.Value = 0f;
		buySpin.MaxValue = 0f;
		buyButton.Disabled = true;
		
		sellSpin.Value = 0f;
		sellSpin.MaxValue = 0f;
		sellButton.Disabled = true;
		
		craftCrSpin.Value = 0f;
		craftCrSpin.MaxValue = 0f;
		craftCrButton.Disabled = true;
		
		craftPrButton.Disabled = true;
		
		breakCrSpin.Value = 0f;
		breakCrSpin.MaxValue = 0f;
		breakCrButton.Disabled = true;
		
		breakPrButton.Disabled = true;
		
		fortStatsPanel.Visible = false;
		fortifySpin.Value = 0f;
		fortifySpin.MaxValue = 0f;
		fortifyButton.Disabled = true;
		
		enhStatsPanel.Visible = false;
		enhCrystalMenu.Clear();
		enhCrystalMenu.AddItem("(No Crystal)");
		enhCrystalMenu.Select(0);
		enhCrystalMenu.Disabled = true;
		enhStatMenu.Select(0);
		
		shopItemLabel.Text = "";
		inventoryItemLabel.Text = "";
		craftCrResultLabel.Text = "";
		craftPrResultLabel.Text = "";
	}
	
	public void UpdateFortifyRecipe()
	{
		int idx = -1;
		
		var selectedInv = fortInvFamList.GetSelectedItems();
		
		if (selectedInv.Length > 0)
		{
			idx = selectedInv[0];
		}
		
		if (idx < 0)
		{
			fortCostLabel.Text = "";
			fortifySpin.Value = 0f;
			fortifySpin.MaxValue = 0f;
			fortifyButton.Disabled = true;
			return;
		}
		
		RFamiliarInstance selectedFamiliar = fortInvFamList.GetItemMetadata(idx).As<RFamiliarInstance>();
		
		if (selectedFamiliar == null)
		{
			fortCostLabel.Text = "";
			fortifySpin.Value = 0f;
			fortifySpin.MaxValue = 0f;
			fortifyButton.Disabled = true;
			return;
		}
		
		Godot.Collections.Dictionary<string, int> fortRec = selectedFamiliar.data?.fortifyRecipe;
		
		if (fortRec == null || fortRec.Count == 0)
		{
			fortCostLabel.Text = "";
			fortifySpin.Value = 0f;
			fortifySpin.MaxValue = 0f;
			fortifyButton.Disabled = true;
			return;
		}
		
		Godot.Collections.Array<string> recText = new();
		
		foreach (var rec in fortRec)
		{
			RQualiaCrystal crystal = registry.Crystal(rec.Key);
			
			int have = session.qualiaCrystals.TryGetValue(rec.Key, out int n) ? n : 0;
			
			string text = "";
			
			if (fortifySpin.Value > 0f)
			{
				text = $"{crystal.name}: x{rec.Value * (int)fortifySpin.Value} (have: {have})";
			}
			else
			{
				text = $"{crystal.name}: x{rec.Value} (have: {have})";
			}
			
			recText.Add(text);
		}
		
		int fortCost = selectedFamiliar.data?.fortifyCost ?? 0;
		
		if (fortCost > 0 && fortifySpin.Value > 0f)
		{
			recText.Add($"Generic Qualia: {fortCost * (int)fortifySpin.Value}");
		}
		else if (fortCost > 0)
		{
			recText.Add($"Generic Qualia: {fortCost}");
		}
		
		fortCostLabel.Text = string.Join("\n", recText);
		
		int maxFort = TimesFortifiable(selectedFamiliar.data);
		
		fortifySpin.MaxValue = maxFort;
		fortifyButton.Disabled = maxFort == 0;
	}
	
	public void UpdateEnhanceRecipe()
	{
		int idx = -1;
		
		var selectedInv = enhInvFamList.GetSelectedItems();
		
		if (selectedInv.Length > 0)
		{
			idx = selectedInv[0];
		}
		
		if (idx < 0)
		{
			enhCrystalMenu.AddItem("(No crystal)");
			enhCrystalMenu.Select(0);
			enhCrystalMenu.Disabled = true;
			enhanceSpin.Value = 0f;
			enhanceSpin.MaxValue = 0f;
			enhanceButton.Disabled = true;
			return;
		}
		
		RFamiliarInstance selectedFamiliar = enhInvFamList.GetItemMetadata(idx).As<RFamiliarInstance>();
		
		if (selectedFamiliar == null)
		{
			enhCrystalMenu.AddItem("(No crystal)");
			enhCrystalMenu.Select(0);
			enhCrystalMenu.Disabled = true;
			enhanceSpin.Value = 0f;
			enhanceSpin.MaxValue = 0f;
			enhanceButton.Disabled = true;
			return;
		}
		
		int maxRec = TimesEnhanceable(selectedFamiliar);
		
		enhanceSpin.MaxValue = maxRec;
		enhanceButton.Disabled = maxRec == 0;
	}
	
	public void OnShopSelected(long index)
	{
		string id = shopList.GetItemMetadata((int)index).AsString();
		RQualiaCrystal data = registry.Crystal(id);
		
		if (data == null)
		{
			shopItemLabel.Text = "Invalid crystal data.";
			
			buySpin.Value = 0f;
			buySpin.MaxValue = 0f;
			buyButton.Disabled = true;
			
			return;
		}
		
		shopItemLabel.Text = data.FormatDescription();
		
		if (selectedForge?.itemStock != null)
		{
			int stock = selectedForge.itemStock.TryGetValue(id, out int n) ? n : 0;
			int canAfford = data.value <= 0 ? 0 : session.qualiaGeneric / data.value;
			int maxAvailable = stock < 0 ? canAfford : Mathf.Min(canAfford, stock);
			
			buySpin.Value = maxAvailable > 0 ? 1f : 0f;
			buySpin.MaxValue = maxAvailable;
			buyButton.Disabled = maxAvailable <= 0;
		}
		else
		{
			buySpin.Value = 0f;
			buySpin.MaxValue = 0f;
			buyButton.Disabled = true;
		}
	}
	
	public void OnBuySpinGui(InputEvent e)
	{
		if (e.IsActionPressed("ui_accept"))
		{
			var selected = shopList.GetSelectedItems();
			
			if (selected.Length == 0)
			{
				return;
			}
			
			string crystalId = shopList.GetItemMetadata(selected[0]).AsString();
			
			BuyCrystal(crystalId);
		}
	}
	
	public void OnBuyPressed()
	{
		var selected = shopList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		string crystalId = shopList.GetItemMetadata(selected[0]).AsString();
		
		BuyCrystal(crystalId);
	}
	
	public void OnInventorySelected(long index)
	{
		string id = inventoryList.GetItemMetadata((int)index).AsString();
		RQualiaCrystal data = registry.Crystal(id);
		
		if (data == null)
		{
			inventoryItemLabel.Text = "Invalid crystal data.";
			
			sellSpin.Value = 0f;
			sellSpin.MaxValue = 0f;
			sellButton.Disabled = true;
			
			return;
		}
		
		inventoryItemLabel.Text = data.FormatDescription();
		
		if (session.qualiaCrystals != null)
		{
			int maxAvailable = session.qualiaCrystals.TryGetValue(id, out int n) && n > 0 ? n : 0;
			
			sellSpin.Value = maxAvailable > 0 ? 1f : 0f;
			sellSpin.MaxValue = maxAvailable;
			sellButton.Disabled = maxAvailable <= 0;
		}
		else
		{
			sellSpin.Value = 0f;
			sellSpin.MaxValue = 0f;
			sellButton.Disabled = true;
		}
	}
	
	public void OnSellSpinGui(InputEvent e)
	{
		if (e.IsActionPressed("ui_accept"))
		{
			var selected = inventoryList.GetSelectedItems();
			
			if (selected.Length == 0)
			{
				return;
			}
			
			string crystalId = inventoryList.GetItemMetadata(selected[0]).AsString();
			
			SellCrystal(crystalId);
		}
	}
	
	public void OnSellPressed()
	{
		var selected = inventoryList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		string crystalId = inventoryList.GetItemMetadata(selected[0]).AsString();
		
		SellCrystal(crystalId);
	}
	
	public void OnRecipeCrSelected(long index)
	{
		string id = recipeCrList.GetItemMetadata((int)index).AsString();
		RForgeRecipe data = registry.Recipe(id);
		
		if (data == null || data.makesPrism)
		{
			craftCrResultLabel.Text = "Invalid recipe data.";
			
			craftCrSpin.Value = 0f;
			craftCrSpin.MaxValue = 0f;
			craftCrButton.Disabled = true;
			
			return;
		}
		
		craftCrResultLabel.Text = data.FormatDescription(session, registry);
		
		if (session.qualiaCrystals != null)
		{
			int maxCraftable = TimesCraftable(data);
			
			craftCrSpin.Value = maxCraftable > 0 ? 1f : 0f;
			craftCrSpin.MaxValue = maxCraftable;
			craftCrButton.Disabled = maxCraftable <= 0;
		}
		else
		{
			craftCrSpin.Value = 0f;
			craftCrSpin.MaxValue = 0f;
			craftCrButton.Disabled = true;
		}
	}
	
	public void OnCraftCrSpinGui(InputEvent e)
	{
		if (e.IsActionPressed("ui_accept"))
		{
			var selected = recipeCrList.GetSelectedItems();
			
			if (selected.Length == 0)
			{
				return;
			}
			
			string recipeId = recipeCrList.GetItemMetadata(selected[0]).AsString();
			
			CraftCrystal(recipeId);
		}
	}
	
	public void OnCraftCrPressed()
	{
		var selected = recipeCrList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		string recipeId = recipeCrList.GetItemMetadata(selected[0]).AsString();
		
		CraftCrystal(recipeId);
	}
	
	public void OnRecipePrSelected(long index)
	{
		string id = recipePrList.GetItemMetadata((int)index).AsString();
		RForgeRecipe data = registry.Recipe(id);
		
		if (data == null || !data.makesPrism)
		{
			craftPrResultLabel.Text = "Invalid recipe data.";
			
			craftPrButton.Disabled = true;
			
			return;
		}
		
		craftPrResultLabel.Text = data.FormatDescription(session, registry);
		
		if (session.qualiaCrystals != null)
		{
			int maxCraftable = TimesCraftable(data);
			
			craftPrButton.Disabled = maxCraftable <= 0 || session.playerProjector.ownedFamiliars.Count >= 10;
		}
		else
		{
			craftPrButton.Disabled = true;
		}
	}
	
	public void OnCraftPrPressed()
	{
		var selected = recipePrList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		string recipeId = recipePrList.GetItemMetadata(selected[0]).AsString();
		
		CraftPrism(recipeId);
	}
	
	public void OnBreakCrSelected(long index)
	{
		string id = breakCrList.GetItemMetadata((int)index).AsString();
		RQualiaCrystal data = registry.Crystal(id);
		
		if (data == null)
		{
			breakCrResultLabel.Text = "Invalid crystal data.";
			
			breakCrSpin.Value = 0f;
			breakCrSpin.MaxValue = 0f;
			breakCrButton.Disabled = true;
			
			return;
		}
		
		breakCrResultLabel.Text = data.FormatDescription();
		
		if (session.qualiaCrystals != null)
		{
			int maxAvailable = session.qualiaCrystals.TryGetValue(id, out int n) && n > 0 ? n : 0;
			
			breakCrSpin.Value = maxAvailable > 0 ? 1f : 0f;
			breakCrSpin.MaxValue = maxAvailable;
			breakCrButton.Disabled = maxAvailable <= 0;
		}
		else
		{
			breakCrSpin.Value = 0f;
			breakCrSpin.MaxValue = 0f;
			breakCrButton.Disabled = true;
		}
	}
	
	public void OnBreakCrSpinGui(InputEvent e)
	{
		if (e.IsActionPressed("ui_accept"))
		{
			var selected = breakCrList.GetSelectedItems();
			
			if (selected.Length == 0)
			{
				return;
			}
			
			string crystalId = breakCrList.GetItemMetadata(selected[0]).AsString();
			
			BreakCrystal(crystalId);
		}
	}
	
	public void OnBreakCrPressed()
	{
		var selected = breakCrList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		string crystalId = breakCrList.GetItemMetadata(selected[0]).AsString();
		
		BreakCrystal(crystalId);
	}
	
	public void OnBreakPrSelected(long index)
	{
		int idx = (int)breakPrList.GetItemMetadata((int)index);
		RFamiliarInstance familiar = session.playerProjector?.ownedFamiliars?[idx];
		RFamiliarData data = familiar?.data;
		
		if (familiar == null || data == null)
		{
			breakPrResultLabel.Text = "Invalid familiar data.";
			
			breakPrButton.Disabled = true;
			
			return;
		}
		
		breakPrResultLabel.Text = familiar.GetPreferredName();
		
		breakPrButton.Disabled = session.playerProjector.ownedFamiliars.Count <= 1;
	}
	
	public void OnBreakPrPressed()
	{
		breakConfirm.PopupCentered();
		
		/*var selected = breakPrList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		int idx = breakPrList.GetItemMetadata(selected[0]).AsInt32();
		Godot.Collections.Array<RFamiliarInstance> familiars = session.playerProjector?.ownedFamiliars;
		
		if (familiars == null || idx < 0 || idx >= familiars.Count)
		{
			return;
		}
		
		BreakPrism(familiars[idx]);*/
	}
	
	public void OnFortInvFamSelected(long index)
	{
		RFamiliarInstance selectedFamiliar = fortInvFamList.GetItemMetadata((int)index).As<RFamiliarInstance>();
		//deselect on storage list
		
		if (selectedFamiliar == null)
		{
			fortStatsPanel.Visible = false;
			return;
		}
		
		fortStatsPanel.Visible = true;
		
		fortNickNameLabel.Text = selectedFamiliar.GetPreferredName();
		fortFamNameLabel.Text = string.IsNullOrEmpty(selectedFamiliar.data?.name) ? "(no name)" : selectedFamiliar.data.name;
		
		Godot.Collections.Array<RTypeData> types = selectedFamiliar.types != null && selectedFamiliar.types.Count > 0 ? selectedFamiliar.types : selectedFamiliar.data?.types;
		
		if (types != null && types.Count > 0)
		{
			System.Collections.Generic.List<string> typeTexts = new();
			
			foreach (var t in types)
			{
				if (t == null)
				{
					continue;
				}
				
				string name = string.IsNullOrEmpty(t.name) ? "(type)" : t.name;
				string fill = t.color.ToHtml(false);
				string edge = t.outline.A > 0 ? t.outline.ToHtml(false) : t.ContrastColor().ToHtml(false);
				
				typeTexts.Add($"[outline_size=4][outline_color=#{edge}][color=#{fill}]{name}[/color][/outline_color][/outline_size]");
			}
			
			string typeFull = string.Join("\n", typeTexts);
			fortTypesLabel.Text = typeFull;
		}
		else
		{
			fortTypesLabel.Text = "(untyped)";
		}
		
		fortLevelLabel.Text = $"{selectedFamiliar.level}";
		
		int next = selectedFamiliar.ExpToNextLevel();
		fortExperienceProgress.Value = selectedFamiliar.experience;
		fortExperienceProgress.MaxValue = next;
		fortExperienceLabel.Text = $"{selectedFamiliar.experience}";
		fortExperienceNextLabel.Text = $"{next}";
		
		fortEnergyLabel.Text = $"{selectedFamiliar.energy}";
		fortPhysAttackLabel.Text = $"{selectedFamiliar.pAttack}";
		fortMagAttackLabel.Text = $"{selectedFamiliar.mAttack}";
		fortPhysDefenseLabel.Text = $"{selectedFamiliar.pDefense}";
		fortMagDefenseLabel.Text = $"{selectedFamiliar.mDefense}";
		fortSpeedLabel.Text = $"{selectedFamiliar.speed}";
		
		UpdateFortifyRecipe();
		
		fortFamPortraitRect.Texture = selectedFamiliar.data.portrait;
	}
	
	public void OnFortifySpinGui(InputEvent e)
	{
		if (e.IsActionPressed("ui_accept"))
		{
			var selected = fortInvFamList.GetSelectedItems();
			
			if (selected.Length == 0)
			{
				return;
			}
			
			RFamiliarInstance fam = fortInvFamList.GetItemMetadata(selected[0]).As<RFamiliarInstance>();
			
			if (fam?.data == null)
			{
				return;
			}
			
			int times = (int)fortifySpin.Value;
			
			FortifyFamiliar(fam, times);
			
			OnFortInvFamSelected(selected[0]);
		}
	}
	
	public void OnFortifyChanged(double value)
	{
		UpdateFortifyRecipe();
	}
	
	public void OnFortifyPressed()
	{
		var selected = fortInvFamList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		RFamiliarInstance fam = fortInvFamList.GetItemMetadata(selected[0]).As<RFamiliarInstance>();
		
		if (fam?.data == null)
		{
			return;
		}
		
		int times = (int)fortifySpin.Value;
		
		FortifyFamiliar(fam, times);
		
		OnFortInvFamSelected(selected[0]);
	}
	
	public void OnEnhInvFamSelected(long index)
	{
		RFamiliarInstance selectedFamiliar = enhInvFamList.GetItemMetadata((int)index).As<RFamiliarInstance>();
		//deselect on storage list
		
		if (selectedFamiliar == null)
		{
			enhStatsPanel.Visible = false;
			return;
		}
		
		enhStatsPanel.Visible = true;
		
		enhNickNameLabel.Text = selectedFamiliar.GetPreferredName();
		enhFamNameLabel.Text = string.IsNullOrEmpty(selectedFamiliar.data?.name) ? "(no name)" : selectedFamiliar.data.name;
		
		Godot.Collections.Array<RTypeData> types = selectedFamiliar.types != null && selectedFamiliar.types.Count > 0 ? selectedFamiliar.types : selectedFamiliar.data?.types;
		
		if (types != null && types.Count > 0)
		{
			System.Collections.Generic.List<string> typeTexts = new();
			
			foreach (var t in types)
			{
				if (t == null)
				{
					continue;
				}
				
				string name = string.IsNullOrEmpty(t.name) ? "(type)" : t.name;
				string fill = t.color.ToHtml(false);
				string edge = t.outline.A > 0 ? t.outline.ToHtml(false) : t.ContrastColor().ToHtml(false);
				
				typeTexts.Add($"[outline_size=4][outline_color=#{edge}][color=#{fill}]{name}[/color][/outline_color][/outline_size]");
			}
			
			string typeFull = string.Join("\n", typeTexts);
			enhTypesLabel.Text = typeFull;
		}
		else
		{
			enhTypesLabel.Text = "(untyped)";
		}
		
		enhLevelLabel.Text = $"{selectedFamiliar.level}";
		
		int maxTotal = selectedFamiliar.MaxBonusTotal();
		int maxStat = selectedFamiliar.MaxStatBonus();
		
		totalBonusProgress.Value = selectedFamiliar.TotalBonus();
		totalBonusProgress.MaxValue = maxTotal;
		totalBonusLabel.Text = $"{selectedFamiliar.TotalBonus()}";
		totalBonusMaxLabel.Text = $"{maxTotal}";
		
		enhEnergyLabel.Text = $"{selectedFamiliar.energy}";
		enhPhysAttackLabel.Text = $"{selectedFamiliar.pAttack - selectedFamiliar.pAttackBonus}";
		enhPhysAttackBonusLabel.Text = $"+{selectedFamiliar.pAttackBonus}";
		enhPhysAttackProgress.Value = selectedFamiliar.pAttackBonus;
		enhPhysAttackProgress.MaxValue = maxStat;
		enhMagAttackLabel.Text = $"{selectedFamiliar.mAttack - selectedFamiliar.mAttackBonus}";
		enhMagAttackBonusLabel.Text = $"+{selectedFamiliar.mAttackBonus}";
		enhMagAttackProgress.Value = selectedFamiliar.mAttackBonus;
		enhMagAttackProgress.MaxValue = maxStat;
		enhPhysDefenseLabel.Text = $"{selectedFamiliar.pDefense - selectedFamiliar.pDefenseBonus}";
		enhPhysDefenseBonusLabel.Text = $"+{selectedFamiliar.pDefenseBonus}";
		enhPhysDefenseProgress.Value = selectedFamiliar.pDefenseBonus;
		enhPhysDefenseProgress.MaxValue = maxStat;
		enhMagDefenseLabel.Text = $"{selectedFamiliar.mDefense - selectedFamiliar.mDefenseBonus}";
		enhMagDefenseBonusLabel.Text = $"+{selectedFamiliar.mDefenseBonus}";
		enhMagDefenseProgress.Value = selectedFamiliar.mDefenseBonus;
		enhMagDefenseProgress.MaxValue = maxStat;
		enhSpeedLabel.Text = $"{selectedFamiliar.speed - selectedFamiliar.speedBonus}";
		enhSpeedBonusLabel.Text = $"+{selectedFamiliar.speedBonus}";
		enhSpeedProgress.Value = selectedFamiliar.speedBonus;
		enhSpeedProgress.MaxValue = maxStat;
		
		enhCrystalMenu.Clear();
		
		if (selectedFamiliar.data?.enhanceCrystals != null && selectedFamiliar.data.enhanceCrystals.Count > 0)
		{
			foreach (var id in selectedFamiliar.data.enhanceCrystals)
			{
				RQualiaCrystal crystal = registry.Crystal(id);
				
				if (crystal == null)
				{
					continue;
				}
				
				enhCrystalMenu.AddItem(crystal.name);
				int idx = enhCrystalMenu.ItemCount - 1;
				enhCrystalMenu.SetItemMetadata(idx, id);
			}
			
			enhCrystalMenu.Select(0);
			OnEnhCrystalSelected(0);
			enhCrystalMenu.Disabled = false;
		}
		else
		{
			enhCrystalMenu.AddItem("(No crystal)");
			enhCrystalMenu.Select(0);
			enhCrystalMenu.Disabled = true;
		}
		
		UpdateEnhanceRecipe();
		
		enhFamPortraitRect.Texture = selectedFamiliar.data.portrait;
	}
	
	public void OnEnhCrystalSelected(long index)
	{
		string id = enhCrystalMenu.GetItemMetadata((int)index).AsString();
		
		RQualiaCrystal crystal = registry.Crystal(id);
		
		if (crystal == null)
		{
			enhanceSpin.Value = 0f;
			enhanceSpin.MaxValue = 0f;
			enhanceButton.Disabled = true;
			return;
		}
		
		var selected = enhInvFamList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			enhanceSpin.Value = 0f;
			enhanceSpin.MaxValue = 0f;
			enhanceButton.Disabled = true;
			return;
		}
		
		RFamiliarInstance selectedFamiliar = enhInvFamList.GetItemMetadata(selected[0]).As<RFamiliarInstance>();
		
		UpdateEnhanceRecipe();
	}
	
	public void OnEnhStatSelected(long index)
	{
		UpdateEnhanceRecipe();
	}
	
	public void OnEnhanceSpinGui(InputEvent e)
	{
		if (e.IsActionPressed("ui_accept"))
		{
			
		}
	}
	
	public void OnEnhancePressed()
	{
		var selected = enhInvFamList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		RFamiliarInstance fam = enhInvFamList.GetItemMetadata(selected[0]).As<RFamiliarInstance>();
		
		if (fam?.data == null)
		{
			return;
		}
		
		int times = (int)enhanceSpin.Value;
		
		EnhanceFamiliar(fam, times);
		
		OnEnhInvFamSelected(selected[0]);
	}
	
	public void OnClosePressed()
	{
		Close();
	}
	
	public void OnBreakConfirmed()
	{
		var selected = breakPrList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		int idx = breakPrList.GetItemMetadata(selected[0]).AsInt32();
		Godot.Collections.Array<RFamiliarInstance> familiars = session.playerProjector?.ownedFamiliars;
		
		if (familiars == null || idx < 0 || idx >= familiars.Count)
		{
			return;
		}
		
		BreakPrism(familiars[idx]);
	}
	
	public void BuyCrystal(string id)
	{
		RQualiaCrystal data = registry.Crystal(id);
		
		if (data == null)
		{
			return;
		}
		
		int amount = (int)buySpin.Value;
		int cost = data.value * amount;
		
		if (session.qualiaGeneric < cost)
		{
			return;
		}
		
		session.qualiaGeneric -= cost;
		
		session.AddCrystal(data, amount);
		
		if (selectedForge?.itemStock != null && selectedForge.itemStock.TryGetValue(id, out int n) && n >= 0)
		{
			selectedForge.itemStock[id] = Mathf.Max(selectedForge.itemStock[id] - amount, 0);
		}
		
		UpdateInventory();
	}
	
	public void SellCrystal(string id)
	{
		RQualiaCrystal data = registry.Crystal(id);
		
		if (data == null)
		{
			return;
		}
		
		int amount = (int)sellSpin.Value;
		
		if (amount <= 0 || data.value <= 0)
		{
			return;
		}
		
		int cost = Mathf.RoundToInt(data.value * 0.5f * amount);
		
		if (!session.qualiaCrystals.TryGetValue(id, out int n) || n < amount || amount <= 0)
		{
			return;
		}
		
		session.qualiaGeneric += cost;
		
		session.RemoveCrystal(data, amount);
		
		if (selectedForge?.itemStock != null && (!selectedForge.itemStock.TryGetValue(id, out int m) || m >= 0))
		{
			selectedForge.itemStock[id] = (selectedForge.itemStock.TryGetValue(id, out int o) ? o : 0) + amount;
		}
		
		UpdateInventory();
	}
	
	public void CraftCrystal(string id)
	{
		RForgeRecipe data = registry.Recipe(id);
		
		if (data == null || data.makesPrism)
		{
			return;
		}
		
		int amount = (int)craftCrSpin.Value;
		
		if (amount <= 0 || !CanCraft(data, amount))
		{
			return;
		}
		
		RQualiaCrystal result = registry.Crystal(data.resultId);
		
		if (result == null)
		{
			return;
		}
		
		foreach (var ing in data.ingredients)
		{
			if (ing.Value <= 0)
			{
				continue;
			}
			
			RQualiaCrystal cData = registry.Crystal(ing.Key);
			
			if (cData == null || !session.RemoveCrystal(cData, ing.Value * amount))
			{
				return;
			}
		}
		
		if (data.cost > 0)
		{
			session.qualiaGeneric -= data.cost * amount;
		}
		
		session.AddCrystal(result, amount);
		UpdateInventory();
	}
	
	public void CraftPrism(string id)
	{
		if (session.playerProjector?.ownedFamiliars == null || session.playerProjector.ownedFamiliars.Count >= 10)
		{
			return;
		}
		
		RForgeRecipe data = registry.Recipe(id);
		
		if (data == null || !data.makesPrism)
		{
			return;
		}
		
		if (!CanCraft(data, 1))
		{
			return;
		}
		
		RFamiliarData result = registry.Familiar(data.resultId);
		
		if (result == null)
		{
			return;
		}
		
		RFamiliarInstance instance = new();
		instance.Initialize(result);
		
		foreach (var ing in data.ingredients)
		{
			if (ing.Value <= 0)
			{
				continue;
			}
			
			RQualiaCrystal cData = registry.Crystal(ing.Key);
			
			if (cData == null || !session.RemoveCrystal(cData, ing.Value))
			{
				return;
			}
		}
		
		if (data.cost > 0)
		{
			session.qualiaGeneric -= data.cost;
		}
		
		if (!session.playerProjector.GiveFamiliar(instance))
		{
			return;
		}
		
		UpdateInventory();
	}
	
	public void BreakCrystal(string id)
	{
		RQualiaCrystal data = registry.Crystal(id);
		RForgeRecipe recipe = data == null ? null : GetBreakRecipe(data);
		
		int amount = (int)breakCrSpin.Value;
		
		if (recipe == null || amount <= 0)
		{
			return;
		}
		
		if (!session.qualiaCrystals.TryGetValue(id, out int n) || n < amount)
		{
			return;
		}
		
		if (!session.RemoveCrystal(data, amount))
		{
			return;
		}
		
		Godot.Collections.Dictionary<string, int> total = new();
		
		foreach (var ing in recipe.ingredients)
		{
			for (int i = 0; i < ing.Value * amount; i++)
			{
				if (GD.Randf() > 0.5f)
				{
					total[ing.Key] = (total.TryGetValue(ing.Key, out int m) ? m : 0) + 1;
				}
			}
		}
		
		System.Collections.Generic.List<string> lines = new();
		lines.Add("Total Result:");
		
		if (total.Count == 0)
		{
			lines.Add("Nothing");
		}
		
		foreach (var ing in total)
		{
			RQualiaCrystal cData = registry.Crystal(ing.Key);
			
			if (cData == null)
			{
				continue;
			}
			
			session.AddCrystal(cData, ing.Value);
			lines.Add($"{cData.name} x{ing.Value}");
		}
		
		breakCrResultLabel.Text = string.Join("\n", lines);
		UpdateInventory();
	}
	
	public void BreakPrism(RFamiliarInstance familiar)
	{
		RFamiliarData data = familiar?.data;
		RForgeRecipe recipe = data == null ? null : GetBreakRecipe(data);
		
		int level = familiar != null ? familiar.level : -1;
		
		if (recipe == null || level <= 0)
		{
			return;
		}
		
		if (session.playerProjector.RemoveFamiliar(familiar) == null)
		{
			return;
		}
		
		Godot.Collections.Dictionary<string, int> total = new();
		
		foreach (var ing in recipe.ingredients)
		{
			for (int i = 0; i < ing.Value * level; i++)
			{
				if (GD.Randf() > 0.5f)
				{
					total[ing.Key] = (total.TryGetValue(ing.Key, out int m) ? m : 0) + 1;
				}
			}
		}
		
		System.Collections.Generic.List<string> lines = new();
		lines.Add("Total Result:");
		
		if (total.Count == 0)
		{
			lines.Add("Nothing");
		}
		
		foreach (var ing in total)
		{
			RQualiaCrystal cData = registry.Crystal(ing.Key);
			
			if (cData == null)
			{
				continue;
			}
			
			session.AddCrystal(cData, ing.Value);
			lines.Add($"{cData.name} x{ing.Value}");
		}
		
		breakPrResultLabel.Text = string.Join("\n", lines);
		UpdateInventory();
	}
	
	public void FortifyFamiliar(RFamiliarInstance fam, int times)
	{
		if (fam == null || times <= 0 || times > TimesFortifiable(fam.data))
		{
			return;
		}
		
		Godot.Collections.Dictionary<string, int> fortRecipe = fam.data.fortifyRecipe;
		
		foreach (var ing in fortRecipe)
		{
			RQualiaCrystal crystal = registry.Crystal(ing.Key);
			
			if (crystal == null || !session.RemoveCrystal(crystal, ing.Value * times))
			{
				return;
			}
		}
		
		if (fam.data.fortifyCost > 0)
		{
			session.qualiaGeneric -= fam.data.fortifyCost * times;
		}
		
		int expGain = times * 100;
		
		fam.GiveExperience(expGain);
		
		UpdateInventory();
		UpdateFortifyRecipe();
	}
	
	public void EnhanceFamiliar(RFamiliarInstance fam, int times)
	{
		if (fam == null || times <= 0 || TimesEnhanceable(fam) < times)
		{
			return;
		}
		
		string crysId = enhCrystalMenu.GetItemMetadata(enhCrystalMenu.Selected).AsString();
		RQualiaCrystal crystal = registry.Crystal(crysId);
		
		if (crystal == null || !session.RemoveCrystal(crystal, times))
		{
			return;
		}
		
		switch (enhStatMenu.Selected)
		{
			case 0:
				fam.pAttackBonus += times;
				break;
			case 1:
				fam.mAttackBonus += times;
				break;
			case 2:
				fam.pDefenseBonus += times;
				break;
			case 3:
				fam.mDefenseBonus += times;
				break;
			case 4:
				fam.speedBonus += times;
				break;
		}
		
		UpdateInventory();
		UpdateEnhanceRecipe();
	}
	
	public int TimesCraftable(RForgeRecipe recipe)
	{
		if (recipe.ingredients == null || recipe.ingredients.Count == 0)
		{
			return 0;
		}
		
		int max = int.MaxValue;
		
		foreach (var ing in recipe.ingredients)
		{
			if (ing.Value <= 0)
			{
				return 0;
			}
			
			int have = session.qualiaCrystals.TryGetValue(ing.Key, out int n) ? n : 0;
			
			max = Mathf.Min(have / ing.Value, max);
		}
		
		if (recipe.cost > 0)
		{
			max = Mathf.Min(session.qualiaGeneric / recipe.cost, max);
		}
		
		return max == int.MaxValue ? 0 : max;
	}
	
	public int TimesFortifiable(RFamiliarData data)
	{
		if (data == null)
		{
			return 0;
		}
		
		if ((data.fortifyRecipe == null || data.fortifyRecipe.Count == 0) && data.fortifyCost == 0)
		{
			return 0;
		}
		
		int max = int.MaxValue;
		
		if (data.fortifyRecipe != null)
		{
			foreach (var ing in data.fortifyRecipe)
			{
				if (ing.Value <= 0)
				{
					return 0;
				}
				
				int have = session.qualiaCrystals.TryGetValue(ing.Key, out int n) ? n : 0;
				
				max = Mathf.Min(have / ing.Value, max);
			}
		}
		
		if (data.fortifyCost > 0)
		{
			max = Mathf.Min(session.qualiaGeneric / data.fortifyCost, max);
		}
		
		return max == int.MaxValue ? 0 : max;
	}
	
	public int TimesEnhanceable(RFamiliarInstance fam)
	{
		if (fam == null)
		{
			return 0;
		}
		
		string crysId = enhCrystalMenu.GetItemMetadata(enhCrystalMenu.Selected).AsString();
		RQualiaCrystal crystal = registry.Crystal(crysId);
		
		if (crystal == null)
		{
			return 0;
		}
		
		int max = int.MaxValue;
		
		int have = session.qualiaCrystals.TryGetValue(crysId, out int n) ? n : 0;
		
		max = Mathf.Min(have, max);
		
		max = Mathf.Min(fam.MaxBonusTotal() - fam.TotalBonus(), max);
		
		switch (enhStatMenu.Selected)
		{
			case 0:
				max = Mathf.Min(fam.MaxStatBonus() - fam.pAttackBonus, max);
				break;
			case 1:
				max = Mathf.Min(fam.MaxStatBonus() - fam.mAttackBonus, max);
				break;
			case 2:
				max = Mathf.Min(fam.MaxStatBonus() - fam.pDefenseBonus, max);
				break;
			case 3:
				max = Mathf.Min(fam.MaxStatBonus() - fam.mDefenseBonus, max);
				break;
			case 4:
				max = Mathf.Min(fam.MaxStatBonus() - fam.speedBonus, max);
				break;
			default:
				max = 0;
				break;
		}
		
		return max == int.MaxValue ? 0 : max;
	}
	
	public bool CanCraft(RForgeRecipe recipe, int times)
	{
		if (session.qualiaGeneric < recipe.cost * times)
		{
			return false;
		}
		
		foreach (var ing in recipe.ingredients)
		{
			int need = ing.Value * times;
			int have = session.qualiaCrystals.TryGetValue(ing.Key, out int n) ? n : 0;
			
			if (have < need)
			{
				return false;
			}
		}
		
		return true;
	}
	
	public bool HasRecipe(RQualiaCrystal crystal)
	{
		string id = crystal.id;
		
		foreach (var recipe in registry.recipes.Values)
		{
			 if (!recipe.makesPrism && recipe.resultId == id)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool HasRecipe(RFamiliarData familiar)
	{
		string id = familiar.id;
		
		foreach (var recipe in registry.recipes.Values)
		{
			if (recipe.makesPrism && recipe.resultId == id)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public RForgeRecipe GetBreakRecipe(RFamiliarData familiar)
	{
		string id = familiar.id;
		
		foreach (var recipe in registry.recipes.Values)
		{
			if (recipe.makesPrism && recipe.resultId == id)
			{
				return recipe;
			}
		}
		
		return null;
	}
	
		public RForgeRecipe GetBreakRecipe(RQualiaCrystal crystal)
	{
		string id = crystal.id;
		
		foreach (var recipe in registry.recipes.Values)
		{
			if (!recipe.makesPrism && recipe.resultId == id)
			{
				return recipe;
			}
		}
		
		return null;
	}
	
	public void Open(ForgeNPC forge)
	{
		selectedForge = forge;
		Visible = true;
		session.gameMode = GameSession.GameMode.Shop;
		UpdateInventory();
	}
	
	public void Close()
	{
		Visible = false;
		selectedForge = null;
		session.gameMode = GameSession.GameMode.World;
	}
}
