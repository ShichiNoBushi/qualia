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
	
	public Button closeButton;
	
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
		
		closeButton = GetNode<Button>("Panel/CloseButton");
		
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
		
		closeButton.Pressed += OnClosePressed;
	}
	
	public void UpdateInventory()
	{
		shopList.Clear();
		inventoryList.Clear();
		recipeCrList.Clear();
		recipePrList.Clear();
		breakCrList.Clear();
		breakPrList.Clear();
		
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
		
		shopItemLabel.Text = "";
		inventoryItemLabel.Text = "";
		craftCrResultLabel.Text = "";
		craftPrResultLabel.Text = "";
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
			inventoryItemLabel.Text = "Invalid recipe data.";
			
			craftCrSpin.Value = 0f;
			craftCrSpin.MaxValue = 0f;
			craftCrButton.Disabled = true;
			
			return;
		}
		
		craftCrResultLabel.Text = data.FormatDescription(registry);
		
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
			inventoryItemLabel.Text = "Invalid recipe data.";
			
			craftCrButton.Disabled = true;
			
			return;
		}
		
		craftPrResultLabel.Text = data.FormatDescription(registry);
		
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
			breakCrResultLabel.Text = "Invalid familiar data.";
			
			breakCrButton.Disabled = true;
			
			return;
		}
		
		breakPrResultLabel.Text = familiar.GetPreferredName();
		
		breakPrButton.Disabled = session.playerProjector.ownedFamiliars.Count <= 1;
	}
	
	public void OnBreakPrPressed()
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
	
	public void OnClosePressed()
	{
		Close();
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
