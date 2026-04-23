using System.Collections;
using UnityEngine;

public class InteractablePackagingBox_Item : InteractablePackagingBox
{
	public bool m_IsBigBox;

	public bool m_IsStored;

	public ShelfCompartment m_ItemCompartment;

	public GameObject m_StaticMeshGrp;

	public GameObject m_RigMeshGrp;

	public GameObject m_ClosedBox;

	public GameObject m_OpenBox;

	public GameObject m_OutlineClosedBox;

	public GameObject m_OutlineOpenBox;

	private EItemType m_ItemType;

	private Vector3 m_ItemDimension;

	private bool m_IsBoxOpened;

	private bool m_IsTogglingOpenClose;

	private bool m_IsFirstOpenDone;

	private bool m_PreventWorkerTakeBox;

	private int m_ItemCount;

	private int m_StoredWarehouseShelfIndex;

	private int m_StorageCompartmentIndex;

	private int m_ItemAmountToSpawn;

	private ShelfCompartment m_BoxStoredCompartment;

	protected override void Awake()
	{
		base.Awake();
		RestockManager.InitItemPackageBox(this);
		m_StaticMeshGrp.gameObject.SetActive(value: true);
		m_RigMeshGrp.gameObject.SetActive(value: false);
		m_ClosedBox.SetActive(!m_IsBoxOpened);
		m_OpenBox.SetActive(m_IsBoxOpened);
		m_OutlineClosedBox.SetActive(!m_IsBoxOpened);
		m_OutlineOpenBox.SetActive(m_IsBoxOpened);
	}

	public override void StartHoldBox(bool isPlayer, Transform holdItemPos)
	{
		if (!m_IsBeingHold)
		{
			base.StartHoldBox(isPlayer, holdItemPos);
			m_IsStored = false;
			if (!m_IsBoxOpened && m_ItemCompartment.GetItemType() != EItemType.None && m_ItemCompartment.GetItemCount() > 0)
			{
				SetPriceTagVisibility(isVisible: true);
			}
			if (isPlayer)
			{
				UpdateToolTip();
				TutorialManager.AddTaskValue(ETutorialTaskCondition.PickupBox, 1f);
				m_ItemCompartment.SetIgnoreCull(ignoreCull: true);
			}
		}
	}

	public override void ThrowBox(bool isPlayer)
	{
		base.ThrowBox(isPlayer);
		m_ItemCompartment.SetIgnoreCull(ignoreCull: false);
	}

	public override void DropBox(bool isPlayer)
	{
		base.DropBox(isPlayer);
		m_ItemCompartment.SetIgnoreCull(ignoreCull: false);
	}

	protected override void SpawnPriceTag()
	{
		base.SpawnPriceTag();
		for (int i = 0; i < m_ItemCompartment.m_InteractablePriceTagList.Count; i++)
		{
			UI_PriceTag priceTagUI = PriceTagUISpawner.SpawnPriceTagItemBoxWorldUIGrp(m_Shelf_WorldUIGrp, m_ItemCompartment.m_InteractablePriceTagList[i].transform);
			m_ItemCompartment.m_InteractablePriceTagList[i].SetPriceTagUI(priceTagUI);
			m_ItemCompartment.m_InteractablePriceTagList[i].SetVisibility(isVisible: true);
		}
	}

	public void FillBoxWithItem(EItemType itemType, int amount)
	{
		SpawnPriceTag();
		m_ItemType = itemType;
		m_ItemDimension = InventoryBase.GetItemData(m_ItemType).itemDimension;
		m_ItemCompartment.gameObject.SetActive(value: false);
		m_ItemCompartment.SetCompartmentItemType(m_ItemType);
		m_ItemCompartment.CalculatePositionList();
		if (m_IsBoxOpened)
		{
			m_ItemCompartment.SpawnItemCoroutine(amount, spawnFromFront: false);
			m_ItemCount = m_ItemCompartment.GetItemCount();
		}
		else
		{
			m_ItemCompartment.PreSpawnItemUpdate(amount);
			m_ItemCount = m_ItemCompartment.GetItemCount();
			m_ItemAmountToSpawn = m_ItemCount;
		}
		if (amount <= 0)
		{
			SetPriceTagVisibility(isVisible: false);
		}
	}

