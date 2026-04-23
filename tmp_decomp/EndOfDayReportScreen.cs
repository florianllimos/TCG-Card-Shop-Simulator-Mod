using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

public class EndOfDayReportScreen : CSingleton<EndOfDayReportScreen>
{
	public GameObject m_ScreenGrp;

	public GameObject m_LoadingScreenGrp;

	public GameObject m_NextDayButton;

	public List<EndDayReportTextUI> m_EndDayReportTextUIList;

	private int m_CurrentReportTextIndex;

	public TextMeshProUGUI m_CurrentDayText;

	public TextMeshProUGUI m_LoadingCurrentDayText;

	public TextMeshProUGUI m_LoadingNextDayText;

	public InputTooltipUI m_NextDayTooltipUI;

	public Color m_GreenColor;

	public Color m_RedColor;

	public Color m_WhiteColor;

	private bool m_IsActive;

	private bool m_IsLoadingNextDay;

	private bool m_IsLerpingNumber;

	private bool m_IsHoldingMouseDown;

	private float m_MouseDownTime;

	private float m_MouseHoldAutoFireRate = 0.05f;

	private float m_TotalProfit;

	public static void OpenScreen()
	{
		if (CSingleton<EndOfDayReportScreen>.Instance.m_ScreenGrp.activeSelf)
		{
			CloseScreen();
			return;
		}
		CSingleton<EndOfDayReportScreen>.Instance.m_CurrentDayText.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (CPlayerData.m_CurrentDay + 1).ToString());
		CSingleton<EndOfDayReportScreen>.Instance.SetAllLerpNumber(CPlayerData.m_GameReportDataCollect);
		CSingleton<EndOfDayReportScreen>.Instance.m_IsLerpingNumber = true;
		CSingleton<EndOfDayReportScreen>.Instance.m_CurrentReportTextIndex = 0;
		CSingleton<InteractionPlayerController>.Instance.ShowCursor();
		CSingleton<InteractionPlayerController>.Instance.EnterLockMoveMode();
		CSingleton<EndOfDayReportScreen>.Instance.m_NextDayButton.SetActive(value: false);
		CSingleton<EndOfDayReportScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		CSingleton<EndOfDayReportScreen>.Instance.m_LoadingScreenGrp.SetActive(value: false);
		CSingleton<EndOfDayReportScreen>.Instance.m_IsActive = true;
		CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
		CSingleton<CGameManager>.Instance.SaveGameData(0);
	}

	private void SetAllLerpNumber(GameReportDataCollect gameReportDataCollect)
	{
		float lerpTime = 0.3f;
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[0].SetNumber(0f, gameReportDataCollect.customerVisited, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[1].SetNumber(0f, gameReportDataCollect.checkoutCount, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[2].SetNumber(0f, gameReportDataCollect.customerDisatisfied, lerpTime, m_RedColor, m_GreenColor, m_GreenColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[3].SetNumber(0f, gameReportDataCollect.customerBoughtItem, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[4].SetNumber(0f, gameReportDataCollect.customerBoughtCard, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[5].SetNumber(0f, gameReportDataCollect.customerPlayed, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[6].SetNumber(0f, gameReportDataCollect.storeExpGained, lerpTime, m_WhiteColor, m_WhiteColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[7].SetNumber(0f, gameReportDataCollect.itemAmountSold, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[8].SetNumber(0f, gameReportDataCollect.cardAmountSold, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[9].SetNumber(0f, gameReportDataCollect.totalPlayTableTime * 60f, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[10].SetNumber(0f, gameReportDataCollect.storeLevelGained, lerpTime, m_WhiteColor, m_WhiteColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[11].SetNumber(0f, gameReportDataCollect.totalItemEarning, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[12].SetNumber(0f, gameReportDataCollect.totalCardEarning, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[13].SetNumber(0f, gameReportDataCollect.totalPlayTableEarning, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		float num = gameReportDataCollect.totalItemEarning + gameReportDataCollect.totalCardEarning + gameReportDataCollect.totalPlayTableEarning;
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[14].SetNumber(0f, num, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[15].SetNumber(0f, gameReportDataCollect.supplyCost, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[16].SetNumber(0f, gameReportDataCollect.upgradeCost, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[17].SetNumber(0f, gameReportDataCollect.employeeCost, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[18].SetNumber(0f, gameReportDataCollect.rentCost + gameReportDataCollect.billCost, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		float num2 = gameReportDataCollect.supplyCost + gameReportDataCollect.upgradeCost + gameReportDataCollect.employeeCost + gameReportDataCollect.rentCost + gameReportDataCollect.billCost;
		float num3 = num + num2;
		CSingleton<EndOfDayReportScreen>.Instance.m_EndDayReportTextUIList[19].SetNumber(0f, num3, lerpTime, m_GreenColor, m_RedColor, m_WhiteColor);
		m_TotalProfit = num3;
	}

	public static void CloseScreen()
	{
		CSingleton<InteractionPlayerController>.Instance.HideCursor();
		CSingleton<InteractionPlayerController>.Instance.ExitLockMoveMode();
		CSingleton<EndOfDayReportScreen>.Instance.m_ScreenGrp.SetActive(value: false);
		CSingleton<EndOfDayReportScreen>.Instance.m_IsActive = false;
		GameReportDataCollect item = default(GameReportDataCollect);
		item.CopyData(CPlayerData.m_GameReportDataCollect);
		CPlayerData.m_GameReportDataCollectPastList.Add(item);
		if (CPlayerData.m_GameReportDataCollectPastList.Count > 30)
		{
			CPlayerData.m_GameReportDataCollectPastList.RemoveAt(0);
		}
		CPlayerData.m_GameReportDataCollect.ResetData();
	}

	public void OnPressGoNextDay()
	{
		if (!m_IsLoadingNextDay)
		{
			m_IsLoadingNextDay = true;
			CSingleton<EndOfDayReportScreen>.Instance.m_LoadingCurrentDayText.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (CPlayerData.m_CurrentDay + 1).ToString());
			CSingleton<EndOfDayReportScreen>.Instance.m_LoadingNextDayText.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (CPlayerData.m_CurrentDay + 2).ToString());
			StartCoroutine(DelayGoNextDay());
		}
	}

	private IEnumerator DelayGoNextDay()
	{
		m_LoadingScreenGrp.SetActive(value: true);
		yield return new WaitForSeconds(0.5f);
		CloseScreen();
		LightManager.GoNextDay();
		yield return new WaitForSeconds(2.5f);
		m_LoadingScreenGrp.SetActive(value: false);
		if (CPlayerData.m_PendingGameEventFormat != EGameEventFormat.None)
		{
			CPlayerData.m_GameEventFormat = CPlayerData.m_PendingGameEventFormat;
			CPlayerData.m_PendingGameEventFormat = EGameEventFormat.None;
		}
		if (CPlayerData.m_GameEventExpansionType != CPlayerData.m_PendingGameEventExpansionType)
		{
			CPlayerData.m_GameEventExpansionType = CPlayerData.m_PendingGameEventExpansionType;
		}
		GameEventData gameEventData = InventoryBase.GetGameEventData(CPlayerData.m_GameEventFormat);
		if (gameEventData.hostEventCost > 0)
		{
			PriceChangeManager.AddTransaction(-gameEventData.hostEventCost, ETransactionType.PlayTableFee, (int)CPlayerData.m_GameEventFormat);
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(gameEventData.hostEventCost));
			CPlayerData.m_GameReportDataCollect.supplyCost -= gameEventData.hostEventCost;
			CPlayerData.m_GameReportDataCollectPermanent.supplyCost -= gameEventData.hostEventCost;
		}
		yield return new WaitForSeconds(5f);
		m_IsLoadingNextDay = false;
	}

	public void OnPressGoNextButton()
	{
		if (m_IsLerpingNumber)
		{
			if (m_CurrentReportTextIndex < m_EndDayReportTextUIList.Count)
			{
				m_EndDayReportTextUIList[m_CurrentReportTextIndex].EndLerpInstantly();
			}
		}
		else if (!m_IsLoadingNextDay && LightManager.GetHasDayEnded())
		{
			OnPressGoNextDay();
		}
	}

	private void Update()
	{
		if (!m_IsActive)
		{
			return;
		}
		if (InputManager.GetKeyDownAction(EGameAction.InteractLeft) || InputManager.GetKeyDownAction(EGameAction.GoNextDay))
		{
			m_IsHoldingMouseDown = true;
		}
		if (InputManager.GetKeyUpAction(EGameAction.InteractLeft) || InputManager.GetKeyUpAction(EGameAction.GoNextDay))
		{
			m_IsHoldingMouseDown = false;
		}
		if (m_IsHoldingMouseDown)
		{
			m_MouseDownTime += Time.deltaTime;
			if (m_MouseDownTime >= m_MouseHoldAutoFireRate)
			{
				OnPressGoNextButton();
				m_MouseDownTime = 0f;
			}
		}
		else if (m_MouseDownTime > 0f)
		{
			m_MouseDownTime = 0f;
			OnPressGoNextButton();
		}
		if (!m_IsLerpingNumber)
		{
			return;
		}
		if (m_CurrentReportTextIndex < m_EndDayReportTextUIList.Count)
		{
			if (!m_EndDayReportTextUIList[m_CurrentReportTextIndex].IsLerpEnded())
			{
				m_EndDayReportTextUIList[m_CurrentReportTextIndex].UpdateLerp();
			}
			else
			{
				m_CurrentReportTextIndex++;
			}
		}
		else
		{
			m_MouseDownTime = 0f;
			m_IsLerpingNumber = false;
			m_IsHoldingMouseDown = false;
			m_NextDayButton.SetActive(value: true);
			AchievementManager.OnDailyProfitReached(m_TotalProfit);
		}
	}

	public static bool IsActive()
	{
		return CSingleton<EndOfDayReportScreen>.Instance.m_IsActive;
	}
}
