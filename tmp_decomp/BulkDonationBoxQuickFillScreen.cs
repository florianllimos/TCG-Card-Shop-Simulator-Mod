using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulkDonationBoxQuickFillScreen : MonoBehaviour
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

	private int m_MinimumCardLimit = 4;

	private int m_MaxCapacity;

	private int m_MaxTypeCount;

	private float m_TaskTime = 5f;

	private float m_TaskTimer;

	private float m_PriceLimit = 1f;

	private float m_PriceMinimum = 0.01f;

	private ERarity m_RarityLimit = ERarity.None;

	private ECardExpansionType m_CurrentCardExpansionType;

	private BulkDonationBoxUIScreen m_BulkDonationBoxUIScreen;

	public List<CompactCardDataAmount> m_CompactCardDataList = new List<CompactCardDataAmount>();

	private float m_MinPriceMinimum = 0.01f;

	private float m_MinPriceMaximum = 1000f;

	private float m_MaxPriceMinimum = 0.01f;

	private float m_MaxPriceMaximum = 1000f;

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
		m_BulkDonationBoxUIScreen.QuickFillOutputCompactCardData(m_CompactCardDataList);
		m_SliderPriceLimit.interactable = true;
		m_SliderPriceMinimum.interactable = true;
		m_SliderMinCard.interactable = true;
		m_TaskFinishCirlceGrp.SetActive(value: false);
		CloseScreen(playSound: false);
	}

	public void OpenScreen(BulkDonationBoxUIScreen bulkDonationBoxUIScreen, int capacity, int maxTypeCount)
	{
		if (CPlayerData.m_QuickFillPriceLimit > 0f)
		{
			m_MinimumCardLimit = CPlayerData.m_QuickFillMinimumCardLimit;
			m_PriceLimit = CPlayerData.m_QuickFillPriceLimit;
			if (CPlayerData.m_QuickFillPriceMinimum > 0f)
			{
				m_PriceMinimum = CPlayerData.m_QuickFillPriceMinimum;
			}
			m_RarityLimit = CPlayerData.m_QuickFillRarityLimit;
			m_CurrentCardExpansionType = CPlayerData.m_QuickFillCardExpansionType;
		}
		m_IgnoreSliderUpdateFunction = true;
		m_MaxCapacity = capacity;
		m_MaxTypeCount = maxTypeCount;
		m_SliderPriceLimit.value = CPlayerData.m_QuickFillPriceLimit * 100f;
		m_SliderPriceMinimum.value = CPlayerData.m_QuickFillPriceMinimum * 100f;
		m_SliderMinCard.value = (float)CPlayerData.m_QuickFillMinimumCardLimit / 10f * 10f;
		m_PriceLimitMinText.text = GameInstance.GetPriceString(m_MinPriceMinimum);
		m_PriceLimitMaxText.text = GameInstance.GetPriceString(m_MinPriceMaximum);
		m_PriceMinimumMinText.text = GameInstance.GetPriceString(m_MaxPriceMinimum);
		m_PriceMinimumMaxText.text = GameInstance.GetPriceString(m_MaxPriceMaximum);
		m_IgnoreSliderUpdateFunction = false;
		m_MinimumCardLimit = CPlayerData.m_QuickFillMinimumCardLimit;
		m_CardExpansionText.text = InventoryBase.GetCardExpansionName(m_CurrentCardExpansionType);
		if (m_HasRarityLimit)
		{
			m_RarityLimitText.text = LocalizationManager.GetTranslation(m_RarityLimit.ToString());
		}
		else
		{
			m_RarityLimitText.text = LocalizationManager.GetTranslation("Any Rarity");
		}
		m_PriceLimitText.text = GameInstance.GetPriceString(m_PriceLimit);
		m_PriceMinimumText.text = GameInstance.GetPriceString(m_PriceMinimum);
		m_SetMaxPriceInputDisplay.text = m_PriceLimitText.text;
		m_SetMinPriceInputDisplay.text = m_PriceMinimumText.text;
		m_SetMaxPriceInputDisplay.gameObject.SetActive(value: false);
		m_SetMinPriceInputDisplay.gameObject.SetActive(value: false);
		m_MinimumCardText.text = m_MinimumCardLimit.ToString();
		m_TaskFinishCirlceGrp.SetActive(value: false);
		m_BulkDonationBoxUIScreen = bulkDonationBoxUIScreen;
		m_CompactCardDataList.Clear();
		m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
		TutorialManager.SetGameUIVisible(isVisible: false);
	}

	public void CloseScreen(bool playSound)
	{
		if (!InputManager.IsSliderActive() && !m_IsWorkingOnTask)
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
			m_ScreenGrp.SetActive(value: false);
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
			TutorialManager.SetGameUIVisible(isVisible: true);
		}
	}

	public void OnPressQuickFillButton()
	{
		RunBundleCardBulkFunction();
	}

	public void OnPressChangeCardExpannsionButton()
	{
		if (!m_IsWorkingOnTask)
		{
			SoundManager.GenericMenuOpen();
			CardExpansionSelectScreen.OpenScreen(m_CurrentCardExpansionType);
			CEventManager.AddListener<CEventPlayer_OnCardExpansionSelectScreenUpdated>(OnCardExpansionUpdated);
		}
	}

	protected void OnCardExpansionUpdated(CEventPlayer_OnCardExpansionSelectScreenUpdated evt)
	{
		m_CurrentCardExpansionType = (ECardExpansionType)evt.m_CardExpansionTypeIndex;
		CPlayerData.m_QuickFillCardExpansionType = m_CurrentCardExpansionType;
		m_CardExpansionText.text = InventoryBase.GetCardExpansionName(m_CurrentCardExpansionType);
		CEventManager.RemoveListener<CEventPlayer_OnCardExpansionSelectScreenUpdated>(OnCardExpansionUpdated);
	}

	public void OnPressChangeRarityButton()
	{
		if (!m_IsWorkingOnTask)
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
		CPlayerData.m_QuickFillRarityLimit = m_RarityLimit;
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
			CPlayerData.m_QuickFillPriceLimit = m_PriceLimit;
		}
	}

	public void OnSliderValueChanged_PriceMinimum()
	{
		if (!m_IgnoreSliderUpdateFunction)
		{
			m_PriceMinimum = (float)Mathf.RoundToInt(m_SliderPriceMinimum.value) / 100f;
			m_PriceMinimumText.text = GameInstance.GetPriceString(m_PriceMinimum);
			CPlayerData.m_QuickFillPriceMinimum = m_PriceMinimum;
		}
	}

	public void OnSliderValueChanged_MinimumCard()
	{
		if (!m_IgnoreSliderUpdateFunction)
		{
			m_MinimumCardLimit = Mathf.CeilToInt(m_SliderMinCard.value);
			m_MinimumCardText.text = m_MinimumCardLimit.ToString();
			CPlayerData.m_QuickFillMinimumCardLimit = m_MinimumCardLimit;
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
			if (cardMarketPrice >= m_PriceLimit || cardMarketPrice < m_PriceMinimum || (m_HasRarityLimit && m_RarityLimit != rarity) || cardAmountByIndex <= CPlayerData.m_QuickFillMinimumCardLimit)
			{
				continue;
			}
			int num3 = cardAmountByIndex - CPlayerData.m_QuickFillMinimumCardLimit;
			bool flag2 = false;
			for (int j = 0; j < list2.Count; j++)
			{
				if (num3 > list2[j])
				{
					flag2 = true;
					list.Insert(j, num);
					list2.Insert(j, num3);
					num2 += num3;
					break;
				}
			}
			if (!flag2)
			{
				list.Add(num);
				list2.Add(num3);
				num2 += num3;
			}
		}
		m_SliderPriceLimit.interactable = false;
		m_SliderPriceMinimum.interactable = false;
		m_SliderMinCard.interactable = false;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int maxCapacity = m_MaxCapacity;
		new List<CardData>();
		m_CompactCardDataList = new List<CompactCardDataAmount>();
		while (num4 < maxCapacity)
		{
			for (int k = 0; k < list.Count; k++)
			{
				num6 = Mathf.Clamp(Random.Range(1, list2[k] + 5), 1, 1000);
				if (num6 > list2[k])
				{
					num6 = list2[k];
				}
				if (num6 > maxCapacity - num4)
				{
					num6 = maxCapacity - num4;
				}
				num4 += num6;
				list2[k] -= num6;
				CompactCardDataAmount compactCardDataAmount = new CompactCardDataAmount();
				compactCardDataAmount.cardSaveIndex = list[k];
				compactCardDataAmount.expansionType = currentCardExpansionType;
				compactCardDataAmount.isDestiny = flag;
				compactCardDataAmount.gradedCardIndex = 0;
				compactCardDataAmount.amount = num6;
				m_CompactCardDataList.Add(compactCardDataAmount);
				CPlayerData.ReduceCardUsingIndex(list[k], currentCardExpansionType, flag, num6);
				if (m_CompactCardDataList.Count >= m_MaxTypeCount || num4 >= maxCapacity)
				{
					break;
				}
			}
			num5++;
			if (num5 > 10000)
			{
				break;
			}
		}
		m_CurrentCardExpansionType = currentCardExpansionType;
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
		if (m_PriceMinimum < m_MinPriceMinimum)
		{
			m_PriceMinimum = m_MinPriceMinimum;
		}
		if (m_PriceMinimum > m_MinPriceMaximum)
		{
			m_PriceMinimum = m_MinPriceMaximum;
		}
		CPlayerData.m_QuickFillPriceMinimum = m_PriceMinimum;
		m_SliderPriceMinimum.value = CPlayerData.m_QuickFillPriceMinimum * 100f;
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
		if (m_PriceLimit < m_MaxPriceMinimum)
		{
			m_PriceLimit = m_MaxPriceMinimum;
		}
		if (m_PriceLimit > m_MaxPriceMaximum)
		{
			m_PriceLimit = m_MaxPriceMaximum;
		}
		CPlayerData.m_QuickFillPriceLimit = m_PriceLimit;
		m_SliderPriceLimit.value = CPlayerData.m_QuickFillPriceLimit * 100f;
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
