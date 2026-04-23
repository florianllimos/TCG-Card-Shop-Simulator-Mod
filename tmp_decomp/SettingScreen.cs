using System.Collections.Generic;
using I2.Loc;
using Unity.XGamingRuntime;
using UnityEngine;
using UniversalSettings;

public class SettingScreen : CSingleton<SettingScreen>
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public ControllerScreenUIExtension m_KeybindChangesConfirmScreenUIExtension;

	public GameObject m_ScreenGrp;

	public GameObject m_GameSettingScreen;

	public GameObject m_DisplaySettingScreen;

	public GameObject m_LanguageScreen;

	public GameObject m_KeybindScreen;

	public GameObject m_KeybindKeyboardHighlight;

	public GameObject m_KeybindControllerHighlight;

	public GameObject m_KeybindKeyboardScreen;

	public GameObject m_KeybindControllerScreen;

	public GameObject m_KeybindChangesConfirmScreen;

	public List<GameObject> m_SubSettingBtnHighlight;

	public List<KeybindSetting> m_KeybindSettingList;

	private bool m_IsCursorVisible;

	private bool m_IsTitleScreen = true;

	private bool m_IsChangingKeybind;

	private EMoneyCurrencyType m_CurrentCurrencyType;

	public static void OpenScreen(bool isTitleScreen)
	{
		CSingleton<SettingScreen>.Instance.m_IsTitleScreen = isTitleScreen;
		CSingleton<SettingScreen>.Instance.m_GameSettingScreen.SetActive(value: true);
		CSingleton<SettingScreen>.Instance.m_DisplaySettingScreen.SetActive(value: false);
		CSingleton<SettingScreen>.Instance.m_LanguageScreen.SetActive(value: false);
		CSingleton<SettingScreen>.Instance.m_KeybindScreen.SetActive(value: false);
		CSingleton<SettingScreen>.Instance.ShowSubSettingBtnHighlight(0);
		CSingleton<SettingScreen>.Instance.m_IsChangingKeybind = false;
		CSingleton<SettingScreen>.Instance.m_CurrentCurrencyType = CSingleton<CGameManager>.Instance.m_CurrencyType;
		if (CSingleton<SettingScreen>.Instance.m_ScreenGrp.activeSelf)
		{
			CloseScreen();
			return;
		}
		CSingleton<SettingScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		ControllerScreenUIExtManager.OnOpenScreen(CSingleton<SettingScreen>.Instance.m_ControllerScreenUIExtension);
		SoundManager.GenericMenuOpen();
	}

	public static void CloseScreen()
	{
		if (!CSingleton<SettingScreen>.Instance.m_IsChangingKeybind && !CheckKeybindChangesConfirmScreenPopup())
		{
			CSingleton<SettingScreen>.Instance.m_ScreenGrp.SetActive(value: false);
			ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SettingScreen>.Instance.m_ControllerScreenUIExtension);
			CSettingData.instance.SaveSettingData();
			PersistentStorage.ForceSaveSettings();
			SoundManager.GenericMenuClose();
			if (CSingleton<SettingScreen>.Instance.m_CurrentCurrencyType != CSingleton<CGameManager>.Instance.m_CurrencyType)
			{
				CSingleton<SettingScreen>.Instance.m_CurrentCurrencyType = CSingleton<CGameManager>.Instance.m_CurrencyType;
				CEventManager.QueueEvent(new CEventPlayer_OnMoneyCurrencyUpdated());
			}
		}
	}

	private static bool CheckKeybindChangesConfirmScreenPopup()
	{
		if (InputManager.HasKeybindChanges())
		{
			ControllerScreenUIExtManager.OnOpenScreen(CSingleton<SettingScreen>.Instance.m_KeybindChangesConfirmScreenUIExtension);
			CSingleton<SettingScreen>.Instance.m_KeybindChangesConfirmScreen.SetActive(value: true);
			return true;
		}
		return false;
	}

	public void OnPressConfirmKeybindChange()
	{
		m_KeybindChangesConfirmScreen.SetActive(value: false);
		m_IsChangingKeybind = false;
		OnPressSaveKeybind();
		ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SettingScreen>.Instance.m_KeybindChangesConfirmScreenUIExtension);
	}

	public void OnPressCancelKeybindChange()
	{
		m_KeybindChangesConfirmScreen.SetActive(value: false);
		m_IsChangingKeybind = false;
		OnPressUndoKeybind();
		ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SettingScreen>.Instance.m_KeybindChangesConfirmScreenUIExtension);
	}

	private void Start()
	{
		UniversalSettingsRunner.Instance.onApplySettings += delegate
		{
			CSingleton<CGameManager>.Instance.m_EnableTooltip = UniversalSettingsRunner.Instance.GetCustomBoolean(0);
			CSingleton<CGameManager>.Instance.m_InvertedMouse = UniversalSettingsRunner.Instance.GetCustomBoolean(1);
			CSingleton<CGameManager>.Instance.m_InvertedMouseX = UniversalSettingsRunner.Instance.GetCustomBoolean(10);
			CSingleton<CGameManager>.Instance.m_CashierLockMovement = UniversalSettingsRunner.Instance.GetCustomBoolean(2);
			CSingleton<CGameManager>.Instance.m_KeyboardTypeIndex = UniversalSettingsRunner.Instance.GetCustomInteger(1);
			InputManager.OnKeyboardTypeUpdated(CSingleton<CGameManager>.Instance.m_KeyboardTypeIndex);
			CSingleton<CGameManager>.Instance.m_QualitySettingIndex = UniversalSettingsRunner.Instance.GetCustomInteger(2);
			CSingleton<CGameManager>.Instance.m_MouseSensitivity = UniversalSettingsRunner.Instance.GetCustomFloat(0);
			SoundManager.MusicVolume = UniversalSettingsRunner.Instance.GetCustomFloat(1);
			SoundManager.SFXVolume = UniversalSettingsRunner.Instance.GetCustomFloat(2);
			CSingleton<CGameManager>.Instance.m_CameraFOVSlider = UniversalSettingsRunner.Instance.GetCustomFloat(3);
			CSingleton<CGameManager>.Instance.m_CenterDotSizeSlider = UniversalSettingsRunner.Instance.GetCustomFloat(4);
			CSingleton<CGameManager>.Instance.m_OpenPackSpeedSlider = UniversalSettingsRunner.Instance.GetCustomFloat(5);
			CSingleton<CGameManager>.Instance.m_CenterDotColorIndex = UniversalSettingsRunner.Instance.GetCustomInteger(3);
			CSingleton<CGameManager>.Instance.m_CenterDotSpriteTypeIndex = UniversalSettingsRunner.Instance.GetCustomInteger(4);
			CSingleton<CGameManager>.Instance.m_CurrencyType = (EMoneyCurrencyType)UniversalSettingsRunner.Instance.GetCustomInteger(5);
			EvaluateRestockBoxLimit(UniversalSettingsRunner.Instance.GetCustomInteger(6));
			CSingleton<CGameManager>.Instance.m_CenterDotHasOutline = UniversalSettingsRunner.Instance.GetCustomBoolean(3);
			CSingleton<CGameManager>.Instance.m_OpenPackShowNewCard = UniversalSettingsRunner.Instance.GetCustomBoolean(4);
			CSingleton<CGameManager>.Instance.m_OpenPacAutoNextCard = UniversalSettingsRunner.Instance.GetCustomBoolean(5);
			CSingleton<CGameManager>.Instance.m_DisableController = UniversalSettingsRunner.Instance.GetCustomBoolean(6);
			CSingleton<CGameManager>.Instance.m_IsTurnVSyncOff = UniversalSettingsRunner.Instance.GetCustomBoolean(7);
			CSingleton<CGameManager>.Instance.m_IsHoldToSprint = UniversalSettingsRunner.Instance.GetCustomBoolean(8);
			CSingleton<CGameManager>.Instance.m_IsHoldToCrouch = UniversalSettingsRunner.Instance.GetCustomBoolean(9);
			if (CSingleton<CGameManager>.Instance.m_IsTurnVSyncOff)
			{
				QualitySettings.vSyncCount = 0;
			}
			else
			{
				QualitySettings.vSyncCount = 1;
			}
			CSingleton<InputManager>.Instance.SetIsControllerDisabledSetting(CSingleton<CGameManager>.Instance.m_DisableController);
			CSingleton<SoundManager>.Instance.EvaluateSoundVolume();
			CSingleton<CGameManager>.Instance.m_MouseSensitivityLerp = Mathf.Lerp(0.05f, 1.7f, CSingleton<CGameManager>.Instance.m_MouseSensitivity);
			CEventManager.QueueEvent(new CEventPlayer_OnSettingUpdated());
		};
		CSingleton<CGameManager>.Instance.m_EnableTooltip = UniversalSettingsRunner.Instance.GetCustomBoolean(0);
		CSingleton<CGameManager>.Instance.m_InvertedMouse = UniversalSettingsRunner.Instance.GetCustomBoolean(1);
		CSingleton<CGameManager>.Instance.m_InvertedMouseX = UniversalSettingsRunner.Instance.GetCustomBoolean(10);
		CSingleton<CGameManager>.Instance.m_CashierLockMovement = UniversalSettingsRunner.Instance.GetCustomBoolean(2);
		CSingleton<CGameManager>.Instance.m_KeyboardTypeIndex = UniversalSettingsRunner.Instance.GetCustomInteger(1);
		InputManager.OnKeyboardTypeUpdated(CSingleton<CGameManager>.Instance.m_KeyboardTypeIndex);
		CSingleton<CGameManager>.Instance.m_QualitySettingIndex = UniversalSettingsRunner.Instance.GetCustomInteger(2);
		CSingleton<CGameManager>.Instance.m_MouseSensitivity = UniversalSettingsRunner.Instance.GetCustomFloat(0);
		SoundManager.MusicVolume = UniversalSettingsRunner.Instance.GetCustomFloat(1);
		SoundManager.SFXVolume = UniversalSettingsRunner.Instance.GetCustomFloat(2);
		CSingleton<CGameManager>.Instance.m_CameraFOVSlider = UniversalSettingsRunner.Instance.GetCustomFloat(3);
		CSingleton<CGameManager>.Instance.m_CenterDotSizeSlider = UniversalSettingsRunner.Instance.GetCustomFloat(4);
		CSingleton<CGameManager>.Instance.m_OpenPackSpeedSlider = UniversalSettingsRunner.Instance.GetCustomFloat(5);
		CSingleton<CGameManager>.Instance.m_CenterDotColorIndex = UniversalSettingsRunner.Instance.GetCustomInteger(3);
		CSingleton<CGameManager>.Instance.m_CenterDotSpriteTypeIndex = UniversalSettingsRunner.Instance.GetCustomInteger(4);
		CSingleton<CGameManager>.Instance.m_CurrencyType = (EMoneyCurrencyType)UniversalSettingsRunner.Instance.GetCustomInteger(5);
		EvaluateRestockBoxLimit(UniversalSettingsRunner.Instance.GetCustomInteger(6));
		CSingleton<CGameManager>.Instance.m_CenterDotHasOutline = UniversalSettingsRunner.Instance.GetCustomBoolean(3);
		CSingleton<CGameManager>.Instance.m_OpenPackShowNewCard = UniversalSettingsRunner.Instance.GetCustomBoolean(4);
		CSingleton<CGameManager>.Instance.m_OpenPacAutoNextCard = UniversalSettingsRunner.Instance.GetCustomBoolean(5);
		CSingleton<CGameManager>.Instance.m_DisableController = UniversalSettingsRunner.Instance.GetCustomBoolean(6);
		CSingleton<CGameManager>.Instance.m_IsTurnVSyncOff = UniversalSettingsRunner.Instance.GetCustomBoolean(7);
		CSingleton<CGameManager>.Instance.m_IsHoldToSprint = UniversalSettingsRunner.Instance.GetCustomBoolean(8);
		CSingleton<CGameManager>.Instance.m_IsHoldToCrouch = UniversalSettingsRunner.Instance.GetCustomBoolean(9);
		if (CSingleton<CGameManager>.Instance.m_IsTurnVSyncOff)
		{
			QualitySettings.vSyncCount = 0;
		}
		else
		{
			QualitySettings.vSyncCount = 1;
		}
		CSingleton<InputManager>.Instance.SetIsControllerDisabledSetting(CSingleton<CGameManager>.Instance.m_DisableController);
		CSingleton<CGameManager>.Instance.m_MouseSensitivityLerp = Mathf.Lerp(0.05f, 1.7f, CSingleton<CGameManager>.Instance.m_MouseSensitivity);
		switch (SDK.XSystemGetDeviceType())
		{
		case XSystemDeviceType.XboxOne:
		case XSystemDeviceType.XboxOneS:
			Texture.SetGlobalAnisotropicFilteringLimits(2, 2);
			break;
		case XSystemDeviceType.XboxOneX:
		case XSystemDeviceType.XboxOneXDevkit:
		case XSystemDeviceType.XboxScarlettLockhart:
			Texture.SetGlobalAnisotropicFilteringLimits(4, 4);
			break;
		case XSystemDeviceType.XboxScarlettAnaconda:
		case XSystemDeviceType.XboxScarlettDevkit:
			Texture.SetGlobalAnisotropicFilteringLimits(8, 8);
			break;
		}
		CEventManager.QueueEvent(new CEventPlayer_OnSettingUpdated());
	}

	private void EvaluateRestockBoxLimit(int restockSpawnBoxIndex)
	{
		switch (restockSpawnBoxIndex)
		{
		case 1:
			CSingleton<CGameManager>.Instance.m_RestockSpawnBoxLimit = 5000;
			break;
		case 2:
			CSingleton<CGameManager>.Instance.m_RestockSpawnBoxLimit = 10000;
			break;
		case 3:
			CSingleton<CGameManager>.Instance.m_RestockSpawnBoxLimit = 20000;
			break;
		default:
			CSingleton<CGameManager>.Instance.m_RestockSpawnBoxLimit = 2000;
			break;
		}
	}

	public void ManualUpdate()
	{
		if (!m_IsChangingKeybind && CSingleton<SettingScreen>.Instance.m_ScreenGrp.activeSelf && (Input.GetKeyUp(KeyCode.Escape) || InputManager.GetKeyUpAction(EGameAction.ClosePhone)))
		{
			CloseScreen();
		}
	}

	public void OnSettingUpdated(float amount)
	{
	}

	public void OnPressGameSetting()
	{
		if (!m_IsChangingKeybind && !CheckKeybindChangesConfirmScreenPopup())
		{
			SoundManager.GenericConfirm();
			m_GameSettingScreen.SetActive(value: true);
			m_DisplaySettingScreen.SetActive(value: false);
			m_LanguageScreen.SetActive(value: false);
			m_KeybindScreen.SetActive(value: false);
			ShowSubSettingBtnHighlight(0);
		}
	}

	public void OnPressDisplaySetting()
	{
		if (!m_IsChangingKeybind && !CheckKeybindChangesConfirmScreenPopup())
		{
			SoundManager.GenericConfirm();
			m_GameSettingScreen.SetActive(value: false);
			m_DisplaySettingScreen.SetActive(value: true);
			m_LanguageScreen.SetActive(value: false);
			m_KeybindScreen.SetActive(value: false);
			ShowSubSettingBtnHighlight(1);
		}
	}

	public void OnPressLanguage()
	{
		if (!m_IsChangingKeybind && !CheckKeybindChangesConfirmScreenPopup())
		{
			SoundManager.GenericConfirm();
			m_GameSettingScreen.SetActive(value: false);
			m_DisplaySettingScreen.SetActive(value: false);
			m_LanguageScreen.SetActive(value: true);
			m_KeybindScreen.SetActive(value: false);
			ShowSubSettingBtnHighlight(2);
		}
	}

	public void OnPressKeybindSetting()
	{
		if (!m_IsChangingKeybind)
		{
			SoundManager.GenericConfirm();
			m_GameSettingScreen.SetActive(value: false);
			m_DisplaySettingScreen.SetActive(value: false);
			m_LanguageScreen.SetActive(value: false);
			m_KeybindScreen.SetActive(value: true);
			m_KeybindKeyboardHighlight.SetActive(value: true);
			m_KeybindControllerHighlight.SetActive(value: false);
			if (CSingleton<InputManager>.Instance.m_IsControllerActive)
			{
				m_KeybindKeyboardScreen.SetActive(value: false);
				m_KeybindControllerScreen.SetActive(value: true);
				m_KeybindKeyboardHighlight.SetActive(value: false);
				m_KeybindControllerHighlight.SetActive(value: true);
			}
			else
			{
				m_KeybindKeyboardScreen.SetActive(value: true);
				m_KeybindControllerScreen.SetActive(value: false);
				m_KeybindKeyboardHighlight.SetActive(value: true);
				m_KeybindControllerHighlight.SetActive(value: false);
			}
			ShowSubSettingBtnHighlight(3);
			EvaluateKeybindSettingUI();
		}
	}

	public void OnPressKeybindKeyboard()
	{
		if (!m_IsChangingKeybind)
		{
			SoundManager.GenericConfirm();
			m_KeybindKeyboardHighlight.SetActive(value: true);
			m_KeybindControllerHighlight.SetActive(value: false);
			m_KeybindKeyboardScreen.SetActive(value: true);
			m_KeybindControllerScreen.SetActive(value: false);
			EvaluateKeybindSettingUI();
		}
	}

	public void OnPressKeybindController()
	{
		if (!m_IsChangingKeybind)
		{
			SoundManager.GenericConfirm();
			m_KeybindKeyboardHighlight.SetActive(value: false);
			m_KeybindControllerHighlight.SetActive(value: true);
			m_KeybindKeyboardScreen.SetActive(value: false);
			m_KeybindControllerScreen.SetActive(value: true);
			EvaluateKeybindSettingUI();
		}
	}

	public void OnPressResetKeybind()
	{
		if (!m_IsChangingKeybind)
		{
			SoundManager.GenericConfirm();
			CSingleton<InputManager>.Instance.ResetToDefaultKeybind();
			CSettingData.instance.SaveSettingData();
			EvaluateKeybindSettingUI();
		}
	}

	public void OnPressUndoKeybind()
	{
		if (!m_IsChangingKeybind)
		{
			SoundManager.GenericConfirm();
			CSingleton<InputManager>.Instance.UndoKeybind();
			EvaluateKeybindSettingUI();
		}
	}

	public void OnPressSaveKeybind()
	{
		if (!m_IsChangingKeybind)
		{
			SoundManager.GenericConfirm();
			CSettingData.instance.SaveSettingData();
			CEventManager.QueueEvent(new CEventPlayer_OnKeybindChanged());
		}
	}

	private void EvaluateKeybindSettingUI()
	{
		for (int i = 0; i < m_KeybindSettingList.Count; i++)
		{
			m_KeybindSettingList[i].UpdateInputUI();
		}
	}

	public void OnPressLanguageSelect(string language)
	{
		switch (language)
		{
		case "English":
			LocalizationManager.CurrentLanguageCode = "en";
			break;
		case "France":
			LocalizationManager.CurrentLanguageCode = "fr";
			break;
		case "Germany":
			LocalizationManager.CurrentLanguageCode = "de";
			break;
		case "Spanish":
			LocalizationManager.CurrentLanguageCode = "es";
			break;
		case "ChineseT":
			LocalizationManager.CurrentLanguageCode = "zh-TW";
			break;
		case "ChineseS":
			LocalizationManager.CurrentLanguageCode = "zh-CN";
			break;
		case "Korean":
			LocalizationManager.CurrentLanguageCode = "ko";
			break;
		case "Japanese":
			LocalizationManager.CurrentLanguageCode = "ja";
			break;
		case "Portuguese":
			LocalizationManager.CurrentLanguageCode = "pt-BR";
			break;
		case "Italian":
			LocalizationManager.CurrentLanguageCode = "it";
			break;
		case "Russian":
			LocalizationManager.CurrentLanguageCode = "ru";
			break;
		case "Hindi":
			LocalizationManager.CurrentLanguageCode = "hi";
			break;
		case "Thai":
			LocalizationManager.CurrentLanguageCode = "th";
			break;
		case "Arabic":
			LocalizationManager.CurrentLanguageCode = "ar";
			LocalizationManager.IsRight2Left = false;
			break;
		case "Dutch":
			LocalizationManager.CurrentLanguageCode = "nl";
			break;
		}
		CEventManager.QueueEvent(new CEventPlayer_OnLanguageChanged());
	}

	private void ShowSubSettingBtnHighlight(int index)
	{
		for (int i = 0; i < m_SubSettingBtnHighlight.Count; i++)
		{
			m_SubSettingBtnHighlight[i].SetActive(value: false);
		}
		m_SubSettingBtnHighlight[index].SetActive(value: true);
	}

	public void SetIsChangingKeybind(bool isChangingKeybind)
	{
		m_IsChangingKeybind = isChangingKeybind;
	}

	public static bool IsChangingKeybind()
	{
		return CSingleton<SettingScreen>.Instance.m_IsChangingKeybind;
	}
}
