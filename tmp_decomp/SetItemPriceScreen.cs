using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetItemPriceScreen : CSingleton<SetItemPriceScreen>
{
	public GameObject m_ScreenGrp;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public ItemPriceGraphScreen m_ItemPriceGraphScreen;

	public Image m_ItemImage;

	public TextMeshProUGUI m_ItemName;

	public TextMeshProUGUI m_AverageCostText;

	public TextMeshProUGUI m_MarketPriceText;

	public TMP_InputField m_SetPriceInput;

	public TextMeshProUGUI m_SetPriceInputDisplay;

	public TextMeshProUGUI m_SetPrice;

	public TextMeshProUGUI m_Profit;

	public CardUI m_SetPriceCardUI;

	private float m_PriceSet;

	private float m_InitPrice;

	private float m_AverageCost;

	private float m_MarketPrice;

	private EItemType m_ItemType = EItemType.None;

	private CardData m_CardData;

	private bool m_IsSetCardPrice = true;

	private ShelfCompartment m_CurrentShelfCompartment;

	private InteractableCardCompartment m_CurrentCardCompartment;

	private void Awake()
	{
		m_ScreenGrp.SetActive(value: false);
	}

	private void Update()
	{
		if (m_ScreenGrp.activeSelf && Input.GetKeyUp(KeyCode.Escape))
		{
			OnPressConfirm();
		}
	}

	public void OpenSetCardPriceScreen(CardData cardData, InteractableCardCompartment cardCompartment)
	{
		m_CurrentCardCompartment = cardCompartment;
		m_IsSetCardPrice = true;
		if (cardData == null || cardData.monsterType == EMonsterType.None)
		{
			Debug.LogError("Card type cannot be none");
			return;
		}
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		m_CardData = cardData;
		m_PriceSet = CPlayerData.GetCardPrice(m_CardData);
		m_InitPrice = m_PriceSet;
		m_AverageCost = 0f;
		m_MarketPrice = CPlayerData.GetCardMarketPrice(m_CardData);
		m_ItemImage.gameObject.SetActive(value: false);
		m_ItemName.gameObject.SetActive(value: false);
		m_SetPriceCardUI.SetCardUI(m_CardData);
		m_SetPriceCardUI.gameObject.SetActive(value: true);
		m_SetPrice.text = GameInstance.GetPriceString(m_PriceSet);
		m_SetPriceInputDisplay.text = GameInstance.GetPriceString(m_PriceSet);
		m_AverageCostText.text = "-";
		m_MarketPriceText.text = GameInstance.GetPriceString(m_MarketPrice);
		m_SetPriceInputDisplay.gameObject.SetActive(value: false);
		EvaluateProfit();
		m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
	}

	public void OpenScreen(EItemType itemType, ShelfCompartment currentShelfCompartment)
	{
		m_CurrentShelfCompartment = currentShelfCompartment;
		m_IsSetCardPrice = false;
		if (itemType == EItemType.None)
		{
			Debug.LogError("Item type cannot be none");
			return;
		}
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: true);
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		m_ItemType = itemType;
		m_PriceSet = CPlayerData.GetItemPrice(itemType, preventZero: false);
		m_InitPrice = m_PriceSet;
		m_AverageCost = CPlayerData.GetAverageItemCost(itemType);
		m_MarketPrice = CPlayerData.GetItemMarketPrice(itemType);
		m_ItemImage.sprite = InventoryBase.GetItemData(itemType).icon;
		m_ItemName.text = InventoryBase.GetItemData(itemType).GetName();
		m_ItemImage.gameObject.SetActive(value: true);
		m_ItemName.gameObject.SetActive(value: true);
		m_SetPriceCardUI.gameObject.SetActive(value: false);
		m_SetPrice.text = GameInstance.GetPriceString(m_PriceSet);
		m_SetPriceInputDisplay.text = GameInstance.GetPriceString(m_PriceSet);
		m_AverageCostText.text = GameInstance.GetPriceString(m_AverageCost);
		m_MarketPriceText.text = GameInstance.GetPriceString(m_MarketPrice);
		m_SetPriceInputDisplay.gameObject.SetActive(value: false);
		EvaluateProfit();
		m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
	}

	public void CloseScreen()
	{
		if (!m_ItemPriceGraphScreen.m_ScreenGroup.activeSelf)
		{
			m_ItemType = EItemType.None;
			m_CardData = null;
			if ((bool)m_CurrentShelfCompartment)
			{
				m_CurrentShelfCompartment.OnFinishSetPrice();
			}
			if ((bool)m_CurrentCardCompartment)
			{
				m_CurrentCardCompartment.OnFinishSetPrice();
			}
			CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
			CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
			m_ScreenGrp.SetActive(value: false);
			SoundManager.GenericMenuClose();
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
		}
	}

	private void EvaluateProfit()
	{
		double num = m_PriceSet - m_AverageCost;
		m_Profit.text = GameInstance.GetPriceString(num);
		if (num > 0.0)
		{
			m_Profit.color = Color.green;
		}
		else if (num < 0.0)
		{
			m_Profit.color = Color.red;
		}
		else
		{
			m_Profit.color = Color.white;
		}
	}

	public void OnPressConfirm()
	{
		if (m_PriceSet != m_InitPrice)
		{
			if (m_IsSetCardPrice)
			{
				CPlayerData.SetCardPrice(m_CardData, m_PriceSet);
			}
			else
			{
				TutorialManager.AddTaskValue(ETutorialTaskCondition.SetItemPrice, 1f);
				CPlayerData.SetItemPrice(m_ItemType, m_PriceSet);
			}
		}
		CloseScreen();
	}

	public void OnInputChanged(string text)
	{
		float num = GameInstance.GetInvariantCultureDecimal(text) / GameInstance.GetCurrencyConversionRate();
		num = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
		m_SetPriceInputDisplay.text = GameInstance.GetPriceString(num);
		m_SetPriceInputDisplay.gameObject.SetActive(value: true);
		m_SetPrice.gameObject.SetActive(value: false);
		EvaluateProfit();
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
		EvaluateProfit();
	}

	public void OnPressSetMarketPrice()
	{
		SoundManager.GenericConfirm();
		OnInputTextUpdated((m_MarketPrice * GameInstance.GetCurrencyConversionRate()).ToString());
	}

	public void OnPressAddPriceTenPercent()
	{
		SoundManager.GenericConfirm();
		if (m_PriceSet <= 0f)
		{
			m_PriceSet = m_MarketPrice;
		}
		OnInputTextUpdated((m_PriceSet * 1.1f * GameInstance.GetCurrencyConversionRate()).ToString());
	}

	public void OnPressReducePriceTenPercent()
	{
		SoundManager.GenericConfirm();
		if (m_PriceSet <= 0f)
		{
			m_PriceSet = m_MarketPrice;
		}
		OnInputTextUpdated((m_PriceSet * 0.9f * GameInstance.GetCurrencyConversionRate()).ToString());
	}

	public void OnPressRoundPriceToFullNumber()
	{
		SoundManager.GenericConfirm();
		if (m_PriceSet <= 0f)
		{
			m_PriceSet = m_MarketPrice;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			OnInputTextUpdated((Mathf.RoundToInt(m_PriceSet * GameInstance.GetCurrencyConversionRate() / 100f) * 100).ToString());
		}
		else
		{
			OnInputTextUpdated(Mathf.RoundToInt(m_PriceSet * GameInstance.GetCurrencyConversionRate()).ToString());
		}
	}

	public void OnPressOpenPriceChartScreen()
	{
		SoundManager.GenericMenuOpen();
		if (m_IsSetCardPrice)
		{
			m_ItemPriceGraphScreen.ShowCardPriceChart(CPlayerData.GetCardSaveIndex(m_CardData), m_CardData.expansionType, m_CardData.isDestiny, m_CardData.cardGrade);
		}
		else
		{
			m_ItemPriceGraphScreen.ShowItemPriceChart(m_ItemType);
		}
		m_ItemPriceGraphScreen.OpenScreen();
	}

	public EItemType GetCurrentSettingPriceItemType()
	{
		return m_ItemType;
	}

	public CardData GetCurrentSettingPriceCardData()
	{
		return m_CardData;
	}
}
