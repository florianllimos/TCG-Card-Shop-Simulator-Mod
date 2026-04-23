using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestockItemScreen : GenericSliderScreen
{
	public List<RestockItemPanelUI> m_RestockItemPanelUIList;

	public List<GameObject> m_PageButtonHighlightList;

	public RestockItemAddToCartScreen m_RestockItemAddToCartScreen;

	public RestockItemCheckoutScreen m_RestockItemCheckoutScreen;

	public UIScreenBase m_SortUIScreen;

	public List<ControllerButton> m_SortBtnList;

	public TextMeshProUGUI m_TotalCostText;

	public TextMeshProUGUI m_TotalCartItemCountText;

	public GameObject m_CartNotification;

	protected int m_PageIndex = -1;

	protected float m_TotalCost;

	protected Dictionary<int, int> m_CartItemList = new Dictionary<int, int>();

	protected List<int> m_CurrentRestockDataIndexList = new List<int>();

	protected List<int> m_SortedRestockDataIndexList = new List<int>();

	protected List<float> m_SortTempValue = new List<float>();

	protected ERestockSortingType m_CurrentSortingMethod;

	protected override void Awake()
	{
		base.Awake();
		m_CartNotification.SetActive(value: false);
	}

	protected override void Init()
	{
		m_CurrentSortingMethod = CPlayerData.m_RestockSortingType;
		m_TotalCostText.text = GameInstance.GetPriceString(m_TotalCost);
		m_PageIndex = -1;
		EvaluateRestockItemPanelUI(0);
		base.Init();
	}

	protected virtual void EvaluateRestockItemPanelUI(int pageIndex)
	{
		if (m_PageIndex == pageIndex)
		{
			return;
		}
		m_PageIndex = pageIndex;
		for (int i = 0; i < m_PageButtonHighlightList.Count; i++)
		{
			m_PageButtonHighlightList[i].SetActive(value: false);
		}
		m_PageButtonHighlightList[m_PageIndex].SetActive(value: true);
		List<EItemType> list = new List<EItemType>();
		switch (pageIndex)
		{
		case 0:
			list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType;
			break;
		case 1:
			list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAccessoryItemType;
			break;
		case 2:
			list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownFigurineItemType;
			break;
		case 3:
			list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType;
			break;
		}
		for (int j = 0; j < m_RestockItemPanelUIList.Count; j++)
		{
			m_RestockItemPanelUIList[j].SetActive(isActive: false);
		}
		m_CurrentRestockDataIndexList.Clear();
		for (int k = 0; k < list.Count; k++)
		{
			for (int l = 0; l < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; l++)
			{
				if (list[k] == CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[l].itemType)
				{
					m_CurrentRestockDataIndexList.Add(l);
				}
			}
		}
		EvaluateSorting();
		for (int m = 0; m < m_SortedRestockDataIndexList.Count && m < m_RestockItemPanelUIList.Count; m++)
		{
			m_RestockItemPanelUIList[m].Init(this, m_SortedRestockDataIndexList[m]);
			m_RestockItemPanelUIList[m].SetActive(isActive: true);
			m_ScrollEndPosParent = m_RestockItemPanelUIList[m].gameObject;
		}
	}

	protected void EvaluateSorting()
	{
		m_SortTempValue.Clear();
		m_SortedRestockDataIndexList.Clear();
		if (m_CurrentSortingMethod == ERestockSortingType.LicenseLevelHighToLow)
		{
			for (int i = 0; i < m_CurrentRestockDataIndexList.Count; i++)
			{
				int licenseShopLevelRequired = InventoryBase.GetRestockData(m_CurrentRestockDataIndexList[i]).licenseShopLevelRequired;
				int index = m_SortTempValue.Count;
				for (int j = 0; j < m_SortTempValue.Count; j++)
				{
					if ((float)licenseShopLevelRequired > m_SortTempValue[j])
					{
						index = j;
						break;
					}
				}
				m_SortTempValue.Insert(index, licenseShopLevelRequired);
				m_SortedRestockDataIndexList.Insert(index, m_CurrentRestockDataIndexList[i]);
			}
		}
		else if (m_CurrentSortingMethod == ERestockSortingType.LicenseLevelLowToHigh)
		{
			for (int k = 0; k < m_CurrentRestockDataIndexList.Count; k++)
			{
				int licenseShopLevelRequired2 = InventoryBase.GetRestockData(m_CurrentRestockDataIndexList[k]).licenseShopLevelRequired;
				int index2 = m_SortTempValue.Count;
				for (int l = 0; l < m_SortTempValue.Count; l++)
				{
					if ((float)licenseShopLevelRequired2 < m_SortTempValue[l])
					{
						index2 = l;
						break;
					}
				}
				m_SortTempValue.Insert(index2, licenseShopLevelRequired2);
				m_SortedRestockDataIndexList.Insert(index2, m_CurrentRestockDataIndexList[k]);
			}
		}
		else if (m_CurrentSortingMethod == ERestockSortingType.CostLowToHigh)
		{
			for (int m = 0; m < m_CurrentRestockDataIndexList.Count; m++)
			{
				RestockData restockData = InventoryBase.GetRestockData(m_CurrentRestockDataIndexList[m]);
				float num = restockData.licensePrice;
				if (CPlayerData.GetIsItemLicenseUnlocked(m_CurrentRestockDataIndexList[m]))
				{
					num = CPlayerData.GetItemCost(restockData.itemType) * (float)RestockManager.GetMaxItemCountInBox(restockData.itemType, restockData.isBigBox);
				}
				int index3 = m_SortTempValue.Count;
				for (int n = 0; n < m_SortTempValue.Count; n++)
				{
					if (num < m_SortTempValue[n])
					{
						index3 = n;
						break;
					}
				}
				m_SortTempValue.Insert(index3, num);
				m_SortedRestockDataIndexList.Insert(index3, m_CurrentRestockDataIndexList[m]);
			}
		}
		else if (m_CurrentSortingMethod == ERestockSortingType.CostHighToLow)
		{
			for (int num2 = 0; num2 < m_CurrentRestockDataIndexList.Count; num2++)
			{
				RestockData restockData2 = InventoryBase.GetRestockData(m_CurrentRestockDataIndexList[num2]);
				float num3 = restockData2.licensePrice;
				if (CPlayerData.GetIsItemLicenseUnlocked(m_CurrentRestockDataIndexList[num2]))
				{
					num3 = CPlayerData.GetItemCost(restockData2.itemType) * (float)RestockManager.GetMaxItemCountInBox(restockData2.itemType, restockData2.isBigBox);
				}
				int index4 = m_SortTempValue.Count;
				for (int num4 = 0; num4 < m_SortTempValue.Count; num4++)
				{
					if (num3 > m_SortTempValue[num4])
					{
						index4 = num4;
						break;
					}
				}
				m_SortTempValue.Insert(index4, num3);
				m_SortedRestockDataIndexList.Insert(index4, m_CurrentRestockDataIndexList[num2]);
			}
		}
		else
		{
			for (int num5 = 0; num5 < m_CurrentRestockDataIndexList.Count; num5++)
			{
				m_SortedRestockDataIndexList.Add(m_CurrentRestockDataIndexList[num5]);
			}
		}
	}

	public void OpenSortScreen()
	{
		for (int i = 0; i < m_SortBtnList.Count; i++)
		{
			m_SortBtnList[i].SetBGHighlightVisibility(isVisible: false);
		}
		m_SortBtnList[(int)m_CurrentSortingMethod].SetBGHighlightVisibility(isVisible: true);
		m_SortUIScreen.OpenScreen();
	}

	public void CloseSortScreen()
	{
		m_SortUIScreen.CloseScreen();
	}

	public void OnPressSortButton(int index)
	{
		m_CurrentSortingMethod = (ERestockSortingType)index;
		CPlayerData.m_RestockSortingType = m_CurrentSortingMethod;
		EvaluateSorting();
		for (int i = 0; i < m_SortedRestockDataIndexList.Count && i < m_RestockItemPanelUIList.Count; i++)
		{
			m_RestockItemPanelUIList[i].Init(this, m_SortedRestockDataIndexList[i]);
			m_RestockItemPanelUIList[i].SetActive(isActive: true);
			m_ScrollEndPosParent = m_RestockItemPanelUIList[i].gameObject;
		}
		StartCoroutine(EvaluateActiveRestockUIScroller());
		m_PosX = 0f;
		m_LerpPosX = 0f;
		CloseSortScreen();
	}

	protected override void OnOpenScreen()
	{
		Init();
		base.OnOpenScreen();
	}

	public void OnPressChangePageButton(int index)
	{
		EvaluateRestockItemPanelUI(index);
		StartCoroutine(EvaluateActiveRestockUIScroller());
		m_PosX = 0f;
		m_LerpPosX = 0f;
	}

	public void EvaluateCartCheckout(float totalCost)
	{
		CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(totalCost));
		CPlayerData.m_GameReportDataCollect.supplyCost -= totalCost;
		CPlayerData.m_GameReportDataCollectPermanent.supplyCost -= totalCost;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		foreach (KeyValuePair<int, int> cartItem in m_CartItemList)
		{
			list.Add(cartItem.Key);
			list2.Add(cartItem.Value);
		}
		CSingleton<CGameManager>.Instance.m_CanRunDebugString = true;
		CPlayerData.m_DebugString = "indexList.Count" + list.Count;
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			RestockManager.SpawnPackageBoxItemMultipleFrame(list[i], list2[i]);
			num += list2[i];
		}
		m_CartItemList.Clear();
		m_RestockItemCheckoutScreen.UpdateData(this, m_CartItemList, hasItemRemoved: false);
		StartCoroutine(DelaySaveShelfData());
		CEventManager.QueueEvent(new CEventPlayer_AddShopExp(num * 5));
		TutorialManager.AddTaskValue(ETutorialTaskCondition.RestockItem, num);
		SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
	}

	private IEnumerator DelaySaveShelfData()
	{
		yield return new WaitForSeconds(1f);
		CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
	}

	private void SpawnPackageItemBox(EItemType itemType, bool isBigBox, int boxCount)
	{
		for (int i = 0; i < boxCount; i++)
		{
			if (isBigBox)
			{
				RestockManager.SpawnPackageBoxItem(itemType, 64, isBigBox: true);
			}
			else
			{
				RestockManager.SpawnPackageBoxItem(itemType, 32, isBigBox: false);
			}
		}
	}

	public void OnPressAddToCartButton(int index)
	{
		m_RestockItemAddToCartScreen.UpdateData(this, index);
		OpenChildScreen(m_RestockItemAddToCartScreen);
	}

	public bool HasEnoughCartSlot()
	{
		if (m_CartItemList.Count >= m_RestockItemCheckoutScreen.m_RestockCheckoutItemBarUIList.Count)
		{
			return false;
		}
		return true;
	}

	public void AddToCartForCheckout(int index, int boxCount)
	{
		if (m_CartItemList.ContainsKey(index))
		{
			m_CartItemList[index] += boxCount;
		}
		else
		{
			m_CartItemList.Add(index, boxCount);
		}
		m_RestockItemCheckoutScreen.UpdateData(this, m_CartItemList, hasItemRemoved: false);
	}

	public void RemoveFromCartForCheckout(int index, int boxCount)
	{
		bool hasItemRemoved = false;
		if (m_CartItemList.ContainsKey(index))
		{
			m_CartItemList[index] -= boxCount;
			if (m_CartItemList[index] <= 0)
			{
				hasItemRemoved = true;
				m_CartItemList.Remove(index);
			}
		}
		m_RestockItemCheckoutScreen.UpdateData(this, m_CartItemList, hasItemRemoved);
	}

	public int GetBoxCountInCart(int index)
	{
		if (m_CartItemList.ContainsKey(index))
		{
			return m_CartItemList[index];
		}
		return 0;
	}

	public void OnPressCheckoutButton()
	{
		m_RestockItemCheckoutScreen.UpdateData(this, m_CartItemList, hasItemRemoved: false);
		OpenChildScreen(m_RestockItemCheckoutScreen);
	}

	public void UpdateCartTotalCost(float cost, int count)
	{
		m_TotalCost = cost;
		m_TotalCostText.text = GameInstance.GetPriceString(m_TotalCost);
		m_TotalCartItemCountText.text = count.ToString();
		if (count > 0)
		{
			m_CartNotification.SetActive(value: true);
		}
		else
		{
			m_CartNotification.SetActive(value: false);
		}
	}

	public RestockItemPanelUI GetRestockItemPanelUI(int index)
	{
		for (int i = 0; i < m_RestockItemPanelUIList.Count; i++)
		{
			if (m_RestockItemPanelUIList[i].GetIndex() == (float)index)
			{
				return m_RestockItemPanelUIList[i];
			}
		}
		return m_RestockItemPanelUIList[0];
	}
}
