using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GradedCardSubmitSelectScreen : UIScreenBase
{
	public CanvasGroup m_CanvasGrp;

	public GradeCardWebsiteUIScreen m_GradeCardWebsiteUIScreen;

	public List<GradeCardPanelUI> m_GradeCardPanelUIList;

	public TextMeshProUGUI m_ServiceTypeText;

	public TextMeshProUGUI m_ServiceDaysText;

	public TextMeshProUGUI m_ServiceCostPerCardText;

	public TextMeshProUGUI m_ServiceDeliveryFeeText;

	public TextMeshProUGUI m_ServiceSubtotalText;

	public TextMeshProUGUI m_ServiceTotalCostText;

	private bool m_IsShowingCanvasGrpAlpha;

	private bool m_IsHidingCanvasGrpAlpha;

	private float m_CanvasGrpAlphaLerpTimer;

	private float m_ServiceTotalCost;

	private int m_CurrentSelectedSlotIndex;

	private GradeCardServiceData m_CurrentGradeCardServiceData;

	protected override void Start()
	{
		for (int i = 0; i < m_GradeCardPanelUIList.Count; i++)
		{
			m_GradeCardPanelUIList[i].Init(this, i);
			m_GradeCardPanelUIList[i].UpdateCardUI(null);
		}
		base.Start();
	}

	protected override void RunUpdate()
	{
		base.RunUpdate();
		if (m_IsShowingCanvasGrpAlpha)
		{
			m_CanvasGrpAlphaLerpTimer += Time.deltaTime * 2f;
			m_CanvasGrp.alpha = Mathf.Lerp(0f, 1f, m_CanvasGrpAlphaLerpTimer);
			if (m_CanvasGrpAlphaLerpTimer >= 1f)
			{
				m_IsShowingCanvasGrpAlpha = false;
				m_CanvasGrpAlphaLerpTimer = 1f;
				CSingleton<InteractionPlayerController>.Instance.ExitSelectingCardFromGradedCardUIScreenResetCamera();
			}
		}
		else if (m_IsHidingCanvasGrpAlpha)
		{
			m_CanvasGrpAlphaLerpTimer -= Time.deltaTime * 2f;
			m_CanvasGrp.alpha = Mathf.Lerp(0f, 1f, m_CanvasGrpAlphaLerpTimer);
			if (m_CanvasGrpAlphaLerpTimer <= 0f)
			{
				m_IsShowingCanvasGrpAlpha = false;
				m_CanvasGrpAlphaLerpTimer = 0f;
			}
		}
	}

	protected override void OnOpenScreen()
	{
		EvaluateUITextData();
		EvaluateTotalCost();
		for (int i = 0; i < CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList.Count; i++)
		{
			m_GradeCardPanelUIList[i].UpdateCardUI(CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[i]);
		}
		base.OnOpenScreen();
	}

	protected override void OnCloseScreen()
	{
		for (int num = CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList.Count - 1; num >= 0; num--)
		{
			if (CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[num].monsterType != EMonsterType.None)
			{
				CPlayerData.AddCard(CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[num], 1);
			}
			CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[num].monsterType = EMonsterType.None;
		}
		base.OnCloseScreen();
	}

	private void EvaluateUITextData()
	{
		m_CurrentGradeCardServiceData = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.GetGradeCardServiceData(CPlayerData.m_CurrentGradeCardSubmitSet.m_ServiceLevel);
		m_ServiceTypeText.text = m_CurrentGradeCardServiceData.GetServiceName();
		m_ServiceDaysText.text = m_CurrentGradeCardServiceData.GetServiceDayString();
		m_ServiceCostPerCardText.text = GameInstance.GetPriceString(m_CurrentGradeCardServiceData.m_CostPerCard);
	}

	private void EvaluateTotalCost()
	{
		float num = (m_ServiceTotalCost = 10f);
		for (int i = 0; i < CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList.Count; i++)
		{
			if (CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[i] != null && CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[i].monsterType != EMonsterType.None)
			{
				m_ServiceTotalCost += m_CurrentGradeCardServiceData.m_CostPerCard;
			}
		}
		m_ServiceDeliveryFeeText.text = GameInstance.GetPriceString(num);
		m_ServiceSubtotalText.text = GameInstance.GetPriceString(m_ServiceTotalCost - num);
		m_ServiceTotalCostText.text = GameInstance.GetPriceString(m_ServiceTotalCost);
	}

	public void UpdateSelectedCardData(CardData cardData)
	{
		if (CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[m_CurrentSelectedSlotIndex].monsterType != EMonsterType.None)
		{
			CPlayerData.AddCard(CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[m_CurrentSelectedSlotIndex], 1);
		}
		if (cardData == null)
		{
			cardData = new CardData();
		}
		CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[m_CurrentSelectedSlotIndex] = cardData;
		m_GradeCardPanelUIList[m_CurrentSelectedSlotIndex].UpdateCardUI(cardData);
		EvaluateTotalCost();
		m_ControllerScreenUIExtension.SetControllerUIActive(isActive: true);
	}

	public void OnPressSelectCardSlotIndex(int cardSlotIndex)
	{
		if (!m_IsShowingCanvasGrpAlpha && !m_IsHidingCanvasGrpAlpha)
		{
			m_CurrentSelectedSlotIndex = cardSlotIndex;
			CSingleton<InteractionPlayerController>.Instance.StartSelectingCardFromGradedCardUIScreen();
			m_ControllerScreenUIExtension.SetControllerUIActive(isActive: false);
		}
	}

	public void OnPressServiceToggleButton()
	{
		if (!m_IsShowingCanvasGrpAlpha && !m_IsHidingCanvasGrpAlpha)
		{
			CPlayerData.m_CurrentGradeCardSubmitSet.m_ServiceLevel = (CPlayerData.m_CurrentGradeCardSubmitSet.m_ServiceLevel + 1) % 4;
			EvaluateUITextData();
			EvaluateTotalCost();
		}
	}

	public void OnPressSubmitButton()
	{
		if (m_IsShowingCanvasGrpAlpha || m_IsHidingCanvasGrpAlpha)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList.Count; i++)
		{
			if (CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[i] != null && CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList[i].monsterType != EMonsterType.None)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NoCardSelected);
			return;
		}
		if (CPlayerData.m_CoinAmountDouble < (double)m_ServiceTotalCost)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			return;
		}
		PriceChangeManager.AddTransaction(0f - m_ServiceTotalCost, ETransactionType.GradingFee, CPlayerData.m_CurrentGradeCardSubmitSet.m_ServiceLevel);
		CPlayerData.m_GameReportDataCollect.supplyCost -= m_ServiceTotalCost;
		CPlayerData.m_GameReportDataCollectPermanent.supplyCost -= m_ServiceTotalCost;
		CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(m_ServiceTotalCost));
		SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
		CPlayerData.m_CurrentGradeCardSubmitSet.m_DayPassed = 0;
		CPlayerData.m_CurrentGradeCardSubmitSet.m_MinutePassed = 0f;
		CPlayerData.m_GradeCardInProgressList.Add(CPlayerData.m_CurrentGradeCardSubmitSet);
		int serviceLevel = CPlayerData.m_CurrentGradeCardSubmitSet.m_ServiceLevel;
		CPlayerData.m_CurrentGradeCardSubmitSet = new GradeCardSubmitSet();
		CPlayerData.m_CurrentGradeCardSubmitSet.m_ServiceLevel = serviceLevel;
		CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList = new List<CardData>();
		for (int j = 0; j < 8; j++)
		{
			CPlayerData.m_CurrentGradeCardSubmitSet.m_CardDataList.Add(new CardData());
		}
		CloseScreen();
		m_GradeCardWebsiteUIScreen.UpdateSubmissionProgressPanelUI();
		CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
	}

	public void SetGameUIVisible(bool isVisible)
	{
		if (isVisible)
		{
			if (m_CanvasGrp.alpha != 1f)
			{
				m_IsShowingCanvasGrpAlpha = true;
				m_IsHidingCanvasGrpAlpha = false;
				m_CanvasGrp.interactable = true;
				m_CanvasGrp.blocksRaycasts = true;
			}
		}
		else if (m_CanvasGrp.alpha != 0f)
		{
			m_IsShowingCanvasGrpAlpha = false;
			m_IsHidingCanvasGrpAlpha = true;
			m_CanvasGrp.interactable = false;
			m_CanvasGrp.blocksRaycasts = false;
		}
	}
}
