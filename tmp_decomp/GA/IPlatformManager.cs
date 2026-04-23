using System;

namespace GA;

public interface IPlatformManager
{
	bool IsInitialized { get; }

	bool HasAnySaves { get; }

	void Initialize();

	void Update();

	void SaveAsync(string fileName, byte[] data, Action<bool> onComplete = null);

	bool Load(string fileName, out byte[] data);

	void UnlockAchievement(string achievementId);

	void OnSceneLoaded(string sceneName);

	void OnQuit();

	string GetWishlistUrl();

	string GetFeedbackUrl();

	string GetLanguage();

	void SetPresence(string presence);
}
