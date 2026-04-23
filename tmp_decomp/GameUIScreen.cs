using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIScreen : CSingleton<GameUIScreen>
{
	public CanvasGroup m_CanvasGrp;

	public GameObject m_AutoSavingGrp;

	public GameObject m_AutoSavingSucessGrp;

	public GameObject m_AutoSavingFailedGrp;

	public TextMeshProUGUI m_CoinText;

	public TextMeshProUGUI m_TimeText;

	public TextMeshProUGUI m_DayText;

	public TextMeshProUGUI m_ShopLevelText;

	public TextMeshProUGUI m_ShopEXPText;

	public TextMeshProUGUI m_AddMoneyText;

	public TextMeshProUGUI m_AddShopExpText;

	public TextMeshProUGUI m_GoNextDayText;

	public Color m_AddMoneyColor;

	public Color m_ReduceMoneyColor;

	public Image m_ShopExpBar;

	public Image m_DeodorantBar;

	public GameObject m_DeodorantBarGrp;

	public GameObject m_PressEnterGoNextDayIndicator;

	public GameObject m_PressEnterGoNextDayIndicatorText;

	public InputTooltipUI m_PhoneTooltipUI;

	public InputTooltipUI m_AlbumTooltipUI;

	public InputTooltipUI m_DecoInventoryTooltipUI;

	private double m_CurrentCoinDouble;

	private double m_FinalCoinDouble;

	private float m_CurrentCoin;

	private float m_FinalCoin;

	private float m_CoinLerpAmount;

	private bool m_IsShowingCanvasGrpAlpha;

	private bool m_IsHidingCanvasGrpAlpha;

	private bool m_IsPlayingAddCoinSFX;

	private bool m_IsTooltipVisible;

	private float m_PlayAddCoinSFXTimer;

	private float m_AddShopExpPopupTimer;

	private float m_AddMoneyPopupTimer;

	private float m_CanvasGrpAlphaLerpTimer;

	private List<int> m_AddShopExpPopupList = new List<int>();

	private List<float> m_AddMoneyPopupList = new List<float>();

	private void Awake()
	{
		m_AddMoneyText.gameObject.SetActive(value: false);
		m_AddShopExpText.gameObject.SetActive(value: false);
		m_PressEnterGoNextDayIndicator.SetActive(value: false);
		m_DeodorantBarGrp.SetActive(value: false);
		if (m_GoNextDayText != null)
		{
			Localize component = m_GoNextDayText.GetComponent<Localize>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		UpdateGoNextDayText();
	}

	private void Init()
	{
		m_CurrentCoin = CPlayerData.m_CoinAmount;
		m_CurrentCoinDouble = CPlayerData.m_CoinAmountDouble;
		m_FinalCoin = m_CurrentCoin;
		m_FinalCoinDouble = m_CurrentCoinDouble;
		m_CoinText.text = GameInstance.GetPriceString(m_CurrentCoinDouble);
		m_DayText.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (CPlayerData.m_CurrentDay + 1).ToString());
		EvaluateShopLevelAndExp();
	}

	private void Update()
	{
		EvaluateAddMoneyPopup();
		EvaluateAddShopExpPopup();
		m_TimeText.text = CSingleton<LightManager>.Instance.m_TimeString;
		if (m_IsShowingCanvasGrpAlpha)
		{
			m_CanvasGrpAlphaLerpTimer += Time.deltaTime * 2f;
			m_CanvasGrp.alpha = Mathf.Lerp(0f, 1f, m_CanvasGrpAlphaLerpTimer);
			if (m_CanvasGrpAlphaLerpTimer >= 1f)
			{
				m_IsShowingCanvasGrpAlpha = false;
				m_CanvasGrpAlphaLerpTimer = 1f;
			}
		}
		else if (m_IsHidingCanvasGrpAlpha)
		{
			m_CanvasGrpAlphaLerpTimer -= Time.deltaTime * 2f;
			m_CanvasGrp.alpha = Mathf.Lerp(0f, 1f, m_CanvasGrpAlphaLerpTimer);
			if (m_CanvasGrpAlphaLerpTimer <= 0f)
			{
				m_IsShowingCanvasGrpAlpha = false;
				m_CanvasGrpAlphaLerpTimer = 0f;
			}
		}
		if (m_IsPlayingAddCoinSFX)
		{
			m_PlayAddCoinSFXTimer += Time.deltaTime;
			if (m_PlayAddCoinSFXTimer > 0.05f)
			{
				SoundManager.SetEnableSound_CoinIncrease(isEnable: false);
				m_IsPlayingAddCoinSFX = false;
				m_PlayAddCoinSFXTimer = 0f;
			}
		}
		if (!GameInstance.m_StopCoinGemTextLerp)
		{
			_ = GameInstance.m_StopCoinGemTextLerp;
		}
	}

	private void EvaluateAddMoneyPopup()
	{
		if (m_AddMoneyPopupList.Count > 0)
		{
			m_AddMoneyPopupTimer += Time.deltaTime;
			if (m_AddMoneyPopupTimer >= 1f)
			{
				m_AddMoneyPopupTimer = 0f;
				m_AddMoneyText.gameObject.SetActive(value: false);
				if (m_AddMoneyPopupList[0] > 0f)
				{
					m_AddMoneyText.text = "+" + GameInstance.GetPriceString(m_AddMoneyPopupList[0]);
					m_AddMoneyText.color = m_AddMoneyColor;
					m_AddMoneyText.gameObject.SetActive(value: true);
				}
				else if (m_AddMoneyPopupList[0] < 0f)
				{
					m_AddMoneyText.text = "-" + GameInstance.GetPriceString(0f - m_AddMoneyPopupList[0]);
					m_AddMoneyText.color = m_ReduceMoneyColor;
					m_AddMoneyText.gameObject.SetActive(value: true);
				}
				m_AddMoneyPopupList.RemoveAt(0);
			}
		}
		else if (m_AddMoneyText.gameObject.activeSelf)
		{
			m_AddMoneyPopupTimer += Time.deltaTime;
			if (m_AddMoneyPopupTimer >= 2f)
			{
				m_AddMoneyPopupTimer = 0f;
				m_AddMoneyText.gameObject.SetActive(value: false);
			}
		}
	}

	private void EvaluateAddShopExpPopup()
	{
		if (m_AddShopExpPopupList.Count > 0)
		{
			m_AddShopExpPopupTimer += Time.deltaTime;
			if (m_AddShopExpPopupTimer >= 1f)
			{
				m_AddShopExpPopupTimer = 0f;
				m_AddShopExpText.gameObject.SetActive(value: false);
				if (m_AddShopExpPopupList[0] > 0)
				{
					m_AddShopExpText.text = "+" + m_AddShopExpPopupList[0] + " xp";
					m_AddShopExpText.gameObject.SetActive(value: true);
				}
				m_AddShopExpPopupList.RemoveAt(0);
			}
		}
		else if (m_AddShopExpText.gameObject.activeSelf)
		{
			m_AddShopExpPopupTimer += Time.deltaTime;
			if (m_AddShopExpPopupTimer >= 2f)
			{
				m_AddShopExpPopupTimer = 0f;
				m_AddShopExpText.gameObject.SetActive(value: false);
			}
		}
	}

	public void AddCoin(float coinValue, bool noLerp)
	{
		m_FinalCoin += coinValue;
		m_FinalCoinDouble += coinValue;
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			m_FinalCoinDouble = Math.Round(m_FinalCoinDouble, 3, MidpointRounding.AwayFromZero);
		}
		else
		{
			m_FinalCoinDouble = Math.Round(m_FinalCoinDouble, 2, MidpointRounding.AwayFromZero);
		}
		_ = 0f;
		if (noLerp)
		{
			m_CurrentCoin = m_FinalCoin;
			m_CurrentCoinDouble = m_FinalCoinDouble;
			m_CoinText.text = GameInstance.GetPriceString(m_FinalCoinDouble);
			PlayAddCoinAnimation();
			m_PlayAddCoinSFXTimer = 0f;
			m_IsPlayingAddCoinSFX = true;
		}
		if (!GameInstance.m_StopCoinGemTextLerp)
		{
			m_CoinText.text = GameInstance.GetPriceString(m_FinalCoinDouble);
		}
	}

	public void ReduceCoin(float coinValue, bool noLerp)
	{
		m_FinalCoin -= coinValue;
		m_FinalCoinDouble -= coinValue;
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			m_FinalCoinDouble = Math.Round(m_FinalCoinDouble, 3, MidpointRounding.AwayFromZero);
		}
		else
		{
			m_FinalCoinDouble = Math.Round(m_FinalCoinDouble, 2, MidpointRounding.AwayFromZero);
		}
		if (noLerp)
		{
			m_CurrentCoin = m_FinalCoin;
			m_CurrentCoinDouble = m_FinalCoinDouble;
			m_CoinText.text = GameInstance.GetPriceString(m_FinalCoinDouble);
			PlayAddCoinAnimation();
			m_PlayAddCoinSFXTimer = 0f;
			m_IsPlayingAddCoinSFX = true;
		}
		m_CoinText.text = GameInstance.GetPriceString(m_FinalCoinDouble);
	}

	public void PlayAddCoinAnimation()
	{
	}

	private void EvaluateShopLevelAndExp()
	{
		m_ShopLevelText.text = LocalizationManager.GetTranslation("Shop Level XXX").Replace("XXX", (CPlayerData.m_ShopLevel + 1).ToString());
		m_ShopExpBar.fillAmount = (float)CPlayerData.m_ShopExpPoint / (float)CPlayerData.GetExpRequiredToLevelUp();
		m_ShopEXPText.text = CPlayerData.m_ShopExpPoint + "/" + CPlayerData.GetExpRequiredToLevelUp() + " XP";
	}

	public static void HideEnterGoNextDayIndicatorVisible()
	{
		CSingleton<GameUIScreen>.Instance.m_PressEnterGoNextDayIndicatorText.SetActive(value: false);
	}

	public static void ResetEnterGoNextDayIndicatorVisible()
	{
		CSingleton<GameUIScreen>.Instance.UpdateGoNextDayText();
		CSingleton<GameUIScreen>.Instance.m_PressEnterGoNextDayIndicatorText.SetActive(value: true);
	}

	public static void SetGameUIVisible(bool isVisible)
	{
		if (isVisible)
		{
			if (CSingleton<GameUIScreen>.Instance.m_CanvasGrp.alpha != 1f)
			{
				CSingleton<GameUIScreen>.Instance.m_IsShowingCanvasGrpAlpha = true;
				CSingleton<GameUIScreen>.Instance.m_IsHidingCanvasGrpAlpha = false;
			}
		}
		else if (CSingleton<GameUIScreen>.Instance.m_CanvasGrp.alpha != 0f)
		{
			CSingleton<GameUIScreen>.Instance.m_IsShowingCanvasGrpAlpha = false;
			CSingleton<GameUIScreen>.Instance.m_IsHidingCanvasGrpAlpha = true;
		}
	}

	public static void HideToolTip()
	{
		CSingleton<GameUIScreen>.Instance.m_IsTooltipVisible = CSingleton<GameUIScreen>.Instance.m_PhoneTooltipUI.gameObject.activeSelf;
		CSingleton<GameUIScreen>.Instance.m_PhoneTooltipUI.gameObject.SetActive(value: false);
		CSingleton<GameUIScreen>.Instance.m_AlbumTooltipUI.gameObject.SetActive(value: false);
		CSingleton<GameUIScreen>.Instance.m_DecoInventoryTooltipUI.gameObject.SetActive(value: false);
	}

	public static void ResetToolTipVisibility()
	{
		CSingleton<GameUIScreen>.Instance.m_PhoneTooltipUI.gameObject.SetActive(CSingleton<GameUIScreen>.Instance.m_IsTooltipVisible);
		CSingleton<GameUIScreen>.Instance.m_AlbumTooltipUI.gameObject.SetActive(CSingleton<GameUIScreen>.Instance.m_IsTooltipVisible);
		CSingleton<GameUIScreen>.Instance.m_DecoInventoryTooltipUI.gameObject.SetActive(CSingleton<GameUIScreen>.Instance.m_IsTooltipVisible);
	}

	private void EvaluateCoinTextColor()
	{
		if (m_FinalCoin < 0f)
		{
			m_CoinText.color = m_ReduceMoneyColor;
		}
		else
		{
			m_CoinText.color = m_AddMoneyColor;
		}
	}

	public void UpdateGoNextDayText()
	{
		string newValue = "Enter";
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			newValue = "R3";
			newValue = "RS";
		}
		m_GoNextDayText.text = LocalizationManager.GetTranslation("Press XXX to start next day").Replace("XXX", newValue);
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_SetCoin>(CPlayer_OnSetCoin);
			CEventManager.AddListener<CEventPlayer_AddCoin>(CPlayer_OnAddCoin);
			CEventManager.AddListener<CEventPlayer_ReduceCoin>(CPlayer_OnReduceCoin);
			CEventManager.AddListener<CEventPlayer_AddShopExp>(CPlayer_OnAddShopExp);
			CEventManager.AddListener<CEventPlayer_SetShopExp>(CPlayer_OnSetShopExp);
			CEventManager.AddListener<CEventPlayer_ShopLeveledUp>(CPlayer_OnShopLeveledUp);
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.AddListener<CEventPlayer_OnDayEnded>(OnDayEnded);
			CEventManager.AddListener<CEventPlayer_OnLanguageChanged>(OnLanguageChanged);
			CEventManager.AddListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
			CEventManager.AddListener<CEventPlayer_OnSaveStatusUpdated>(OnSaveStatusUpdated);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_SetCoin>(CPlayer_OnSetCoin);
			CEventManager.RemoveListener<CEventPlayer_AddCoin>(CPlayer_OnAddCoin);
			CEventManager.RemoveListener<CEventPlayer_ReduceCoin>(CPlayer_OnReduceCoin);
			CEventManager.RemoveListener<CEventPlayer_AddShopExp>(CPlayer_OnAddShopExp);
			CEventManager.RemoveListener<CEventPlayer_SetShopExp>(CPlayer_OnSetShopExp);
			CEventManager.RemoveListener<CEventPlayer_ShopLeveledUp>(CPlayer_OnShopLeveledUp);
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.RemoveListener<CEventPlayer_OnDayEnded>(OnDayEnded);
			CEventManager.RemoveListener<CEventPlayer_OnLanguageChanged>(OnLanguageChanged);
			CEventManager.RemoveListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
			CEventManager.RemoveListener<CEventPlayer_OnSaveStatusUpdated>(OnSaveStatusUpdated);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
	}

	private void CPlayer_OnAddCoin(CEventPlayer_AddCoin evt)
	{
		if (m_AddMoneyPopupList.Count == 0)
		{
			m_AddMoneyPopupTimer = 10f;
		}
		m_AddMoneyPopupList.Add(evt.m_CoinValue);
		AddCoin(evt.m_CoinValue, evt.m_NoLerp);
		EvaluateCoinTextColor();
	}

	private void CPlayer_OnReduceCoin(CEventPlayer_ReduceCoin evt)
	{
		if (m_AddMoneyPopupList.Count == 0)
		{
			m_AddMoneyPopupTimer = 10f;
		}
		m_AddMoneyPopupList.Add(0f - evt.m_CoinValue);
		ReduceCoin(evt.m_CoinValue, evt.m_NoLerp);
		EvaluateCoinTextColor();
	}

	private void CPlayer_OnSetCoin(CEventPlayer_SetCoin evt)
	{
		m_CurrentCoin = evt.m_CoinValue;
		m_FinalCoin = evt.m_CoinValue;
		m_CurrentCoinDouble = evt.m_CoinValueDouble;
		m_FinalCoinDouble = evt.m_CoinValueDouble;
		m_CoinText.text = GameInstance.GetPriceString(m_FinalCoinDouble);
		EvaluateCoinTextColor();
	}

	private void CPlayer_OnAddShopExp(CEventPlayer_AddShopExp evt)
	{
		if (m_AddShopExpPopupList.Count == 0)
		{
			m_AddShopExpPopupTimer = 10f;
		}
		m_AddShopExpPopupList.Add(evt.m_ExpValue);
		EvaluateShopLevelAndExp();
	}

	private void CPlayer_OnSetShopExp(CEventPlayer_SetShopExp evt)
	{
		EvaluateShopLevelAndExp();
	}

	private void CPlayer_OnShopLeveledUp(CEventPlayer_ShopLeveledUp evt)
	{
		EvaluateShopLevelAndExp();
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		m_DayText.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (CPlayerData.m_CurrentDay + 1).ToString());
		m_PressEnterGoNextDayIndicator.SetActive(value: false);
	}

	protected void OnDayEnded(CEventPlayer_OnDayEnded evt)
	{
		m_PressEnterGoNextDayIndicator.SetActive(value: true);
	}

	protected void OnLanguageChanged(CEventPlayer_OnLanguageChanged evt)
	{
		UpdateGoNextDayText();
		m_DayText.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (CPlayerData.m_CurrentDay + 1).ToString());
		EvaluateShopLevelAndExp();
	}

	protected void OnMoneyCurrencyUpdated(CEventPlayer_OnMoneyCurrencyUpdated evt)
	{
		m_CoinText.text = GameInstance.GetPriceString(m_FinalCoin);
	}

	protected void OnSaveStatusUpdated(CEventPlayer_OnSaveStatusUpdated evt)
	{
		if (evt.m_IsAutosaving)
		{
			if (!m_AutoSavingSucessGrp.activeSelf && !m_AutoSavingFailedGrp.activeSelf)
			{
				m_AutoSavingGrp.SetActive(value: true);
			}
			else
			{
				m_AutoSavingGrp.SetActive(value: false);
			}
			return;
		}
		m_AutoSavingGrp.SetActive(value: false);
		if (evt.m_IsSuccess)
		{
			m_AutoSavingSucessGrp.SetActive(value: true);
		}
		else
		{
			m_AutoSavingFailedGrp.SetActive(value: true);
		}
	}
}
