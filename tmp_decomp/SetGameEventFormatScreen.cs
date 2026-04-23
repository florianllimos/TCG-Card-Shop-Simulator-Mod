using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetGameEventFormatScreen : UIScreenBase
{
	public GameObject m_LockedGrp;

	public Button m_ConfirmButton;

	public Button m_PreviousButton;

	public Button m_NextButton;

	public TextMeshProUGUI m_PlayCountRequired;

	public TextMeshProUGUI m_FeeText;

	public TextMeshProUGUI m_FormatText;

	public TextMeshProUGUI m_CostText;

	public TextMeshProUGUI m_PositiveEffectText;

	public TextMeshProUGUI m_NegativeEffectText;

	private string m_FeePrefix = "Market Fee";

	private string m_FormatPrefix = "Format";

	private string m_CostPrefix = "Cost";

	private string m_FeeSuffix = "hr";

	private string m_CostSuffix = "day";

	private int m_CurrentFormatIndex;

	private bool m_IsFormatUnlocked;

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
		m_CurrentFormatIndex = (int)CPlayerData.m_GameEventFormat;
		if (CPlayerData.m_PendingGameEventFormat != EGameEventFormat.None)
		{
			m_CurrentFormatIndex = (int)CPlayerData.m_PendingGameEventFormat;
		}
		EvaluateText((EGameEventFormat)m_CurrentFormatIndex);
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
	}

	public void OnPressConfirmBtn()
	{
		CPlayerData.m_PendingGameEventFormat = (EGameEventFormat)m_CurrentFormatIndex;
		SoundManager.GenericConfirm();
		CloseScreen();
	}

	public void OnPressCancelBtn()
	{
		SoundManager.GenericCancel();
		CloseScreen();
	}

	public void OnPressNextEventSelect()
	{
		if (m_CurrentFormatIndex < 11)
		{
			SoundManager.GenericLightTap();
			m_CurrentFormatIndex++;
			EvaluateText((EGameEventFormat)m_CurrentFormatIndex);
		}
	}

	public void OnPressPreviousEventSelect()
	{
		if (m_CurrentFormatIndex > 0)
		{
			SoundManager.GenericLightTap();
			m_CurrentFormatIndex--;
			EvaluateText((EGameEventFormat)m_CurrentFormatIndex);
		}
	}

	private void EvaluateText(EGameEventFormat gameEventFormat)
	{
		GameEventData gameEventData = InventoryBase.GetGameEventData(gameEventFormat);
		m_IsFormatUnlocked = false;
		int customerPlayed = CPlayerData.m_GameReportDataCollectPermanent.customerPlayed;
		int unlockPlayCountRequired = gameEventData.unlockPlayCountRequired;
		if (customerPlayed >= unlockPlayCountRequired)
		{
			m_IsFormatUnlocked = true;
			m_LockedGrp.SetActive(value: false);
			m_ConfirmButton.interactable = true;
		}
		else
		{
			m_PlayCountRequired.text = customerPlayed + "/" + unlockPlayCountRequired;
			m_LockedGrp.SetActive(value: true);
			m_ConfirmButton.interactable = false;
		}
		m_FormatText.text = gameEventData.GetName();
		m_CostText.text = LocalizationManager.GetTranslation(m_CostPrefix) + " : " + GameInstance.GetPriceString(gameEventData.hostEventCost) + "/" + LocalizationManager.GetTranslation(m_CostSuffix);
		m_FeeText.text = LocalizationManager.GetTranslation(m_FeePrefix) + " : " + GameInstance.GetPriceString(PriceChangeManager.GetGameEventPrice(gameEventFormat)) + LocalizationManager.GetTranslation(m_FeeSuffix);
		m_PositiveEffectText.text = "(+) " + InventoryBase.GetPriceChangeTypeText(gameEventData.positivePriceChangeType, isIncrease: true);
		m_NegativeEffectText.text = "(-) " + InventoryBase.GetPriceChangeTypeText(gameEventData.negativePriceChangeType, isIncrease: false);
		m_NextButton.interactable = m_CurrentFormatIndex < 11;
		m_PreviousButton.interactable = m_CurrentFormatIndex > 0;
	}
}
