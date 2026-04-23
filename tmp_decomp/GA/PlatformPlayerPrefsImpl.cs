namespace GA;

public class PlatformPlayerPrefsImpl : IPlatformPlayerPrefs
{
	public int GetInt(string key, int defaultValue)
	{
		return PlatformPlayerPrefs.GetInt(key, defaultValue);
	}

	public void SetInt(string key, int value)
	{
		PlatformPlayerPrefs.SetInt(key, value);
	}

	public float GetFloat(string key, float defaultValue)
	{
		return PlatformPlayerPrefs.GetFloat(key, defaultValue);
	}

	public void SetFloat(string key, float value)
	{
		PlatformPlayerPrefs.SetFloat(key, value);
	}

	public string GetString(string key, string defaultValue)
	{
		return PlatformPlayerPrefs.GetString(key, defaultValue);
	}

	public void SetString(string key, string value)
	{
		PlatformPlayerPrefs.SetString(key, value);
	}

	public bool HasKey(string key)
	{
		return PlatformPlayerPrefs.HasKey(key);
	}

	public void DeleteKey(string key)
	{
		PlatformPlayerPrefs.DeleteKey(key);
	}

	public void Save()
	{
		PlatformPlayerPrefs.Save();
	}
}
