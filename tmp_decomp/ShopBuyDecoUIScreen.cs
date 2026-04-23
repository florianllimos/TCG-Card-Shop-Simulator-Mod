using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyDecoUIScreen : UIScreenBase
{
	public List<ShopDecoPanelUI> m_ShopDecoPanelUIList;

	public List<GameObject> m_PageButtonHighlightList;

	public TextMeshProUGUI m_PageText;

	public Button m_NextPageBtn;

	public Button m_PreviousPageBtn;

	private bool m_IsShopWallFloorCeiling;

	private int m_CategoryIndex = -1;

	private int m_PageIndex;

	private int m_PageMaxIndex;

	private int m_MaxUICountPerPage = 8;

	private int m_CurrentEquippedShopDecoIndex;

	private int m_CurrentEquippedShopDecoIndexB;

	private List<ShopDecoData> m_CurrentShopDecoDataList = new List<ShopDecoData>();

	private List<EDecoObject> m_CurrentItemDecoList = new List<EDecoObject>();

	protected override void Start()
	{
		for (int i = 0; i < m_ShopDecoPanelUIList.Count; i++)
		{
			m_ShopDecoPanelUIList[i].InitShop(this);
		}
		base.Start();
	}

	protected override void Init()
	{
		m_CategoryIndex = -1;
		OnPressChangePageButton(0);
		base.Init();
	}

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
		if (m_CategoryIndex >= 0)
		{
			EvaluateDecoPageUI(m_CategoryIndex);
		}
	}

	public void OnPressChangePageButton(int index)
	{
		if (m_CategoryIndex != index)
		{
			m_CategoryIndex = index;
			m_PageIndex = 0;
			for (int i = 0; i < m_PageButtonHighlightList.Count; i++)
			{
				m_PageButtonHighlightList[i].SetActive(value: false);
			}
			m_PageButtonHighlightList[m_CategoryIndex].SetActive(value: true);
			EvaluateDecoPageUI(m_CategoryIndex);
		}
	}

	private void EvaluateDecoPageUI(int categoryIndex)
	{
		new List<EDecoObject>();
		switch (categoryIndex)
		{
		case 0:
			m_IsShopWallFloorCeiling = true;
			m_CurrentShopDecoDataList = CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_WallDecoDataList;
			m_CurrentEquippedShopDecoIndex = CPlayerData.m_EquippedWallDecoIndex;
			m_CurrentEquippedShopDecoIndexB = CPlayerData.m_EquippedWallDecoIndexB;
			m_PageMaxIndex = (m_CurrentShopDecoDataList.Count - 1) / m_MaxUICountPerPage;
			break;
		case 1:
			m_IsShopWallFloorCeiling = true;
			m_CurrentShopDecoDataList = CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_FloorDecoDataList;
			m_CurrentEquippedShopDecoIndex = CPlayerData.m_EquippedFloorDecoIndex;
			m_CurrentEquippedShopDecoIndexB = CPlayerData.m_EquippedFloorDecoIndexB;
			m_PageMaxIndex = (m_CurrentShopDecoDataList.Count - 1) / m_MaxUICountPerPage;
			break;
		case 2:
			m_IsShopWallFloorCeiling = true;
			m_CurrentShopDecoDataList = CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_CeilingDecoDataList;
			m_CurrentEquippedShopDecoIndex = CPlayerData.m_EquippedCeilingDecoIndex;
			m_CurrentEquippedShopDecoIndexB = CPlayerData.m_EquippedCeilingDecoIndexB;
			m_PageMaxIndex = (m_CurrentShopDecoDataList.Count - 1) / m_MaxUICountPerPage;
			break;
		case 3:
			m_IsShopWallFloorCeiling = false;
			m_CurrentItemDecoList = CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_PosterDecoList;
			m_PageMaxIndex = (m_CurrentItemDecoList.Count - 1) / m_MaxUICountPerPage;
			break;
		case 4:
			m_IsShopWallFloorCeiling = false;
			m_CurrentItemDecoList = CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_OtherDecoList;
			m_PageMaxIndex = (m_CurrentItemDecoList.Count - 1) / m_MaxUICountPerPage;
			break;
		}
		EvaluatePanelUIPage(0);
	}

	public void OnPressNextPage()
	{
		if (m_PageIndex < m_PageMaxIndex)
		{
			m_PageIndex++;
			EvaluatePanelUIPage(m_PageIndex);
		}
	}

	public void OnPressPreviousPage()
	{
		if (m_PageIndex > 0)
		{
			m_PageIndex--;
			EvaluatePanelUIPage(m_PageIndex);
		}
	}

	private void EvaluatePanelUIPage(int pageIndex)
	{
		m_PageText.text = m_PageIndex + 1 + " / " + (m_PageMaxIndex + 1);
		if (m_PageIndex == 0)
		{
			m_PreviousPageBtn.interactable = false;
			m_NextPageBtn.interactable = m_PageIndex < m_PageMaxIndex;
		}
		else if (m_PageIndex >= m_PageMaxIndex)
		{
			m_PreviousPageBtn.interactable = true;
			m_NextPageBtn.interactable = false;
		}
		else
		{
			m_PreviousPageBtn.interactable = true;
			m_NextPageBtn.interactable = true;
		}
		if (m_IsShopWallFloorCeiling)
		{
			for (int i = 0; i < m_ShopDecoPanelUIList.Count; i++)
			{
				int num = m_PageIndex * 8 + i;
				if (num >= m_CurrentShopDecoDataList.Count)
				{
					m_ShopDecoPanelUIList[i].SetActive(isActive: false);
					continue;
				}
				m_ShopDecoPanelUIList[i].InitShopWallFloorCeiling(m_CurrentShopDecoDataList[num], num);
				m_ShopDecoPanelUIList[i].ShowPurchaseDecoButtonUI();
				m_ShopDecoPanelUIList[i].EvaluateShopDecoBoughtUIState(m_CategoryIndex, num);
				m_ShopDecoPanelUIList[i].SetActive(isActive: true);
			}
			return;
		}
		for (int j = 0; j < m_ShopDecoPanelUIList.Count; j++)
		{
			int num2 = m_PageIndex * 8 + j;
			if (num2 >= m_CurrentItemDecoList.Count)
			{
				m_ShopDecoPanelUIList[j].SetActive(isActive: false);
				continue;
			}
			m_ShopDecoPanelUIList[j].InitDecoObject(InventoryBase.GetItemDecoPurchaseData(m_CurrentItemDecoList[num2]), m_CurrentItemDecoList[num2], num2);
			m_ShopDecoPanelUIList[j].ShowPurchaseDecoButtonUI();
			m_ShopDecoPanelUIList[j].EvaluateOwnedDecoItemCount();
			m_ShopDecoPanelUIList[j].SetActive(isActive: true);
		}
	}

	public void OnPressBuyShopDeco(int shopDecoIndex, float price)
	{
		if (CPlayerData.m_CoinAmountDouble < (double)price)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			return;
		}
		CPlayerData.m_GameReportDataCollect.upgradeCost -= price;
		CPlayerData.m_GameReportDataCollectPermanent.upgradeCost -= price;
		PriceChangeManager.AddTransaction(0f - price, ETransactionType.BuyDecoration, m_CategoryIndex, shopDecoIndex);
		CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(price));
		if (m_CategoryIndex == 0)
		{
			CPlayerData.SetUnlockDecoWall(shopDecoIndex, isUnlocked: true);
		}
		else if (m_CategoryIndex == 1)
		{
			CPlayerData.SetUnlockDecoFloor(shopDecoIndex, isUnlocked: true);
		}
		else if (m_CategoryIndex == 2)
		{
			CPlayerData.SetUnlockDecoCeiling(shopDecoIndex, isUnlocked: true);
		}
		for (int i = 0; i < m_ShopDecoPanelUIList.Count; i++)
		{
			m_ShopDecoPanelUIList[i].EvaluateShopDecoBoughtUIState(m_CategoryIndex, shopDecoIndex);
		}
		EvaluatePanelUIPage(m_PageIndex);
		SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
	}

	public void OnPressBuyShopDecoItem(EDecoObject itemType, float price)
	{
		if (CPlayerData.m_CoinAmountDouble < (double)price)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			return;
		}
		CPlayerData.m_GameReportDataCollect.supplyCost -= price;
		CPlayerData.m_GameReportDataCollectPermanent.supplyCost -= price;
		PriceChangeManager.AddTransaction(0f - price, ETransactionType.BuyDecoration, 3, (int)itemType);
		CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(price));
		CPlayerData.AddDecoItemToInventory(itemType, 1);
		EvaluatePanelUIPage(m_PageIndex);
		SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
	}
}
