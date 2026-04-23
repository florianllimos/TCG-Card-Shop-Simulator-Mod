using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchUIScreen : CSingleton<WorkbenchUIScreen>
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	public GameObject m_TaskFinishCirlceGrp;

	public Image m_TaskFinishCirlceFillBar;

	public Slider m_SliderPriceLimit;

	public Slider m_SliderPriceMinimum;

	public Slider m_SliderMinCard;

	public TextMeshProUGUI m_PriceLimitText;

	public TextMeshProUGUI m_PriceMinimumText;

	public TextMeshProUGUI m_MinimumCardText;

	public TextMeshProUGUI m_CardExpansionText;

	public TextMeshProUGUI m_RarityLimitText;

	public TextMeshProUGUI m_PriceLimitMinText;

	public TextMeshProUGUI m_PriceLimitMaxText;

	public TextMeshProUGUI m_PriceMinimumMinText;

	public TextMeshProUGUI m_PriceMinimumMaxText;

	public TMP_InputField m_SetMinPriceInput;

	public TextMeshProUGUI m_SetMinPriceInputDisplay;

	public TMP_InputField m_SetMaxPriceInput;

	public TextMeshProUGUI m_SetMaxPriceInputDisplay;

	private bool m_IsWorkingOnTask;

	private bool m_HasRarityLimit;

	private bool m_IgnoreSliderUpdateFunction;

	private bool m_IsEditingDeck;

	private int m_MinimumCardLimit = 4;

	private float m_TaskTime = 5f;

	private float m_TaskTimer;

	private float m_PriceLimit = 0.5f;

	private float m_PriceMinimum = 0.01f;

	private ERarity m_RarityLimit = ERarity.None;

	private ECardExpansionType m_CurrentCardExpansionType;

	private InteractableWorkbench m_CurrentInteractableWorkbench;

	private float m_MinPriceMinimum = 0.01f;

	private float m_MinPriceMaximum = 20f;

	private float m_MaxPriceMinimum = 0.01f;

	private float m_MaxPriceMaximum = 20f;

	private void Update()
	{
		if (m_IsWorkingOnTask)
		{
			m_TaskTimer += Time.deltaTime;
			m_TaskFinishCirlceFillBar.fillAmount = Mathf.Lerp(0f, 1f, m_TaskTimer / m_TaskTime);
			if (m_TaskTimer >= m_TaskTime)
			{
				m_TaskTimer = 0f;
				m_IsWorkingOnTask = false;
				OnTaskCompleted();
			}
		}
	}

	private void OnTaskCompleted()
	{
		m_CurrentInteractableWorkbench.OnTaskCompleted(m_CurrentCardExpansionType);
		m_SliderPriceLimit.interactable = true;
		m_SliderPriceMinimum.interactable = true;
		m_SliderMinCard.interactable = true;
		m_TaskFinishCirlceGrp.SetActive(value: false);
		CloseScreen(playSound: false);
	}

	public static void OpenScreen(InteractableWorkbench interactableWorkbench)
	{
		if (CSingleton<WorkbenchUIScreen>.Instance.m_IsEditingDeck)
		{
			return;
		}
		if (CPlayerData.m_WorkbenchPriceLimit > 0f)
		{
			CSingleton<WorkbenchUIScreen>.Instance.m_MinimumCardLimit = CPlayerData.m_WorkbenchMinimumCardLimit;
			CSingleton<WorkbenchUIScreen>.Instance.m_PriceLimit = CPlayerData.m_WorkbenchPriceLimit;
			if (CPlayerData.m_WorkbenchPriceMinimum > 0f)
			{
				CSingleton<WorkbenchUIScreen>.Instance.m_PriceMinimum = CPlayerData.m_WorkbenchPriceMinimum;
			}
			CSingleton<WorkbenchUIScreen>.Instance.m_RarityLimit = CPlayerData.m_WorkbenchRarityLimit;
			CSingleton<WorkbenchUIScreen>.Instance.m_CurrentCardExpansionType = CPlayerData.m_WorkbenchCardExpansionType;
		}
		CSingleton<WorkbenchUIScreen>.Instance.m_IgnoreSliderUpdateFunction = true;
		CSingleton<WorkbenchUIScreen>.Instance.m_SliderPriceLimit.value = CPlayerData.m_WorkbenchPriceLimit * 100f;
		CSingleton<WorkbenchUIScreen>.Instance.m_SliderPriceMinimum.value = CPlayerData.m_WorkbenchPriceMinimum * 100f;
		CSingleton<WorkbenchUIScreen>.Instance.m_SliderMinCard.value = (float)CPlayerData.m_WorkbenchMinimumCardLimit / 10f * 10f;
		CSingleton<WorkbenchUIScreen>.Instance.m_PriceLimitMinText.text = GameInstance.GetPriceString(CSingleton<WorkbenchUIScreen>.Instance.m_MinPriceMinimum);
		CSingleton<WorkbenchUIScreen>.Instance.m_PriceLimitMaxText.text = GameInstance.GetPriceString(CSingleton<WorkbenchUIScreen>.Instance.m_MinPriceMaximum);
		CSingleton<WorkbenchUIScreen>.Instance.m_PriceMinimumMinText.text = GameInstance.GetPriceString(CSingleton<WorkbenchUIScreen>.Instance.m_MaxPriceMinimum);
		CSingleton<WorkbenchUIScreen>.Instance.m_PriceMinimumMaxText.text = GameInstance.GetPriceString(CSingleton<WorkbenchUIScreen>.Instance.m_MaxPriceMaximum);
		CSingleton<WorkbenchUIScreen>.Instance.m_IgnoreSliderUpdateFunction = false;
		CSingleton<WorkbenchUIScreen>.Instance.m_MinimumCardLimit = CPlayerData.m_WorkbenchMinimumCardLimit;
		CSingleton<WorkbenchUIScreen>.Instance.m_CardExpansionText.text = InventoryBase.GetCardExpansionName(CSingleton<WorkbenchUIScreen>.Instance.m_CurrentCardExpansionType);
		if (CSingleton<WorkbenchUIScreen>.Instance.m_HasRarityLimit)
		{
			CSingleton<WorkbenchUIScreen>.Instance.m_RarityLimitText.text = LocalizationManager.GetTranslation(CSingleton<WorkbenchUIScreen>.Instance.m_RarityLimit.ToString());
		}
		else
		{
			CSingleton<WorkbenchUIScreen>.Instance.m_RarityLimitText.text = LocalizationManager.GetTranslation("Any Rarity");
		}
		CSingleton<WorkbenchUIScreen>.Instance.m_PriceLimitText.text = GameInstance.GetPriceString(CSingleton<WorkbenchUIScreen>.Instance.m_PriceLimit);
		CSingleton<WorkbenchUIScreen>.Instance.m_PriceMinimumText.text = GameInstance.GetPriceString(CSingleton<WorkbenchUIScreen>.Instance.m_PriceMinimum);
		CSingleton<WorkbenchUIScreen>.Instance.m_SetMaxPriceInputDisplay.text = CSingleton<WorkbenchUIScreen>.Instance.m_PriceLimitText.text;
		CSingleton<WorkbenchUIScreen>.Instance.m_SetMinPriceInputDisplay.text = CSingleton<WorkbenchUIScreen>.Instance.m_PriceMinimumText.text;
		CSingleton<WorkbenchUIScreen>.Instance.m_SetMaxPriceInputDisplay.gameObject.SetActive(value: false);
		CSingleton<WorkbenchUIScreen>.Instance.m_SetMinPriceInputDisplay.gameObject.SetActive(value: false);
		CSingleton<WorkbenchUIScreen>.Instance.m_MinimumCardText.text = CSingleton<WorkbenchUIScreen>.Instance.m_MinimumCardLimit.ToString();
		CSingleton<WorkbenchUIScreen>.Instance.m_TaskFinishCirlceGrp.SetActive(value: false);
		CSingleton<WorkbenchUIScreen>.Instance.m_CurrentInteractableWorkbench = interactableWorkbench;
		CSingleton<WorkbenchUIScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(CSingleton<WorkbenchUIScreen>.Instance.m_ControllerScreenUIExtension);
		TutorialManager.SetGameUIVisible(isVisible: false);
	}

	public void CloseScreen(bool playSound)
	{
		if (!CSingleton<WorkbenchUIScreen>.Instance.m_IsEditingDeck && !InputManager.IsSliderActive() && !m_IsWorkingOnTask)
		{
			if (playSound)
			{
				SoundManager.GenericMenuClose();
			}
			m_PriceLimitText.text = GameInstance.GetPriceString(m_PriceLimit);
			m_PriceMinimumText.text = GameInstance.GetPriceString(m_PriceMinimum);
			m_MinimumCardText.text = m_MinimumCardLimit.ToString();
			m_CardExpansionText.text = InventoryBase.GetCardExpansionName(m_CurrentCardExpansionType);
			if (m_HasRarityLimit)
			{
				m_RarityLimitText.text = LocalizationManager.GetTranslation(m_RarityLimit.ToString());
			}
			else
			{
				m_RarityLimitText.text = LocalizationManager.GetTranslation("Any Rarity");
			}
			CSingleton<WorkbenchUIScreen>.Instance.m_CurrentInteractableWorkbench.OnPressEsc();
			m_ScreenGrp.SetActive(value: false);
			ControllerScreenUIExtManager.OnCloseScreen(CSingleton<WorkbenchUIScreen>.Instance.m_ControllerScreenUIExtension);
			TutorialManager.SetGameUIVisible(isVisible: true);
		}
	}

	public void OpenBundleCardScreen()
	{
		if (CSingleton<WorkbenchUIScreen>.Instance.m_IsEditingDeck || m_IsWorkingOnTask)
		{
			return;
		}
		bool flag = false;
		List<InteractableWorkbench> workbenchList = CSingleton<ShelfManager>.Instance.m_WorkbenchList;
		for (int i = 0; i < workbenchList.Count; i++)
		{
			if (workbenchList[i].HasEnoughSlot())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			EItemType bulkBoxItemType = m_CurrentInteractableWorkbench.GetBulkBoxItemType(m_CurrentCardExpansionType, 0f);
			if (ShelfManager.GetShelfListToRestockItem(bulkBoxItemType).Count > 0)
			{
				flag = true;
			}
			else if (RestockManager.GetItemPackagingBoxListWithSpaceForItem(bulkBoxItemType).Count > 0)
			{
				flag = true;
			}
		}
		SoundManager.GenericConfirm();
		if (flag)
		{
			RunBundleCardBulkFunction();
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NoSpaceToStartBundle);
		}
	}

	public void OnPressChangeCardExpannsionButton()
	{
		if (!CSingleton<WorkbenchUIScreen>.Instance.m_IsEditingDeck && !m_IsWorkingOnTask)
		{
			SoundManager.GenericMenuOpen();
			CardExpansionSelectScreen.OpenScreen(m_CurrentCardExpansionType);
			CEventManager.AddListener<CEventPlayer_OnCardExpansionSelectScreenUpdated>(OnCardExpansionUpdated);
		}
	}

	protected void OnCardExpansionUpdated(CEventPlayer_OnCardExpansionSelectScreenUpdated evt)
	{
		if (!CSingleton<WorkbenchUIScreen>.Instance.m_IsEditingDeck)
		{
			m_CurrentCardExpansionType = (ECardExpansionType)evt.m_CardExpansionTypeIndex;
			CPlayerData.m_WorkbenchCardExpansionType = m_CurrentCardExpansionType;
			m_CardExpansionText.text = InventoryBase.GetCardExpansionName(m_CurrentCardExpansionType);
			CEventManager.RemoveListener<CEventPlayer_OnCardExpansionSelectScreenUpdated>(OnCardExpansionUpdated);
		}
	}

	public void OnPressEditDeckButton()
	{
		if (!CSingleton<WorkbenchUIScreen>.Instance.m_IsEditingDeck && !m_IsWorkingOnTask)
		{
			m_IsEditingDeck = true;
			m_CurrentInteractableWorkbench.SetIsEditingDeck(isEditingDeck: true);
			PlayCardGameManager.OpenDeckListScreen();
		}
	}

	public void OnCloseEditDeckScreen()
	{
		m_IsEditingDeck = false;
		m_CurrentInteractableWorkbench.SetIsEditingDeck(isEditingDeck: false);
	}

	public void OnPressChangeRarityButton()
	{
		if (!CSingleton<WorkbenchUIScreen>.Instance.m_IsEditingDeck && !m_IsWorkingOnTask)
		{
			SoundManager.GenericMenuOpen();
			CardRaritySelectScreen.OpenScreen(m_RarityLimit);
			CEventManager.AddListener<CEventPlayer_OnCardRaritySelectScreenUpdated>(OnRarityLimitUpdated);
		}
	}

	public void OnRarityLimitUpdated(CEventPlayer_OnCardRaritySelectScreenUpdated evt)
	{
		if (evt.m_CardRarityIndex == -1)
		{
			m_HasRarityLimit = false;
		}
		else
		{
			m_HasRarityLimit = true;
			m_RarityLimit = (ERarity)evt.m_CardRarityIndex;
		}
		CPlayerData.m_WorkbenchRarityLimit = m_RarityLimit;
		if (m_HasRarityLimit)
		{
			m_RarityLimitText.text = LocalizationManager.GetTranslation(m_RarityLimit.ToString());
		}
		else
		{
			m_RarityLimitText.text = LocalizationManager.GetTranslation("Any Rarity");
		}
		CEventManager.RemoveListener<CEventPlayer_OnCardRaritySelectScreenUpdated>(OnRarityLimitUpdated);
	}

	public void OnSliderValueChanged_PriceLimit()
	{
		if (!m_IgnoreSliderUpdateFunction)
		{
			m_PriceLimit = (float)Mathf.RoundToInt(m_SliderPriceLimit.value) / 100f;
			m_PriceLimitText.text = GameInstance.GetPriceString(m_PriceLimit);
			CPlayerData.m_WorkbenchPriceLimit = m_PriceLimit;
		}
	}

	public void OnSliderValueChanged_PriceMinimum()
	{
		if (!m_IgnoreSliderUpdateFunction)
		{
			m_PriceMinimum = (float)Mathf.RoundToInt(m_SliderPriceMinimum.value) / 100f;
			m_PriceMinimumText.text = GameInstance.GetPriceString(m_PriceMinimum);
			CPlayerData.m_WorkbenchPriceMinimum = m_PriceMinimum;
		}
	}

	public void OnSliderValueChanged_MinimumCard()
	{
		if (!m_IgnoreSliderUpdateFunction)
		{
			m_MinimumCardLimit = Mathf.CeilToInt(m_SliderMinCard.value);
			m_MinimumCardText.text = m_MinimumCardLimit.ToString();
			CPlayerData.m_WorkbenchMinimumCardLimit = m_MinimumCardLimit;
		}
	}

	private void RunBundleCardBulkFunction()
	{
		int num = 0;
		ECardExpansionType currentCardExpansionType = m_CurrentCardExpansionType;
		bool flag = false;
		int num2 = 0;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < InventoryBase.GetShownMonsterList(currentCardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(currentCardExpansionType); i++)
		{
			num = i;
			EMonsterType monsterTypeFromCardSaveIndex = CPlayerData.GetMonsterTypeFromCardSaveIndex(num, currentCardExpansionType);
			float cardMarketPrice = CPlayerData.GetCardMarketPrice(num, currentCardExpansionType, flag, 0);
			ERarity rarity = InventoryBase.GetMonsterData(monsterTypeFromCardSaveIndex).Rarity;
			int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, currentCardExpansionType, flag);
			if (!(cardMarketPrice >= m_PriceLimit) && !(cardMarketPrice < m_PriceMinimum) && (!m_HasRarityLimit || m_RarityLimit == rarity) && cardAmountByIndex > CPlayerData.m_WorkbenchMinimumCardLimit)
			{
				list.Add(num);
				list2.Add(cardAmountByIndex - CPlayerData.m_WorkbenchMinimumCardLimit);
				num2 += cardAmountByIndex - CPlayerData.m_WorkbenchMinimumCardLimit;
			}
		}
		if (num2 < 100)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NotEnoughCardForBundle);
			return;
		}
		m_SliderPriceLimit.interactable = false;
		m_SliderPriceMinimum.interactable = false;
		m_SliderMinCard.interactable = false;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		float num6 = 0f;
		List<CardData> list3 = new List<CardData>();
		while (num3 < 100)
		{
			for (int j = 0; j < list.Count; j++)
			{
				num5 = Mathf.Clamp(Random.Range(1, list2[j] + 5), 1, 10);
				if (num5 > list2[j])
				{
					num5 = list2[j];
				}
				if (num5 > 100 - num3)
				{
					num5 = 100 - num3;
				}
				num3 += num5;
				list2[j] -= num5;
				if (list3.Count < 10)
				{
					list3.Add(CPlayerData.GetCardData(list[j], currentCardExpansionType, flag));
				}
				num6 += CPlayerData.GetCardMarketPrice(list[j], currentCardExpansionType, flag, 0) * (float)num5;
				CPlayerData.ReduceCardUsingIndex(list[j], currentCardExpansionType, flag, num5);
				if (num3 >= 100)
				{
					break;
				}
			}
			num4++;
			if (num4 > 10000)
			{
				break;
			}
		}
		m_CurrentCardExpansionType = currentCardExpansionType;
		m_CurrentInteractableWorkbench.PlayBundlingCardBoxSequence(list3, currentCardExpansionType, num6);
		m_IsWorkingOnTask = true;
		m_TaskFinishCirlceGrp.SetActive(value: true);
	}

	public void OnInputChangedMinPrice(string text)
	{
		float num = GameInstance.GetInvariantCultureDecimal(text) / GameInstance.GetCurrencyConversionRate();
		num = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
		m_SetMinPriceInputDisplay.text = GameInstance.GetPriceString(num);
		m_SetMinPriceInputDisplay.gameObject.SetActive(value: true);
		m_PriceMinimumText.gameObject.SetActive(value: false);
	}

	public void OnInputTextSelectedMinPrice(string text)
	{
		m_SetMinPriceInputDisplay.gameObject.SetActive(value: true);
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			m_SetMinPriceInput.text = GameInstance.GetPriceString(m_PriceMinimum, useDashAsZero: false, useCurrencySymbol: false, useCentSymbol: false, "F0");
		}
		else
		{
			m_SetMinPriceInput.text = GameInstance.GetPriceString(m_PriceMinimum, useDashAsZero: false, useCurrencySymbol: false);
		}
		m_PriceMinimumText.gameObject.SetActive(value: false);
	}

	public void OnInputTextUpdatedMinPrice(string text)
	{
		float num = GameInstance.GetInvariantCultureDecimal(text) / GameInstance.GetCurrencyConversionRate();
		m_PriceMinimum = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
		if (m_PriceMinimum < CSingleton<WorkbenchUIScreen>.Instance.m_MinPriceMinimum)
		{
			m_PriceMinimum = CSingleton<WorkbenchUIScreen>.Instance.m_MinPriceMinimum;
		}
		if (m_PriceMinimum > CSingleton<WorkbenchUIScreen>.Instance.m_MinPriceMaximum)
		{
			m_PriceMinimum = CSingleton<WorkbenchUIScreen>.Instance.m_MinPriceMaximum;
		}
		CPlayerData.m_WorkbenchPriceMinimum = m_PriceMinimum;
		CSingleton<WorkbenchUIScreen>.Instance.m_SliderPriceMinimum.value = CPlayerData.m_WorkbenchPriceMinimum * 100f;
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			m_SetMinPriceInput.text = GameInstance.GetPriceString(m_PriceMinimum, useDashAsZero: false, useCurrencySymbol: false, useCentSymbol: false, "F0");
		}
		else
		{
			m_SetMinPriceInput.text = GameInstance.GetPriceString(m_PriceMinimum, useDashAsZero: false, useCurrencySymbol: false);
		}
		m_SetMinPriceInputDisplay.text = GameInstance.GetPriceString(m_PriceMinimum);
		m_PriceMinimumText.text = GameInstance.GetPriceString(m_PriceMinimum);
		m_SetMinPriceInputDisplay.gameObject.SetActive(value: false);
		m_PriceMinimumText.gameObject.SetActive(value: true);
	}

	public void OnInputChangedMaxPrice(string text)
	{
		float num = GameInstance.GetInvariantCultureDecimal(text) / GameInstance.GetCurrencyConversionRate();
		num = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
		m_SetMaxPriceInputDisplay.text = GameInstance.GetPriceString(num);
		m_SetMaxPriceInputDisplay.gameObject.SetActive(value: true);
		m_PriceLimitText.gameObject.SetActive(value: false);
	}

	public void OnInputTextSelectedMaxPrice(string text)
	{
		m_SetMaxPriceInputDisplay.gameObject.SetActive(value: true);
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			m_SetMaxPriceInput.text = GameInstance.GetPriceString(m_PriceLimit, useDashAsZero: false, useCurrencySymbol: false, useCentSymbol: false, "F0");
		}
		else
		{
			m_SetMaxPriceInput.text = GameInstance.GetPriceString(m_PriceLimit, useDashAsZero: false, useCurrencySymbol: false);
		}
		m_PriceLimitText.gameObject.SetActive(value: false);
	}

	public void OnInputTextUpdatedMaxPrice(string text)
	{
		float num = GameInstance.GetInvariantCultureDecimal(text) / GameInstance.GetCurrencyConversionRate();
		m_PriceLimit = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
		if (m_PriceLimit < CSingleton<WorkbenchUIScreen>.Instance.m_MaxPriceMinimum)
		{
			m_PriceLimit = CSingleton<WorkbenchUIScreen>.Instance.m_MaxPriceMinimum;
		}
		if (m_PriceLimit > CSingleton<WorkbenchUIScreen>.Instance.m_MaxPriceMaximum)
		{
			m_PriceLimit = CSingleton<WorkbenchUIScreen>.Instance.m_MaxPriceMaximum;
		}
		CPlayerData.m_WorkbenchPriceLimit = m_PriceLimit;
		CSingleton<WorkbenchUIScreen>.Instance.m_SliderPriceLimit.value = CPlayerData.m_WorkbenchPriceLimit * 100f;
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			m_SetMaxPriceInput.text = GameInstance.GetPriceString(m_PriceLimit, useDashAsZero: false, useCurrencySymbol: false, useCentSymbol: false, "F0");
		}
		else
		{
			m_SetMaxPriceInput.text = GameInstance.GetPriceString(m_PriceLimit, useDashAsZero: false, useCurrencySymbol: false);
		}
		m_SetMaxPriceInputDisplay.text = GameInstance.GetPriceString(m_PriceLimit);
		m_PriceLimitText.text = GameInstance.GetPriceString(m_PriceLimit);
		m_SetMaxPriceInputDisplay.gameObject.SetActive(value: false);
		m_PriceLimitText.gameObject.SetActive(value: true);
	}
}
