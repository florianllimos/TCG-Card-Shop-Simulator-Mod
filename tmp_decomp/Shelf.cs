using System.Collections.Generic;
using UnityEngine;

public class Shelf : InteractableObject
{
	public bool m_ItemNotForSale;

	public bool m_GamepadQuickSelectSequenceReverse;

	public List<Transform> m_ShelfCompartmentGrpList;

	public List<UI_PriceTag> m_UIPriceTagList = new List<UI_PriceTag>();

	public bool m_IsHalfVisible { get; private set; } = true;

	public void SetHalfVisibility(bool visible)
	{
		if (m_IsHalfVisible != visible)
		{
			m_IsHalfVisible = visible;
			List<ShelfCompartment> itemCompartmentList = GetItemCompartmentList();
			for (int i = 0; i < itemCompartmentList.Count; i++)
			{
				itemCompartmentList[i].ApplyHalfVisibility(visible);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitShelf(this);
	}

	public override void Init()
	{
		if (m_HasInit)
		{
			return;
		}
		base.Init();
		for (int i = 0; i < m_ShelfCompartmentGrpList.Count; i++)
		{
			for (int j = 0; j < m_ShelfCompartmentGrpList[i].childCount; j++)
			{
				m_ItemCompartmentList.Add(m_ShelfCompartmentGrpList[i].GetChild(j).GetComponent<ShelfCompartment>());
			}
		}
		m_Shelf_WorldUIGrp = PriceTagUISpawner.SpawnShelfWorldUIGrp(base.transform);
		for (int k = 0; k < m_ItemCompartmentList.Count; k++)
		{
			m_ItemCompartmentList[k].m_ItemNotForSale = m_ItemNotForSale;
			m_ItemCompartmentList[k].InitShelf(this);
			if (!m_GamepadQuickSelectSequenceReverse || m_ItemNotForSale)
			{
				m_GamepadQuickSelectTransformList.Add(m_ItemCompartmentList[k].m_GamepadQuickSelectAimLoc.transform);
			}
			if (m_ItemNotForSale)
			{
				continue;
			}
			if (m_ItemCompartmentList[k].m_InteractablePriceTagList.Count > 0)
			{
				m_GamepadQuickSelectTransformList.Add(m_ItemCompartmentList[k].m_InteractablePriceTagList[0].transform);
				m_GamepadQuickSelectPriceTagTransformList.Add(m_ItemCompartmentList[k].m_InteractablePriceTagList[0].transform);
				if (m_GamepadQuickSelectSequenceReverse)
				{
					m_GamepadQuickSelectTransformList.Add(m_ItemCompartmentList[k].m_GamepadQuickSelectAimLoc.transform);
				}
			}
			for (int l = 0; l < m_ItemCompartmentList[k].m_InteractablePriceTagList.Count; l++)
			{
				UI_PriceTag uI_PriceTag = PriceTagUISpawner.SpawnPriceTagWorldUIGrp(m_Shelf_WorldUIGrp, m_ItemCompartmentList[k].m_InteractablePriceTagList[l].transform);
				m_UIPriceTagList.Add(uI_PriceTag);
				m_ItemCompartmentList[k].m_InteractablePriceTagList[l].SetPriceTagUI(uI_PriceTag);
				m_ItemCompartmentList[k].m_InteractablePriceTagList[l].SetVisibility(isVisible: false);
			}
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (m_IsMovingObject && (bool)m_Shelf_WorldUIGrp)
		{
			m_Shelf_WorldUIGrp.transform.position = base.transform.position;
			m_Shelf_WorldUIGrp.transform.rotation = base.transform.rotation;
		}
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemoveShelf(this);
		base.OnDestroyed();
	}

	protected override void OnPlacedMovedObject()
	{
		base.OnPlacedMovedObject();
	}

	public override void BoxUpObject(bool holdBox)
	{
		base.BoxUpObject(holdBox);
	}

	public bool HasEmptyItemSlotOnShelf()
	{
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (m_ItemCompartmentList[i].GetItemType() == EItemType.None)
			{
				return true;
			}
			if (m_ItemCompartmentList[i].GetItemCount() < m_ItemCompartmentList[i].GetMaxItemCount())
			{
				return true;
			}
		}
		return false;
	}

	public bool HasItemOnShelf()
	{
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (m_ItemCompartmentList[i].GetItemCount() > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasItemTypeOnShelf(List<EItemType> itemTypeList)
	{
		if (itemTypeList == null || itemTypeList.Count == 0)
		{
			return true;
		}
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (itemTypeList.Contains(m_ItemCompartmentList[i].GetItemType()))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasItemTypeOnShelf(EItemType itemType)
	{
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (itemType == m_ItemCompartmentList[i].GetItemType() && m_ItemCompartmentList[i].GetItemCount() > 0)
			{
				return true;
			}
		}
		return false;
	}

	public ShelfCompartment GetNonFullItemCompartment(EItemType itemType, bool ignoreNoneType = false)
	{
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (!ignoreNoneType && (m_ItemCompartmentList[i].GetItemType() == EItemType.None || m_ItemCompartmentList[i].GetItemCount() == 0))
			{
				return m_ItemCompartmentList[i];
			}
			if (m_ItemCompartmentList[i].GetItemType() == itemType && m_ItemCompartmentList[i].GetItemCount() < m_ItemCompartmentList[i].GetMaxItemCount())
			{
				return m_ItemCompartmentList[i];
			}
		}
		return null;
	}

	public List<ShelfCompartment> GetNonFullItemCompartmentList(EItemType itemType, bool ignoreNoneType = false)
	{
		List<ShelfCompartment> list = new List<ShelfCompartment>();
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (!ignoreNoneType && (m_ItemCompartmentList[i].GetItemType() == EItemType.None || m_ItemCompartmentList[i].GetItemCount() == 0))
			{
				list.Add(m_ItemCompartmentList[i]);
			}
			else if (m_ItemCompartmentList[i].GetItemType() == itemType && m_ItemCompartmentList[i].GetItemCount() < m_ItemCompartmentList[i].GetMaxItemCount())
			{
				list.Add(m_ItemCompartmentList[i]);
			}
		}
		return list;
	}

	public ShelfCompartment GetCustomerTargetItemCompartment(List<EItemType> targetBuyItemList)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (m_ItemCompartmentList[i].GetItemCount() > 0 && (targetBuyItemList == null || targetBuyItemList.Count == 0 || targetBuyItemList.Contains(m_ItemCompartmentList[i].GetItemType())))
			{
				list.Add(i);
			}
		}
		if (list.Count > 0)
		{
			int index = list[Random.Range(0, list.Count)];
			return m_ItemCompartmentList[index];
		}
		return m_ItemCompartmentList[Random.Range(0, m_ItemCompartmentList.Count)];
	}

	public List<ShelfCompartment> GetItemCompartmentList()
	{
		return m_ItemCompartmentList;
	}

	public void LoadItemCompartment(List<ItemTypeAmount> itemTypeAmountList)
	{
		for (int i = 0; i < m_ItemCompartmentList.Count && i < itemTypeAmountList.Count; i++)
		{
			m_ItemCompartmentList[i].SetCompartmentItemType(itemTypeAmountList[i].itemType);
			m_ItemCompartmentList[i].CalculatePositionList();
			m_ItemCompartmentList[i].SpawnItem(itemTypeAmountList[i].amount, spawnFromFront: true);
		}
	}
}
