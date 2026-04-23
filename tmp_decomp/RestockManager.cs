using System.Collections.Generic;
using UnityEngine;

public class RestockManager : CSingleton<RestockManager>
{
	public static RestockManager m_Instance;

	public InteractablePackagingBox_Item m_PackageBoxPrefab;

	public InteractablePackagingBox_Item m_PackageBoxSmallPrefab;

	public InteractablePackagingBox_Shelf m_PackageBoxShelfPrefab;

	public InteractablePackagingBox_Card m_PackageBoxCardPrefab;

	public Transform m_PackageBoxSpawnLocation;

	public List<Transform> m_PackageBoxSpawnLocationList;

	public Transform m_PackageBoxParentGrp;

	public List<InteractablePackagingBox_Item> m_ItemPackagingBoxList = new List<InteractablePackagingBox_Item>();

	public List<InteractablePackagingBox_Shelf> m_ShelfPackagingBoxList = new List<InteractablePackagingBox_Shelf>();

	public List<InteractablePackagingBox_Card> m_CardPackagingBoxList = new List<InteractablePackagingBox_Card>();

	private List<RestockData> m_SpawnBoxItemWaitingList = new List<RestockData>();

	private float m_SpawnBoxItemTimer;

	private float m_SpawnBoxItemTime = 0.02f;

	private float m_OutofBoundCheckTimer;

	private int m_SpawnedBoxCount;

	private List<InteractablePackagingBox_Item> m_PackageBoxDelayedResetOpenCloseList = new List<InteractablePackagingBox_Item>();

	private List<float> m_DelayedResetOpenCloseTimer = new List<float>();

	public List<float> m_GeneratedCostPriceList = new List<float>();

	public List<float> m_GeneratedMarketPriceList = new List<float>();

	public List<float> m_ItemPricePercentChangeList = new List<float>();

	public List<FloatList> m_ItemPricePercentPastChangeList = new List<FloatList>();

	public List<MarketPrice> m_GenCardMarketPriceList = new List<MarketPrice>();

	public List<float> m_GenGradedCardPriceMultiplierList = new List<float>();

	private void Awake()
	{
	}

	public void AddToDelayedResetOpenCloseList(InteractablePackagingBox_Item packageBox)
	{
		m_PackageBoxDelayedResetOpenCloseList.Add(packageBox);
		m_DelayedResetOpenCloseTimer.Add(0f);
	}

