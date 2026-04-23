using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckPricePanelUI : UIElementBase
{
	public Image m_ItemImage;

	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_TotalPriceText;

	public TextMeshProUGUI m_PriceChangeText;

	public GameObject m_UpArrow;

	public GameObject m_DownArrow;

	public GameObject m_NoChangeArrow;

	public GameObject m_PrologueUIGrp;

	public CardUI m_CardUI;

	private bool m_IsItem;

	private bool m_IsCard;

	private bool m_IsDestiny;

	private int m_CardIndex;

	private int m_CardGrade;

	private ECardExpansionType m_CardExpansionType;

	private float m_TotalPrice;

	private EItemType m_ItemType = EItemType.None;

	private CheckPriceScreen m_CheckPriceScreen;

	public void InitItem(CheckPriceScreen checkPriceScreen, EItemType itemType)
	{
		m_UIGrp.SetActive(value: true);
		m_PrologueUIGrp.SetActive(value: false);
		if (CSingleton<CGameManager>.Instance.m_IsPrologue)
		{
			List<RestockData> restockDataUsingItemType = InventoryBase.GetRestockDataUsingItemType(itemType);
			if (restockDataUsingItemType.Count > 0 && !restockDataUsingItemType[0].prologueShow)
			{
				m_UIGrp.SetActive(value: false);
				m_PrologueUIGrp.SetActive(value: true);
			}
		}
		m_IsItem = true;
		m_IsCard = false;
		m_CheckPriceScreen = checkPriceScreen;
		m_ItemType = itemType;
		ItemData itemData = InventoryBase.GetItemData(m_ItemType);
		m_ItemImage.sprite = itemData.icon;
		m_TotalPrice = CPlayerData.GetItemMarketPrice(m_ItemType);
		m_NameText.text = itemData.GetName();
		m_TotalPriceText.text = GameInstance.GetPriceString(m_TotalPrice);
		m_ItemImage.enabled = true;
		m_CardUI.gameObject.SetActive(value: false);
		if (itemData.isHideItemUntilUnlocked)
		{
			int restockDataIndex = InventoryBase.GetRestockDataIndex(m_ItemType);
			bool flag = false;
			if (restockDataIndex > 0)
			{
				flag = CPlayerData.GetIsItemLicenseUnlocked(restockDataIndex);
			}
			if (!flag)
			{
				m_ItemImage.sprite = InventoryBase.GetQuestionMarkSprite();
				m_NameText.text = "???";
			}
		}
		List<float> floatDataList = CPlayerData.m_ItemPricePercentPastChangeList[(int)itemType].floatDataList;
		if (floatDataList.Count > 1)
		{
			float itemMarketPriceCustomPercent = CPlayerData.GetItemMarketPriceCustomPercent(itemType, floatDataList[floatDataList.Count - 2]);
			float num = m_TotalPrice - itemMarketPriceCustomPercent;
			if (num > 0.005f)
			{
				m_UpArrow.SetActive(value: true);
				m_DownArrow.SetActive(value: false);
				m_NoChangeArrow.SetActive(value: false);
				m_PriceChangeText.text = "+" + GameInstance.GetPriceString(num);
				m_PriceChangeText.color = m_CheckPriceScreen.m_PositiveColor;
			}
			else if (num < -0.005f)
			{
				m_UpArrow.SetActive(value: false);
				m_DownArrow.SetActive(value: true);
				m_NoChangeArrow.SetActive(value: false);
				m_PriceChangeText.text = GameInstance.GetPriceString(num);
				m_PriceChangeText.color = m_CheckPriceScreen.m_NegativeColor;
			}
			else
			{
				m_UpArrow.SetActive(value: false);
				m_DownArrow.SetActive(value: false);
				m_NoChangeArrow.SetActive(value: true);
				m_PriceChangeText.text = "+" + GameInstance.GetPriceString(0f);
				m_PriceChangeText.color = m_CheckPriceScreen.m_NeutralColor;
			}
		}
		else
		{
			m_UpArrow.SetActive(value: false);
			m_DownArrow.SetActive(value: false);
			m_NoChangeArrow.SetActive(value: true);
			m_PriceChangeText.text = "+" + GameInstance.GetPriceString(0f);
			m_PriceChangeText.color = m_CheckPriceScreen.m_NeutralColor;
		}
	}

	public void InitCard(CheckPriceScreen checkPriceScreen, int cardIndex, ECardExpansionType expansionType, bool isDestiny, int cardGrade)
	{
		CardData cardData = new CardData();
		cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(cardIndex, expansionType);
		cardData.isFoil = cardIndex % CPlayerData.GetCardAmountPerMonsterType(expansionType) >= CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false);
		cardData.borderType = (ECardBorderType)(cardIndex % CPlayerData.GetCardAmountPerMonsterType(expansionType, includeFoilCount: false));
		cardData.isDestiny = isDestiny;
		cardData.expansionType = expansionType;
		m_UIGrp.SetActive(value: true);
		m_PrologueUIGrp.SetActive(value: false);
		if (CSingleton<CGameManager>.Instance.m_IsPrologue && InventoryBase.GetMonsterData(cardData.monsterType).Rarity > ERarity.Common)
		{
			m_UIGrp.SetActive(value: false);
			m_PrologueUIGrp.SetActive(value: true);
			return;
		}
		m_IsItem = false;
		m_IsCard = true;
		m_CheckPriceScreen = checkPriceScreen;
		m_CardIndex = cardIndex;
		m_CardExpansionType = expansionType;
		m_IsDestiny = isDestiny;
		m_CardGrade = cardGrade;
		m_CardUI.SetCardUI(cardData);
		m_TotalPrice = CPlayerData.GetCardMarketPrice(cardData);
		m_NameText.text = InventoryBase.GetMonsterData(cardData.monsterType).GetName() + " - " + CPlayerData.GetFullCardTypeName(cardData);
		m_TotalPriceText.text = GameInstance.GetPriceString(m_TotalPrice);
		m_ItemImage.enabled = false;
		m_CardUI.gameObject.SetActive(value: true);
		List<float> pastCardPricePercentChange = CPlayerData.GetPastCardPricePercentChange(cardIndex, expansionType, isDestiny);
		if (pastCardPricePercentChange.Count > 1)
		{
			float cardMarketPriceCustomPercent = CPlayerData.GetCardMarketPriceCustomPercent(cardIndex, expansionType, isDestiny, pastCardPricePercentChange[pastCardPricePercentChange.Count - 2], cardGrade);
			float num = m_TotalPrice - cardMarketPriceCustomPercent;
			if (num > 0.005f)
			{
				m_UpArrow.SetActive(value: true);
				m_DownArrow.SetActive(value: false);
				m_NoChangeArrow.SetActive(value: false);
				m_PriceChangeText.text = "+" + GameInstance.GetPriceString(num);
				m_PriceChangeText.color = m_CheckPriceScreen.m_PositiveColor;
			}
			else if (num < -0.005f)
			{
				m_UpArrow.SetActive(value: false);
				m_DownArrow.SetActive(value: true);
				m_NoChangeArrow.SetActive(value: false);
				m_PriceChangeText.text = GameInstance.GetPriceString(num);
				m_PriceChangeText.color = m_CheckPriceScreen.m_NegativeColor;
			}
			else
			{
				m_UpArrow.SetActive(value: false);
				m_DownArrow.SetActive(value: false);
				m_NoChangeArrow.SetActive(value: true);
				m_PriceChangeText.text = "+" + GameInstance.GetPriceString(0f);
				m_PriceChangeText.color = m_CheckPriceScreen.m_NeutralColor;
			}
		}
		else
		{
			m_UpArrow.SetActive(value: false);
			m_DownArrow.SetActive(value: false);
			m_NoChangeArrow.SetActive(value: true);
			m_PriceChangeText.text = "+" + GameInstance.GetPriceString(0f);
			m_PriceChangeText.color = m_CheckPriceScreen.m_NeutralColor;
		}
	}

	public override void OnPressButton()
	{
		SoundManager.GenericConfirm();
		if (m_IsItem)
		{
			m_CheckPriceScreen.OnPressOpenItemPriceGraph(m_ItemType);
		}
		else if (m_IsCard)
		{
			m_CheckPriceScreen.OnPressOpenCardPriceGraph(m_CardIndex, m_CardExpansionType, m_IsDestiny, CPlayerData.m_LastSelectedPriceGraphCardGrade);
		}
	}
}
