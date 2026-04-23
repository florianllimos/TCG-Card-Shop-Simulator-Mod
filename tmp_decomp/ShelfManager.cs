using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : CSingleton<ShelfManager>
{
	public static ShelfManager m_Instance;

	public Transform m_ShelfParentGrp;

	public BoxCollider m_MoveObjectCustomerBlocker;

	public MeshFilter m_MoveObjectPreviewModel;

	public MeshRenderer m_MoveObjectPreviewRenderer;

	private MeshFilter m_MoveObjectTargetMesh;

	private Material m_MoveObjectPreviewModelMat;

	public Color m_PreviewMeshValidColor;

	public Color m_PreviewMeshInvalidColor;

	private List<DashedLine> m_DashedLineList = new List<DashedLine>();

	public List<InteractableObject> m_InteractableObjectList = new List<InteractableObject>();

	public List<InteractableObject> m_DecoObjectList = new List<InteractableObject>();

	public List<Shelf> m_ShelfList = new List<Shelf>();

	public List<WarehouseShelf> m_WarehouseShelfList = new List<WarehouseShelf>();

	public List<InteractableCashierCounter> m_CashierCounterList = new List<InteractableCashierCounter>();

	public List<CardShelf> m_CardShelfList = new List<CardShelf>();

	public List<InteractablePlayTable> m_PlayTableList = new List<InteractablePlayTable>();

	public List<InteractableAutoCleanser> m_AutoCleanserList = new List<InteractableAutoCleanser>();

	public List<InteractableAutoPackOpener> m_AutoPackOpenerList = new List<InteractableAutoPackOpener>();

	public List<InteractableWorkbench> m_WorkbenchList = new List<InteractableWorkbench>();

	public List<InteractableTrashBin> m_TrashBinList = new List<InteractableTrashBin>();

	public List<InteractableEmptyBoxStorage> m_EmptyBoxStorageList = new List<InteractableEmptyBoxStorage>();

	public List<InteractableBulkDonationBox> m_BulkDonationBoxList = new List<InteractableBulkDonationBox>();

	public List<InteractableCardStorageShelf> m_CardStorageShelfList = new List<InteractableCardStorageShelf>();

	public List<ShelfSaveData> m_ShelfSaveDataList = new List<ShelfSaveData>();

	public List<WarehouseShelfSaveData> m_WarehouseShelfSaveDataList = new List<WarehouseShelfSaveData>();

	public List<PackageBoxItemaveData> m_PackageBoxItemSaveDataList = new List<PackageBoxItemaveData>();

	public List<PackageBoxCardSaveData> m_PackageBoxCardSaveDataList = new List<PackageBoxCardSaveData>();

	public List<InteractableObjectSaveData> m_InteractableObjectSaveDataList = new List<InteractableObjectSaveData>();

	public List<DecoObjectSaveData> m_DecoObjectSaveDataList = new List<DecoObjectSaveData>();

	public List<CardShelfSaveData> m_CardShelfSaveDataList = new List<CardShelfSaveData>();

	public List<PlayTableSaveData> m_PlayTableSaveDataList = new List<PlayTableSaveData>();

	public List<AutoCleanserSaveData> m_AutoCleanserSaveDataList = new List<AutoCleanserSaveData>();

	public List<AutoPackOpenerSaveData> m_AutoPackOpenerSaveDataList = new List<AutoPackOpenerSaveData>();

	public List<WorkbenchSaveData> m_WorkbenchSaveDataList = new List<WorkbenchSaveData>();

	public List<CashierCounterSaveData> m_CashCounterSaveDataList = new List<CashierCounterSaveData>();

	public List<EmptyBoxStorageSaveData> m_EmptyBoxStorageSaveDataList = new List<EmptyBoxStorageSaveData>();

	public List<BulkDonationBoxSaveData> m_BulkDonationSaveDataList = new List<BulkDonationBoxSaveData>();

	public List<CardStorageShelfSaveData> m_CardStorageShelfSaveDataList = new List<CardStorageShelfSaveData>();

	private List<Shelf> m_SpawnedShelfList = new List<Shelf>();

	private List<WarehouseShelf> m_SpawnedWarehouseShelfList = new List<WarehouseShelf>();

	private List<InteractablePackagingBox_Item> m_SpawnedPackageBoxItemList = new List<InteractablePackagingBox_Item>();

	private List<InteractablePackagingBox_Card> m_SpawnedPackageBoxCardList = new List<InteractablePackagingBox_Card>();

	private List<InteractableObject> m_SpawnedInteractableObjectList = new List<InteractableObject>();

	private List<InteractableObject> m_SpawnedDecoObjectList = new List<InteractableObject>();

	private List<CardShelf> m_SpawnedCardShelfList = new List<CardShelf>();

	private List<InteractablePlayTable> m_SpawnedPlayTableList = new List<InteractablePlayTable>();

	private List<InteractableAutoCleanser> m_SpawnedAutoCleanserList = new List<InteractableAutoCleanser>();

	private List<InteractableAutoPackOpener> m_SpawnedAutoPackOpenerList = new List<InteractableAutoPackOpener>();

	private List<InteractableWorkbench> m_SpawnedWorkbenchList = new List<InteractableWorkbench>();

	private List<InteractableCashierCounter> m_SpawnedCashierCounterList = new List<InteractableCashierCounter>();

	private List<InteractableEmptyBoxStorage> m_SpawnedEmptyBoxStorageList = new List<InteractableEmptyBoxStorage>();

	private List<InteractableBulkDonationBox> m_SpawnedBulkDonationBoxList = new List<InteractableBulkDonationBox>();

	private List<InteractableCardStorageShelf> m_SpawnedCardStorageShelfList = new List<InteractableCardStorageShelf>();

	private int m_SpawnedObjectCount;

	public bool m_FinishLoadingObjectData;

	private float m_CullTimer;

	public void SaveInteractableObjectData(bool ignoreFinishLoadingCheck = false)
	{
		if (!ignoreFinishLoadingCheck && !m_FinishLoadingObjectData)
		{
			Debug.Log("Dont SaveInteractableObjectData, havent finish load data");
			return;
		}
		CSingleton<CustomerManager>.Instance.SaveCustomerData();
		CSingleton<WorkerManager>.Instance.SaveWorkerData();
		m_InteractableObjectSaveDataList.Clear();
		for (int i = 0; i < m_InteractableObjectList.Count; i++)
		{
			if ((bool)m_InteractableObjectList[i] && m_InteractableObjectList[i].m_IsGenericObject)
			{
				InteractableObjectSaveData interactableObjectSaveData = new InteractableObjectSaveData();
				interactableObjectSaveData.pos.SetData(m_InteractableObjectList[i].transform.position);
				interactableObjectSaveData.rot.SetData(m_InteractableObjectList[i].transform.rotation);
				interactableObjectSaveData.objectType = m_InteractableObjectList[i].m_ObjectType;
				interactableObjectSaveData.isBoxed = m_InteractableObjectList[i].GetIsBoxedUp();
				if (interactableObjectSaveData.isBoxed)
				{
					interactableObjectSaveData.boxedPackagePos.SetData(m_InteractableObjectList[i].GetPackagingBoxShelf().transform.position);
					interactableObjectSaveData.boxedPackageRot.SetData(m_InteractableObjectList[i].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_InteractableObjectList[i].GetIsMovingObject())
				{
					interactableObjectSaveData.isBoxed = true;
					interactableObjectSaveData.boxedPackagePos.SetData(m_InteractableObjectList[i].transform.position);
					interactableObjectSaveData.boxedPackageRot.SetData(m_InteractableObjectList[i].transform.rotation);
				}
				m_InteractableObjectSaveDataList.Add(interactableObjectSaveData);
			}
		}
		m_ShelfSaveDataList.Clear();
		for (int j = 0; j < m_ShelfList.Count; j++)
		{
			if ((bool)m_ShelfList[j])
			{
				ShelfSaveData shelfSaveData = new ShelfSaveData();
				List<ItemTypeAmount> list = new List<ItemTypeAmount>();
				for (int k = 0; k < m_ShelfList[j].GetItemCompartmentList().Count; k++)
				{
					ItemTypeAmount itemTypeAmount = new ItemTypeAmount();
					itemTypeAmount.itemType = m_ShelfList[j].GetItemCompartmentList()[k].GetItemType();
					itemTypeAmount.amount = m_ShelfList[j].GetItemCompartmentList()[k].GetItemCount();
					list.Add(itemTypeAmount);
				}
				shelfSaveData.itemTypeAmountList = list;
				shelfSaveData.pos.SetData(m_ShelfList[j].transform.position);
				shelfSaveData.rot.SetData(m_ShelfList[j].transform.rotation);
				shelfSaveData.objectType = m_ShelfList[j].m_ObjectType;
				shelfSaveData.isBoxed = m_ShelfList[j].GetIsBoxedUp();
				if (shelfSaveData.isBoxed)
				{
					shelfSaveData.boxedPackagePos.SetData(m_ShelfList[j].GetPackagingBoxShelf().transform.position);
					shelfSaveData.boxedPackageRot.SetData(m_ShelfList[j].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_ShelfList[j].GetIsMovingObject())
				{
					shelfSaveData.isBoxed = true;
					shelfSaveData.boxedPackagePos.SetData(m_ShelfList[j].transform.position);
					shelfSaveData.boxedPackageRot.SetData(m_ShelfList[j].transform.rotation);
				}
				m_ShelfSaveDataList.Add(shelfSaveData);
			}
		}
		m_WarehouseShelfSaveDataList.Clear();
		for (int l = 0; l < m_WarehouseShelfList.Count; l++)
		{
			if ((bool)m_WarehouseShelfList[l])
			{
				WarehouseShelfSaveData warehouseShelfSaveData = new WarehouseShelfSaveData();
				warehouseShelfSaveData.pos.SetData(m_WarehouseShelfList[l].transform.position);
				warehouseShelfSaveData.rot.SetData(m_WarehouseShelfList[l].transform.rotation);
				warehouseShelfSaveData.objectType = m_WarehouseShelfList[l].m_ObjectType;
				warehouseShelfSaveData.isBoxed = m_WarehouseShelfList[l].GetIsBoxedUp();
				if (warehouseShelfSaveData.isBoxed)
				{
					warehouseShelfSaveData.boxedPackagePos.SetData(m_WarehouseShelfList[l].GetPackagingBoxShelf().transform.position);
					warehouseShelfSaveData.boxedPackageRot.SetData(m_WarehouseShelfList[l].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_WarehouseShelfList[l].GetIsMovingObject())
				{
					warehouseShelfSaveData.isBoxed = true;
					warehouseShelfSaveData.boxedPackagePos.SetData(m_WarehouseShelfList[l].transform.position);
					warehouseShelfSaveData.boxedPackageRot.SetData(m_WarehouseShelfList[l].transform.rotation);
				}
				warehouseShelfSaveData.compartmentItemType = new List<EItemType>();
				for (int m = 0; m < m_WarehouseShelfList[l].GetStorageCompartmentList().Count; m++)
				{
					warehouseShelfSaveData.compartmentItemType.Add(m_WarehouseShelfList[l].GetStorageCompartmentList()[m].GetShelfCompartment().GetItemType());
				}
				m_WarehouseShelfSaveDataList.Add(warehouseShelfSaveData);
			}
		}
		m_CardShelfSaveDataList.Clear();
		for (int n = 0; n < m_CardShelfList.Count; n++)
		{
			if (!m_CardShelfList[n])
			{
				continue;
			}
			CardShelfSaveData cardShelfSaveData = new CardShelfSaveData();
			List<CardData> list2 = new List<CardData>();
			cardShelfSaveData.pos.SetData(m_CardShelfList[n].transform.position);
			cardShelfSaveData.rot.SetData(m_CardShelfList[n].transform.rotation);
			cardShelfSaveData.objectType = m_CardShelfList[n].m_ObjectType;
			cardShelfSaveData.isVerticalSnapToWarehouseWall = m_CardShelfList[n].GetIsVerticalSnapToWarehouseWall();
			cardShelfSaveData.verticalSnapWallIndex = m_CardShelfList[n].GetVerticalSnapWallIndex();
			cardShelfSaveData.isBoxed = m_CardShelfList[n].GetIsBoxedUp();
			if (cardShelfSaveData.isBoxed)
			{
				cardShelfSaveData.boxedPackagePos.SetData(m_CardShelfList[n].GetPackagingBoxShelf().transform.position);
				cardShelfSaveData.boxedPackageRot.SetData(m_CardShelfList[n].GetPackagingBoxShelf().transform.rotation);
			}
			if (m_CardShelfList[n].GetIsMovingObject())
			{
				cardShelfSaveData.isBoxed = true;
				cardShelfSaveData.boxedPackagePos.SetData(m_CardShelfList[n].transform.position);
				cardShelfSaveData.boxedPackageRot.SetData(m_CardShelfList[n].transform.rotation);
			}
			for (int num = 0; num < m_CardShelfList[n].GetCardCompartmentList().Count; num++)
			{
				if (m_CardShelfList[n].GetCardCompartmentList()[num].m_StoredCardList.Count > 0)
				{
					list2.Add(m_CardShelfList[n].GetCardCompartmentList()[num].m_StoredCardList[0].m_Card3dUI.m_CardUI.GetCardData());
				}
				else
				{
					list2.Add(null);
				}
			}
			cardShelfSaveData.cardDataList = list2;
			m_CardShelfSaveDataList.Add(cardShelfSaveData);
		}
		m_PlayTableSaveDataList.Clear();
		for (int num2 = 0; num2 < m_PlayTableList.Count; num2++)
		{
			if ((bool)m_PlayTableList[num2])
			{
				PlayTableSaveData playTableSaveData = new PlayTableSaveData();
				playTableSaveData.pos.SetData(m_PlayTableList[num2].transform.position);
				playTableSaveData.rot.SetData(m_PlayTableList[num2].transform.rotation);
				playTableSaveData.objectType = m_PlayTableList[num2].m_ObjectType;
				playTableSaveData.isBoxed = m_PlayTableList[num2].GetIsBoxedUp();
				if (playTableSaveData.isBoxed)
				{
					playTableSaveData.boxedPackagePos.SetData(m_PlayTableList[num2].GetPackagingBoxShelf().transform.position);
					playTableSaveData.boxedPackageRot.SetData(m_PlayTableList[num2].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_PlayTableList[num2].GetIsMovingObject())
				{
					playTableSaveData.isBoxed = true;
					playTableSaveData.boxedPackagePos.SetData(m_PlayTableList[num2].transform.position);
					playTableSaveData.boxedPackageRot.SetData(m_PlayTableList[num2].transform.rotation);
				}
				playTableSaveData.hasStartPlay = m_PlayTableList[num2].GetHasStartPlay();
				playTableSaveData.isSeatOccupied = m_PlayTableList[num2].GetIsSeatOccupied();
				playTableSaveData.isCustomerSmelly = m_PlayTableList[num2].GetIsCustomerSmelly();
				playTableSaveData.playTableFee = m_PlayTableList[num2].GetPlayTableFee();
				playTableSaveData.currentPlayerCount = m_PlayTableList[num2].GetCurrentPlayerCount();
				playTableSaveData.currentPlayTime = m_PlayTableList[num2].GetCurrentPlayTime();
				playTableSaveData.currentPlayTimeMax = m_PlayTableList[num2].GetCurrentPlayTimeMax();
				m_PlayTableSaveDataList.Add(playTableSaveData);
			}
		}
		m_AutoCleanserSaveDataList.Clear();
		for (int num3 = 0; num3 < m_AutoCleanserList.Count; num3++)
		{
			if ((bool)m_AutoCleanserList[num3])
			{
				AutoCleanserSaveData autoCleanserSaveData = new AutoCleanserSaveData();
				autoCleanserSaveData.isTurnedOn = m_AutoCleanserList[num3].IsTurnedOn();
				autoCleanserSaveData.isNeedRefill = m_AutoCleanserList[num3].IsNeedRefill();
				autoCleanserSaveData.itemAmount = m_AutoCleanserList[num3].GetStoredItemList().Count;
				List<float> list3 = new List<float>();
				for (int num4 = 0; num4 < m_AutoCleanserList[num3].GetStoredItemList().Count; num4++)
				{
					list3.Add(m_AutoCleanserList[num3].GetStoredItemList()[num4].GetContentFill());
				}
				autoCleanserSaveData.contentFillList = list3;
				autoCleanserSaveData.pos.SetData(m_AutoCleanserList[num3].transform.position);
				autoCleanserSaveData.rot.SetData(m_AutoCleanserList[num3].transform.rotation);
				autoCleanserSaveData.objectType = m_AutoCleanserList[num3].m_ObjectType;
				autoCleanserSaveData.isBoxed = m_AutoCleanserList[num3].GetIsBoxedUp();
				if (autoCleanserSaveData.isBoxed)
				{
					autoCleanserSaveData.boxedPackagePos.SetData(m_AutoCleanserList[num3].GetPackagingBoxShelf().transform.position);
					autoCleanserSaveData.boxedPackageRot.SetData(m_AutoCleanserList[num3].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_AutoCleanserList[num3].GetIsMovingObject())
				{
					autoCleanserSaveData.isBoxed = true;
					autoCleanserSaveData.boxedPackagePos.SetData(m_AutoCleanserList[num3].transform.position);
					autoCleanserSaveData.boxedPackageRot.SetData(m_AutoCleanserList[num3].transform.rotation);
				}
				m_AutoCleanserSaveDataList.Add(autoCleanserSaveData);
			}
		}
		m_AutoPackOpenerSaveDataList.Clear();
		for (int num5 = 0; num5 < m_AutoPackOpenerList.Count; num5++)
		{
			if ((bool)m_AutoPackOpenerList[num5])
			{
				AutoPackOpenerSaveData autoPackOpenerSaveData = new AutoPackOpenerSaveData();
				autoPackOpenerSaveData.isProcessing = m_AutoPackOpenerList[num5].GetIsProcessing();
				autoPackOpenerSaveData.packOpenedCount = m_AutoPackOpenerList[num5].GetPackOpenedCount();
				List<EItemType> list4 = new List<EItemType>();
				for (int num6 = 0; num6 < m_AutoPackOpenerList[num5].GetStoredItemList().Count; num6++)
				{
					list4.Add(m_AutoPackOpenerList[num5].GetStoredItemList()[num6].GetItemType());
				}
				autoPackOpenerSaveData.itemTypeList = list4;
				autoPackOpenerSaveData.compactCardDataAmountList = m_AutoPackOpenerList[num5].GetCompactCardDataAmountList();
				autoPackOpenerSaveData.pos.SetData(m_AutoPackOpenerList[num5].transform.position);
				autoPackOpenerSaveData.rot.SetData(m_AutoPackOpenerList[num5].transform.rotation);
				autoPackOpenerSaveData.objectType = m_AutoPackOpenerList[num5].m_ObjectType;
				autoPackOpenerSaveData.isBoxed = m_AutoPackOpenerList[num5].GetIsBoxedUp();
				if (autoPackOpenerSaveData.isBoxed)
				{
					autoPackOpenerSaveData.boxedPackagePos.SetData(m_AutoPackOpenerList[num5].GetPackagingBoxShelf().transform.position);
					autoPackOpenerSaveData.boxedPackageRot.SetData(m_AutoPackOpenerList[num5].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_AutoPackOpenerList[num5].GetIsMovingObject())
				{
					autoPackOpenerSaveData.isBoxed = true;
					autoPackOpenerSaveData.boxedPackagePos.SetData(m_AutoPackOpenerList[num5].transform.position);
					autoPackOpenerSaveData.boxedPackageRot.SetData(m_AutoPackOpenerList[num5].transform.rotation);
				}
				m_AutoPackOpenerSaveDataList.Add(autoPackOpenerSaveData);
			}
		}
		m_EmptyBoxStorageSaveDataList.Clear();
		for (int num7 = 0; num7 < m_EmptyBoxStorageList.Count; num7++)
		{
			if ((bool)m_EmptyBoxStorageList[num7])
			{
				EmptyBoxStorageSaveData emptyBoxStorageSaveData = new EmptyBoxStorageSaveData();
				emptyBoxStorageSaveData.storedBoxCount = m_EmptyBoxStorageList[num7].GetBoxStoredCount();
				emptyBoxStorageSaveData.pos.SetData(m_EmptyBoxStorageList[num7].transform.position);
				emptyBoxStorageSaveData.rot.SetData(m_EmptyBoxStorageList[num7].transform.rotation);
				emptyBoxStorageSaveData.objectType = m_EmptyBoxStorageList[num7].m_ObjectType;
				emptyBoxStorageSaveData.isBoxed = m_EmptyBoxStorageList[num7].GetIsBoxedUp();
				if (emptyBoxStorageSaveData.isBoxed)
				{
					emptyBoxStorageSaveData.boxedPackagePos.SetData(m_EmptyBoxStorageList[num7].GetPackagingBoxShelf().transform.position);
					emptyBoxStorageSaveData.boxedPackageRot.SetData(m_EmptyBoxStorageList[num7].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_EmptyBoxStorageList[num7].GetIsMovingObject())
				{
					emptyBoxStorageSaveData.isBoxed = true;
					emptyBoxStorageSaveData.boxedPackagePos.SetData(m_EmptyBoxStorageList[num7].transform.position);
					emptyBoxStorageSaveData.boxedPackageRot.SetData(m_EmptyBoxStorageList[num7].transform.rotation);
				}
				m_EmptyBoxStorageSaveDataList.Add(emptyBoxStorageSaveData);
			}
		}
		m_BulkDonationSaveDataList.Clear();
		for (int num8 = 0; num8 < m_BulkDonationBoxList.Count; num8++)
		{
			if ((bool)m_BulkDonationBoxList[num8])
			{
				BulkDonationBoxSaveData bulkDonationBoxSaveData = new BulkDonationBoxSaveData();
				bulkDonationBoxSaveData.compactCardDataAmountList = m_BulkDonationBoxList[num8].GetCompactCardDataAmountList();
				bulkDonationBoxSaveData.pos.SetData(m_BulkDonationBoxList[num8].transform.position);
				bulkDonationBoxSaveData.rot.SetData(m_BulkDonationBoxList[num8].transform.rotation);
				bulkDonationBoxSaveData.objectType = m_BulkDonationBoxList[num8].m_ObjectType;
				bulkDonationBoxSaveData.isBoxed = m_BulkDonationBoxList[num8].GetIsBoxedUp();
				if (bulkDonationBoxSaveData.isBoxed)
				{
					bulkDonationBoxSaveData.boxedPackagePos.SetData(m_BulkDonationBoxList[num8].GetPackagingBoxShelf().transform.position);
					bulkDonationBoxSaveData.boxedPackageRot.SetData(m_BulkDonationBoxList[num8].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_BulkDonationBoxList[num8].GetIsMovingObject())
				{
					bulkDonationBoxSaveData.isBoxed = true;
					bulkDonationBoxSaveData.boxedPackagePos.SetData(m_BulkDonationBoxList[num8].transform.position);
					bulkDonationBoxSaveData.boxedPackageRot.SetData(m_BulkDonationBoxList[num8].transform.rotation);
				}
				m_BulkDonationSaveDataList.Add(bulkDonationBoxSaveData);
			}
		}
		m_CardStorageShelfSaveDataList.Clear();
		for (int num9 = 0; num9 < m_CardStorageShelfList.Count; num9++)
		{
			if ((bool)m_CardStorageShelfList[num9])
			{
				CardStorageShelfSaveData cardStorageShelfSaveData = new CardStorageShelfSaveData();
				cardStorageShelfSaveData.compactCardDataAmountList = m_CardStorageShelfList[num9].GetCompactCardDataAmountList();
				cardStorageShelfSaveData.pos.SetData(m_CardStorageShelfList[num9].transform.position);
				cardStorageShelfSaveData.rot.SetData(m_CardStorageShelfList[num9].transform.rotation);
				cardStorageShelfSaveData.objectType = m_CardStorageShelfList[num9].m_ObjectType;
				cardStorageShelfSaveData.isBoxed = m_CardStorageShelfList[num9].GetIsBoxedUp();
				if (cardStorageShelfSaveData.isBoxed)
				{
					cardStorageShelfSaveData.boxedPackagePos.SetData(m_CardStorageShelfList[num9].GetPackagingBoxShelf().transform.position);
					cardStorageShelfSaveData.boxedPackageRot.SetData(m_CardStorageShelfList[num9].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_CardStorageShelfList[num9].GetIsMovingObject())
				{
					cardStorageShelfSaveData.isBoxed = true;
					cardStorageShelfSaveData.boxedPackagePos.SetData(m_CardStorageShelfList[num9].transform.position);
					cardStorageShelfSaveData.boxedPackageRot.SetData(m_CardStorageShelfList[num9].transform.rotation);
				}
				cardStorageShelfSaveData.isDisableWorkerTake = !m_CardStorageShelfList[num9].CanWorkerTake();
				m_CardStorageShelfSaveDataList.Add(cardStorageShelfSaveData);
			}
		}
		m_WorkbenchSaveDataList.Clear();
		for (int num10 = 0; num10 < m_WorkbenchList.Count; num10++)
		{
			if ((bool)m_WorkbenchList[num10])
			{
				WorkbenchSaveData workbenchSaveData = new WorkbenchSaveData();
				List<EItemType> list5 = new List<EItemType>();
				for (int num11 = 0; num11 < m_WorkbenchList[num10].m_StoredItemList.Count; num11++)
				{
					list5.Add(m_WorkbenchList[num10].m_StoredItemList[num11].GetItemType());
				}
				workbenchSaveData.itemTypeList = list5;
				workbenchSaveData.pos.SetData(m_WorkbenchList[num10].transform.position);
				workbenchSaveData.rot.SetData(m_WorkbenchList[num10].transform.rotation);
				workbenchSaveData.objectType = m_WorkbenchList[num10].m_ObjectType;
				workbenchSaveData.isBoxed = m_WorkbenchList[num10].GetIsBoxedUp();
				if (workbenchSaveData.isBoxed)
				{
					workbenchSaveData.boxedPackagePos.SetData(m_WorkbenchList[num10].GetPackagingBoxShelf().transform.position);
					workbenchSaveData.boxedPackageRot.SetData(m_WorkbenchList[num10].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_WorkbenchList[num10].GetIsMovingObject())
				{
					workbenchSaveData.isBoxed = true;
					workbenchSaveData.boxedPackagePos.SetData(m_WorkbenchList[num10].transform.position);
					workbenchSaveData.boxedPackageRot.SetData(m_WorkbenchList[num10].transform.rotation);
				}
				m_WorkbenchSaveDataList.Add(workbenchSaveData);
			}
		}
		m_CashCounterSaveDataList.Clear();
		for (int num12 = 0; num12 < m_CashierCounterList.Count; num12++)
		{
			if ((bool)m_CashierCounterList[num12])
			{
				CashierCounterSaveData cashierCounterSaveData = new CashierCounterSaveData();
				cashierCounterSaveData.pos.SetData(m_CashierCounterList[num12].transform.position);
				cashierCounterSaveData.rot.SetData(m_CashierCounterList[num12].transform.rotation);
				cashierCounterSaveData.objectType = m_CashierCounterList[num12].m_ObjectType;
				cashierCounterSaveData.isBoxed = m_CashierCounterList[num12].GetIsBoxedUp();
				if (cashierCounterSaveData.isBoxed)
				{
					cashierCounterSaveData.boxedPackagePos.SetData(m_CashierCounterList[num12].GetPackagingBoxShelf().transform.position);
					cashierCounterSaveData.boxedPackageRot.SetData(m_CashierCounterList[num12].GetPackagingBoxShelf().transform.rotation);
				}
				if (m_CashierCounterList[num12].GetIsMovingObject())
				{
					cashierCounterSaveData.isBoxed = true;
					cashierCounterSaveData.boxedPackagePos.SetData(m_CashierCounterList[num12].transform.position);
					cashierCounterSaveData.boxedPackageRot.SetData(m_CashierCounterList[num12].transform.rotation);
				}
				cashierCounterSaveData.isDisableCheckout = !m_CashierCounterList[num12].CanCheckout();
				cashierCounterSaveData.isDisableTrading = !m_CashierCounterList[num12].CanTradeCard();
				m_CashCounterSaveDataList.Add(cashierCounterSaveData);
			}
		}
		m_PackageBoxItemSaveDataList.Clear();
		for (int num13 = 0; num13 < RestockManager.GetItemPackagingBoxList().Count; num13++)
		{
			if ((bool)RestockManager.GetItemPackagingBoxList()[num13])
			{
				PackageBoxItemaveData packageBoxItemaveData = new PackageBoxItemaveData();
				ItemTypeAmount itemTypeAmount2 = new ItemTypeAmount();
				itemTypeAmount2.itemType = RestockManager.GetItemPackagingBoxList()[num13].m_ItemCompartment.GetItemType();
				itemTypeAmount2.amount = RestockManager.GetItemPackagingBoxList()[num13].m_ItemCompartment.GetItemCount();
				packageBoxItemaveData.itemTypeAmount = itemTypeAmount2;
				packageBoxItemaveData.isBigBox = RestockManager.GetItemPackagingBoxList()[num13].m_IsBigBox;
				packageBoxItemaveData.isStored = RestockManager.GetItemPackagingBoxList()[num13].m_IsStored;
				packageBoxItemaveData.storedWarehouseShelfIndex = RestockManager.GetItemPackagingBoxList()[num13].GeStoredWarehouseShelfIndex();
				packageBoxItemaveData.storageCompartmentIndex = RestockManager.GetItemPackagingBoxList()[num13].GetStorageCompartmentIndex();
				packageBoxItemaveData.pos.SetData(RestockManager.GetItemPackagingBoxList()[num13].transform.position);
				packageBoxItemaveData.rot.SetData(RestockManager.GetItemPackagingBoxList()[num13].transform.rotation);
				m_PackageBoxItemSaveDataList.Add(packageBoxItemaveData);
			}
		}
		m_PackageBoxCardSaveDataList.Clear();
		for (int num14 = 0; num14 < RestockManager.GetCardPackagingBoxList().Count; num14++)
		{
			if ((bool)RestockManager.GetCardPackagingBoxList()[num14])
			{
				PackageBoxCardSaveData packageBoxCardSaveData = new PackageBoxCardSaveData();
				packageBoxCardSaveData.cardDataList = RestockManager.GetCardPackagingBoxList()[num14].GetCardDataList();
				packageBoxCardSaveData.pos.SetData(RestockManager.GetCardPackagingBoxList()[num14].transform.position);
				packageBoxCardSaveData.rot.SetData(RestockManager.GetCardPackagingBoxList()[num14].transform.rotation);
				m_PackageBoxCardSaveDataList.Add(packageBoxCardSaveData);
			}
		}
		m_DecoObjectSaveDataList.Clear();
		for (int num15 = 0; num15 < m_DecoObjectList.Count; num15++)
		{
			if ((bool)m_DecoObjectList[num15])
			{
				DecoObjectSaveData decoObjectSaveData = new DecoObjectSaveData();
				decoObjectSaveData.pos.SetData(m_DecoObjectList[num15].transform.position);
				decoObjectSaveData.rot.SetData(m_DecoObjectList[num15].transform.rotation);
				decoObjectSaveData.decoObjectType = m_DecoObjectList[num15].m_DecoObjectType;
				decoObjectSaveData.isVerticalSnapToWarehouseWall = m_DecoObjectList[num15].GetIsVerticalSnapToWarehouseWall();
				decoObjectSaveData.verticalSnapWallIndex = m_DecoObjectList[num15].GetVerticalSnapWallIndex();
				decoObjectSaveData.isMovingObject = m_DecoObjectList[num15].GetIsMovingObject();
				m_DecoObjectSaveDataList.Add(decoObjectSaveData);
			}
		}
		CPlayerData.m_ShelfSaveDataList = m_ShelfSaveDataList;
		CPlayerData.m_WarehouseShelfSaveDataList = m_WarehouseShelfSaveDataList;
		CPlayerData.m_PackageBoxItemSaveDataList = m_PackageBoxItemSaveDataList;
		CPlayerData.m_PackageBoxCardSaveDataList = m_PackageBoxCardSaveDataList;
		CPlayerData.m_InteractableObjectSaveDataList = m_InteractableObjectSaveDataList;
		CPlayerData.m_CardShelfSaveDataList = m_CardShelfSaveDataList;
		CPlayerData.m_PlayTableSaveDataList = m_PlayTableSaveDataList;
		CPlayerData.m_AutoCleanserSaveDataList = m_AutoCleanserSaveDataList;
		CPlayerData.m_AutoPackOpenerSaveDataList = m_AutoPackOpenerSaveDataList;
		CPlayerData.m_EmptyBoxStorageSaveDataList = m_EmptyBoxStorageSaveDataList;
		CPlayerData.m_BulkDonationSaveDataList = m_BulkDonationSaveDataList;
		CPlayerData.m_CardStorageShelfSaveDataList = m_CardStorageShelfSaveDataList;
		CPlayerData.m_WorkbenchSaveDataList = m_WorkbenchSaveDataList;
		CPlayerData.m_CashCounterSaveDataList = m_CashCounterSaveDataList;
		CPlayerData.m_DecoObjectSaveDataList = m_DecoObjectSaveDataList;
	}

	private IEnumerator CheckLoadCorrect()
	{
		if (Time.timeScale != 0f)
		{
			yield return new WaitForSeconds(7f);
		}
		else
		{
			yield return new WaitForSecondsRealtime(30f);
		}
		if (!m_FinishLoadingObjectData)
		{
			Debug.Log("Shelf data not loaded properly");
			CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfDataNotLoadedCorrectly);
			GameInstance.m_HasLoadingError = true;
			Time.timeScale = 1f;
			CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Title");
			CSingleton<LoadingScreen>.Instance.gameObject.SetActive(value: false);
		}
	}

	public void LoadInteractableObjectData()
	{
		StartCoroutine(CheckLoadCorrect());
		m_FinishLoadingObjectData = false;
		RestockManager.DestroyAllObject();
		DestroyAllObject();
		m_ShelfSaveDataList = CPlayerData.m_ShelfSaveDataList;
		m_WarehouseShelfSaveDataList = CPlayerData.m_WarehouseShelfSaveDataList;
		m_PackageBoxItemSaveDataList = CPlayerData.m_PackageBoxItemSaveDataList;
		m_PackageBoxCardSaveDataList = CPlayerData.m_PackageBoxCardSaveDataList;
		m_InteractableObjectSaveDataList = CPlayerData.m_InteractableObjectSaveDataList;
		m_CardShelfSaveDataList = CPlayerData.m_CardShelfSaveDataList;
		m_PlayTableSaveDataList = CPlayerData.m_PlayTableSaveDataList;
		m_AutoCleanserSaveDataList = CPlayerData.m_AutoCleanserSaveDataList;
		m_AutoPackOpenerSaveDataList = CPlayerData.m_AutoPackOpenerSaveDataList;
		m_BulkDonationSaveDataList = CPlayerData.m_BulkDonationSaveDataList;
		m_EmptyBoxStorageSaveDataList = CPlayerData.m_EmptyBoxStorageSaveDataList;
		m_CardStorageShelfSaveDataList = CPlayerData.m_CardStorageShelfSaveDataList;
		m_WorkbenchSaveDataList = CPlayerData.m_WorkbenchSaveDataList;
		m_CashCounterSaveDataList = CPlayerData.m_CashCounterSaveDataList;
		m_DecoObjectSaveDataList = CPlayerData.m_DecoObjectSaveDataList;
		m_SpawnedShelfList.Clear();
		for (int i = 0; i < m_ShelfSaveDataList.Count; i++)
		{
			Shelf component = SpawnInteractableObject(m_ShelfSaveDataList[i].objectType).GetComponent<Shelf>();
			component.transform.position = m_ShelfSaveDataList[i].pos.Data;
			component.transform.rotation = m_ShelfSaveDataList[i].rot.Data;
			m_SpawnedShelfList.Add(component);
		}
		m_SpawnedWarehouseShelfList.Clear();
		for (int j = 0; j < m_WarehouseShelfSaveDataList.Count; j++)
		{
			WarehouseShelf component2 = SpawnInteractableObject(m_WarehouseShelfSaveDataList[j].objectType).GetComponent<WarehouseShelf>();
			component2.transform.position = m_WarehouseShelfSaveDataList[j].pos.Data;
			component2.transform.rotation = m_WarehouseShelfSaveDataList[j].rot.Data;
			m_SpawnedWarehouseShelfList.Add(component2);
		}
		m_SpawnedCardShelfList.Clear();
		for (int k = 0; k < m_CardShelfSaveDataList.Count; k++)
		{
			CardShelf component3 = SpawnInteractableObject(m_CardShelfSaveDataList[k].objectType).GetComponent<CardShelf>();
			component3.transform.position = m_CardShelfSaveDataList[k].pos.Data;
			component3.transform.rotation = m_CardShelfSaveDataList[k].rot.Data;
			component3.SetVerticalSnapToWarehouseWall(m_CardShelfSaveDataList[k].isVerticalSnapToWarehouseWall, m_CardShelfSaveDataList[k].verticalSnapWallIndex);
			m_SpawnedCardShelfList.Add(component3);
		}
		m_SpawnedPlayTableList.Clear();
		for (int l = 0; l < m_PlayTableSaveDataList.Count; l++)
		{
			InteractablePlayTable component4 = SpawnInteractableObject(m_PlayTableSaveDataList[l].objectType).GetComponent<InteractablePlayTable>();
			component4.transform.position = m_PlayTableSaveDataList[l].pos.Data;
			component4.transform.rotation = m_PlayTableSaveDataList[l].rot.Data;
			m_SpawnedPlayTableList.Add(component4);
		}
		m_SpawnedAutoCleanserList.Clear();
		for (int m = 0; m < m_AutoCleanserSaveDataList.Count; m++)
		{
			InteractableAutoCleanser component5 = SpawnInteractableObject(m_AutoCleanserSaveDataList[m].objectType).GetComponent<InteractableAutoCleanser>();
			component5.transform.position = m_AutoCleanserSaveDataList[m].pos.Data;
			component5.transform.rotation = m_AutoCleanserSaveDataList[m].rot.Data;
			m_SpawnedAutoCleanserList.Add(component5);
		}
		m_SpawnedAutoPackOpenerList.Clear();
		for (int n = 0; n < m_AutoPackOpenerSaveDataList.Count; n++)
		{
			InteractableAutoPackOpener component6 = SpawnInteractableObject(m_AutoPackOpenerSaveDataList[n].objectType).GetComponent<InteractableAutoPackOpener>();
			component6.transform.position = m_AutoPackOpenerSaveDataList[n].pos.Data;
			component6.transform.rotation = m_AutoPackOpenerSaveDataList[n].rot.Data;
			m_SpawnedAutoPackOpenerList.Add(component6);
		}
		m_SpawnedEmptyBoxStorageList.Clear();
		for (int num = 0; num < m_EmptyBoxStorageSaveDataList.Count; num++)
		{
			InteractableEmptyBoxStorage component7 = SpawnInteractableObject(m_EmptyBoxStorageSaveDataList[num].objectType).GetComponent<InteractableEmptyBoxStorage>();
			component7.transform.position = m_EmptyBoxStorageSaveDataList[num].pos.Data;
			component7.transform.rotation = m_EmptyBoxStorageSaveDataList[num].rot.Data;
			m_SpawnedEmptyBoxStorageList.Add(component7);
		}
		m_SpawnedBulkDonationBoxList.Clear();
		for (int num2 = 0; num2 < m_BulkDonationSaveDataList.Count; num2++)
		{
			InteractableBulkDonationBox component8 = SpawnInteractableObject(m_BulkDonationSaveDataList[num2].objectType).GetComponent<InteractableBulkDonationBox>();
			component8.transform.position = m_BulkDonationSaveDataList[num2].pos.Data;
			component8.transform.rotation = m_BulkDonationSaveDataList[num2].rot.Data;
			m_SpawnedBulkDonationBoxList.Add(component8);
		}
		m_SpawnedCardStorageShelfList.Clear();
		for (int num3 = 0; num3 < m_CardStorageShelfSaveDataList.Count; num3++)
		{
			InteractableCardStorageShelf component9 = SpawnInteractableObject(m_CardStorageShelfSaveDataList[num3].objectType).GetComponent<InteractableCardStorageShelf>();
			component9.transform.position = m_CardStorageShelfSaveDataList[num3].pos.Data;
			component9.transform.rotation = m_CardStorageShelfSaveDataList[num3].rot.Data;
			m_SpawnedCardStorageShelfList.Add(component9);
		}
		m_SpawnedWorkbenchList.Clear();
		for (int num4 = 0; num4 < m_WorkbenchSaveDataList.Count; num4++)
		{
			InteractableWorkbench component10 = SpawnInteractableObject(m_WorkbenchSaveDataList[num4].objectType).GetComponent<InteractableWorkbench>();
			component10.transform.position = m_WorkbenchSaveDataList[num4].pos.Data;
			component10.transform.rotation = m_WorkbenchSaveDataList[num4].rot.Data;
			m_SpawnedWorkbenchList.Add(component10);
		}
		m_SpawnedCashierCounterList.Clear();
		for (int num5 = 0; num5 < m_CashCounterSaveDataList.Count; num5++)
		{
			InteractableCashierCounter component11 = SpawnInteractableObject(m_CashCounterSaveDataList[num5].objectType).GetComponent<InteractableCashierCounter>();
			component11.transform.position = m_CashCounterSaveDataList[num5].pos.Data;
			component11.transform.rotation = m_CashCounterSaveDataList[num5].rot.Data;
			m_SpawnedCashierCounterList.Add(component11);
		}
		m_SpawnedInteractableObjectList.Clear();
		for (int num6 = 0; num6 < m_InteractableObjectSaveDataList.Count; num6++)
		{
			InteractableObject interactableObject = SpawnInteractableObject(m_InteractableObjectSaveDataList[num6].objectType);
			interactableObject.transform.position = m_InteractableObjectSaveDataList[num6].pos.Data;
			interactableObject.transform.rotation = m_InteractableObjectSaveDataList[num6].rot.Data;
			m_SpawnedInteractableObjectList.Add(interactableObject);
		}
		m_SpawnedDecoObjectList.Clear();
		for (int num7 = 0; num7 < m_DecoObjectSaveDataList.Count; num7++)
		{
			if (m_DecoObjectSaveDataList[num7].isMovingObject)
			{
				CPlayerData.AddDecoItemToInventory(m_DecoObjectSaveDataList[num7].decoObjectType, 1);
				continue;
			}
			InteractableObject interactableObject2 = SpawnDecoObject(m_DecoObjectSaveDataList[num7].decoObjectType);
			interactableObject2.transform.position = m_DecoObjectSaveDataList[num7].pos.Data;
			interactableObject2.transform.rotation = m_DecoObjectSaveDataList[num7].rot.Data;
			interactableObject2.SetVerticalSnapToWarehouseWall(m_DecoObjectSaveDataList[num7].isVerticalSnapToWarehouseWall, m_DecoObjectSaveDataList[num7].verticalSnapWallIndex);
			m_SpawnedDecoObjectList.Add(interactableObject2);
		}
		m_SpawnedPackageBoxItemList.Clear();
		for (int num8 = 0; num8 < m_PackageBoxItemSaveDataList.Count; num8++)
		{
			EItemType itemType = m_PackageBoxItemSaveDataList[num8].itemTypeAmount.itemType;
			int amount = m_PackageBoxItemSaveDataList[num8].itemTypeAmount.amount;
			InteractablePackagingBox_Item interactablePackagingBox_Item = RestockManager.SpawnPackageBoxItem(itemType, amount, m_PackageBoxItemSaveDataList[num8].isBigBox);
			interactablePackagingBox_Item.transform.position = m_PackageBoxItemSaveDataList[num8].pos.Data;
			interactablePackagingBox_Item.transform.rotation = m_PackageBoxItemSaveDataList[num8].rot.Data;
			m_SpawnedPackageBoxItemList.Add(interactablePackagingBox_Item);
		}
		m_SpawnedPackageBoxCardList.Clear();
		for (int num9 = 0; num9 < m_PackageBoxCardSaveDataList.Count; num9++)
		{
			InteractablePackagingBox_Card interactablePackagingBox_Card = RestockManager.SpawnPackageBoxCard(m_PackageBoxCardSaveDataList[num9].cardDataList, RestockManager.GetRandomPackageSpawnPos());
			interactablePackagingBox_Card.transform.position = m_PackageBoxCardSaveDataList[num9].pos.Data;
			interactablePackagingBox_Card.transform.rotation = m_PackageBoxCardSaveDataList[num9].rot.Data;
			m_SpawnedPackageBoxCardList.Add(interactablePackagingBox_Card);
		}
		StartCoroutine(DelayLoad());
	}

	private IEnumerator DelayLoad()
	{
		yield return new WaitForSeconds(0.05f);
		for (int i = 0; i < m_SpawnedShelfList.Count && i < m_ShelfSaveDataList.Count; i++)
		{
			m_SpawnedShelfList[i].LoadItemCompartment(m_ShelfSaveDataList[i].itemTypeAmountList);
		}
		for (int j = 0; j < m_SpawnedCardShelfList.Count && j < m_CardShelfSaveDataList.Count; j++)
		{
			m_SpawnedCardShelfList[j].LoadCardCompartment(m_CardShelfSaveDataList[j].cardDataList);
		}
		for (int k = 0; k < m_SpawnedPackageBoxItemList.Count && k < m_PackageBoxItemSaveDataList.Count; k++)
		{
			if (m_PackageBoxItemSaveDataList[k].isStored)
			{
				WarehouseShelf warehouseShelf = null;
				ShelfCompartment shelfCompartment = null;
				if (m_PackageBoxItemSaveDataList[k].storedWarehouseShelfIndex < m_SpawnedWarehouseShelfList.Count)
				{
					warehouseShelf = m_SpawnedWarehouseShelfList[m_PackageBoxItemSaveDataList[k].storedWarehouseShelfIndex];
				}
				if ((bool)warehouseShelf)
				{
					shelfCompartment = warehouseShelf.GetWarehouseCompartment(m_PackageBoxItemSaveDataList[k].storageCompartmentIndex);
				}
				if ((bool)shelfCompartment)
				{
					m_SpawnedPackageBoxItemList[k].SetPhysicsEnabled(isEnable: false);
					m_SpawnedPackageBoxItemList[k].DispenseItem(isPlayer: false, shelfCompartment);
				}
			}
		}
		yield return new WaitForSeconds(0.5f);
		for (int l = 0; l < m_SpawnedShelfList.Count && l < m_ShelfSaveDataList.Count; l++)
		{
			if (m_ShelfSaveDataList[l].isBoxed)
			{
				m_SpawnedShelfList[l].BoxUpObject(holdBox: false);
				m_SpawnedShelfList[l].GetPackagingBoxShelf().transform.position = m_ShelfSaveDataList[l].boxedPackagePos.Data;
				m_SpawnedShelfList[l].GetPackagingBoxShelf().transform.rotation = m_ShelfSaveDataList[l].boxedPackageRot.Data;
			}
		}
		for (int m = 0; m < m_SpawnedWarehouseShelfList.Count && m < m_WarehouseShelfSaveDataList.Count; m++)
		{
			for (int n = 0; n < m_SpawnedWarehouseShelfList[m].GetStorageCompartmentList().Count; n++)
			{
				if (m_WarehouseShelfSaveDataList[m].compartmentItemType != null)
				{
					if (n >= m_WarehouseShelfSaveDataList[m].compartmentItemType.Count)
					{
						break;
					}
					m_SpawnedWarehouseShelfList[m].GetStorageCompartmentList()[n].GetShelfCompartment().SetCompartmentItemType(m_WarehouseShelfSaveDataList[m].compartmentItemType[n]);
					m_SpawnedWarehouseShelfList[m].GetStorageCompartmentList()[n].GetShelfCompartment().SetPriceTagItemAmountText();
					m_SpawnedWarehouseShelfList[m].GetStorageCompartmentList()[n].GetShelfCompartment().SetPriceTagItemImage(m_WarehouseShelfSaveDataList[m].compartmentItemType[n]);
				}
			}
			if (m_WarehouseShelfSaveDataList[m].isBoxed)
			{
				m_SpawnedWarehouseShelfList[m].BoxUpObject(holdBox: false);
				m_SpawnedWarehouseShelfList[m].GetPackagingBoxShelf().transform.position = m_WarehouseShelfSaveDataList[m].boxedPackagePos.Data;
				m_SpawnedWarehouseShelfList[m].GetPackagingBoxShelf().transform.rotation = m_WarehouseShelfSaveDataList[m].boxedPackageRot.Data;
			}
		}
		for (int num = 0; num < m_SpawnedCardShelfList.Count && num < m_CardShelfSaveDataList.Count; num++)
		{
			if (m_CardShelfSaveDataList[num].isBoxed)
			{
				m_SpawnedCardShelfList[num].BoxUpObject(holdBox: false);
				m_SpawnedCardShelfList[num].GetPackagingBoxShelf().transform.position = m_CardShelfSaveDataList[num].boxedPackagePos.Data;
				m_SpawnedCardShelfList[num].GetPackagingBoxShelf().transform.rotation = m_CardShelfSaveDataList[num].boxedPackageRot.Data;
			}
		}
		for (int num2 = 0; num2 < m_SpawnedPlayTableList.Count && num2 < m_PlayTableSaveDataList.Count; num2++)
		{
			if (m_PlayTableSaveDataList[num2].isBoxed)
			{
				m_SpawnedPlayTableList[num2].BoxUpObject(holdBox: false);
				m_SpawnedPlayTableList[num2].GetPackagingBoxShelf().transform.position = m_PlayTableSaveDataList[num2].boxedPackagePos.Data;
				m_SpawnedPlayTableList[num2].GetPackagingBoxShelf().transform.rotation = m_PlayTableSaveDataList[num2].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedPlayTableList[num2].LoadData(m_PlayTableSaveDataList[num2]);
			}
		}
		for (int num3 = 0; num3 < m_SpawnedAutoCleanserList.Count && num3 < m_AutoCleanserSaveDataList.Count; num3++)
		{
			if (m_AutoCleanserSaveDataList[num3].isBoxed)
			{
				m_SpawnedAutoCleanserList[num3].LoadData(m_AutoCleanserSaveDataList[num3]);
				m_SpawnedAutoCleanserList[num3].BoxUpObject(holdBox: false);
				m_SpawnedAutoCleanserList[num3].GetPackagingBoxShelf().transform.position = m_AutoCleanserSaveDataList[num3].boxedPackagePos.Data;
				m_SpawnedAutoCleanserList[num3].GetPackagingBoxShelf().transform.rotation = m_AutoCleanserSaveDataList[num3].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedAutoCleanserList[num3].LoadData(m_AutoCleanserSaveDataList[num3]);
			}
		}
		for (int num4 = 0; num4 < m_SpawnedAutoPackOpenerList.Count && num4 < m_AutoPackOpenerSaveDataList.Count; num4++)
		{
			if (m_AutoPackOpenerSaveDataList[num4].isBoxed)
			{
				m_SpawnedAutoPackOpenerList[num4].LoadData(m_AutoPackOpenerSaveDataList[num4]);
				m_SpawnedAutoPackOpenerList[num4].BoxUpObject(holdBox: false);
				m_SpawnedAutoPackOpenerList[num4].GetPackagingBoxShelf().transform.position = m_AutoPackOpenerSaveDataList[num4].boxedPackagePos.Data;
				m_SpawnedAutoPackOpenerList[num4].GetPackagingBoxShelf().transform.rotation = m_AutoPackOpenerSaveDataList[num4].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedAutoPackOpenerList[num4].LoadData(m_AutoPackOpenerSaveDataList[num4]);
			}
		}
		for (int num5 = 0; num5 < m_SpawnedEmptyBoxStorageList.Count && num5 < m_EmptyBoxStorageSaveDataList.Count; num5++)
		{
			if (m_EmptyBoxStorageSaveDataList[num5].isBoxed)
			{
				m_SpawnedEmptyBoxStorageList[num5].LoadData(m_EmptyBoxStorageSaveDataList[num5]);
				m_SpawnedEmptyBoxStorageList[num5].BoxUpObject(holdBox: false);
				m_SpawnedEmptyBoxStorageList[num5].GetPackagingBoxShelf().transform.position = m_EmptyBoxStorageSaveDataList[num5].boxedPackagePos.Data;
				m_SpawnedEmptyBoxStorageList[num5].GetPackagingBoxShelf().transform.rotation = m_EmptyBoxStorageSaveDataList[num5].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedEmptyBoxStorageList[num5].LoadData(m_EmptyBoxStorageSaveDataList[num5]);
			}
		}
		for (int num6 = 0; num6 < m_SpawnedBulkDonationBoxList.Count && num6 < m_BulkDonationSaveDataList.Count; num6++)
		{
			if (m_BulkDonationSaveDataList[num6].isBoxed)
			{
				m_SpawnedBulkDonationBoxList[num6].LoadData(m_BulkDonationSaveDataList[num6]);
				m_SpawnedBulkDonationBoxList[num6].BoxUpObject(holdBox: false);
				m_SpawnedBulkDonationBoxList[num6].GetPackagingBoxShelf().transform.position = m_BulkDonationSaveDataList[num6].boxedPackagePos.Data;
				m_SpawnedBulkDonationBoxList[num6].GetPackagingBoxShelf().transform.rotation = m_BulkDonationSaveDataList[num6].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedBulkDonationBoxList[num6].LoadData(m_BulkDonationSaveDataList[num6]);
			}
		}
		for (int num7 = 0; num7 < m_SpawnedCardStorageShelfList.Count && num7 < m_CardStorageShelfSaveDataList.Count; num7++)
		{
			if (m_CardStorageShelfSaveDataList[num7].isBoxed)
			{
				m_SpawnedCardStorageShelfList[num7].LoadData(m_CardStorageShelfSaveDataList[num7]);
				m_SpawnedCardStorageShelfList[num7].BoxUpObject(holdBox: false);
				m_SpawnedCardStorageShelfList[num7].GetPackagingBoxShelf().transform.position = m_CardStorageShelfSaveDataList[num7].boxedPackagePos.Data;
				m_SpawnedCardStorageShelfList[num7].GetPackagingBoxShelf().transform.rotation = m_CardStorageShelfSaveDataList[num7].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedCardStorageShelfList[num7].LoadData(m_CardStorageShelfSaveDataList[num7]);
			}
		}
		for (int num8 = 0; num8 < m_SpawnedWorkbenchList.Count && num8 < m_WorkbenchSaveDataList.Count; num8++)
		{
			if (m_WorkbenchSaveDataList[num8].isBoxed)
			{
				m_SpawnedWorkbenchList[num8].LoadData(m_WorkbenchSaveDataList[num8]);
				m_SpawnedWorkbenchList[num8].BoxUpObject(holdBox: false);
				m_SpawnedWorkbenchList[num8].GetPackagingBoxShelf().transform.position = m_WorkbenchSaveDataList[num8].boxedPackagePos.Data;
				m_SpawnedWorkbenchList[num8].GetPackagingBoxShelf().transform.rotation = m_WorkbenchSaveDataList[num8].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedWorkbenchList[num8].LoadData(m_WorkbenchSaveDataList[num8]);
			}
		}
		for (int num9 = 0; num9 < m_SpawnedCashierCounterList.Count && num9 < m_CashCounterSaveDataList.Count; num9++)
		{
			if (m_CashCounterSaveDataList[num9].isBoxed)
			{
				m_SpawnedCashierCounterList[num9].LoadData(m_CashCounterSaveDataList[num9]);
				m_SpawnedCashierCounterList[num9].BoxUpObject(holdBox: false);
				m_SpawnedCashierCounterList[num9].GetPackagingBoxShelf().transform.position = m_CashCounterSaveDataList[num9].boxedPackagePos.Data;
				m_SpawnedCashierCounterList[num9].GetPackagingBoxShelf().transform.rotation = m_CashCounterSaveDataList[num9].boxedPackageRot.Data;
			}
			else
			{
				m_SpawnedCashierCounterList[num9].LoadData(m_CashCounterSaveDataList[num9]);
			}
		}
		for (int num10 = 0; num10 < m_SpawnedInteractableObjectList.Count && num10 < m_InteractableObjectSaveDataList.Count; num10++)
		{
			if (m_InteractableObjectSaveDataList[num10].isBoxed)
			{
				m_SpawnedInteractableObjectList[num10].BoxUpObject(holdBox: false);
				m_SpawnedInteractableObjectList[num10].GetPackagingBoxShelf().transform.position = m_InteractableObjectSaveDataList[num10].boxedPackagePos.Data;
				m_SpawnedInteractableObjectList[num10].GetPackagingBoxShelf().transform.rotation = m_InteractableObjectSaveDataList[num10].boxedPackageRot.Data;
			}
		}
		if (m_CashierCounterList.Count == 0 && m_SpawnedCashierCounterList.Count == 0)
		{
			Transform randomPackageSpawnPos = RestockManager.GetRandomPackageSpawnPos();
			SpawnInteractableObjectInPackageBox(EObjectType.CashCounter, randomPackageSpawnPos.position, randomPackageSpawnPos.rotation);
		}
		m_FinishLoadingObjectData = true;
		CEventManager.QueueEvent(new CEventPlayer_GameDataFinishLoaded());
		CSingleton<LoadingScreen>.Instance.FinishLoading();
		GameInstance.m_FinishedSavefileLoading = true;
		TempGemMintCardCountAddBack();
	}

	private void TempGemMintCardCountAddBack()
	{
		if (CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained != 0 || CPlayerData.m_GradedCardInventoryList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_CardShelfList.Count; i++)
		{
			for (int j = 0; j < m_CardShelfList[i].GetCardCompartmentList().Count; j++)
			{
				if (m_CardShelfList[i].GetCardCompartmentList()[j].m_StoredCardList.Count > 0 && m_CardShelfList[i].GetCardCompartmentList()[j].m_StoredCardList[0].m_Card3dUI.m_CardUI.GetCardData().cardGrade >= 10)
				{
					CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained++;
				}
			}
		}
		for (int k = 0; k < CPlayerData.m_GradedCardInventoryList.Count; k++)
		{
			if (CPlayerData.m_GradedCardInventoryList[k].amount >= 10)
			{
				CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained++;
			}
		}
	}

	public static InteractableObject SpawnInteractableObject(EObjectType objType)
	{
		InteractableObject interactableObject = Object.Instantiate(InventoryBase.GetSpawnInteractableObjectPrefab(objType), Vector3.zero, Quaternion.identity, CSingleton<ShelfManager>.Instance.m_ShelfParentGrp);
		interactableObject.name = objType.ToString() + CSingleton<ShelfManager>.Instance.m_SpawnedObjectCount;
		CSingleton<ShelfManager>.Instance.m_SpawnedObjectCount = CSingleton<ShelfManager>.Instance.m_SpawnedObjectCount + 1;
		interactableObject.gameObject.SetActive(value: true);
		return interactableObject;
	}

	public static InteractableObject SpawnDecoObject(EDecoObject objType)
	{
		InteractableObject interactableObject = Object.Instantiate(InventoryBase.GetSpawnDecoObjectPrefab(objType), Vector3.zero, Quaternion.identity, CSingleton<ShelfManager>.Instance.m_ShelfParentGrp);
		interactableObject.name = objType.ToString() + CSingleton<ShelfManager>.Instance.m_SpawnedObjectCount;
		CSingleton<ShelfManager>.Instance.m_SpawnedObjectCount = CSingleton<ShelfManager>.Instance.m_SpawnedObjectCount + 1;
		interactableObject.gameObject.SetActive(value: true);
		return interactableObject;
	}

	public static void SpawnInteractableObjectInPackageBox(EObjectType objType, Vector3 spawnPos, Quaternion spawnRot)
	{
		InteractableObject interactableObject = SpawnInteractableObject(objType);
		interactableObject.Init();
		interactableObject.BoxUpObject(holdBox: false);
		interactableObject.GetPackagingBoxShelf().transform.position = spawnPos;
		interactableObject.GetPackagingBoxShelf().transform.rotation = spawnRot;
	}

	private IEnumerator DelayBoxUp(InteractableObject obj, Vector3 spawnPos, Quaternion spawnRot)
	{
		yield return new WaitForSeconds(0.02f);
		obj.BoxUpObject(holdBox: false);
		obj.GetPackagingBoxShelf().transform.position = spawnPos;
		obj.GetPackagingBoxShelf().transform.rotation = spawnRot;
	}

	public static void SpawnDecoObjectOnHand(EDecoObject objType)
	{
		InteractableObject interactableObject = SpawnDecoObject(objType);
		interactableObject.transform.position = CSingleton<InteractionPlayerController>.Instance.m_HoldItemPos.position;
		interactableObject.Init();
		if (interactableObject.m_IsDecorationVertical)
		{
			Quaternion rotation = CSingleton<InteractionPlayerController>.Instance.m_HoldItemPos.rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.y += 180f;
			eulerAngles.z = 0f;
			rotation.eulerAngles = eulerAngles;
			interactableObject.transform.rotation = rotation;
		}
		else
		{
			Quaternion identity = Quaternion.identity;
			Vector3 eulerAngles2 = identity.eulerAngles;
			eulerAngles2.x = 0f;
			eulerAngles2.y += 180f;
			eulerAngles2.z = 0f;
			identity.eulerAngles = eulerAngles2;
			interactableObject.transform.rotation = identity;
		}
		CSingleton<InteractionPlayerController>.Instance.ForceStartMoveObject(interactableObject);
	}

	private void Awake()
	{
		m_MoveObjectPreviewModelMat = m_MoveObjectPreviewRenderer.material;
		Material[] materials = new Material[5] { m_MoveObjectPreviewModelMat, m_MoveObjectPreviewModelMat, m_MoveObjectPreviewModelMat, m_MoveObjectPreviewModelMat, m_MoveObjectPreviewModelMat };
		m_MoveObjectPreviewRenderer.materials = materials;
	}

	private void Update()
	{
		Vector3 forward = CSingleton<InteractionPlayerController>.Instance.m_Cam.transform.forward;
		Vector3 position = CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.transform.position;
		for (int i = 0; i < m_ShelfList.Count; i++)
		{
			Shelf shelf = m_ShelfList[i];
			if ((bool)shelf)
			{
				bool num = (shelf.transform.position - position).sqrMagnitude > 36f;
				float f = Vector3.Dot(forward, shelf.transform.forward);
				bool flag = !num || !(Mathf.Abs(f) > 0.7f);
				if (shelf.m_IsHalfVisible != flag)
				{
					shelf.SetHalfVisibility(flag);
				}
			}
		}
	}

	public static void ActivateMoveObjectPreviewMode(Transform targetParent, MeshFilter targetMesh, Transform moveStateValidArea)
	{
		CSingleton<ShelfManager>.Instance.m_MoveObjectTargetMesh = targetMesh;
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.transform.position = targetMesh.transform.position;
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.transform.rotation = targetMesh.transform.rotation;
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.transform.localScale = targetMesh.transform.lossyScale + Vector3.one * 0.001f;
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.transform.parent = targetParent;
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.mesh = targetMesh.mesh;
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.gameObject.SetActive(value: true);
		SetAllDashedLineVisibility(isVisible: true);
	}

	public static void DisableMoveObjectPreviewMode()
	{
		CSingleton<ShelfManager>.Instance.m_MoveObjectTargetMesh = null;
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.gameObject.SetActive(value: false);
		CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModel.transform.parent = CSingleton<ShelfManager>.Instance.transform;
		SetAllDashedLineVisibility(isVisible: false);
	}

	public static void SetMoveObjectPreviewModelValidState(bool isValid)
	{
		if (isValid)
		{
			CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModelMat.SetColor("_Color", CSingleton<ShelfManager>.Instance.m_PreviewMeshValidColor);
		}
		else
		{
			CSingleton<ShelfManager>.Instance.m_MoveObjectPreviewModelMat.SetColor("_Color", CSingleton<ShelfManager>.Instance.m_PreviewMeshInvalidColor);
		}
	}

	public static void AddDashedLine(DashedLine dashline)
	{
		CSingleton<ShelfManager>.Instance.m_DashedLineList.Add(dashline);
	}

	public static void SetAllDashedLineVisibility(bool isVisible)
	{
		for (int num = CSingleton<ShelfManager>.Instance.m_DashedLineList.Count - 1; num >= 0; num--)
		{
			if ((bool)CSingleton<ShelfManager>.Instance.m_DashedLineList[num])
			{
				CSingleton<ShelfManager>.Instance.m_DashedLineList[num].gameObject.SetActive(isVisible);
			}
			else
			{
				CSingleton<ShelfManager>.Instance.m_DashedLineList.RemoveAt(num);
			}
		}
	}

	public static void InitInteractableObject(InteractableObject obj)
	{
		CSingleton<ShelfManager>.Instance.m_InteractableObjectList.Add(obj);
	}

	public static List<InteractableObject> GetInteractableObjectList()
	{
		return CSingleton<ShelfManager>.Instance.m_InteractableObjectList;
	}

	public static void RemoveInteractableObject(InteractableObject obj)
	{
		CSingleton<ShelfManager>.Instance.m_InteractableObjectList.Remove(obj);
	}

	public static void InitDecoObject(InteractableObject obj)
	{
		CSingleton<ShelfManager>.Instance.m_DecoObjectList.Add(obj);
	}

	public static List<InteractableObject> GetDecoObjectList()
	{
		return CSingleton<ShelfManager>.Instance.m_DecoObjectList;
	}

	public static void RemoveDecoObject(InteractableObject obj)
	{
		CSingleton<ShelfManager>.Instance.m_DecoObjectList.Remove(obj);
	}

	public static void InitShelf(Shelf shelf)
	{
		CSingleton<ShelfManager>.Instance.m_ShelfList.Add(shelf);
	}

	public static List<Shelf> GetShelfList()
	{
		return CSingleton<ShelfManager>.Instance.m_ShelfList;
	}

	public static void RemoveShelf(Shelf shelf)
	{
		CSingleton<ShelfManager>.Instance.m_ShelfList.Remove(shelf);
	}

	public static List<CardShelf> GetCardShelfList()
	{
		return CSingleton<ShelfManager>.Instance.m_CardShelfList;
	}

	public static void InitWarehouseShelf(WarehouseShelf shelf)
	{
		CSingleton<ShelfManager>.Instance.m_WarehouseShelfList.Add(shelf);
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_WarehouseShelfList.Count; i++)
		{
			CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i].SetIndex(i);
		}
	}

	public static void RemoveWarehouseShelf(WarehouseShelf shelf)
	{
		CSingleton<ShelfManager>.Instance.m_WarehouseShelfList.Remove(shelf);
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_WarehouseShelfList.Count; i++)
		{
			CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i].SetIndex(i);
			for (int j = 0; j < CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i].GetStorageCompartmentList().Count; j++)
			{
				for (int k = 0; k < CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i].GetStorageCompartmentList()[j].GetShelfCompartment().GetInteractablePackagingBoxList().Count; k++)
				{
					CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i].GetStorageCompartmentList()[j].GetShelfCompartment().GetInteractablePackagingBoxList()[k].UpdateStoredWarehouseShelfIndex();
				}
			}
		}
	}

	public static void InitCardShelf(CardShelf shelf)
	{
		CSingleton<ShelfManager>.Instance.m_CardShelfList.Add(shelf);
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_CardShelfList.Count; i++)
		{
			CSingleton<ShelfManager>.Instance.m_CardShelfList[i].SetIndex(i);
		}
	}

	public static void RemoveCardShelf(CardShelf shelf)
	{
		CSingleton<ShelfManager>.Instance.m_CardShelfList.Remove(shelf);
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_CardShelfList.Count; i++)
		{
			CSingleton<ShelfManager>.Instance.m_CardShelfList[i].SetIndex(i);
		}
	}

	public static void InitPlayTable(InteractablePlayTable shelf)
	{
		CSingleton<ShelfManager>.Instance.m_PlayTableList.Add(shelf);
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_PlayTableList.Count; i++)
		{
			CSingleton<ShelfManager>.Instance.m_PlayTableList[i].SetIndex(i);
		}
	}

	public static void RemovePlayTable(InteractablePlayTable shelf)
	{
		CSingleton<ShelfManager>.Instance.m_PlayTableList.Remove(shelf);
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_PlayTableList.Count; i++)
		{
			CSingleton<ShelfManager>.Instance.m_PlayTableList[i].SetIndex(i);
		}
	}

	public static void InitAutoCleanser(InteractableAutoCleanser autoCleanser)
	{
		CSingleton<ShelfManager>.Instance.m_AutoCleanserList.Add(autoCleanser);
	}

	public static void InitAutoPackOpener(InteractableAutoPackOpener autoPackOpener)
	{
		CSingleton<ShelfManager>.Instance.m_AutoPackOpenerList.Add(autoPackOpener);
	}

	public static void InitWorkbench(InteractableWorkbench workbench)
	{
		CSingleton<ShelfManager>.Instance.m_WorkbenchList.Add(workbench);
	}

	public static void InitBulkDonationBox(InteractableBulkDonationBox bulkDonationBox)
	{
		CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList.Add(bulkDonationBox);
	}

	public static void RemoveBulkDonationBox(InteractableBulkDonationBox bulkDonationBox)
	{
		CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList.Remove(bulkDonationBox);
	}

	public static List<InteractableBulkDonationBox> GetBulkDonationBoxList()
	{
		return CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList;
	}

	public static void InitCardStorageShelf(InteractableCardStorageShelf cardStorageShelf)
	{
		CSingleton<ShelfManager>.Instance.m_CardStorageShelfList.Add(cardStorageShelf);
	}

	public static void RemoveCardStorageShelf(InteractableCardStorageShelf cardStorageShelf)
	{
		CSingleton<ShelfManager>.Instance.m_CardStorageShelfList.Remove(cardStorageShelf);
	}

	public static List<InteractableCardStorageShelf> GetCardStorageShelfList()
	{
		return CSingleton<ShelfManager>.Instance.m_CardStorageShelfList;
	}

	public static List<InteractableAutoCleanser> GetAutoCleanserList()
	{
		return CSingleton<ShelfManager>.Instance.m_AutoCleanserList;
	}

	public static void RemoveAutoCleanser(InteractableAutoCleanser autoCleanser)
	{
		CSingleton<ShelfManager>.Instance.m_AutoCleanserList.Remove(autoCleanser);
	}

	public static void RemoveAutoPackOpener(InteractableAutoPackOpener autoPackOpener)
	{
		CSingleton<ShelfManager>.Instance.m_AutoPackOpenerList.Remove(autoPackOpener);
	}

	public static List<InteractableAutoPackOpener> GetAutoPackOpenerList()
	{
		return CSingleton<ShelfManager>.Instance.m_AutoPackOpenerList;
	}

	public static void InitTrashBin(InteractableTrashBin trashBin)
	{
		CSingleton<ShelfManager>.Instance.m_TrashBinList.Add(trashBin);
	}

	public static List<InteractableTrashBin> GetTrashBinList()
	{
		return CSingleton<ShelfManager>.Instance.m_TrashBinList;
	}

	public static InteractableTrashBin GetClosestTrashBin(Vector3 pos)
	{
		float num = 10000f;
		int num2 = -1;
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_TrashBinList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_TrashBinList[i].IsValidObject())
			{
				float magnitude = (CSingleton<ShelfManager>.Instance.m_TrashBinList[i].transform.position - pos).magnitude;
				if (magnitude < num)
				{
					num = magnitude;
					num2 = i;
				}
			}
		}
		if (num2 >= 0)
		{
			return CSingleton<ShelfManager>.Instance.m_TrashBinList[num2];
		}
		return null;
	}

	public static void RemoveTrashBin(InteractableTrashBin trashBin)
	{
		CSingleton<ShelfManager>.Instance.m_TrashBinList.Remove(trashBin);
	}

	public static void InitEmptyBoxStorage(InteractableEmptyBoxStorage emptyBoxStorage)
	{
		CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList.Add(emptyBoxStorage);
	}

	public static List<InteractableEmptyBoxStorage> GetEmptyBoxStorageList()
	{
		return CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList;
	}

	public static InteractableEmptyBoxStorage GetEmptyBoxStorage(Vector3 pos)
	{
		float num = 10000f;
		int num2 = -1;
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList[i].IsValidObject() && CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList[i].HasStorageSpace())
			{
				float magnitude = (CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList[i].transform.position - pos).magnitude;
				if (magnitude < num)
				{
					num = magnitude;
					num2 = i;
				}
			}
		}
		if (num2 >= 0)
		{
			return CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList[num2];
		}
		return null;
	}

	public static void RemoveEmptyBoxStorage(InteractableEmptyBoxStorage emptyBoxStorage)
	{
		CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList.Remove(emptyBoxStorage);
	}

	public static void InitCashierCounter(InteractableCashierCounter cashierCounter)
	{
		CSingleton<ShelfManager>.Instance.m_CashierCounterList.Add(cashierCounter);
	}

	public static void RemoveCashierCounter(InteractableCashierCounter cashierCounter)
	{
		CSingleton<ShelfManager>.Instance.m_CashierCounterList.Remove(cashierCounter);
	}

	public static InteractableCashierCounter GetUnmannedCashierCounter()
	{
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count; i++)
		{
			if (!CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].IsMannedByPlayer() && !CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].IsMannedByNPC() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].IsValidObject() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].CanCheckout() && !CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].GetCurrentWorker())
			{
				return CSingleton<ShelfManager>.Instance.m_CashierCounterList[i];
			}
		}
		return null;
	}

	public static int GetCashierCounterCount()
	{
		return CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count;
	}

	public static InteractableCashierCounter GetCashierCounter()
	{
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].IsMannedByPlayer() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].IsValidObject() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].CanCheckout() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].GetCurrentQueingCustomerCount() < 1)
			{
				return CSingleton<ShelfManager>.Instance.m_CashierCounterList[i];
			}
		}
		for (int j = 0; j < CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count; j++)
		{
			if (CSingleton<ShelfManager>.Instance.m_CashierCounterList[j].IsMannedByNPC() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[j].IsValidObject() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[j].CanCheckout() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[j].GetCurrentQueingCustomerCount() < 1)
			{
				return CSingleton<ShelfManager>.Instance.m_CashierCounterList[j];
			}
		}
		int num = 100;
		int num2 = -1;
		for (int k = 0; k < CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count; k++)
		{
			if (CSingleton<ShelfManager>.Instance.m_CashierCounterList[k].IsValidObject() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[k].CanCheckout() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[k].GetCurrentQueingCustomerCount() < num && (CSingleton<ShelfManager>.Instance.m_CashierCounterList[k].IsMannedByPlayer() || CSingleton<ShelfManager>.Instance.m_CashierCounterList[k].IsMannedByNPC()))
			{
				num = CSingleton<ShelfManager>.Instance.m_CashierCounterList[k].GetCurrentQueingCustomerCount();
				num2 = k;
			}
		}
		if (num2 != -1 && Random.Range(0, 100) < 100)
		{
			return CSingleton<ShelfManager>.Instance.m_CashierCounterList[num2];
		}
		List<int> list = new List<int>();
		for (int l = 0; l < CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count; l++)
		{
			if (CSingleton<ShelfManager>.Instance.m_CashierCounterList[l].IsValidObject() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[l].CanCheckout())
			{
				list.Add(l);
			}
		}
		if (list.Count > 0)
		{
			int index = list[Random.Range(0, list.Count)];
			return CSingleton<ShelfManager>.Instance.m_CashierCounterList[index];
		}
		return null;
	}

	public static InteractableCashierCounter GetCashierCounterToTradeCard()
	{
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].IsValidObject() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].HasTradeCardSpace() && CSingleton<ShelfManager>.Instance.m_CashierCounterList[i].CanTradeCard())
			{
				return CSingleton<ShelfManager>.Instance.m_CashierCounterList[i];
			}
		}
		return null;
	}

	public static Shelf GetShelfToRestockItem(EItemType itemType, bool ignoreNoneType = false)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_ShelfList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_ShelfList[i].IsValidObject() && !CSingleton<ShelfManager>.Instance.m_ShelfList[i].m_ItemNotForSale && (bool)CSingleton<ShelfManager>.Instance.m_ShelfList[i].GetNonFullItemCompartment(itemType, ignoreNoneType))
			{
				list.Add(i);
			}
		}
		if (list.Count > 0)
		{
			int index = list[Random.Range(0, list.Count)];
			return CSingleton<ShelfManager>.Instance.m_ShelfList[index];
		}
		return null;
	}

	public static List<Shelf> GetShelfListToRestockItem(EItemType itemType, bool ignoreNoneType = false)
	{
		List<Shelf> list = new List<Shelf>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_ShelfList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_ShelfList[i].IsValidObject() && !CSingleton<ShelfManager>.Instance.m_ShelfList[i].m_ItemNotForSale && (bool)CSingleton<ShelfManager>.Instance.m_ShelfList[i].GetNonFullItemCompartment(itemType, ignoreNoneType))
			{
				list.Add(CSingleton<ShelfManager>.Instance.m_ShelfList[i]);
			}
		}
		return list;
	}

	public static List<EItemType> GetItemTypeListOnShelf()
	{
		List<EItemType> list = new List<EItemType>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_ShelfList.Count; i++)
		{
			if (!CSingleton<ShelfManager>.Instance.m_ShelfList[i].IsValidObject() || CSingleton<ShelfManager>.Instance.m_ShelfList[i].m_ItemNotForSale)
			{
				continue;
			}
			for (int j = 0; j < CSingleton<ShelfManager>.Instance.m_ShelfList[i].GetItemCompartmentList().Count; j++)
			{
				if (CSingleton<ShelfManager>.Instance.m_ShelfList[i].GetItemCompartmentList()[j].GetItemType() != EItemType.None && !list.Contains(CSingleton<ShelfManager>.Instance.m_ShelfList[i].GetItemCompartmentList()[j].GetItemType()))
				{
					list.Add(CSingleton<ShelfManager>.Instance.m_ShelfList[i].GetItemCompartmentList()[j].GetItemType());
				}
			}
		}
		return list;
	}

	public static List<WarehouseShelf> GetWarehouseShelfListToStoreItem(EItemType itemType, bool isBigBox, bool ignoreNoneType = false)
	{
		List<WarehouseShelf> list = new List<WarehouseShelf>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_WarehouseShelfList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i].IsValidObject() && (bool)CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i].GetNonFullItemCompartment(itemType, isBigBox, ignoreNoneType))
			{
				list.Add(CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[i]);
			}
		}
		return list;
	}

	public static Shelf GetShelfToBuyItem(List<EItemType> targetBuyItemList, int randomizeFindAnyShelfChanceOffset = 0, bool canReturnNull = false)
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_ShelfList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_ShelfList[i].IsValidObject() && !CSingleton<ShelfManager>.Instance.m_ShelfList[i].m_ItemNotForSale)
			{
				if (CSingleton<ShelfManager>.Instance.m_ShelfList[i].HasItemOnShelf() && (targetBuyItemList == null || targetBuyItemList.Count == 0 || CSingleton<ShelfManager>.Instance.m_ShelfList[i].HasItemTypeOnShelf(targetBuyItemList)))
				{
					list.Add(i);
				}
				list2.Add(i);
			}
		}
		if (list.Count > 0)
		{
			if (Random.Range(0, 100) < 80 + randomizeFindAnyShelfChanceOffset)
			{
				int index = list[Random.Range(0, list.Count)];
				return CSingleton<ShelfManager>.Instance.m_ShelfList[index];
			}
		}
		else if (canReturnNull)
		{
			return null;
		}
		if (list2.Count > 0)
		{
			int index2 = list2[Random.Range(0, list2.Count)];
			return CSingleton<ShelfManager>.Instance.m_ShelfList[index2];
		}
		return null;
	}

	public static Shelf GetShelfWithItemType(EItemType itemType)
	{
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_ShelfList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_ShelfList[i].IsValidObject() && !CSingleton<ShelfManager>.Instance.m_ShelfList[i].m_ItemNotForSale && CSingleton<ShelfManager>.Instance.m_ShelfList[i].HasItemOnShelf() && itemType != EItemType.None && CSingleton<ShelfManager>.Instance.m_ShelfList[i].HasItemTypeOnShelf(itemType))
			{
				return CSingleton<ShelfManager>.Instance.m_ShelfList[i];
			}
		}
		return null;
	}

	public static CardShelf GetCardShelfToBuyCard()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_CardShelfList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_CardShelfList[i].IsValidObject() && !CSingleton<ShelfManager>.Instance.m_CardShelfList[i].m_ItemNotForSale)
			{
				if (CSingleton<ShelfManager>.Instance.m_CardShelfList[i].HasCardOnShelf())
				{
					list.Add(i);
				}
				list2.Add(i);
			}
		}
		if (list.Count > 0 && Random.Range(0, 100) < 80)
		{
			int index = list[Random.Range(0, list.Count)];
			return CSingleton<ShelfManager>.Instance.m_CardShelfList[index];
		}
		if (list2.Count > 0)
		{
			int index2 = list2[Random.Range(0, list2.Count)];
			return CSingleton<ShelfManager>.Instance.m_CardShelfList[index2];
		}
		return null;
	}

	public static InteractableBulkDonationBox GetBulkDonationBoxToGetCard()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList[i].IsValidObject())
			{
				if (CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList[i].GetTotalCardAmount() > 0)
				{
					list.Add(i);
				}
				list2.Add(i);
			}
		}
		if (list.Count > 0 && Random.Range(0, 100) < 80)
		{
			int index = list[Random.Range(0, list.Count)];
			return CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList[index];
		}
		if (list2.Count > 0)
		{
			int index2 = list2[Random.Range(0, list2.Count)];
			return CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList[index2];
		}
		return null;
	}

	public static InteractableCardStorageShelf GetCardStorageShelf()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_CardStorageShelfList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_CardStorageShelfList[i].IsValidObject())
			{
				if (CSingleton<ShelfManager>.Instance.m_CardStorageShelfList[i].GetTotalCardAmount() > 0 && CSingleton<ShelfManager>.Instance.m_CardStorageShelfList[i].CanWorkerTake())
				{
					list.Add(i);
				}
				list2.Add(i);
			}
		}
		if (list.Count > 0 && Random.Range(0, 100) < 80)
		{
			int index = list[Random.Range(0, list.Count)];
			return CSingleton<ShelfManager>.Instance.m_CardStorageShelfList[index];
		}
		if (list2.Count > 0)
		{
			int index2 = list2[Random.Range(0, list2.Count)];
			return CSingleton<ShelfManager>.Instance.m_CardStorageShelfList[index2];
		}
		return null;
	}

	public static bool HasPlayTableWithPlayerWaiting()
	{
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_PlayTableList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_PlayTableList[i].IsValidObject() && CSingleton<ShelfManager>.Instance.m_PlayTableList[i].IsLookingForPlayer(findTableWithPlayerWaiting: true))
			{
				return true;
			}
		}
		return false;
	}

	public static InteractablePlayTable GetPlayTableToPlay(bool findTableWithPlayerWaiting)
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < CSingleton<ShelfManager>.Instance.m_PlayTableList.Count; i++)
		{
			if (CSingleton<ShelfManager>.Instance.m_PlayTableList[i].IsValidObject())
			{
				if (CSingleton<ShelfManager>.Instance.m_PlayTableList[i].IsLookingForPlayer(findTableWithPlayerWaiting))
				{
					list.Add(i);
					list2.Add(i);
				}
				else if (!findTableWithPlayerWaiting && CSingleton<ShelfManager>.Instance.m_PlayTableList[i].HasEmptySeatBooking() && CSingleton<ShelfManager>.Instance.m_PlayTableList[i].HasEmptyQueue() && !CSingleton<ShelfManager>.Instance.m_PlayTableList[i].GetHasStartPlay() && !CSingleton<ShelfManager>.Instance.m_PlayTableList[i].GetHasStartPlayerPlayCard())
				{
					list2.Add(i);
				}
			}
		}
		if (list.Count > 0 && Random.Range(0, 100) < 80)
		{
			int index = list[Random.Range(0, list.Count)];
			return CSingleton<ShelfManager>.Instance.m_PlayTableList[index];
		}
		if (list2.Count > 0)
		{
			int index2 = list2[Random.Range(0, list2.Count)];
			return CSingleton<ShelfManager>.Instance.m_PlayTableList[index2];
		}
		return null;
	}

	public static void DestroyAllObject()
	{
		CSingleton<ShelfManager>.Instance.m_SpawnedObjectCount = 0;
		for (int num = CSingleton<ShelfManager>.Instance.m_InteractableObjectList.Count - 1; num >= 0; num--)
		{
			CSingleton<ShelfManager>.Instance.m_InteractableObjectList[num].OnDestroyed();
		}
		for (int num2 = CSingleton<ShelfManager>.Instance.m_ShelfList.Count - 1; num2 >= 0; num2--)
		{
			CSingleton<ShelfManager>.Instance.m_ShelfList[num2].OnDestroyed();
		}
		for (int num3 = CSingleton<ShelfManager>.Instance.m_WarehouseShelfList.Count - 1; num3 >= 0; num3--)
		{
			CSingleton<ShelfManager>.Instance.m_WarehouseShelfList[num3].OnDestroyed();
		}
		for (int num4 = CSingleton<ShelfManager>.Instance.m_PlayTableList.Count - 1; num4 >= 0; num4--)
		{
			CSingleton<ShelfManager>.Instance.m_PlayTableList[num4].OnDestroyed();
		}
		for (int num5 = CSingleton<ShelfManager>.Instance.m_AutoCleanserList.Count - 1; num5 >= 0; num5--)
		{
			CSingleton<ShelfManager>.Instance.m_AutoCleanserList[num5].OnDestroyed();
		}
		for (int num6 = CSingleton<ShelfManager>.Instance.m_AutoPackOpenerList.Count - 1; num6 >= 0; num6--)
		{
			CSingleton<ShelfManager>.Instance.m_AutoPackOpenerList[num6].OnDestroyed();
		}
		for (int num7 = CSingleton<ShelfManager>.Instance.m_CardShelfList.Count - 1; num7 >= 0; num7--)
		{
			CSingleton<ShelfManager>.Instance.m_CardShelfList[num7].OnDestroyed();
		}
		for (int num8 = CSingleton<ShelfManager>.Instance.m_WorkbenchList.Count - 1; num8 >= 0; num8--)
		{
			CSingleton<ShelfManager>.Instance.m_WorkbenchList[num8].OnDestroyed();
		}
		for (int num9 = CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count - 1; num9 >= 0; num9--)
		{
			CSingleton<ShelfManager>.Instance.m_CashierCounterList[num9].OnDestroyed();
		}
		for (int num10 = CSingleton<ShelfManager>.Instance.m_DecoObjectList.Count - 1; num10 >= 0; num10--)
		{
			CSingleton<ShelfManager>.Instance.m_DecoObjectList[num10].OnDestroyed();
		}
		for (int num11 = CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList.Count - 1; num11 >= 0; num11--)
		{
			CSingleton<ShelfManager>.Instance.m_EmptyBoxStorageList[num11].OnDestroyed();
		}
		for (int num12 = CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList.Count - 1; num12 >= 0; num12--)
		{
			CSingleton<ShelfManager>.Instance.m_BulkDonationBoxList[num12].OnDestroyed();
		}
		for (int num13 = CSingleton<ShelfManager>.Instance.m_CardStorageShelfList.Count - 1; num13 >= 0; num13--)
		{
			CSingleton<ShelfManager>.Instance.m_CardStorageShelfList[num13].OnDestroyed();
		}
	}

	public static bool HasCustomerInCashCounterQueue()
	{
		for (int num = CSingleton<ShelfManager>.Instance.m_CashierCounterList.Count - 1; num >= 0; num--)
		{
			if (CSingleton<ShelfManager>.Instance.m_CashierCounterList[num].GetCustomerInQueueCount() > 0)
			{
				return true;
			}
		}
		return false;
	}

	public static void OnPressGoNextDay()
	{
		for (int num = CSingleton<ShelfManager>.Instance.m_PlayTableList.Count - 1; num >= 0; num--)
		{
			CSingleton<ShelfManager>.Instance.m_PlayTableList[num].OnPressGoNextDay();
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected void OnMoneyCurrencyUpdated(CEventPlayer_OnMoneyCurrencyUpdated evt)
	{
		for (int i = 0; i < m_ShelfList.Count; i++)
		{
			for (int j = 0; j < m_ShelfList[i].GetItemCompartmentList().Count; j++)
			{
				m_ShelfList[i].GetItemCompartmentList()[j].RefreshPriceTagItemPriceText();
			}
		}
		for (int k = 0; k < m_CardShelfList.Count; k++)
		{
			for (int l = 0; l < m_CardShelfList[k].GetCardCompartmentList().Count; l++)
			{
				m_CardShelfList[k].GetCardCompartmentList()[l].RefreshPriceTagItemPriceText();
			}
		}
	}
}
