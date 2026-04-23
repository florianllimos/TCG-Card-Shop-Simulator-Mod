using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAutoPackOpener : InteractableObject
{
	public Transform m_Pos;

	public Transform m_PosInside;

	public Transform m_UIPos;

	public Transform m_StandLoc;

	public int m_MaxPackCount = 10;

	public float m_PackOpenTime = 10f;

	public float m_UIPosYOffset = 1f;

	private bool m_IsProcessing;

	private int m_PackOpenedCount;

	public int m_CurrentState;

	private float m_Timer;

	private float m_PackOpenTimer;

	private float m_PackOpenTimerSecond;

	private AutoCardOpenerUI m_AutoCardOpenerUI;

	private List<Item> m_StoredItemList = new List<Item>();

	private List<CompactCardDataAmount> m_CompactCardDataAmountList = new List<CompactCardDataAmount>();

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitAutoPackOpener(this);
		m_AutoCardOpenerUI = CSingleton<AutoCardOpenerUISpawner>.Instance.GetAutoCardOpenerUI();
		m_AutoCardOpenerUI.SetVisibility(isVisible: true);
		m_AutoCardOpenerUI.SetUIState(0);
		m_AutoCardOpenerUI.UpdatePackCountText(0, m_MaxPackCount);
		SetUITransform();
	}

	private void SetUITransform()
	{
		m_AutoCardOpenerUI.transform.position = m_UIPos.transform.position;
		m_AutoCardOpenerUI.transform.rotation = m_UIPos.transform.rotation;
		m_AutoCardOpenerUI.transform.localScale = m_UIPos.transform.localScale;
	}

	public override void OnMouseButtonUp()
	{
		base.OnMouseButtonUp();
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
		if (!m_IsProcessing)
		{
			if (m_StoredItemList.Count > 0)
			{
				m_IsProcessing = true;
				m_CurrentState = 1;
				m_AutoCardOpenerUI.SetUIState(1);
				m_AutoCardOpenerUI.UpdateProcessingFillBar(1f - (float)m_StoredItemList.Count / (float)m_MaxPackCount);
				m_AutoCardOpenerUI.UpdateProcessingTimeLeftText(m_PackOpenTime * (float)m_StoredItemList.Count);
				InteractionPlayerController.RemoveToolTip(EGameAction.TurnOn);
				SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NoCardPackInMachine);
			}
		}
		else if (m_CompactCardDataAmountList.Count > 0)
		{
			if (m_StoredItemList.Count <= 0)
			{
				CPlayerData.m_GameReportDataCollect.cardPackOpened += m_PackOpenedCount;
				CPlayerData.m_GameReportDataCollectPermanent.cardPackOpened += m_PackOpenedCount;
				AchievementManager.OnCardPackOpened(CPlayerData.m_GameReportDataCollectPermanent.cardPackOpened);
				CSingleton<InteractionPlayerController>.Instance.m_ShowCardObtainedPage.ShowCardObtained(m_CompactCardDataAmountList);
				m_PackOpenedCount = 0;
				m_CompactCardDataAmountList.Clear();
				m_CurrentState = 0;
				m_AutoCardOpenerUI.SetUIState(0);
				m_AutoCardOpenerUI.UpdatePackCountText(0, m_MaxPackCount);
				m_IsProcessing = false;
				InteractionPlayerController.RemoveToolTip(EGameAction.Collect);
				SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
				SoundManager.PlayAudio("SFX_PercStarJingle3", 0.6f);
				SoundManager.PlayAudio("SFX_Gift", 0.6f);
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.WaitAllCardPacksToBeProcessed);
			}
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.WaitAllCardPacksToBeProcessed);
		}
	}

	public override void OnRightMouseButtonUp()
	{
		base.OnRightMouseButtonUp();
	}

	protected override void Update()
	{
		base.Update();
		if (m_IsBoxedUp || !m_IsProcessing || m_StoredItemList.Count <= 0)
		{
			return;
		}
		m_PackOpenTimer += Time.deltaTime;
		m_PackOpenTimerSecond += Time.deltaTime;
		if (m_PackOpenTimerSecond >= 1f)
		{
			m_PackOpenTimerSecond -= 1f;
			m_AutoCardOpenerUI.UpdateProcessingTimeLeftText(m_PackOpenTime * (float)m_StoredItemList.Count - m_PackOpenTimer);
		}
		if (m_PackOpenTimer >= m_PackOpenTime)
		{
			m_PackOpenTimer = 0f;
			OpenPack();
			RemoveItem(m_StoredItemList[0]);
			m_AutoCardOpenerUI.UpdateProcessingFillBar(1f - (float)m_StoredItemList.Count / (float)m_MaxPackCount);
			if (m_StoredItemList.Count <= 0)
			{
				m_CurrentState = 2;
				m_AutoCardOpenerUI.SetUIState(2);
			}
		}
	}

	private void OpenPack()
	{
		m_PackOpenedCount++;
		ECollectionPackType collectionPackType = InventoryBase.ItemTypeToCollectionPackType(m_StoredItemList[0].GetItemType());
		InventoryBase.GetCardExpansionType(collectionPackType);
		List<CardData> packContentCardDataList = CSingleton<CardOpeningSequence>.Instance.GetPackContentCardDataList(collectionPackType);
		for (int i = 0; i < packContentCardDataList.Count; i++)
		{
			bool flag = false;
			int cardSaveIndex = CPlayerData.GetCardSaveIndex(packContentCardDataList[i]);
			for (int j = 0; j < m_CompactCardDataAmountList.Count; j++)
			{
				if (m_CompactCardDataAmountList[j].cardSaveIndex == cardSaveIndex && m_CompactCardDataAmountList[j].expansionType == packContentCardDataList[i].expansionType && m_CompactCardDataAmountList[j].isDestiny == packContentCardDataList[i].isDestiny)
				{
					flag = true;
					m_CompactCardDataAmountList[j].amount++;
					break;
				}
			}
			if (!flag)
			{
				CompactCardDataAmount compactCardDataAmount = new CompactCardDataAmount();
				compactCardDataAmount.expansionType = packContentCardDataList[i].expansionType;
				compactCardDataAmount.isDestiny = packContentCardDataList[i].isDestiny;
				compactCardDataAmount.cardSaveIndex = cardSaveIndex;
				compactCardDataAmount.amount = 1;
				m_CompactCardDataAmountList.Add(compactCardDataAmount);
			}
		}
	}

	public override void OnRaycasted()
	{
		base.OnRaycasted();
		m_IsRaycasted = true;
		if (m_IsProcessing)
		{
			InteractionPlayerController.AddToolTip(EGameAction.Collect);
		}
		else
		{
			InteractionPlayerController.AddToolTip(EGameAction.TurnOn);
		}
	}

	public override void OnRaycastEnded()
	{
		base.OnRaycastEnded();
		m_IsRaycasted = false;
		InteractionPlayerController.RemoveToolTip(EGameAction.Collect);
		InteractionPlayerController.RemoveToolTip(EGameAction.TurnOn);
	}

	public void DispenseItemFromBox(InteractablePackagingBox_Item itemBox, bool isPlayer)
	{
		if (isPlayer && m_IsProcessing)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotAddMachineRunning);
		}
		else if ((bool)itemBox && itemBox.IsBoxOpened())
		{
			if (CSingleton<InteractionPlayerController>.Instance.IsCardPackType(itemBox.m_ItemCompartment.GetItemType()))
			{
				if (itemBox.m_ItemCompartment.GetItemCount() > 0)
				{
					if (HasEnoughSlot())
					{
						Item firstItem = itemBox.m_ItemCompartment.GetFirstItem();
						AddItem(firstItem, addToFront: true, isPlayer);
						itemBox.m_ItemCompartment.RemoveItem(firstItem);
						if (isPlayer)
						{
							SoundManager.GenericPop();
						}
					}
					else if (isPlayer)
					{
						NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserNoSlot);
					}
				}
				else if (isPlayer)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxNoItem);
				}
			}
			else if (isPlayer)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.MachineOnlyAcceptCardPacks);
			}
		}
		else if ((bool)itemBox && !itemBox.IsBoxOpened())
		{
			itemBox.OnPressOpenBox();
		}
	}

	public void AddItem(Item item, bool addToFront, bool isPlayer)
	{
		if (isPlayer && m_IsProcessing)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotAddMachineRunning);
			return;
		}
		if (addToFront)
		{
			m_StoredItemList.Insert(0, item);
		}
		else
		{
			m_StoredItemList.Add(item);
		}
		item.LerpToTransform(m_Pos, m_Pos);
		StartCoroutine(DelayGoInside(item));
		m_AutoCardOpenerUI.UpdatePackCountText(m_StoredItemList.Count, m_MaxPackCount);
		if (!m_IsProcessing && m_StoredItemList.Count >= m_MaxPackCount)
		{
			m_IsProcessing = true;
			m_AutoCardOpenerUI.SetUIState(1);
			m_AutoCardOpenerUI.UpdateProcessingFillBar(1f - (float)m_StoredItemList.Count / (float)m_MaxPackCount);
			m_AutoCardOpenerUI.UpdateProcessingTimeLeftText(m_PackOpenTime * (float)m_StoredItemList.Count);
			if (isPlayer)
			{
				SoundManager.PlayAudio("SFX_ButtonLightTap", 0.6f, 0.5f);
			}
		}
	}

	private IEnumerator DelayGoInside(Item item)
	{
		yield return new WaitForSeconds(0.4f);
		item.LerpToTransform(m_PosInside, m_PosInside, ignoreUpForce: true);
	}

	public void RemoveItem(Item item)
	{
		m_StoredItemList.Remove(item);
	}

	public Item TakeItemToHand(bool getLastItem = true)
	{
		if (m_StoredItemList.Count <= 0)
		{
			return null;
		}
		Item item = GetLastItem();
		if (!getLastItem)
		{
			item = GetFirstItem();
		}
		m_StoredItemList.Remove(item);
		_ = m_IsRaycasted;
		return item;
	}

	public override void BoxUpObject(bool holdBox)
	{
		base.BoxUpObject(holdBox);
		m_AutoCardOpenerUI.SetVisibility(isVisible: false);
	}

	protected override void OnStartMoveObject()
	{
		m_AutoCardOpenerUI.SetVisibility(isVisible: false);
		base.OnStartMoveObject();
	}

	protected override void OnPlacedMovedObject()
	{
		m_AutoCardOpenerUI.SetVisibility(isVisible: true);
		base.OnPlacedMovedObject();
		SetUITransform();
	}

	public override void OnDestroyed()
	{
		m_AutoCardOpenerUI.gameObject.SetActive(value: false);
		m_AutoCardOpenerUI = null;
		ShelfManager.RemoveAutoPackOpener(this);
		base.OnDestroyed();
	}

	public bool HasEnoughSlot()
	{
		if (m_StoredItemList.Count + m_PackOpenedCount < m_MaxPackCount)
		{
			return true;
		}
		return false;
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
		return m_StoredItemList[m_StoredItemList.Count - 1];
	}

	public float GetTimer()
	{
		return m_Timer;
	}

	public int GetPackOpenedCount()
	{
		return m_PackOpenedCount;
	}

	public bool GetIsProcessing()
	{
		return m_IsProcessing;
	}

	public List<Item> GetStoredItemList()
	{
		return m_StoredItemList;
	}

	public List<CompactCardDataAmount> GetCompactCardDataAmountList()
	{
		return m_CompactCardDataAmountList;
	}

	public void LoadData(AutoPackOpenerSaveData saveData)
	{
		m_Timer = 0f;
		m_CompactCardDataAmountList = saveData.compactCardDataAmountList;
		m_PackOpenedCount = saveData.packOpenedCount;
		for (int i = 0; i < saveData.itemTypeList.Count; i++)
		{
			Item item = null;
			ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(saveData.itemTypeList[i]);
			item = ItemSpawnManager.GetItem(m_PosInside);
			item.SetMesh(itemMeshData.mesh, itemMeshData.material, saveData.itemTypeList[i], itemMeshData.meshSecondary, itemMeshData.materialSecondary);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			item.gameObject.SetActive(value: true);
			AddItem(item, addToFront: true, isPlayer: false);
		}
		m_IsProcessing = saveData.isProcessing;
		if (!m_IsProcessing && m_StoredItemList.Count >= m_MaxPackCount)
		{
			m_IsProcessing = true;
		}
		if (m_IsProcessing)
		{
			if (m_StoredItemList.Count <= 0)
			{
				m_CurrentState = 2;
				m_AutoCardOpenerUI.SetUIState(2);
			}
			else
			{
				m_CurrentState = 1;
				m_AutoCardOpenerUI.SetUIState(1);
				m_AutoCardOpenerUI.UpdateProcessingTimeLeftText(m_PackOpenTime * (float)m_StoredItemList.Count);
				m_AutoCardOpenerUI.UpdateProcessingFillBar(1f - (float)m_StoredItemList.Count / (float)m_MaxPackCount);
			}
		}
		else
		{
			m_CurrentState = 0;
			m_AutoCardOpenerUI.SetUIState(0);
		}
		SetUITransform();
	}
}