	private void Update()
	{
		if (m_PackageBoxDelayedResetOpenCloseList.Count > 0)
		{
			m_DelayedResetOpenCloseTimer[0] += Time.deltaTime;
			if (m_DelayedResetOpenCloseTimer[0] > 0.85f)
			{
				if (m_PackageBoxDelayedResetOpenCloseList[0] != null)
				{
					m_PackageBoxDelayedResetOpenCloseList[0].ResetToggleOpenClose();
				}
				m_PackageBoxDelayedResetOpenCloseList.RemoveAt(0);
				m_DelayedResetOpenCloseTimer.RemoveAt(0);
			}
		}
		if (CSingleton<RestockManager>.Instance.m_SpawnBoxItemWaitingList.Count > 0 && m_ItemPackagingBoxList.Count < CSingleton<CGameManager>.Instance.m_RestockSpawnBoxLimit)
		{
			m_SpawnBoxItemTimer += Time.deltaTime;
			if (m_SpawnBoxItemTimer >= m_SpawnBoxItemTime)
			{
				if (m_SpawnBoxItemWaitingList[0].isBigBox)
				{
					SpawnPackageBoxItem(m_SpawnBoxItemWaitingList[0].itemType, 64, m_SpawnBoxItemWaitingList[0].isBigBox);
				}
				else
				{
					SpawnPackageBoxItem(m_SpawnBoxItemWaitingList[0].itemType, 32, m_SpawnBoxItemWaitingList[0].isBigBox);
				}
				CPlayerData.m_SpawnBoxItemCountWaitingList[0]--;
				if (CPlayerData.m_SpawnBoxItemCountWaitingList[0] <= 0)
				{
					CPlayerData.m_SpawnBoxRestockIndexWaitingList.RemoveAt(0);
					CPlayerData.m_SpawnBoxItemCountWaitingList.RemoveAt(0);
					m_SpawnBoxItemWaitingList.RemoveAt(0);
				}
				m_SpawnBoxItemTimer = 0f;
			}
		}
		m_OutofBoundCheckTimer += Time.deltaTime;
		if (!(m_OutofBoundCheckTimer > 5f))
		{
			return;
		}
		m_OutofBoundCheckTimer = 0f;
		for (int i = 0; i < m_ItemPackagingBoxList.Count; i++)
		{
			if (m_ItemPackagingBoxList[i].transform.position.y > 8f || m_ItemPackagingBoxList[i].transform.position.y < -1f || m_ItemPackagingBoxList[i].transform.position.x < -8.1f || m_ItemPackagingBoxList[i].transform.position.x > 17.5f)
			{
				m_ItemPackagingBoxList[i].transform.position = CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList[Random.Range(0, CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList.Count)].position;
			}
			if (!CPlayerData.m_IsWarehouseRoomUnlocked && m_ItemPackagingBoxList[i].transform.position.x < 3.5f && m_ItemPackagingBoxList[i].transform.position.z > -6.6f && m_ItemPackagingBoxList[i].transform.position.z < 1f)
			{
				m_ItemPackagingBoxList[i].transform.position = CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList[Random.Range(0, CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList.Count)].position;
			}
		}
		for (int j = 0; j < m_ShelfPackagingBoxList.Count; j++)
		{
			if (m_ShelfPackagingBoxList[j].transform.position.y > 8f || m_ShelfPackagingBoxList[j].transform.position.y < -1f || m_ShelfPackagingBoxList[j].transform.position.x < -8.1f || m_ShelfPackagingBoxList[j].transform.position.x > 17.5f)
			{
				m_ShelfPackagingBoxList[j].transform.position = CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList[Random.Range(0, CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList.Count)].position;
			}
			if (!CPlayerData.m_IsWarehouseRoomUnlocked && m_ShelfPackagingBoxList[j].transform.position.x < 3.5f && m_ShelfPackagingBoxList[j].transform.position.z > -6.5f && m_ShelfPackagingBoxList[j].transform.position.z < 1f)
			{
				m_ShelfPackagingBoxList[j].transform.position = CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList[Random.Range(0, CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList.Count)].position;
			}
		}
	}

