using GA;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension_OverwriteSaveScreen;

	public GameObject m_ConfirmOverwriteSaveScreen;

	public Button m_ContinueButton;

	public Button m_LoadGameButton;

	public TextMeshProUGUI m_VersionText;

	private bool m_IsOpeningLevel;

	private void Start()
	{
		if (CSaveLoad.HasSaveFile(0))
		{
			m_ContinueButton.interactable = true;
		}
		else
		{
			m_ContinueButton.interactable = false;
		}
		if (!CSaveLoad.HasSaveFile(0) && !CSaveLoad.HasSaveFile(1) && !CSaveLoad.HasSaveFile(2))
		{
			CSaveLoad.HasSaveFile(3);
		}
		m_VersionText.text = "v" + Application.version;
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
	}

	public void OnPressStartGame()
	{
		if (!m_IsOpeningLevel)
		{
			SoundManager.GenericLightTap();
			if (CSaveLoad.HasSaveFile(0) && m_ConfirmOverwriteSaveScreen != null)
			{
				m_ConfirmOverwriteSaveScreen.SetActive(value: true);
				ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension_OverwriteSaveScreen);
				Debug.Log("Has save file, prompt if want to overwrite");
			}
			else
			{
				m_IsOpeningLevel = true;
				CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start");
				ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
			}
		}
	}

	public void OnPressConfirmOverwrite()
	{
		if (!m_IsOpeningLevel)
		{
			CSaveLoad.AutoSaveMoveToEmptySaveSlot();
			SoundManager.GenericLightTap();
			m_IsOpeningLevel = true;
			CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start");
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension_OverwriteSaveScreen);
		}
	}

	public void OnPressCloseOverwrite()
	{
		SoundManager.GenericLightTap();
		if (m_ConfirmOverwriteSaveScreen != null)
		{
			m_ConfirmOverwriteSaveScreen.SetActive(value: false);
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension_OverwriteSaveScreen);
		}
	}

	public void OnPressLoadGame()
	{
		if (!m_IsOpeningLevel)
		{
			SoundManager.GenericLightTap();
			m_IsOpeningLevel = true;
			CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start", 0);
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
		}
	}

	public void OpenLoadGameSlotScreen()
	{
		if (!m_IsOpeningLevel)
		{
			SoundManager.GenericLightTap();
			SaveLoadGameSlotSelectScreen.OpenScreen(isSaveState: false);
		}
	}

	public void OnPressSetting()
	{
		SoundManager.GenericLightTap();
		SettingScreen.OpenScreen(isTitleScreen: true);
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
