using TMPro;
using UnityEngine;

public class SetGameEventPriceScreen : UIScreenBase
{
	public TextMeshProUGUI m_MarketPriceText;

	public TMP_InputField m_SetPriceInput;

	public TextMeshProUGUI m_SetPriceInputDisplay;

	public TextMeshProUGUI m_SetPrice;

	private float m_PriceSet;

	private float m_MarketPrice;

	private EGameEventFormat m_GameEventFormat;

	protected override void OnOpenScreen()
	{
		EGameEventFormat eGameEventFormat = CPlayerData.m_GameEventFormat;
		if (CPlayerData.m_PendingGameEventFormat != EGameEventFormat.None)
		{
			eGameEventFormat = CPlayerData.m_PendingGameEventFormat;
		}
		if (eGameEventFormat == EGameEventFormat.None)
		{
			Debug.LogError("Event type cannot be none");
			return;
		}
		m_GameEventFormat = eGameEventFormat;
		m_PriceSet = PriceChangeManager.GetGameEventPrice(m_GameEventFormat, preventZero: false);
		m_MarketPrice = PriceChangeManager.GetGameEventMarketPrice(m_GameEventFormat);
		m_SetPrice.text = GameInstance.GetPriceString(m_PriceSet);
		m_SetPriceInputDisplay.text = GameInstance.GetPriceString(m_PriceSet);
		m_MarketPriceText.text = GameInstance.GetPriceString(m_MarketPrice);
		m_SetPriceInputDisplay.gameObject.SetActive(value: false);
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
	}

	public void OnPressConfirm()
	{
		PriceChangeManager.SetGameEventPrice(m_GameEventFormat, m_PriceSet);
		CloseScreen();
		SoundManager.GenericConfirm();
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
