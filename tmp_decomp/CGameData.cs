using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CGameData
{
	public static CGameData instance;

	public int m_SaveIndex;

	public int m_SaveCycle;

	public bool m_CanCloudLoad;

	public DateTime m_LastLocalExitTime;

	public string m_LastLoginPlayfabID;

	public LightTimeData m_LightTimeData;

	public EGameEventFormat m_GameEventFormat;

	public EGameEventFormat m_PendingGameEventFormat;

	public ECardExpansionType m_GameEventExpansionType;

	public ECardExpansionType m_PendingGameEventExpansionType;

	public GameReportDataCollect m_GameReportDataCollect;

	public GameReportDataCollect m_GameReportDataCollectPermanent;

	public List<GameReportDataCollect> m_GameReportDataCollectPastList;

	public List<ShelfSaveData> m_ShelfSaveDataList;

	public List<WarehouseShelfSaveData> m_WarehouseShelfSaveDataList;

	public List<PackageBoxItemaveData> m_PackageBoxItemSaveDataList;

	public List<PackageBoxCardSaveData> m_PackageBoxCardSaveDataList;

	public List<InteractableObjectSaveData> m_InteractableObjectSaveDataList;

	public List<DecoObjectSaveData> m_DecoObjectSaveDataList;

	public List<CardShelfSaveData> m_CardShelfSaveDataList;

	public List<PlayTableSaveData> m_PlayTableSaveDataList;

	public List<AutoCleanserSaveData> m_AutoCleanserSaveDataList;

	public List<AutoPackOpenerSaveData> m_AutoPackOpenerSaveDataList;

	public List<EmptyBoxStorageSaveData> m_EmptyBoxStorageSaveDataList;

	public List<BulkDonationBoxSaveData> m_BulkDonationSaveDataList;

	public List<CardStorageShelfSaveData> m_CardStorageShelfSaveDataList;

	public List<WorkbenchSaveData> m_WorkbenchSaveDataList;

	public List<CashierCounterSaveData> m_CashCounterSaveDataList;

	public List<CustomerSaveData> m_CustomerSaveDataList;

	public List<WorkerSaveData> m_WorkerSaveDataList;

	public List<TutorialData> m_TutorialDataList;

	public List<CustomerReviewData> m_CustomerReviewDataList;

	public List<DeckCompactCardDataList> m_DeckCompactCardDataList;

	public List<BillData> m_BillList;

	public List<CardData> m_HoldCardDataList;

	public List<EItemType> m_HoldItemTypeList;

	public List<EItemType> m_TargetBuyItemList;

	public bool m_IsShopOpen;

	public bool m_IsShopOnceOpen;

	public bool m_IsWarehouseDoorClosed;

	public bool m_IsItemPriceGenerated;

	public bool m_IsCardPriceGenerated;

	public List<int> m_CurrentTotalItemCountList;

	public List<float> m_SetItemPriceList;

	public List<float> m_AverageItemCostList;

	public List<float> m_GeneratedCostPriceList;

	public List<float> m_GeneratedMarketPriceList;

	public List<float> m_ItemPricePercentChangeList;

	public List<FloatList> m_ItemPricePercentPastChangeList;

	public List<float> m_SetGameEventPriceList;

	public List<float> m_GeneratedGameEventPriceList;

	public List<float> m_GameEventPricePercentChangeList;

	public List<TransactionData> m_TransactionDataList;

	public List<int> m_SpawnBoxRestockIndexWaitingList;

	public List<int> m_SpawnBoxItemCountWaitingList;

	public List<int> m_StockSoldList;

	public List<int> m_CollectionCardPackCountList;

	public List<int> m_CardCollectedList;

	public List<int> m_CardCollectedListDestiny;

	public List<int> m_CardCollectedListGhost;

	public List<int> m_CardCollectedListGhostBlack;

	public List<int> m_CardCollectedListMegabot;

	public List<int> m_CardCollectedListFantasyRPG;

	public List<int> m_CardCollectedListCatJob;

	public List<CompactCardDataAmount> m_GradedCardInventoryList;

	public List<bool> m_IsCardCollectedList;

	public List<bool> m_IsCardCollectedListDestiny;

	public List<bool> m_IsCardCollectedListGhost;

	public List<bool> m_IsCardCollectedListGhostBlack;

	public List<bool> m_IsCardCollectedListMegabot;

	public List<bool> m_IsCardCollectedListFantasyRPG;

	public List<bool> m_IsCardCollectedListCatJob;

	public List<float> m_CardPriceSetList;

	public List<float> m_CardPriceSetListDestiny;

	public List<float> m_CardPriceSetListGhost;

	public List<float> m_CardPriceSetListGhostBlack;

	public List<float> m_CardPriceSetListMegabot;

	public List<float> m_CardPriceSetListFantasyRPG;

	public List<float> m_CardPriceSetListCatJob;

	public List<FloatList> m_GradedCardPriceSetList;

	public List<FloatList> m_GradedCardPriceSetListDestiny;

	public List<FloatList> m_GradedCardPriceSetListGhost;

	public List<FloatList> m_GradedCardPriceSetListGhostBlack;

	public List<FloatList> m_GradedCardPriceSetListMegabot;

	public List<FloatList> m_GradedCardPriceSetListFantasyRPG;

	public List<FloatList> m_GradedCardPriceSetListCatJob;

	public List<MarketPrice> m_GenCardMarketPriceList;

	public List<MarketPrice> m_GenCardMarketPriceListDestiny;

	public List<MarketPrice> m_GenCardMarketPriceListGhost;

	public List<MarketPrice> m_GenCardMarketPriceListGhostBlack;

	public List<MarketPrice> m_GenCardMarketPriceListMegabot;

	public List<MarketPrice> m_GenCardMarketPriceListFantasyRPG;

	public List<MarketPrice> m_GenCardMarketPriceListCatJob;

	public List<float> m_GenGradedCardPriceMultiplierList;

	public GradeCardSubmitSet m_CurrentGradeCardSubmitSet;

	public List<GradeCardSubmitSet> m_GradeCardInProgressList;

	public List<int> m_CollectionSortingMethodIndexList;

	public List<int> m_ChampionCardCollectedList;

	public List<bool> m_IsItemLicenseUnlocked;

	public List<bool> m_IsWorkerHired;

	public List<bool> m_IsAchievementUnlocked;

	public List<int> m_DecorationInventoryList;

	public List<bool> m_UnlockedDecoWallList;

	public List<bool> m_UnlockedDecoFloorList;

	public List<bool> m_UnlockedDecoCeilingList;

	public string m_PlayerName;

	public float m_CoinAmount;

	public double m_CoinAmountDouble;

	public int m_FamePoint;

	public int m_TotalFameAdd;

	public bool m_IsScannerRestockUnlocked;

	public bool m_IsWarehouseRoomUnlocked;

	public int m_UnlockRoomCount;

	public int m_UnlockWarehouseRoomCount;

	public int m_CurrentDay;

	public int m_ShopExpPoint;

	public int m_ShopLevel;

	public int m_EquippedWallDecoIndex;

	public int m_EquippedWallDecoIndexB;

	public int m_EquippedFloorDecoIndex;

	public int m_EquippedFloorDecoIndexB;

	public int m_EquippedCeilingDecoIndex;

	public int m_EquippedCeilingDecoIndexB;

	public int m_GradedCollectionSortingMethodIndex;

	public int m_CloudSaveCountdown;

	public int m_TutorialIndex;

	public int m_TutorialSubgroupIndex;

	public int m_CurrentSelectedDeckIndex;

	public int m_CustomerReviewCount;

	public float m_CustomerReviewScoreAverage;

	public bool m_HasFinishedTutorial;

	public bool m_IsMainGame;

	public bool m_HasGetGhostCard;

	public float m_MusicVolumeDecrease;

	public float m_SoundVolumeDecrease;

	public int m_WorkbenchMinimumCardLimit;

	public float m_WorkbenchPriceLimit;

	public float m_WorkbenchPriceMinimum;

	public ERarity m_WorkbenchRarityLimit;

	public ECardExpansionType m_WorkbenchCardExpansionType;

	public ERestockSortingType m_RestockSortingType;

	public int m_QuickFillMinimumCardLimit;

	public float m_QuickFillPriceLimit;

	public float m_QuickFillPriceMinimum;

	public ERarity m_QuickFillRarityLimit;

	public ECardExpansionType m_QuickFillCardExpansionType;

	public string m_DebugString;

	public string m_DebugString2;

	public int m_DebugDataCount;

	public CGameData()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Awake()
	{
	}

	public bool IsCloudVersionNewerThanLocal()
	{
		int num = 0;
		int num2 = 0;
		string text = "";
		if (CSaveLoad.m_SavedGame != null)
		{
			num = CSaveLoad.m_SavedGame.m_SaveIndex;
			num2 = CSaveLoad.m_SavedGame.m_SaveCycle;
			text = CSaveLoad.m_SavedGame.m_LastLoginPlayfabID;
		}
		Debug.Log("DSCloudSaveLoadTest Local save index : " + num + " Cloud save index : " + CSaveLoad.m_SavedGameBackup.m_SaveIndex);
		Debug.Log("DSCloudSaveLoadTest Local save cycle : " + num2 + " Cloud save cycle : " + CSaveLoad.m_SavedGameBackup.m_SaveCycle);
		bool result = false;
		bool flag = false;
		if (CSaveLoad.m_SavedGameBackup.m_LastLoginPlayfabID != null && CSaveLoad.m_SavedGameBackup.m_LastLoginPlayfabID != " " && CSaveLoad.m_SavedGameBackup.m_LastLoginPlayfabID != "" && CSaveLoad.m_SavedGameBackup.m_LastLoginPlayfabID != text)
		{
			flag = true;
		}
		Debug.Log("DSCloudSaveLoadTest localPlayfabID : " + text + " CSaveLoad.m_SavedGameBackup.m_LastLoginPlayfabID : " + CSaveLoad.m_SavedGameBackup.m_LastLoginPlayfabID + " isDifferentID : " + flag);
		if (flag)
		{
			CSaveLoad.Delete();
			CSaveLoad.DeleteBackup();
			Debug.Log("DSCloudSaveLoadTest has different id");
			return true;
		}
		if (CSaveLoad.m_SavedGameBackup.m_SaveCycle > num2)
		{
			Debug.Log("DSCloudSaveLoadTest Cloud file has higher version number");
			result = true;
		}
		else if (CSaveLoad.m_SavedGameBackup.m_SaveCycle == num2)
		{
			if (CSaveLoad.m_SavedGameBackup.m_SaveIndex > num)
			{
				Debug.Log("DSCloudSaveLoadTest Cloud file has higher version number");
				result = true;
			}
			else if (CSaveLoad.m_SavedGameBackup.m_SaveIndex == num)
			{
				Debug.Log("DSCloudSaveLoadTest Cloud and local file have same version number");
			}
			else
			{
				Debug.Log("DSCloudSaveLoadTest Local file has higher version number");
			}
		}
		else
		{
			Debug.Log("DSCloudSaveLoadTest Local file has higher version number");
		}
		return result;
	}

	public LoadSavedSlotData GetLoadSavedSlotData(CGameData gameData)
	{
		LoadSavedSlotData result = default(LoadSavedSlotData);
		if (gameData != null)
		{
			result.name = gameData.m_PlayerName;
			result.moneyAmount = gameData.m_CoinAmount;
			result.level = gameData.m_ShopLevel;
			result.daysPassed = gameData.m_CurrentDay;
			result.hasSaveData = true;
		}
		return result;
	}

	public void PropagateLoadDataPrologue(CGameData gameData)
	{
	}

	public void PropagateLoadData(CGameData gameData)
	{
		GameInstance.m_HasLoadingError = true;
		if (!gameData.m_IsMainGame)
		{
			GameInstance.m_HasLoadingError = false;
			GameInstance.m_SaveFileNotFound = true;
			CPlayerData.CreateDefaultData();
			return;
		}
		try
		{
			CPlayerData.m_SaveIndex = gameData.m_SaveIndex;
			CPlayerData.m_SaveCycle = gameData.m_SaveCycle;
			CPlayerData.m_LastLoginPlayfabID = gameData.m_LastLoginPlayfabID;
			CPlayerData.m_LastLocalExitTime = gameData.m_LastLocalExitTime;
			CPlayerData.PlayerName = gameData.m_PlayerName;
			CPlayerData.m_CoinAmount = gameData.m_CoinAmount;
			if (gameData.m_CoinAmountDouble < (double)gameData.m_CoinAmount)
			{
				gameData.m_CoinAmountDouble = gameData.m_CoinAmount;
			}
			CPlayerData.m_CoinAmountDouble = gameData.m_CoinAmountDouble;
			CEventManager.QueueEvent(new CEventPlayer_SetCoin(CPlayerData.m_CoinAmount, CPlayerData.m_CoinAmountDouble));
			CPlayerData.m_FamePoint = gameData.m_FamePoint;
			CPlayerData.m_IsScannerRestockUnlocked = gameData.m_IsScannerRestockUnlocked;
			CPlayerData.m_IsWarehouseRoomUnlocked = gameData.m_IsWarehouseRoomUnlocked;
			CPlayerData.m_UnlockRoomCount = gameData.m_UnlockRoomCount;
			CPlayerData.m_UnlockWarehouseRoomCount = gameData.m_UnlockWarehouseRoomCount;
			CEventManager.QueueEvent(new CEventPlayer_SetFame(CPlayerData.m_FamePoint));
			CPlayerData.m_TotalFameAdd = gameData.m_TotalFameAdd;
			CPlayerData.m_CloudSaveCountdown = gameData.m_CloudSaveCountdown;
			CPlayerData.m_IsShopOpen = gameData.m_IsShopOpen;
			CPlayerData.m_IsShopOnceOpen = gameData.m_IsShopOnceOpen;
			CPlayerData.m_IsWarehouseDoorClosed = gameData.m_IsWarehouseDoorClosed;
			CPlayerData.m_IsItemPriceGenerated = gameData.m_IsItemPriceGenerated;
			CPlayerData.m_IsCardPriceGenerated = gameData.m_IsCardPriceGenerated;
			CPlayerData.m_CurrentDay = gameData.m_CurrentDay;
			CPlayerData.m_ShopExpPoint = gameData.m_ShopExpPoint;
			CPlayerData.m_ShopLevel = gameData.m_ShopLevel;
			CPlayerData.m_EquippedWallDecoIndex = gameData.m_EquippedWallDecoIndex;
			CPlayerData.m_EquippedWallDecoIndexB = gameData.m_EquippedWallDecoIndexB;
			CPlayerData.m_EquippedFloorDecoIndex = gameData.m_EquippedFloorDecoIndex;
			CPlayerData.m_EquippedFloorDecoIndexB = gameData.m_EquippedFloorDecoIndexB;
			CPlayerData.m_EquippedCeilingDecoIndex = gameData.m_EquippedCeilingDecoIndex;
			CPlayerData.m_EquippedCeilingDecoIndexB = gameData.m_EquippedCeilingDecoIndexB;
			CPlayerData.m_GradedCollectionSortingMethodIndex = gameData.m_GradedCollectionSortingMethodIndex;
			if (gameData.m_CurrentTotalItemCountList != null && gameData.m_CurrentTotalItemCountList.Count > 0)
			{
				CPlayerData.m_CurrentTotalItemCountList = gameData.m_CurrentTotalItemCountList;
			}
			else
			{
				CPlayerData.m_CurrentTotalItemCountList.Clear();
				for (int i = 0; i < 100; i++)
				{
					CPlayerData.m_CurrentTotalItemCountList.Add(0);
				}
			}
			if (gameData.m_SetItemPriceList != null && gameData.m_SetItemPriceList.Count > 0)
			{
				CPlayerData.m_SetItemPriceList = gameData.m_SetItemPriceList;
			}
			else
			{
				CPlayerData.m_SetItemPriceList.Clear();
				for (int j = 0; j < 100; j++)
				{
					CPlayerData.m_SetItemPriceList.Add(0f);
				}
			}
			if (gameData.m_AverageItemCostList != null && gameData.m_AverageItemCostList.Count > 0)
			{
				CPlayerData.m_AverageItemCostList = gameData.m_AverageItemCostList;
			}
			else
			{
				CPlayerData.m_AverageItemCostList.Clear();
				for (int k = 0; k < 100; k++)
				{
					CPlayerData.m_AverageItemCostList.Add(0f);
				}
			}
			if (gameData.m_GeneratedCostPriceList != null && gameData.m_GeneratedCostPriceList.Count > 0)
			{
				CPlayerData.m_GeneratedCostPriceList = gameData.m_GeneratedCostPriceList;
			}
			else
			{
				CPlayerData.m_GeneratedCostPriceList.Clear();
				for (int l = 0; l < 100; l++)
				{
					CPlayerData.m_GeneratedCostPriceList.Add(0f);
				}
			}
			if (gameData.m_GeneratedMarketPriceList != null && gameData.m_GeneratedMarketPriceList.Count > 0)
			{
				CPlayerData.m_GeneratedMarketPriceList = gameData.m_GeneratedMarketPriceList;
			}
			else
			{
				CPlayerData.m_GeneratedMarketPriceList.Clear();
				for (int m = 0; m < 100; m++)
				{
					CPlayerData.m_GeneratedMarketPriceList.Add(0f);
				}
			}
			if (gameData.m_ItemPricePercentChangeList != null && gameData.m_ItemPricePercentChangeList.Count > 0)
			{
				CPlayerData.m_ItemPricePercentChangeList = gameData.m_ItemPricePercentChangeList;
			}
			else
			{
				CPlayerData.m_ItemPricePercentChangeList.Clear();
				for (int n = 0; n < 100; n++)
				{
					CPlayerData.m_ItemPricePercentChangeList.Add(0f);
				}
			}
			if (gameData.m_ItemPricePercentPastChangeList != null && gameData.m_ItemPricePercentPastChangeList.Count > 0)
			{
				CPlayerData.m_ItemPricePercentPastChangeList = gameData.m_ItemPricePercentPastChangeList;
			}
			if (gameData.m_SetGameEventPriceList != null && gameData.m_SetGameEventPriceList.Count > 0)
			{
				CPlayerData.m_SetGameEventPriceList = gameData.m_SetGameEventPriceList;
			}
			else
			{
				CPlayerData.m_SetGameEventPriceList.Clear();
				for (int num = 0; num < 100; num++)
				{
					CPlayerData.m_SetGameEventPriceList.Add(0f);
				}
			}
			if (gameData.m_GeneratedGameEventPriceList != null && gameData.m_GeneratedGameEventPriceList.Count > 0)
			{
				CPlayerData.m_GeneratedGameEventPriceList = gameData.m_GeneratedGameEventPriceList;
			}
			else
			{
				CPlayerData.m_GeneratedGameEventPriceList.Clear();
				for (int num2 = 0; num2 < 100; num2++)
				{
					CPlayerData.m_GeneratedGameEventPriceList.Add(0f);
				}
			}
			if (gameData.m_GameEventPricePercentChangeList != null && gameData.m_GameEventPricePercentChangeList.Count > 0)
			{
				CPlayerData.m_GameEventPricePercentChangeList = gameData.m_GameEventPricePercentChangeList;
			}
			else
			{
				CPlayerData.m_GameEventPricePercentChangeList.Clear();
				for (int num3 = 0; num3 < 100; num3++)
				{
					CPlayerData.m_GameEventPricePercentChangeList.Add(0f);
				}
			}
			if (gameData.m_TransactionDataList != null && gameData.m_TransactionDataList.Count > 0)
			{
				CPlayerData.m_TransactionDataList = gameData.m_TransactionDataList;
			}
			if (gameData.m_SpawnBoxRestockIndexWaitingList != null && gameData.m_SpawnBoxRestockIndexWaitingList.Count > 0)
			{
				CPlayerData.m_SpawnBoxRestockIndexWaitingList = gameData.m_SpawnBoxRestockIndexWaitingList;
			}
			if (gameData.m_SpawnBoxItemCountWaitingList != null && gameData.m_SpawnBoxItemCountWaitingList.Count > 0)
			{
				CPlayerData.m_SpawnBoxItemCountWaitingList = gameData.m_SpawnBoxItemCountWaitingList;
			}
			if (gameData.m_StockSoldList != null && gameData.m_StockSoldList.Count > 0)
			{
				CPlayerData.m_StockSoldList = gameData.m_StockSoldList;
			}
			if (gameData.m_CollectionCardPackCountList != null && gameData.m_CollectionCardPackCountList.Count > 0)
			{
				CPlayerData.m_CollectionCardPackCountList = gameData.m_CollectionCardPackCountList;
			}
			if (gameData.m_CardCollectedList != null && gameData.m_CardCollectedList.Count > 0)
			{
				CPlayerData.m_CardCollectedList = gameData.m_CardCollectedList;
			}
			if (CPlayerData.m_CardCollectedList.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num4 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_CardCollectedList.Count;
				for (int num5 = 0; num5 < num4; num5++)
				{
					CPlayerData.m_CardCollectedList.Add(0);
				}
			}
			if (gameData.m_CardCollectedListDestiny != null && gameData.m_CardCollectedListDestiny.Count > 0)
			{
				CPlayerData.m_CardCollectedListDestiny = gameData.m_CardCollectedListDestiny;
			}
			if (CPlayerData.m_CardCollectedListDestiny.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num6 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_CardCollectedListDestiny.Count;
				for (int num7 = 0; num7 < num6; num7++)
				{
					CPlayerData.m_CardCollectedListDestiny.Add(0);
				}
			}
			if (gameData.m_CardCollectedListGhost != null && gameData.m_CardCollectedListGhost.Count > 0)
			{
				CPlayerData.m_CardCollectedListGhost = gameData.m_CardCollectedListGhost;
			}
			if (CPlayerData.m_CardCollectedListGhost.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num8 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_CardCollectedListGhost.Count;
				for (int num9 = 0; num9 < num8; num9++)
				{
					CPlayerData.m_CardCollectedListGhost.Add(0);
				}
			}
			if (gameData.m_CardCollectedListGhostBlack != null && gameData.m_CardCollectedListGhostBlack.Count > 0)
			{
				CPlayerData.m_CardCollectedListGhostBlack = gameData.m_CardCollectedListGhostBlack;
			}
			if (CPlayerData.m_CardCollectedListGhostBlack.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num10 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_CardCollectedListGhostBlack.Count;
				for (int num11 = 0; num11 < num10; num11++)
				{
					CPlayerData.m_CardCollectedListGhostBlack.Add(0);
				}
			}
			if (gameData.m_CardCollectedListMegabot != null && gameData.m_CardCollectedListMegabot.Count > 0)
			{
				CPlayerData.m_CardCollectedListMegabot = gameData.m_CardCollectedListMegabot;
			}
			if (CPlayerData.m_CardCollectedListMegabot.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num12 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_CardCollectedListMegabot.Count;
				for (int num13 = 0; num13 < num12; num13++)
				{
					CPlayerData.m_CardCollectedListMegabot.Add(0);
				}
			}
			if (gameData.m_CardCollectedListFantasyRPG != null && gameData.m_CardCollectedListFantasyRPG.Count > 0)
			{
				CPlayerData.m_CardCollectedListFantasyRPG = gameData.m_CardCollectedListFantasyRPG;
			}
			if (CPlayerData.m_CardCollectedListFantasyRPG.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num14 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_CardCollectedListFantasyRPG.Count;
				for (int num15 = 0; num15 < num14; num15++)
				{
					CPlayerData.m_CardCollectedListFantasyRPG.Add(0);
				}
			}
			if (gameData.m_CardCollectedListCatJob != null && gameData.m_CardCollectedListCatJob.Count > 0)
			{
				CPlayerData.m_CardCollectedListCatJob = gameData.m_CardCollectedListCatJob;
			}
			if (CPlayerData.m_CardCollectedListCatJob.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num16 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_CardCollectedListCatJob.Count;
				for (int num17 = 0; num17 < num16; num17++)
				{
					CPlayerData.m_CardCollectedListCatJob.Add(0);
				}
			}
			if (gameData.m_GradedCardInventoryList != null && gameData.m_GradedCardInventoryList.Count > 0)
			{
				CPlayerData.m_GradedCardInventoryList = gameData.m_GradedCardInventoryList;
			}
			if (gameData.m_IsCardCollectedList != null && gameData.m_IsCardCollectedList.Count > 0)
			{
				CPlayerData.m_IsCardCollectedList = gameData.m_IsCardCollectedList;
			}
			if (CPlayerData.m_IsCardCollectedList.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num18 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_IsCardCollectedList.Count;
				for (int num19 = 0; num19 < num18; num19++)
				{
					CPlayerData.m_IsCardCollectedList.Add(item: false);
				}
			}
			if (gameData.m_IsCardCollectedListDestiny != null && gameData.m_IsCardCollectedListDestiny.Count > 0)
			{
				CPlayerData.m_IsCardCollectedListDestiny = gameData.m_IsCardCollectedListDestiny;
			}
			if (CPlayerData.m_IsCardCollectedListDestiny.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num20 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_IsCardCollectedListDestiny.Count;
				for (int num21 = 0; num21 < num20; num21++)
				{
					CPlayerData.m_IsCardCollectedListDestiny.Add(item: false);
				}
			}
			if (gameData.m_IsCardCollectedListGhost != null && gameData.m_IsCardCollectedListGhost.Count > 0)
			{
				CPlayerData.m_IsCardCollectedListGhost = gameData.m_IsCardCollectedListGhost;
			}
			if (CPlayerData.m_IsCardCollectedListGhost.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num22 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_IsCardCollectedListGhost.Count;
				for (int num23 = 0; num23 < num22; num23++)
				{
					CPlayerData.m_IsCardCollectedListGhost.Add(item: false);
				}
			}
			if (gameData.m_IsCardCollectedListGhostBlack != null && gameData.m_IsCardCollectedListGhostBlack.Count > 0)
			{
				CPlayerData.m_IsCardCollectedListGhostBlack = gameData.m_IsCardCollectedListGhostBlack;
			}
			if (CPlayerData.m_IsCardCollectedListGhostBlack.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num24 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_IsCardCollectedListGhostBlack.Count;
				for (int num25 = 0; num25 < num24; num25++)
				{
					CPlayerData.m_IsCardCollectedListGhostBlack.Add(item: false);
				}
			}
			if (gameData.m_IsCardCollectedListMegabot != null && gameData.m_IsCardCollectedListMegabot.Count > 0)
			{
				CPlayerData.m_IsCardCollectedListMegabot = gameData.m_IsCardCollectedListMegabot;
			}
			if (CPlayerData.m_IsCardCollectedListMegabot.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num26 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_IsCardCollectedListMegabot.Count;
				for (int num27 = 0; num27 < num26; num27++)
				{
					CPlayerData.m_IsCardCollectedListMegabot.Add(item: false);
				}
			}
			if (gameData.m_IsCardCollectedListFantasyRPG != null && gameData.m_IsCardCollectedListFantasyRPG.Count > 0)
			{
				CPlayerData.m_IsCardCollectedListFantasyRPG = gameData.m_IsCardCollectedListFantasyRPG;
			}
			if (CPlayerData.m_IsCardCollectedListFantasyRPG.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num28 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_IsCardCollectedListFantasyRPG.Count;
				for (int num29 = 0; num29 < num28; num29++)
				{
					CPlayerData.m_IsCardCollectedListFantasyRPG.Add(item: false);
				}
			}
			if (gameData.m_IsCardCollectedListCatJob != null && gameData.m_IsCardCollectedListCatJob.Count > 0)
			{
				CPlayerData.m_IsCardCollectedListCatJob = gameData.m_IsCardCollectedListCatJob;
			}
			if (CPlayerData.m_IsCardCollectedListCatJob.Count < CPlayerData.GetCardCollectionDataCount())
			{
				int num30 = CPlayerData.GetCardCollectionDataCount() - CPlayerData.m_IsCardCollectedListCatJob.Count;
				for (int num31 = 0; num31 < num30; num31++)
				{
					CPlayerData.m_IsCardCollectedListCatJob.Add(item: false);
				}
			}
			if (gameData.m_CardPriceSetList != null && gameData.m_CardPriceSetList.Count > 0)
			{
				CPlayerData.m_CardPriceSetList = gameData.m_CardPriceSetList;
				CPlayerData.m_CardPriceSetListDestiny = gameData.m_CardPriceSetListDestiny;
				CPlayerData.m_CardPriceSetListGhost = gameData.m_CardPriceSetListGhost;
				CPlayerData.m_CardPriceSetListGhostBlack = gameData.m_CardPriceSetListGhostBlack;
				CPlayerData.m_CardPriceSetListMegabot = gameData.m_CardPriceSetListMegabot;
				CPlayerData.m_CardPriceSetListFantasyRPG = gameData.m_CardPriceSetListFantasyRPG;
				CPlayerData.m_CardPriceSetListCatJob = gameData.m_CardPriceSetListCatJob;
				CPlayerData.m_GenCardMarketPriceList = gameData.m_GenCardMarketPriceList;
				CPlayerData.m_GenCardMarketPriceListDestiny = gameData.m_GenCardMarketPriceListDestiny;
				CPlayerData.m_GenCardMarketPriceListGhost = gameData.m_GenCardMarketPriceListGhost;
				CPlayerData.m_GenCardMarketPriceListGhostBlack = gameData.m_GenCardMarketPriceListGhostBlack;
				CPlayerData.m_GenCardMarketPriceListMegabot = gameData.m_GenCardMarketPriceListMegabot;
				CPlayerData.m_GenCardMarketPriceListFantasyRPG = gameData.m_GenCardMarketPriceListFantasyRPG;
				CPlayerData.m_GenCardMarketPriceListCatJob = gameData.m_GenCardMarketPriceListCatJob;
			}
			if (gameData.m_GradedCardPriceSetList != null && gameData.m_GradedCardPriceSetList.Count > 0)
			{
				CPlayerData.m_GradedCardPriceSetList = gameData.m_GradedCardPriceSetList;
				CPlayerData.m_GradedCardPriceSetListDestiny = gameData.m_GradedCardPriceSetListDestiny;
				CPlayerData.m_GradedCardPriceSetListGhost = gameData.m_GradedCardPriceSetListGhost;
				CPlayerData.m_GradedCardPriceSetListGhostBlack = gameData.m_GradedCardPriceSetListGhostBlack;
				CPlayerData.m_GradedCardPriceSetListMegabot = gameData.m_GradedCardPriceSetListMegabot;
				CPlayerData.m_GradedCardPriceSetListFantasyRPG = gameData.m_GradedCardPriceSetListFantasyRPG;
				CPlayerData.m_GradedCardPriceSetListCatJob = gameData.m_GradedCardPriceSetListCatJob;
			}
			if (gameData.m_CardPriceSetList != null && gameData.m_CardPriceSetList.Count > 0)
			{
				CPlayerData.m_GenGradedCardPriceMultiplierList = gameData.m_GenGradedCardPriceMultiplierList;
			}
			if (gameData.m_GradeCardInProgressList != null && gameData.m_GradeCardInProgressList.Count > 0)
			{
				CPlayerData.m_GradeCardInProgressList = gameData.m_GradeCardInProgressList;
			}
			if (gameData.m_CurrentGradeCardSubmitSet != null && gameData.m_CurrentGradeCardSubmitSet.m_CardDataList != null && gameData.m_CurrentGradeCardSubmitSet.m_CardDataList.Count > 0)
			{
				CPlayerData.m_CurrentGradeCardSubmitSet = gameData.m_CurrentGradeCardSubmitSet;
			}
			if (gameData.m_ChampionCardCollectedList != null && gameData.m_ChampionCardCollectedList.Count > 0)
			{
				CPlayerData.m_ChampionCardCollectedList = gameData.m_ChampionCardCollectedList;
			}
			if (gameData.m_CollectionSortingMethodIndexList != null && gameData.m_CollectionSortingMethodIndexList.Count > 0)
			{
				CPlayerData.m_CollectionSortingMethodIndexList = gameData.m_CollectionSortingMethodIndexList;
			}
			if (gameData.m_IsItemLicenseUnlocked != null && gameData.m_IsItemLicenseUnlocked.Count > 0)
			{
				CPlayerData.m_IsItemLicenseUnlocked = gameData.m_IsItemLicenseUnlocked;
			}
			if (CPlayerData.m_IsItemLicenseUnlocked.Count < 500)
			{
				for (int num32 = 0; num32 < 500; num32++)
				{
					CPlayerData.m_IsItemLicenseUnlocked.Add(item: false);
				}
			}
			if (gameData.m_IsWorkerHired != null && gameData.m_IsWorkerHired.Count > 0)
			{
				CPlayerData.m_IsWorkerHired = gameData.m_IsWorkerHired;
			}
			if (gameData.m_IsAchievementUnlocked != null && gameData.m_IsAchievementUnlocked.Count > 0)
			{
				CPlayerData.m_IsAchievementUnlocked = gameData.m_IsAchievementUnlocked;
			}
			if (gameData.m_DecorationInventoryList != null && gameData.m_DecorationInventoryList.Count > 0)
			{
				CPlayerData.m_DecorationInventoryList = gameData.m_DecorationInventoryList;
			}
			if (gameData.m_UnlockedDecoWallList != null && gameData.m_UnlockedDecoWallList.Count > 0)
			{
				CPlayerData.m_UnlockedDecoWallList = gameData.m_UnlockedDecoWallList;
			}
			if (gameData.m_UnlockedDecoFloorList != null && gameData.m_UnlockedDecoFloorList.Count > 0)
			{
				CPlayerData.m_UnlockedDecoFloorList = gameData.m_UnlockedDecoFloorList;
			}
			if (gameData.m_UnlockedDecoCeilingList != null && gameData.m_UnlockedDecoCeilingList.Count > 0)
			{
				CPlayerData.m_UnlockedDecoCeilingList = gameData.m_UnlockedDecoCeilingList;
			}
			CPlayerData.m_TutorialIndex = gameData.m_TutorialIndex;
			CPlayerData.m_TutorialSubgroupIndex = gameData.m_TutorialSubgroupIndex;
			CPlayerData.m_HasFinishedTutorial = gameData.m_HasFinishedTutorial;
			CPlayerData.m_HasGetGhostCard = gameData.m_HasGetGhostCard;
			CPlayerData.m_MusicVolumeDecrease = gameData.m_MusicVolumeDecrease;
			CPlayerData.m_SoundVolumeDecrease = gameData.m_SoundVolumeDecrease;
			CPlayerData.m_CurrentSelectedDeckIndex = gameData.m_CurrentSelectedDeckIndex;
			CPlayerData.m_CustomerReviewCount = gameData.m_CustomerReviewCount;
			CPlayerData.m_CustomerReviewScoreAverage = gameData.m_CustomerReviewScoreAverage;
			CPlayerData.m_WorkbenchMinimumCardLimit = gameData.m_WorkbenchMinimumCardLimit;
			CPlayerData.m_WorkbenchPriceLimit = gameData.m_WorkbenchPriceLimit;
			CPlayerData.m_WorkbenchPriceMinimum = gameData.m_WorkbenchPriceMinimum;
			CPlayerData.m_WorkbenchRarityLimit = gameData.m_WorkbenchRarityLimit;
			CPlayerData.m_WorkbenchCardExpansionType = gameData.m_WorkbenchCardExpansionType;
			CPlayerData.m_RestockSortingType = gameData.m_RestockSortingType;
			CPlayerData.m_QuickFillMinimumCardLimit = gameData.m_QuickFillMinimumCardLimit;
			CPlayerData.m_QuickFillPriceLimit = gameData.m_QuickFillPriceLimit;
			CPlayerData.m_QuickFillPriceMinimum = gameData.m_QuickFillPriceMinimum;
			CPlayerData.m_QuickFillRarityLimit = gameData.m_QuickFillRarityLimit;
			CPlayerData.m_QuickFillCardExpansionType = gameData.m_QuickFillCardExpansionType;
			CPlayerData.m_DebugString = gameData.m_DebugString;
			CPlayerData.m_DebugString2 = gameData.m_DebugString2;
			CPlayerData.m_LastLoginPlayfabID = gameData.m_LastLoginPlayfabID;
			if (gameData.m_LightTimeData != null)
			{
				CPlayerData.m_LightTimeData = gameData.m_LightTimeData;
			}
			CPlayerData.m_GameEventFormat = gameData.m_GameEventFormat;
			CPlayerData.m_PendingGameEventFormat = gameData.m_PendingGameEventFormat;
			CPlayerData.m_GameEventExpansionType = gameData.m_GameEventExpansionType;
			CPlayerData.m_PendingGameEventExpansionType = gameData.m_PendingGameEventExpansionType;
			CPlayerData.m_GameReportDataCollect = gameData.m_GameReportDataCollect;
			CPlayerData.m_GameReportDataCollectPermanent = gameData.m_GameReportDataCollectPermanent;
			if (gameData.m_GameReportDataCollectPastList != null)
			{
				CPlayerData.m_GameReportDataCollectPastList = gameData.m_GameReportDataCollectPastList;
			}
			CPlayerData.m_ShelfSaveDataList = gameData.m_ShelfSaveDataList;
			CPlayerData.m_WarehouseShelfSaveDataList = gameData.m_WarehouseShelfSaveDataList;
			CPlayerData.m_PackageBoxItemSaveDataList = gameData.m_PackageBoxItemSaveDataList;
			if (gameData.m_PackageBoxCardSaveDataList != null)
			{
				CPlayerData.m_PackageBoxCardSaveDataList = gameData.m_PackageBoxCardSaveDataList;
			}
			CPlayerData.m_InteractableObjectSaveDataList = gameData.m_InteractableObjectSaveDataList;
			if (gameData.m_CardShelfSaveDataList != null)
			{
				CPlayerData.m_CardShelfSaveDataList = gameData.m_CardShelfSaveDataList;
			}
			if (gameData.m_PlayTableSaveDataList != null)
			{
				CPlayerData.m_PlayTableSaveDataList = gameData.m_PlayTableSaveDataList;
			}
			if (gameData.m_AutoCleanserSaveDataList != null)
			{
				CPlayerData.m_AutoCleanserSaveDataList = gameData.m_AutoCleanserSaveDataList;
			}
			if (gameData.m_AutoPackOpenerSaveDataList != null)
			{
				CPlayerData.m_AutoPackOpenerSaveDataList = gameData.m_AutoPackOpenerSaveDataList;
			}
			if (gameData.m_EmptyBoxStorageSaveDataList != null)
			{
				CPlayerData.m_EmptyBoxStorageSaveDataList = gameData.m_EmptyBoxStorageSaveDataList;
			}
			if (gameData.m_BulkDonationSaveDataList != null)
			{
				CPlayerData.m_BulkDonationSaveDataList = gameData.m_BulkDonationSaveDataList;
			}
			if (gameData.m_CardStorageShelfSaveDataList != null)
			{
				CPlayerData.m_CardStorageShelfSaveDataList = gameData.m_CardStorageShelfSaveDataList;
			}
			if (gameData.m_WorkbenchSaveDataList != null)
			{
				CPlayerData.m_WorkbenchSaveDataList = gameData.m_WorkbenchSaveDataList;
			}
			if (gameData.m_CashCounterSaveDataList != null)
			{
				CPlayerData.m_CashCounterSaveDataList = gameData.m_CashCounterSaveDataList;
			}
			if (gameData.m_DecoObjectSaveDataList != null)
			{
				CPlayerData.m_DecoObjectSaveDataList = gameData.m_DecoObjectSaveDataList;
			}
			if (gameData.m_CustomerSaveDataList != null)
			{
				CPlayerData.m_CustomerSaveDataList = gameData.m_CustomerSaveDataList;
			}
			if (gameData.m_WorkerSaveDataList != null)
			{
				CPlayerData.m_WorkerSaveDataList = gameData.m_WorkerSaveDataList;
			}
			if (gameData.m_TutorialDataList != null)
			{
				CPlayerData.m_TutorialDataList = gameData.m_TutorialDataList;
			}
			if (gameData.m_CustomerReviewDataList != null)
			{
				CPlayerData.m_CustomerReviewDataList = gameData.m_CustomerReviewDataList;
			}
			if (gameData.m_DeckCompactCardDataList != null)
			{
				CPlayerData.m_DeckCompactCardDataList = gameData.m_DeckCompactCardDataList;
			}
			if (gameData.m_BillList != null)
			{
				CPlayerData.m_BillList = gameData.m_BillList;
			}
			if (gameData.m_HoldCardDataList != null)
			{
				CPlayerData.m_HoldCardDataList = gameData.m_HoldCardDataList;
			}
			if (gameData.m_HoldItemTypeList != null)
			{
				CPlayerData.m_HoldItemTypeList = gameData.m_HoldItemTypeList;
			}
			if (gameData.m_TargetBuyItemList != null)
			{
				CPlayerData.m_TargetBuyItemList = gameData.m_TargetBuyItemList;
			}
			if (CPlayerData.m_CurrentTotalItemCountList.Count < 500)
			{
				for (int num33 = 0; num33 < 500; num33++)
				{
					CPlayerData.m_CurrentTotalItemCountList.Add(0);
					CPlayerData.m_SetItemPriceList.Add(0f);
					CPlayerData.m_AverageItemCostList.Add(0f);
					CPlayerData.m_GeneratedCostPriceList.Add(0f);
					CPlayerData.m_GeneratedMarketPriceList.Add(0f);
					CPlayerData.m_ItemPricePercentChangeList.Add(0f);
					FloatList floatList = new FloatList();
					floatList.floatDataList = new List<float>();
					CPlayerData.m_ItemPricePercentPastChangeList.Add(floatList);
					CPlayerData.m_StockSoldList.Add(0);
				}
			}
			CSingleton<ShelfManager>.Instance.LoadInteractableObjectData();
			GameInstance.m_HasLoadingError = false;
		}
		catch
		{
			Debug.LogError("Version : " + Application.version + "Error: Loading game data error");
			m_CanCloudLoad = true;
		}
	}

	public void SetLoadData<T>(ref T data, T loadData)
	{
		m_DebugDataCount++;
		if (loadData != null)
		{
			if (data == null)
			{
				Debug.Log("Null Ref Data Present SetLoadData" + m_DebugDataCount);
			}
			data = loadData;
		}
		else
		{
			Debug.Log("Null Data Present SetLoadData" + m_DebugDataCount);
		}
	}

	public void SaveGameData(int saveSlotIndex)
	{
		m_DebugDataCount = 0;
		Debug.Log("SaveGameData saveSlotIndex " + saveSlotIndex);
		SetLoadData(ref m_GameEventFormat, CPlayerData.m_GameEventFormat);
		SetLoadData(ref m_PendingGameEventFormat, CPlayerData.m_PendingGameEventFormat);
		SetLoadData(ref m_GameEventExpansionType, CPlayerData.m_GameEventExpansionType);
		SetLoadData(ref m_PendingGameEventExpansionType, CPlayerData.m_PendingGameEventExpansionType);
		SetLoadData(ref m_LightTimeData, CPlayerData.m_LightTimeData);
		SetLoadData(ref m_GameReportDataCollect, CPlayerData.m_GameReportDataCollect);
		SetLoadData(ref m_GameReportDataCollectPermanent, CPlayerData.m_GameReportDataCollectPermanent);
		SetLoadData(ref m_GameReportDataCollectPastList, CPlayerData.m_GameReportDataCollectPastList);
		SetLoadData(ref m_ShelfSaveDataList, CPlayerData.m_ShelfSaveDataList);
		SetLoadData(ref m_WarehouseShelfSaveDataList, CPlayerData.m_WarehouseShelfSaveDataList);
		SetLoadData(ref m_PackageBoxItemSaveDataList, CPlayerData.m_PackageBoxItemSaveDataList);
		SetLoadData(ref m_PackageBoxCardSaveDataList, CPlayerData.m_PackageBoxCardSaveDataList);
		SetLoadData(ref m_InteractableObjectSaveDataList, CPlayerData.m_InteractableObjectSaveDataList);
		SetLoadData(ref m_CardShelfSaveDataList, CPlayerData.m_CardShelfSaveDataList);
		SetLoadData(ref m_PlayTableSaveDataList, CPlayerData.m_PlayTableSaveDataList);
		SetLoadData(ref m_AutoCleanserSaveDataList, CPlayerData.m_AutoCleanserSaveDataList);
		SetLoadData(ref m_AutoPackOpenerSaveDataList, CPlayerData.m_AutoPackOpenerSaveDataList);
		SetLoadData(ref m_EmptyBoxStorageSaveDataList, CPlayerData.m_EmptyBoxStorageSaveDataList);
		SetLoadData(ref m_BulkDonationSaveDataList, CPlayerData.m_BulkDonationSaveDataList);
		SetLoadData(ref m_CardStorageShelfSaveDataList, CPlayerData.m_CardStorageShelfSaveDataList);
		SetLoadData(ref m_WorkbenchSaveDataList, CPlayerData.m_WorkbenchSaveDataList);
		SetLoadData(ref m_CashCounterSaveDataList, CPlayerData.m_CashCounterSaveDataList);
		SetLoadData(ref m_DecoObjectSaveDataList, CPlayerData.m_DecoObjectSaveDataList);
		SetLoadData(ref m_CustomerSaveDataList, CPlayerData.m_CustomerSaveDataList);
		SetLoadData(ref m_WorkerSaveDataList, CPlayerData.m_WorkerSaveDataList);
		SetLoadData(ref m_TutorialDataList, CPlayerData.m_TutorialDataList);
		SetLoadData(ref m_CustomerReviewDataList, CPlayerData.m_CustomerReviewDataList);
		SetLoadData(ref m_DeckCompactCardDataList, CPlayerData.m_DeckCompactCardDataList);
		SetLoadData(ref m_BillList, CPlayerData.m_BillList);
		SetLoadData(ref m_HoldCardDataList, CPlayerData.m_HoldCardDataList);
		SetLoadData(ref m_HoldItemTypeList, CPlayerData.m_HoldItemTypeList);
		SetLoadData(ref m_TargetBuyItemList, CPlayerData.m_TargetBuyItemList);
		CPlayerData.m_SaveIndex++;
		if (CPlayerData.m_SaveIndex >= 1000000000)
		{
			CPlayerData.m_SaveIndex = 0;
			CPlayerData.m_SaveCycle++;
		}
		SetLoadData(ref m_SaveIndex, CPlayerData.m_SaveIndex);
		SetLoadData(ref m_SaveCycle, CPlayerData.m_SaveCycle);
		SetLoadData(ref m_LastLoginPlayfabID, CPlayerData.m_LastLoginPlayfabID);
		SetLoadData(ref m_CanCloudLoad, CPlayerData.m_CanCloudLoad);
		SetLoadData(ref m_LastLocalExitTime, CPlayerData.m_LastLocalExitTime);
		SetLoadData(ref m_PlayerName, CPlayerData.PlayerName);
		SetLoadData(ref m_CoinAmount, CPlayerData.m_CoinAmount);
		SetLoadData(ref m_CoinAmountDouble, CPlayerData.m_CoinAmountDouble);
		SetLoadData(ref m_FamePoint, CPlayerData.m_FamePoint);
		SetLoadData(ref m_TotalFameAdd, CPlayerData.m_TotalFameAdd);
		SetLoadData(ref m_CloudSaveCountdown, CPlayerData.m_CloudSaveCountdown);
		SetLoadData(ref m_CurrentDay, CPlayerData.m_CurrentDay);
		SetLoadData(ref m_ShopExpPoint, CPlayerData.m_ShopExpPoint);
		SetLoadData(ref m_ShopLevel, CPlayerData.m_ShopLevel);
		SetLoadData(ref m_EquippedWallDecoIndex, CPlayerData.m_EquippedWallDecoIndex);
		SetLoadData(ref m_EquippedWallDecoIndexB, CPlayerData.m_EquippedWallDecoIndexB);
		SetLoadData(ref m_EquippedFloorDecoIndex, CPlayerData.m_EquippedFloorDecoIndex);
		SetLoadData(ref m_EquippedFloorDecoIndexB, CPlayerData.m_EquippedFloorDecoIndexB);
		SetLoadData(ref m_EquippedCeilingDecoIndex, CPlayerData.m_EquippedCeilingDecoIndex);
		SetLoadData(ref m_EquippedCeilingDecoIndexB, CPlayerData.m_EquippedCeilingDecoIndexB);
		SetLoadData(ref m_GradedCollectionSortingMethodIndex, CPlayerData.m_GradedCollectionSortingMethodIndex);
		SetLoadData(ref m_CurrentTotalItemCountList, CPlayerData.m_CurrentTotalItemCountList);
		SetLoadData(ref m_IsShopOpen, CPlayerData.m_IsShopOpen);
		SetLoadData(ref m_IsShopOnceOpen, CPlayerData.m_IsShopOnceOpen);
		SetLoadData(ref m_IsWarehouseDoorClosed, CPlayerData.m_IsWarehouseDoorClosed);
		SetLoadData(ref m_IsItemPriceGenerated, CPlayerData.m_IsItemPriceGenerated);
		SetLoadData(ref m_IsCardPriceGenerated, CPlayerData.m_IsCardPriceGenerated);
		SetLoadData(ref m_SetItemPriceList, CPlayerData.m_SetItemPriceList);
		SetLoadData(ref m_AverageItemCostList, CPlayerData.m_AverageItemCostList);
		SetLoadData(ref m_GeneratedCostPriceList, CPlayerData.m_GeneratedCostPriceList);
		SetLoadData(ref m_GeneratedMarketPriceList, CPlayerData.m_GeneratedMarketPriceList);
		SetLoadData(ref m_ItemPricePercentChangeList, CPlayerData.m_ItemPricePercentChangeList);
		SetLoadData(ref m_ItemPricePercentPastChangeList, CPlayerData.m_ItemPricePercentPastChangeList);
		SetLoadData(ref m_SetGameEventPriceList, CPlayerData.m_SetGameEventPriceList);
		SetLoadData(ref m_GeneratedGameEventPriceList, CPlayerData.m_GeneratedGameEventPriceList);
		SetLoadData(ref m_GameEventPricePercentChangeList, CPlayerData.m_GameEventPricePercentChangeList);
		SetLoadData(ref m_TransactionDataList, CPlayerData.m_TransactionDataList);
		SetLoadData(ref m_SpawnBoxRestockIndexWaitingList, CPlayerData.m_SpawnBoxRestockIndexWaitingList);
		SetLoadData(ref m_SpawnBoxItemCountWaitingList, CPlayerData.m_SpawnBoxItemCountWaitingList);
		SetLoadData(ref m_StockSoldList, CPlayerData.m_StockSoldList);
		SetLoadData(ref m_CollectionCardPackCountList, CPlayerData.m_CollectionCardPackCountList);
		SetLoadData(ref m_CardCollectedList, CPlayerData.m_CardCollectedList);
		SetLoadData(ref m_CardCollectedListDestiny, CPlayerData.m_CardCollectedListDestiny);
		SetLoadData(ref m_CardCollectedListGhost, CPlayerData.m_CardCollectedListGhost);
		SetLoadData(ref m_CardCollectedListGhostBlack, CPlayerData.m_CardCollectedListGhostBlack);
		SetLoadData(ref m_CardCollectedListMegabot, CPlayerData.m_CardCollectedListMegabot);
		SetLoadData(ref m_CardCollectedListFantasyRPG, CPlayerData.m_CardCollectedListFantasyRPG);
		SetLoadData(ref m_CardCollectedListCatJob, CPlayerData.m_CardCollectedListCatJob);
		SetLoadData(ref m_GradedCardInventoryList, CPlayerData.m_GradedCardInventoryList);
		SetLoadData(ref m_IsCardCollectedList, CPlayerData.m_IsCardCollectedList);
		SetLoadData(ref m_IsCardCollectedListDestiny, CPlayerData.m_IsCardCollectedListDestiny);
		SetLoadData(ref m_IsCardCollectedListGhost, CPlayerData.m_IsCardCollectedListGhost);
		SetLoadData(ref m_IsCardCollectedListGhostBlack, CPlayerData.m_IsCardCollectedListGhostBlack);
		SetLoadData(ref m_IsCardCollectedListMegabot, CPlayerData.m_IsCardCollectedListMegabot);
		SetLoadData(ref m_IsCardCollectedListFantasyRPG, CPlayerData.m_IsCardCollectedListFantasyRPG);
		SetLoadData(ref m_IsCardCollectedListCatJob, CPlayerData.m_IsCardCollectedListCatJob);
		SetLoadData(ref m_CardPriceSetList, CPlayerData.m_CardPriceSetList);
		SetLoadData(ref m_CardPriceSetListDestiny, CPlayerData.m_CardPriceSetListDestiny);
		SetLoadData(ref m_CardPriceSetListGhost, CPlayerData.m_CardPriceSetListGhost);
		SetLoadData(ref m_CardPriceSetListGhostBlack, CPlayerData.m_CardPriceSetListGhostBlack);
		SetLoadData(ref m_CardPriceSetListMegabot, CPlayerData.m_CardPriceSetListMegabot);
		SetLoadData(ref m_CardPriceSetListFantasyRPG, CPlayerData.m_CardPriceSetListFantasyRPG);
		SetLoadData(ref m_CardPriceSetListCatJob, CPlayerData.m_CardPriceSetListCatJob);
		SetLoadData(ref m_GradedCardPriceSetList, CPlayerData.m_GradedCardPriceSetList);
		SetLoadData(ref m_GradedCardPriceSetListDestiny, CPlayerData.m_GradedCardPriceSetListDestiny);
		SetLoadData(ref m_GradedCardPriceSetListGhost, CPlayerData.m_GradedCardPriceSetListGhost);
		SetLoadData(ref m_GradedCardPriceSetListGhostBlack, CPlayerData.m_GradedCardPriceSetListGhostBlack);
		SetLoadData(ref m_GradedCardPriceSetListMegabot, CPlayerData.m_GradedCardPriceSetListMegabot);
		SetLoadData(ref m_GradedCardPriceSetListFantasyRPG, CPlayerData.m_GradedCardPriceSetListFantasyRPG);
		SetLoadData(ref m_GradedCardPriceSetListCatJob, CPlayerData.m_GradedCardPriceSetListCatJob);
		SetLoadData(ref m_GenCardMarketPriceList, CPlayerData.m_GenCardMarketPriceList);
		SetLoadData(ref m_GenCardMarketPriceListDestiny, CPlayerData.m_GenCardMarketPriceListDestiny);
		SetLoadData(ref m_GenCardMarketPriceListGhost, CPlayerData.m_GenCardMarketPriceListGhost);
		SetLoadData(ref m_GenCardMarketPriceListGhostBlack, CPlayerData.m_GenCardMarketPriceListGhostBlack);
		SetLoadData(ref m_GenCardMarketPriceListMegabot, CPlayerData.m_GenCardMarketPriceListMegabot);
		SetLoadData(ref m_GenCardMarketPriceListFantasyRPG, CPlayerData.m_GenCardMarketPriceListFantasyRPG);
		SetLoadData(ref m_GenCardMarketPriceListCatJob, CPlayerData.m_GenCardMarketPriceListCatJob);
		SetLoadData(ref m_GenGradedCardPriceMultiplierList, CPlayerData.m_GenGradedCardPriceMultiplierList);
		SetLoadData(ref m_CurrentGradeCardSubmitSet, CPlayerData.m_CurrentGradeCardSubmitSet);
		SetLoadData(ref m_GradeCardInProgressList, CPlayerData.m_GradeCardInProgressList);
		SetLoadData(ref m_CollectionSortingMethodIndexList, CPlayerData.m_CollectionSortingMethodIndexList);
		SetLoadData(ref m_ChampionCardCollectedList, CPlayerData.m_ChampionCardCollectedList);
		SetLoadData(ref m_IsItemLicenseUnlocked, CPlayerData.m_IsItemLicenseUnlocked);
		SetLoadData(ref m_IsWorkerHired, CPlayerData.m_IsWorkerHired);
		SetLoadData(ref m_IsAchievementUnlocked, CPlayerData.m_IsAchievementUnlocked);
		SetLoadData(ref m_DecorationInventoryList, CPlayerData.m_DecorationInventoryList);
		SetLoadData(ref m_UnlockedDecoWallList, CPlayerData.m_UnlockedDecoWallList);
		SetLoadData(ref m_UnlockedDecoFloorList, CPlayerData.m_UnlockedDecoFloorList);
		SetLoadData(ref m_UnlockedDecoCeilingList, CPlayerData.m_UnlockedDecoCeilingList);
		SetLoadData(ref m_IsScannerRestockUnlocked, CPlayerData.m_IsScannerRestockUnlocked);
		SetLoadData(ref m_IsWarehouseRoomUnlocked, CPlayerData.m_IsWarehouseRoomUnlocked);
		SetLoadData(ref m_UnlockRoomCount, CPlayerData.m_UnlockRoomCount);
		SetLoadData(ref m_UnlockWarehouseRoomCount, CPlayerData.m_UnlockWarehouseRoomCount);
		SetLoadData(ref m_MusicVolumeDecrease, CPlayerData.m_MusicVolumeDecrease);
		SetLoadData(ref m_SoundVolumeDecrease, CPlayerData.m_SoundVolumeDecrease);
		SetLoadData(ref m_TutorialIndex, CPlayerData.m_TutorialIndex);
		SetLoadData(ref m_TutorialSubgroupIndex, CPlayerData.m_TutorialSubgroupIndex);
		SetLoadData(ref m_HasFinishedTutorial, CPlayerData.m_HasFinishedTutorial);
		SetLoadData(ref m_HasGetGhostCard, CPlayerData.m_HasGetGhostCard);
		SetLoadData(ref m_IsMainGame, CPlayerData.m_IsMainGame);
		SetLoadData(ref m_CurrentSelectedDeckIndex, CPlayerData.m_CurrentSelectedDeckIndex);
		SetLoadData(ref m_CustomerReviewCount, CPlayerData.m_CustomerReviewCount);
		SetLoadData(ref m_CustomerReviewScoreAverage, CPlayerData.m_CustomerReviewScoreAverage);
		SetLoadData(ref m_WorkbenchMinimumCardLimit, CPlayerData.m_WorkbenchMinimumCardLimit);
		SetLoadData(ref m_WorkbenchPriceLimit, CPlayerData.m_WorkbenchPriceLimit);
		SetLoadData(ref m_WorkbenchPriceMinimum, CPlayerData.m_WorkbenchPriceMinimum);
		SetLoadData(ref m_WorkbenchRarityLimit, CPlayerData.m_WorkbenchRarityLimit);
		SetLoadData(ref m_WorkbenchCardExpansionType, CPlayerData.m_WorkbenchCardExpansionType);
		SetLoadData(ref m_RestockSortingType, CPlayerData.m_RestockSortingType);
		SetLoadData(ref m_QuickFillMinimumCardLimit, CPlayerData.m_QuickFillMinimumCardLimit);
		SetLoadData(ref m_QuickFillPriceLimit, CPlayerData.m_QuickFillPriceLimit);
		SetLoadData(ref m_QuickFillPriceMinimum, CPlayerData.m_QuickFillPriceMinimum);
		SetLoadData(ref m_QuickFillRarityLimit, CPlayerData.m_QuickFillRarityLimit);
		SetLoadData(ref m_QuickFillCardExpansionType, CPlayerData.m_QuickFillCardExpansionType);
		SetLoadData(ref m_DebugString, CPlayerData.m_DebugString);
		SetLoadData(ref m_DebugString2, CPlayerData.m_DebugString2);
		Debug.Log("SetLoadData finish");
		CSaveLoad.Save(saveSlotIndex);
		Debug.Log("CSaveLoad.Save() finish");
	}

	public void MatchSaveIndex(CGameData gameData)
	{
		if (gameData != null)
		{
			m_SaveIndex = gameData.m_SaveIndex;
			m_SaveCycle = gameData.m_SaveCycle;
			m_LastLoginPlayfabID = gameData.m_LastLoginPlayfabID;
		}
	}
}
