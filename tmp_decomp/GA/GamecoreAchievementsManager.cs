using System;
using System.Collections.Generic;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;
using UnityEngine;

namespace GA;

public class GamecoreAchievementsManager
{
	private static readonly Dictionary<string, string> s_achievementMap = new Dictionary<string, string>
	{
		{ "ACH_UnlockBasicCardBox", "1" },
		{ "ACH_UnlockRareCardPack", "2" },
		{ "ACH_UnlockEpicCardPack", "3" },
		{ "ACH_UnlockLegendCardPack", "4" },
		{ "ACH_UnlockDestinyBasicPack", "5" },
		{ "ACH_UnlockDestinyRarePack", "6" },
		{ "ACH_UnlockDestinyEpicPack", "7" },
		{ "ACH_UnlockDestinyLegendPack", "8" },
		{ "ACH_ShopLevel1", "9" },
		{ "ACH_ShopLevel2", "10" },
		{ "ACH_ShopLevel3", "11" },
		{ "ACH_ShopLevel4", "12" },
		{ "ACH_Worker1", "13" },
		{ "ACH_Worker2", "14" },
		{ "ACH_Worker3", "15" },
		{ "ACH_OpenPack1", "16" },
		{ "ACH_OpenPack2", "17" },
		{ "ACH_OpenPack3", "18" },
		{ "ACH_CustomerPlay1", "19" },
		{ "ACH_CustomerPlay2", "20" },
		{ "ACH_CustomerPlay3", "21" },
		{ "ACH_SellCard1", "22" },
		{ "ACH_SellCard2", "23" },
		{ "ACH_SellCard3", "24" },
		{ "ACH_Profit1", "25" },
		{ "ACH_Profit2", "26" },
		{ "ACH_Profit3", "27" },
		{ "ACH_Checkout", "28" },
		{ "ACH_FullArtFoil", "29" },
		{ "ACH_CardCollector1", "30" },
		{ "ACH_CardCollector2", "31" },
		{ "ACH_CardCollector3", "32" },
		{ "ACH_CardCollector4", "33" },
		{ "ACH_SmellyClean1", "34" },
		{ "ACH_SmellyClean2", "35" },
		{ "ACH_UnlockShopLotB", "36" },
		{ "ACH_ShopExpansion1", "37" },
		{ "ACH_ShopExpansion2", "38" },
		{ "ACH_ShopExpansion3", "39" },
		{ "ACH_GhostFoil", "40" },
		{ "ACH_GradingGem1", "41" },
		{ "ACH_GradingGem2", "42" },
		{ "ACH_GradingGem3", "43" },
		{ "ACH_GradingCollector", "44" },
		{ "ACH_GradingGold", "45" }
	};

	private static readonly Dictionary<string, uint> s_incrementalAchievements = new Dictionary<string, uint>();

	private Queue<GamecoreAchievement> m_achievementQueue = new Queue<GamecoreAchievement>();

	private Dictionary<string, bool> m_unlockedAchievements = new Dictionary<string, bool>();

	private float m_achievementsUpdateDelay = 0.5f;

	private float m_timeSinceLastAchievementsUpdate;

	private bool m_isAchievementsInitialised;

	private UserData m_userData;

	public void Initialize(UserData userData)
	{
		m_isAchievementsInitialised = false;
		m_userData = userData;
		m_unlockedAchievements.Clear();
		m_achievementQueue.Clear();
		GetAchievements();
		m_isAchievementsInitialised = true;
	}

	public void Update()
	{
		if (m_isAchievementsInitialised && m_achievementQueue.Count > 0)
		{
			m_timeSinceLastAchievementsUpdate += Time.deltaTime;
			if (m_timeSinceLastAchievementsUpdate >= m_achievementsUpdateDelay)
			{
				GamecoreAchievement achievement = m_achievementQueue.Dequeue();
				UpdateAchievement(achievement);
				m_timeSinceLastAchievementsUpdate = 0f;
			}
		}
	}

