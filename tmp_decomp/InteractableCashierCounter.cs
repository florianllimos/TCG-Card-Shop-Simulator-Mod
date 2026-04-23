using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCashierCounter : InteractableObject
{
	public Transform m_LockPlayerPos;

	public Transform m_QueueStartPos;

	public Transform m_CustomerPlaceItemPos;

	public Transform m_ScannedItemLerpPos;

	public Transform m_PlaceMoneyLocation;

	public Transform m_PlaceCoinLocation;

	public Transform m_CounterScreenFollowLoc;

	public Transform m_CreditCardScreenFollowLoc;

	public Transform m_CreditCardMachineModel;

	public Transform m_CreditCardMachineTargetPos;

	public Transform m_CreditCardPlayerLookRot;

	public List<Transform> m_TradeCardStandLocList;

	private Vector3 m_CreditCardMachineOriginalPos;

	private Quaternion m_CreditCardMachineOriginalRot;

	public GameObject m_CreditCardModel;

	public GameObject m_ScanItemPlasticBag;

	public GameObject m_NavMeshCutWhenManned;

	public GameObject m_CounterClosedMesh;

	public GameObject m_NoTradingNoticeMesh;

	public Animation m_OpenCloseDrawerAnim;

	public List<GameObject> m_QueueTransformList;

	public List<InteractableCounterMoneyChange> m_InteractableCounterMoneyChangeList;

	public bool m_IsMannedByNPC;

	private bool m_IsMannedByPlayer;

	private bool m_IsStartGivingChange;

	private bool m_IsCustomerTradingCard;

	private bool m_IsDisableCheckout;

	private bool m_IsDisableTrading;

	public int m_CustomerQueueCount;

	public List<Transform> m_ValidQueuePosList = new List<Transform>();

	private Worker m_CurrentWorker;

	public Customer m_CurrentCustomer;

	public List<Customer> m_CustomerListInQueue = new List<Customer>();

	private UI_CashCounterScreen m_UICashCounterScreen;

	private UI_CreditCardScreen m_UICreditCardScreen;

	public ECashierCounterState m_CashierCounterState;

	private int m_ChangeMoneyAddedCount;

	private int m_ChangeCoinAddedCount;

	private int m_GamepadCurrentQuickSelectIndex = -1;

	private float m_NPCTimer;

	private float m_NPCScanItemSpeed = 1.5f;

	private float m_WorkerExp;

	private double m_TotalScannedItemCost;

	private double m_CustomerPaidAmount;

	private double m_CurrentMoneyChangeValue;

	private bool m_IsChangeReady;

	private bool m_TooMuchChangeGiven;

	private bool m_IsUsingCard;

	private EMoneyCurrencyType m_CurrentCurrencyType = EMoneyCurrencyType.None;

	protected override void Awake()
	{
		base.Awake();
		ShelfManager.InitCashierCounter(this);
		SetPlsaticBagVisibility(isShow: false);
		m_CounterClosedMesh.SetActive(value: false);
		m_NoTradingNoticeMesh.SetActive(value: false);
		if (!m_UICashCounterScreen)
		{
			m_UICashCounterScreen = WorldCanvasUIManager.SpawnCashCounterScreenUI(m_CounterScreenFollowLoc);
			m_UICashCounterScreen.gameObject.SetActive(value: true);
			m_UICashCounterScreen.Init(this);
		}
		if (!m_UICreditCardScreen)
		{
			m_UICreditCardScreen = WorldCanvasUIManager.SpawnCreditCardScreenUI(m_CreditCardScreenFollowLoc);
			m_UICreditCardScreen.SetCashierCounter(this);
			m_UICreditCardScreen.gameObject.SetActive(value: true);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (m_IsMannedByNPC && (bool)m_CurrentWorker && !m_CurrentWorker.m_IsPausingAction)
		{
			if (m_CashierCounterState == ECashierCounterState.ScanningItem)
			{
				m_NPCTimer += Time.deltaTime * m_NPCScanItemSpeed;
				if (m_NPCTimer > m_CurrentWorker.m_ScanItemTime)
				{
					bool flag = false;
					if (m_CurrentCustomer.GetItemInBagList().Count > 0)
					{
						for (int i = 0; i < m_CurrentCustomer.GetItemInBagList().Count; i++)
						{
							if (m_CurrentCustomer.GetItemInBagList()[i].m_InteractableScanItem.IsNotScanned())
							{
								m_CurrentCustomer.GetItemInBagList()[i].m_InteractableScanItem.OnMouseButtonUp();
								flag = true;
								break;
							}
						}
					}
					if (!flag && m_CurrentCustomer.GetCardInBagList().Count > 0)
					{
						for (int j = 0; j < m_CurrentCustomer.GetCardInBagList().Count; j++)
						{
							if (m_CurrentCustomer.GetCardInBagList()[j].IsNotScanned())
							{
								m_CurrentCustomer.GetCardInBagList()[j].OnMouseButtonUp();
								flag = true;
								break;
							}
						}
					}
					m_NPCTimer = 0f;
					PlayWorkerActionAnim();
					m_WorkerExp += 0.2f;
				}
			}
			else if (m_CashierCounterState == ECashierCounterState.TakingCash)
			{
				m_NPCTimer += Time.deltaTime;
				if (m_NPCTimer > m_CurrentWorker.m_GiveChangeTime)
				{
					m_CurrentCustomer.m_CustomerCash.OnMouseButtonUp();
					m_NPCTimer = 0f;
					PlayWorkerActionAnim();
				}
			}
			else if (m_CashierCounterState == ECashierCounterState.GivingChange)
			{
				m_NPCTimer += Time.deltaTime;
				if (m_IsUsingCard)
				{
					if (m_NPCTimer > m_CurrentWorker.m_GiveChangeTime)
					{
						EvaluateCreditCard(m_TotalScannedItemCost);
						m_NPCTimer = 0f;
						PlayWorkerActionAnim();
						m_CurrentWorker.AddExp(EWorkerTask.ManCounter, 5 + Mathf.CeilToInt(m_WorkerExp));
						m_CurrentWorker.CounterNextCustomer(m_CustomerQueueCount);
					}
				}
				else if (m_NPCTimer > m_CurrentWorker.m_GiveChangeTime)
				{
					if (!m_IsChangeReady)
					{
						NPCEvaluateMoneyChange();
						CheckChangeReady();
						m_WorkerExp += 1f;
					}
					else
					{
						m_CurrentWorker.AddExp(EWorkerTask.ManCounter, Mathf.CeilToInt(m_WorkerExp));
						OnPressSpaceBar();
						m_CurrentWorker.CounterNextCustomer(m_CustomerQueueCount);
					}
					m_NPCTimer = 0f;
					PlayWorkerActionAnim();
				}
			}
		}
		if (m_IsMannedByPlayer)
		{
			EvaluateDPadQuickSelectControl();
		}
		if (m_IsMannedByPlayer && !CSingleton<CGameManager>.Instance.m_CashierLockMovement && (!m_IsStartGivingChange || !m_IsUsingCard))
		{
			if (InputManager.GetKeyDownAction(EGameAction.MoveForward) || InputManager.GetKeyDownAction(EGameAction.MoveForwardAlt))
			{
				OnPressEsc();
			}
			else if (InputManager.GetKeyDownAction(EGameAction.MoveBackward) || InputManager.GetKeyDownAction(EGameAction.MoveBackwardAlt))
			{
				OnPressEsc();
			}
			else if (InputManager.GetKeyDownAction(EGameAction.MoveLeft) || InputManager.GetKeyDownAction(EGameAction.MoveLeftAlt))
			{
				OnPressEsc();
			}
			else if (InputManager.GetKeyDownAction(EGameAction.MoveRight) || InputManager.GetKeyDownAction(EGameAction.MoveRightAlt))
			{
				OnPressEsc();
			}
			else if (InputManager.GetLeftAnalogDown(0, positiveValue: true) || InputManager.GetLeftAnalogDown(0, positiveValue: false))
			{
				OnPressEsc();
			}
			else if (InputManager.GetLeftAnalogDown(1, positiveValue: true) || InputManager.GetLeftAnalogDown(1, positiveValue: false))
			{
				OnPressEsc();
			}
			else if (!InputManager.GetLeftAnalogUp(0, positiveValue: true) && !InputManager.GetLeftAnalogUp(0, positiveValue: false) && !InputManager.GetLeftAnalogUp(1, positiveValue: true) && !InputManager.GetLeftAnalogUp(1, positiveValue: false) && InputManager.GetKeyDownAction(EGameAction.Crouch))
			{
				OnPressEsc();
				CSingleton<InteractionPlayerController>.Instance.SetIsCrouching(isCrouching: true);
			}
		}
	}

	private void NPCEvaluateMoneyChange()
	{
		double changeRequired = m_CustomerPaidAmount - m_TotalScannedItemCost - m_CurrentMoneyChangeValue;
		if (NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[4].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[3].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[2].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[1].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[0].m_ValueDouble))
		{
			return;
		}
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			if (NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[5].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[6].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[7].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[8].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[9].m_ValueDouble))
			{
				return;
			}
			NPCEvaluateMoneyChangeAction(m_InteractableCounterMoneyChangeList[9].m_ValueDouble, m_InteractableCounterMoneyChangeList[9].m_ValueDouble, forceGive: true);
		}
		else
		{
			if (NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[9].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[8].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[7].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[6].m_ValueDouble) || NPCEvaluateMoneyChangeAction(changeRequired, m_InteractableCounterMoneyChangeList[5].m_ValueDouble))
			{
				return;
			}
			NPCEvaluateMoneyChangeAction(m_InteractableCounterMoneyChangeList[5].m_ValueDouble, m_InteractableCounterMoneyChangeList[5].m_ValueDouble, forceGive: true);
		}
		NPCEvaluateMoneyChangeAction(0.009999999776482582, 0.009999999776482582, forceGive: true);
	}

	private bool NPCEvaluateMoneyChangeAction(double changeRequired, double amount, bool forceGive = false)
	{
		int num = Mathf.FloorToInt((float)(changeRequired / amount));
		if (forceGive)
		{
			num = 1;
		}
		if (num > 0)
		{
			for (int i = 0; i < m_InteractableCounterMoneyChangeList.Count; i++)
			{
				if (m_InteractableCounterMoneyChangeList[i].m_ValueDouble == amount)
				{
					m_InteractableCounterMoneyChangeList[i].OnMouseButtonUp();
					return true;
				}
			}
		}
		return false;
	}

	public override void OnMouseButtonUp()
	{
		CSingleton<InteractionPlayerController>.Instance.OnEnterCashCounterMode(this);
		OnRaycastEnded();
		InteractionPlayerController.SetPlayerPos(m_LockPlayerPos.position);
		m_IsMannedByPlayer = true;
		StopCurrentWorker();
		m_IsMannedByNPC = false;
		m_NavMeshCutWhenManned.SetActive(value: true);
		EvaluateTooltip();
		TutorialManager.AddTaskValue(ETutorialTaskCondition.InteractCashierCounter, 1f);
		if (m_IsUsingCard && m_IsStartGivingChange)
		{
			StartGivingChange();
		}
	}

	public override void OnRightMouseButtonUp()
	{
		CSingleton<InteractionPlayerController>.Instance.OpenCashierSettingScreen(this);
	}

	public override void OnPressEsc()
	{
		m_GamepadCurrentQuickSelectIndex = -1;
		CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
		CSingleton<InteractionPlayerController>.Instance.OnExitCashCounterMode();
		m_IsMannedByPlayer = false;
		m_NavMeshCutWhenManned.SetActive(value: false);
	}

	public override void OnPressSpaceBar()
	{
		if (m_IsChangeReady)
		{
			m_IsChangeReady = false;
			double value = m_CustomerPaidAmount - m_CurrentMoneyChangeValue;
			value = ((!(GameInstance.GetCurrencyConversionRate() > 1f)) ? ((double)(float)Math.Round(value, 2, MidpointRounding.AwayFromZero)) : ((double)(float)Math.Round(value, 3, MidpointRounding.AwayFromZero)));
			float num = (float)value;
			if ((bool)m_CurrentCustomer)
			{
				foreach (KeyValuePair<EItemType, int> item in m_UICashCounterScreen.GetItemScannedListDict())
				{
					float num2 = 0f;
					for (int i = 0; i < m_CurrentCustomer.GetItemInBagList().Count; i++)
					{
						if (m_CurrentCustomer.GetItemInBagList()[i].GetItemType() == item.Key)
						{
							num2 += m_CurrentCustomer.GetItemInBagList()[i].GetCurrentPrice();
						}
					}
					PriceChangeManager.AddTransaction(num2, ETransactionType.ItemSold, (int)item.Key, item.Value);
				}
				for (int j = 0; j < m_CurrentCustomer.GetCardInBagList().Count; j++)
				{
					PriceChangeManager.AddTransaction(m_CurrentCustomer.GetCardInBagList()[j].GetCurrentPrice(), ETransactionType.CardSold, 0, 1, m_CurrentCustomer.GetCardInBagList()[j].m_Card3dUI.m_CardUI.GetCardData());
				}
			}
			if (num > 0f)
			{
				CEventManager.QueueEvent(new CEventPlayer_AddCoin(num));
			}
			else if (num < 0f)
			{
				PriceChangeManager.AddTransaction(0f - num, ETransactionType.CashierCounterBalanceChange, 0);
				CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(Mathf.Abs(num)));
				CPlayerData.m_GameReportDataCollect.supplyCost -= num;
				CPlayerData.m_GameReportDataCollectPermanent.supplyCost -= num;
			}
			m_CurrentMoneyChangeValue = 0.0;
			m_TotalScannedItemCost = 0.0;
			m_CurrentCustomer.CounterGiveChangeCompleted(m_ChangeCoinAddedCount);
			if (m_IsUsingCard)
			{
				if (m_IsMannedByPlayer)
				{
					CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
					m_CreditCardMachineModel.transform.position = m_CreditCardMachineOriginalPos;
					m_CreditCardMachineModel.transform.rotation = m_CreditCardMachineOriginalRot;
					m_CreditCardModel.SetActive(value: false);
				}
				m_UICreditCardScreen.ResetCounter();
				m_UICashCounterScreen.ResetCounter();
			}
			else
			{
				m_OpenCloseDrawerAnim.Play("CashRegisterCloseDrawer");
				m_UICashCounterScreen.ResetCounter();
				if (m_IsMannedByPlayer)
				{
					SoundManager.PlayAudio("SFX_CheckoutRegisterOpen", 0.6f);
					m_GamepadCurrentQuickSelectIndex = -1;
					CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
				}
			}
			for (int k = 0; k < m_InteractableCounterMoneyChangeList.Count; k++)
			{
				m_InteractableCounterMoneyChangeList[k].ResetAmountGiven();
			}
			if (m_IsMannedByPlayer)
			{
				SoundManager.PlayAudio("SFX_CheckoutDone", 0.6f);
				TutorialManager.AddTaskValue(ETutorialTaskCondition.CheckoutCustomer, 1f);
				InteractionPlayerController.RestoreHiddenToolTip();
				CSingleton<InteractionPlayerController>.Instance.ShowControllerQuickSelectTooltip();
				CPlayerData.m_GameReportDataCollect.manualCheckoutCount++;
				CPlayerData.m_GameReportDataCollectPermanent.manualCheckoutCount++;
				AchievementManager.OnCustomerFinishCheckout(CPlayerData.m_GameReportDataCollectPermanent.manualCheckoutCount);
			}
			if (m_IsMannedByNPC && m_CurrentWorker.m_WorkerTask != EWorkerTask.ManCounter)
			{
				StopCurrentWorker();
			}
			m_IsStartGivingChange = false;
		}
		else if (m_IsMannedByPlayer && m_IsStartGivingChange)
		{
			if (m_IsUsingCard)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.WrongCounterCardAmount);
			}
			else if (m_TooMuchChangeGiven)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.TooMuchCounterChangeAmount);
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.WrongCounterChangeAmount);
			}
		}
	}

	public void EvaluateCreditCard(double value)
	{
		if (value == 0.0 && m_IsMannedByPlayer && m_IsStartGivingChange)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.WrongCounterCardAmount);
		}
		if (value >= m_TotalScannedItemCost - 0.009999999776482582 && value <= m_TotalScannedItemCost + 0.009999999776482582)
		{
			m_IsChangeReady = true;
			m_CustomerPaidAmount = value;
			m_CurrentMoneyChangeValue = 0.0;
		}
		else
		{
			m_IsChangeReady = false;
		}
		OnPressSpaceBar();
	}

	public override void StartMoveObject()
	{
		if (m_ValidQueuePosList.Count > 0)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotMoveCounterCustomerInQueue);
		}
		else
		{
			base.StartMoveObject();
		}
	}

	protected override void OnStartMoveObject()
	{
		base.OnStartMoveObject();
		m_UICashCounterScreen.gameObject.SetActive(value: true);
		m_UICreditCardScreen.gameObject.SetActive(value: true);
		StopCurrentWorker();
	}

	protected override void OnPlacedMovedObject()
	{
		base.OnPlacedMovedObject();
	}

	public override void BoxUpObject(bool holdBox)
	{
		base.BoxUpObject(holdBox);
		m_UICashCounterScreen.gameObject.SetActive(value: false);
		m_UICreditCardScreen.gameObject.SetActive(value: false);
	}

	private void EvaluateCheckoutTradingVisibility()
	{
		m_CounterClosedMesh.SetActive(m_IsDisableCheckout);
		m_NoTradingNoticeMesh.SetActive(m_IsDisableTrading);
	}

	public void AddScannedItemCostTotal(double value, EItemType itemType)
	{
		m_TotalScannedItemCost += value;
		double totalScannedItemCost = Math.Round(m_TotalScannedItemCost, 2, MidpointRounding.AwayFromZero);
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			totalScannedItemCost = Math.Round(m_TotalScannedItemCost, 3, MidpointRounding.AwayFromZero);
		}
		m_TotalScannedItemCost = totalScannedItemCost;
		m_UICashCounterScreen.OnItemScanned(value, itemType, m_TotalScannedItemCost);
		if (m_IsMannedByPlayer)
		{
			SoundManager.PlayAudio("SFX_CheckoutScan", 0.25f);
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
		}
	}

	public void AddScannedCardCostTotal(double value, CardData cardData)
	{
		m_TotalScannedItemCost += value;
		double num = Math.Round(m_TotalScannedItemCost, 2, MidpointRounding.AwayFromZero);
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			num = Math.Round(m_TotalScannedItemCost, 3, MidpointRounding.AwayFromZero);
		}
		m_TotalScannedItemCost = (float)num;
		m_UICashCounterScreen.OnCardScanned(value, cardData, m_TotalScannedItemCost);
		if (m_IsMannedByPlayer)
		{
			SoundManager.PlayAudio("SFX_CheckoutScan", 0.25f);
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
		}
	}

	public void SetCustomerPaidAmount(bool isUseCard, double customerPaidAmount)
	{
		m_IsUsingCard = isUseCard;
		m_CustomerPaidAmount = customerPaidAmount;
	}

	public void StartGivingChange()
	{
		m_IsStartGivingChange = true;
		if (m_IsUsingCard)
		{
			if (m_IsMannedByPlayer)
			{
				CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
				CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
				CSingleton<InteractionPlayerController>.Instance.ForceLookAt(m_CreditCardPlayerLookRot, 3f);
				m_CreditCardMachineOriginalPos = m_CreditCardMachineModel.transform.position;
				m_CreditCardMachineOriginalRot = m_CreditCardMachineModel.transform.rotation;
				m_CreditCardMachineModel.transform.position = m_CreditCardMachineTargetPos.position;
				m_CreditCardMachineModel.transform.rotation = m_CreditCardMachineTargetPos.rotation;
				m_CreditCardModel.SetActive(value: true);
			}
			m_UICreditCardScreen.EnableCreditCardMode(m_IsMannedByPlayer);
			m_UICashCounterScreen.ShowScaledUpTotalCost();
			return;
		}
		m_GamepadCurrentQuickSelectIndex = -1;
		m_ChangeMoneyAddedCount = 0;
		m_ChangeCoinAddedCount = 0;
		m_UICashCounterScreen.OnStartGivingChange();
		m_OpenCloseDrawerAnim.Play("CashRegisterOpenDrawer");
		CheckChangeReady();
		if (m_IsMannedByPlayer)
		{
			SoundManager.PlayAudio("SFX_CheckoutRegisterOpen", 0.6f);
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
		}
		if (m_CurrentCurrencyType != CSingleton<CGameManager>.Instance.m_CurrencyType)
		{
			m_CurrentCurrencyType = CSingleton<CGameManager>.Instance.m_CurrencyType;
			for (int i = 0; i < m_InteractableCounterMoneyChangeList.Count; i++)
			{
				m_InteractableCounterMoneyChangeList[i].UpdateCurrency();
			}
		}
	}

	public void OnGiveChange(double value, bool isTakingBack)
	{
		if (isTakingBack)
		{
			if (value >= 1.0)
			{
				m_ChangeMoneyAddedCount--;
			}
			else
			{
				m_ChangeCoinAddedCount--;
			}
			m_CurrentMoneyChangeValue -= value;
		}
		else
		{
			if (value >= 1.0)
			{
				m_ChangeMoneyAddedCount++;
			}
			else
			{
				m_ChangeCoinAddedCount++;
			}
			m_CurrentMoneyChangeValue += value;
		}
		CheckChangeReady();
		if (m_IsMannedByPlayer)
		{
			SoundManager.PlayAudio("SFX_WhipSoft", 0.4f);
		}
	}

	public Vector3 GetChangeMoneyPosYOffset(bool isCoin, int count)
	{
		if (isCoin)
		{
			if (m_ChangeCoinAddedCount >= 150)
			{
				return Vector3.up * 0.005f;
			}
			return Vector3.up * (0.005f * (float)Mathf.Clamp(count, 0, 150));
		}
		if (m_ChangeMoneyAddedCount >= 150)
		{
			return Vector3.up * 0.001f;
		}
		return Vector3.up * (0.001f * (float)Mathf.Clamp(count, 0, 150));
	}

	private void CheckChangeReady()
	{
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			m_TotalScannedItemCost = Math.Round(m_TotalScannedItemCost, 3, MidpointRounding.AwayFromZero);
		}
		else
		{
			m_TotalScannedItemCost = Math.Round(m_TotalScannedItemCost, 2, MidpointRounding.AwayFromZero);
		}
		float num = 0.005f;
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			num = 0.0099f;
		}
		double value = m_CustomerPaidAmount - m_TotalScannedItemCost - (double)num;
		value = ((!(GameInstance.GetCurrencyConversionRate() > 1f)) ? Math.Round(value, 2, MidpointRounding.AwayFromZero) : Math.Round(value, 3, MidpointRounding.AwayFromZero));
		if (m_CurrentMoneyChangeValue >= value - (double)num && m_TotalScannedItemCost > 0.0)
		{
			if (m_CustomerPaidAmount - m_TotalScannedItemCost - m_CurrentMoneyChangeValue - (double)num < -2.0)
			{
				m_IsChangeReady = false;
				m_TooMuchChangeGiven = true;
			}
			else
			{
				m_IsChangeReady = true;
				m_TooMuchChangeGiven = false;
			}
		}
		else
		{
			m_IsChangeReady = false;
			m_TooMuchChangeGiven = false;
		}
		m_UICashCounterScreen.UpdateMoneyChangeAmount(m_IsChangeReady, m_CustomerPaidAmount, m_TotalScannedItemCost, m_CurrentMoneyChangeValue);
	}

	public Transform GetQueuePosition()
	{
		if (m_CustomerQueueCount == 0)
		{
			for (int i = 0; i < m_QueueTransformList.Count; i++)
			{
				if (!m_QueueTransformList[i].gameObject.activeSelf && !m_ValidQueuePosList.Contains(m_QueueTransformList[i].transform))
				{
					m_QueueTransformList[i].transform.position = m_QueueStartPos.position;
					m_QueueTransformList[i].transform.rotation = m_QueueStartPos.rotation;
					m_QueueTransformList[i].gameObject.SetActive(value: true);
					m_ValidQueuePosList.Add(m_QueueTransformList[i].transform);
					m_CustomerQueueCount++;
					return m_QueueTransformList[i].transform;
				}
			}
		}
		else
		{
			Transform transform = m_ValidQueuePosList[m_ValidQueuePosList.Count - 1];
			int mask = LayerMask.GetMask("ShopModel", "Glass", "Customer", "QueueBlocker");
			Vector3 vector = transform.position - transform.forward;
			Collider[] array = Physics.OverlapBox(vector + Vector3.up, Vector3.one * 0.25f, transform.rotation, mask);
			Collider[] array2 = Physics.OverlapBox(transform.position - transform.forward * 0.5f + Vector3.up, Vector3.one * 0.25f, transform.rotation, mask);
			Vector3 vector2 = transform.position + transform.right;
			Collider[] array3 = Physics.OverlapBox(vector2 + Vector3.up, Vector3.one * 0.25f, transform.rotation, mask);
			Collider[] array4 = Physics.OverlapBox(transform.position + transform.right * 0.5f + Vector3.up, Vector3.one * 0.25f, transform.rotation, mask);
			Vector3 vector3 = transform.position - transform.right;
			Collider[] array5 = Physics.OverlapBox(vector3 + Vector3.up, Vector3.one * 0.25f, transform.rotation, mask);
			Collider[] array6 = Physics.OverlapBox(transform.position - transform.right * 0.5f + Vector3.up, Vector3.one * 0.25f, transform.rotation, mask);
			List<int> list = new List<int>();
			if (array.Length == 0 && array2.Length == 0)
			{
				list.Add(0);
			}
			if (array3.Length == 0 && array4.Length == 0)
			{
				list.Add(1);
			}
			if (array5.Length == 0 && array6.Length == 0)
			{
				list.Add(2);
			}
			if (list.Count == 0)
			{
				return null;
			}
			int num = list[UnityEngine.Random.Range(0, list.Count)];
			for (int j = 0; j < m_QueueTransformList.Count; j++)
			{
				if (!m_QueueTransformList[j].gameObject.activeSelf && !m_ValidQueuePosList.Contains(m_QueueTransformList[j].transform))
				{
					switch (num)
					{
					case 0:
						m_QueueTransformList[j].transform.position = vector;
						break;
					case 1:
						m_QueueTransformList[j].transform.position = vector2;
						break;
					case 2:
						m_QueueTransformList[j].transform.position = vector3;
						break;
					}
					m_QueueTransformList[j].transform.LookAt(transform.transform, Vector3.up);
					m_QueueTransformList[j].gameObject.SetActive(value: true);
					m_ValidQueuePosList.Add(m_QueueTransformList[j].transform);
					m_CustomerQueueCount++;
					return m_QueueTransformList[j].transform;
				}
			}
		}
		return null;
	}

	public void RemoveCurrentCustomerFromQueue()
	{
		if (m_CustomerQueueCount <= 0)
		{
			return;
		}
		if (m_ValidQueuePosList.Count > 1)
		{
			int num = m_ValidQueuePosList.Count - 1;
			while (num >= 0 && num != 0)
			{
				m_ValidQueuePosList[num].transform.position = m_ValidQueuePosList[num - 1].transform.position;
				m_ValidQueuePosList[num].transform.rotation = m_ValidQueuePosList[num - 1].transform.rotation;
				num--;
			}
		}
		m_ValidQueuePosList[0].gameObject.SetActive(value: false);
		m_ValidQueuePosList.RemoveAt(0);
		m_CustomerQueueCount--;
		for (int i = 0; i < m_CustomerListInQueue.Count; i++)
		{
			m_CustomerListInQueue[i].OnCashierCounterQueueMoved(i);
		}
	}

	public void RemoveLastCustomerFromQueue()
	{
		if (m_CustomerQueueCount > 0)
		{
			m_ValidQueuePosList[m_CustomerQueueCount - 1].gameObject.SetActive(value: false);
			m_ValidQueuePosList.RemoveAt(m_CustomerQueueCount - 1);
			m_CustomerQueueCount--;
		}
	}

	private void EvaluateDPadQuickSelectControl()
	{
		if (!CSingleton<InputManager>.Instance.m_IsControllerActive || InputManager.IsMovingLeftThumbstick() || InputManager.IsMovingRightThumbstick() || InputManager.GetKeyDownAction(EGameAction.Jump))
		{
			if (m_CashierCounterState != ECashierCounterState.GivingChange || !m_IsUsingCard)
			{
				m_GamepadCurrentQuickSelectIndex = -1;
				CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
			}
		}
		else
		{
			if (!InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Up) && !InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Down) && !InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_L) && !InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_R))
			{
				return;
			}
			if (m_CashierCounterState == ECashierCounterState.TakingCash)
			{
				m_GamepadCurrentQuickSelectIndex = -1;
				CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
				CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_CurrentCustomer.m_CustomerCash.transform, 8f);
				return;
			}
			if (m_CashierCounterState == ECashierCounterState.GivingChange && !m_IsUsingCard)
			{
				if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Up) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Down))
				{
					if (m_GamepadCurrentQuickSelectIndex == -1)
					{
						m_GamepadCurrentQuickSelectIndex = 0;
					}
					else if (m_GamepadCurrentQuickSelectIndex >= m_InteractableCounterMoneyChangeList.Count / 2)
					{
						m_GamepadCurrentQuickSelectIndex -= m_InteractableCounterMoneyChangeList.Count / 2;
					}
					else
					{
						m_GamepadCurrentQuickSelectIndex += m_InteractableCounterMoneyChangeList.Count / 2;
					}
					CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
					CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_InteractableCounterMoneyChangeList[m_GamepadCurrentQuickSelectIndex].transform, 8f);
				}
				else if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_R))
				{
					CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
					m_GamepadCurrentQuickSelectIndex++;
					if (m_GamepadCurrentQuickSelectIndex >= m_InteractableCounterMoneyChangeList.Count)
					{
						m_GamepadCurrentQuickSelectIndex = 0;
					}
					CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_InteractableCounterMoneyChangeList[m_GamepadCurrentQuickSelectIndex].transform, 8f);
				}
				else if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_L))
				{
					CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
					if (m_GamepadCurrentQuickSelectIndex == -1)
					{
						m_GamepadCurrentQuickSelectIndex = 0;
					}
					else
					{
						m_GamepadCurrentQuickSelectIndex--;
					}
					if (m_GamepadCurrentQuickSelectIndex < 0)
					{
						m_GamepadCurrentQuickSelectIndex = m_InteractableCounterMoneyChangeList.Count - 1;
					}
					CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_InteractableCounterMoneyChangeList[m_GamepadCurrentQuickSelectIndex].transform, 8f);
				}
				return;
			}
			List<Transform> list = new List<Transform>();
			if ((bool)m_CurrentCustomer)
			{
				for (int i = 0; i < m_CurrentCustomer.GetItemInBagList().Count; i++)
				{
					if (m_CurrentCustomer.GetItemInBagList()[i].m_InteractableScanItem.IsNotScanned())
					{
						list.Add(m_CurrentCustomer.GetItemInBagList()[i].transform);
					}
				}
				for (int j = 0; j < m_CurrentCustomer.GetCardInBagList().Count; j++)
				{
					if (m_CurrentCustomer.GetCardInBagList()[j].IsNotScanned())
					{
						list.Add(m_CurrentCustomer.GetCardInBagList()[j].transform);
					}
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Down) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_R))
			{
				m_GamepadCurrentQuickSelectIndex++;
				if (m_GamepadCurrentQuickSelectIndex >= list.Count)
				{
					m_GamepadCurrentQuickSelectIndex = 0;
				}
				if (list.Count <= m_GamepadCurrentQuickSelectIndex)
				{
					return;
				}
				if (!list[m_GamepadCurrentQuickSelectIndex].gameObject.activeSelf)
				{
					m_GamepadCurrentQuickSelectIndex++;
					if (m_GamepadCurrentQuickSelectIndex >= list.Count)
					{
						m_GamepadCurrentQuickSelectIndex = 0;
					}
				}
				CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
				CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(list[m_GamepadCurrentQuickSelectIndex], 8f);
			}
			else
			{
				if (!InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Up) && !InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_L))
				{
					return;
				}
				if (m_GamepadCurrentQuickSelectIndex == -1)
				{
					m_GamepadCurrentQuickSelectIndex = 0;
				}
				else
				{
					m_GamepadCurrentQuickSelectIndex--;
				}
				if (m_GamepadCurrentQuickSelectIndex < 0)
				{
					m_GamepadCurrentQuickSelectIndex = list.Count - 1;
				}
				if (list.Count <= m_GamepadCurrentQuickSelectIndex)
				{
					return;
				}
				if (!list[m_GamepadCurrentQuickSelectIndex].gameObject.activeSelf)
				{
					m_GamepadCurrentQuickSelectIndex--;
					if (m_GamepadCurrentQuickSelectIndex < 0)
					{
						m_GamepadCurrentQuickSelectIndex = list.Count - 1;
					}
				}
				CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
				CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(list[m_GamepadCurrentQuickSelectIndex], 8f);
			}
		}
	}

	public Worker GetCurrentWorker()
	{
		return m_CurrentWorker;
	}

	public void SetCurrentWorker(Worker worker)
	{
		m_CurrentWorker = worker;
		if (worker == null)
		{
			m_IsMannedByNPC = false;
			m_NavMeshCutWhenManned.SetActive(value: false);
		}
	}

	public void NPCStartManCounter(Worker worker)
	{
		m_IsMannedByNPC = true;
		m_NavMeshCutWhenManned.SetActive(value: true);
	}

	public void PlayWorkerActionAnim()
	{
		if ((bool)m_CurrentWorker)
		{
			m_CurrentWorker.PlayWorkerActionAnim();
		}
	}

	public void StopCurrentWorker()
	{
		if ((bool)m_CurrentWorker)
		{
			m_IsMannedByNPC = false;
			m_CurrentWorker.StopManningCounter();
			m_CurrentWorker = null;
			m_NavMeshCutWhenManned.SetActive(value: false);
		}
	}

	public void AddCustomerToQueue(Customer customer)
	{
		m_CustomerListInQueue.Add(customer);
	}

	public void RemoveCustomerFromQueue(Customer customer)
	{
		m_CustomerListInQueue.Remove(customer);
	}

	public void UpdateCurrentCustomer(Customer customer)
	{
		m_CurrentCustomer = customer;
		m_TotalScannedItemCost = 0.0;
		m_CurrentMoneyChangeValue = 0.0;
		m_WorkerExp = 0f;
	}

	public void UpdateCashierCounterState(ECashierCounterState state)
	{
		m_CashierCounterState = state;
		EvaluateTooltip();
	}

	private void EvaluateTooltip()
	{
		if (!m_IsMannedByPlayer)
		{
			return;
		}
		if (m_CashierCounterState == ECashierCounterState.TakingCash)
		{
			InteractionPlayerController.TempHideToolTip();
			InteractionPlayerController.AddToolTip(EGameAction.InteractLeft);
			CSingleton<InteractionPlayerController>.Instance.ShowControllerQuickSelectTooltip();
		}
		else if (m_CashierCounterState == ECashierCounterState.GivingChange)
		{
			InteractionPlayerController.TempHideToolTip();
			if (m_IsUsingCard)
			{
				InteractionPlayerController.AddToolTip(EGameAction.InteractLeft);
				InteractionPlayerController.AddToolTip(EGameAction.DoneCounter);
				return;
			}
			InteractionPlayerController.AddToolTip(EGameAction.AddChange, isHold: true);
			InteractionPlayerController.AddToolTip(EGameAction.RemoveChange, isHold: true);
			InteractionPlayerController.AddToolTip(EGameAction.DoneCounter);
			CSingleton<InteractionPlayerController>.Instance.ShowControllerQuickSelectTooltip();
		}
		else
		{
			InteractionPlayerController.RestoreHiddenToolTip();
			CSingleton<InteractionPlayerController>.Instance.ShowControllerQuickSelectTooltip();
		}
	}

	public bool IsCustomerAtPayingPosition(Vector3 customerTargetPos)
	{
		if (customerTargetPos == m_QueueStartPos.position)
		{
			return true;
		}
		return false;
	}

	public bool IsCustomerNextInLine(Customer customer)
	{
		if (m_CustomerListInQueue.Count > 0 && m_CustomerListInQueue[0] == customer)
		{
			return true;
		}
		return false;
	}

	public bool IsCustomerLastInLine(Customer customer)
	{
		if (m_CustomerListInQueue.Count > 0 && m_CustomerListInQueue[m_CustomerListInQueue.Count - 1] == customer)
		{
			return true;
		}
		return false;
	}

	public int GetCurrentQueingCustomerCount()
	{
		return m_ValidQueuePosList.Count;
	}

	public bool IsMannedByNPC()
	{
		return m_IsMannedByNPC;
	}

	public bool IsMannedByPlayer()
	{
		return m_IsMannedByPlayer;
	}

	public bool HasTradeCardSpace()
	{
		return !m_IsCustomerTradingCard;
	}

	public bool CanTradeCard()
	{
		return !m_IsDisableTrading;
	}

	public bool CanCheckout()
	{
		return !m_IsDisableCheckout;
	}

	public void SetCanTradeCard(bool canTrade)
	{
		m_IsDisableTrading = !canTrade;
		EvaluateCheckoutTradingVisibility();
	}

	public void SetCanCheckout(bool canCheckout)
	{
		m_IsDisableCheckout = !canCheckout;
		EvaluateCheckoutTradingVisibility();
	}

	public void OnCashierSettingDone()
	{
		if (m_IsDisableCheckout)
		{
			StopCurrentWorker();
		}
	}

	public Transform GetTradeCardStandLoc()
	{
		if (m_IsCustomerTradingCard)
		{
			return null;
		}
		m_IsCustomerTradingCard = true;
		return m_TradeCardStandLocList[UnityEngine.Random.Range(0, m_TradeCardStandLocList.Count)];
	}

	public void CustomerFinishTradingCard()
	{
		m_IsCustomerTradingCard = false;
	}

	public void SetPlsaticBagVisibility(bool isShow)
	{
		m_ScanItemPlasticBag.SetActive(isShow);
	}

	public override void OnDestroyed()
	{
		ShelfManager.RemoveCashierCounter(this);
		UnityEngine.Object.Destroy(m_UICashCounterScreen.gameObject);
		UnityEngine.Object.Destroy(m_UICreditCardScreen.gameObject);
		m_UICashCounterScreen = null;
		m_UICreditCardScreen = null;
		base.OnDestroyed();
	}

	public bool IsGivingChange()
	{
		return m_CashierCounterState == ECashierCounterState.GivingChange;
	}

	private void ForceResetCounter()
	{
		for (int i = 0; i < m_QueueTransformList.Count; i++)
		{
			m_QueueTransformList[i].SetActive(value: false);
		}
		if (m_IsStartGivingChange)
		{
			m_OpenCloseDrawerAnim.Play("CashRegisterCloseDrawer");
			for (int j = 0; j < m_InteractableCounterMoneyChangeList.Count; j++)
			{
				m_InteractableCounterMoneyChangeList[j].ResetAmountGiven();
			}
		}
		m_CustomerListInQueue.Clear();
		UpdateCashierCounterState(ECashierCounterState.Idle);
		m_UICashCounterScreen.ResetCounter();
		UpdateCurrentCustomer(null);
		m_CurrentWorker = null;
		SetPlsaticBagVisibility(isShow: false);
		m_NPCTimer = 0f;
		m_TotalScannedItemCost = 0.0;
		m_CurrentMoneyChangeValue = 0.0;
		m_CustomerPaidAmount = 0.0;
		m_IsChangeReady = false;
		m_IsUsingCard = false;
		m_IsMannedByPlayer = false;
		m_IsMannedByNPC = false;
		m_IsStartGivingChange = false;
		m_ValidQueuePosList.Clear();
		m_CustomerQueueCount = 0;
		m_NavMeshCutWhenManned.SetActive(value: false);
	}

	public int GetCustomerInQueueCount()
	{
		return m_CustomerQueueCount;
	}

	public void LoadData(CashierCounterSaveData saveData)
	{
		m_IsDisableCheckout = saveData.isDisableCheckout;
		m_IsDisableTrading = saveData.isDisableTrading;
		EvaluateCheckoutTradingVisibility();
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.AddListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected new void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.RemoveListener<CEventPlayer_OnMoneyCurrencyUpdated>(OnMoneyCurrencyUpdated);
		}
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		ForceResetCounter();
	}

	protected void OnMoneyCurrencyUpdated(CEventPlayer_OnMoneyCurrencyUpdated evt)
	{
		if (!m_IsUsingCard)
		{
			m_ChangeMoneyAddedCount = 0;
			m_CurrentMoneyChangeValue = 0.0;
			CheckChangeReady();
		}
	}
}
