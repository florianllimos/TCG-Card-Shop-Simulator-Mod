using GA;
using TMPro;
using UnityEngine;

public class PauseScreen : CSingleton<PauseScreen>
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	public TextMeshProUGUI m_VersionText;

	private bool m_IsCursorVisible;

	private bool m_IsCameraEnabled;

	private CursorLockMode m_CursorLockMode;

	public static void OpenScreen()
	{
		if (CSingleton<PauseScreen>.Instance.m_ScreenGrp.activeSelf)
		{
			CloseScreen();
			return;
		}
		CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
		CSingleton<PauseScreen>.Instance.m_IsCursorVisible = Cursor.visible;
		CSingleton<PauseScreen>.Instance.m_CursorLockMode = InputManager.GetCursorLockMode();
		CSingleton<PauseScreen>.Instance.m_IsCameraEnabled = CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled;
		CSingleton<InteractionPlayerController>.Instance.ShowCursor();
		CSingleton<PauseScreen>.Instance.m_VersionText.text = "v" + Application.version;
		CSingleton<PauseScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		SoundManager.MuteAllSound();
		ControllerScreenUIExtManager.OnOpenScreen(CSingleton<PauseScreen>.Instance.m_ControllerScreenUIExtension);
		Time.timeScale = 0f;
	}

	public static void CloseScreen()
	{
		if (CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ScreenGrp.activeSelf)
		{
			SaveLoadGameSlotSelectScreen.CloseScreen();
			return;
		}
		if (CSingleton<SettingScreen>.Instance.m_ScreenGrp.activeSelf)
		{
			SettingScreen.CloseScreen();
			return;
		}
		Time.timeScale = 1f;
		SoundManager.UnMuteAllSound();
		CSingleton<PauseScreen>.Instance.m_ScreenGrp.SetActive(value: false);
		if (InputManager.GetCursorLockMode() == CursorLockMode.Confined && !CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			Cursor.visible = true;
		}
		else
		{
			Cursor.visible = false;
		}
		if (CSingleton<PauseScreen>.Instance.m_CursorLockMode == CursorLockMode.Locked)
		{
			CenterDot.SetVisibility(isVisible: true);
		}
		InputManager.SetCursorLockMode(CSingleton<PauseScreen>.Instance.m_CursorLockMode);
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = CSingleton<PauseScreen>.Instance.m_IsCameraEnabled;
		SoundManager.GenericLightTap();
		ControllerScreenUIExtManager.OnCloseScreen(CSingleton<PauseScreen>.Instance.m_ControllerScreenUIExtension);
		CSingleton<InteractionPlayerController>.Instance.ResetMousePress();
	}

	public void OnPressSetting()
	{
		SoundManager.GenericLightTap();
		SettingScreen.OpenScreen(isTitleScreen: false);
	}

	public void OpenLoadGameSlotScreen()
	{
		SoundManager.GenericLightTap();
		SaveLoadGameSlotSelectScreen.OpenScreen(isSaveState: false);
	}

	public void OpenSaveGameSlotScreen()
	{
		SoundManager.GenericLightTap();
		SaveLoadGameSlotSelectScreen.OpenScreen(isSaveState: true);
	}

	public void OnPressBackMenu()
	{
		Time.timeScale = 1f;
		SoundManager.GenericLightTap();
		CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
		CSingleton<CGameManager>.Instance.SaveGameData(0);
		CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Title");
	}

	public void OnPressQuit()
	{
		SoundManager.GenericLightTap();
		Application.Quit();
	}

	public void OnPressWishlistOnSteam()
	{
		string wishlistUrl = PlatformManager.Instance.GetWishlistUrl();
		if (!string.IsNullOrEmpty(wishlistUrl))
		{
			Application.OpenURL(wishlistUrl);
		}
	}

	public void OnPressFeedbackBtn()
	{
		string feedbackUrl = PlatformManager.Instance.GetFeedbackUrl();
		if (!string.IsNullOrEmpty(feedbackUrl))
		{
			Application.OpenURL(feedbackUrl);
		}
	}

	public void OnPressDiscordBtn()
	{
		Application.OpenURL("https://discord.gg/2YaaUZzrRY");
	}
}
