using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PhoneScreen : UIScreenBase
{
	public TextMeshProUGUI m_TimeText;

	public TextMeshProUGUI m_DayText;

	public Image m_BatteryFill;

	public List<Button> m_PhoneButtonList;

	public GameObject m_RentBillNotification;

	protected override void RunUpdate()
	{
		base.RunUpdate();
		m_TimeText.text = CSingleton<LightManager>.Instance.m_TimeString;
		m_BatteryFill.fillAmount = 1f - CSingleton<LightManager>.Instance.GetPercentTillDayEnd();
	}

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
		SetPhoneButtonRaycastEnable(isEnable: true);
		m_DayText.text = CSingleton<GameUIScreen>.Instance.m_DayText.text;
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressExitPhoneMode()
	{
		PhoneManager.ExitPhoneMode();
	}

	public void OnPressBuyProductBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_RestockItemScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressBuyBoardGameProductBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_RestockItemBoardGameScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressBuyFurnitureBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_FurnitureShopUIScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressExpandShopBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_ExpandShopUIScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressSettingBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		SettingScreen.OpenScreen(isTitleScreen: false);
	}

	public void OnPressManageEventBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_SetGameEventUIScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressHireWorkerBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_HireWorkerScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressCustomerReviewBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_CustomerReviewScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressCheckPriceScreenBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_CheckPriceScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressRentBillBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_RentBillScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressBuyDecorationBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_ShopBuyDecoUIScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressGradingCardBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_GradeCardWebsiteUIScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void OnPressScannerBtn()
	{
		SoundManager.PlayAudio("SFX_ButtonLightTap", 0.15f);
		OpenChildScreen(CSingleton<PhoneManager>.Instance.m_ScannerRestockScreen);
		PhoneManager.SetCanClosePhone(canClose: false);
		SetPhoneButtonRaycastEnable(isEnable: false);
	}

	public void SetBillNotificationVisible(bool isVisible)
	{
		m_RentBillNotification.SetActive(isVisible);
	}

	protected override void OnChildScreenClosed(UIScreenBase childScreen)
	{
		PhoneManager.SetCanClosePhone(canClose: true);
		SetPhoneButtonRaycastEnable(isEnable: true);
	}

	private void SetPhoneButtonRaycastEnable(bool isEnable)
	{
		for (int i = 0; i < m_PhoneButtonList.Count; i++)
		{
			m_PhoneButtonList[i].interactable = isEnable;
		}
	}
}
