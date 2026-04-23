using System.Collections.Generic;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;
using UnityEngine;
using UnityEngine.Events;

namespace GA;

public class GamecoreSaveManager
{
	private const int E_GAMERUNTIME_INVALID_HANDLE = -1994129140;

	private XUserHandle m_userHandle;

	private XGameSaveProviderHandle m_gameSaveProviderHandle;

	private XGameSaveContainerHandle m_gameSaveContainerHandle;

	private XGameSaveUpdateHandle m_gameSaveContainerUpdateHandle;

	~GamecoreSaveManager()
	{
		SDK.XGameSaveCloseProvider(m_gameSaveProviderHandle);
		SDK.XUserCloseHandle(m_userHandle);
	}

	public void Initialize(XUserHandle userHandle, string scid, bool syncOnDemand, UnityAction<int> onInitializationComplete)
	{
		m_userHandle = null;
		m_gameSaveProviderHandle = null;
		int num = SDK.XUserDuplicateHandle(userHandle, out m_userHandle);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			onInitializationComplete(num);
			return;
		}
		SDK.XGameSaveInitializeProviderAsync(m_userHandle, scid, syncOnDemand, delegate(int hresult, XGameSaveProviderHandle gameSaveProviderHandle)
		{
			OnSaveGameInitialized(hresult, gameSaveProviderHandle, onInitializationComplete);
		});
	}

	private void OnSaveGameInitialized(int hresult, XGameSaveProviderHandle gameSaveProviderHandle, UnityAction<int> onInitializatioComplete)
	{
		if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hresult) || hresult == -2138898428)
		{
			m_gameSaveProviderHandle = gameSaveProviderHandle;
		}
		onInitializatioComplete?.Invoke(hresult);
	}

	public void SaveGame(string displayName, string blobBufferName, byte[] blobData, UnityAction<int> onSaveGameCompleted)
	{
		SaveGame(displayName, new string[1] { blobBufferName }, new List<byte[]> { blobData }, onSaveGameCompleted);
	}

	public void SaveGame(string displayName, string[] blobBufferNames, List<byte[]> blobDataList, UnityAction<int> onSaveGameCompleted)
	{
		if (m_gameSaveContainerHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] You have not created or retrieved a container. Not doing aything.");
			return;
		}
		int num = StartContainerUpdate(displayName);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			onSaveGameCompleted?.Invoke(num);
			return;
		}
		for (int i = 0; i < blobBufferNames.Length; i++)
		{
			num = SubmitDataBlobToWrite(blobBufferNames[i], blobDataList[i]);
			if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
			{
				onSaveGameCompleted?.Invoke(num);
				return;
			}
		}
		SubmitGameSaveUpdate(onSaveGameCompleted);
	}

	public void GetOrCreateContainer(string containerName, UnityAction<int> onContainerCreated)
	{
		if (m_gameSaveProviderHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] Game Save Provider not initialized. Not doing anything.");
			onContainerCreated?.Invoke(-1);
			return;
		}
		int num = SDK.XGameSaveCreateContainer(m_gameSaveProviderHandle, containerName, out m_gameSaveContainerHandle);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when creating the {containerName} container. HResult: 0x{num:x}");
			onContainerCreated?.Invoke(num);
		}
		else
		{
			onContainerCreated?.Invoke(num);
		}
	}

	public int StartContainerUpdate(string containerDisplayName)
	{
		int num = SDK.XGameSaveCreateUpdate(m_gameSaveContainerHandle, containerDisplayName, out m_gameSaveContainerUpdateHandle);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when creating the {containerDisplayName} update process. HResult: 0x{num:x}");
			return num;
		}
		Debug.Log("[GamecoreSaveManager] Container " + containerDisplayName + " update process created.");
		return num;
	}

	public int SubmitDataBlobToWrite(string blobName, byte[] data)
	{
		if (m_gameSaveContainerUpdateHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] You have not started a Update save process. not doing anything");
			return -1;
		}
		int num = SDK.XGameSaveSubmitBlobWrite(m_gameSaveContainerUpdateHandle, blobName, data);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when submitting the blob {blobName}. HResult: 0x{num:x}");
			return num;
		}
		Debug.Log("[GamecoreSaveManager] Blob " + blobName + " submitted.");
		return num;
	}

	public void SubmitGameSaveUpdate(UnityAction<int> onSubmitGameSaveComplete)
	{
		if (m_gameSaveContainerUpdateHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] You have not started a Update save process. not doing anything");
			onSubmitGameSaveComplete?.Invoke(-1);
		}
		else
		{
			SDK.XGameSaveSubmitUpdateAsync(m_gameSaveContainerUpdateHandle, delegate(int hresult)
			{
				OnSubmitUpdateCompleted(hresult, onSubmitGameSaveComplete);
			});
		}
	}

	private void OnSubmitUpdateCompleted(int hresult, UnityAction<int> onSubmitGameSaveComplete)
	{
		if (Unity.XGamingRuntime.Interop.HR.FAILED(hresult))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when submitting container updated process. HResult: 0x{hresult:x}");
			onSubmitGameSaveComplete?.Invoke(hresult);
			return;
		}
		Debug.Log("[GamecoreSaveManager] Update process submitted. Closing Update handle and container.");
		SDK.XGameSaveCloseUpdate(m_gameSaveContainerUpdateHandle);
		SDK.XGameSaveCloseContainer(m_gameSaveContainerHandle);
		onSubmitGameSaveComplete?.Invoke(hresult);
	}

	public void LoadGame(string blobBufferName, UnityAction<int, XGameSaveBlob[]> onLoadGameCompleted)
	{
		if (m_gameSaveContainerHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] You have not created or retrieved a container. Not doing aything.");
			return;
		}
		LoadGame(new string[1] { blobBufferName }, onLoadGameCompleted);
	}

	public void LoadGame(string[] blobBufferNames, UnityAction<int, XGameSaveBlob[]> onLoadGameCompleted)
	{
		if (m_gameSaveContainerHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] You have not created or retrieved a container. Not doing aything.");
			return;
		}
		SDK.XGameSaveReadBlobDataAsync(m_gameSaveContainerHandle, blobBufferNames, delegate(int hresult, XGameSaveBlob[] blobs)
		{
			OnLoadSaveGameCompleted(hresult, blobs, onLoadGameCompleted);
		});
	}

	private void OnLoadSaveGameCompleted(int hresult, XGameSaveBlob[] blobs, UnityAction<int, XGameSaveBlob[]> onLoadGameCompleted)
	{
		if (Unity.XGamingRuntime.Interop.HR.FAILED(hresult))
		{
			if (hresult == -1994129140 || hresult == -2138898424)
			{
				Debug.Log($"[GamecoreSaveManager] No data in slot yet (empty container). HResult: 0x{hresult:x}");
				onLoadGameCompleted?.Invoke(0, new XGameSaveBlob[0]);
			}
			else
			{
				Debug.LogError($"[GamecoreSaveManager] Error when loading save game. HResult: 0x{hresult:x}");
				_ = -2147467259;
				onLoadGameCompleted?.Invoke(0, new XGameSaveBlob[0]);
			}
		}
		else
		{
			onLoadGameCompleted?.Invoke(hresult, blobs);
		}
	}

	public void QuerySpaceQuota(UnityAction<int, long> onSpaceQuotaRequested)
	{
		SDK.XGameSaveGetRemainingQuotaAsync(m_gameSaveProviderHandle, delegate(int hresult, long remainingQuota)
		{
			onSpaceQuotaRequested?.Invoke(hresult, remainingQuota);
		});
	}

	public XGameSaveContainerInfo[] QueryContainers()
	{
		if (m_gameSaveProviderHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] Game Save Provider not initialized. Not doing anything.");
			return null;
		}
		XGameSaveContainerInfo[] localInfos;
		int num = SDK.XGameSaveEnumerateContainerInfo(m_gameSaveProviderHandle, out localInfos);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when enumerating containers. HResult: 0x{num:x}");
			return null;
		}
		return localInfos;
	}

	public XGameSaveContainerInfo[] QueryContainers(string containerPrefix)
	{
		if (m_gameSaveProviderHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] Game Save Provider not initialized. Not doing anything.");
			return null;
		}
		XGameSaveContainerInfo[] localInfos;
		int num = SDK.XGameSaveEnumerateContainerInfoByName(m_gameSaveProviderHandle, containerPrefix, out localInfos);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when enumerating containers by name. HResult: 0x{num:x}");
			return null;
		}
		return localInfos;
	}

	public Unity.XGamingRuntime.XGameSaveBlobInfo[] QueryContainerBlobs()
	{
		if (m_gameSaveContainerHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] You have not created or retrieved a container. Not doing anything.");
			return null;
		}
		Unity.XGamingRuntime.XGameSaveBlobInfo[] blobInfos;
		int num = SDK.XGameSaveEnumerateBlobInfo(m_gameSaveContainerHandle, out blobInfos);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when enumerating blobs. HResult: 0x{num:x}");
			return null;
		}
		return blobInfos;
	}

	public Unity.XGamingRuntime.XGameSaveBlobInfo[] QueryContainerBlobs(string blobPrefix)
	{
		if (m_gameSaveContainerHandle == null)
		{
			Debug.LogWarning("[GamecoreSaveManager] You have not created or retrieved a container. Not doing aything.");
			return null;
		}
		Unity.XGamingRuntime.XGameSaveBlobInfo[] blobInfos;
		int num = SDK.XGameSaveEnumerateBlobInfoByName(m_gameSaveContainerHandle, blobPrefix, out blobInfos);
		if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
		{
			Debug.LogError($"[GamecoreSaveManager] Error when enumerating blobs by name. HResult: 0x{num:x}");
			return null;
		}
		return blobInfos;
	}

	public void DeleteContainer(string containerName, UnityAction<int> onDeleteContainercomplete)
	{
		SDK.XGameSaveDeleteContainerAsync(m_gameSaveProviderHandle, containerName, delegate(int hresult)
		{
			onDeleteContainercomplete?.Invoke(hresult);
		});
	}

	public void CloseGameSaveHandles()
	{
		if (m_gameSaveContainerHandle != null)
		{
			Debug.Log("[GamecoreSaveManager] Closing Container handle");
			SDK.XGameSaveCloseContainer(m_gameSaveContainerHandle);
			m_gameSaveContainerHandle = null;
		}
		if (m_gameSaveContainerUpdateHandle != null)
		{
			Debug.Log("[GamecoreSaveManager] Closing Update handle");
			SDK.XGameSaveCloseUpdate(m_gameSaveContainerUpdateHandle);
			m_gameSaveContainerUpdateHandle = null;
		}
		if (m_gameSaveProviderHandle != null)
		{
			Debug.Log("[GamecoreSaveManager] Closing Game Save provider handle");
			SDK.XGameSaveCloseProvider(m_gameSaveProviderHandle);
			m_gameSaveProviderHandle = null;
		}
	}
}