	public void UnlockAchievement(string id)
	{
		GamecoreAchievement item = new GamecoreAchievement(id, 100.0);
		if (!m_achievementQueue.Contains(item))
		{
			m_achievementQueue.Enqueue(item);
		}
	}

	public void SetStat(string id, double value)
	{
		UpdateIncrementalAchievement(id, value);
	}

	private void UpdateAchievement(GamecoreAchievement achievement)
	{
		uint num = CalculatePercentage(achievement);
		if (!s_achievementMap.TryGetValue(achievement.Id, out var value))
		{
			Debug.LogError("[GamecoreAchievementsManager] UpdateAchievement Xbox achievement not found for id '" + achievement.Id + "'");
			return;
		}
		SDK.XBL.XblAchievementsUpdateAchievementAsync(m_userData.XblContext, m_userData.UserXUID, value, num, delegate(int hr)
		{
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
			{
				if (hr == -2145844944)
				{
					Debug.Log("[GamecoreAchievementsManager] Values cannot be modified to be less than or equal " + $"to the current achievement value. Achievement {achievement} has not been updated");
				}
				else
				{
					Debug.Log($"[GamecoreAchievementsManager] XBL.XblAchievementsUpdateAchievementAsync failed - 0x{hr:X8}");
				}
			}
			else
			{
				Debug.Log($"[GamecoreAchievementsManager] Successfully updated achievement status for achievement {achievement}");
			}
		});
		if (num >= 100)
		{
			m_unlockedAchievements.TryAdd(achievement.Id, value: true);
		}
	}

	private void UpdateIncrementalAchievement(string id, double value)
	{
		GamecoreAchievement gamecoreAchievement = null;
		bool flag = false;
		foreach (GamecoreAchievement item2 in m_achievementQueue)
		{
			if (item2.Id == id)
			{
				gamecoreAchievement = item2;
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (value > gamecoreAchievement.Value)
			{
				gamecoreAchievement.Value = value;
			}
		}
		else
		{
			GamecoreAchievement item = new GamecoreAchievement(id, value);
			m_achievementQueue.Enqueue(item);
		}
	}

	private void GetAchievements()
	{
		uint titleId = Convert.ToUInt32(GdkPlatformSettings.gameConfigTitleId, 16);
		SDK.XBL.XblAchievementsGetAchievementsForTitleIdAsync(m_userData.XblContext, m_userData.UserXUID, titleId, XblAchievementType.All, unlockedOnly: false, XblAchievementOrderBy.DefaultOrder, 0u, 2147483647u, delegate(int hr, XblAchievementsResultHandle result)
		{
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
			{
				Debug.LogError($"[GamecoreAchievementsManager] XBL.XblAchievementsGetAchievementsForTitleIdAsync failed - 0x{hr:X8}");
			}
			else
			{
				Debug.Log("[GamecoreAchievementsManager] Successfully retrieved achievements for TitleId '" + GdkPlatformSettings.gameConfigTitleId + "'");
				hr = SDK.XBL.XblAchievementsResultGetAchievements(result, out var achievements);
				if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
				{
					Debug.Log($"[GamecoreAchievementsManager] XBL.XblAchievementsResultGetAchievements failed - 0x{hr:X8}");
				}
				else
				{
					Debug.Log($"[GamecoreAchievementsManager] Total Achievements Found: {achievements.Length}");
					foreach (XblAchievement xblAchievement in achievements)
					{
						if (xblAchievement.ProgressState == XblAchievementProgressState.Achieved)
						{
							m_unlockedAchievements.TryAdd(xblAchievement.Id, value: true);
						}
						Debug.Log($"[GamecoreAchievementsManager] Found achievement '{xblAchievement.Name}' ({xblAchievement.Id}): {xblAchievement.ProgressState}");
					}
				}
			}
		});
	}

	private uint CalculatePercentage(GamecoreAchievement achievement)
	{
		if (s_incrementalAchievements.TryGetValue(achievement.Id, out var value) && value != 0 && achievement.Value > 0.0)
		{
			if (achievement.Value >= (double)value)
			{
				return 100u;
			}
			return (uint)(achievement.Value / (double)value * 100.0);
		}
		return 100u;
	}
}
