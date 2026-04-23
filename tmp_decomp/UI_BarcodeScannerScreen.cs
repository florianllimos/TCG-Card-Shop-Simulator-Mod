using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BarcodeScannerScreen : MonoBehaviour
{
	public GameObject m_ScreenGrp;

	public Image m_ItemImage;

	public Animation m_ScannerItemPopupAnim;

	public TextMeshProUGUI m_ItemNameText;

	public TextMeshProUGUI m_OrderAmountText;

	public TextMeshProUGUI m_OnShelfAmountText;

	public TextMeshProUGUI m_InStorageAmountText;

	public TextMeshProUGUI m_PriceText;

	private EItemType m_CurrentItemType = EItemType.None;

	private int m_RestockIndex = -1;

	private int shelfItemCount;

	private int shelfItemCountMax;

	private int totalWarehouseShelfItemCount;

	private int totalWarehouseShelfItemCountMax;

	private void Start()
	{
		m_ItemImage.enabled = false;
		m_ItemNameText.text = "";
		m_OrderAmountText.text = "-";
		m_OnShelfAmountText.text = "-";
		m_InStorageAmountText.text = "-";
		m_PriceText.text = "-";
	}

	public void UpdateScannerUI(EItemType itemType, bool isBigBox, bool isWarehouseShelf)
	{
		if (m_CurrentItemType == itemType)
		{
			return;
		}
		m_CurrentItemType = itemType;
		if (itemType == EItemType.None)
		{
			m_ItemImage.enabled = false;
			m_ItemNameText.text = "";
			m_OrderAmountText.text = "-";
			m_OnShelfAmountText.text = "-";
			m_InStorageAmountText.text = "-";
			m_PriceText.text = "-";
			m_RestockIndex = -1;
			return;
		}
		m_ScannerItemPopupAnim.Rewind();
		m_ScannerItemPopupAnim.Play();
		ItemData itemData = InventoryBase.GetItemData(itemType);
		m_ItemNameText.text = itemData.GetName();
		m_ItemImage.sprite = itemData.icon;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		num2 = RestockManager.GetMaxItemCountInBox(itemType, isBigBox: false);
		num = RestockManager.GetMaxItemCountInBox(itemType, isBigBox: true);
		List<Shelf> shelfList = CSingleton<ShelfManager>.Instance.m_ShelfList;
		shelfItemCount = 0;
		shelfItemCountMax = 0;
		for (int i = 0; i < shelfList.Count; i++)
		{
			if (!shelfList[i].IsValidObject() || shelfList[i].m_ItemNotForSale)
			{
				continue;
			}
			for (int j = 0; j < shelfList[i].GetItemCompartmentList().Count; j++)
			{
				if (shelfList[i].GetItemCompartmentList()[j].GetItemType() == itemType)
				{
					shelfItemCount += shelfList[i].GetItemCompartmentList()[j].GetItemCount();
					shelfItemCountMax += shelfList[i].GetItemCompartmentList()[j].GetMaxItemCount();
				}
			}
		}
		List<WarehouseShelf> warehouseShelfList = CSingleton<ShelfManager>.Instance.m_WarehouseShelfList;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		for (int k = 0; k < warehouseShelfList.Count; k++)
		{
			if (!warehouseShelfList[k].IsValidObject())
			{
				continue;
			}
			for (int l = 0; l < warehouseShelfList[k].GetStorageCompartmentList().Count; l++)
			{
				if (warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().GetItemType() != itemType)
				{
					continue;
				}
				if (warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().CheckBoxType(isBigBox: true))
				{
					for (int m = 0; m < warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().GetInteractablePackagingBoxList().Count; m++)
					{
						num4 += warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().GetInteractablePackagingBoxList()[m].m_ItemCompartment.GetItemCount();
					}
					num5 += warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().GetMaxItemCount();
				}
				else
				{
					for (int n = 0; n < warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().GetInteractablePackagingBoxList().Count; n++)
					{
						num6 += warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().GetInteractablePackagingBoxList()[n].m_ItemCompartment.GetItemCount();
					}
					num7 += warehouseShelfList[k].GetStorageCompartmentList()[l].GetShelfCompartment().GetMaxItemCount();
				}
			}
		}
		totalWarehouseShelfItemCount = num6 + num4;
		totalWarehouseShelfItemCountMax = num7 * num2 + num5 * num;
		EvaluateShelfAndStorageCount();
		if (isWarehouseShelf)
		{
			num3 = ((!isBigBox) ? num2 : num);
			List<RestockData> restockDataUsingItemType = InventoryBase.GetRestockDataUsingItemType(itemType);
			for (int num8 = 0; num8 < restockDataUsingItemType.Count; num8++)
			{
				if (CPlayerData.GetIsItemLicenseUnlocked(restockDataUsingItemType[num8].index))
				{
					if (restockDataUsingItemType[num8].isBigBox && isBigBox)
					{
						m_RestockIndex = restockDataUsingItemType[num8].index;
						num3 = num;
						break;
					}
					if (!restockDataUsingItemType[num8].isBigBox && !isBigBox)
					{
						m_RestockIndex = restockDataUsingItemType[num8].index;
						num3 = num2;
						break;
					}
					m_RestockIndex = restockDataUsingItemType[num8].index;
					num3 = num2;
				}
			}
		}
		else
		{
			List<RestockData> restockDataUsingItemType2 = InventoryBase.GetRestockDataUsingItemType(itemType);
			for (int num9 = 0; num9 < restockDataUsingItemType2.Count; num9++)
			{
				if (CPlayerData.GetIsItemLicenseUnlocked(restockDataUsingItemType2[num9].index))
				{
					if (restockDataUsingItemType2[num9].isBigBox)
					{
						m_RestockIndex = restockDataUsingItemType2[num9].index;
						num3 = num;
						break;
					}
					m_RestockIndex = restockDataUsingItemType2[num9].index;
					num3 = num2;
				}
			}
		}
		m_OrderAmountText.text = num3.ToString();
		m_PriceText.text = GameInstance.GetPriceString(CPlayerData.GetItemCost(itemType) * (float)num3);
		m_ItemImage.enabled = true;
	}

	public void EvaluateShelfAndStorageCount()
	{
		bool flag = false;
		int itemCountInCart = CSingleton<PhoneManager>.Instance.m_ScannerRestockScreen.GetItemCountInCart(m_CurrentItemType);
		if (itemCountInCart > 0)
		{
			int num = shelfItemCount + itemCountInCart - shelfItemCountMax;
			if (shelfItemCount >= shelfItemCountMax)
			{
				num = itemCountInCart;
				flag = true;
			}
			if (num > 0)
			{
				m_OnShelfAmountText.text = "<color=#FFE800>" + shelfItemCountMax + "</color>/" + shelfItemCountMax;
				m_InStorageAmountText.text = "<color=#FFE800>" + (totalWarehouseShelfItemCount + num) + "</color>/" + totalWarehouseShelfItemCountMax;
			}
			else
			{
				m_OnShelfAmountText.text = "<color=#FFE800>" + (shelfItemCount + itemCountInCart) + "</color>/" + shelfItemCountMax;
				m_InStorageAmountText.text = totalWarehouseShelfItemCount + "/" + totalWarehouseShelfItemCountMax;
			}
			if (flag || shelfItemCountMax == 0)
			{
				m_OnShelfAmountText.text = shelfItemCountMax + "/" + shelfItemCountMax;
			}
		}
		else
		{
			m_OnShelfAmountText.text = shelfItemCount + "/" + shelfItemCountMax;
			m_InStorageAmountText.text = totalWarehouseShelfItemCount + "/" + totalWarehouseShelfItemCountMax;
		}
	}

	public int GetRestockIndex()
	{
		return m_RestockIndex;
	}
}
