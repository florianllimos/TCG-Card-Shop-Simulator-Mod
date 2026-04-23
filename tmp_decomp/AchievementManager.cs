using System.Collections.Generic;
using GA;
using UnityEngine;

public class AchievementManager : CSingleton<AchievementManager>
{
	public static AchievementManager m_Instance;

	public List<string> m_AchievementSringList;

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(this);
	}

	public static void OnItemLicenseUnlocked(EItemType itemType)
	{
		switch (itemType)
		{
		case EItemType.BasicCardBox:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockBasicCardBox");
			break;
		case EItemType.RareCardPack:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockRareCardPack");
			break;
		case EItemType.EpicCardPack:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockEpicCardPack");
			break;
		case EItemType.LegendaryCardPack:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockLegendCardPack");
			break;
		case EItemType.DestinyBasicCardPack:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockDestinyBasicPack");
			break;
		case EItemType.DestinyRareCardPack:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockDestinyRarePack");
			break;
		case EItemType.DestinyEpicCardPack:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockDestinyEpicPack");
			break;
		case EItemType.DestinyLegendaryCardPack:
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockDestinyLegendPack");
			break;
		}
	}

	public static void OnStaffHired(int count)
	{
		if (count >= 8)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_Worker3");
		}
		if (count >= 4)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_Worker2");
		}
		if (count >= 1)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_Worker1");
		}
	}

	public static void OnCardPackOpened(int count)
	{
		if (count >= 100)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_OpenPack1");
		}
		if (count >= 1000)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_OpenPack2");
		}
		if (count >= 10000)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_OpenPack3");
		}
	}

	public static void OnCustomerFinishPlay(int count)
	{
		if (count >= 100)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_CustomerPlay1");
		}
		if (count >= 1500)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_CustomerPlay2");
		}
		if (count >= 5000)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_CustomerPlay3");
		}
	}

	public static void OnCustomerFinishCheckout(int count)
	{
		if (count >= 1000)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_Checkout");
		}
	}

	public static void OnSoldCardPrice(float priceAmount)
	{
		if (priceAmount >= 200f)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_SellCard1");
		}
		if (priceAmount >= 5000f)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_SellCard2");
		}
		if (priceAmount >= 10000f)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_SellCard3");
		}
	}

	public static void OnDailyProfitReached(float priceAmount)
	{
		if (priceAmount >= 30000f)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_Profit3");
		}
		if (priceAmount >= 10000f)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_Profit2");
		}
		if (priceAmount >= 1000f)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_Profit1");
		}
	}

	public static void OnGetFullArtFoil(bool isGet)
	{
		if (isGet)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_FullArtFoil");
		}
	}

	public static void OnGetFullArtGhostFoil(bool isGet)
	{
		if (isGet)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_GhostFoil");
		}
	}

	public static void OnCheckAlbumCardCount(int count)
	{
		if (count >= 100)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_CardCollector1");
		}
		if (count >= 500)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_CardCollector2");
		}
		if (count >= 1500)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_CardCollector3");
		}
		if (count >= 2500)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_CardCollector4");
		}
	}

	public static void OnCheckGemMintCardCount(int count)
	{
		if (count >= 10)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_GradingGem1");
		}
		if (count >= 100)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_GradingGem2");
		}
		if (count >= 500)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_GradingGem3");
		}
	}

	public static void OnSoldGradedCard100K(float priceAmount, int cardGrade)
	{
		if (cardGrade > 0 && priceAmount >= 100000f)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_GradingGold");
		}
	}

	public static void OnCheckCollectedGradedCardSet()
	{
		int index = CSingleton<AchievementManager>.Instance.m_AchievementSringList.IndexOf("ACH_GradingCollector");
		if (CPlayerData.m_IsAchievementUnlocked[index])
		{
			return;
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<CompactCardDataAmount> list3 = new List<CompactCardDataAmount>();
		for (int i = 0; i < CPlayerData.m_HoldCardDataList.Count; i++)
		{
			CompactCardDataAmount compactCardDataAmount = new CompactCardDataAmount();
			compactCardDataAmount.cardSaveIndex = CPlayerData.GetCardSaveIndex(CPlayerData.m_HoldCardDataList[i]);
			compactCardDataAmount.amount = CPlayerData.m_HoldCardDataList[i].cardGrade;
			compactCardDataAmount.expansionType = CPlayerData.m_HoldCardDataList[i].expansionType;
			compactCardDataAmount.isDestiny = CPlayerData.m_HoldCardDataList[i].isDestiny;
			list3.Add(compactCardDataAmount);
		}
		for (int j = 0; j < CPlayerData.m_GradedCardInventoryList.Count; j++)
		{
			list3.Add(CPlayerData.m_GradedCardInventoryList[j]);
		}
		for (int k = 0; k < list3.Count; k++)
		{
			if (!list.Contains(list3[k].cardSaveIndex))
			{
				list.Add(list3[k].cardSaveIndex);
			}
			else if (!list2.Contains(list3[k].cardSaveIndex))
			{
				list2.Add(list3[k].cardSaveIndex);
			}
		}
		for (int l = 0; l < list2.Count; l++)
		{
			list.Clear();
			for (int m = 0; m < list3.Count; m++)
			{
				if (list3[m].cardSaveIndex == list2[l] && !list.Contains(list3[m].amount))
				{
					list.Add(list3[m].amount);
				}
			}
			if (list.Count >= 10)
			{
				CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_GradingCollector");
				break;
			}
		}
		list3.Clear();
	}

	public static void OnCleanSmellyCustomer(int count)
	{
		if (count >= 20)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_SmellyClean1");
		}
		if (count >= 200)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_SmellyClean2");
		}
	}

	public static void OnShopLotBUnlocked()
	{
		CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_UnlockShopLotB");
	}

	public static void OnUnlockShopExpansion(int roomCount)
	{
		if (roomCount >= 4)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopExpansion1");
		}
		if (roomCount >= 10)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopExpansion2");
		}
		if (roomCount >= 20)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopExpansion3");
		}
	}

	private void OnShopLevelUp(CEventPlayer_ShopLeveledUp evt)
	{
		if (evt.m_ShopLevel + 1 >= 5)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel1");
		}
		if (evt.m_ShopLevel + 1 >= 20)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel2");
		}
		if (evt.m_ShopLevel + 1 >= 50)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel3");
		}
		if (evt.m_ShopLevel + 1 >= 100)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel4");
		}
	}

	public static void ShopLevelCheck(int level)
	{
		if (level + 1 >= 5)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel1");
		}
		if (level + 1 >= 20)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel2");
		}
		if (level + 1 >= 50)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel3");
		}
		if (level + 1 >= 100)
		{
			CSingleton<AchievementManager>.Instance.UnlockAchievement("ACH_ShopLevel4");
		}
	}

	public void UnlockAchievement(string achievementID)
	{
		PlatformManager.Instance.UnlockAchievement(achievementID);
		int index = m_AchievementSringList.IndexOf(achievementID);
		CPlayerData.m_IsAchievementUnlocked[index] = true;
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_ShopLeveledUp>(OnShopLevelUp);
			CEventManager.AddListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_ShopLeveledUp>(OnShopLevelUp);
			CEventManager.RemoveListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
		}
	}

	protected void OnFinishHideLoadingScreen(CEventPlayer_FinishHideLoadingScreen evt)
	{
		ShopLevelCheck(CPlayerData.m_ShopLevel);
		for (int i = 0; i < CPlayerData.m_IsAchievementUnlocked.Count; i++)
		{
			if (CPlayerData.m_IsAchievementUnlocked[i])
			{
				UnlockAchievement(m_AchievementSringList[i]);
			}
		}
	}
}
