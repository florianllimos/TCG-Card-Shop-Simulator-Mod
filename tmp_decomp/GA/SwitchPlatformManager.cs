using System;
using UnityEngine;

namespace GA;

public class SwitchPlatformManager : IPlatformManager
{
	public bool IsInitialized => true;

	public bool HasAnySaves => true;

	public void Initialize()
	{
		Debug.Log("[SwitchPlatformManager] Initialize");
	}

	public void Update()
	{
	}

	public void SaveAsync(string fileName, byte[] data, Action<bool> onComplete = null)
	{
		Debug.Log("[SwitchPlatformManager] SaveAsync called for " + fileName);
	}

	public bool Load(string fileName, out byte[] data)
	{
		Debug.Log("[SwitchPlatformManager] Load called for " + fileName);
		data = null;
		return true;
	}

	public void UnlockAchievement(string achievementId)
	{
		Debug.Log("[SwitchPlatformManager] UnlockAchievement " + achievementId + ". Not supported.");
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
