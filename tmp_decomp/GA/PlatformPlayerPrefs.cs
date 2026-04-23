using System;

namespace GA;

public static class PlatformPlayerPrefs
{
	private static PlatformPlayerPrefsStorage _storage;

	private static PlatformPlayerPrefsStorage Storage
	{
		get
		{
			if (_storage == null)
			{
				throw new InvalidOperationException("PlatformPlayerPrefs is not initialized. Call PlatformPlayerPrefs.Init(...) during platform initialization (PlatformManager.Initialize).");
			}
			return _storage;
		}
	}

	public static void Init(PlatformPlayerPrefsStorage storage)
	{
		if (storage == null)
		{
			throw new ArgumentNullException("storage");
		}
		_storage = storage;
	}

	public static void SetInt(string key, int value)
	{
		Storage.Data.ints[key] = value;
		Storage.MarkDirty();
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		if (!Storage.Data.ints.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public static void SetFloat(string key, float value)
	{
		Storage.Data.floats[key] = value;
		Storage.MarkDirty();
	}

	public static float GetFloat(string key, float defaultValue = 0f)
	{
		if (!Storage.Data.floats.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public static void SetString(string key, string value)
	{
		Storage.Data.strings[key] = value;
		Storage.MarkDirty();
	}

	public static string GetString(string key, string defaultValue = "")
	{
		if (!Storage.Data.strings.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public static bool HasKey(string key)
	{
		if (!Storage.Data.ints.ContainsKey(key) && !Storage.Data.floats.ContainsKey(key))
		{
			return Storage.Data.strings.ContainsKey(key);
		}
		return true;
	}

	public static void DeleteKey(string key)
	{
		if (Storage.Data.ints.Remove(key) | Storage.Data.floats.Remove(key) | Storage.Data.strings.Remove(key))
		{
			Storage.MarkDirty();
		}
	}

	public static void Save()
	{
		Storage.Save();
	}
}
