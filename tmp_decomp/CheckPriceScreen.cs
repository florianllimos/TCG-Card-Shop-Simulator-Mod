using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckPriceScreen : GenericSliderScreen
{
	public List<CheckPricePanelUI> m_CheckPricePanelUIList;

	public List<GameObject> m_PageButtonHighlightList;

	public ItemPriceGraphScreen m_ItemPriceGraphScreen;

	public GameObject m_CardPageOptionGrp;

	public GameObject m_ScrollEndParent;

	public Transform m_CardScrollOffsetPosStart;

	public Transform m_CardScrollOffsetPosEnd;

	public TextMeshProUGUI m_PageText;

	public TextMeshProUGUI m_CardExpansionText;

	public Color m_PositiveColor;

	public Color m_NegativeColor;

	public Color m_NeutralColor;

	private int m_PageIndex = -1;

	private int m_CardPageIndex;

	private int m_CardPageMaxIndex;

	public int m_MaxCardUICountPerPage = 12;

	private ECardExpansionType m_CurrentExpansionType;

	protected override void Init()
	{
		m_PageIndex = -1;
		m_CardPageOptionGrp.SetActive(value: false);
		OnPressChangePageButton(0);
		base.Init();
	}

	public void OnPressChangePageButton(int index)
	{
		if (m_PageIndex != index)
		{
			m_PageIndex = index;
			for (int i = 0; i < m_PageButtonHighlightList.Count; i++)
			{
				m_PageButtonHighlightList[i].SetActive(value: false);
			}
			m_PageButtonHighlightList[m_PageIndex].SetActive(value: true);
			if (index < 5)
			{
				EvaluateItemPanelUI(index);
			}
			else
			{
				EvaluateCardPanelUI(m_CardPageIndex);
			}
			StartCoroutine(EvaluateActiveRestockUIScroller());
			m_PosX = 0f;
			m_LerpPosX = 0f;
		}
	}

	private void EvaluateItemPanelUI(int pageIndex)
	{
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
		case 4:
		{
			for (int m = 0; m < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType.Count; m++)
			{
				if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType[m] != EItemType.None)
				{
					list.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType[m]);
				}
			}
			break;
		}
		case 3:
		{
			for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Count; i++)
			{
				list.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType[i]);
			}
			for (int j = 0; j < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAccessoryItemType.Count; j++)
			{
				list.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAccessoryItemType[j]);
			}
			for (int k = 0; k < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownFigurineItemType.Count; k++)
			{
				list.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownFigurineItemType[k]);
			}
			for (int l = 0; l < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType.Count; l++)
			{
				if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType[l] != EItemType.None)
				{
					list.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType[l]);
				}
			}
			break;
		}
		}
		for (int n = 0; n < m_CheckPricePanelUIList.Count; n++)
		{
			m_CheckPricePanelUIList[n].SetActive(isActive: false);
		}
		for (int num = 0; num < list.Count && num < m_CheckPricePanelUIList.Count; num++)
		{
			m_CheckPricePanelUIList[num].InitItem(this, list[num]);
			m_CheckPricePanelUIList[num].SetActive(isActive: true);
			m_ScrollEndParent.transform.parent = m_CheckPricePanelUIList[num].transform;
			m_ScrollEndParent.transform.position = m_CheckPricePanelUIList[num].transform.position;
			m_ScrollEndPosParent = m_ScrollEndParent;
		}
		m_CardPageOptionGrp.SetActive(value: false);
	}

	public void OnPressNextCardPage()
	{
		if (m_CardPageIndex < m_CardPageMaxIndex)
		{
			m_CardPageIndex++;
			EvaluateCardPanelUI(m_CardPageIndex);
		}
	}

	public void OnPressPreviousCardPage()
	{
		if (m_CardPageIndex > 0)
		{
			m_CardPageIndex--;
			EvaluateCardPanelUI(m_CardPageIndex);
		}
	}

	public void OnPressNext10CardPage()
	{
		if (m_CardPageIndex < m_CardPageMaxIndex)
		{
			m_CardPageIndex += 10;
			if (m_CardPageIndex > m_CardPageMaxIndex)
			{
				m_CardPageIndex = m_CardPageMaxIndex;
			}
			EvaluateCardPanelUI(m_CardPageIndex);
		}
	}

	public void OnPressPrevious10CardPage()
	{
		if (m_CardPageIndex > 0)
		{
			m_CardPageIndex -= 10;
			if (m_CardPageIndex < 0)
			{
				m_CardPageIndex = 0;
			}
			EvaluateCardPanelUI(m_CardPageIndex);
		}
	}

	public void OnPressChangeCardExpansion()
	{
		SoundManager.GenericMenuOpen();
		CardExpansionSelectScreen.OpenScreen(m_CurrentExpansionType);
		CEventManager.AddListener<CEventPlayer_OnCardExpansionSelectScreenUpdated>(OnCardExpansionUpdated);
	}

	protected void OnCardExpansionUpdated(CEventPlayer_OnCardExpansionSelectScreenUpdated evt)
	{
		m_CurrentExpansionType = (ECardExpansionType)evt.m_CardExpansionTypeIndex;
		m_CardExpansionText.text = InventoryBase.GetCardExpansionName(m_CurrentExpansionType);
		m_CardPageIndex = 0;
		m_CardPageMaxIndex = InventoryBase.GetShownMonsterList(m_CurrentExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_CurrentExpansionType) / m_MaxCardUICountPerPage - 1;
		if (m_CurrentExpansionType == ECardExpansionType.Ghost)
		{
			m_CardPageMaxIndex *= 2;
		}
		EvaluateCardPanelUI(m_CardPageIndex);
		CEventManager.RemoveListener<CEventPlayer_OnCardExpansionSelectScreenUpdated>(OnCardExpansionUpdated);
	}

	private void EvaluateCardPanelUI(int cardPageIndex)
	{
		m_PosX = 0f;
		m_LerpPosX = 0f;
		for (int i = 0; i < m_CheckPricePanelUIList.Count; i++)
		{
			m_CheckPricePanelUIList[i].SetActive(isActive: false);
		}
		m_CardExpansionText.text = InventoryBase.GetCardExpansionName(m_CurrentExpansionType);
		int cardGrade = 0;
		int num = cardPageIndex * m_MaxCardUICountPerPage;
		int num2 = num;
		int num3 = InventoryBase.GetShownMonsterList(m_CurrentExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_CurrentExpansionType);
		if (m_CurrentExpansionType == ECardExpansionType.Ghost)
		{
			num3 *= 2;
		}
		m_CardPageMaxIndex = Mathf.CeilToInt((float)num3 / (float)m_MaxCardUICountPerPage) - 1;
		for (int j = 0; j < m_MaxCardUICountPerPage; j++)
		{
			if (num >= num3)
			{
				m_CheckPricePanelUIList[j].SetActive(isActive: false);
				continue;
			}
			bool isDestiny = false;
			if (m_CurrentExpansionType == ECardExpansionType.Ghost)
			{
				if (num2 >= InventoryBase.GetShownMonsterList(m_CurrentExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_CurrentExpansionType))
				{
					isDestiny = true;
					num2 -= InventoryBase.GetShownMonsterList(m_CurrentExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(m_CurrentExpansionType);
				}
				m_CheckPricePanelUIList[j].InitCard(this, num2, m_CurrentExpansionType, isDestiny, cardGrade);
			}
			else
			{
				m_CheckPricePanelUIList[j].InitCard(this, num, m_CurrentExpansionType, isDestiny, cardGrade);
			}
			m_CheckPricePanelUIList[j].SetActive(isActive: true);
			m_ScrollEndParent.transform.parent = m_CheckPricePanelUIList[j].transform;
			m_ScrollEndParent.transform.position = m_CheckPricePanelUIList[j].transform.position;
			Vector3 position = m_ScrollEndParent.transform.position;
			position.y += m_CardScrollOffsetPosEnd.position.y - m_CardScrollOffsetPosStart.position.y;
			m_ScrollEndParent.transform.position = position;
			m_ScrollEndPosParent = m_ScrollEndParent;
			num++;
			num2 = num;
		}
		m_PageText.text = m_CardPageIndex + 1 + " / " + (m_CardPageMaxIndex + 1);
		m_CardPageOptionGrp.SetActive(value: true);
	}

	protected override void OnOpenScreen()
	{
		Init();
		base.OnOpenScreen();
	}

	public void OnPressOpenItemPriceGraph(EItemType itemType)
	{
		m_ItemPriceGraphScreen.ShowItemPriceChart(itemType);
		OpenChildScreen(m_ItemPriceGraphScreen);
	}

	public void OnPressOpenCardPriceGraph(int cardIndex, ECardExpansionType expansionType, bool isDestiny, int cardGrade)
	{
		m_ItemPriceGraphScreen.ShowCardPriceChart(cardIndex, expansionType, isDestiny, cardGrade);
		OpenChildScreen(m_ItemPriceGraphScreen);
	}
}
