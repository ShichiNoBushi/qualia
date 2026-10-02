using Godot;
using System;

public partial class MerchantMenu : CanvasLayer
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
	
	public Button closeButton;
	
	public MerchantNPC selectedMerchant;
	
	public override void _Ready()
	{
		session = GetNode<GameSession>("/root/GameSession");
		registry = GetNode<DataRegistry>("/root/DataRegistry");
		
		shopList = GetNode<ItemList>("Panel/TabContainer/Buy/ShopList");
		crystalBuyLabel = GetNode<Label>("Panel/TabContainer/Buy/CrystalsBuyLabel");
		buySpin = GetNode<SpinBox>("Panel/TabContainer/Buy/BuySpin");
		buyButton = GetNode<Button>("Panel/TabContainer/Buy/BuyButton");
		shopItemLabel = GetNode<RichTextLabel>("Panel/TabContainer/Buy/ShopItemLabel");
		
		inventoryList = GetNode<ItemList>("Panel/TabContainer/Sell/InventoryList");
		crystalSellLabel = GetNode<Label>("Panel/TabContainer/Sell/CrystalsSellLabel");
		sellSpin = GetNode<SpinBox>("Panel/TabContainer/Sell/SellSpin");
		sellButton = GetNode<Button>("Panel/TabContainer/Sell/SellButton");
		inventoryItemLabel = GetNode<RichTextLabel>("Panel/TabContainer/Sell/InventoryItemLabel");
		
		closeButton = GetNode<Button>("Panel/CloseButton");
		
		shopList.ItemSelected += OnShopSelected;
		buySpin.GuiInput += OnBuySpinGui;
		buyButton.Pressed += OnBuyPressed;
		
		inventoryList.ItemSelected += OnInventorySelected;
		sellSpin.GuiInput += OnSellSpinGui;
		sellButton.Pressed += OnSellPressed;
		
		closeButton.Pressed += OnClosePressed;
	}
	
	public void UpdateInventory()
	{
		shopList.Clear();
		inventoryList.Clear();
		
		foreach (var item in session.itemStacks)
		{
			RItemData data = registry.Item(item.Key);
			
			string label = $"{data.name} ({data.value} qc) x{item.Value}";
			
			inventoryList.AddItem(label);
			int idx = inventoryList.ItemCount - 1;
			inventoryList.SetItemMetadata(idx, item.Key);
		}
		
		crystalBuyLabel.Text = $"{session.qualiaGeneric}";
		crystalSellLabel.Text = $"{session.qualiaGeneric}";
		
		if (selectedMerchant != null && selectedMerchant.itemStock != null)
		{
			foreach (var item in selectedMerchant.itemStock)
			{
				RItemData data = registry.Item(item.Key);
				
				string label = $"{data.name} ({data.value} qc)";
				
				if (item.Value > 0)
				{
					
					label += $" x{item.Value}";
				}
				else if (item.Value == 0)
				{
					label += $" (Out of Stock)";
				}
				
				shopList.AddItem(label);
				int idx = shopList.ItemCount - 1;
				shopList.SetItemMetadata(idx, item.Key);
			}
		}
		
		buySpin.Value = 0f;
		buySpin.MaxValue = 0f;
		sellSpin.Value = 0f;
		sellSpin.MaxValue = 0f;
		
		shopItemLabel.Text = "";
		inventoryItemLabel.Text = "";
	}
	
	public void OnShopSelected(long index)
	{
		string id = shopList.GetItemMetadata((int)index).AsString();
		RItemData data = registry.Item(id);
		
		if (data == null)
		{
			shopItemLabel.Text = "Invalid item data.";
			
			buySpin.Value = 0f;
			buySpin.MaxValue = 0f;
			buyButton.Disabled = true;
			
			return;
		}
		
		shopItemLabel.Text = data.FormatDescription();
		
		if (selectedMerchant?.itemStock != null)
		{
			int stock = selectedMerchant.itemStock.TryGetValue(id, out int n) ? n : 0;
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
			
			string itemId = shopList.GetItemMetadata(selected[0]).AsString();
			
			BuyItem(itemId);
		}
	}
	
	public void OnBuyPressed()
	{
		var selected = shopList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		string itemId = shopList.GetItemMetadata(selected[0]).AsString();
		
		BuyItem(itemId);
	}
	
	public void OnInventorySelected(long index)
	{
		string id = inventoryList.GetItemMetadata((int)index).AsString();
		RItemData data = registry.Item(id);
		
		if (data == null)
		{
			inventoryItemLabel.Text = "Invalid item data.";
			
			sellSpin.Value = 0f;
			sellSpin.MaxValue = 0f;
			sellButton.Disabled = true;
			
			return;
		}
		
		inventoryItemLabel.Text = data.FormatDescription();
		
		if (session.itemStacks != null)
		{
			int maxAvailable = session.itemStacks.TryGetValue(id, out int n) && n > 0 ? n : 0;
			
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
			
			string itemId = inventoryList.GetItemMetadata(selected[0]).AsString();
			
			SellItem(itemId);
		}
	}
	
	public void OnSellPressed()
	{
		var selected = inventoryList.GetSelectedItems();
		
		if (selected.Length == 0)
		{
			return;
		}
		
		string itemId = inventoryList.GetItemMetadata(selected[0]).AsString();
		
		SellItem(itemId);
	}
	
	public void OnClosePressed()
	{
		Close();
	}
	
	public void BuyItem(string id)
	{
		RItemData data = registry.Item(id);
		
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
		
		session.AddItem(data, amount);
		
		if (selectedMerchant?.itemStock != null && selectedMerchant.itemStock.TryGetValue(id, out int n) && n >= 0)
		{
			selectedMerchant.itemStock[id] = Mathf.Max(selectedMerchant.itemStock[id] - amount, 0);
		}
		
		UpdateInventory();
	}
	
	public void SellItem(string id)
	{
		RItemData data = registry.Item(id);
		
		if (data == null)
		{
			return;
		}
		
		int amount = (int)sellSpin.Value;
		int cost = Mathf.RoundToInt(data.value * 0.5 * amount);
		
		if (!session.itemStacks.TryGetValue(id, out int n) || n < amount || amount <= 0)
		{
			return;
		}
		
		session.qualiaGeneric += cost;
		
		session.RemoveItem(data, amount);
		
		if (selectedMerchant?.itemStock != null && (!selectedMerchant.itemStock.TryGetValue(id, out int m) || m >= 0))
		{
			selectedMerchant.itemStock[id] = (selectedMerchant.itemStock.TryGetValue(id, out int o) ? o : 0) + amount;
		}
		
		UpdateInventory();
	}
	
	public void Open(MerchantNPC merchant)
	{
		selectedMerchant = merchant;
		Visible = true;
		session.gameMode = GameSession.GameMode.Shop;
		UpdateInventory();
	}
	
	public void Close()
	{
		Visible = false;
		selectedMerchant = null;
		session.gameMode = GameSession.GameMode.World;
	}
}
