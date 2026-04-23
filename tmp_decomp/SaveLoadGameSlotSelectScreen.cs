using System.Collections;
using System.Collections.Generic;
using GA;
using UnityEngine;

public class SaveLoadGameSlotSelectScreen : CSingleton<SaveLoadGameSlotSelectScreen>
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension_OverwriteSaveFileScreen;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension_OverwriteLoadGameAutoSaveFileScreen;

	public GameObject m_ScreenGrp;

	public GameObject m_OverwriteSaveFileScreenGrp;

	public GameObject m_OverwriteLoadGameAutoSaveFileScreenGrp;

	public GameObject m_LoadGameText;

	public GameObject m_SaveGameText;

	public GameObject m_SavingGameScreen;

	public List<SaveLoadSlotPanelUI> m_SaveLoadSlotPanelUIList;

	public List<LoadSavedSlotData> m_SavedDataList = new List<LoadSavedSlotData>();

	private bool m_IsOpeningLevel;

	private bool m_IsSavingGame;

	private bool m_IsSaveState;

	private bool m_IsDataLoaded;

	private int m_CurrentSaveSlotIndex;

	public static void OpenScreen(bool isSaveState)
	{
		if (CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsSavingGame)
		{
			return;
		}
		CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsSaveState = isSaveState;
		CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_LoadGameText.SetActive(!isSaveState);
		CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SaveGameText.SetActive(isSaveState);
		if (!isSaveState && CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Count >= 4)
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsDataLoaded = true;
		}
		if (!CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsDataLoaded)
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Clear();
			if (CSaveLoad.LoadSavedSlotData(0))
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(CGameData.instance.GetLoadSavedSlotData(CSaveLoad.m_SavedGameBackup));
			}
			else
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(default(LoadSavedSlotData));
			}
			if (CSaveLoad.LoadSavedSlotData(1))
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(CGameData.instance.GetLoadSavedSlotData(CSaveLoad.m_SavedGameBackup));
			}
			else
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(default(LoadSavedSlotData));
			}
			if (CSaveLoad.LoadSavedSlotData(2))
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(CGameData.instance.GetLoadSavedSlotData(CSaveLoad.m_SavedGameBackup));
			}
			else
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(default(LoadSavedSlotData));
			}
			if (CSaveLoad.LoadSavedSlotData(3))
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(CGameData.instance.GetLoadSavedSlotData(CSaveLoad.m_SavedGameBackup));
			}
			else
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Add(default(LoadSavedSlotData));
			}
		}
		for (int i = 0; i < CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Count; i++)
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SaveLoadSlotPanelUIList[i].LoadSlotData(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList[i]);
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SaveLoadSlotPanelUIList[i].SetSaveOrLoadState(isSaveState);
		}
		CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension);
	}

	public static void CloseScreen()
	{
		if (!CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsSavingGame)
		{
			if (CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteSaveFileScreenGrp.activeSelf)
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.CloseConfirmOverwriteSaveScreen();
				return;
			}
			if (CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteLoadGameAutoSaveFileScreenGrp.activeSelf)
			{
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.CloseConfirmOverwriteLoadGameAutoSaveScreen();
				return;
			}
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ScreenGrp.SetActive(value: false);
			SoundManager.GenericMenuClose();
			ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension);
		}
	}

	public void OnPressLoadGame(int slotIndex)
	{
		if (!m_IsSavingGame && !m_IsOpeningLevel)
		{
			if (slotIndex != 0 && CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Count > 0 && CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList[0].hasSaveData)
			{
				SoundManager.GenericMenuOpen();
				m_CurrentSaveSlotIndex = slotIndex;
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteLoadGameAutoSaveFileScreenGrp.SetActive(value: true);
				ControllerScreenUIExtManager.OnOpenScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension_OverwriteLoadGameAutoSaveFileScreen);
			}
			else
			{
				SoundManager.GenericLightTap();
				m_IsOpeningLevel = true;
				CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = slotIndex;
				CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
				CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start", 0);
			}
		}
	}

	public void OnPressSaveGame(int slotIndex)
	{
		if (!m_IsSavingGame)
		{
			if (CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList[slotIndex].hasSaveData && CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList[slotIndex].name != CPlayerData.PlayerName)
			{
				SoundManager.GenericMenuOpen();
				m_CurrentSaveSlotIndex = slotIndex;
				CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteSaveFileScreenGrp.SetActive(value: true);
				ControllerScreenUIExtManager.OnOpenScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension_OverwriteSaveFileScreen);
				return;
			}
			m_CurrentSaveSlotIndex = slotIndex;
			SoundManager.GenericLightTap();
			CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = slotIndex;
			CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
			CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
			CSingleton<CGameManager>.Instance.SaveGameData(slotIndex);
			m_IsSavingGame = true;
			m_SavingGameScreen.gameObject.SetActive(value: true);
			StartCoroutine(DelaySavingGame());
		}
	}

	public void OnPressConfirmOverwriteSave()
	{
		if (!m_IsSavingGame)
		{
			SoundManager.GenericLightTap();
			CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = m_CurrentSaveSlotIndex;
			CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
			CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
			CSingleton<CGameManager>.Instance.SaveGameData(m_CurrentSaveSlotIndex);
			m_IsSavingGame = true;
			m_SavingGameScreen.gameObject.SetActive(value: true);
			StartCoroutine(DelaySavingGame());
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteSaveFileScreenGrp.SetActive(value: false);
			ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension_OverwriteSaveFileScreen);
		}
	}

	public void CloseConfirmOverwriteSaveScreen()
	{
		if (!CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsSavingGame)
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteSaveFileScreenGrp.SetActive(value: false);
			SoundManager.GenericMenuClose();
			ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension_OverwriteSaveFileScreen);
		}
	}

	public void OnPressConfirmLoadGameOverwriteAutoSave()
	{
		if (!CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsSavingGame && !CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsOpeningLevel)
		{
			CSaveLoad.AutoSaveMoveToEmptySaveSlot();
			SoundManager.GenericLightTap();
			m_IsOpeningLevel = true;
			CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex = m_CurrentSaveSlotIndex;
			CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = true;
			CSingleton<CGameManager>.Instance.LoadMainLevelAsync("Start", 0);
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteLoadGameAutoSaveFileScreenGrp.SetActive(value: false);
			ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension_OverwriteLoadGameAutoSaveFileScreen);
		}
	}

	public void CloseConfirmOverwriteLoadGameAutoSaveScreen()
	{
		if (!CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_IsSavingGame)
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_OverwriteLoadGameAutoSaveFileScreenGrp.SetActive(value: false);
			SoundManager.GenericMenuClose();
			ControllerScreenUIExtManager.OnCloseScreen(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_ControllerScreenUIExtension_OverwriteLoadGameAutoSaveFileScreen);
		}
	}

	private IEnumerator DelaySavingGame()
	{
		if (PlatformManager.Instance != null && PlatformManager.Instance.UseNativeSaves())
		{
			int currentSaveSlotIndex = m_CurrentSaveSlotIndex;
			float elapsed = 0f;
			string fileName = string.Format("{0}_{1}", "GameData", currentSaveSlotIndex);
			while (PlatformManager.Instance.IsSaving(fileName))
			{
				yield return null;
				elapsed += Time.unscaledDeltaTime;
			}
			float num = Mathf.Max(0f, 1f - elapsed);
			if (num > 0f)
			{
				yield return new WaitForSecondsRealtime(num);
			}
		}
		else
		{
			yield return new WaitForSecondsRealtime(2f);
		}
		m_SavingGameScreen.gameObject.SetActive(value: false);
		m_IsSavingGame = false;
		if (CSaveLoad.LoadSavedSlotData(m_CurrentSaveSlotIndex))
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList[m_CurrentSaveSlotIndex] = CGameData.instance.GetLoadSavedSlotData(CSaveLoad.m_SavedGameBackup);
		}
		else
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList[m_CurrentSaveSlotIndex] = default(LoadSavedSlotData);
		}
		for (int i = 0; i < CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList.Count; i++)
		{
			CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SaveLoadSlotPanelUIList[i].LoadSlotData(CSingleton<SaveLoadGameSlotSelectScreen>.Instance.m_SavedDataList[i]);
		}
	}
}