	private void Init()
	{
		CSingleton<RestockManager>.Instance.m_SpawnBoxItemWaitingList.Clear();
		for (int i = 0; i < CPlayerData.m_SpawnBoxRestockIndexWaitingList.Count; i++)
		{
			CSingleton<RestockManager>.Instance.m_SpawnBoxItemWaitingList.Add(InventoryBase.GetRestockData(CPlayerData.m_SpawnBoxRestockIndexWaitingList[i]));
		}
		for (int j = 0; j < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList.Count; j++)
		{
			if (CPlayerData.m_GeneratedCostPriceList[j] == 0f)
			{
				CPlayerData.m_GeneratedCostPriceList[j] = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[j].baseCost * Mathf.Clamp(Random.Range(0.9f, 1.3f), 1f, 1.3f);
				if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[j].boxFollowItemPrice != EItemType.None)
				{
					CPlayerData.m_GeneratedCostPriceList[j] = CPlayerData.GetItemCost(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[j].boxFollowItemPrice) * 8f * Mathf.Clamp(Random.Range(0.9f, 1.2f), 1f, 1.15f);
				}
				float num = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[j].marketPriceMinPercent;
				float maxInclusive = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[j].marketPriceMaxPercent;
				if (num == 0f)
				{
					num = 1.5f;
					maxInclusive = 3f;
				}
				if (CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[j].boxFollowItemPrice != EItemType.None)
				{
					CPlayerData.m_GeneratedMarketPriceList[j] = CPlayerData.GetItemMarketPrice(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[j].boxFollowItemPrice) * 8f * Random.Range(num, maxInclusive);
				}
				else
				{
					CPlayerData.m_GeneratedMarketPriceList[j] = CPlayerData.m_GeneratedCostPriceList[j] * Random.Range(num, maxInclusive);
				}
			}
		}
		if (!CPlayerData.m_IsItemPriceGenerated)
		{
			CPlayerData.UpdateItemPricePercentChange();
		}
		CPlayerData.m_IsItemPriceGenerated = true;
		GenerateCardMarketPrice(ECardExpansionType.Tetramon);
		GenerateCardMarketPrice(ECardExpansionType.Destiny);
		GenerateCardMarketPrice(ECardExpansionType.Ghost);
		GenerateCardMarketPrice(ECardExpansionType.FantasyRPG);
		GenerateCardMarketPrice(ECardExpansionType.Megabot);
		GenerateCardMarketPrice(ECardExpansionType.CatJob);
		if (!CPlayerData.m_IsCardPriceGenerated)
		{
			CPlayerData.UpdatePastCardPricePercentChange();
			CPlayerData.m_IsCardPriceGenerated = true;
		}
		if (CPlayerData.m_GenGradedCardPriceMultiplierList == null)
		{
			CPlayerData.m_GenGradedCardPriceMultiplierList = new List<float>();
		}
		if (CPlayerData.m_GenGradedCardPriceMultiplierList.Count < 1000)
		{
			CPlayerData.m_GenGradedCardPriceMultiplierList.Clear();
			for (int k = 0; k < 2000; k++)
			{
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(0.01f, 0.15f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(0.15f, 0.25f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(0.25f, 0.35f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(0.35f, 0.45f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(0.45f, 0.65f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(0.65f, 1f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(1f, 1.25f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(1.25f, 1.5f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(1.5f, 3f));
				CPlayerData.m_GenGradedCardPriceMultiplierList.Add(Random.Range(3f, 7f));
			}
		}
		m_GeneratedCostPriceList = CPlayerData.m_GeneratedCostPriceList;
		m_GeneratedMarketPriceList = CPlayerData.m_GeneratedMarketPriceList;
		m_ItemPricePercentChangeList = CPlayerData.m_ItemPricePercentChangeList;
		m_ItemPricePercentPastChangeList = CPlayerData.m_ItemPricePercentPastChangeList;
		m_GenCardMarketPriceList = CPlayerData.m_GenCardMarketPriceList;
		m_GenGradedCardPriceMultiplierList = CPlayerData.m_GenGradedCardPriceMultiplierList;
	}

	private void GenerateCardMarketPrice(ECardExpansionType expansionType)
	{
		CardData cardData = new CardData();
		int num = InventoryBase.GetShownMonsterList(expansionType).Count * CPlayerData.GetCardAmountPerMonsterType(expansionType);
		if (expansionType == ECardExpansionType.Ghost)
		{
			num *= 2;
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = i;
			bool isDestiny = false;
			if (expansionType == ECardExpansionType.Ghost && num2 >= InventoryBase.GetShownMonsterList(expansionType).Count * CPlayerData.GetCardAmountPerMonsterType(expansionType))
			{
				isDestiny = true;
				num2 -= InventoryBase.GetShownMonsterList(expansionType).Count * CPlayerData.GetCardAmountPerMonsterType(expansionType);
			}
			ECardBorderType borderType = (ECardBorderType)(i % CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false));
			EMonsterType monsterTypeFromCardSaveIndex = CPlayerData.GetMonsterTypeFromCardSaveIndex(num2, expansionType);
			ERarity rarity = InventoryBase.GetMonsterData(monsterTypeFromCardSaveIndex).Rarity;
			bool flag = i % CPlayerData.GetCardAmountPerMonsterType(expansionType) >= CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false);
			float num3 = 1f;
			cardData.monsterType = monsterTypeFromCardSaveIndex;
			cardData.borderType = borderType;
			cardData.isFoil = flag;
			cardData.isDestiny = isDestiny;
			cardData.expansionType = expansionType;
			if (CPlayerData.GetCardMarketPrice(cardData) != 0f)
			{
				continue;
			}
			borderType = cardData.GetCardBorderType();
			float num4 = 0f;
			switch (rarity)
			{
			case ERarity.Legendary:
				num4 = 0.5f;
				break;
			case ERarity.Epic:
				num4 = 0.2f;
				break;
			case ERarity.Rare:
				num4 = 0.1f;
				break;
			}
			num3 = (flag ? (borderType switch
			{
				ECardBorderType.FullArt => 1500f * Random.Range(0.65f + num4, 1.4f + num4), 
				ECardBorderType.EX => 500f * Random.Range(0.6f + num4, 1.3f + num4), 
				ECardBorderType.Gold => 100f * Random.Range(0.5f + num4, 1.2f + num4), 
				ECardBorderType.Silver => 30f * Random.Range(0.4f + num4, 1.2f + num4), 
				ECardBorderType.FirstEdition => 10f * Random.Range(0.3f + num4, 1.2f + num4), 
				_ => 5f * Random.Range(0.2f + num4, 1.2f + num4), 
			}) : (borderType switch
			{
				ECardBorderType.FullArt => 50f * Random.Range(0.6f + num4, 1.4f + num4), 
				ECardBorderType.EX => 15f * Random.Range(0.5f + num4, 1.3f + num4), 
				ECardBorderType.Gold => 5f * Random.Range(0.4f + num4, 1.2f + num4), 
				ECardBorderType.Silver => 2.5f * Random.Range(0.3f + num4, 1.2f + num4), 
				ECardBorderType.FirstEdition => 1f * Random.Range(0.2f + num4, 1.2f + num4), 
				_ => 0.2f * Random.Range(0.1f + num4, 1.2f + num4), 
			}));
			switch (rarity)
			{
			case ERarity.Legendary:
				if (borderType >= ECardBorderType.Gold)
				{
					num3 *= Random.Range(1.5f, 2f);
				}
				break;
			case ERarity.Epic:
				if (borderType >= ECardBorderType.Gold)
				{
					num3 *= Random.Range(1.25f, 1.5f);
				}
				break;
			case ERarity.Rare:
				if (borderType >= ECardBorderType.Gold)
				{
					num3 *= Random.Range(1f, 1.25f);
				}
				break;
			}
			switch (expansionType)
			{
			case ECardExpansionType.Destiny:
				num3 *= Random.Range(1.2f, 1.5f);
				break;
			case ECardExpansionType.Ghost:
				num3 *= Random.Range(2.8f, 3.2f);
				break;
			}
			switch (rarity)
			{
			case ERarity.Legendary:
				num3 += 0.5f * (float)(borderType + 1) * Random.Range(0.9f, 1.1f);
				break;
			case ERarity.Epic:
				num3 += 0.3f * (float)(borderType + 1) * Random.Range(0.7f, 1.3f);
				break;
			case ERarity.Rare:
				num3 += 0.1f * (float)(borderType + 1) * Random.Range(0.5f, 1.5f);
				break;
			}
			CPlayerData.SetCardGeneratedMarketPrice(num2, expansionType, isDestiny, num3);
		}
	}

	public static float GetItemCost(EItemType itemType)
	{
		return (float)Mathf.RoundToInt((CPlayerData.m_GeneratedCostPriceList[(int)itemType] + CPlayerData.m_GeneratedCostPriceList[(int)itemType] * (CPlayerData.m_ItemPricePercentChangeList[(int)itemType] / 100f)) * 100f) / 100f;
	}

	public static float GetItemMarketPrice(EItemType itemType)
	{
		return (float)Mathf.RoundToInt((CPlayerData.m_GeneratedMarketPriceList[(int)itemType] + CPlayerData.m_GeneratedMarketPriceList[(int)itemType] * (CPlayerData.m_ItemPricePercentChangeList[(int)itemType] / 100f)) * 100f) / 100f;
	}

	public static float GetItemMarketPriceCustomPercent(EItemType itemType, float percent)
	{
		return (float)Mathf.RoundToInt((CPlayerData.m_GeneratedMarketPriceList[(int)itemType] + CPlayerData.m_GeneratedMarketPriceList[(int)itemType] * (percent / 100f)) * 100f) / 100f;
	}

	public static void SpawnPackageBoxItemMultipleFrame(int restockIndex, int count)
	{
		CSingleton<RestockManager>.Instance.m_SpawnBoxItemWaitingList.Add(InventoryBase.GetRestockData(restockIndex));
		CPlayerData.m_SpawnBoxRestockIndexWaitingList.Add(restockIndex);
		CPlayerData.m_SpawnBoxItemCountWaitingList.Add(count);
		if (CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList.Count >= CSingleton<CGameManager>.Instance.m_RestockSpawnBoxLimit)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.TooManyBoxClearBoxFirst);
		}
	}

	public static InteractablePackagingBox_Item SpawnPackageBoxItem(EItemType itemType, int amount, bool isBigBox)
	{
		Transform obj = CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList[Random.Range(0, CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList.Count)];
		Vector3 position = obj.position;
		Quaternion rotation = obj.rotation;
		if (isBigBox)
		{
			InteractablePackagingBox_Item interactablePackagingBox_Item = Object.Instantiate(CSingleton<RestockManager>.Instance.m_PackageBoxPrefab, position, rotation, CSingleton<RestockManager>.Instance.m_PackageBoxParentGrp);
			interactablePackagingBox_Item.FillBoxWithItem(itemType, amount);
			interactablePackagingBox_Item.name = interactablePackagingBox_Item.m_ObjectType.ToString() + CSingleton<RestockManager>.Instance.m_SpawnedBoxCount;
			CSingleton<RestockManager>.Instance.m_SpawnedBoxCount = CSingleton<RestockManager>.Instance.m_SpawnedBoxCount + 1;
			return interactablePackagingBox_Item;
		}
		InteractablePackagingBox_Item interactablePackagingBox_Item2 = Object.Instantiate(CSingleton<RestockManager>.Instance.m_PackageBoxSmallPrefab, position, rotation, CSingleton<RestockManager>.Instance.m_PackageBoxParentGrp);
		interactablePackagingBox_Item2.FillBoxWithItem(itemType, amount);
		interactablePackagingBox_Item2.name = interactablePackagingBox_Item2.m_ObjectType.ToString() + CSingleton<RestockManager>.Instance.m_SpawnedBoxCount;
		CSingleton<RestockManager>.Instance.m_SpawnedBoxCount = CSingleton<RestockManager>.Instance.m_SpawnedBoxCount + 1;
		return interactablePackagingBox_Item2;
	}

	public static InteractablePackagingBox_Shelf SpawnPackageBoxShelf(InteractableObject obj, bool holdBox)
	{
		Vector3 position = obj.transform.position;
		Quaternion rotation = obj.transform.rotation;
		InteractablePackagingBox_Shelf interactablePackagingBox_Shelf = Object.Instantiate(CSingleton<RestockManager>.Instance.m_PackageBoxShelfPrefab, position, rotation, CSingleton<RestockManager>.Instance.m_PackageBoxParentGrp);
		interactablePackagingBox_Shelf.ExecuteBoxUpObject(obj, holdBox);
		interactablePackagingBox_Shelf.name = interactablePackagingBox_Shelf.m_ObjectType.ToString() + CSingleton<RestockManager>.Instance.m_SpawnedBoxCount;
		CSingleton<RestockManager>.Instance.m_SpawnedBoxCount = CSingleton<RestockManager>.Instance.m_SpawnedBoxCount + 1;
		return interactablePackagingBox_Shelf;
	}

	public static InteractablePackagingBox_Card SpawnPackageBoxCard(List<CardData> cardDataList, Transform spawnLocation)
	{
		Vector3 position = spawnLocation.position;
		Quaternion rotation = spawnLocation.rotation;
		InteractablePackagingBox_Card interactablePackagingBox_Card = Object.Instantiate(CSingleton<RestockManager>.Instance.m_PackageBoxCardPrefab, position, rotation, CSingleton<RestockManager>.Instance.m_PackageBoxParentGrp);
		interactablePackagingBox_Card.UpdateCardData(cardDataList);
		interactablePackagingBox_Card.name = interactablePackagingBox_Card.m_ObjectType.ToString() + CSingleton<RestockManager>.Instance.m_SpawnedBoxCount;
		CSingleton<RestockManager>.Instance.m_SpawnedBoxCount = CSingleton<RestockManager>.Instance.m_SpawnedBoxCount + 1;
		return interactablePackagingBox_Card;
	}

	public static void InitItemPackageBox(InteractablePackagingBox_Item itemPackagingBox)
	{
		CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList.Add(itemPackagingBox);
	}

	public static void RemoveItemPackageBox(InteractablePackagingBox_Item itemPackagingBox)
	{
		CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList.Remove(itemPackagingBox);
	}

	public static List<InteractablePackagingBox_Item> GetItemPackagingBoxList()
	{
		return CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList;
	}

	public static InteractablePackagingBox_Item GetItemPackagingBoxWithItem()
	{
		for (int i = 0; i < CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList.Count; i++)
		{
			if (CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].IsValidObject() && CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].m_ItemCompartment.GetItemCount() > 0)
			{
				return CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i];
			}
		}
		return null;
	}

	public static List<InteractablePackagingBox_Item> GetItemPackagingBoxListWithItem(bool includeStoredItem)
	{
		List<InteractablePackagingBox_Item> list = new List<InteractablePackagingBox_Item>();
		for (int i = 0; i < CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList.Count; i++)
		{
			if ((includeStoredItem || !CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].m_IsStored) && CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].IsValidObject() && CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].m_ItemCompartment.GetItemCount() > 0)
			{
				list.Add(CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i]);
			}
		}
		return list;
	}

	public static List<InteractablePackagingBox_Item> GetItemPackagingBoxListWithSpaceForItem(EItemType itemType)
	{
		List<InteractablePackagingBox_Item> list = new List<InteractablePackagingBox_Item>();
		for (int i = 0; i < CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList.Count; i++)
		{
			if (!CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].m_IsStored && (CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].GetItemType() == EItemType.None || CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].m_ItemCompartment.GetItemCount() <= 0 || CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].GetItemType() == itemType) && CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].IsValidObject() && (CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].GetItemType() == EItemType.None || CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i].m_ItemCompartment.HasEnoughSlot()))
			{
				list.Add(CSingleton<RestockManager>.Instance.m_ItemPackagingBoxList[i]);
			}
		}
		return list;
	}

	public static void InitShelfPackageBox(InteractablePackagingBox_Shelf shelfPackagingBox)
	{
		CSingleton<RestockManager>.Instance.m_ShelfPackagingBoxList.Add(shelfPackagingBox);
	}

	public static void RemoveShelfPackageBox(InteractablePackagingBox_Shelf shelfPackagingBox)
	{
		CSingleton<RestockManager>.Instance.m_ShelfPackagingBoxList.Remove(shelfPackagingBox);
	}

	public static List<InteractablePackagingBox_Shelf> GetShelfPackagingBoxList()
	{
		return CSingleton<RestockManager>.Instance.m_ShelfPackagingBoxList;
	}

	public static void InitCardPackageBox(InteractablePackagingBox_Card cardPackagingBox)
	{
		CSingleton<RestockManager>.Instance.m_CardPackagingBoxList.Add(cardPackagingBox);
	}

	public static void RemoveCardPackageBox(InteractablePackagingBox_Card cardPackagingBox)
	{
		CSingleton<RestockManager>.Instance.m_CardPackagingBoxList.Remove(cardPackagingBox);
	}

	public static List<InteractablePackagingBox_Card> GetCardPackagingBoxList()
	{
		return CSingleton<RestockManager>.Instance.m_CardPackagingBoxList;
	}

	public static void DestroyAllObject()
	{
		CSingleton<RestockManager>.Instance.m_SpawnedBoxCount = 0;
		for (int num = GetItemPackagingBoxList().Count - 1; num >= 0; num--)
		{
			GetItemPackagingBoxList()[num].OnDestroyed();
		}
		for (int num2 = GetShelfPackagingBoxList().Count - 1; num2 >= 0; num2--)
		{
			GetShelfPackagingBoxList()[num2].OnDestroyed();
		}
		for (int num3 = GetCardPackagingBoxList().Count - 1; num3 >= 0; num3--)
		{
			GetCardPackagingBoxList()[num3].OnDestroyed();
		}
	}

	public static int GetMaxItemCountInBox(EItemType itemType, bool isBigBox)
	{
		ItemData itemData = InventoryBase.GetItemData(itemType);
		float num = 8f;
		float num2 = 1f;
		if (isBigBox)
		{
			num2 = 2f;
		}
		int num3 = Mathf.RoundToInt(4f / itemData.itemDimension.x);
		int num4 = Mathf.RoundToInt(num / itemData.itemDimension.y);
		int num5 = Mathf.RoundToInt(num2 / itemData.itemDimension.z);
		if (itemData.isTallItem)
		{
			num5 = Mathf.RoundToInt(num2 / (itemData.itemDimension.z * 2f));
		}
		return num3 * num4 * num5;
	}

	public static Transform GetRandomPackageSpawnPos()
	{
		return CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList[Random.Range(0, CSingleton<RestockManager>.Instance.m_PackageBoxSpawnLocationList.Count)];
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		for (int num = CPlayerData.m_GradeCardInProgressList.Count - 1; num >= 0; num--)
		{
			if (CPlayerData.m_GradeCardInProgressList[num].m_MinutePassed >= 540f)
			{
				GradeCardServiceData gradeCardServiceData = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.GetGradeCardServiceData(CPlayerData.m_GradeCardInProgressList[num].m_ServiceLevel);
				CPlayerData.m_GradeCardInProgressList[num].m_DayPassed++;
				if (CPlayerData.m_GradeCardInProgressList[num].m_DayPassed >= gradeCardServiceData.m_ServiceDays)
				{
					GradeCardSubmitSet gradeCardSubmitSet = CPlayerData.m_GradeCardInProgressList[num];
					for (int i = 0; i < gradeCardSubmitSet.m_CardDataList.Count; i++)
					{
						if (gradeCardSubmitSet.m_CardDataList[i].cardGrade > 0)
						{
							int num2 = Random.Range(0, 100);
							if (num2 < 30)
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade++;
								if (gradeCardSubmitSet.m_CardDataList[i].cardGrade > 10)
								{
									gradeCardSubmitSet.m_CardDataList[i].cardGrade = 10;
								}
							}
							else if (num2 > 70)
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade--;
								if (gradeCardSubmitSet.m_CardDataList[i].cardGrade < 1)
								{
									gradeCardSubmitSet.m_CardDataList[i].cardGrade = 1;
								}
							}
						}
						else if (Random.Range(0, 100) < 96)
						{
							if (Random.Range(0, 100) < 40)
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade = 10;
							}
							else if (Random.Range(0, 100) < 40)
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade = 9;
							}
							else if (Random.Range(0, 100) < 50)
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade = 8;
							}
							else if (Random.Range(0, 100) < 50)
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade = 7;
							}
							else if (Random.Range(0, 100) < 50)
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade = 6;
							}
							else
							{
								gradeCardSubmitSet.m_CardDataList[i].cardGrade = 5;
							}
						}
						else if (Random.Range(0, 100) < 40)
						{
							gradeCardSubmitSet.m_CardDataList[i].cardGrade = 5;
						}
						else if (Random.Range(0, 100) < 50)
						{
							gradeCardSubmitSet.m_CardDataList[i].cardGrade = 4;
						}
						else if (Random.Range(0, 100) < 50)
						{
							gradeCardSubmitSet.m_CardDataList[i].cardGrade = 3;
						}
						else if (Random.Range(0, 100) < 50)
						{
							gradeCardSubmitSet.m_CardDataList[i].cardGrade = 2;
						}
						else
						{
							gradeCardSubmitSet.m_CardDataList[i].cardGrade = 1;
						}
					}
					SpawnPackageBoxCard(gradeCardSubmitSet.m_CardDataList, GetRandomPackageSpawnPos());
					CPlayerData.m_GradeCardInProgressList.RemoveAt(num);
				}
			}
		}
	}
}
