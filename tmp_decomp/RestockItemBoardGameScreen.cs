using System.Collections.Generic;

public class RestockItemBoardGameScreen : RestockItemScreen
{
	protected override void EvaluateRestockItemPanelUI(int pageIndex)
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
		list = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownBoardGameItemType;
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
				else if (list[k] == EItemType.None)
				{
					m_CurrentRestockDataIndexList.Add(-1);
					break;
				}
			}
		}
		for (int m = 0; m < m_CurrentRestockDataIndexList.Count && m < m_RestockItemPanelUIList.Count; m++)
		{
			m_RestockItemPanelUIList[m].Init(this, m_CurrentRestockDataIndexList[m]);
			m_RestockItemPanelUIList[m].SetActive(isActive: true);
			m_ScrollEndPosParent = m_RestockItemPanelUIList[m].gameObject;
		}
	}
}
