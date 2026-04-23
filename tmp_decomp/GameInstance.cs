using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GameInstance : CSingleton<GameInstance>
{
	public static GameInstance m_Instance = null;

	public static bool m_HasFinishHideLoadingScreen = false;

	public static bool m_IsRestartingGameDeleteAll = false;

	public static bool m_HasLoadingError = false;

	public static bool m_IsSaveDataDirtySyncToCloud = false;

	public static bool m_SaveFileNotFound = false;

	public static bool m_LoadingDifferentAccount = false;

	public static bool m_StopCoinGemTextLerp = false;

	public static bool m_CanDragMainMenuSlider = false;

	public static bool m_FinishedSavefileLoading = false;

	public static bool m_IsVirtualKeyboardActive = false;

	public static bool m_IsItemLicenseUnlocked = false;

	public static int m_SaveDataDirtySyncToCloudCount = 0;

	public static int m_CurrentSceneIndex = 0;

	private static string m_DayTranslatedText = "d";

	private static string m_HourTranslatedText = "h";

	private static string m_MinTranslatedText = "min";

	private static string m_SecondTranslatedText = "s";

	private static int m_Day = 0;

	private static int m_Hour = 0;

	private static int m_Minute = 0;

	private static int m_Second = 0;

	private static string m_TimeString;

	public void Start()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public static float GetInvariantCultureDecimal(string text)
	{
		string text2 = text.Replace(",", ".");
		List<int> list = new List<int>();
		int num = 0;
		string text3 = text2;
		for (int i = 0; i < text3.Length; i++)
		{
			if (text3[i] == '.')
			{
				list.Add(num);
			}
			num++;
		}
		if (list.Count > 1)
		{
			for (int num2 = list.Count - 2; num2 >= 0; num2--)
			{
				text2 = text2.Remove(list[num2], 1);
			}
		}
		try
		{
			return (float)Math.Round(float.Parse(text2, CultureInfo.InvariantCulture), 2, MidpointRounding.AwayFromZero);
		}
		catch
		{
			return 0f;
		}
	}

	public static string GetPriceString(float price, bool useDashAsZero = false, bool useCurrencySymbol = true, bool useCentSymbol = false, string decimalString = "F2")
	{
		return GetPriceString((double)price, useDashAsZero, useCurrencySymbol, useCentSymbol, decimalString);
	}

	public static string GetPriceString(double price, bool useDashAsZero = false, bool useCurrencySymbol = true, bool useCentSymbol = false, string decimalString = "F2")
	{
		if (price <= 0.0 && useDashAsZero)
		{
			return "-";
		}
		if (GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price) < 1.0)
		{
			decimalString = "F2";
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Euro)
		{
			if (useCurrencySymbol)
			{
				if (useCentSymbol && price < 1.0)
				{
					return (GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price) * 100.0).ToString("F0") + "¢";
				}
				return "€" + GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
			}
			return GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			if (decimalString == "F2" || decimalString == "N2")
			{
				decimalString = "N0";
			}
			double currencyValue = GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price);
			string text = "";
			text = ((!(currencyValue >= 100000000.0)) ? ((int)currencyValue).ToString(decimalString) : currencyValue.ToString(decimalString));
			if (useCurrencySymbol)
			{
				return "¥" + text;
			}
			return text;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.GBP)
		{
			if (useCurrencySymbol)
			{
				if (useCentSymbol && price < 1.0)
				{
					return (GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price) * 100.0).ToString("F0") + "p";
				}
				return "£" + GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
			}
			return GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.CNY)
		{
			if (useCurrencySymbol)
			{
				return "¥" + GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
			}
			return GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.MYR)
		{
			if (useCurrencySymbol)
			{
				double currencyValue2 = GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price);
				if (useCentSymbol && price < 1.0 && currencyValue2 < 1.0)
				{
					return (GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price) * 100.0).ToString("F0") + " sen";
				}
				return "RM" + GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
			}
			return GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.AUD)
		{
			if (useCurrencySymbol)
			{
				double currencyValue3 = GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price);
				if (useCentSymbol && price < 1.0 && currencyValue3 < 1.0)
				{
					return (GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price) * 100.0).ToString("F0") + "¢";
				}
				return "$" + GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
			}
			return GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
		}
		if (useCurrencySymbol)
		{
			if (useCentSymbol && price < 1.0)
			{
				return (GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price) * 100.0).ToString("F0") + "¢";
			}
			return "$" + GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
		}
		return GetCurrencyValue(CSingleton<CGameManager>.Instance.m_CurrencyType, price).ToString(decimalString);
	}

	public static double GetConvertedCurrencyPrice(double price)
	{
		if (price >= 100000.0)
		{
			float num = (float)(price - (double)Mathf.FloorToInt((float)price));
			float num2 = (float)Mathf.FloorToInt((float)price) * GetCurrencyConversionRate();
			return (float)Mathf.RoundToInt(num * GetCurrencyConversionRate() * 100f) / 100f + num2;
		}
		return (float)Mathf.RoundToInt((float)price * GetCurrencyConversionRate() * 100f) / 100f;
	}

	public static int GetMaxDeckCardCount()
	{
		return 50;
	}

	public static float GetCurrencyConversionRate()
	{
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Euro)
		{
			return 1f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			return 100f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.GBP)
		{
			return 1f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.CNY)
		{
			return 2.5f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.MYR)
		{
			return 2f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.AUD)
		{
			return 2f;
		}
		return 1f;
	}

	public static float GetCurrencyRoundDivideAmount()
	{
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.CNY)
		{
			return 1000f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.MYR)
		{
			return 1000f;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.AUD)
		{
			return 1000f;
		}
		return 100f;
	}

	public static double GetCurrencyValue(EMoneyCurrencyType currencyType, double baseValue)
	{
		return Math.Round(baseValue * (double)GetCurrencyConversionRate(), 2, MidpointRounding.AwayFromZero);
	}

	public static bool GetCurrencyHasDecimal(EMoneyCurrencyType currencyType)
	{
		if (currencyType == EMoneyCurrencyType.Yen)
		{
			return false;
		}
		return true;
	}

	public static string GetTimeString(float time, bool showDay = true, bool showHour = true, bool showMinutes = true, bool showSeconds = true, bool removeZero = false, bool convertDayToHour = false)
	{
		if (time < 0f)
		{
			time = 0f;
		}
		m_Day = (int)time / 86400;
		m_Hour = (int)time / 3600;
		if (!convertDayToHour)
		{
			m_Hour = (int)time % 86400 / 3600;
		}
		m_Minute = (int)time % 3600 / 60;
		m_Second = (int)time % 60;
		if (m_Day == 0)
		{
			showDay = false;
		}
		if (m_Hour == 0)
		{
			showHour = false;
		}
		if (m_Minute == 0)
		{
			showMinutes = false;
		}
		if (m_Second == 0)
		{
			showSeconds = false;
		}
		m_TimeString = "";
		if (showDay)
		{
			m_TimeString = m_Day + m_DayTranslatedText;
			if (showMinutes || showSeconds)
			{
				showHour = true;
			}
		}
		if (showHour)
		{
			if (m_TimeString != "")
			{
				m_TimeString = m_TimeString + " " + m_Hour + m_HourTranslatedText;
			}
			else
			{
				m_TimeString = m_TimeString + m_Hour + m_HourTranslatedText;
			}
			if (showSeconds)
			{
				showMinutes = true;
			}
		}
		if (showDay && showHour && removeZero)
		{
			if (m_Minute == 0)
			{
				showMinutes = false;
				showSeconds = false;
			}
			else
			{
				showMinutes = true;
			}
		}
		if (showMinutes)
		{
			if (m_TimeString != "")
			{
				m_TimeString = m_TimeString + " " + m_Minute + m_MinTranslatedText;
			}
			else
			{
				m_TimeString = m_TimeString + m_Minute + m_MinTranslatedText;
			}
		}
		if (!showDay && !showHour && !showMinutes)
		{
			showSeconds = true;
		}
		if (!showDay && !showHour)
		{
			showSeconds = true;
		}
		if (removeZero && m_Second == 0)
		{
			showSeconds = false;
		}
		if (showSeconds)
		{
			if (m_TimeString != "")
			{
				m_TimeString = m_TimeString + " " + m_Second + m_SecondTranslatedText;
			}
			else
			{
				m_TimeString = m_TimeString + m_Second + m_SecondTranslatedText;
			}
		}
		return m_TimeString;
	}

	public static int GetDigit(float value, int digit)
	{
		return (int)(value / Mathf.Pow(10f, digit - 1)) % 10;
	}

	public static int GetDecimal(float value, int digit)
	{
		return GetDigit(Mathf.FloorToInt(value * Mathf.Pow(10f, digit)), digit);
	}

	public static string GetCardGradeString(int cardGrade)
	{
		string result = "-";
		switch (cardGrade)
		{
		case 1:
			result = "Very Poor";
			break;
		case 2:
			result = "Poor";
			break;
		case 3:
			result = "Fair";
			break;
		case 4:
			result = "Good";
			break;
		case 5:
			result = "Very Good";
			break;
		case 6:
			result = "Fine";
			break;
		case 7:
			result = "Excellent";
			break;
		case 8:
			result = "Near Mint";
			break;
		case 9:
			result = "Mint";
			break;
		case 10:
			result = "Gem Mint";
			break;
		}
		return result;
	}

	public static void SetIsSaveDataDirtySyncToCloud(bool isDirty, int priorityPoint = 1, float upgradeTime = 0f)
	{
		if (m_IsSaveDataDirtySyncToCloud != isDirty)
		{
			m_IsSaveDataDirtySyncToCloud = isDirty;
		}
		if (isDirty)
		{
			m_SaveDataDirtySyncToCloudCount += priorityPoint + Mathf.FloorToInt(upgradeTime / 1800f);
			if (m_SaveDataDirtySyncToCloudCount > 2 && CGameManager.m_ForceSyncCloudResetTimer > 6f)
			{
				CGameManager.m_ForceSyncCloudResetTimer = 0f;
				m_SaveDataDirtySyncToCloudCount = 0;
				CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
				CSingleton<CGameManager>.Instance.SaveGameData(0);
			}
		}
		else
		{
			m_SaveDataDirtySyncToCloudCount = 0;
			CGameManager.m_ForceSyncCloudResetTimer = 0f;
		}
	}
}