	public void SetItemType(EItemType itemType)
	{
		m_ItemType = itemType;
	}

	public EItemType GetItemType()
	{
		return m_ItemType;
	}

	public override void OnHoldStateLeftMousePress(bool isPlayer, ShelfCompartment targetItemCompartment)
	{
		base.OnHoldStateLeftMousePress(isPlayer, targetItemCompartment);
		DispenseItem(isPlayer, targetItemCompartment);
	}

	public override void OnHoldStateRightMousePress(bool isPlayer, ShelfCompartment targetItemCompartment)
	{
		base.OnHoldStateRightMousePress(isPlayer, targetItemCompartment);
		RemoveItemFromShelf(isPlayer, targetItemCompartment);
	}

	public void DispenseItem(bool isPlayer, ShelfCompartment targetItemCompartment)
	{
		if (!CanPickup())
		{
			return;
		}
		if ((bool)targetItemCompartment && targetItemCompartment.m_CanPutBox)
		{
			if (m_IsLerpingToPos)
			{
				return;
			}
			if (m_ItemCompartment.GetItemType() == EItemType.None || m_ItemCompartment.GetItemCount() <= 0)
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotStoreEmptyBox);
				}
				return;
			}
			bool flag = targetItemCompartment.CheckBoxType(m_IsBigBox);
			if (!targetItemCompartment.HasEnoughSlot())
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfNoSlot);
				}
				return;
			}
			if (flag != m_IsBigBox)
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotMixDifferentBoxSizes);
				}
				return;
			}
			if (!targetItemCompartment.CheckBoxItemType(m_ItemType))
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfWrongItemType);
				}
				return;
			}
			if (m_IsBoxOpened)
			{
				m_IsTogglingOpenClose = false;
				SetOpenCloseBox(isOpen: false, isPlayer);
			}
			Transform emptySlotTransform = targetItemCompartment.GetEmptySlotTransform();
			Transform emptySlotParent = targetItemCompartment.GetEmptySlotParent();
			LerpToTransform(emptySlotTransform, emptySlotParent);
			targetItemCompartment.AddBox(this);
			SetPriceTagVisibility(isVisible: false);
			m_MoveStateValidArea.gameObject.SetActive(value: false);
			m_IsStored = true;
			m_IsBeingHold = false;
			m_StoredWarehouseShelfIndex = targetItemCompartment.GetWarehouseIndex();
			m_StorageCompartmentIndex = targetItemCompartment.GetIndex();
			m_BoxStoredCompartment = targetItemCompartment;
			if (isPlayer)
			{
				CSingleton<InteractionPlayerController>.Instance.OnExitHoldBoxMode();
				SoundManager.GenericPop();
			}
			return;
		}
		m_BoxStoredCompartment = null;
		if (m_IsBoxOpened && (bool)targetItemCompartment)
		{
			if (!targetItemCompartment.m_CanPutItem)
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CanOnlyStoreBoxOnThisShelf);
				}
				return;
			}
			if (m_ItemCompartment.GetItemCount() <= 0)
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxNoItem);
				}
				return;
			}
			EItemType eItemType = targetItemCompartment.CheckItemType(m_ItemType);
			if (!targetItemCompartment.HasEnoughSlot())
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfNoSlot);
				}
				return;
			}
			if (eItemType != m_ItemType)
			{
				if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfWrongItemType);
				}
				return;
			}
			Transform emptySlotTransform2 = targetItemCompartment.GetEmptySlotTransform();
			Transform emptySlotParent2 = targetItemCompartment.GetEmptySlotParent();
			Item firstItem = m_ItemCompartment.GetFirstItem();
			firstItem.LerpToTransform(emptySlotTransform2, emptySlotParent2);
			targetItemCompartment.AddItem(firstItem, addToFront: false);
			m_ItemCompartment.RemoveItem(firstItem);
			if (isPlayer)
			{
				TutorialManager.AddTaskValue(ETutorialTaskCondition.PutItemOnShelf, 1f);
				SoundManager.GenericPop();
			}
		}
		else if (!m_IsBoxOpened && (bool)targetItemCompartment && m_ItemCompartment.GetItemCount() > 0)
		{
			OnPressOpenBox();
		}
	}

	public void RemoveItemFromShelf(bool isPlayer, ShelfCompartment targetItemCompartment)
	{
		if (!m_IsBoxOpened || !targetItemCompartment || !targetItemCompartment.m_CanPutItem)
		{
			return;
		}
		if (targetItemCompartment.GetItemCount() <= 0)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfNoItem);
			return;
		}
		EItemType eItemType = targetItemCompartment.CheckItemType(m_ItemType);
		EItemType eItemType2 = m_ItemCompartment.CheckItemType(eItemType);
		if (eItemType2 != m_ItemType)
		{
			m_ItemType = eItemType2;
		}
		if (eItemType != m_ItemType)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxWrongItemType);
		}
		else if (m_ItemCompartment.HasEnoughSlot())
		{
			Transform lastEmptySlotTransform = m_ItemCompartment.GetLastEmptySlotTransform();
			Transform emptySlotParent = m_ItemCompartment.GetEmptySlotParent();
			Item lastItem = targetItemCompartment.GetLastItem();
			lastItem.LerpToTransform(lastEmptySlotTransform, emptySlotParent);
			targetItemCompartment.RemoveItem(lastItem);
			m_ItemCompartment.AddItem(lastItem, addToFront: true);
			SetPriceTagVisibility(isVisible: false);
			if (isPlayer)
			{
				SoundManager.GenericPop(1f, 0.9f);
			}
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxNoSlot);
			SetPriceTagVisibility(isVisible: false);
		}
	}

	protected override void OnFinishLerp()
	{
		base.OnFinishLerp();
		if (m_IsStored)
		{
			m_BoxStoredCompartment.ArrangeBoxItemBasedOnItemCount();
		}
		else
		{
			m_BoxStoredCompartment = null;
		}
	}

	public override bool IsValidObject()
	{
		if (m_IsStored && (bool)m_BoxStoredCompartment && (bool)m_BoxStoredCompartment.GetWarehouseShelf())
		{
			if (base.IsValidObject())
			{
				return m_BoxStoredCompartment.GetWarehouseShelf().IsValidObject();
			}
			return false;
		}
		return base.IsValidObject();
	}

	public bool CanWorkerTakeBox()
	{
		return !m_PreventWorkerTakeBox;
	}

	public override void OnPressOpenBox()
	{
		base.OnPressOpenBox();
		SetOpenCloseBox(!m_IsBoxOpened, isPlayer: true);
	}

	public void SetOpenCloseBox(bool isOpen, bool isPlayer)
	{
		if (m_IsTogglingOpenClose)
		{
			return;
		}
		m_IsTogglingOpenClose = true;
		m_StaticMeshGrp.gameObject.SetActive(value: false);
		m_RigMeshGrp.gameObject.SetActive(value: true);
		CSingleton<RestockManager>.Instance.AddToDelayedResetOpenCloseList(this);
		m_IsBoxOpened = isOpen;
		if (!m_IsBoxOpened)
		{
			m_BoxAnim.Play("Close");
			if (m_ItemCompartment.GetItemType() != EItemType.None && m_ItemCompartment.GetItemCount() > 0)
			{
				StartCoroutine(DelaySetPriceTagVisibility(0.85f, isVisible: true));
			}
			if (isPlayer)
			{
				SoundManager.PlayAudio("SFX_BoxClose", 0.5f);
			}
			m_PreventWorkerTakeBox = false;
		}
		else
		{
			m_BoxAnim.Play("Open");
			SetPriceTagVisibility(isVisible: false);
			if (!m_IsFirstOpenDone)
			{
				m_ItemCompartment.SpawnItem(m_ItemAmountToSpawn, spawnFromFront: false);
				m_IsFirstOpenDone = true;
				m_ItemCompartment.CalculatePositionList();
				m_ItemCompartment.RefreshItemPosition(spawnFromFront: false);
			}
			m_ItemCompartment.gameObject.SetActive(value: true);
			if (isPlayer)
			{
				m_PreventWorkerTakeBox = true;
				SoundManager.PlayAudio("SFX_BoxOpen", 0.5f);
			}
		}
		if (isPlayer)
		{
			UpdateToolTip();
		}
	}

	public void ResetToggleOpenClose()
	{
		m_IsTogglingOpenClose = false;
		if (!m_IsBoxOpened)
		{
			m_ItemCompartment.gameObject.SetActive(value: false);
		}
		m_StaticMeshGrp.gameObject.SetActive(value: true);
		m_RigMeshGrp.gameObject.SetActive(value: false);
		m_ClosedBox.SetActive(!m_IsBoxOpened);
		m_OpenBox.SetActive(m_IsBoxOpened);
		m_OutlineClosedBox.SetActive(!m_IsBoxOpened);
		m_OutlineOpenBox.SetActive(m_IsBoxOpened);
	}

	public void ForceSetOpenCloseInstant(bool isOpen)
	{
		m_IsBoxOpened = isOpen;
		ResetToggleOpenClose();
	}

	protected IEnumerator DelayResetToggleOpenClose()
	{
		m_StaticMeshGrp.gameObject.SetActive(value: false);
		m_RigMeshGrp.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(0.85f);
		m_IsTogglingOpenClose = false;
		if (!m_IsBoxOpened)
		{
			m_ItemCompartment.gameObject.SetActive(value: false);
		}
		m_StaticMeshGrp.gameObject.SetActive(value: true);
		m_RigMeshGrp.gameObject.SetActive(value: false);
		m_ClosedBox.SetActive(!m_IsBoxOpened);
		m_OpenBox.SetActive(m_IsBoxOpened);
		m_OutlineClosedBox.SetActive(!m_IsBoxOpened);
		m_OutlineOpenBox.SetActive(m_IsBoxOpened);
	}

	private void UpdateToolTip()
	{
		InteractionPlayerController.RemoveToolTip(EGameAction.OpenBox);
		InteractionPlayerController.RemoveToolTip(EGameAction.CloseBox);
		if (m_IsBoxOpened)
		{
			InteractionPlayerController.AddToolTip(EGameAction.CloseBox);
		}
		else
		{
			InteractionPlayerController.AddToolTip(EGameAction.OpenBox);
		}
	}

	public override void OnDestroyed()
	{
		m_ItemCompartment.SetIgnoreCull(ignoreCull: false);
		m_ItemCompartment.DisableAllItem();
		RestockManager.RemoveItemPackageBox(this);
		base.OnDestroyed();
	}

	protected override void SetPriceTagVisibility(bool isVisible)
	{
		if (!isVisible || !m_IsStored)
		{
			base.SetPriceTagVisibility(isVisible);
			for (int i = 0; i < m_ItemCompartment.m_InteractablePriceTagList.Count; i++)
			{
				m_ItemCompartment.m_InteractablePriceTagList[i].SetVisibility(isVisible);
			}
		}
	}

	public void UpdateStoredWarehouseShelfIndex()
	{
		if ((bool)m_BoxStoredCompartment)
		{
			m_StoredWarehouseShelfIndex = m_BoxStoredCompartment.GetWarehouseIndex();
			m_StorageCompartmentIndex = m_BoxStoredCompartment.GetIndex();
		}
	}

	public int GeStoredWarehouseShelfIndex()
	{
		return m_StoredWarehouseShelfIndex;
	}

	public int GetStorageCompartmentIndex()
	{
		return m_StorageCompartmentIndex;
	}

	public override bool IsBoxOpened()
	{
		if (m_IsBoxOpened)
		{
			return !m_IsTogglingOpenClose;
		}
		return false;
	}

	public bool IsTogglingOpenClose()
	{
		return m_IsTogglingOpenClose;
	}

	public ShelfCompartment GetBoxStoredCompartment()
	{
		return m_BoxStoredCompartment;
	}
}
