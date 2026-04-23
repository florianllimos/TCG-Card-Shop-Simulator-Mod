using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfCompartment : MonoBehaviour
{
	private Shelf m_Shelf;

	public bool m_CanPutItem = true;

	public bool m_CanPutBox;

	public bool m_ItemNotForSale;

	public bool m_CanLockItemLabel = true;

	public Transform m_StartLoc;

	public Transform m_EndWidthLoc;

	public Transform m_EndDepthLoc;

	public Transform m_EndHeightLoc;

	public Transform m_PosListGrp;

	public Transform m_CustomerStandLoc;

	public Transform m_StoredItemListGrp;

	public Transform m_GamepadQuickSelectAimLoc;

	private List<Item> m_StoredItemList = new List<Item>();

	private List<Transform> m_PosList = new List<Transform>();

	private List<InteractablePackagingBox_Item> m_InteractablePackagingBoxList = new List<InteractablePackagingBox_Item>();

	public List<InteractablePriceTag> m_InteractablePriceTagList;

	public bool m_ApplyScaleOffset;

	public bool m_HeightGoesUp;

	public bool m_AffectedByTallItem;

	public int m_SizeX = 4;

	public int m_SizeY = 8;

	public int m_SizeZ = 1;

	private int m_Index;

	private int m_ItemAmount;

	private int m_MaxItemCount;

	private float m_Width;

	private float m_Depth;

	private float m_Height;

	private float m_CurrentItemSizeX = 1f;

	private float m_CurrentItemSizeY = 1f;

	private float m_CurrentItemSizeZ = 1f;

	private float m_PosYOffsetInBox;

	private float m_ScaleOffsetInBox;

	private float m_CurrentPrice;

	private int m_MaxItemX;

	private int m_MaxItemY;

	private int m_MaxItemZ;

	private EItemType m_ItemType = EItemType.None;

	private bool m_IsTallItem;

	private bool m_IsBigBox;

	private bool m_IsSettingPrice;

	private List<Vector3> m_ItemPosList = new List<Vector3>();

	private WarehouseShelf m_WarehouseShelf;

	private Coroutine m_ResetIsSettingPriceCoroutine;

	private void Awake()
	{
		for (int i = 0; i < m_PosListGrp.childCount; i++)
		{
			m_PosList.Add(m_PosListGrp.GetChild(i).transform);
		}
	}

	public void InitShelf(Shelf shelf)
	{
		m_Shelf = shelf;
		Vector3 position = m_CustomerStandLoc.position;
		position.y = 0f;
		m_CustomerStandLoc.position = position;
		for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
		{
			m_InteractablePriceTagList[i].Init(this);
		}
		if (m_ItemNotForSale)
		{
			for (int j = 0; j < m_InteractablePriceTagList.Count; j++)
			{
				m_InteractablePriceTagList[j].gameObject.SetActive(value: false);
			}
		}
	}

	public bool CheckBoxType(bool isBigBox)
	{
		if (m_ItemAmount <= 0)
		{
			SetCompartmentBoxType(isBigBox);
			CalculatePositionList();
		}
		return m_IsBigBox;
	}

	public bool CheckBoxItemType(EItemType itemType)
	{
		EItemType eItemType = EItemType.None;
		if (m_InteractablePackagingBoxList.Count > 0)
		{
			eItemType = m_InteractablePackagingBoxList[0].m_ItemCompartment.GetItemType();
			if (itemType == eItemType)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public void SetCompartmentBoxType(bool isBigBox)
	{
		m_IsBigBox = isBigBox;
		if (isBigBox)
		{
			m_CurrentItemSizeX = 4f;
			m_CurrentItemSizeY = 4f;
			m_CurrentItemSizeZ = 2f;
		}
		else
		{
			m_CurrentItemSizeX = 4f;
			m_CurrentItemSizeY = 4f;
			m_CurrentItemSizeZ = 1f;
		}
	}

	public void AddBox(InteractablePackagingBox_Item interactablePackagingBox)
	{
		m_ItemAmount++;
		m_InteractablePackagingBoxList.Add(interactablePackagingBox);
		SetCompartmentItemType(interactablePackagingBox.m_ItemCompartment.GetItemType());
		SetPriceTagItemAmountText();
		SetPriceTagVisibility(isVisible: true);
	}

	public void ArrangeBoxItemBasedOnItemCount()
	{
		List<InteractablePackagingBox_Item> list = new List<InteractablePackagingBox_Item>();
		for (int i = 0; i < m_InteractablePackagingBoxList.Count; i++)
		{
			if (list.Count == 0)
			{
				list.Add(m_InteractablePackagingBoxList[i]);
				continue;
			}
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (m_InteractablePackagingBoxList[i].m_ItemCompartment.GetItemCount() >= list[j].m_ItemCompartment.GetItemCount())
				{
					flag = true;
					list.Insert(j, m_InteractablePackagingBoxList[i]);
					break;
				}
			}
			if (!flag)
			{
				list.Add(m_InteractablePackagingBoxList[i]);
			}
		}
		m_InteractablePackagingBoxList = list;
		for (int k = 0; k < m_InteractablePackagingBoxList.Count; k++)
		{
			m_InteractablePackagingBoxList[k].transform.position = m_PosList[k].position;
		}
	}

	public void RemoveBox(InteractablePackagingBox_Item interactablePackagingBox)
	{
		m_ItemAmount--;
		m_InteractablePackagingBoxList.Remove(interactablePackagingBox);
		if (m_ItemAmount <= 0 && (!m_CanLockItemLabel || !CSingleton<CGameManager>.Instance.m_LockItemLabel))
		{
			SetCompartmentItemType(EItemType.None);
			SetPriceTagVisibility(isVisible: false);
		}
		SetPriceTagItemAmountText();
	}

	public EItemType CheckItemType(EItemType itemType)
	{
		if (m_ItemAmount <= 0)
		{
			SetCompartmentItemType(itemType);
			CalculatePositionList();
		}
		return m_ItemType;
	}

	public EItemType GetItemType()
	{
		return m_ItemType;
	}

	public int GetItemCount()
	{
		return m_ItemAmount;
	}

	public int GetMaxItemCount()
	{
		return m_MaxItemCount;
	}

	public void AddItem(Item item, bool addToFront)
	{
		m_ItemAmount++;
		if (addToFront)
		{
			m_StoredItemList.Insert(0, item);
		}
		else
		{
			m_StoredItemList.Add(item);
		}
		SetPriceTagItemAmountText();
		SetPriceTagItemPriceText();
		if (!m_IsSettingPrice)
		{
			if (m_ResetIsSettingPriceCoroutine != null)
			{
				StopCoroutine(m_ResetIsSettingPriceCoroutine);
			}
			m_ResetIsSettingPriceCoroutine = StartCoroutine(DelayResetIsSettingPrice());
		}
	}

	private IEnumerator DelayResetIsSettingPrice()
	{
		m_IsSettingPrice = true;
		yield return new WaitForSeconds(3f);
		m_IsSettingPrice = false;
	}

	public void RemoveItem(Item item)
	{
		m_ItemAmount--;
		m_StoredItemList.Remove(item);
		if (m_ItemAmount <= 0 && (!m_CanLockItemLabel || !CSingleton<CGameManager>.Instance.m_LockItemLabel))
		{
			SetCompartmentItemType(EItemType.None);
		}
		SetPriceTagItemAmountText();
		SetPriceTagItemPriceText();
	}

	public Item TakeItemToHand(bool getLastItem = true)
	{
		if (m_ItemAmount <= 0)
		{
			return null;
		}
		Item item = GetLastItem();
		if (!getLastItem)
		{
			item = GetFirstItem();
		}
		m_ItemAmount--;
		m_StoredItemList.Remove(item);
		if (m_ItemAmount <= 0 && (!m_CanLockItemLabel || !CSingleton<CGameManager>.Instance.m_LockItemLabel))
		{
			SetCompartmentItemType(EItemType.None);
		}
		SetPriceTagItemAmountText();
		SetPriceTagItemPriceText();
		return item;
	}

	public void RemoveLabel(bool playSound)
	{
		if (m_ItemAmount <= 0)
		{
			if (playSound)
			{
				SoundManager.GenericConfirm();
			}
			SetCompartmentItemType(EItemType.None);
		}
	}

	public bool HasEnoughSlot()
	{
		if (m_ItemAmount < m_MaxItemCount)
		{
			return true;
		}
		return false;
	}

	public Transform GetEmptySlotTransform()
	{
		return m_PosList[m_ItemAmount];
	}

	public Transform GetLastEmptySlotTransform()
	{
		return m_PosList[m_MaxItemCount - m_ItemAmount - 1];
	}

	public Transform GetEmptySlotParent()
	{
		return m_StoredItemListGrp;
	}

	public Item GetFirstItem()
	{
		return m_StoredItemList[0];
	}

	public Item GetLastItem()
	{
		if (m_StoredItemList.Count <= 0)
		{
			return null;
		}
		return m_StoredItemList[m_ItemAmount - 1];
	}

	public List<InteractablePackagingBox_Item> GetInteractablePackagingBoxList()
	{
		return m_InteractablePackagingBoxList;
	}

	public InteractablePackagingBox_Item GetLastInteractablePackagingBox()
	{
		return m_InteractablePackagingBoxList[m_InteractablePackagingBoxList.Count - 1];
	}

	public void SetIndex(int index)
	{
		m_Index = index;
	}

	public int GetIndex()
	{
		return m_Index;
	}

	public int GetWarehouseIndex()
	{
		return m_WarehouseShelf.GetIndex();
	}

	public void SetWarehouseShelf(WarehouseShelf warehouseShelf)
	{
		m_WarehouseShelf = warehouseShelf;
		for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
		{
			m_InteractablePriceTagList[i].Init(this);
		}
	}

	public void SetCompartmentItemType(EItemType itemType)
	{
		m_ItemType = itemType;
		if (itemType == EItemType.None)
		{
			m_PosYOffsetInBox = 0f;
			m_ScaleOffsetInBox = 0f;
			m_CurrentItemSizeX = 1f;
			m_CurrentItemSizeY = 1f;
			m_CurrentItemSizeZ = 1f;
			SetPriceTagVisibility(isVisible: false);
			return;
		}
		Vector3 itemDimension = InventoryBase.GetItemData(m_ItemType).itemDimension;
		m_IsTallItem = InventoryBase.GetItemData(m_ItemType).isTallItem;
		if (m_ApplyScaleOffset)
		{
			m_PosYOffsetInBox = InventoryBase.GetItemData(m_ItemType).posYOffsetInBox;
			m_ScaleOffsetInBox = InventoryBase.GetItemData(m_ItemType).scaleOffsetInBox;
		}
		m_CurrentItemSizeX = itemDimension.x;
		m_CurrentItemSizeY = itemDimension.y;
		m_CurrentItemSizeZ = itemDimension.z;
		SetPriceTagItemImage();
		SetPriceTagVisibility(isVisible: true);
	}

	public void CalculatePositionList()
	{
		m_Width = (m_EndWidthLoc.position - m_StartLoc.position).magnitude;
		m_Depth = (m_EndDepthLoc.position - m_StartLoc.position).magnitude;
		m_Height = (m_EndHeightLoc.position - m_StartLoc.position).magnitude;
		m_MaxItemX = Mathf.RoundToInt((float)m_SizeX / m_CurrentItemSizeX);
		m_MaxItemY = Mathf.RoundToInt((float)m_SizeY / m_CurrentItemSizeY);
		m_MaxItemZ = Mathf.RoundToInt((float)m_SizeZ / m_CurrentItemSizeZ);
		if (m_IsTallItem && m_AffectedByTallItem)
		{
			m_MaxItemZ = Mathf.RoundToInt((float)m_SizeZ / (m_CurrentItemSizeZ * 2f));
		}
		m_MaxItemCount = m_MaxItemX * m_MaxItemY * m_MaxItemZ;
		m_ItemPosList.Clear();
		Vector3 vector = default(Vector3);
		Vector3 vector2 = m_StartLoc.up;
		if (!m_HeightGoesUp)
		{
			vector2 = -m_StartLoc.up;
		}
		for (int i = 0; i < m_MaxItemZ; i++)
		{
			float num = m_Height / (float)m_MaxItemZ / 2f;
			if (m_IsTallItem && m_AffectedByTallItem)
			{
				num = m_Height / (float)m_MaxItemZ;
			}
			float num2 = num * (float)(i * 2 + 1);
			for (int j = 0; j < m_MaxItemY; j++)
			{
				float num3 = m_Depth / (float)m_MaxItemY / 2f * (float)(j * 2 + 1);
				for (int k = 0; k < m_MaxItemX; k++)
				{
					float num4 = m_Width / (float)m_MaxItemX / 2f * (float)(k * 2 + 1);
					vector = m_StartLoc.position + -m_StartLoc.right * num4 + m_StartLoc.forward * num3 + (vector2 * num2 + vector2 * (0f - m_PosYOffsetInBox));
					m_ItemPosList.Add(vector);
				}
			}
		}
		for (int l = 0; l < m_ItemPosList.Count; l++)
		{
			m_PosList[l].transform.position = m_ItemPosList[l];
			m_PosList[l].transform.rotation = m_StartLoc.rotation;
			m_PosList[l].transform.localScale = Vector3.one + Vector3.one * m_ScaleOffsetInBox;
		}
	}

	public int GetItemPosListCount()
	{
		return m_ItemPosList.Count;
	}

	public void PreSpawnItemUpdate(int amount)
	{
		m_ItemAmount = Mathf.Clamp(amount, 0, m_ItemPosList.Count);
		SetPriceTagItemAmountText();
		SetPriceTagItemPriceText();
	}

	private IEnumerator SpawnItemRoutine(int amount, bool spawnFromFront)
	{
		ItemMeshData meshData = InventoryBase.GetItemMeshData(m_ItemType);
		m_ItemAmount = Mathf.Clamp(amount, 0, m_ItemPosList.Count);
		int i = 0;
		while (i < m_ItemPosList.Count && i < amount)
		{
			Item item = ItemSpawnManager.GetItem(m_StoredItemListGrp);
			item.SetMesh(meshData.mesh, meshData.material, m_ItemType, meshData.meshSecondary, meshData.materialSecondary, meshData.materialList);
			if (spawnFromFront)
			{
				item.transform.position = m_PosList[i].transform.position;
				item.transform.localScale = m_PosList[i].localScale;
			}
			else
			{
				item.transform.position = m_PosList[m_MaxItemCount - i - 1].transform.position;
				item.transform.localScale = m_PosList[m_MaxItemCount - i - 1].localScale;
			}
			item.transform.rotation = m_StartLoc.rotation;
			item.gameObject.SetActive(value: true);
			if (spawnFromFront)
			{
				m_StoredItemList.Add(item);
			}
			else
			{
				m_StoredItemList.Insert(0, item);
			}
			yield return null;
			int num = i + 1;
			i = num;
		}
		SetPriceTagItemAmountText();
		SetPriceTagItemPriceText();
	}

	public void SpawnItemCoroutine(int amount, bool spawnFromFront)
	{
		CSingleton<CGameManager>.Instance.StartCoroutine(SpawnItemRoutine(amount, spawnFromFront));
	}

	public void SpawnItem(int amount, bool spawnFromFront)
	{
		Item item = null;
		ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(m_ItemType);
		m_ItemAmount = Mathf.Clamp(amount, 0, m_ItemPosList.Count);
		for (int i = 0; i < m_ItemPosList.Count && i < amount; i++)
		{
			item = ItemSpawnManager.GetItem(m_StoredItemListGrp);
			item.SetMesh(itemMeshData.mesh, itemMeshData.material, m_ItemType, itemMeshData.meshSecondary, itemMeshData.materialSecondary, itemMeshData.materialList);
			if (spawnFromFront)
			{
				item.transform.position = m_PosList[i].transform.position;
				item.transform.localScale = m_PosList[i].localScale;
			}
			else
			{
				item.transform.position = m_PosList[m_MaxItemCount - i - 1].transform.position;
				item.transform.localScale = m_PosList[m_MaxItemCount - i - 1].localScale;
			}
			item.transform.rotation = m_StartLoc.rotation;
			item.gameObject.SetActive(value: true);
			if (spawnFromFront)
			{
				m_StoredItemList.Add(item);
			}
			else
			{
				m_StoredItemList.Insert(0, item);
			}
		}
		SetPriceTagItemAmountText();
		SetPriceTagItemPriceText();
	}

	public void RefreshItemPosition(bool spawnFromFront)
	{
		for (int i = 0; i < m_StoredItemList.Count; i++)
		{
			if (spawnFromFront)
			{
				m_StoredItemList[i].transform.position = m_PosList[i].transform.position;
			}
			else
			{
				m_StoredItemList[m_StoredItemList.Count - i - 1].transform.position = m_PosList[m_MaxItemCount - i - 1].transform.position;
			}
		}
	}

	public void SetPriceTagVisibility(bool isVisible)
	{
		if (!m_ItemNotForSale)
		{
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetVisibility(isVisible);
			}
		}
	}

	public void SetIgnoreCull(bool ignoreCull)
	{
		if (!m_ItemNotForSale)
		{
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetIgnoreCull(ignoreCull);
			}
		}
	}

	public void SetPriceTagItemImage(EItemType itemType = EItemType.None)
	{
		if (!m_ItemNotForSale)
		{
			if (m_CanPutItem)
			{
				itemType = GetItemType();
			}
			else if (m_CanPutBox && m_InteractablePackagingBoxList.Count > 0)
			{
				itemType = m_InteractablePackagingBoxList[0].m_ItemCompartment.GetItemType();
			}
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetItemImage(itemType);
			}
		}
	}

	public void SetPriceTagItemAmountText()
	{
		if (m_ItemNotForSale)
		{
			return;
		}
		if (m_CanPutItem)
		{
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetAmountText(m_ItemAmount);
			}
		}
		else if (m_CanPutBox)
		{
			int num = 0;
			for (int j = 0; j < m_InteractablePackagingBoxList.Count; j++)
			{
				num += m_InteractablePackagingBoxList[j].m_ItemCompartment.GetItemCount();
			}
			for (int k = 0; k < m_InteractablePriceTagList.Count; k++)
			{
				m_InteractablePriceTagList[k].SetAmountText(num);
			}
		}
	}

	public void SetPriceTagItemPriceText()
	{
		if (m_ItemNotForSale)
		{
			return;
		}
		if (m_CanPutItem)
		{
			m_CurrentPrice = CPlayerData.GetItemPrice(m_ItemType, preventZero: false);
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].SetPriceText(m_CurrentPrice);
			}
		}
		else if (m_CanPutBox)
		{
			for (int j = 0; j < m_InteractablePriceTagList.Count; j++)
			{
			}
		}
	}

	public void RefreshPriceTagItemPriceText()
	{
		if (!m_ItemNotForSale)
		{
			for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
			{
				m_InteractablePriceTagList[i].RefreshPriceText();
			}
		}
	}

	[Obsolete("Use ApplyHalfVisibility(bool) instead. This method will be removed.")]
	public void SetVisibilityHalfItemMesh(bool isVisible)
	{
		for (int i = 0; i < m_StoredItemList.Count; i++)
		{
			if (isVisible)
			{
				m_StoredItemList[i].m_Mesh.enabled = true;
			}
			else if (m_StoredItemList.Count > 8)
			{
				if (i / m_MaxItemX % 2 == 1)
				{
					m_StoredItemList[i].m_Mesh.enabled = false;
				}
				else
				{
					m_StoredItemList[i].m_Mesh.enabled = true;
				}
			}
		}
	}

	public void ApplyHalfVisibility(bool isVisible)
	{
		int count = m_StoredItemList.Count;
		bool flag = !isVisible && count > 8;
		for (int i = 0; i < count; i++)
		{
			bool flag2 = true;
			if (flag)
			{
				flag2 = i / m_MaxItemX % 2 != 1;
			}
			m_StoredItemList[i].m_Mesh.enabled = flag2;
		}
	}

	public void DisableAllItem()
	{
		for (int i = 0; i < m_StoredItemList.Count; i++)
		{
			if ((bool)m_StoredItemList[i])
			{
				m_StoredItemList[i].DisableItem();
			}
		}
	}

	public Shelf GetShelf()
	{
		return m_Shelf;
	}

	public WarehouseShelf GetWarehouseShelf()
	{
		return m_WarehouseShelf;
	}

	public bool HasSetPrice()
	{
		for (int i = 0; i < m_InteractablePriceTagList.Count; i++)
		{
			if (m_InteractablePriceTagList[i].GetIsPriceSet())
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSettingPrice()
	{
		return m_IsSettingPrice;
	}

	public void OnStartSetPrice()
	{
		if (m_ResetIsSettingPriceCoroutine != null)
		{
			StopCoroutine(m_ResetIsSettingPriceCoroutine);
		}
		m_IsSettingPrice = true;
	}

	public void OnFinishSetPrice()
	{
		m_IsSettingPrice = false;
	}
}
