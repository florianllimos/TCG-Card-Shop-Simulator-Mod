using System;
using System.Collections;
using System.Threading;
using TMPro;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;
using UnityEngine;

namespace GA;

public class GamecorePlatformManager : IPlatformManager
{
	private const string k_wishlistUrl = "https://www.xbox.com/en-au/games/store/tcg-card-shop-simulator/9p9gx5d8h9dw";

	private const string k_feedbackUrl = "https://www.xbox.com/en-au/games/store/tcg-card-shop-simulator/9p9gx5d8h9dw";

	private GamecoreUserManager m_userManager;

	private bool m_usersChanged;

	private CancellationTokenSource m_cancellationTokenSource;

	private GamecoreSaveManager m_saveManager;

	private const string k_blobName = "PlayerData";

	private bool m_isSavesInitialized;

	private bool m_isPreloaded;

	private bool m_hasAnySaves;

	private GamecoreAchievementsManager m_achievementsManager;

	private string m_currentPresence;

	private TextMeshProUGUI m_userTagText;

	public static bool s_isRuntimeInitialized { get; private set; }

	public bool IsInitialized
	{
		get
		{
			if (s_isRuntimeInitialized && m_isSavesInitialized)
			{
				return m_isPreloaded;
			}
			return false;
		}
	}

	public bool HasAnySaves => m_hasAnySaves;

	public void Initialize()
	{
		Debug.Log("[GamecorePlatformManager] Initialize");
		if (!InitializeRuntime())
		{
			Debug.Log("[GamecorePlatformManager] InitializeRuntime Failed");
		}
		m_cancellationTokenSource = new CancellationTokenSource();
		PlatformManager.Instance.StartCoroutine(DispatchTaskQueue(m_cancellationTokenSource.Token));
		if (m_userManager == null)
		{
			m_userManager = new GamecoreUserManager();
		}
		m_userManager.UsersChanged += UserManager_UsersChanged;
		m_userManager.UserInfoUpdated += UserManager_UserInfoUpdated;
		AddUserWithUI();
		if (m_saveManager == null)
		{
			m_saveManager = new GamecoreSaveManager();
		}
		if (m_achievementsManager == null)
		{
			m_achievementsManager = new GamecoreAchievementsManager();
		}
	}

