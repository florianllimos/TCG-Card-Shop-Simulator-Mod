using TMPro;
using UnityEngine;

public class RestockCheckoutItemBar : UIElementBase
{
	public Animation m_AnimGrp;

	public TextMeshProUGUI m_ItemNameText;

	public TextMeshProUGUI m_AmountText;

	public TextMeshProUGUI m_UnitPriceText;

	public TextMeshProUGUI m_TotalPriceText;

	private int m_Index;

	private int m_BoxCount;

	private int m_TotalItemUnitCount;

	private float m_UnitPrice;

	private float m_TotalPrice;

	private EItemType m_ItemType;

	private RestockItemScreen m_RestockItemScreen;

	private ScannerRestockScreen m_ScannerRestockScreen;

	public void Init(RestockItemScreen restockItemScreen)
	{
		m_RestockItemScreen = restockItemScreen;
	}

	public void InitScannerRestock(ScannerRestockScreen scannerRestockScreen)
	{
		m_ScannerRestockScreen = scannerRestockScreen;
	}

	public void UpdateData(int index, int boxCount)
	{
		m_Index = index;
		m_BoxCount = boxCount;
		RestockData restockData = InventoryBase.GetRestockData(index);
		m_ItemType = restockData.itemType;
		ItemData itemData = InventoryBase.GetItemData(m_ItemType);
		int maxItemCountInBox = RestockManager.GetMaxItemCountInBox(m_ItemType, restockData.isBigBox);
		m_TotalItemUnitCount = m_BoxCount * maxItemCountInBox;
		m_UnitPrice = CPlayerData.GetItemCost(m_ItemType);
		m_TotalPrice = m_UnitPrice * (float)m_TotalItemUnitCount;
		m_ItemNameText.text = itemData.GetName() + " (" + maxItemCountInBox + ")";
		m_AmountText.text = m_BoxCount.ToString();
		m_UnitPriceText.text = GameInstance.GetPriceString(m_UnitPrice);
		m_TotalPriceText.text = GameInstance.GetPriceString(m_TotalPrice);
	}

	public void HighlightUIBar()
	{
		m_AnimGrp.Play("HighlightCheckoutItemBar");
	}

	public float GetUnitPrice()
	{
		return m_UnitPrice;
	}

	public float GetTotalPrice()
	{
		return m_TotalPrice;
	}

	public int GetTotalUnit()
	{
		return m_TotalItemUnitCount;
	}

	public EItemType GetItemType()
	{
		return m_ItemType;
	}

	public void OnPressAddItem()
	{
		if ((bool)m_ScannerRestockScreen)
		{
			m_ScannerRestockScreen.AddToCartForCheckout(m_Index, 1);
		}
		else
		{
			m_RestockItemScreen.AddToCartForCheckout(m_Index, 1);
		}
	}

	public void OnPressRemoveItem()
	{
		if ((bool)m_ScannerRestockScreen)
		{
			m_ScannerRestockScreen.RemoveFromCartForCheckout(m_Index, 1);
		}
		else
		{
			m_RestockItemScreen.RemoveFromCartForCheckout(m_Index, 1);
		}
	}

	public void OnPressClearItem()
	{
		if ((bool)m_ScannerRestockScreen)
		{
			m_ScannerRestockScreen.RemoveFromCartForCheckout(m_Index, m_BoxCount);
		}
		else
		{
			m_RestockItemScreen.RemoveFromCartForCheckout(m_Index, m_BoxCount);
		}
	}
}
