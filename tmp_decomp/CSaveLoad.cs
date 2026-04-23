using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using GA;
using UnityEngine;

public static class CSaveLoad
{
	public static CGameData m_SavedGame = new CGameData();

	public static CGameData m_SavedGameBackup = new CGameData();

	public static CSettingData m_SavedSetting = new CSettingData();

	private const string m_FileName = "/savedGames_Release";

	private const string m_JSONFileNameExt = ".json";

	private const string m_BackupFileName = "/savedGames_ReleaseBackupFile";

	private const string m_CloudFileName = "/savedGames_Debug02.gd";

	private const string m_SettingFileName = "/savedGames_KeybindSetting.gd";

	private static int m_ValidateSaveCount = 0;

	public static void AutoSaveMoveToEmptySaveSlot()
	{
		Debug.Log("[ga] TODO: AutoSaveMoveToEmptySaveSlot");
		if (PlatformManager.Instance.UseNativeSaves())
		{
			return;
		}
		string text = Application.persistentDataPath + "/savedGames_Release0.json";
		if (File.Exists(text))
		{
			string text2 = "";
			if (!File.Exists(Application.persistentDataPath + "/savedGames_Release1.json") && !File.Exists(Application.persistentDataPath + "/savedGames_Release1.gd"))
			{
				text2 = Application.persistentDataPath + "/savedGames_Release1.json";
				File.Copy(text, text2);
			}
			else if (!File.Exists(Application.persistentDataPath + "/savedGames_Release2.json") && !File.Exists(Application.persistentDataPath + "/savedGames_Release2.gd"))
			{
				text2 = Application.persistentDataPath + "/savedGames_Release2.json";
				File.Copy(text, text2);
			}
			else if (!File.Exists(Application.persistentDataPath + "/savedGames_Release3.json") && !File.Exists(Application.persistentDataPath + "/savedGames_Release3.gd"))
			{
				text2 = Application.persistentDataPath + "/savedGames_Release3.json";
				File.Copy(text, text2);
			}
		}
	}

	public static void Save(int saveSlotIndex, bool skipJSONSave = false)
	{
		Debug.Log("Save saveSlotIndex " + saveSlotIndex);
		m_SavedGame = CGameData.instance;
		if (PlatformManager.Instance.UseNativeSaves())
		{
			SaveNative(string.Format("{0}_{1}", "GameData", saveSlotIndex), m_SavedGame);
			m_SavedGameBackup = m_SavedGame;
			if (saveSlotIndex == 0)
			{
				CEventManager.QueueEvent(new CEventPlayer_OnSaveStatusUpdated(isSuccess: true, isAutosaving: false));
			}
			return;
		}
		string text = Application.persistentDataPath + "/savedGames_Release" + saveSlotIndex + ".json";
		string text2 = Application.persistentDataPath + "/savedGames_ReleaseBackupFile" + saveSlotIndex + ".json";
		string text3 = Application.persistentDataPath + "Temp" + saveSlotIndex + ".json";
		try
		{
			using FileStream fileStream = File.Create(Application.persistentDataPath + "/savedGames_Release" + saveSlotIndex + ".gd");
			new BinaryFormatter().Serialize(fileStream, m_SavedGame);
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Error saving gd");
		}
		try
		{
			string contents = JsonUtility.ToJson(m_SavedGame);
			File.WriteAllText(text3, contents);
		}
		catch
		{
			Debug.Log("Error saving JSON");
		}
		if (ValidateSavedSlotData(text3))
		{
			if (File.Exists(text))
			{
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				File.Move(text, text2);
			}
			if (File.Exists(text3))
			{
				File.Move(text3, text);
			}
			m_ValidateSaveCount = 0;
			Debug.Log("Save process done");
			if (saveSlotIndex == 0)
			{
				CEventManager.QueueEvent(new CEventPlayer_OnSaveStatusUpdated(isSuccess: true, isAutosaving: false));
			}
		}
		else
		{
			m_ValidateSaveCount++;
			Debug.Log("ValidateSavedSlotData Save file invalid, try resave m_ValidateSaveCount " + m_ValidateSaveCount);
			if (m_ValidateSaveCount < 10)
			{
				Save(saveSlotIndex, skipJSONSave: true);
				return;
			}
			Debug.Log("Error - Cannot save");
			CEventManager.QueueEvent(new CEventPlayer_OnSaveStatusUpdated(isSuccess: false, isAutosaving: false));
		}
	}

