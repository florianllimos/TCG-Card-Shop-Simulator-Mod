using System;
using UnityEngine;

namespace GA;

public class PSPlatformManager : IPlatformManager
{
	public bool IsInitialized => true;

	public bool HasAnySaves => false;

	public void Initialize()
	{
		Debug.Log("[PSPlatformManager] Initialize");
	}

	public void Update()
	{
	}

	public void SaveAsync(string fileName, byte[] data, Action<bool> onComplete = null)
	{
		Debug.Log("[PSPlatformManager] SaveAsync called for " + fileName);
	}

	public bool Load(string fileName, out byte[] data)
	{
		Debug.Log("[PSPlatformManager] Load called for " + fileName);
		data = null;
		return true;
	}

	public void UnlockAchievement(string achievementId)
	{
		Debug.Log("[PSPlatformManager] UnlockAchievement " + achievementId);
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
