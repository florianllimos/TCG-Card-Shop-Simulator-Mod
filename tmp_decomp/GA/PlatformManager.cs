using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GA;

public class PlatformManager : MonoBehaviour, IPlatformManager
{
	private class DefaultPlatformManager : IPlatformManager
	{
		public bool IsInitialized => true;

		public bool HasAnySaves => false;

		public void Initialize()
		{
		}

		public void Update()
		{
		}

		public void SaveAsync(string fileName, byte[] data, Action<bool> onComplete = null)
		{
			onComplete?.Invoke(obj: true);
		}

		public bool Load(string fileName, out byte[] data)
		{
			data = null;
			return true;
		}

		public void UnlockAchievement(string achievementId)
		{
		}

		public void OnSceneLoaded(string sceneName)
		{
		}

		public void OnQuit()
		{
		}

		public string GetWishlistUrl()
		{
			return string.Empty;
		}

		public string GetFeedbackUrl()
		{
			return string.Empty;
		}

		public string GetLanguage()
		{
			return "English";
		}

		public void SetPresence(string presence)
		{
		}
	}

	public const string GAME_DATA_FILE_NAME = "GameData";

	public const string SETTINGS_FILE_NAME = "Settings";

	public const string PLAYERPREFS_FILE_NAME = "PlayerPrefs";

	public const int MAX_SAVE_SLOTS = 3;

	public const string DEFAULT_LANGUAGE = "English";

	private const float PHYSICS_FPS = 50f;

	public static bool TEST_IN_EDITOR;

	[SerializeField]
	private PlatformPrefabSet m_prefabSet;

	private IPlatformManager m_platformImpl;

	private PlatformPlayerPrefsStorage m_playerPrefs;

	private readonly Dictionary<string, byte[]> m_cachedFiles = new Dictionary<string, byte[]>();

	private readonly HashSet<string> m_savesInProgress = new HashSet<string>();

	public static PlatformManager Instance { get; private set; }

	public PlatformPlayerPrefsStorage PlayerPrefsStorage => m_playerPrefs;

	public bool IsInitialized
	{
		get
		{
			if (m_platformImpl != null && m_platformImpl.IsInitialized)
			{
				return m_playerPrefs.IsLoaded;
			}
			return false;
		}
	}

	public bool HasAnySaves
	{
		get
		{
			if (m_platformImpl != null)
			{
				return m_platformImpl.HasAnySaves;
			}
			return false;
		}
	}

	public PlatformPrefabSet GetPrefabSet()
	{
		return m_prefabSet;
	}

	public void Initialize()
	{
		Time.fixedDeltaTime = 0.02f;
		Time.maximumDeltaTime = 0.04f;
		Input.simulateMouseWithTouches = false;
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
		m_platformImpl = CreatePlatformManager();
		m_platformImpl.Initialize();
		m_playerPrefs = new PlatformPlayerPrefsStorage("PlayerPrefs", () => m_platformImpl.Load("PlayerPrefs", out var data) ? data : null, delegate(byte[] bytes)
		{
			m_platformImpl.SaveAsync("PlayerPrefs", bytes, delegate(bool result)
			{
				Debug.Log(string.Format("[PlayerPrefsStorage] Save '{0}' completed: {1}", "PlayerPrefs", result));
			});
		});
		PlatformPlayerPrefs.Init(m_playerPrefs);
		PlatformPlayerPrefsWrapper.Impl = new PlatformPlayerPrefsImpl();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Update()
	{
		m_platformImpl.Update();
	}

	public void SaveAsync(string fileName, byte[] data, Action<bool> onComplete = null)
	{
		m_platformImpl.SaveAsync(fileName, data, onComplete);
	}

	public bool Load(string fileName, out byte[] data)
	{
		return m_platformImpl.Load(fileName, out data);
	}

	public void UnlockAchievement(string achievementId)
	{
		m_platformImpl.UnlockAchievement(achievementId);
	}

	public void OnSceneLoaded(string sceneName)
	{
		if (GetPlatform() != EPlatform.Unknown)
		{
			if (sceneName == "Title")
			{
				PlatformPrefabLoader.ReplaceIfNeeded("TitleScreen");
				PlatformPrefabLoader.ReplaceIfNeeded("SettingScreen");
			}
			else if (sceneName == "Start")
			{
				PlatformPrefabLoader.ReplaceIfNeeded("PauseScreen");
				PauseScreen.CloseScreen();
				PlatformPrefabLoader.ReplaceIfNeeded("SettingScreen");
				EndOfDayReportScreen.IsActive();
			}
			m_platformImpl?.OnSceneLoaded(sceneName);
		}
	}

	public void OnQuit()
	{
		m_platformImpl?.OnQuit();
	}

	public string GetWishlistUrl()
	{
		return m_platformImpl?.GetWishlistUrl();
	}

	public string GetFeedbackUrl()
	{
		return m_platformImpl?.GetFeedbackUrl();
	}

	public string GetLanguage()
	{
		return m_platformImpl?.GetLanguage();
	}

	public void SetPresence(string presence)
	{
		m_platformImpl?.SetPresence(presence);
	}

	public void OnApplicationQuit()
	{
		OnQuit();
	}

	private static IPlatformManager CreatePlatformManager()
	{
		return new GamecorePlatformManager();
	}

	public static EPlatform GetPlatform()
	{
		return EPlatform.MSStore;
	}

	public bool UseNativeSaves()
	{
		return GetPlatform() != EPlatform.Unknown;
	}

	[RuntimeInitializeOnLoadMethod]
	private static void InitializeInPlayer()
	{
		Instance.Initialize();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		OnSceneLoaded(scene.name);
		if (scene.name == "Title")
		{
			SetPresence("RP_IN_MENU");
		}
		else if (scene.name == "Start")
		{
			SetPresence("RP_IN_GAME");
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void CacheFile(string fileName, byte[] data)
	{
		m_cachedFiles[fileName] = data;
	}

	public bool TryGetCachedFile(string fileName, out byte[] data)
	{
		return m_cachedFiles.TryGetValue(fileName, out data);
	}

	public void AddSaveInProgress(string fileName)
	{
		m_savesInProgress.Add(fileName);
	}

	public void RemoveSaveInProgress(string fileName)
	{
		m_savesInProgress.Remove(fileName);
	}

	public bool IsSaving(string fileName)
	{
		return m_savesInProgress.Contains(fileName);
	}
}
