using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

public class CustomerTradeCardScreen : UIScreenBase
{
	public ItemPriceGraphScreen m_ItemPriceGraphScreen;

	public CardUI m_CardUI_L;

	public CardUI m_CardUI_R;

	public CardUI m_CardUI_Album_L;

	public CardUI m_CardUI_Album;

	public CardUI m_CardUI_Buying;

	public List<string> m_CustomerSellCardTextList;

	public List<string> m_CustomerTradeCardTextList;

	public List<string> m_CustomerHaggleTextList;

	public List<string> m_CustomerDeclineTextList;

	public List<string> m_CustomerAcceptedTextList;

	public Animation m_CustomerTopTextAnim;

	public TMP_InputField m_SetPriceInput;

	public TextMeshProUGUI m_SetPriceInputDisplay;

	public TextMeshProUGUI m_SetPrice;

	public TextMeshProUGUI m_CustomerTopText;

	public TextMeshProUGUI m_MarketPrice_L;

	public TextMeshProUGUI m_MarketPrice_R;

	public TextMeshProUGUI m_AlbumCardCount_L;

	public TextMeshProUGUI m_AlbumCardCount;

	public GameObject m_CustomerTradingText;

	public GameObject m_CustomerSellingText;

	public GameObject m_TradeGrp_R;

	public GameObject m_SellGrp_R;

	public GameObject m_IsNewUI;

	public GameObject m_AcceptBtn;

	public GameObject m_CancelBtn;

	public GameObject m_LetMeThinkBtn;

	public GameObject m_DoneBtn;

	private bool m_IsTrading;

	private bool m_HasAccepted;

	private float m_PriceSet;

	private float m_LastPriceSet;

	private float m_SellCardAskPrice;

	private float m_SellCardMarketPrice;

	private int m_MaxDeclineCount;

	private int m_DeclineCount;

	private Customer m_CurrentCustomer;

	private CardData m_CardData_L;

	private CardData m_CardData_R;

	public void OnPressDecline()
	{
		SoundManager.GenericConfirm();
		CloseScreen();
	}

	public void OnPressLetMeThink()
	{
		SoundManager.GenericConfirm();
		CustomerTradeData customerTradeData = new CustomerTradeData();
		customerTradeData.m_IsTrading = m_IsTrading;
		customerTradeData.m_PriceSet = m_PriceSet;
		customerTradeData.m_LastPriceSet = m_LastPriceSet;
		customerTradeData.m_SellCardAskPrice = m_SellCardAskPrice;
		customerTradeData.m_SellCardMarketPrice = m_SellCardMarketPrice;
		customerTradeData.m_MaxDeclineCount = m_MaxDeclineCount;
		customerTradeData.m_DeclineCount = m_DeclineCount;
		customerTradeData.m_CardData_L = m_CardData_L;
		customerTradeData.m_CardData_R = m_CardData_R;
		m_CurrentCustomer.OnPressRefreshInteract(customerTradeData);
		CSingleton<CustomerManager>.Instance.m_IsPlayerTrading = false;
		ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
		base.OnCloseScreen();
	}

