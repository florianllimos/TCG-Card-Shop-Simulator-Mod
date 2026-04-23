using System;
using System.Collections;
using System.Collections.Generic;
using GA;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CGameManager : CSingleton<CGameManager>
{
	public static CGameManager m_Instance;

	public const string k_StartSceneName = "Start";

	public Text_ScriptableObject m_TextSO;

	public bool m_IsGameLevel;

	public bool m_NoSave;

	public bool m_IsPrologue;

	public bool m_IsLightweightDev;

	public bool m_DeveloperModeEnabled;

	public bool m_ForceNoCloudSaveLoad;

	public bool m_ForceCollectionPackFull;

	public bool m_EnableScreenshotTaking;

	public bool m_IsManualSaveLoad;

	public int m_CurrentSaveLoadSlotSelectedIndex;

	public int m_ScreenshotScaling = 2;

	public int m_TimeScale = 1;

	public int m_LoadGameIndex = -1;

	public int m_KeyboardTypeIndex;

	public int m_QualitySettingIndex;

	public int m_CenterDotColorIndex;

	public int m_CenterDotSpriteTypeIndex;

	public int m_RestockSpawnBoxLimit = 2000;

	public EMoneyCurrencyType m_CurrencyType;

	public float m_MouseSensitivity = 0.5f;

	public float m_MouseSensitivityLerp = 0.5f;

	public float m_CameraFOVSlider = 0.5f;

	public float m_CenterDotSizeSlider = 0.3f;

	public float m_OpenPackSpeedSlider;

	public bool m_EnableTooltip = true;

	public bool m_InvertedMouse;

	public bool m_InvertedMouseX;

	public bool m_CashierLockMovement;

	public bool m_LockItemLabel;

	public bool m_CanConfineMouseCursor;

	public bool m_CenterDotHasOutline;

	public bool m_OpenPackShowNewCard;

	public bool m_OpenPacAutoNextCard;

	public bool m_DisableController;

	public bool m_IsTurnVSyncOff;

	public bool m_IsHoldToSprint;

	public bool m_IsHoldToCrouch;

	public bool m_CanRunDebugString;

	public bool m_CanRunDebugString2;

	public List<GameObject> m_ScreenshotObjectList;

	private float m_ScreenshotTimer;

	private int m_ScreenshotObjIndex;

	private static bool m_InitLoaded;

	private CGameData m_GameData;

	private static Transform m_Player;

	private static float m_TotalGameSceneTime;

	public static float m_TutorialManagerTimer;

	public static float m_TutorialSubGrpTimer;

	private static double m_TimePassed;

	public static double m_LastTimePassed;

	private static double m_TotalTimePassed;

	public static float m_ForceSyncCloudResetTimer;

	public static DateTime m_LoginTime;

	private static bool m_HasSavedGame;

	private static float m_HasSavedGameTimer;

	private static bool m_HasSavedBackupGame;

	private static float m_HasSavedBackupGameTimer;

	private bool m_IsOpenCloseGameScreen;

	private float m_CanSaveGameTimer;

	private bool m_CanSaveGame;

	private int m_LastScreenIndex;

	private DateTime m_LastPauseDateTime;

	private float m_SecondTimer;

	private float m_MinuteTimer;

	private float m_CloudSaveDirtyTimer;

	public static Transform Player
	{
		get
		{
			return m_Player;
		}
		set
		{
			m_Player = value;
		}
	}

	public static float TotalGameSceneTime
	{
		get
		{
			return m_TotalGameSceneTime;
		}
		set
		{
			m_TotalGameSceneTime = value;
		}
	}

	public static double TimePassed => m_TimePassed;

	public static double TotalTimePassed
	{
		get
		{
			return m_TotalTimePassed;
		}
		set
		{
			m_TotalTimePassed = value;
		}
	}

	private void Start()
	{
		m_LockItemLabel = true;
		Init();
	}

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		UnityEngine.Object.DontDestroyOnLoad(this);
		m_GameData = CGameData.instance;
		if (PlatformPlayerPrefs.HasKey("ResetGame") && PlatformPlayerPrefs.GetInt("ResetGame") == 1)
		{
			CSaveLoad.Delete();
			PlatformPlayerPrefs.SetInt("ResetGame", 0);
		}
		m_LastPauseDateTime = DateTime.UtcNow;
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		LoadingScreen.CloseScreen();
		if (scene.name == "Title")
		{
			m_IsGameLevel = false;
			SoundManager.BlendToMusic("BGM_Silence", 1f, isLinearBlend: true);
			CSingleton<LoadingScreen>.Instance.FinishLoading();
		}
		else
		{
			m_IsGameLevel = true;
		}
		InputManager.OnLevelFinishedLoading();
		if (!m_InitLoaded && !(scene.name == "Title"))
		{
			if (PlatformPlayerPrefs.HasKey("ResetGame") && PlatformPlayerPrefs.GetInt("ResetGame") == 1)
			{
				CSaveLoad.Delete();
				PlatformPlayerPrefs.SetInt("ResetGame", 0);
			}
			if (m_LoadGameIndex != -1 && m_IsGameLevel)
			{
				m_LoadGameIndex = -1;
				StartCoroutine(LoadDelay());
			}
			else if (m_IsGameLevel)
			{
				CSingleton<ShelfManager>.Instance.m_FinishLoadingObjectData = true;
				GameInstance.m_SaveFileNotFound = true;
				CPlayerData.CreateDefaultData();
				m_InitLoaded = true;
				CEventManager.QueueEvent(new CEventPlayer_GameDataFinishLoaded());
				CSingleton<LoadingScreen>.Instance.FinishLoading();
				GameInstance.m_FinishedSavefileLoading = true;
			}
		}
	}

	private void Update()
	{
		_ = m_DeveloperModeEnabled;
		m_TotalGameSceneTime += Time.deltaTime;
		m_TotalTimePassed += Time.deltaTime;
		m_TutorialManagerTimer += Time.deltaTime;
		m_TutorialSubGrpTimer += Time.deltaTime;
		m_ForceSyncCloudResetTimer += Time.deltaTime;
		m_SecondTimer += Time.deltaTime;
		if (m_SecondTimer >= 1f)
		{
			m_SecondTimer -= 1f;
		}
		m_MinuteTimer += Time.deltaTime;
		if (m_MinuteTimer >= 60f)
		{
			m_MinuteTimer = 0f;
		}
		if (m_EnableScreenshotTaking && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.V)))
		{
			ScreenCapture.CaptureScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Screenshots/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png", m_ScreenshotScaling);
		}
		if (m_HasSavedGame)
		{
			m_HasSavedGameTimer += Time.deltaTime;
			if (m_HasSavedGameTimer > 5f)
			{
				m_HasSavedGameTimer = 0f;
				m_HasSavedGame = false;
			}
		}
		if (m_HasSavedBackupGame)
		{
			m_HasSavedBackupGameTimer += Time.deltaTime;
			if (m_HasSavedBackupGameTimer > 5f)
			{
				m_HasSavedBackupGameTimer = 0f;
				m_HasSavedBackupGame = false;
			}
		}
		if (!m_CanSaveGame)
		{
			m_CanSaveGameTimer += Time.deltaTime;
			if (m_CanSaveGameTimer >= 300f)
			{
				m_CanSaveGame = true;
				m_CanSaveGameTimer = 0f;
			}
		}
		if (m_ScreenshotObjectList.Count <= 0 || m_ScreenshotObjIndex >= m_ScreenshotObjectList.Count)
		{
			return;
		}
		m_ScreenshotTimer += Time.deltaTime;
		if (m_ScreenshotTimer > 1f)
		{
			for (int i = 0; i < m_ScreenshotObjectList.Count; i++)
			{
				m_ScreenshotObjectList[i].SetActive(value: false);
			}
			m_ScreenshotObjectList[m_ScreenshotObjIndex].SetActive(value: true);
			TakeScreenshot();
			m_ScreenshotTimer = 0f;
			m_ScreenshotObjIndex++;
		}
	}

	public void TakeScreenshot()
	{
		if (m_EnableScreenshotTaking)
		{
			ScreenCapture.CaptureScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Screenshots/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png", m_ScreenshotScaling);
		}
	}

	private void FixedUpdate()
	{
	}

	private IEnumerator LoadDelay()
	{
		yield return new WaitForSeconds(0.0001f);
		if (!m_InitLoaded)
		{
			LoadData();
		}
		yield return new WaitForSeconds(0.1f);
		m_InitLoaded = true;
		if (PlatformPlayerPrefs.HasKey("HasFinishedTutorial"))
		{
			if ((!CPlayerData.m_HasFinishedTutorial || PlatformPlayerPrefs.GetInt("HasFinishedTutorial") != 1) && (CPlayerData.m_HasFinishedTutorial || PlatformPlayerPrefs.GetInt("HasFinishedTutorial") != 0))
			{
				CPlayerData.m_CanCloudLoad = true;
			}
		}
		else if (CPlayerData.m_HasFinishedTutorial)
		{
			PlatformPlayerPrefs.SetInt("HasFinishedTutorial", 1);
		}
	}

	private void Init()
	{
		m_TotalGameSceneTime = 0f;
	}

	private void LoadData()
	{
		Debug.Log("LoadData m_CurrentSaveLoadSlotSelectedIndex " + m_CurrentSaveLoadSlotSelectedIndex);
		if (CSaveLoad.Load(m_CurrentSaveLoadSlotSelectedIndex))
		{
			CGameData.instance.PropagateLoadData(CSaveLoad.m_SavedGame);
		}
		else
		{
			LoadBackupData(m_CurrentSaveLoadSlotSelectedIndex);
		}
	}

	private IEnumerator changeFramerate()
	{
		yield return new WaitForSeconds(1f);
		Application.targetFrameRate = 30;
		Application.runInBackground = false;
	}

	private new void OnApplicationQuit()
	{
		m_HasSavedGame = false;
		GameInstance.m_CurrentSceneIndex = 0;
		SaveGameData(0);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			m_HasSavedGame = false;
			if (CSingleton<InputManager>.Instance.m_IsSteamDeck)
			{
				SaveGameData(0);
			}
			m_LastPauseDateTime = DateTime.UtcNow;
		}
		else
		{
			_ = DateTime.UtcNow - m_LastPauseDateTime;
			m_LastPauseDateTime = DateTime.UtcNow;
		}
	}

	public void ResetLastPauseTime()
	{
		m_LastPauseDateTime = DateTime.UtcNow;
	}

	public double DateTimeToUnixTimestamp(DateTime dateTime)
	{
		return (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public void SaveGameData(int saveSlotIndex)
	{
		if (!m_IsGameLevel)
		{
			Debug.Log("Skip save at title screen");
		}
		else if (GameInstance.m_HasLoadingError)
		{
			Debug.Log("Cant save due to loading error");
		}
		else if (CPlayerData.m_CashCounterSaveDataList.Count <= 0)
		{
			Debug.Log("Cant save, no shelf data");
		}
		else if (GameInstance.m_FinishedSavefileLoading)
		{
			CPlayerData.m_LastLocalExitTime = DateTime.UtcNow;
			m_IsManualSaveLoad = true;
			m_CurrentSaveLoadSlotSelectedIndex = saveSlotIndex;
			CGameData.instance.SaveGameData(saveSlotIndex);
			m_HasSavedGame = true;
			m_HasSavedGameTimer = 0f;
			GC.Collect();
			if (saveSlotIndex == 0)
			{
				CEventManager.QueueEvent(new CEventPlayer_OnSaveStatusUpdated(isSuccess: false, isAutosaving: true));
			}
		}
	}

	public void LoadBackupData(int slotIndex)
	{
		if (CSaveLoad.LoadBackup(slotIndex))
		{
			Debug.Log("Load backup save data slotIndex " + slotIndex);
			CGameData.instance.PropagateLoadData(CSaveLoad.m_SavedGame);
			return;
		}
		Debug.Log("No save found, start new data");
		GameInstance.m_HasLoadingError = false;
		GameInstance.m_SaveFileNotFound = true;
		CPlayerData.CreateDefaultData();
		m_InitLoaded = true;
		CSingleton<ShelfManager>.Instance.m_FinishLoadingObjectData = true;
		CEventManager.QueueEvent(new CEventPlayer_GameDataFinishLoaded());
		CSingleton<LoadingScreen>.Instance.FinishLoading();
		GameInstance.m_FinishedSavefileLoading = true;
	}

	public static bool CanChangeScene()
	{
		if (m_TotalGameSceneTime > 0.5f)
		{
			return true;
		}
		return false;
	}

	public static void RestartGame()
	{
		GameInstance.m_CurrentSceneIndex = 0;
		CEventManager.QueueEvent(new CEventPlayer_ChangeScene(ELevelName.Start));
	}

	private void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_ChangeScene>(CPlayer_OnChangeScene);
			SceneManager.sceneLoaded += OnLevelFinishedLoading;
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_ChangeScene>(CPlayer_OnChangeScene);
			SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		}
	}

	private void CPlayer_OnChangeScene(CEventPlayer_ChangeScene evt)
	{
	}

	public static void SaveGameToCloud()
	{
		if (!CSingleton<CGameManager>.Instance.m_ForceNoCloudSaveLoad)
		{
			CGameData.instance.IsCloudVersionNewerThanLocal();
		}
	}

	public static int GetSaveLoadSlotSelectedIndex()
	{
		if (CSingleton<CGameManager>.Instance.m_IsManualSaveLoad)
		{
			CSingleton<CGameManager>.Instance.m_IsManualSaveLoad = false;
			return CSingleton<CGameManager>.Instance.m_CurrentSaveLoadSlotSelectedIndex;
		}
		return 0;
	}

	public static void LoadGameFromCloud()
	{
		_ = CSingleton<CGameManager>.Instance.m_ForceNoCloudSaveLoad;
	}

	private IEnumerator DelayLoadScene(string sceneName)
	{
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(sceneName);
	}

	public void LoadMainLevelAsync(string sceneName, int loadGameIndex = -1)
	{
		m_LoadGameIndex = loadGameIndex;
		StartCoroutine(LoadLobbySceneAsync(sceneName));
	}

	private IEnumerator DelayLoadLobbyScene(string sceneName)
	{
		if (sceneName == "Title")
		{
			m_InitLoaded = false;
		}
		Debug.Log("DelayLoadLobbyScene");
		LoadingScreen.OpenScreen();
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(sceneName);
	}

	private IEnumerator LoadLobbySceneAsync(string sceneName)
	{
		if (sceneName == "Title")
		{
			m_InitLoaded = false;
		}
		LoadingScreen.OpenScreen();
		yield return new WaitForSeconds(2f);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		while (!asyncLoad.isDone)
		{
			string text = (int)(100f * asyncLoad.progress / 0.9f) + "%";
			LoadingScreen.SetPercentDone((int)(100f * asyncLoad.progress / 0.9f));
			Debug.Log("percentDone " + text);
			yield return null;
		}
	}
}
