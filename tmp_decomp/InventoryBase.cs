using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class InventoryBase : CSingleton<InventoryBase>
{
	public StockItemData_ScriptableObject m_StockItemData_SO;

	public MonsterData_ScriptableObject m_MonsterData_SO;

	public ShelfData_ScriptableObject m_ObjectData_SO;

	public Text_ScriptableObject m_TextSO;

	public static InventoryBase m_Instance;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public static InteractableObject GetSpawnInteractableObjectPrefab(EObjectType objType)
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_ObjectDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_ObjectDataList[i].objectType == objType)
			{
				return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_ObjectDataList[i].spawnPrefab;
			}
		}
		return null;
	}

	public static ObjectData GetObjectData(EObjectType objType)
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_ObjectDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_ObjectDataList[i].objectType == objType)
			{
				return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_ObjectDataList[i];
			}
		}
		return null;
	}

	public static InteractableObject GetSpawnDecoObjectPrefab(EDecoObject objType)
	{
		return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_DecoDataList[(int)objType].spawnPrefab;
	}

	public static FurniturePurchaseData GetFurniturePurchaseData(int index)
	{
		return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_FurniturePurchaseDataList[index];
	}

	public static FurniturePurchaseData GetFurniturePurchaseData(EObjectType objType)
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_ObjectDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_FurniturePurchaseDataList[i].objectType == objType)
			{
				return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_FurniturePurchaseDataList[i];
			}
		}
		return null;
	}

	public static DecoPurchaseData GetItemDecoPurchaseData(EDecoObject decoObject)
	{
		return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_DecoPurchaseDataList[(int)decoObject];
	}

	public static ShopDecoData GetFloorDecoData(int index)
	{
		if (index >= CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_FloorDecoDataList.Count)
		{
			index = 0;
		}
		return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_FloorDecoDataList[index];
	}

	public static ShopDecoData GetWallDecoData(int index)
	{
		if (index >= CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_WallDecoDataList.Count)
		{
			index = 0;
		}
		return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_WallDecoDataList[index];
	}

	public static ShopDecoData GetWallBarDecoData(int index)
	{
		if (index >= CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_WallBarDecoDataList.Count)
		{
			index = 0;
		}
		return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_WallBarDecoDataList[index];
	}

	public static ShopDecoData GetCeilingDecoData(int index)
	{
		if (index >= CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_CeilingDecoDataList.Count)
		{
			index = 0;
		}
		return CSingleton<InventoryBase>.Instance.m_ObjectData_SO.m_CeilingDecoDataList[index];
	}

	public static GameEventData GetGameEventData(EGameEventFormat gameEventFormat)
	{
		if (gameEventFormat == EGameEventFormat.None)
		{
			return new GameEventData();
		}
		return CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_GameEventDataList[(int)gameEventFormat];
	}

	public static ItemData GetItemData(EItemType itemType)
	{
		if (itemType == EItemType.None)
		{
			return new ItemData();
		}
		return CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[(int)itemType];
	}

	public static ItemMeshData GetItemMeshData(EItemType itemType)
	{
		if (itemType == EItemType.None)
		{
			return new ItemMeshData();
		}
		return CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemMeshDataList[(int)itemType];
	}

	public static EItemType GetRandomItemTypeFromCategory(EItemCategory category, bool unlockedOnly)
	{
		List<EItemType> list = new List<EItemType>();
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[i].category != category)
			{
				continue;
			}
			if (unlockedOnly)
			{
				List<int> restockDataIndexList = GetRestockDataIndexList((EItemType)i);
				if (restockDataIndexList.Count > 1)
				{
					if (CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[0]) || CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[1]))
					{
						list.Add((EItemType)i);
					}
				}
				else if (restockDataIndexList.Count > 0 && CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[0]))
				{
					list.Add((EItemType)i);
				}
			}
			else
			{
				list.Add((EItemType)i);
			}
		}
		if (list.Count == 0)
		{
			return EItemType.None;
		}
		return list[Random.Range(0, list.Count)];
	}

	public static List<EItemType> GetItemTypeListFromCategory(EItemCategory category, bool unlockedOnly)
	{
		List<EItemType> list = new List<EItemType>();
		EItemType eItemType = EItemType.None;
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[i].category != category)
			{
				continue;
			}
			eItemType = (EItemType)i;
			if (unlockedOnly)
			{
				List<int> restockDataIndexList = GetRestockDataIndexList(eItemType);
				if (restockDataIndexList.Count > 1)
				{
					if ((CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[0]) || CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[1])) && !list.Contains(eItemType))
					{
						list.Add(eItemType);
					}
				}
				else if (restockDataIndexList.Count > 0 && CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[0]) && !list.Contains(eItemType))
				{
					list.Add(eItemType);
				}
			}
			else if (!list.Contains(eItemType))
			{
				list.Add(eItemType);
			}
		}
		return list;
	}

	public static RestockData GetRestockData(int index)
	{
		return CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[index];
	}

	public static List<RestockData> GetRestockDataUsingItemType(EItemType itemType)
	{
		List<RestockData> list = new List<RestockData>();
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].itemType == itemType)
			{
				CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].index = i;
				list.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i]);
			}
		}
		return list;
	}

	public static int GetRestockDataIndex(EItemType itemType)
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].itemType == itemType)
			{
				return i;
			}
		}
		return -1;
	}

	public static List<int> GetRestockDataIndexList(EItemType itemType)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].itemType == itemType)
			{
				list.Add(i);
			}
		}
		return list;
	}

	public static int GetUnlockItemLevelRequired(EItemType itemType)
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].itemType == itemType)
			{
				return CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].licenseShopLevelRequired;
			}
		}
		return 0;
	}

	public static List<EItemType> GetUnlockableItemTypeAtShopLevel(int level)
	{
		List<EItemType> list = new List<EItemType>();
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Count; i++)
		{
			if (!CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].isHideItemUntilUnlocked && level >= CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].licenseShopLevelRequired)
			{
				list.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList[i].itemType);
			}
		}
		return list;
	}

	public static List<EMonsterType> GetShownMonsterList(ECardExpansionType expansionType)
	{
		switch (expansionType)
		{
		case ECardExpansionType.Tetramon:
		case ECardExpansionType.Destiny:
			return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_ShownMonsterList;
		case ECardExpansionType.Ghost:
			return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_ShownGhostMonsterList;
		case ECardExpansionType.Megabot:
			return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_ShownMegabotList;
		case ECardExpansionType.FantasyRPG:
			return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_ShownFantasyRPGList;
		case ECardExpansionType.CatJob:
			return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_ShownCatJobList;
		default:
			return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_ShownMonsterList;
		}
	}

	public static MonsterData GetMonsterData(EMonsterType monsterType)
	{
		if (monsterType == EMonsterType.None || (monsterType >= EMonsterType.MAX && monsterType < EMonsterType.Alpha))
		{
			return null;
		}
		if (monsterType < EMonsterType.MAX)
		{
			return GetMonsterDataMatchWithType(CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_DataList, monsterType);
		}
		if (monsterType > EMonsterType.MAX && monsterType < EMonsterType.MAX_MEGABOT)
		{
			return GetMonsterDataMatchWithType(CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_MegabotDataList, monsterType);
		}
		if (monsterType > EMonsterType.MAX_MEGABOT && monsterType < EMonsterType.MAX_FANTASYRPG)
		{
			return GetMonsterDataMatchWithType(CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_FantasyRPGDataList, monsterType);
		}
		if (monsterType > EMonsterType.MAX_FANTASYRPG && monsterType < EMonsterType.MAX_CATJOB)
		{
			return GetMonsterDataMatchWithType(CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CatJobDataList, monsterType);
		}
		return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_DataList[(int)monsterType];
	}

	private static MonsterData GetMonsterDataMatchWithType(List<MonsterData> listMonsterData, EMonsterType monsterType)
	{
		for (int i = 0; i < listMonsterData.Count; i++)
		{
			if (listMonsterData[i].MonsterType == monsterType)
			{
				return listMonsterData[i];
			}
		}
		return null;
	}

	public static Sprite GetAncientArtifactSprite(EMonsterType monsterType)
	{
		_ = 0;
		return null;
	}

	public static Sprite GetMonsterGhostIconSprite(EMonsterType monsterType)
	{
		return null;
	}

	public static Sprite GetTetramonIconSprite(EMonsterType monsterType)
	{
		if (monsterType == EMonsterType.None)
		{
			return null;
		}
		int index = (int)monsterType % CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_TetramonImageList.Count;
		return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_TetramonImageList[index];
	}

	public static Sprite GetSpecialCardImage(EMonsterType monsterType)
	{
		if (monsterType == EMonsterType.None)
		{
			return null;
		}
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_SpecialCardImageList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_SpecialCardImageList[i].MonsterType == monsterType)
			{
				return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_SpecialCardImageList[i].GetIcon(ECardExpansionType.None);
			}
		}
		return null;
	}

	public static CardUISetting GetCardUISetting(ECardExpansionType expansionType)
	{
		return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CardUISettingList[(int)expansionType];
	}

	public static CardUISetting GetCardUISettingWithBorder(ECardExpansionType expansionType, ECardBorderType borderType)
	{
		return CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CardUISettingList[(int)expansionType];
	}

	public static ECardExpansionType GetCardExpansionType(ECollectionPackType collectionPackType)
	{
		switch (collectionPackType)
		{
		case ECollectionPackType.BasicCardPack:
		case ECollectionPackType.RareCardPack:
		case ECollectionPackType.EpicCardPack:
		case ECollectionPackType.LegendaryCardPack:
			return ECardExpansionType.Tetramon;
		case ECollectionPackType.DestinyBasicCardPack:
		case ECollectionPackType.DestinyRareCardPack:
		case ECollectionPackType.DestinyEpicCardPack:
		case ECollectionPackType.DestinyLegendaryCardPack:
			return ECardExpansionType.Destiny;
		case ECollectionPackType.GhostPack:
			return ECardExpansionType.Ghost;
		case ECollectionPackType.MegabotPack:
			return ECardExpansionType.Megabot;
		case ECollectionPackType.FantasyRPGPack:
			return ECardExpansionType.FantasyRPG;
		case ECollectionPackType.CatJobPack:
			return ECardExpansionType.CatJob;
		default:
			return ECardExpansionType.None;
		}
	}

	public static ECollectionPackType ItemTypeToCollectionPackType(EItemType itemType)
	{
		switch (itemType)
		{
		case EItemType.BasicCardPack:
		case EItemType.BasicCardBox:
			return ECollectionPackType.BasicCardPack;
		case EItemType.RareCardPack:
		case EItemType.RareCardBox:
			return ECollectionPackType.RareCardPack;
		case EItemType.EpicCardPack:
		case EItemType.EpicCardBox:
			return ECollectionPackType.EpicCardPack;
		case EItemType.LegendaryCardPack:
		case EItemType.LegendaryCardBox:
			return ECollectionPackType.LegendaryCardPack;
		case EItemType.DestinyBasicCardPack:
		case EItemType.DestinyBasicCardBox:
			return ECollectionPackType.DestinyBasicCardPack;
		case EItemType.DestinyRareCardPack:
		case EItemType.DestinyRareCardBox:
			return ECollectionPackType.DestinyRareCardPack;
		case EItemType.DestinyEpicCardPack:
		case EItemType.DestinyEpicCardBox:
			return ECollectionPackType.DestinyEpicCardPack;
		case EItemType.DestinyLegendaryCardPack:
		case EItemType.DestinyLegendaryCardBox:
			return ECollectionPackType.DestinyLegendaryCardPack;
		case EItemType.GhostPack:
			return ECollectionPackType.GhostPack;
		case EItemType.MegabotPack:
			return ECollectionPackType.MegabotPack;
		case EItemType.FantasyRPGPack:
			return ECollectionPackType.FantasyRPGPack;
		case EItemType.CatJobPack:
			return ECollectionPackType.CatJobPack;
		default:
			return ECollectionPackType.None;
		}
	}

	public static string GetPriceChangeTypeText(EPriceChangeType priceChangeType, bool isIncrease)
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_TextSO.m_PriceChangeTypeTextList.Count; i++)
		{
			if (CSingleton<InventoryBase>.Instance.m_TextSO.m_PriceChangeTypeTextList[i].priceChangeType == priceChangeType)
			{
				return CSingleton<InventoryBase>.Instance.m_TextSO.m_PriceChangeTypeTextList[i].GetName(isIncrease);
			}
		}
		return LocalizationManager.GetTranslation("No effect on card pack price.");
	}

	public static string GetCardExpansionName(ECardExpansionType cardExpansion)
	{
		return LocalizationManager.GetTranslation(CSingleton<InventoryBase>.Instance.m_TextSO.m_CardExpansionNameList[(int)cardExpansion]);
	}

	public static Material GetCurrencyMaterial(EMoneyCurrencyType currencyType)
	{
		return CSingleton<InventoryBase>.Instance.m_TextSO.m_CurrencyMaterialList[(int)currencyType];
	}

	public static Sprite GetQuestionMarkSprite()
	{
		return CSingleton<InventoryBase>.Instance.m_TextSO.m_QuestionMarkImage;
	}
}
