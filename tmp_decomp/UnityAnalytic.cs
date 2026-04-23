using Unity.Services.Analytics;
using UnityEngine;

public class UnityAnalytic : CSingleton<UnityAnalytic>
{
	public static UnityAnalytic m_Instance;

	private static bool m_CanUseAnalytic;

	private bool m_HasStart;

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(this);
	}

	private void StartCollection()
	{
		if (!m_HasStart)
		{
			m_HasStart = true;
		}
	}

	public static void StartTutorial(int index)
	{
	}

	public static void SetUserSegmentIndex(int index)
	{
	}

	public static void JoinDiscord()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("JoinDiscord");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PressFeedback(int index)
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PressFeedback") { { "Index", index } };
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PressWishlist(int index)
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PressWishlist") { { "Index", index } };
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PressLoadGame()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PressLoadGame");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PressOverwriteSave()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PressOverwriteSave");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void GoNextDay(int index)
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("GoNextDay") { { "Day", index } };
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void ShopLevelUp(int index)
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("ShopLevelUp") { { "Level", index } };
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void UnlockNewRoom(int index)
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("UnlockNewRoom") { { "roomUnlocked", index } };
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void OpenPack()
	{
	}

	public static void OpenAlbum()
	{
	}

	public static void OpenPhone()
	{
	}

	public static void PhoneRestock()
	{
	}

	public static void PhoneBuyShelf()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PhoneBuyShelf");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PhoneShopExpansion()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PhoneShopExpansion");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PhoneManageEvent()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PhoneManageEvent");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PhoneHireWorker()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PhoneHireWorker");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}

	public static void PhoneCheckPrice()
	{
		if (m_CanUseAnalytic)
		{
			CSingleton<UnityAnalytic>.Instance.StartCollection();
			CustomEvent e = new CustomEvent("PhoneCheckPrice");
			AnalyticsService.Instance.RecordEvent(e);
		}
	}
}
