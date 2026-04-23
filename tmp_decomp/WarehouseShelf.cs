using System.Collections.Generic;
using UnityEngine;

public class WarehouseShelf : InteractableObject
{
	public List<Transform> m_ShelfCompartmentGrpList;

	private List<InteractableStorageCompartment> m_StorageCompartmentList = new List<InteractableStorageCompartment>();

	private List<UI_PriceTag> m_UIPriceTagList = new List<UI_PriceTag>();

	private int m_Index;

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitWarehouseShelf(this);
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
				m_StorageCompartmentList.Add(m_ShelfCompartmentGrpList[i].GetChild(j).GetComponent<InteractableStorageCompartment>());
			}
		}
		m_Shelf_WorldUIGrp = PriceTagUISpawner.SpawnShelfWorldUIGrp(base.transform);
		for (int k = 0; k < m_StorageCompartmentList.Count; k++)
		{
			m_StorageCompartmentList[k].InitWarehouseShelf(this, m_ItemCompartmentList[k], k);
			m_GamepadQuickSelectTransformList.Add(m_ItemCompartmentList[k].m_GamepadQuickSelectAimLoc.transform);
			for (int l = 0; l < m_ItemCompartmentList[k].m_InteractablePriceTagList.Count; l++)
			{
				m_GamepadQuickSelectPriceTagTransformList.Add(m_ItemCompartmentList[k].m_InteractablePriceTagList[l].transform);
				UI_PriceTag uI_PriceTag = PriceTagUISpawner.SpawnPriceTagWarehouseRakWorldUIGrp(m_Shelf_WorldUIGrp, m_ItemCompartmentList[k].m_InteractablePriceTagList[l].transform);
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

	protected override void OnPlacedMovedObject()
	{
		base.OnPlacedMovedObject();
	}

	public override void BoxUpObject(bool holdBox)
	{
		base.BoxUpObject(holdBox);
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemoveWarehouseShelf(this);
		for (int i = 0; i < m_StorageCompartmentList.Count; i++)
		{
			if ((bool)m_StorageCompartmentList[i])
			{
				m_StorageCompartmentList[i].DisableAllItem();
			}
		}
		base.OnDestroyed();
	}

	public ShelfCompartment GetWarehouseCompartment(int index)
	{
		if (index >= m_StorageCompartmentList.Count)
		{
			return null;
		}
		return m_StorageCompartmentList[index].GetShelfCompartment();
	}

	public void SetIndex(int index)
	{
		m_Index = index;
	}

	public int GetIndex()
	{
		return m_Index;
	}

	public List<InteractableStorageCompartment> GetStorageCompartmentList()
	{
		return m_StorageCompartmentList;
	}

	public ShelfCompartment GetNonFullItemCompartment(EItemType itemType, bool isBigBox, bool ignoreNoneType = false)
	{
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (!ignoreNoneType && (m_ItemCompartmentList[i].GetItemType() == EItemType.None || m_ItemCompartmentList[i].GetItemCount() == 0))
			{
				return m_ItemCompartmentList[i];
			}
			if (m_ItemCompartmentList[i].GetItemType() == itemType && m_ItemCompartmentList[i].CheckBoxType(isBigBox) == isBigBox && m_ItemCompartmentList[i].GetItemCount() < m_ItemCompartmentList[i].GetMaxItemCount())
			{
				return m_ItemCompartmentList[i];
			}
		}
		return null;
	}

	public List<ShelfCompartment> GetNonFullItemCompartmentList(EItemType itemType, bool isBigBox, bool ignoreNoneType = false)
	{
		List<ShelfCompartment> list = new List<ShelfCompartment>();
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if (!ignoreNoneType && (m_ItemCompartmentList[i].GetItemType() == EItemType.None || m_ItemCompartmentList[i].GetItemCount() == 0))
			{
				list.Add(m_ItemCompartmentList[i]);
			}
			else if (m_ItemCompartmentList[i].GetItemType() == itemType && m_ItemCompartmentList[i].CheckBoxType(isBigBox) == isBigBox && m_ItemCompartmentList[i].GetItemCount() < m_ItemCompartmentList[i].GetMaxItemCount())
			{
				list.Add(m_ItemCompartmentList[i]);
			}
		}
		return list;
	}
}
