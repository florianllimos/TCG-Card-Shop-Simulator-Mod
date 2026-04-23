using System;
using System.Collections.Generic;
using UnityEngine;

public class PriceChangeManager : CSingleton<PriceChangeManager>
{
	public List<float> m_SetGameEventPriceList = new List<float>();

	public List<float> m_GeneratedGameEventPriceList = new List<float>();

	public List<float> m_GameEventPricePercentChangeList = new List<float>();

	public List<TransactionData> m_TransactionDataList = new List<TransactionData>();

	private float m_PriceChangeMin = 0.2f;

	private float m_PriceChangeMax = 5f;

	private float m_PriceCrashMin = 5f;

	private float m_PriceCrashMax = 50f;

	public bool m_Debug;

	private void Init()
	{
		if (false || CPlayerData.m_GeneratedGameEventPriceList[0] == 0f)
		{
			for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_GameEventDataList.Count; i++)
			{
				CPlayerData.m_GeneratedGameEventPriceList[i] = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_GameEventDataList[i].baseFee * Mathf.Clamp(UnityEngine.Random.Range(0.9f, 1.1f), 1f, 1.1f);
				CPlayerData.m_GeneratedGameEventPriceList[i] = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_GameEventDataList[i].baseFee;
				float marketPriceMinPercent = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_GameEventDataList[i].marketPriceMinPercent;
				float marketPriceMaxPercent = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_GameEventDataList[i].marketPriceMaxPercent;
				if (marketPriceMinPercent == 0f)
				{
					marketPriceMinPercent = 0.9f;
					marketPriceMaxPercent = 1.1f;
				}
				marketPriceMinPercent = 1f;
				marketPriceMaxPercent = 1f;
				CPlayerData.m_GeneratedGameEventPriceList[i] = CPlayerData.m_GeneratedGameEventPriceList[i] * UnityEngine.Random.Range(marketPriceMinPercent, marketPriceMaxPercent);
			}
			for (int j = 0; j < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_GameEventDataList.Count; j++)
			{
				CPlayerData.m_GameEventPricePercentChangeList[j] = UnityEngine.Random.Range(0, 10);
				CPlayerData.m_GameEventPricePercentChangeList[j] = 0f;
				CPlayerData.m_SetGameEventPriceList[j] = GetGameEventMarketPrice((EGameEventFormat)j);
			}
		}
		m_SetGameEventPriceList = CPlayerData.m_SetGameEventPriceList;
		m_GeneratedGameEventPriceList = CPlayerData.m_GeneratedGameEventPriceList;
		m_GameEventPricePercentChangeList = CPlayerData.m_GameEventPricePercentChangeList;
		m_TransactionDataList = CPlayerData.m_TransactionDataList;
	}

	private void EvaluatePriceChange()
	{
		GameEventData gameEventData = InventoryBase.GetGameEventData(CPlayerData.m_GameEventFormat);
		EPriceChangeType randomPriceChangeType = GetRandomPriceChangeType(gameEventData.positivePriceChangeType);
		EPriceChangeType randomPriceChangeType2 = GetRandomPriceChangeType(gameEventData.negativePriceChangeType);
		if (randomPriceChangeType == randomPriceChangeType2)
		{
			return;
		}
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList.Count; i++)
		{
			EItemType itemType = (EItemType)i;
			bool num = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[i].affectedPriceChangeType.Contains(randomPriceChangeType);
			bool flag = CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList[i].affectedPriceChangeType.Contains(randomPriceChangeType2);
			float num2 = UnityEngine.Random.Range(m_PriceChangeMin, m_PriceChangeMax);
			if (num)
			{
				CPlayerData.AddItemPricePercentChange(itemType, num2);
			}
			else if (flag)
			{
				CPlayerData.AddItemPricePercentChange(itemType, 0f - num2);
			}
			int num3 = UnityEngine.Random.Range(0, 100);
			if (num3 < 40)
			{
				if (num3 % 2 == 0)
				{
					CPlayerData.AddItemPricePercentChange(itemType, num2 / 2.5f);
				}
				else
				{
					CPlayerData.AddItemPricePercentChange(itemType, (0f - num2) / 2.5f);
				}
			}
		}
		UpdateCardMarketPrice(CPlayerData.m_GameEventExpansionType, randomPriceChangeType, isIncrease: true);
		UpdateCardMarketPrice(CPlayerData.m_GameEventExpansionType, randomPriceChangeType2, isIncrease: false);
		if (CPlayerData.m_GameEventExpansionType == ECardExpansionType.Tetramon)
		{
			UpdateCardMarketPrice(ECardExpansionType.Destiny, randomPriceChangeType, isIncrease: true);
			UpdateCardMarketPrice(ECardExpansionType.Destiny, randomPriceChangeType2, isIncrease: false);
			UpdateCardMarketPrice(ECardExpansionType.Ghost, randomPriceChangeType, isIncrease: true);
			UpdateCardMarketPrice(ECardExpansionType.Ghost, randomPriceChangeType2, isIncrease: false);
		}
	}

	private EPriceChangeType GetRandomPriceChangeType(EPriceChangeType priceChangeType)
	{
		List<EPriceChangeType> list = new List<EPriceChangeType>();
		switch (priceChangeType)
		{
		case EPriceChangeType.RandomRarityPackCard:
			list.Add(EPriceChangeType.CommonPackCard);
			list.Add(EPriceChangeType.RarePackCard);
			list.Add(EPriceChangeType.EpicPackCard);
			list.Add(EPriceChangeType.LegendPackCard);
			break;
		case EPriceChangeType.NoneCommonPackCard:
			list.Add(EPriceChangeType.RarePackCard);
			list.Add(EPriceChangeType.EpicPackCard);
			list.Add(EPriceChangeType.LegendPackCard);
			break;
		case EPriceChangeType.RandomElement:
			list.Add(EPriceChangeType.FireElement);
			list.Add(EPriceChangeType.EarthElement);
			list.Add(EPriceChangeType.WaterElement);
			list.Add(EPriceChangeType.WindElement);
			break;
		case EPriceChangeType.RandomBorder:
			list.Add(EPriceChangeType.FirstEdition);
			list.Add(EPriceChangeType.SilverBorder);
			list.Add(EPriceChangeType.GoldBorder);
			list.Add(EPriceChangeType.ExBorder);
			list.Add(EPriceChangeType.FullArtBorder);
			break;
		case EPriceChangeType.RandomEffect:
			list.Add(EPriceChangeType.RandomRarityPackCard);
			list.Add(EPriceChangeType.RandomElement);
			list.Add(EPriceChangeType.RandomBorder);
			return GetRandomPriceChangeType(list[UnityEngine.Random.Range(0, list.Count)]);
		default:
			list.Add(priceChangeType);
			break;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private void UpdateCardMarketPrice(ECardExpansionType expansionType, EPriceChangeType priceChangeType, bool isIncrease)
	{
		MonsterData monsterData = null;
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
			ECardBorderType eCardBorderType = (ECardBorderType)(i % CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false));
			monsterData = InventoryBase.GetMonsterData(CPlayerData.GetMonsterTypeFromCardSaveIndex(num2, expansionType));
			ERarity rarity = monsterData.Rarity;
			bool flag = i % CPlayerData.GetCardAmountPerMonsterType(expansionType) >= CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false);
			bool flag2 = false;
			if (priceChangeType == EPriceChangeType.Foil && flag)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.NonFoil && !flag)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.CommonPackCard && rarity == ERarity.Common)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.RarePackCard && rarity == ERarity.Rare)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.EpicPackCard && rarity == ERarity.Epic)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.LegendPackCard && rarity == ERarity.Legendary)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.FirstEdition && eCardBorderType == ECardBorderType.FirstEdition)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.SilverBorder && eCardBorderType == ECardBorderType.Silver)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.GoldBorder && eCardBorderType == ECardBorderType.Gold)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.ExBorder && eCardBorderType == ECardBorderType.EX)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.FullArtBorder && eCardBorderType == ECardBorderType.FullArt)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.FireElement && monsterData.ElementIndex == EElementIndex.Fire)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.EarthElement && monsterData.ElementIndex == EElementIndex.Earth)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.WaterElement && monsterData.ElementIndex == EElementIndex.Water)
			{
				flag2 = true;
			}
			else if (priceChangeType == EPriceChangeType.WindElement && monsterData.ElementIndex == EElementIndex.Wind)
			{
				flag2 = true;
			}
			float num3 = UnityEngine.Random.Range(m_PriceChangeMin, m_PriceChangeMax);
			if (flag2)
			{
				if (!isIncrease)
				{
					num3 *= -1f;
				}
				CPlayerData.AddCardPricePercentChange(num2, expansionType, isDestiny, num3);
			}
			int num4 = UnityEngine.Random.Range(0, 100);
			if (num4 < 40)
			{
				if (num4 % 2 == 0)
				{
					CPlayerData.AddCardPricePercentChange(num2, expansionType, isDestiny, num3 / 2.5f);
				}
				else
				{
					CPlayerData.AddCardPricePercentChange(num2, expansionType, isDestiny, (0f - num3) / 2.5f);
				}
			}
		}
	}

	private void EvaluatePriceCrash()
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ItemDataList.Count; i++)
		{
			EItemType eItemType = (EItemType)i;
			float num = CPlayerData.m_ItemPricePercentChangeList[(int)eItemType];
			if (num > 25f)
			{
				if ((float)UnityEngine.Random.Range(0, 100) < Mathf.Abs(num))
				{
					float percentChange = UnityEngine.Random.Range(m_PriceCrashMin, (0f - num) / 2.1f);
					CPlayerData.AddItemPricePercentChange(eItemType, percentChange);
				}
			}
			else if (num < -25f && (float)UnityEngine.Random.Range(0, 100) < Mathf.Abs(num))
			{
				float percentChange2 = UnityEngine.Random.Range(m_PriceCrashMin, Mathf.Abs(num) / 2.1f);
				CPlayerData.AddItemPricePercentChange(eItemType, percentChange2);
			}
		}
		EvaluateCardPriceCrash(ECardExpansionType.Tetramon);
		EvaluateCardPriceCrash(ECardExpansionType.Destiny);
		EvaluateCardPriceCrash(ECardExpansionType.Ghost);
		EvaluateCardPriceCrash(ECardExpansionType.FantasyRPG);
		EvaluateCardPriceCrash(ECardExpansionType.Megabot);
		EvaluateCardPriceCrash(ECardExpansionType.CatJob);
	}

	private void EvaluateCardPriceCrash(ECardExpansionType expansionType)
	{
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
			float cardPricePercentChange = CPlayerData.GetCardPricePercentChange(num2, expansionType, isDestiny);
			if (cardPricePercentChange > 25f)
			{
				if ((float)UnityEngine.Random.Range(0, 100) < Mathf.Abs(cardPricePercentChange))
				{
					float percentChange = UnityEngine.Random.Range(m_PriceCrashMin, (0f - cardPricePercentChange) / 2.1f);
					CPlayerData.AddCardPricePercentChange(num2, expansionType, isDestiny, percentChange);
				}
			}
			else if (cardPricePercentChange < -25f && (float)UnityEngine.Random.Range(0, 100) < Mathf.Abs(cardPricePercentChange))
			{
				float percentChange2 = UnityEngine.Random.Range(m_PriceCrashMin, Mathf.Abs(cardPricePercentChange) / 2.1f);
				CPlayerData.AddCardPricePercentChange(num2, expansionType, isDestiny, percentChange2);
			}
		}
	}

	public static void SetGameEventPrice(EGameEventFormat gameEventFormat, float price)
	{
		CPlayerData.m_SetGameEventPriceList[(int)gameEventFormat] = price;
	}

	public static float GetGameEventPrice(EGameEventFormat gameEventFormat, bool preventZero = true)
	{
		if (gameEventFormat == EGameEventFormat.None)
		{
			return 0f;
		}
		double num = Math.Round(CPlayerData.m_SetGameEventPriceList[(int)gameEventFormat], 2, MidpointRounding.AwayFromZero);
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			num = Math.Round(CPlayerData.m_SetGameEventPriceList[(int)gameEventFormat], 3, MidpointRounding.AwayFromZero);
		}
		return (float)num;
	}

	public static float GetGameEventMarketPrice(EGameEventFormat gameEventFormat)
	{
		if (gameEventFormat == EGameEventFormat.None)
		{
			return 0f;
		}
		return Mathf.CeilToInt(Mathf.RoundToInt((CPlayerData.m_GeneratedGameEventPriceList[(int)gameEventFormat] + CPlayerData.m_GeneratedGameEventPriceList[(int)gameEventFormat] * (CPlayerData.m_GameEventPricePercentChangeList[(int)gameEventFormat] / 100f)) / 1f));
	}

	public static void AddTransaction(float mMoneyChangeAmount, ETransactionType mTransactionType, int mIndex, int mAmount = 0, CardData mCardData = null)
	{
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
		EvaluatePriceChange();
		EvaluatePriceCrash();
		CPlayerData.UpdateItemPricePercentChange();
		CPlayerData.UpdatePastCardPricePercentChange();
	}
}