	public void SaveAsync(string fileName, byte[] data, Action<bool> onComplete = null)
	{
		PlatformManager.Instance.AddSaveInProgress(fileName);
		string containerName = fileName;
		string blobBufferName = "PlayerData";
		Debug.Log("[GamecorePlatformManager] Saving Container: " + containerName + ". blob Name: " + blobBufferName);
		m_saveManager.GetOrCreateContainer(containerName, delegate(int hresult)
		{
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hresult))
			{
				PlatformManager.Instance.RemoveSaveInProgress(fileName);
				onComplete?.Invoke(obj: false);
			}
			else
			{
				m_saveManager.SaveGame(containerName, blobBufferName, data, delegate(int num)
				{
					bool flag = Unity.XGamingRuntime.Interop.HR.SUCCEEDED(num);
					if (flag)
					{
						PlatformManager.Instance.CacheFile(fileName, data);
						Debug.Log("Saved (and cached) data successfully to container '" + containerName + "' and blob '" + blobBufferName + "'.");
					}
					else
					{
						Debug.LogError($"Error saving game. HRESULT=0x{num:x} (Container: {containerName}, Blob: {blobBufferName})");
					}
					PlatformManager.Instance.RemoveSaveInProgress(fileName);
					onComplete?.Invoke(flag);
				});
			}
		});
	}

	public void Update()
	{
		if (m_userManager != null)
		{
			m_userManager.Update();
		}
		if (m_achievementsManager != null)
		{
			m_achievementsManager.Update();
		}
	}

	public bool Load(string fileName, out byte[] data)
	{
		Debug.Log("[GamecorePlatformManager] Load called for " + fileName);
		if (PlatformManager.Instance.TryGetCachedFile(fileName, out var data2))
		{
			data = (byte[])data2.Clone();
			Debug.Log($"[PlatformManager] Loaded {fileName} from cache ({data.Length} bytes)");
			return true;
		}
		Debug.Log("[GamecorePlatformManager] No cached data for " + fileName + " yet. Call LoadAsync.");
		LoadAsync(fileName);
		data = null;
		return false;
	}

	public void UnlockAchievement(string achievementId)
	{
		Debug.Log("[GamecorePlatformManager] UnlockAchievement " + achievementId);
		m_achievementsManager?.UnlockAchievement(achievementId);
	}

	public void OnSceneLoaded(string sceneName)
	{
		if (sceneName == "Title")
		{
			GameObject gameObject = GameObject.Find("XboxUserTag");
			if (gameObject == null)
			{
				Debug.LogError("[GamecorePlatformManager] Could not find GameObject 'XboxUserTag' in the Title scene.");
				return;
			}
			TextMeshProUGUI component = gameObject.GetComponent<TextMeshProUGUI>();
			if (component == null)
			{
				Debug.LogError("[GamecorePlatformManager] 'XboxUserTag' exists but has no TextMeshProUGUI component attached.");
				return;
			}
			m_userTagText = component;
			m_userTagText.text = m_userManager.CurrentUserData.UserGamertag;
			SetPresence("RP_IN_MENU");
		}
		else if (sceneName == "Start")
		{
			SetPresence("RP_IN_GAME");
		}
	}

	public void OnQuit()
	{
		if (m_userManager != null)
		{
			m_userManager.UsersChanged -= UserManager_UsersChanged;
			m_userManager.UserInfoUpdated -= UserManager_UserInfoUpdated;
		}
		if (m_saveManager != null)
		{
			m_saveManager.CloseGameSaveHandles();
			m_isSavesInitialized = false;
		}
		m_cancellationTokenSource.Cancel();
		m_cancellationTokenSource.Dispose();
		Debug.Log("[GamecorePlatformManager] Uninitializing Xbox Live API.");
		SDK.XBL.XblCleanup(null);
		Debug.Log("[GamecorePlatformManager] Closing default XTaskQueue.");
		SDK.CloseDefaultXTaskQueue();
		Debug.Log("[GamecorePlatformManager] Uninitializing XGame Runtime Library.");
		SDK.XGameRuntimeUninitialize();
		s_isRuntimeInitialized = false;
	}

	public string GetWishlistUrl()
	{
		return "https://www.xbox.com/en-au/games/store/tcg-card-shop-simulator/9p9gx5d8h9dw";
	}

	public string GetFeedbackUrl()
	{
		return "https://www.xbox.com/en-au/games/store/tcg-card-shop-simulator/9p9gx5d8h9dw";
	}

	public string GetLanguage()
	{
		string locale;
		int num = SDK.XPackageGetUserLocale(out locale);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecorePlatformManager] SDK.XPackageGetUserLocale failed - 0x{num:X8}");
			return "English";
		}
		return MapCodeToLocale(locale);
	}

	private static string MapCodeToLocale(string locale)
	{
		if (locale.StartsWith("en"))
		{
			return "English";
		}
		if (locale.StartsWith("fr"))
		{
			return "French";
		}
		if (locale.StartsWith("pt"))
		{
			return "Portuguese";
		}
		if (locale.StartsWith("zh"))
		{
			if (locale.StartsWith("zh-HK") || locale.StartsWith("zh-TW"))
			{
				return "Chinese (Traditional)";
			}
			return "Chinese (Simplified)";
		}
		if (locale.StartsWith("ja"))
		{
			return "Japanese";
		}
		if (locale.StartsWith("de"))
		{
			return "German";
		}
		if (locale.StartsWith("ko"))
		{
			return "Korean";
		}
		if (locale.StartsWith("it"))
		{
			return "Italian";
		}
		if (locale.StartsWith("es"))
		{
			return "Spanish";
		}
		return "English";
	}

	public void SetPresence(string presence)
	{
		if (m_userManager == null || m_userManager.CurrentUserData.UserHandle.IsInvalid)
		{
			Debug.LogWarning("[GamecorePlatformManager] Trying to set presence with a null user.");
		}
		else
		{
			if (presence.Equals(m_currentPresence))
			{
				return;
			}
			m_currentPresence = presence;
			XblPresenceRichPresenceIds.Create(GdkPlatformSettings.gameConfigScid, presence, null, out var richPresenceIds);
			SDK.XBL.XblPresenceSetPresenceAsync(m_userManager.CurrentUserData.XblContext, isUserActiveInTitle: true, richPresenceIds, delegate(int hr)
			{
				if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
				{
					Debug.LogError($"[GamecorePlatformManager] XBL.XblPresenceSetPresenceAsync failed - 0x{hr:X8}");
				}
				else
				{
					m_currentPresence = presence;
					Debug.Log("[GamecorePlatformManager] Successfully set rich presence '" + m_currentPresence + "'");
				}
			});
		}
	}

	private static bool InitializeRuntime(bool forceInitialization = false)
	{
		if (Unity.XGamingRuntime.Interop.HR.FAILED(InitializeGamingRuntime(forceInitialization)) || !InitializeXboxLive(GdkPlatformSettings.gameConfigScid))
		{
			s_isRuntimeInitialized = false;
			return false;
		}
		int num = SDK.XGameGetXboxTitleId(out var titleId);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.Log($"FAILED: Could not get TitleID! hResult: 0x{num:x} ({Unity.XGamingRuntime.HR.NameOf(num)})");
		}
		if (!titleId.ToString("X").ToLower().Equals(GdkPlatformSettings.gameConfigTitleId.ToLower()))
		{
			Debug.LogWarning($"WARNING! Expected Title Id: {GdkPlatformSettings.gameConfigTitleId} got: {titleId:X}");
		}
		num = SDK.XSystemGetXboxLiveSandboxId(out var sandboxId);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.Log($"FAILED: Could not get SandboxID! HResult: 0x{num:x} ({Unity.XGamingRuntime.HR.NameOf(num)})");
		}
		Debug.Log("GDK Initialized, titleId: " + GdkPlatformSettings.gameConfigTitleId + ", sandboxId: " + sandboxId);
		s_isRuntimeInitialized = true;
		return true;
	}

	private static int InitializeGamingRuntime(bool forceInitialization = false)
	{
		Debug.Log("Initializing XGame Runtime Library.");
		if (s_isRuntimeInitialized && !forceInitialization)
		{
			Debug.Log("Gaming Runtime already initialized.");
			return 0;
		}
		int num = SDK.XGameRuntimeInitialize();
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.Log($"FAILED: Initialize XGameRuntime, HResult: 0x{num:X} ({Unity.XGamingRuntime.HR.NameOf(num)})");
			return num;
		}
		num = SDK.CreateDefaultTaskQueue();
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"FAILED: XTaskQueueCreate, HResult: 0x{num:X}");
			return num;
		}
		return 0;
	}

	private static bool InitializeXboxLive(string scid)
	{
		Debug.Log("Initializing Xbox Live API (SCID: " + scid + ").");
		int num = SDK.XBL.XblInitialize(scid);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num) && num != -1994173945)
		{
			Debug.Log($"FAILED: Initialize Xbox Live, HResult: 0x{num:X}, {Unity.XGamingRuntime.HR.NameOf(num)}");
			return false;
		}
		return true;
	}

	private static IEnumerator DispatchTaskQueue(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			SDK.XTaskQueueDispatch(0u);
			yield return null;
		}
	}

	private void UserManager_UsersChanged(object sender, XUserChangeEvent e)
	{
		m_usersChanged = true;
		m_achievementsManager.Initialize(m_userManager.CurrentUserData);
	}

	private void UserManager_UserInfoUpdated(object sender, UserData data)
	{
		if ((bool)m_userTagText)
		{
			m_userTagText.text = data.UserGamertag;
		}
	}

	private void AddUserWithUI()
	{
		if (m_userManager.UserDataList.Count == 0)
		{
			m_userManager.AddDefaultUserSilently(AddUserCompleted);
		}
		else
		{
			m_userManager.AddUserWithUI(AddUserCompleted);
		}
	}

	private void AddUserCompleted(GamecoreUserManager.UserOpResult result)
	{
		switch (result)
		{
		case GamecoreUserManager.UserOpResult.Success:
			m_saveManager.Initialize(m_userManager.CurrentUserData.UserHandle, GdkPlatformSettings.gameConfigScid, syncOnDemand: false, OnInitializeSaveGamesComplete);
			m_achievementsManager.Initialize(m_userManager.CurrentUserData);
			m_usersChanged = true;
			break;
		case GamecoreUserManager.UserOpResult.NoDefaultUser:
			m_userManager.AddUserWithUI(AddUserCompleted);
			break;
		case GamecoreUserManager.UserOpResult.UnknownError:
			Debug.LogError("Error adding user.");
			break;
		case GamecoreUserManager.UserOpResult.ResolveUserIssueRequired:
		case GamecoreUserManager.UserOpResult.UnclearedVetoes:
			break;
		}
	}

	private void OnInitializeSaveGamesComplete(int hresult)
	{
		if (Unity.XGamingRuntime.Interop.HR.FAILED(hresult))
		{
			if (hresult == -2138898428)
			{
				Debug.Log("XGameSave initialized in offline mode.");
				QuerySaveContainers();
				PreloadAllSaves();
				m_isSavesInitialized = true;
			}
			else
			{
				Debug.LogError($"Error when initializing XGameSave. HResult 0x{hresult:x} ({Unity.XGamingRuntime.HR.NameOf(hresult)})");
			}
		}
		else
		{
			Debug.Log("XGameSave initialized succesfully");
			QuerySaveContainers();
			PreloadAllSaves();
			m_isSavesInitialized = true;
		}
	}

	private void QuerySaveContainers()
	{
		Debug.Log("[GamecorePlatformManager] Querying slot containers...");
		QueryContainersAndBlobs("GameData", "save data");
		Debug.Log("[GamecorePlatformManager] Querying settings containers...");
		QueryContainersAndBlobs("Settings", "settings");
		Debug.Log("[GamecorePlatformManager] Querying player prefs containers...");
		QueryContainersAndBlobs("PlayerPrefs", "player prefs");
	}

	private void QueryContainersAndBlobs(string containerRoot, string label)
	{
		XGameSaveContainerInfo[] array = m_saveManager.QueryContainers(containerRoot);
		Debug.Log($"[GamecorePlatformManager] Found {array.Length} containers ({label})");
		if (!m_hasAnySaves && array.Length != 0)
		{
			m_hasAnySaves = true;
		}
		XGameSaveContainerInfo[] array2 = array;
		foreach (XGameSaveContainerInfo xGameSaveContainerInfo in array2)
		{
			Debug.Log("\tContainer: " + xGameSaveContainerInfo.Name);
			string containerName = xGameSaveContainerInfo.Name;
			m_saveManager.GetOrCreateContainer(containerName, delegate(int hresult)
			{
				if (Unity.XGamingRuntime.Interop.HR.FAILED(hresult))
				{
					Debug.LogError($"\tFailed to open container {containerName}, HRESULT=0x{hresult:X}");
				}
				else
				{
					Unity.XGamingRuntime.XGameSaveBlobInfo[] array3 = m_saveManager.QueryContainerBlobs("PlayerData");
					if (array3 == null || array3.Length == 0)
					{
						Debug.Log("\t\tNo blobs found in container '" + containerName + "'");
					}
					else
					{
						Debug.Log($"\t\tFound {array3.Length} blobs in container '{containerName}'");
						for (int j = 0; j < array3.Length; j++)
						{
							Unity.XGamingRuntime.XGameSaveBlobInfo xGameSaveBlobInfo = array3[j];
							Debug.Log($"\t\tBlob {j}: {xGameSaveBlobInfo.Name}, Size: {xGameSaveBlobInfo.Size} bytes");
						}
					}
				}
			});
		}
	}

	private void PreloadAllSaves()
	{
		PreloadFile("PlayerPrefs", delegate
		{
			PlatformManager.Instance.PlayerPrefsStorage.Load();
		});
		PreloadFile("Settings");
		PreloadSlots(0);
	}

	private void PreloadFile(string fileName, Action onComplete = null)
	{
		LoadInternalBytes(fileName, delegate(bool ok, byte[] data)
		{
			if (ok && data != null)
			{
				PlatformManager.Instance.CacheFile(fileName, data);
				Debug.Log($"[GamecorePlatformManager] Cached {fileName} as raw bytes ({data.Length} bytes)");
			}
			else
			{
				Debug.LogWarning("[GamecorePlatformManager] " + fileName + " is empty or failed to load.");
			}
			onComplete?.Invoke();
		});
	}

	private void PreloadSlots(int slotIndex)
	{
		if (slotIndex > 3)
		{
			Debug.Log("[GamecorePlatformManager] All save slots preloaded.");
			m_isPreloaded = true;
			return;
		}
		string currentFile = string.Format("{0}_{1}", "GameData", slotIndex);
		LoadInternalBytes(currentFile, delegate(bool ok, byte[] data)
		{
			if (ok && data != null)
			{
				PlatformManager.Instance.CacheFile(currentFile, data);
				Debug.Log($"[GamecorePlatformManager] Cached slot {currentFile} as raw bytes ({data.Length} bytes)");
			}
			else
			{
				Debug.LogWarning("[GamecorePlatformManager] Slot " + currentFile + " is empty or failed to load.");
			}
			PreloadSlots(slotIndex + 1);
		});
	}

	public void LoadAsync(string fileName, Action<bool, byte[]> onComplete = null)
	{
		Debug.Log("[GamecorePlatformManager] LoadAsync called for " + fileName);
		LoadInternalBytes(fileName, delegate(bool ok, byte[] data)
		{
			if (ok && data != null)
			{
				PlatformManager.Instance.CacheFile(fileName, data);
				Debug.Log($"[GamecorePlatformManager] LoadAsync: cached {fileName} ({data.Length} bytes)");
			}
			else
			{
				Debug.LogWarning("[GamecorePlatformManager] LoadAsync: Failed to load " + fileName + " or data was empty.");
			}
			onComplete?.Invoke(ok, data);
		});
	}

	private void LoadInternalBytes(string fileName, Action<bool, byte[]> onComplete = null)
	{
		string blobName = "PlayerData";
		m_saveManager.GetOrCreateContainer(fileName, delegate(int hresult)
		{
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hresult))
			{
				Debug.LogError("[GamecorePlatformManager] Failed to open container " + fileName);
				onComplete?.Invoke(arg1: false, null);
			}
			else
			{
				m_saveManager.LoadGame(blobName, delegate(int hr, XGameSaveBlob[] blobs)
				{
					if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
					{
						Debug.LogError($"[GamecorePlatformManager] Error when loading {fileName}. HResult=0x{hr:X}");
						onComplete?.Invoke(arg1: false, null);
					}
					else if (blobs == null || blobs.Length == 0)
					{
						Debug.Log("[GamecorePlatformManager] Container " + fileName + " is empty.");
						onComplete?.Invoke(arg1: false, null);
					}
					else
					{
						XGameSaveBlob xGameSaveBlob = blobs[0];
						Debug.Log($"[GamecorePlatformManager] Loaded data buffer successfully. Name: {xGameSaveBlob.Info.Name}, Size: {xGameSaveBlob.Info.Size} bytes, blobs.Length: {blobs.Length}");
						onComplete?.Invoke(arg1: true, xGameSaveBlob.Data);
					}
				});
			}
		});
	}

	public void Delete(int slotIndex)
	{
		string containerName = "GameData_" + slotIndex;
		Debug.Log($"Deleting slot {slotIndex}. Container: {containerName}.");
		m_saveManager.DeleteContainer(containerName, delegate(int hresult)
		{
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hresult))
			{
				Debug.LogError($"Error when deleting container {containerName}. HResult: {hresult:x}");
			}
			else
			{
				Debug.Log("Deleted container " + containerName);
				QuerySaveContainers();
			}
		});
	}
}
