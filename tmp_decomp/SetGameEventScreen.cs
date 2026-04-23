using I2.Loc;
using TMPro;
using UnityEngine;

public class SetGameEventScreen : UIScreenBase
{
	public SetGameEventPriceScreen m_SetPriceScreen;

	public SetGameEventFormatScreen m_SetFormatScreen;

	public GameObject m_CurrentGameEventText;

	public GameObject m_NextGameEventText;

	public TextMeshProUGUI m_ShopNameText;

	public TextMeshProUGUI m_FeeText;

	public TextMeshProUGUI m_FormatText;

	public TextMeshProUGUI m_CostText;

	public TextMeshProUGUI m_PositiveEffectText;

	public TextMeshProUGUI m_NegativeEffectText;

	private string m_FeePrefix = "Fee";

	private string m_FormatPrefix = "Format";

	private string m_CostPrefix = "Cost";

	private string m_FeeSuffix = "hr";

	private string m_CostSuffix = "day";

	private float m_LastSetFee;

	protected override void OnOpenScreen()
	{
		m_LastSetFee = PriceChangeManager.GetGameEventPrice(CPlayerData.m_GameEventFormat);
		base.OnOpenScreen();
		m_ShopNameText.text = CPlayerData.PlayerName;
		EvaluateText();
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
	}

	public void OnPressSetPrice()
	{
		OpenChildScreen(m_SetPriceScreen);
		SoundManager.GenericConfirm();
	}

	public void OnPressSetFormat()
	{
		OpenChildScreen(m_SetFormatScreen);
		SoundManager.GenericConfirm();
	}

	public void OnPressReset()
	{
		CPlayerData.m_PendingGameEventFormat = EGameEventFormat.None;
		CPlayerData.m_PendingGameEventExpansionType = CPlayerData.m_GameEventExpansionType;
		PriceChangeManager.SetGameEventPrice(CPlayerData.m_GameEventFormat, m_LastSetFee);
		EvaluateText();
		SoundManager.GenericConfirm();
	}

	protected override void OnChildScreenClosed(UIScreenBase childScreen)
	{
		base.OnChildScreenClosed(childScreen);
		EvaluateText();
	}

	private void EvaluateText()
	{
		if (CPlayerData.m_PendingGameEventFormat != EGameEventFormat.None)
		{
			GameEventData gameEventData = InventoryBase.GetGameEventData(CPlayerData.m_PendingGameEventFormat);
			m_FormatText.text = LocalizationManager.GetTranslation(m_FormatPrefix) + " : " + gameEventData.GetName();
			m_CostText.text = LocalizationManager.GetTranslation(m_CostPrefix) + " : " + GameInstance.GetPriceString(gameEventData.hostEventCost) + "/" + LocalizationManager.GetTranslation(m_CostSuffix);
			m_FeeText.text = LocalizationManager.GetTranslation(m_FeePrefix) + " : " + GameInstance.GetPriceString(PriceChangeManager.GetGameEventPrice(CPlayerData.m_PendingGameEventFormat)) + LocalizationManager.GetTranslation(m_FeeSuffix);
			m_PositiveEffectText.text = "(+) " + InventoryBase.GetPriceChangeTypeText(gameEventData.positivePriceChangeType, isIncrease: true);
			m_NegativeEffectText.text = "(-) " + InventoryBase.GetPriceChangeTypeText(gameEventData.negativePriceChangeType, isIncrease: false);
			m_CurrentGameEventText.gameObject.SetActive(value: false);
			m_NextGameEventText.gameObject.SetActive(value: true);
		}
		else
		{
			GameEventData gameEventData2 = InventoryBase.GetGameEventData(CPlayerData.m_GameEventFormat);
			m_FormatText.text = LocalizationManager.GetTranslation(m_FormatPrefix) + " : " + gameEventData2.GetName();
			m_CostText.text = LocalizationManager.GetTranslation(m_CostPrefix) + " : " + GameInstance.GetPriceString(gameEventData2.hostEventCost) + "/" + LocalizationManager.GetTranslation(m_CostSuffix);
			m_FeeText.text = LocalizationManager.GetTranslation(m_FeePrefix) + " : " + GameInstance.GetPriceString(PriceChangeManager.GetGameEventPrice(CPlayerData.m_GameEventFormat)) + LocalizationManager.GetTranslation(m_FeeSuffix);
			m_PositiveEffectText.text = "(+) " + InventoryBase.GetPriceChangeTypeText(gameEventData2.positivePriceChangeType, isIncrease: true);
			m_NegativeEffectText.text = "(-) " + InventoryBase.GetPriceChangeTypeText(gameEventData2.negativePriceChangeType, isIncrease: false);
			m_CurrentGameEventText.gameObject.SetActive(value: true);
			m_NextGameEventText.gameObject.SetActive(value: false);
		}
		if (CPlayerData.m_GameEventExpansionType != CPlayerData.m_PendingGameEventExpansionType)
		{
			m_CurrentGameEventText.gameObject.SetActive(value: false);
			m_NextGameEventText.gameObject.SetActive(value: true);
		}
	}
}