	public static bool Load(int slotIndex)
	{
		if (PlatformManager.Instance.UseNativeSaves())
		{
			return LoadNative(string.Format("{0}_{1}", "GameData", slotIndex), ref m_SavedGame);
		}
		bool flag = false;
		string path = Application.persistentDataPath + "/savedGames_Release" + slotIndex + ".gd";
		string path2 = Application.persistentDataPath + "/savedGames_Release" + slotIndex + ".json";
		if (File.Exists(path2))
		{
			try
			{
				string text = File.ReadAllText(path2);
				if (!(text == "") && text != null)
				{
					m_SavedGame = JsonUtility.FromJson<CGameData>(text);
					return true;
				}
				flag = true;
			}
			catch
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			if (File.Exists(path))
			{
				using (FileStream fileStream = File.Open(path, FileMode.Open))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					if (fileStream.Length > 0)
					{
						try
						{
							fileStream.Position = 0L;
							m_SavedGame = (CGameData)binaryFormatter.Deserialize(fileStream);
							fileStream.Close();
							binaryFormatter = null;
							return true;
						}
						catch
						{
							fileStream.Close();
							binaryFormatter = null;
							return false;
						}
					}
					fileStream.Close();
					binaryFormatter = null;
					return false;
				}
			}
			return false;
		}
		return false;
	}

	public static bool LoadBackup(int slotIndex)
	{
		string path = Application.persistentDataPath + "/savedGames_ReleaseBackupFile" + slotIndex + ".json";
		if (File.Exists(path))
		{
			try
			{
				m_SavedGame = JsonUtility.FromJson<CGameData>(File.ReadAllText(path));
				return true;
			}
			catch
			{
				return false;
			}
		}
		return false;
	}

	public static void SaveSetting()
	{
		m_SavedSetting = CSettingData.instance;
		if (PlatformManager.Instance.UseNativeSaves())
		{
			SaveNative("Settings", m_SavedSetting);
			return;
		}
		using FileStream fileStream = File.Create(Application.persistentDataPath + "/savedGames_KeybindSetting.gd");
		new BinaryFormatter().Serialize(fileStream, m_SavedSetting);
		fileStream.Close();
	}

	public static bool LoadSetting()
	{
		if (PlatformManager.Instance.UseNativeSaves())
		{
			return LoadNative("Settings", ref m_SavedSetting);
		}
		if (File.Exists(Application.persistentDataPath + "/savedGames_KeybindSetting.gd"))
		{
			using (FileStream fileStream = File.Open(Application.persistentDataPath + "/savedGames_KeybindSetting.gd", FileMode.Open))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				if (fileStream.Length > 0)
				{
					try
					{
						fileStream.Position = 0L;
						m_SavedSetting = (CSettingData)binaryFormatter.Deserialize(fileStream);
						fileStream.Close();
						binaryFormatter = null;
						return true;
					}
					catch
					{
						fileStream.Close();
						binaryFormatter = null;
						return false;
					}
				}
				fileStream.Close();
				binaryFormatter = null;
				return false;
			}
		}
		return false;
	}

	public static void Delete()
	{
		if (File.Exists(Application.persistentDataPath + "/savedGames_Release"))
		{
			File.Delete(Application.persistentDataPath + "/savedGames_Release");
		}
	}

	public static void DeleteBackup()
	{
		if (File.Exists(Application.persistentDataPath + "/savedGames_ReleaseBackupFile0.gd"))
		{
			File.Delete(Application.persistentDataPath + "/savedGames_ReleaseBackupFile0.gd");
		}
	}

	public static void DeleteCloudFile()
	{
		if (File.Exists(Application.persistentDataPath + "/savedGames_Debug02.gd"))
		{
			File.Delete(Application.persistentDataPath + "/savedGames_Debug02.gd");
		}
	}

	public static byte[] GetLocalSaveFileAsByteArray()
	{
		if (!File.Exists(Application.persistentDataPath + "/savedGames_Release"))
		{
			return null;
		}
		return File.ReadAllBytes(Application.persistentDataPath + "/savedGames_Release");
	}

	public static void LoadCloudFile(byte[] data)
	{
		Debug.Log("DSCloudSaveLoadTest LoadingCloudFile");
		try
		{
			using FileStream fileStream = new FileStream(Application.persistentDataPath + "/savedGames_Debug02.gd", FileMode.Create, FileAccess.Write);
			fileStream.Write(data, 0, data.Length);
			fileStream.Close();
			if (!File.Exists(Application.persistentDataPath + "/savedGames_Debug02.gd"))
			{
				return;
			}
			using FileStream fileStream2 = File.Open(Application.persistentDataPath + "/savedGames_Debug02.gd", FileMode.Open);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			if (fileStream2.Length > 0)
			{
				try
				{
					fileStream2.Position = 0L;
					m_SavedGameBackup = (CGameData)binaryFormatter.Deserialize(fileStream2);
					fileStream2.Close();
					binaryFormatter = null;
					Debug.Log("DSCloudSaveLoadTest LoadCloudFile sucess");
					return;
				}
				catch
				{
					fileStream2.Close();
					binaryFormatter = null;
					Debug.Log("DSCloudSaveLoadTest LoadCloudFile failed to deserialize");
					return;
				}
			}
			fileStream2.Close();
			binaryFormatter = null;
			Debug.Log("DSCloudSaveLoadTest LoadCloudFile failed, file length 0");
		}
		catch
		{
			Debug.Log("DSCloudSaveLoadTest LoadCloudFile failed");
		}
	}

	public static bool HasSaveFile(int saveloadSlotIndex)
	{
		return File.Exists(Application.persistentDataPath + "/savedGames_Release" + saveloadSlotIndex + ".json");
	}

	public static bool LoadSavedSlotData(int slotIndex)
	{
		if (PlatformManager.Instance.UseNativeSaves())
		{
			return LoadNative(string.Format("{0}_{1}", "GameData", slotIndex), ref m_SavedGameBackup);
		}
		bool flag = false;
		string path = Application.persistentDataPath + "/savedGames_Release" + slotIndex + ".gd";
		string path2 = Application.persistentDataPath + "/savedGames_Release" + slotIndex + ".json";
		if (File.Exists(path2))
		{
			try
			{
				string text = File.ReadAllText(path2);
				if (!(text == "") && text != null)
				{
					m_SavedGameBackup = JsonUtility.FromJson<CGameData>(text);
					return true;
				}
				flag = true;
			}
			catch
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			if (File.Exists(path))
			{
				using (FileStream fileStream = File.Open(path, FileMode.Open))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					if (fileStream.Length > 0)
					{
						try
						{
							fileStream.Position = 0L;
							m_SavedGameBackup = (CGameData)binaryFormatter.Deserialize(fileStream);
							fileStream.Close();
							binaryFormatter = null;
							return true;
						}
						catch
						{
							fileStream.Close();
							binaryFormatter = null;
							return false;
						}
					}
					fileStream.Close();
					binaryFormatter = null;
					return false;
				}
			}
			return false;
		}
		return false;
	}

	public static bool ValidateSavedSlotData(string filePath)
	{
		if (File.Exists(filePath))
		{
			try
			{
				m_SavedGameBackup = JsonUtility.FromJson<CGameData>(File.ReadAllText(filePath));
				return true;
			}
			catch
			{
				return false;
			}
		}
		return false;
	}

	private static bool SaveNative<T>(string fileName, T data) where T : class
	{
		byte[] array = null;
		try
		{
			string s = JsonUtility.ToJson(data);
			array = Encoding.UTF8.GetBytes(s);
		}
		catch
		{
			Debug.LogError("CSaveLoad::SaveNative() Failed to serialize data for native save '" + fileName + "'");
			return false;
		}
		PlatformManager.Instance?.SaveAsync(fileName, array, delegate
		{
			Debug.Log("CSaveLoad::SaveNative() Successfully saved to platform storage: " + fileName);
		});
		return true;
	}

	private static bool LoadNative<T>(string fileName, ref T target) where T : class
	{
		if (!PlatformManager.Instance.Load(fileName, out var data) || data == null || data.Length == 0)
		{
			Debug.LogWarning("CSaveLoad::LoadNative() load failed or was empty: " + fileName);
			return false;
		}
		try
		{
			string json = Encoding.UTF8.GetString(data);
			target = JsonUtility.FromJson<T>(json);
		}
		catch
		{
			Debug.LogError("CSaveLoad::LoadNative() Failed to deserialize platform save '" + fileName + "'");
			return false;
		}
		Debug.Log("CSaveLoad::LoadNative() Successfully loaded from platform storage: " + fileName);
		return true;
	}
}