	public void OnPressAccept()
	{
		SoundManager.GenericConfirm();
		if (m_HasAccepted)
		{
			CloseScreen();
			return;
		}
		if (m_IsTrading)
		{
			if (m_CardData_R.cardGrade == 0 && CPlayerData.GetCardAmount(m_CardData_R) > 0)
			{
				CPlayerData.AddCard(m_CardData_L, 1);
				CPlayerData.ReduceCard(m_CardData_R, 1);
				CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
				m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + m_CustomerAcceptedTextList[Random.Range(0, m_CustomerAcceptedTextList.Count)]);
				m_CustomerTopTextAnim.Rewind();
				m_CustomerTopTextAnim.Play();
				m_AcceptBtn.SetActive(value: false);
				m_CancelBtn.SetActive(value: false);
				m_LetMeThinkBtn.SetActive(value: false);
				m_DoneBtn.SetActive(value: true);
				m_HasAccepted = true;
				if (m_CardData_L.cardGrade == 10)
				{
					CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained++;
					AchievementManager.OnCheckGemMintCardCount(CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained);
					AchievementManager.OnCheckCollectedGradedCardSet();
				}
			}
			else if (m_CardData_R.cardGrade > 0 && CPlayerData.HasGradedCardInAlbum(m_CardData_R))
			{
				CPlayerData.AddCard(m_CardData_L, 1);
				CPlayerData.RemoveGradedCard(m_CardData_R, ignoreGradedCardIndex: true);
				CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
				m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + m_CustomerAcceptedTextList[Random.Range(0, m_CustomerAcceptedTextList.Count)]);
				m_CustomerTopTextAnim.Rewind();
				m_CustomerTopTextAnim.Play();
				m_AcceptBtn.SetActive(value: false);
				m_CancelBtn.SetActive(value: false);
				m_LetMeThinkBtn.SetActive(value: false);
				m_DoneBtn.SetActive(value: true);
				m_HasAccepted = true;
				if (m_CardData_L.cardGrade == 10)
				{
					CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained++;
					AchievementManager.OnCheckGemMintCardCount(CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained);
					AchievementManager.OnCheckCollectedGradedCardSet();
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.DontHaveTheCard);
			}
			return;
		}
		if (CPlayerData.m_CoinAmountDouble < (double)m_PriceSet)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			return;
		}
		float num = m_PriceSet - m_SellCardAskPrice;
		float num2 = num / m_SellCardAskPrice;
		int num3 = Mathf.RoundToInt((1f + num2) * 100f);
		if (num2 < -0.5f)
		{
			num3 -= Random.Range(45, 75);
		}
		else if (num2 < -0.4f)
		{
			num3 -= Random.Range(35, 65);
		}
		else if (num2 < -0.3f)
		{
			num3 -= Random.Range(25, 55);
		}
		else if (num2 < -0.2f)
		{
			num3 -= Random.Range(15, 35);
		}
		else if (num2 < -0.1f)
		{
			num3 -= Random.Range(5, 15);
		}
		if (m_PriceSet > m_SellCardMarketPrice)
		{
			num3 += Random.Range(5, 15);
		}
		else if (m_PriceSet < m_SellCardMarketPrice)
		{
			num3 -= Random.Range(5, 15);
		}
		num3 -= Random.Range(5 * m_DeclineCount, 15 * m_DeclineCount);
		if (m_DeclineCount > 0 && m_PriceSet <= m_LastPriceSet)
		{
			num3 -= Random.Range(10 * m_DeclineCount, 25 * m_DeclineCount);
		}
		if (m_PriceSet >= m_SellCardAskPrice - 0.01f)
		{
			num3 = 100;
		}
		m_LastPriceSet = m_PriceSet;
		if (Random.Range(0, 100) < num3)
		{
			if (CPlayerData.m_CoinAmountDouble >= (double)m_PriceSet)
			{
				m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + m_CustomerAcceptedTextList[Random.Range(0, m_CustomerAcceptedTextList.Count)]);
				m_CustomerTopTextAnim.Rewind();
				m_CustomerTopTextAnim.Play();
				PriceChangeManager.AddTransaction(0f - m_PriceSet, ETransactionType.TradingBuyCard, 0, 1, m_CardData_L);
				CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(m_PriceSet));
				CPlayerData.m_GameReportDataCollect.supplyCost -= m_PriceSet;
				CPlayerData.m_GameReportDataCollectPermanent.supplyCost -= m_PriceSet;
				CPlayerData.AddCard(m_CardData_L, 1);
				m_CurrentCustomer.SetSoldCard(m_CardData_L);
				CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
				m_AcceptBtn.SetActive(value: false);
				m_CancelBtn.SetActive(value: false);
				m_LetMeThinkBtn.SetActive(value: false);
				m_DoneBtn.SetActive(value: true);
				m_HasAccepted = true;
				if (m_CardData_L.cardGrade == 10)
				{
					CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained++;
					AchievementManager.OnCheckGemMintCardCount(CPlayerData.m_GameReportDataCollectPermanent.gemMintCardObtained);
					AchievementManager.OnCheckCollectedGradedCardSet();
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			}
			return;
		}
		float maxInclusive = 2f;
		if (m_SellCardAskPrice > m_SellCardMarketPrice)
		{
			maxInclusive = 10f;
		}
		if (Random.Range(0, 100) < Mathf.RoundToInt((float)num3 * Random.Range(1f, maxInclusive)))
		{
			m_MaxDeclineCount--;
			m_DeclineCount++;
			if (m_SellCardAskPrice > m_SellCardMarketPrice * 1.2f)
			{
				m_SellCardAskPrice += Random.Range(num * 0.1f, num * 0.9f);
			}
			else
			{
				m_SellCardAskPrice += Random.Range(num * 0.05f, num * 0.25f);
			}
			m_MarketPrice_L.text = LocalizationManager.GetTranslation("Ask Price") + " : " + GameInstance.GetPriceString(m_SellCardAskPrice);
			m_MarketPrice_L.text += "\n";
			TextMeshProUGUI marketPrice_L = m_MarketPrice_L;
			marketPrice_L.text = marketPrice_L.text + LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(m_SellCardMarketPrice);
			m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + m_CustomerHaggleTextList[Random.Range(0, m_CustomerHaggleTextList.Count)]);
			m_CustomerTopTextAnim.Rewind();
			m_CustomerTopTextAnim.Play();
		}
		else if (m_MaxDeclineCount > 0)
		{
			m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + m_CustomerDeclineTextList[Random.Range(0, m_CustomerDeclineTextList.Count)]);
			m_CustomerTopTextAnim.Rewind();
			m_CustomerTopTextAnim.Play();
			m_MaxDeclineCount--;
			m_DeclineCount++;
		}
		else
		{
			CloseScreen();
		}
	}

	public void SetCustomer(Customer customer, CustomerTradeData customerTradeData)
	{
		CSingleton<CustomerManager>.Instance.m_IsPlayerTrading = true;
		m_CurrentCustomer = customer;
		m_HasAccepted = false;
		m_IsTrading = false;
		int num = CPlayerData.m_ShopLevel;
		if (num > 40)
		{
			num = 40;
		}
		if (Random.Range(0, 100) < num)
		{
			m_IsTrading = true;
		}
		if (customerTradeData != null)
		{
			m_IsTrading = customerTradeData.m_IsTrading;
		}
		if (m_IsTrading)
		{
			m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + m_CustomerTradeCardTextList[Random.Range(0, m_CustomerTradeCardTextList.Count)]);
			m_CustomerTopTextAnim.Rewind();
			m_CustomerTopTextAnim.Play();
		}
		else
		{
			m_MaxDeclineCount = Random.Range(0, 5);
			m_DeclineCount = 0;
			m_CustomerTopText.text = LocalizationManager.GetTranslation("CustomerTrade/" + m_CustomerSellCardTextList[Random.Range(0, m_CustomerSellCardTextList.Count)]);
			m_CustomerTopTextAnim.Rewind();
			m_CustomerTopTextAnim.Play();
		}
		m_CustomerTradingText.SetActive(m_IsTrading);
		m_CustomerSellingText.SetActive(!m_IsTrading);
		m_TradeGrp_R.SetActive(m_IsTrading);
		m_SellGrp_R.SetActive(!m_IsTrading);
		ECardExpansionType eCardExpansionType = ECardExpansionType.Tetramon;
		int num2 = 0;
		bool flag = false;
		float num3 = 1f;
		int num4 = 0;
		int num5 = 1;
		int num6 = 0;
		int num7 = 0;
		if (CPlayerData.m_ShopLevel >= InventoryBase.GetUnlockItemLevelRequired(EItemType.RareCardPack))
		{
			num5++;
			num7 += 2;
		}
		if (CPlayerData.m_ShopLevel >= InventoryBase.GetUnlockItemLevelRequired(EItemType.EpicCardPack))
		{
			num5++;
			num7 += 2;
		}
		if (CPlayerData.m_ShopLevel >= InventoryBase.GetUnlockItemLevelRequired(EItemType.LegendaryCardPack))
		{
			num5 += 2;
			num6++;
			num7 += 2;
		}
		if (CPlayerData.m_ShopLevel >= InventoryBase.GetUnlockItemLevelRequired(EItemType.DestinyBasicCardPack))
		{
			num4 += 15;
			num5++;
			num6++;
			num7 += 2;
		}
		if (CPlayerData.m_ShopLevel >= InventoryBase.GetUnlockItemLevelRequired(EItemType.DestinyRareCardPack))
		{
			num4 += 10;
			num5++;
			num6++;
			num7 += 2;
		}
		if (CPlayerData.m_ShopLevel >= InventoryBase.GetUnlockItemLevelRequired(EItemType.DestinyEpicCardPack))
		{
			num4 += 10;
			num5++;
			num6++;
			num7 += 2;
		}
		if (CPlayerData.m_ShopLevel >= InventoryBase.GetUnlockItemLevelRequired(EItemType.DestinyLegendaryCardPack))
		{
			num4 += 15;
			num5 += 2;
			num6++;
			num7 += 3;
		}
		if (Random.Range(0, 250) < num5)
		{
			eCardExpansionType = ECardExpansionType.Ghost;
		}
		else if (Random.Range(0, 100) < num4)
		{
			eCardExpansionType = ECardExpansionType.Destiny;
		}
		int num8 = 0;
		if (Random.Range(0, 100) < num7)
		{
			num8 = Random.Range(1, 11);
		}
		int maxExclusive = InventoryBase.GetShownMonsterList(eCardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(eCardExpansionType);
		int num9 = Random.Range(0, maxExclusive);
		if (eCardExpansionType == ECardExpansionType.Ghost && Random.Range(0, 100) < 50)
		{
			flag = true;
		}
		if (num8 > 0)
		{
			CompactCardDataAmount compactCardDataAmount = new CompactCardDataAmount();
			compactCardDataAmount.cardSaveIndex = num9;
			compactCardDataAmount.expansionType = eCardExpansionType;
			compactCardDataAmount.isDestiny = flag;
			compactCardDataAmount.amount = num8;
			m_CardData_L = CPlayerData.GetGradedCardData(compactCardDataAmount);
		}
		else
		{
			m_CardData_L = CPlayerData.GetCardData(num9, eCardExpansionType, flag);
		}
		if (eCardExpansionType == ECardExpansionType.Ghost)
		{
			if (Random.Range(0, 100) < num6)
			{
				m_CardData_L.isFoil = true;
			}
			else
			{
				m_CardData_L.isFoil = false;
			}
		}
		if (customerTradeData != null)
		{
			m_CardData_L = customerTradeData.m_CardData_L;
			eCardExpansionType = m_CardData_L.expansionType;
		}
		bool active = CPlayerData.GetCardAmount(m_CardData_L) == 0;
		m_IsNewUI.SetActive(active);
		m_CardUI_L.SetCardUI(m_CardData_L);
		m_CardUI_Album_L.SetCardUI(m_CardData_L);
		m_AlbumCardCount_L.text = "X" + CPlayerData.GetCardAmount(m_CardData_L);
		if (m_CardData_L.cardGrade > 0)
		{
			if (CPlayerData.HasGradedCardInAlbum(m_CardData_L))
			{
				m_AlbumCardCount_L.text = "X1";
			}
			else
			{
				m_AlbumCardCount_L.text = "X0";
			}
		}
		m_AcceptBtn.SetActive(value: true);
		m_CancelBtn.SetActive(value: true);
		m_LetMeThinkBtn.SetActive(value: true);
		m_DoneBtn.SetActive(value: false);
		if (!m_IsTrading)
		{
			float num10 = Random.Range(0.6f, 1.3f);
			if (num10 < 0.7f)
			{
				num10 = 0.7f;
			}
			if (num10 > 1.2f)
			{
				num10 = 1.2f;
			}
			if (m_SellCardMarketPrice > 10f)
			{
				num10 += Random.Range(0f, 0.03f);
			}
			if (m_SellCardMarketPrice > 20f)
			{
				num10 += Random.Range(0f, 0.04f);
			}
			if (m_SellCardMarketPrice > 100f)
			{
				num10 += Random.Range(0.01f, 0.05f);
			}
			if (m_SellCardMarketPrice > 300f)
			{
				num10 += Random.Range(0.01f, 0.06f);
			}
			if (m_SellCardMarketPrice > 500f)
			{
				num10 += Random.Range(0.01f, 0.08f);
			}
			if (m_SellCardMarketPrice > 1000f)
			{
				num10 += Random.Range(0.01f, 0.1f);
			}
			if (m_SellCardMarketPrice > 2000f)
			{
				num10 += Random.Range(0.02f, 0.11f);
			}
			if (m_SellCardMarketPrice > 5000f)
			{
				num10 += Random.Range(0.03f, 0.12f);
			}
			if (m_SellCardMarketPrice > 8000f)
			{
				num10 += Random.Range(0.03f, 0.12f);
			}
			m_SellCardMarketPrice = CPlayerData.GetCardMarketPrice(m_CardData_L);
			num3 = (m_SellCardAskPrice = m_SellCardMarketPrice * num10);
			if (customerTradeData != null)
			{
				m_PriceSet = customerTradeData.m_PriceSet;
				m_LastPriceSet = customerTradeData.m_LastPriceSet;
				m_SellCardAskPrice = customerTradeData.m_SellCardAskPrice;
				m_MaxDeclineCount = customerTradeData.m_MaxDeclineCount;
				m_DeclineCount = customerTradeData.m_DeclineCount;
				m_SetPrice.text = GameInstance.GetPriceString(m_PriceSet);
				m_SetPriceInputDisplay.text = GameInstance.GetPriceString(m_PriceSet);
			}
			else
			{
				OnInputTextUpdated("0");
			}
			m_MarketPrice_L.text = LocalizationManager.GetTranslation("Ask Price") + " : " + GameInstance.GetPriceString(m_SellCardAskPrice);
			m_MarketPrice_L.text += "\n";
			TextMeshProUGUI marketPrice_L = m_MarketPrice_L;
			marketPrice_L.text = marketPrice_L.text + LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(m_SellCardMarketPrice);
			m_CardUI_Buying.SetCardUI(m_CardData_L);
		}
		else
		{
			num3 = CPlayerData.GetCardMarketPrice(m_CardData_L);
			m_MarketPrice_L.text = LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(num3);
		}
		if (!m_IsTrading)
		{
			return;
		}
		bool flag2 = false;
		if (customerTradeData == null)
		{
			if (eCardExpansionType == ECardExpansionType.Ghost && Random.Range(0, 100) < 50)
			{
				flag = true;
			}
			for (int i = 0; i < InventoryBase.GetShownMonsterList(eCardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(eCardExpansionType); i++)
			{
				num2 = i;
				float cardMarketPrice = CPlayerData.GetCardMarketPrice(num2, eCardExpansionType, flag, 0);
				if (CPlayerData.GetCardAmountByIndex(num2, eCardExpansionType, flag) > 0 && cardMarketPrice >= num3 * 0.75f && cardMarketPrice < num3 * 1.5f && Random.Range(0, 100) < 25 && num9 != num2)
				{
					flag2 = true;
					m_CardData_R = CPlayerData.GetCardData(num2, eCardExpansionType, flag);
					if (m_CardData_L != m_CardData_R)
					{
						break;
					}
				}
			}
			int num11 = Random.Range(0, 100);
			if (!flag2 || num11 < 50)
			{
				List<int> cardCollectedList = CPlayerData.GetCardCollectedList(ECardExpansionType.Tetramon, isDimension: false);
				List<int> list = new List<int>();
				int num12 = InventoryBase.GetShownMonsterList(ECardExpansionType.Tetramon).Count * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Tetramon);
				for (int j = 0; j < cardCollectedList.Count && j <= num12; j++)
				{
					if (cardCollectedList[j] > 0)
					{
						float cardMarketPrice2 = CPlayerData.GetCardMarketPrice(j, ECardExpansionType.Tetramon, isDestiny: false, 0);
						if (cardMarketPrice2 >= num3 * 0.75f && cardMarketPrice2 < num3 * 1.5f)
						{
							list.Add(j);
						}
					}
				}
				List<int> cardCollectedList2 = CPlayerData.GetCardCollectedList(ECardExpansionType.Destiny, isDimension: false);
				List<int> list2 = new List<int>();
				num12 = InventoryBase.GetShownMonsterList(ECardExpansionType.Destiny).Count * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Destiny);
				for (int k = 0; k < cardCollectedList2.Count && k <= num12; k++)
				{
					if (cardCollectedList2[k] > 0)
					{
						float cardMarketPrice3 = CPlayerData.GetCardMarketPrice(k, ECardExpansionType.Destiny, isDestiny: false, 0);
						if (cardMarketPrice3 >= num3 * 0.75f && cardMarketPrice3 < num3 * 1.5f)
						{
							list2.Add(k);
						}
					}
				}
				List<int> cardCollectedList3 = CPlayerData.GetCardCollectedList(ECardExpansionType.Ghost, isDimension: false);
				List<int> list3 = new List<int>();
				num12 = InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost).Count * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Ghost);
				for (int l = 0; l < cardCollectedList3.Count && l <= num12; l++)
				{
					if (cardCollectedList3[l] > 0)
					{
						float cardMarketPrice4 = CPlayerData.GetCardMarketPrice(l, ECardExpansionType.Ghost, isDestiny: false, 0);
						if (cardMarketPrice4 >= num3 * 0.75f && cardMarketPrice4 < num3 * 1.5f)
						{
							list3.Add(l);
						}
					}
				}
				List<int> cardCollectedList4 = CPlayerData.GetCardCollectedList(ECardExpansionType.Ghost, isDimension: true);
				List<int> list4 = new List<int>();
				num12 = InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost).Count * CPlayerData.GetCardAmountPerMonsterType(ECardExpansionType.Ghost);
				for (int m = 0; m < cardCollectedList4.Count && m <= num12; m++)
				{
					if (cardCollectedList4[m] > 0)
					{
						float cardMarketPrice5 = CPlayerData.GetCardMarketPrice(m, ECardExpansionType.Ghost, isDestiny: true, 0);
						if (cardMarketPrice5 >= num3 * 0.75f && cardMarketPrice5 < num3 * 1.5f)
						{
							list4.Add(m);
						}
					}
				}
				List<int> list5 = new List<int>();
				for (int n = 0; n < CPlayerData.m_GradedCardInventoryList.Count; n++)
				{
					float cardMarketPrice6 = CPlayerData.GetCardMarketPrice(CPlayerData.m_GradedCardInventoryList[n].cardSaveIndex, CPlayerData.m_GradedCardInventoryList[n].expansionType, CPlayerData.m_GradedCardInventoryList[n].isDestiny, CPlayerData.m_GradedCardInventoryList[n].amount);
					if (cardMarketPrice6 >= num3 * 0.75f && cardMarketPrice6 < num3 * 1.5f)
					{
						list5.Add(n);
					}
				}
				List<int> list6 = new List<int>();
				if (list.Count > 0)
				{
					list6.Add(0);
				}
				if (list2.Count > 0)
				{
					list6.Add(1);
				}
				if (list3.Count > 0)
				{
					list6.Add(2);
				}
				if (list4.Count > 0)
				{
					list6.Add(3);
				}
				if (list5.Count > 0)
				{
					list6.Add(4);
				}
				int num13 = -1;
				if (list6.Count > 0)
				{
					num13 = list6[Random.Range(0, list6.Count)];
				}
				switch (num13)
				{
				case 0:
					m_CardData_R = CPlayerData.GetCardData(list[Random.Range(0, list.Count)], ECardExpansionType.Tetramon, isDestiny: false);
					flag2 = true;
					break;
				case 1:
					m_CardData_R = CPlayerData.GetCardData(list2[Random.Range(0, list2.Count)], ECardExpansionType.Destiny, isDestiny: false);
					flag2 = true;
					break;
				case 2:
					m_CardData_R = CPlayerData.GetCardData(list3[Random.Range(0, list3.Count)], ECardExpansionType.Ghost, isDestiny: false);
					flag2 = true;
					break;
				case 3:
					m_CardData_R = CPlayerData.GetCardData(list4[Random.Range(0, list4.Count)], ECardExpansionType.Ghost, isDestiny: true);
					flag2 = true;
					break;
				case 4:
				{
					int num14 = list5[Random.Range(0, list5.Count)];
					if (CPlayerData.m_GradedCardInventoryList.Count > num14)
					{
						m_CardData_R = CPlayerData.GetGradedCardData(CPlayerData.m_GradedCardInventoryList[num14]);
						flag2 = true;
					}
					break;
				}
				}
				if (flag2 && m_CardData_L.isFoil == m_CardData_R.isFoil && m_CardData_L.expansionType == m_CardData_R.expansionType && m_CardData_L.monsterType == m_CardData_R.monsterType && m_CardData_L.borderType == m_CardData_R.borderType && m_CardData_L.isDestiny == m_CardData_R.isDestiny)
				{
					flag2 = false;
				}
			}
			if (!flag2)
			{
				int num15 = 0;
				if (eCardExpansionType == ECardExpansionType.Ghost)
				{
					flag = !flag;
				}
				while (!flag2)
				{
					num2 = Random.Range(0, maxExclusive);
					m_CardData_R = CPlayerData.GetCardData(num2, eCardExpansionType, flag);
					float cardMarketPrice7 = CPlayerData.GetCardMarketPrice(m_CardData_R);
					if (cardMarketPrice7 >= num3 * 0.35f && cardMarketPrice7 < num3 * 2f && num9 != num2 && m_CardData_L != m_CardData_R)
					{
						flag2 = true;
					}
					num15++;
					if (flag2 || num15 >= 10000)
					{
						break;
					}
				}
			}
		}
		else if (customerTradeData != null)
		{
			flag2 = true;
			m_CardData_R = customerTradeData.m_CardData_R;
		}
		if (!flag2)
		{
			m_CardData_R.monsterType = m_CardData_L.monsterType;
			m_CardData_R.expansionType = m_CardData_L.expansionType;
			if (m_CardData_L.expansionType == ECardExpansionType.Ghost)
			{
				m_CardData_R.isDestiny = !m_CardData_L.isDestiny;
			}
			else
			{
				m_CardData_R.isDestiny = m_CardData_L.isDestiny;
			}
			m_CardData_R.isFoil = m_CardData_L.isFoil;
			m_CardData_R.borderType = m_CardData_L.borderType;
			flag2 = true;
		}
		if (!flag2)
		{
			return;
		}
		m_CardUI_R.SetCardUI(m_CardData_R);
		m_CardUI_Album.SetCardUI(m_CardData_R);
		m_AlbumCardCount.text = "X" + CPlayerData.GetCardAmount(m_CardData_R);
		if (m_CardData_R.cardGrade > 0)
		{
			if (CPlayerData.HasGradedCardInAlbum(m_CardData_R))
			{
				m_AlbumCardCount.text = "X1";
			}
			else
			{
				m_AlbumCardCount.text = "X0";
			}
		}
		m_MarketPrice_R.text = LocalizationManager.GetTranslation("Market Price") + " : " + GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(m_CardData_R));
	}

	protected override void OnCloseScreen()
	{
		CSingleton<CustomerManager>.Instance.m_IsPlayerTrading = false;
		m_CurrentCustomer.OnPressStopInteract();
		base.OnCloseScreen();
	}

	public void OnPressOpenPriceChartScreen(bool isLeft)
	{
		SoundManager.GenericMenuOpen();
		if (isLeft)
		{
			m_ItemPriceGraphScreen.ShowCardPriceChart(CPlayerData.GetCardSaveIndex(m_CardData_L), m_CardData_L.expansionType, m_CardData_L.isDestiny, m_CardData_L.cardGrade);
		}
		else
		{
			m_ItemPriceGraphScreen.ShowCardPriceChart(CPlayerData.GetCardSaveIndex(m_CardData_R), m_CardData_R.expansionType, m_CardData_R.isDestiny, m_CardData_R.cardGrade);
		}
		m_ItemPriceGraphScreen.OpenScreen();
	}

	public void OnInputChanged(string text)
	{
		float num = GameInstance.GetInvariantCultureDecimal(text) / GameInstance.GetCurrencyConversionRate();
		num = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
		m_SetPriceInputDisplay.text = GameInstance.GetPriceString(num);
		m_SetPriceInputDisplay.gameObject.SetActive(value: true);
		m_SetPrice.gameObject.SetActive(value: false);
	}

	public void OnInputTextSelected(string text)
	{
		m_SetPriceInputDisplay.gameObject.SetActive(value: true);
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			m_SetPriceInput.text = GameInstance.GetPriceString(m_PriceSet, useDashAsZero: false, useCurrencySymbol: false, useCentSymbol: false, "F0");
		}
		else
		{
			m_SetPriceInput.text = GameInstance.GetPriceString(m_PriceSet, useDashAsZero: false, useCurrencySymbol: false);
		}
		m_SetPrice.gameObject.SetActive(value: false);
	}

	public void OnInputTextUpdated(string text)
	{
		float num = GameInstance.GetInvariantCultureDecimal(text) / GameInstance.GetCurrencyConversionRate();
		m_PriceSet = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
		if (m_PriceSet < 0f)
		{
			m_PriceSet = 0f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			m_SetPriceInput.text = GameInstance.GetPriceString(m_PriceSet, useDashAsZero: false, useCurrencySymbol: false, useCentSymbol: false, "F0");
		}
		else
		{
			m_SetPriceInput.text = GameInstance.GetPriceString(m_PriceSet, useDashAsZero: false, useCurrencySymbol: false);
		}
		m_SetPriceInputDisplay.text = GameInstance.GetPriceString(m_PriceSet);
		m_SetPrice.text = GameInstance.GetPriceString(m_PriceSet);
		m_SetPriceInputDisplay.gameObject.SetActive(value: false);
		m_SetPrice.gameObject.SetActive(value: true);
	}
}
