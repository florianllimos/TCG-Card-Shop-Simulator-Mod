using System;
using System.Collections;
using System.Collections.Generic;
using CC;
using I2.Loc;
using Pathfinding;
using UnityEngine;

public class Customer : MonoBehaviour
{
	public CharacterCustomization m_CharacterCustom;

	public Animator m_Anim;

	public float m_CurrentMoveSpeed;

	private Vector3 m_LastFramePos;

	public bool m_IsFemale;

	public bool m_IsActive;

	public Seeker m_Seeker;

	private StartEndModifier m_DefaultStartEndModifier;

	public StartEndModifier m_OriginalStartEndModifier;

	private Path m_Path;

	public float m_Speed = 1f;

	private float m_ModifiedSpeed = 1f;

	public float m_NextWaypointDistance = 0.1f;

	public float m_RepathRate = 2f;

	public float m_RotationLerpSpeed = 2f;

	public GameObject m_GameCardFanOut;

	public GameObject m_GameCardSingle;

	public GameObject m_SmellyFX;

	public GameObject m_CleanFX;

	public GameObject m_CustomerMeshGrp;

	public GameObject m_ExclaimationMesh;

	public GameObject m_InteractCollider;

	public GameObject m_DebugCube;

	public Transform m_PlayerLookAtTarget;

	public Transform m_ShoppingBagTransform;

	public InteractableCustomerCash m_CustomerCash;

	public ParentTo m_ShoppingBagParentTo;

	public ParentTo m_CustomerCashParentTo;

	public List<Item> m_ItemInBagList = new List<Item>();

	public List<InteractableCard3d> m_CardInBagList = new List<InteractableCard3d>();

	public List<CustomerReviewGatherData> m_CustomerReviewGatherDataList = new List<CustomerReviewGatherData>();

	public ECustomerState m_CurrentState;

	private Shelf m_CurrentShelf;

	private ShelfCompartment m_CurrentItemCompartment;

	private CardShelf m_CurrentCardShelf;

	private InteractablePlayTable m_CurrentPlayTable;

	private int m_CurrentPlayTableSeatIndex = -1;

	private InteractableCardCompartment m_CurrentCardCompartment;

	private InteractableCashierCounter m_CurrentQueueCashierCounter;

	private InteractableCashierCounter m_CurrentTradeCardCashierCounter;

	private InteractableBulkDonationBox m_CurrentBulkDonationBox;

	private bool m_ReachedEndOfPath;

	private bool m_HasPlayedGame;

	private bool m_HasTradedCard;

	private bool m_IsSmelly;

	private bool m_IsAngry;

	private bool m_HasCheckedOut;

	private bool m_IsShopBlocked;

	private bool m_IsBeingSprayed;

	private bool m_IsCheckScanItemOutOfBound;

	private float m_CheckScanItemOutOfBoundTimer;

	private float m_BeingSprayedResetTimer;

	private float m_BeingSprayedResetTimeMax;

	private int m_SmellyMeter;

	private int m_CurrentWaypoint;

	private int m_FailFindShelfAttemptCount;

	private int m_FailFindItemAttemptCount;

	private int m_ItemScannedCount;

	private int m_PlayerOpenPackNearbyCount;

	private float m_LastRepath = float.NegativeInfinity;

	private float m_ExtraSpeedMultiplier = 1f;

	private float m_CounterQueueDistanceReducer;

	private float m_CurrentPlayTableFee;

	public Transform m_TargetTransform;

	private Quaternion m_RotationBeforeInteract;

	private Quaternion m_TargetLerpRotation;

	private Vector3 m_LerpStartPos;

	private Vector3 m_TargetLerpPos;

	private Coroutine m_DecideFinishShopping;

	private CustomerTradeData m_CustomerTradeData;

	private CardData m_SoldCardData;

	private bool m_IsInsideShop;

	private bool m_HasTookItemFromShelf;

	private bool m_HasTookCardFromShelf;

	private bool m_HasTookCardFromBulkDonationBox;

	private bool m_IsAtPayingPosition;

	private bool m_IsWaitingForPathCallback;

	private bool m_UnableToFindQueue;

	private bool m_IsChattyCustomer;

	private bool m_HasUpdatedCustomerCount;

	private bool m_IsPausingAction;

	private bool m_IsExclaimationVisibleState;

	private bool m_LookingForCardOnly;

	private bool m_IsWalkingToPlayTableGame;

	private float m_Timer;

	private float m_TimerMax;

	private float m_SecondaryTimer;

	private float m_SecondaryTimerMax;

	private float m_TotalScannedItemCost;

	public float m_CurrentCostTotal;

	public float m_MaxMoney;

	public List<EItemType> m_TargetBuyItemList = new List<EItemType>();

	private void Start()
	{
		m_DefaultStartEndModifier = m_Seeker.startEndModifier;
		m_ExclaimationMesh.SetActive(value: false);
		m_InteractCollider.SetActive(value: false);
	}

	public void OnMousePress()
	{
		m_IsPausingAction = true;
		CSingleton<InteractionPlayerController>.Instance.EnterWorkerInteractMode();
		m_RotationBeforeInteract = m_TargetLerpRotation;
		Vector3 forward = CSingleton<InteractionPlayerController>.Instance.m_Cam.transform.position - base.transform.position;
		forward.y = 0f;
		m_TargetLerpRotation = Quaternion.LookRotation(forward, Vector3.up);
		_ = base.transform.position + forward.normalized * 1.5f;
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(m_PlayerLookAtTarget, 3f, 0.15f);
		CSingleton<InteractionPlayerController>.Instance.EnterLockMoveMode();
		GameUIScreen.HideToolTip();
		GameUIScreen.HideEnterGoNextDayIndicatorVisible();
		TutorialManager.SetGameUIVisible(isVisible: false);
		CSingleton<CustomerManager>.Instance.m_CustomerTradeCardScreen.SetCustomer(this, m_CustomerTradeData);
		CSingleton<CustomerManager>.Instance.m_CustomerTradeCardScreen.OpenScreen();
		m_IsExclaimationVisibleState = m_ExclaimationMesh.activeSelf;
		m_ExclaimationMesh.SetActive(value: false);
	}

	public void OnPressStopInteract()
	{
		m_CustomerTradeData = null;
		m_Timer = 0f;
		m_TargetLerpRotation = m_RotationBeforeInteract;
		m_IsPausingAction = false;
		CSingleton<InteractionPlayerController>.Instance.ExitWorkerInteractMode();
		CSingleton<InteractionPlayerController>.Instance.StopAimLookAt();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		GameUIScreen.ResetToolTipVisibility();
		GameUIScreen.ResetEnterGoNextDayIndicatorVisible();
		TutorialManager.SetGameUIVisible(isVisible: true);
		m_ExclaimationMesh.SetActive(value: false);
		m_InteractCollider.SetActive(value: false);
		if ((bool)m_CurrentTradeCardCashierCounter)
		{
			m_HasTradedCard = true;
			m_CurrentTradeCardCashierCounter.CustomerFinishTradingCard();
			m_CurrentTradeCardCashierCounter = null;
		}
		DetermineShopAction();
	}

	public void OnPressRefreshInteract(CustomerTradeData customerTradeData)
	{
		m_CustomerTradeData = customerTradeData;
		m_Timer = 0f;
		m_TargetLerpRotation = m_RotationBeforeInteract;
		m_IsPausingAction = false;
		CSingleton<InteractionPlayerController>.Instance.ExitWorkerInteractMode();
		CSingleton<InteractionPlayerController>.Instance.StopAimLookAt();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		GameUIScreen.ResetToolTipVisibility();
		GameUIScreen.ResetEnterGoNextDayIndicatorVisible();
		TutorialManager.SetGameUIVisible(isVisible: true);
		m_ExclaimationMesh.SetActive(value: true);
		m_InteractCollider.SetActive(value: true);
	}

	public void SetSoldCard(CardData cardData)
	{
		m_SoldCardData = cardData;
	}

	public void RandomizeCharacterMesh()
	{
		if (!m_CharacterCustom.m_HasInit)
		{
			if (m_IsFemale)
			{
				m_CharacterCustom.CharacterName = "Female" + CSingleton<CustomerManager>.Instance.GetCustomerModelIndex(isMale: false);
			}
			else
			{
				m_CharacterCustom.CharacterName = "Male" + CSingleton<CustomerManager>.Instance.GetCustomerModelIndex(isMale: true);
			}
			m_CharacterCustom.Initialize();
		}
	}

	public void ActivateCustomer(bool canSpawnSmelly = true)
	{
		m_IsActive = true;
		RandomizeCharacterMesh();
		m_Timer = 0f;
		m_ModifiedSpeed = m_Speed + UnityEngine.Random.Range(0f, 0.25f);
		m_FailFindShelfAttemptCount = 0;
		m_FailFindItemAttemptCount = 0;
		m_ItemScannedCount = 0;
		m_TotalScannedItemCost = 0f;
		m_PlayerOpenPackNearbyCount = 0;
		m_HasTookItemFromShelf = false;
		m_IsAtPayingPosition = false;
		m_IsWaitingForPathCallback = false;
		m_UnableToFindQueue = false;
		m_IsWalkingToPlayTableGame = false;
		m_IsChattyCustomer = UnityEngine.Random.Range(0, 100) < 10;
		m_CustomerCash.Init(this);
		m_TargetTransform = CustomerManager.GetRandomExitPoint();
		base.transform.position = m_TargetTransform.position;
		base.transform.rotation = m_TargetTransform.rotation;
		m_TargetLerpRotation = m_TargetTransform.rotation;
		m_MaxMoney = CSingleton<CustomerManager>.Instance.GetCustomerMaxMoney();
		m_CurrentCostTotal = 0f;
		SetState(ECustomerState.Idle);
		m_Timer = 10f;
		m_ShoppingBagTransform.gameObject.SetActive(value: false);
		m_Anim.SetBool("HoldingBag", value: false);
		m_Anim.SetBool("HandingOverCash", value: false);
		m_CustomerCash.gameObject.SetActive(value: false);
		m_GameCardFanOut.SetActive(value: false);
		m_GameCardSingle.SetActive(value: false);
		m_CleanFX.SetActive(value: false);
		m_ExclaimationMesh.SetActive(value: false);
		m_InteractCollider.SetActive(value: false);
		m_IsSmelly = false;
		m_SmellyFX.SetActive(value: false);
		if (canSpawnSmelly)
		{
			int num = UnityEngine.Random.Range(0, 100);
			if (LightManager.IsEvening())
			{
				num -= 3;
			}
			else if (LightManager.IsNight())
			{
				num -= 5;
			}
			if (num < 2 + Mathf.Clamp(CPlayerData.m_ShopLevel / 2, 0, 15) && CSingleton<CustomerManager>.Instance.GetSmellyCustomerList().Count < CPlayerData.m_ShopLevel / 5 + 1)
			{
				SetSmelly();
			}
		}
		m_LookingForCardOnly = false;
		if (UnityEngine.Random.Range(0, 100) < 5 + Mathf.Clamp(CPlayerData.m_ShopLevel / 10, 0, 15))
		{
			m_LookingForCardOnly = true;
		}
		m_TargetBuyItemList.Clear();
		int num2 = UnityEngine.Random.Range(0, 100);
		int num3 = 60;
		if (CPlayerData.m_ShopLevel < 3)
		{
			num3 = 5;
		}
		else if (CPlayerData.m_ShopLevel < 5)
		{
			num3 = 10;
		}
		else if (CPlayerData.m_ShopLevel < 10)
		{
			num3 = 15;
		}
		else if (CPlayerData.m_ShopLevel < 20)
		{
			num3 = 25;
		}
		else if (CPlayerData.m_ShopLevel < 30)
		{
			num3 = 30;
		}
		else if (CPlayerData.m_ShopLevel < 40)
		{
			num3 = 40;
		}
		else if (CPlayerData.m_ShopLevel < 50)
		{
			num3 = 45;
		}
		else if (CPlayerData.m_ShopLevel < 60)
		{
			num3 = 50;
		}
		else if (CPlayerData.m_ShopLevel < 70)
		{
			num3 = 55;
		}
		if (num2 < num3)
		{
			UnityEngine.Random.Range(0, 100);
			List<EItemType> targetBuyItemList = CPlayerData.m_TargetBuyItemList;
			Mathf.Clamp(UnityEngine.Random.Range(-targetBuyItemList.Count / 2, targetBuyItemList.Count), 1, 6);
			int num4 = 1 + UnityEngine.Random.Range(0, 4);
			for (int i = 0; i < targetBuyItemList.Count; i++)
			{
				m_TargetBuyItemList.Add(targetBuyItemList[UnityEngine.Random.Range(0, targetBuyItemList.Count)]);
				if (m_TargetBuyItemList.Count >= num4)
				{
					break;
				}
			}
		}
		m_CustomerReviewGatherDataList.Clear();
	}

	public void SetSmelly()
	{
		m_IsSmelly = true;
		m_SmellyMeter = 10;
		CSingleton<CustomerManager>.Instance.AddToSmellyCustomerList(this);
		m_SmellyFX.SetActive(value: true);
	}

	public void DeactivateCustomer()
	{
		for (int num = m_ItemInBagList.Count - 1; num >= 0; num--)
		{
			CPlayerData.AddCurrentTotalItemCount(m_ItemInBagList[num].GetItemType(), -1);
			m_ItemInBagList[num].DisableItem();
			m_ItemInBagList.RemoveAt(num);
		}
		for (int num2 = m_CardInBagList.Count - 1; num2 >= 0; num2--)
		{
			m_CardInBagList[num2].OnDestroyed();
			m_CardInBagList.RemoveAt(num2);
		}
		if (!m_HasUpdatedCustomerCount)
		{
			m_HasUpdatedCustomerCount = true;
			CSingleton<CustomerManager>.Instance.UpdateCustomerCount(-1);
		}
		SetOutOfScreen();
		base.gameObject.SetActive(value: false);
		m_IsActive = false;
		m_IsInsideShop = false;
		m_HasPlayedGame = false;
		m_HasCheckedOut = false;
		m_HasTradedCard = false;
		m_HasUpdatedCustomerCount = false;
		m_IsAngry = false;
		if (m_IsSmelly)
		{
			m_IsSmelly = false;
			CSingleton<CustomerManager>.Instance.RemoveFromSmellyCustomerList(this);
		}
		m_CustomerReviewGatherDataList.Clear();
		m_SoldCardData = null;
	}

	public void DeodorantSprayCheck(Vector3 sprayPos, float range, int potency)
	{
		sprayPos.y = base.transform.position.y;
		if (!((base.transform.position - sprayPos).magnitude < range))
		{
			return;
		}
		m_IsBeingSprayed = true;
		m_BeingSprayedResetTimer = 0f;
		m_BeingSprayedResetTimeMax = UnityEngine.Random.Range(1.5f, 3.5f);
		m_Anim.SetBool("IsBeingSprayed", value: true);
		if (m_IsSmelly)
		{
			m_SmellyMeter -= potency;
			if (m_SmellyMeter <= 0)
			{
				m_SmellyMeter = 0;
				m_IsSmelly = false;
				m_SmellyFX.SetActive(value: false);
				m_CleanFX.SetActive(value: true);
				CSingleton<CustomerManager>.Instance.RemoveFromSmellyCustomerList(this);
				CPlayerData.m_GameReportDataCollect.smellyCustomerCleaned++;
				CPlayerData.m_GameReportDataCollectPermanent.smellyCustomerCleaned++;
				AchievementManager.OnCleanSmellyCustomer(CPlayerData.m_GameReportDataCollectPermanent.smellyCustomerCleaned);
			}
		}
	}

	private bool HasNoItemOrCheckedOut()
	{
		if (m_ItemInBagList.Count + m_CardInBagList.Count > 0)
		{
			return m_HasCheckedOut;
		}
		return true;
	}

	private bool StenchLeaveCheck()
	{
		if (m_IsInsideShop && !m_IsSmelly)
		{
			int smellyCustomerInsideShopCount = CSingleton<CustomerManager>.Instance.GetSmellyCustomerInsideShopCount();
			int num = UnityEngine.Random.Range(0, 100);
			if (smellyCustomerInsideShopCount > 0)
			{
				AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 1, -30 * smellyCustomerInsideShopCount);
			}
			if (smellyCustomerInsideShopCount > 0 && num < smellyCustomerInsideShopCount * 15)
			{
				if (!CSingleton<CustomerManager>.Instance.IsWithinSmellyCustomerRange(base.transform.position))
				{
					return false;
				}
				List<string> list = new List<string>();
				list.Add(LocalizationManager.GetTranslation("Ugh, the stench..."));
				list.Add(LocalizationManager.GetTranslation("It's too smelly!"));
				list.Add(LocalizationManager.GetTranslation("Omg this place stinks!"));
				PopupText(list, 100);
				AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 0, -30 * CSingleton<CustomerManager>.Instance.GetSmellyCustomerList().Count, addAsNew: true);
				AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 0, -30 * CSingleton<CustomerManager>.Instance.GetSmellyCustomerList().Count, addAsNew: true);
				if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
				{
					ThinkWantToPay();
				}
				else
				{
					ExitShop();
				}
				return true;
			}
		}
		return false;
	}

	private void OnCustomerReachInsideShop()
	{
		CPlayerData.m_GameReportDataCollect.customerVisited++;
		CPlayerData.m_GameReportDataCollectPermanent.customerVisited++;
		if (m_CurrentState == ECustomerState.WalkToPlayTable)
		{
			if ((bool)m_CurrentPlayTable && m_CurrentPlayTableSeatIndex != -1)
			{
				m_CurrentPlayTable.CustomerUnbookSeatIndex(m_CurrentPlayTableSeatIndex);
			}
			AttemptFindPlayTable();
		}
		if (m_CurrentState == ECustomerState.WalkToShelf)
		{
			AttemptFindShelf();
		}
	}

	private void DetermineShopAction()
	{
		if (EndOfDayReportScreen.IsActive())
		{
			ExitShop();
		}
		else
		{
			if (StenchLeaveCheck())
			{
				return;
			}
			bool flag = ShelfManager.GetShelfToBuyItem(m_TargetBuyItemList, 50, canReturnNull: true) != null;
			bool flag2 = ShelfManager.GetCardShelfToBuyCard() != null;
			bool flag3 = ShelfManager.GetPlayTableToPlay(findTableWithPlayerWaiting: false) != null;
			bool flag4 = ShelfManager.HasPlayTableWithPlayerWaiting();
			bool flag5 = ShelfManager.GetBulkDonationBoxToGetCard() != null;
			bool flag6 = false;
			int num = 3 + CPlayerData.m_ShopLevel / 4;
			if (LightManager.IsMorning())
			{
				if (num > 5)
				{
					num = 5;
				}
			}
			else if (LightManager.IsAfternoon())
			{
				if (num > 10)
				{
					num = 10;
				}
			}
			else if (LightManager.IsEvening())
			{
				if (num > 15)
				{
					num = 15;
				}
			}
			else if (num > 8)
			{
				num = 8;
			}
			num += CPlayerData.m_ShopLevel / 20;
			if (num > 20)
			{
				num = 20;
			}
			if (m_IsInsideShop && !m_HasTradedCard && UnityEngine.Random.Range(0, 100) < num)
			{
				m_CurrentTradeCardCashierCounter = ShelfManager.GetCashierCounterToTradeCard();
				if (m_CurrentTradeCardCashierCounter != null)
				{
					m_TargetTransform = m_CurrentTradeCardCashierCounter.GetTradeCardStandLoc();
					if ((bool)m_TargetTransform)
					{
						flag6 = true;
						m_ReachedEndOfPath = false;
						m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
						m_IsWaitingForPathCallback = true;
						m_UnableToFindQueue = false;
						SetState(ECustomerState.WantToTradeCard);
					}
				}
			}
			else if ((bool)m_CurrentTradeCardCashierCounter)
			{
				m_CurrentTradeCardCashierCounter.CustomerFinishTradingCard();
				m_CurrentTradeCardCashierCounter = null;
			}
			if (!flag6 && flag5 && !m_HasCheckedOut)
			{
				int num2 = UnityEngine.Random.Range(0, 100);
				if (LightManager.IsAfternoon())
				{
					num2 -= 5;
				}
				if (m_HasPlayedGame)
				{
					num2 -= 10;
				}
				if (num2 < 15)
				{
					ThinkWantToGetCardFromBulkBox();
					flag6 = true;
				}
			}
			if (!flag6 && !m_HasPlayedGame && (flag3 || flag4) && HasNoItemOrCheckedOut() && !m_IsAngry)
			{
				int num3 = UnityEngine.Random.Range(0, 100);
				if (LightManager.IsMorning())
				{
					num3 += 15;
				}
				if (LightManager.IsEvening())
				{
					num3 -= 30;
				}
				if (LightManager.IsNight())
				{
					num3 -= 40;
				}
				if (flag4)
				{
					num3 -= 25;
				}
				if (num3 < 33)
				{
					ThinkWantToPlayTable();
					flag6 = true;
				}
			}
			if (!flag6 && flag2 && !m_HasCheckedOut)
			{
				int num4 = UnityEngine.Random.Range(0, 100);
				if (LightManager.IsAfternoon())
				{
					num4 -= 15;
				}
				if (m_HasPlayedGame)
				{
					num4 -= 30;
				}
				if (m_LookingForCardOnly)
				{
					num4 -= 100;
				}
				if (num4 < 35)
				{
					ThinkWantToBuyCard();
					flag6 = true;
				}
			}
			if (!flag6)
			{
				if (!flag && m_ItemInBagList.Count == 0 && m_TargetBuyItemList.Count > 0)
				{
					AddReviewData(ECustomerReviewType.ItemVariety, m_TargetBuyItemList[UnityEngine.Random.Range(0, m_TargetBuyItemList.Count)], 0, -10, addAsNew: true);
				}
				if (!m_HasCheckedOut)
				{
					ThinkWantToBuyItem();
				}
				else
				{
					ExitShop();
				}
			}
		}
	}

	private void ThinkWantToBuyItem()
	{
		if (m_FailFindItemAttemptCount > UnityEngine.Random.Range(1, 5))
		{
			if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
			{
				ThinkWantToPay();
			}
			else
			{
				ExitShop();
			}
		}
		else
		{
			SetState(ECustomerState.WantToBuyItem);
			AttemptFindShelf();
		}
	}

	private void ThinkWantToBuyCard()
	{
		int num = 7 + ShelfManager.GetCardShelfList().Count;
		if (num > 15)
		{
			num = 15;
		}
		if (m_FailFindItemAttemptCount > UnityEngine.Random.Range(3, num))
		{
			if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
			{
				ThinkWantToPay();
			}
			else
			{
				ExitShop();
			}
		}
		else
		{
			SetState(ECustomerState.WantToBuyCard);
			AttemptFindCardShelf();
		}
	}

	private void ThinkWantToGetCardFromBulkBox()
	{
		if (m_FailFindItemAttemptCount > UnityEngine.Random.Range(3, 8))
		{
			if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
			{
				ThinkWantToPay();
			}
			else
			{
				ExitShop();
			}
		}
		else
		{
			SetState(ECustomerState.WantToTakeBulkDonationCard);
			AttemptFindBulkDonationBox();
		}
	}

	private void ThinkWantToPlayTable()
	{
		if (m_FailFindItemAttemptCount > UnityEngine.Random.Range(1, 5))
		{
			if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
			{
				ThinkWantToPay();
			}
			else
			{
				ExitShop();
			}
		}
		else
		{
			SetState(ECustomerState.WantToPlayGame);
			AttemptFindPlayTable();
		}
	}

	private void AttemptFindShelf()
	{
		m_ReachedEndOfPath = false;
		m_CurrentShelf = ShelfManager.GetShelfToBuyItem(m_TargetBuyItemList);
		if ((bool)m_CurrentShelf)
		{
			m_CurrentItemCompartment = m_CurrentShelf.GetCustomerTargetItemCompartment(m_TargetBuyItemList);
			m_TargetTransform = m_CurrentItemCompartment.m_CustomerStandLoc;
		}
		else
		{
			m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
		}
		if ((bool)m_TargetTransform)
		{
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(ECustomerState.WalkToShelf);
		}
		else
		{
			m_UnableToFindQueue = true;
			Debug.Log(base.transform.name + " unable to find shelf");
		}
	}

	private void TakeItemFromShelf()
	{
		m_IsInsideShop = true;
		if (!m_CurrentShelf)
		{
			SetState(ECustomerState.Idle);
			return;
		}
		_ = (m_CurrentShelf.transform.position - base.transform.position).magnitude;
		_ = (m_CurrentItemCompartment.transform.position - base.transform.position).magnitude;
		if (m_CurrentItemCompartment.GetItemCount() > 0 && m_CurrentShelf.IsValidObject())
		{
			Item lastItem = m_CurrentItemCompartment.GetLastItem();
			ItemData itemData = InventoryBase.GetItemData(lastItem.GetItemType());
			int num = Mathf.RoundToInt((50f - itemData.GetItemVolume()) / 10f);
			if (num < 0)
			{
				num = 0;
			}
			if (m_CurrentItemCompartment.GetItemCount() > 3)
			{
				num = 0;
			}
			int num2 = UnityEngine.Random.Range(0, Mathf.Clamp(m_CurrentItemCompartment.GetItemCount() + 1 + num, 4, 14));
			int num3 = UnityEngine.Random.Range(1, 60);
			float num4 = 0f;
			if (itemData.isNotBoosterPack)
			{
				if (num2 > 1)
				{
					num2 /= 2;
				}
				else if (UnityEngine.Random.Range(0, 100) < 75)
				{
					num2 = 1;
				}
			}
			if (lastItem.GetItemType() == EItemType.Deodorant)
			{
				num2 = ((!m_IsSmelly) ? UnityEngine.Random.Range(0, 4) : 0);
			}
			num2 = Mathf.Clamp(num2, 0, m_CurrentItemCompartment.GetItemCount());
			bool flag = false;
			if (m_CurrentItemCompartment.IsSettingPrice())
			{
				flag = true;
				num2 = 0;
			}
			bool flag2 = false;
			for (int i = 0; i < num2; i++)
			{
				lastItem = m_CurrentItemCompartment.GetLastItem();
				if (!lastItem || (CSingleton<SetItemPriceScreen>.Instance.GetCurrentSettingPriceItemType() != EItemType.None && lastItem.GetItemType() == CSingleton<SetItemPriceScreen>.Instance.GetCurrentSettingPriceItemType()))
				{
					continue;
				}
				float itemPrice = CPlayerData.GetItemPrice(lastItem.GetItemType());
				float itemMarketPrice = CPlayerData.GetItemMarketPrice(lastItem.GetItemType());
				int customerBuyItemChance = CSingleton<CustomerManager>.Instance.GetCustomerBuyItemChance(itemPrice, itemMarketPrice);
				int num5 = UnityEngine.Random.Range(0, 100);
				if (!flag2)
				{
					flag2 = BuyItemSpeechPopup(customerBuyItemChance, itemPrice, itemData.GetName(), lastItem.GetItemType());
				}
				if (num5 >= customerBuyItemChance)
				{
					AddReviewData(ECustomerReviewType.ItemPrice, lastItem.GetItemType(), 1, -40);
					continue;
				}
				m_ShoppingBagTransform.gameObject.SetActive(value: true);
				m_CurrentItemCompartment.RemoveItem(lastItem);
				lastItem.m_Mesh.enabled = true;
				m_ItemInBagList.Add(lastItem);
				lastItem.SetCurrentPrice(itemPrice);
				lastItem.LerpToTransform(m_ShoppingBagTransform, m_ShoppingBagTransform);
				lastItem.SetHideItemAfterFinishLerp();
				m_CurrentCostTotal += itemPrice;
				num4 += lastItem.GetItemVolume();
				if (num4 >= (float)num3 || m_CurrentCostTotal >= m_MaxMoney)
				{
					break;
				}
			}
			if (num4 > 0f)
			{
				if (m_TargetBuyItemList.Count > 0)
				{
					m_TargetBuyItemList.Remove(lastItem.GetItemType());
					AddReviewData(ECustomerReviewType.ItemVariety, lastItem.GetItemType(), 1, 25);
				}
				m_ShoppingBagTransform.gameObject.SetActive(value: true);
				m_Anim.SetBool("HoldingBag", value: true);
				m_Anim.SetTrigger("GrabItem");
			}
			else if (!flag)
			{
				m_FailFindItemAttemptCount++;
			}
			if (m_DecideFinishShopping != null)
			{
				StopCoroutine(m_DecideFinishShopping);
			}
			m_DecideFinishShopping = StartCoroutine(DecideIfFinishShopping());
		}
		else
		{
			CustomerConfused();
			m_HasTookItemFromShelf = false;
			m_FailFindItemAttemptCount++;
			SetState(ECustomerState.Idle);
		}
	}

	private bool BuyItemSpeechPopup(int randomBuyItemChance, float currentPrice, string itemName, EItemType itemType)
	{
		List<string> list = new List<string>();
		if (randomBuyItemChance >= 100)
		{
			list.Add(LocalizationManager.GetTranslation("Buy buy buy!!"));
			list.Add(LocalizationManager.GetTranslation("Wow this is the cheapest I've seen!"));
			list.Add(LocalizationManager.GetTranslation("What a steal!"));
			AddReviewData(ECustomerReviewType.ItemPrice, itemType, 1, 80);
		}
		else if (randomBuyItemChance >= 90)
		{
			list.Add(LocalizationManager.GetTranslation("Cheap XXX. What a bargain!").Replace("XXX", itemName));
			list.Add(LocalizationManager.GetTranslation("This is tempting!"));
			list.Add(LocalizationManager.GetTranslation("Wow so cheap!"));
			AddReviewData(ECustomerReviewType.ItemPrice, itemType, 1, 80);
		}
		else if (randomBuyItemChance >= 70)
		{
			if (currentPrice >= 2f)
			{
				list.Add(LocalizationManager.GetTranslation("XXX bucks for this is fair.").Replace("XXX", GameInstance.GetPriceString(currentPrice)));
			}
			list.Add(LocalizationManager.GetTranslation("Good price!"));
			list.Add(LocalizationManager.GetTranslation("Wow, gotta take one of these."));
			AddReviewData(ECustomerReviewType.ItemPrice, itemType, 1, 60);
		}
		else if (randomBuyItemChance >= 50)
		{
			list.Add(LocalizationManager.GetTranslation("Fair price for a XXX.").Replace("XXX", itemName));
			list.Add(LocalizationManager.GetTranslation("Not a bad price really."));
			list.Add(LocalizationManager.GetTranslation("Fair price I'd say."));
			list.Add(LocalizationManager.GetTranslation("This is fine."));
			AddReviewData(ECustomerReviewType.ItemPrice, itemType, 1, 30);
		}
		else if (randomBuyItemChance >= 30)
		{
			list.Add(LocalizationManager.GetTranslation("XXX is a little expensive, hmm...").Replace("XXX", itemName));
			list.Add(LocalizationManager.GetTranslation("Can't decide if I really want this."));
			list.Add(LocalizationManager.GetTranslation("It's pricey, I need to think a little."));
			AddReviewData(ECustomerReviewType.ItemPrice, itemType, 1, -30);
		}
		else if (randomBuyItemChance >= 1)
		{
			if (currentPrice >= 2f)
			{
				list.Add(LocalizationManager.GetTranslation("This cost XXX bucks!?").Replace("XXX", GameInstance.GetPriceString(currentPrice)));
			}
			list.Add(LocalizationManager.GetTranslation("Man, XXX sure is expensive!").Replace("XXX", itemName));
			list.Add(LocalizationManager.GetTranslation("God, so expensive."));
			AddReviewData(ECustomerReviewType.ItemPrice, itemType, 1, -60);
		}
		else if (randomBuyItemChance <= 0)
		{
			if (currentPrice >= 2f)
			{
				list.Add(LocalizationManager.GetTranslation("Ridiculous. XXX bucks for this!?").Replace("XXX", GameInstance.GetPriceString(currentPrice)));
			}
			list.Add(LocalizationManager.GetTranslation("Crazy. Who's gonna buy XXX at this price!?").Replace("XXX", itemName));
			list.Add(LocalizationManager.GetTranslation("This is a scam!"));
			AddReviewData(ECustomerReviewType.ItemPrice, itemType, 1, -100);
		}
		if (list.Count > 0)
		{
			return PopupText(list);
		}
		return false;
	}

	private void TakeCardFromShelf()
	{
		m_IsInsideShop = true;
		if (!m_CurrentCardShelf)
		{
			SetState(ECustomerState.Idle);
			return;
		}
		_ = (m_CurrentCardShelf.transform.position - base.transform.position).magnitude;
		_ = (m_CurrentCardCompartment.transform.position - base.transform.position).magnitude;
		if (m_CurrentCardCompartment.m_StoredCardList.Count > 0 && m_CurrentCardShelf.IsValidObject())
		{
			InteractableCard3d interactableCard3d = m_CurrentCardCompartment.m_StoredCardList[0];
			float cardPrice = CPlayerData.GetCardPrice(interactableCard3d.m_Card3dUI.m_CardUI.GetCardData());
			float cardMarketPrice = CPlayerData.GetCardMarketPrice(interactableCard3d.m_Card3dUI.m_CardUI.GetCardData());
			int customerBuyItemChance = CSingleton<CustomerManager>.Instance.GetCustomerBuyItemChance(cardPrice, cardMarketPrice);
			int num = UnityEngine.Random.Range(0, 100);
			bool flag = true;
			if (num >= customerBuyItemChance)
			{
				flag = false;
				m_FailFindItemAttemptCount++;
			}
			else if (m_CurrentCardCompartment.IsSettingPrice())
			{
				flag = false;
			}
			else if (CSingleton<SetItemPriceScreen>.Instance.GetCurrentSettingPriceCardData() != null && CSingleton<SetItemPriceScreen>.Instance.GetCurrentSettingPriceCardData().IsSameCardDataType(interactableCard3d.m_Card3dUI.m_CardUI.GetCardData()))
			{
				flag = false;
			}
			BuyCardSpeechPopup(customerBuyItemChance, cardPrice, m_CurrentCostTotal + cardPrice <= m_MaxMoney);
			if (cardPrice > 0f && flag && m_CurrentCostTotal + cardPrice <= m_MaxMoney && interactableCard3d.IsDisplayedOnShelf())
			{
				m_ShoppingBagTransform.gameObject.SetActive(value: true);
				interactableCard3d.SetHideItemAfterFinishLerp();
				m_CurrentCardCompartment.RemoveCardFromShelf(m_ShoppingBagTransform, m_ShoppingBagTransform);
				m_CardInBagList.Add(interactableCard3d);
				interactableCard3d.SetCurrentPrice(cardPrice);
				m_CurrentCostTotal += cardPrice;
				m_Anim.SetBool("HoldingBag", value: true);
				m_Anim.SetTrigger("GrabItem");
				if (m_DecideFinishShopping != null)
				{
					StopCoroutine(m_DecideFinishShopping);
				}
				m_DecideFinishShopping = StartCoroutine(DecideIfFinishShopping());
			}
			else
			{
				if (m_DecideFinishShopping != null)
				{
					StopCoroutine(m_DecideFinishShopping);
				}
				m_DecideFinishShopping = StartCoroutine(DecideIfFinishShopping());
			}
		}
		else
		{
			if (UnityEngine.Random.Range(0, 100) < 20)
			{
				AddReviewData(ECustomerReviewType.CardRarity, EItemType.None, 1, -20);
			}
			CustomerConfused();
			m_HasTookItemFromShelf = false;
			m_HasTookCardFromShelf = false;
			m_HasTookCardFromBulkDonationBox = false;
			m_FailFindItemAttemptCount++;
			SetState(ECustomerState.Idle);
		}
	}

	private void TakeCardFromBulkDonationBox()
	{
		m_IsInsideShop = true;
		if (!m_CurrentBulkDonationBox)
		{
			SetState(ECustomerState.Idle);
			return;
		}
		_ = (m_CurrentBulkDonationBox.transform.position - base.transform.position).magnitude;
		if (m_CurrentBulkDonationBox.IsValidObject() && m_CurrentBulkDonationBox.GetTotalCardAmount() > 0 && !m_CurrentBulkDonationBox.IsEditingBulkBox())
		{
			int num = m_CurrentBulkDonationBox.RemoveRandomCardFromShelf();
			m_Anim.SetTrigger("GrabItem");
			int num2 = Mathf.CeilToInt(num / 3);
			for (int i = 0; i < num2; i++)
			{
				AddReviewData(ECustomerReviewType.BulkDonation, EItemType.None, 2, 100, addAsNew: true);
			}
			if (m_DecideFinishShopping != null)
			{
				StopCoroutine(m_DecideFinishShopping);
			}
			m_DecideFinishShopping = StartCoroutine(DecideIfFinishShopping());
		}
		else
		{
			SetState(ECustomerState.Idle);
		}
	}

	private bool BuyCardSpeechPopup(int randomBuyItemChance, float cardPrice, bool hasEnoughMoney)
	{
		int appearChance = 15;
		int num = 70;
		List<string> list = new List<string>();
		if (randomBuyItemChance >= 100)
		{
			if (hasEnoughMoney)
			{
				list.Add(LocalizationManager.GetTranslation("Buy buy buy!!"));
				list.Add(LocalizationManager.GetTranslation("Wow this is the cheapest I've seen!"));
				list.Add(LocalizationManager.GetTranslation("What a steal!"));
				AddReviewData(ECustomerReviewType.CardPrice, EItemType.None, 1, 80);
			}
			else
			{
				appearChance = num;
				list.Add(LocalizationManager.GetTranslation("I want this, but I am broke."));
				list.Add(LocalizationManager.GetTranslation("This is a steal, but I don't have enough money."));
				list.Add(LocalizationManager.GetTranslation("I would so buy this if I have enough money"));
				list.Add(LocalizationManager.GetTranslation("This is the cheapest I've seen, but I am broke."));
				AddReviewData(ECustomerReviewType.CardRarity, EItemType.None, 1, 80);
			}
		}
		else if (randomBuyItemChance >= 90)
		{
			if (hasEnoughMoney)
			{
				list.Add(LocalizationManager.GetTranslation("Cheap card. What a bargain!"));
				list.Add(LocalizationManager.GetTranslation("This is tempting!"));
				list.Add(LocalizationManager.GetTranslation("Wow so cheap!"));
				AddReviewData(ECustomerReviewType.CardPrice, EItemType.None, 1, 80);
			}
			else
			{
				appearChance = num;
				list.Add(LocalizationManager.GetTranslation("So tempting, but I am broke."));
				list.Add(LocalizationManager.GetTranslation("Tempting, but I don't have enough money."));
				list.Add(LocalizationManager.GetTranslation("Wow, if only I have enough money."));
				AddReviewData(ECustomerReviewType.CardRarity, EItemType.None, 1, 80);
			}
		}
		else if (randomBuyItemChance >= 70)
		{
			if (hasEnoughMoney)
			{
				if (cardPrice >= 2f)
				{
					list.Add(LocalizationManager.GetTranslation("XXX bucks for this is fair.").Replace("XXX", GameInstance.GetPriceString(cardPrice)));
				}
				list.Add(LocalizationManager.GetTranslation("Good price!"));
				list.Add(LocalizationManager.GetTranslation("Wow, gotta take one of these."));
				AddReviewData(ECustomerReviewType.CardPrice, EItemType.None, 1, 60);
			}
			else
			{
				appearChance = num;
				list.Add(LocalizationManager.GetTranslation("Good price, but I am broke."));
				list.Add(LocalizationManager.GetTranslation("Seems fair, but I don't have enough money."));
				list.Add(LocalizationManager.GetTranslation("Wow, if only I have enough money."));
				AddReviewData(ECustomerReviewType.CardRarity, EItemType.None, 1, 60);
			}
		}
		else if (randomBuyItemChance >= 50)
		{
			if (hasEnoughMoney)
			{
				list.Add(LocalizationManager.GetTranslation("Not a bad price really."));
				list.Add(LocalizationManager.GetTranslation("Fair price I'd say."));
				list.Add(LocalizationManager.GetTranslation("This is fine."));
				AddReviewData(ECustomerReviewType.CardPrice, EItemType.None, 1, 30);
			}
			else
			{
				appearChance = num;
				list.Add(LocalizationManager.GetTranslation("Not a bad price, but I am broke."));
				list.Add(LocalizationManager.GetTranslation("Fair price, but I don't have enough money."));
				list.Add(LocalizationManager.GetTranslation("Maybe if I have enough money."));
				AddReviewData(ECustomerReviewType.CardRarity, EItemType.None, 1, 30);
			}
		}
		else if (randomBuyItemChance >= 30)
		{
			list.Add(LocalizationManager.GetTranslation("It's a little expensive, hmm..."));
			list.Add(LocalizationManager.GetTranslation("Can't decide if I really want this."));
			list.Add(LocalizationManager.GetTranslation("It's pricey, I need to think a little."));
			AddReviewData(ECustomerReviewType.CardPrice, EItemType.None, 1, -30);
		}
		else if (randomBuyItemChance >= 1)
		{
			if (cardPrice >= 2f)
			{
				list.Add(LocalizationManager.GetTranslation("This cost XXX bucks!?").Replace("XXX", GameInstance.GetPriceString(cardPrice)));
			}
			list.Add(LocalizationManager.GetTranslation("Man, this is expensive!"));
			list.Add(LocalizationManager.GetTranslation("God, so expensive."));
			AddReviewData(ECustomerReviewType.CardPrice, EItemType.None, 1, -60);
		}
		else if (randomBuyItemChance <= 0)
		{
			if (cardPrice >= 2f)
			{
				list.Add(LocalizationManager.GetTranslation("Ridiculous. XXX bucks!?").Replace("XXX", GameInstance.GetPriceString(cardPrice)));
			}
			list.Add(LocalizationManager.GetTranslation("Crazy. Who's gonna buy at this price!?"));
			list.Add(LocalizationManager.GetTranslation("This is a scam!"));
			list.Add(LocalizationManager.GetTranslation("This card is way overpriced!"));
			AddReviewData(ECustomerReviewType.CardPrice, EItemType.None, 1, -100);
		}
		if (list.Count > 0)
		{
			return PopupText(list, appearChance);
		}
		return false;
	}

	private void AttemptFindCardShelf()
	{
		m_ReachedEndOfPath = false;
		m_CurrentCardShelf = ShelfManager.GetCardShelfToBuyCard();
		bool flag = false;
		if ((bool)m_CurrentCardShelf)
		{
			flag = true;
			m_CurrentCardCompartment = m_CurrentCardShelf.GetCustomerTargetCardCompartment();
			m_TargetTransform = m_CurrentCardCompartment.m_CustomerStandLoc;
		}
		else
		{
			m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
		}
		if ((bool)m_TargetTransform && flag)
		{
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(ECustomerState.WalkToCardShelf);
		}
		else
		{
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = true;
			m_FailFindItemAttemptCount++;
			DetermineShopAction();
		}
	}

	private void AttemptFindBulkDonationBox()
	{
		m_ReachedEndOfPath = false;
		m_CurrentBulkDonationBox = ShelfManager.GetBulkDonationBoxToGetCard();
		bool flag = false;
		if ((bool)m_CurrentBulkDonationBox)
		{
			flag = true;
			m_TargetTransform = m_CurrentBulkDonationBox.m_CustomerStandLocList[UnityEngine.Random.Range(0, m_CurrentBulkDonationBox.m_CustomerStandLocList.Count)];
		}
		else
		{
			m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
		}
		if ((bool)m_TargetTransform && flag)
		{
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(ECustomerState.WalkToBulkDonationBox);
		}
		else
		{
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = true;
			DetermineShopAction();
		}
	}

	public bool FindPlaytableWithWaitingCustomer()
	{
		if ((m_CurrentState == ECustomerState.QueueingPlayTable || m_CurrentState == ECustomerState.WalkToPlayTable) && ShelfManager.HasPlayTableWithPlayerWaiting() && !m_IsWalkingToPlayTableGame)
		{
			m_IsWalkingToPlayTableGame = true;
			AttemptFindPlayTable();
			return true;
		}
		return false;
	}

	private void AttemptFindPlayTable()
	{
		m_ReachedEndOfPath = false;
		if (ShelfManager.HasPlayTableWithPlayerWaiting())
		{
			m_CurrentPlayTable = ShelfManager.GetPlayTableToPlay(findTableWithPlayerWaiting: true);
		}
		else
		{
			m_CurrentPlayTable = ShelfManager.GetPlayTableToPlay(findTableWithPlayerWaiting: false);
		}
		if ((bool)m_CurrentPlayTable)
		{
			m_CurrentPlayTableSeatIndex = m_CurrentPlayTable.GetEmptySeatBookingIndex();
			if (m_CurrentPlayTableSeatIndex != -1)
			{
				m_CurrentPlayTable.CustomerBookSeatIndex(m_CurrentPlayTableSeatIndex);
				m_TargetTransform = m_CurrentPlayTable.GetStandLoc(m_CurrentPlayTableSeatIndex);
			}
		}
		else
		{
			m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
		}
		if ((bool)m_TargetTransform && m_CurrentPlayTableSeatIndex != -1)
		{
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(ECustomerState.WalkToPlayTable);
		}
		else
		{
			m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = true;
			m_IsWalkingToPlayTableGame = false;
			m_FailFindItemAttemptCount++;
			DetermineShopAction();
		}
	}

	private IEnumerator DecideIfFinishShopping()
	{
		yield return new WaitForSeconds(1.5f);
		m_HasTookItemFromShelf = false;
		m_HasTookCardFromShelf = false;
		m_HasTookCardFromBulkDonationBox = false;
		float num = 0f;
		for (int i = 0; i < m_ItemInBagList.Count; i++)
		{
			num += m_ItemInBagList[i].GetItemVolume();
		}
		if (UnityEngine.Random.Range(0, 100) < 50 && num < 50f && m_CurrentCostTotal < m_MaxMoney)
		{
			DetermineShopAction();
		}
		else if (m_LookingForCardOnly && m_CardInBagList.Count < UnityEngine.Random.Range(2, 10) && m_CurrentCostTotal < m_MaxMoney)
		{
			DetermineShopAction();
		}
		else if (m_ItemInBagList.Count == 0 && m_CardInBagList.Count == 0)
		{
			DetermineShopAction();
		}
		else
		{
			ThinkWantToPay();
		}
	}

	private void ThinkWantToPay()
	{
		if (m_HasCheckedOut)
		{
			ExitShop();
			return;
		}
		SetState(ECustomerState.WantToPay);
		AttemptFindQueue();
	}

	private void AttemptFindQueue()
	{
		m_ReachedEndOfPath = false;
		m_CurrentQueueCashierCounter = ShelfManager.GetCashierCounter();
		if ((bool)m_CurrentQueueCashierCounter)
		{
			m_TargetTransform = m_CurrentQueueCashierCounter.GetQueuePosition();
		}
		else
		{
			m_TargetTransform = null;
		}
		if ((bool)m_TargetTransform)
		{
			m_CurrentQueueCashierCounter.AddCustomerToQueue(this);
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(ECustomerState.QueuingToPay);
		}
		else
		{
			m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = true;
		}
	}

	private void OnReachedPathEnd()
	{
		if ((bool)m_TargetTransform)
		{
			m_TargetLerpRotation = m_TargetTransform.rotation;
		}
		float num = 0f;
		if ((bool)m_TargetTransform)
		{
			num = (m_TargetTransform.position - base.transform.position).magnitude;
		}
		if (num > 0.5f && m_CurrentState == ECustomerState.ExitingShop && m_IsInsideShop)
		{
			Vector3 forward = m_TargetTransform.position - base.transform.position;
			forward.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward, Vector3.up);
			List<string> list = new List<string>();
			list.Add(LocalizationManager.GetTranslation("The door is blocked!!"));
			list.Add(LocalizationManager.GetTranslation("I can't fit in the exit!"));
			list.Add(LocalizationManager.GetTranslation("How do I get out!?"));
			list.Add(LocalizationManager.GetTranslation("Wait, how do I get out!?"));
			list.Add(LocalizationManager.GetTranslation("Let me out!"));
			AddReviewData(ECustomerReviewType.BlockedStore, EItemType.None, 0, -20);
			PopupText(list, 50);
			m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			CustomerConfused();
			SetState(ECustomerState.Idle);
			return;
		}
		if (num > 0.5f && m_CurrentState != ECustomerState.QueuingToPay && m_CurrentState != ECustomerState.WantToPay)
		{
			Vector3 forward2 = m_TargetTransform.position - base.transform.position;
			forward2.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward2, Vector3.up);
			if (!m_IsInsideShop)
			{
				List<string> list2 = new List<string>();
				list2.Add(LocalizationManager.GetTranslation("The door is blocked!!"));
				list2.Add(LocalizationManager.GetTranslation("I can't fit in the entrance!"));
				list2.Add(LocalizationManager.GetTranslation("How do I get in!?"));
				list2.Add(LocalizationManager.GetTranslation("Wait, how do I get in!?"));
				list2.Add(LocalizationManager.GetTranslation("Let me in!"));
				AddReviewData(ECustomerReviewType.BlockedStore, EItemType.None, 0, -20);
				PopupText(list2, 50);
				m_TargetTransform = CustomerManager.GetRandomShopWindowOutsidePoint();
			}
			else
			{
				m_TargetTransform = CustomerManager.GetRandomShopLocationPoint();
			}
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_IsShopBlocked = true;
			CustomerConfused();
			m_FailFindItemAttemptCount++;
			m_FailFindShelfAttemptCount++;
			return;
		}
		if (m_IsShopBlocked)
		{
			m_IsShopBlocked = false;
			SetState(ECustomerState.Idle);
			return;
		}
		m_IsShopBlocked = false;
		if (m_CurrentState == ECustomerState.ShopNotOpen)
		{
			if (CPlayerData.m_IsShopOpen)
			{
				DetermineShopAction();
				return;
			}
			CustomerConfused();
			m_HasTookItemFromShelf = false;
			m_FailFindItemAttemptCount++;
			SetState(ECustomerState.Idle);
		}
		else if (m_CurrentState == ECustomerState.WalkToShelf)
		{
			if (!m_CurrentShelf)
			{
				CustomerConfused();
				if (m_FailFindShelfAttemptCount > UnityEngine.Random.Range(1, 5))
				{
					StartCoroutine(DelayExitShop(2f));
				}
				else
				{
					AttemptFindShelf();
				}
				m_FailFindShelfAttemptCount++;
				return;
			}
			Vector3 forward3 = m_CurrentItemCompartment.transform.position - base.transform.position;
			forward3.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward3, Vector3.up);
			if (m_CurrentItemCompartment.GetItemCount() > 0 && m_CurrentShelf.IsValidObject())
			{
				if (m_CurrentShelf.HasItemTypeOnShelf(m_TargetBuyItemList))
				{
					SetState(ECustomerState.TakingItemFromShelf);
					return;
				}
				if ((bool)ShelfManager.GetShelfToBuyItem(m_TargetBuyItemList, 50, canReturnNull: true))
				{
					m_FailFindItemAttemptCount++;
					SetState(ECustomerState.Idle);
					return;
				}
				if (m_TargetBuyItemList.Count > 0)
				{
					EItemType itemType = m_TargetBuyItemList[UnityEngine.Random.Range(0, m_TargetBuyItemList.Count)];
					ItemData itemData = InventoryBase.GetItemData(itemType);
					List<string> list3 = new List<string>();
					list3.Add(LocalizationManager.GetTranslation("I can't find XXX.").Replace("XXX", itemData.GetName()));
					list3.Add(LocalizationManager.GetTranslation("Where is XXX?").Replace("XXX", itemData.GetName()));
					list3.Add(LocalizationManager.GetTranslation("No XXX here.").Replace("XXX", itemData.GetName()));
					list3.Add(LocalizationManager.GetTranslation("I need XXX.").Replace("XXX", itemData.GetName()));
					list3.Add(LocalizationManager.GetTranslation("Why can't I get XXX in this shop!?").Replace("XXX", itemData.GetName()));
					list3.Add(LocalizationManager.GetTranslation("I am looking for XXX.").Replace("XXX", itemData.GetName()));
					list3.Add(LocalizationManager.GetTranslation("No XXX here.").Replace("XXX", itemData.GetName()));
					PopupText(list3, 80);
					AddReviewData(ECustomerReviewType.ItemVariety, itemType, 0, -60);
				}
				CustomerConfused();
				m_FailFindItemAttemptCount++;
				SetState(ECustomerState.Idle);
			}
			else
			{
				if (m_TargetBuyItemList.Count > 0)
				{
					EItemType itemType2 = m_TargetBuyItemList[UnityEngine.Random.Range(0, m_TargetBuyItemList.Count)];
					InventoryBase.GetItemData(itemType2);
					AddReviewData(ECustomerReviewType.ItemVariety, itemType2, 0, -60);
					AddReviewData(ECustomerReviewType.ItemVariety, itemType2, 0, -50, addAsNew: true);
				}
				CustomerConfused();
				m_FailFindItemAttemptCount++;
				SetState(ECustomerState.Idle);
			}
		}
		else if (m_CurrentState == ECustomerState.WalkToCardShelf)
		{
			if (!m_CurrentCardShelf)
			{
				CustomerConfused();
				AddReviewData(ECustomerReviewType.CardRarity, EItemType.None, 1, -30);
				if (m_FailFindShelfAttemptCount > UnityEngine.Random.Range(1, 5))
				{
					StartCoroutine(DelayExitShop(2f));
				}
				else
				{
					AttemptFindCardShelf();
				}
				m_FailFindShelfAttemptCount++;
				return;
			}
			Vector3 forward4 = m_CurrentCardCompartment.transform.position - base.transform.position;
			forward4.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward4, Vector3.up);
			if (m_CurrentCardCompartment.m_StoredCardList.Count > 0 && m_CurrentCardShelf.IsValidObject())
			{
				if (m_SoldCardData != m_CurrentCardCompartment.m_StoredCardList[0].m_Card3dUI.m_CardUI.GetCardData())
				{
					SetState(ECustomerState.TakingItemFromCardShelf);
				}
				else
				{
					SetState(ECustomerState.Idle);
				}
				return;
			}
			if (UnityEngine.Random.Range(0, 100) < 20)
			{
				AddReviewData(ECustomerReviewType.CardRarity, EItemType.None, 1, -20);
			}
			CustomerConfused();
			m_FailFindItemAttemptCount++;
			SetState(ECustomerState.Idle);
		}
		else if (m_CurrentState == ECustomerState.WalkToBulkDonationBox)
		{
			if (!m_CurrentBulkDonationBox)
			{
				CustomerConfused();
				if (m_FailFindShelfAttemptCount > UnityEngine.Random.Range(1, 5))
				{
					StartCoroutine(DelayExitShop(2f));
				}
				else
				{
					DetermineShopAction();
				}
				m_FailFindShelfAttemptCount++;
				return;
			}
			Vector3 forward5 = m_CurrentBulkDonationBox.transform.position - base.transform.position;
			forward5.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward5, Vector3.up);
			if (m_CurrentBulkDonationBox.IsValidObject() && m_CurrentBulkDonationBox.GetTotalCardAmount() > 0 && !m_CurrentBulkDonationBox.IsEditingBulkBox())
			{
				SetState(ECustomerState.TakingItemFromBulkDonationBox);
				return;
			}
			CustomerConfused();
			SetState(ECustomerState.Idle);
		}
		else if (m_CurrentState == ECustomerState.WalkToPlayTable)
		{
			if (!m_CurrentPlayTable || ((bool)m_CurrentPlayTable && !m_CurrentPlayTable.IsValidObject()))
			{
				CustomerConfused();
				if (m_FailFindShelfAttemptCount > UnityEngine.Random.Range(1, 5))
				{
					StartCoroutine(DelayExitShop(2f));
				}
				else
				{
					AttemptFindPlayTable();
				}
				m_FailFindShelfAttemptCount++;
				return;
			}
			Vector3 forward6 = m_CurrentPlayTable.transform.position - base.transform.position;
			forward6.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward6, Vector3.up);
			m_CurrentPlayTableFee = PriceChangeManager.GetGameEventPrice(CPlayerData.m_GameEventFormat);
			float num2 = Mathf.Lerp(0.1f, 2f, CustomerReviewManager.GetAverageRating() * CustomerReviewManager.GetAverageRating() / 25f);
			int customerBuyItemChance = CSingleton<CustomerManager>.Instance.GetCustomerBuyItemChance(m_CurrentPlayTableFee / num2, PriceChangeManager.GetGameEventMarketPrice(CPlayerData.m_GameEventFormat));
			if (UnityEngine.Random.Range(0, 90) >= customerBuyItemChance)
			{
				List<string> list4 = new List<string>();
				list4.Add(LocalizationManager.GetTranslation("Too expensive to play..."));
				list4.Add(LocalizationManager.GetTranslation("Fee is too high here!"));
				list4.Add(LocalizationManager.GetTranslation("I will play somewhere else!"));
				AddReviewData(ECustomerReviewType.PlaytablePrice, EItemType.None, 1, -50);
				PopupText(list4, 100);
				m_IsAngry = true;
				m_FailFindItemAttemptCount++;
				SetState(ECustomerState.Idle);
			}
			else
			{
				if (SitDownAndStartPlay())
				{
					return;
				}
				if (UnityEngine.Random.Range(0, 100) < 30)
				{
					if (!m_CurrentPlayTable.IsQueueEmpty(m_CurrentPlayTableSeatIndex))
					{
						AttemptFindPlayTable();
						return;
					}
					m_CurrentPlayTable.CustomerBookQueueIndex(m_CurrentPlayTableSeatIndex);
					SetState(ECustomerState.QueueingPlayTable);
					m_TimerMax = UnityEngine.Random.Range(5, 30);
					m_TargetTransform = null;
					return;
				}
				if ((bool)m_CurrentPlayTable && m_CurrentPlayTableSeatIndex != -1)
				{
					m_CurrentPlayTable.CustomerUnbookSeatIndex(m_CurrentPlayTableSeatIndex);
				}
				m_CurrentPlayTable = null;
				m_CurrentPlayTableSeatIndex = -1;
				CustomerConfused();
				m_FailFindItemAttemptCount++;
				AttemptFindPlayTable();
			}
		}
		else if (m_CurrentState == ECustomerState.QueueingPlayTable)
		{
			if (!m_CurrentPlayTable || ((bool)m_CurrentPlayTable && !m_CurrentPlayTable.IsValidObject()))
			{
				CustomerConfused();
				if (m_FailFindShelfAttemptCount > UnityEngine.Random.Range(1, 5))
				{
					StartCoroutine(DelayExitShop(2f));
				}
				else
				{
					AttemptFindPlayTable();
				}
				m_FailFindShelfAttemptCount++;
				return;
			}
			if (!CPlayerData.m_IsShopOpen || LightManager.GetHasDayEnded())
			{
				m_FailFindItemAttemptCount = 5;
				GoShopNotOpenState();
				return;
			}
			bool flag = SitDownAndStartPlay();
			if (flag && m_TimerMax > 0f)
			{
				m_TimerMax = 0f;
				m_CurrentPlayTable.CustomerUnbookQueueIndex(m_CurrentPlayTableSeatIndex);
				m_CurrentPlayTable.CustomerUnbookSeatIndex(m_CurrentPlayTableSeatIndex);
			}
			else if (!flag && m_TimerMax > 0f)
			{
				if (UnityEngine.Random.Range(0, 100) >= 50)
				{
					AttemptFindPlayTable();
				}
				else if (ShelfManager.HasPlayTableWithPlayerWaiting())
				{
					AttemptFindPlayTable();
				}
			}
			else if (!flag && m_TimerMax <= 0f)
			{
				if ((bool)m_CurrentPlayTable && m_CurrentPlayTableSeatIndex != -1)
				{
					m_CurrentPlayTable.CustomerUnbookQueueIndex(m_CurrentPlayTableSeatIndex);
					m_CurrentPlayTable.CustomerUnbookSeatIndex(m_CurrentPlayTableSeatIndex);
				}
				m_CurrentPlayTable = null;
				m_CurrentPlayTableSeatIndex = -1;
				CustomerConfused();
				m_FailFindItemAttemptCount++;
				SetState(ECustomerState.Idle);
			}
		}
		else if (m_CurrentState == ECustomerState.QueuingToPay)
		{
			m_TimerMax = UnityEngine.Random.Range(1f, 3f);
		}
		else if (m_CurrentState == ECustomerState.WantToTradeCard)
		{
			if ((bool)m_CurrentTradeCardCashierCounter && m_CurrentTradeCardCashierCounter.IsValidObject() && m_CurrentTradeCardCashierCounter.CanTradeCard())
			{
				m_ExclaimationMesh.SetActive(value: true);
				m_InteractCollider.SetActive(value: true);
				SetState(ECustomerState.WaitingToTradeCard);
			}
			else
			{
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == ECustomerState.ExitingShop)
		{
			DeactivateCustomer();
		}
		else if ((m_CurrentState == ECustomerState.WantToBuyItem || m_CurrentState == ECustomerState.WantToPay || m_CurrentState == ECustomerState.WantToBuyCard || m_CurrentState == ECustomerState.WantToPlayGame || m_CurrentState == ECustomerState.WantToTakeBulkDonationCard) && m_UnableToFindQueue)
		{
			CustomerConfused();
		}
	}

	private void CheckShorterQueueCashierCounter()
	{
		if ((bool)m_CurrentQueueCashierCounter && ShelfManager.GetCashierCounterCount() > 1 && m_CurrentQueueCashierCounter.IsCustomerLastInLine(this) && m_CurrentQueueCashierCounter.GetCurrentQueingCustomerCount() >= 2 && ShelfManager.GetCashierCounter() != m_CurrentQueueCashierCounter)
		{
			m_CurrentQueueCashierCounter.RemoveCustomerFromQueue(this);
			m_CurrentQueueCashierCounter.RemoveLastCustomerFromQueue();
			ThinkWantToPay();
		}
	}

	private bool SitDownAndStartPlay()
	{
		m_IsInsideShop = true;
		if ((bool)m_CurrentPlayTable && m_CurrentPlayTable.IsSeatEmpty(m_CurrentPlayTableSeatIndex) && m_CurrentPlayTable.IsValidObject())
		{
			SetState(ECustomerState.PlayingAtTable);
			m_TargetTransform = m_CurrentPlayTable.GetSitLoc(m_CurrentPlayTableSeatIndex);
			m_Timer = 0f;
			m_SecondaryTimer = 0f;
			m_SecondaryTimerMax = UnityEngine.Random.Range(5, 30);
			m_LerpStartPos = base.transform.position;
			m_TargetLerpPos = m_TargetTransform.position;
			m_TargetLerpRotation = m_TargetTransform.rotation;
			m_CurrentPlayTable.CustomerHasReached(this, m_CurrentPlayTableSeatIndex);
			if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0 || m_HasCheckedOut)
			{
				m_ShoppingBagTransform.gameObject.SetActive(value: false);
				m_Anim.SetBool("HoldingBag", value: false);
			}
			m_Anim.SetBool("IsSitting", value: true);
			AddReviewData(ECustomerReviewType.PlaytablePrice, EItemType.None, 1, 30);
			if (!LightManager.IsLightSufficientToSee())
			{
				AddReviewData(ECustomerReviewType.LightTurnedOff, EItemType.None, UnityEngine.Random.Range(0, 2), -20);
			}
			return true;
		}
		if ((bool)m_CurrentPlayTable && !m_CurrentPlayTable.IsSeatEmpty(m_CurrentPlayTableSeatIndex) && m_CurrentPlayTable.IsValidObject() && m_CurrentPlayTable.GetEmptySeatIndex() != -1)
		{
			m_CurrentPlayTableSeatIndex = m_CurrentPlayTable.GetEmptySeatIndex();
			m_TargetTransform = m_CurrentPlayTable.GetStandLoc(m_CurrentPlayTableSeatIndex);
			if ((bool)m_TargetTransform)
			{
				m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
				m_IsWaitingForPathCallback = true;
				m_UnableToFindQueue = false;
				SetState(ECustomerState.WalkToPlayTable);
				return true;
			}
		}
		return false;
	}

	public void SetOutOfScreen()
	{
		base.transform.position = Vector3.one * 10000f;
	}

	private void WaypointEndUpdate()
	{
		if (m_CurrentState != ECustomerState.QueuingToPay || m_IsAtPayingPosition || m_IsWaitingForPathCallback)
		{
			return;
		}
		if (!m_TargetTransform)
		{
			SetState(ECustomerState.Idle);
			return;
		}
		if ((m_TargetTransform.position - base.transform.position).magnitude < 0.1f + m_CounterQueueDistanceReducer)
		{
			m_IsAtPayingPosition = m_CurrentQueueCashierCounter.IsCustomerAtPayingPosition(m_TargetTransform.position);
		}
		if (!m_IsAtPayingPosition && m_CurrentQueueCashierCounter.IsCustomerNextInLine(this))
		{
			m_CounterQueueDistanceReducer += Time.deltaTime * 0.1f;
			if (m_CounterQueueDistanceReducer > 0.4f)
			{
				m_Timer = 0f;
				m_CounterQueueDistanceReducer = 0f;
				m_CurrentQueueCashierCounter.RemoveCustomerFromQueue(this);
				m_CurrentQueueCashierCounter.RemoveCurrentCustomerFromQueue();
				ThinkWantToPay();
				return;
			}
		}
		else if (!m_IsAtPayingPosition && m_TimerMax != 0f)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_TimerMax)
			{
				m_Timer = 0f;
				m_TimerMax = 0f;
				CheckShorterQueueCashierCounter();
			}
		}
		if (m_IsAtPayingPosition)
		{
			m_Timer = 0f;
			m_TimerMax = 0f;
			m_CounterQueueDistanceReducer = 0f;
			Vector3 forward = m_CurrentQueueCashierCounter.transform.position - base.transform.position;
			forward.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward, Vector3.up);
			SetState(ECustomerState.ReadyToPay);
			m_ShoppingBagTransform.gameObject.SetActive(value: false);
			m_Anim.SetBool("HoldingBag", value: false);
			m_CurrentQueueCashierCounter.SetPlsaticBagVisibility(isShow: true);
			m_CurrentQueueCashierCounter.UpdateCashierCounterState(ECashierCounterState.ScanningItem);
			m_CurrentQueueCashierCounter.UpdateCurrentCustomer(this);
			for (int i = 0; i < m_ItemInBagList.Count; i++)
			{
				m_ItemInBagList[i].transform.parent = m_CurrentQueueCashierCounter.transform;
				m_ItemInBagList[i].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				int num = i % 8;
				int num2 = Mathf.Clamp(i / 8, 0, 1);
				int num3 = Mathf.Clamp(i / 16, 0, 2);
				m_ItemInBagList[i].transform.position += m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.forward * (-0.025f * (float)num);
				m_ItemInBagList[i].transform.position += m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.right * (0.1f * (float)num2);
				m_ItemInBagList[i].transform.position += Vector3.up * (0.2f * (float)num3);
				m_ItemInBagList[i].transform.rotation = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.rotation;
				m_ItemInBagList[i].transform.Rotate(new Vector3(UnityEngine.Random.Range(-30, -5), UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5)));
				m_ItemInBagList[i].m_Mesh.enabled = true;
				m_ItemInBagList[i].gameObject.SetActive(value: true);
				m_ItemInBagList[i].m_Collider.enabled = true;
				m_ItemInBagList[i].m_Rigidbody.isKinematic = false;
				m_ItemInBagList[i].m_InteractableScanItem.enabled = true;
				m_ItemInBagList[i].m_InteractableScanItem.RegisterScanItem(this, m_CurrentQueueCashierCounter.m_ScannedItemLerpPos);
			}
			for (int j = 0; j < m_CardInBagList.Count; j++)
			{
				m_CardInBagList[j].transform.parent = m_CurrentQueueCashierCounter.transform;
				m_CardInBagList[j].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				m_CardInBagList[j].transform.localScale = Vector3.one;
				int num4 = j % 8;
				int num5 = Mathf.Clamp(j / 8, 0, 1);
				int num6 = Mathf.Clamp(j / 16, 0, 2);
				m_CardInBagList[j].transform.position += m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.forward * (-0.035f * (float)num4);
				m_CardInBagList[j].transform.position += m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.right * (0.1f * (float)num5);
				m_CardInBagList[j].transform.position += Vector3.up * (0.3f * (float)num6);
				m_CardInBagList[j].transform.rotation = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.rotation;
				m_CardInBagList[j].transform.Rotate(new Vector3(UnityEngine.Random.Range(-30, -5) + 180, UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5)));
				m_CardInBagList[j].m_Card3dUI.gameObject.SetActive(value: true);
				m_CardInBagList[j].gameObject.SetActive(value: true);
				m_CardInBagList[j].m_Collider.enabled = true;
				m_CardInBagList[j].m_Rigidbody.isKinematic = false;
				m_CardInBagList[j].RegisterScanCard(this, m_CurrentQueueCashierCounter.m_ScannedItemLerpPos);
			}
			m_IsCheckScanItemOutOfBound = true;
			m_CheckScanItemOutOfBoundTimer = 2f;
		}
	}

	private void CheckItemOutOfCashierBound()
	{
		float num = 0.2f;
		float num2 = 0.9f;
		float num3 = 0.33f;
		for (int i = 0; i < m_ItemInBagList.Count; i++)
		{
			if (m_ItemInBagList[i].m_InteractableScanItem.IsNotScanned())
			{
				if (m_ItemInBagList[i].transform.position.y < num2)
				{
					m_ItemInBagList[i].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				}
				else if (Mathf.Abs(m_ItemInBagList[i].transform.localPosition.z) > num3)
				{
					m_ItemInBagList[i].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				}
				else if (Mathf.Abs(m_ItemInBagList[i].transform.localPosition.x) > num)
				{
					m_ItemInBagList[i].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				}
				m_ItemInBagList[i].gameObject.SetActive(value: true);
				m_ItemInBagList[i].m_Mesh.enabled = true;
			}
		}
		for (int j = 0; j < m_CardInBagList.Count; j++)
		{
			if (m_CardInBagList[j].IsNotScanned())
			{
				if (m_CardInBagList[j].transform.position.y < num2)
				{
					m_CardInBagList[j].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				}
				else if (Mathf.Abs(m_CardInBagList[j].transform.localPosition.z) > num3)
				{
					m_CardInBagList[j].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				}
				else if (Mathf.Abs(m_CardInBagList[j].transform.localPosition.x) > num)
				{
					m_CardInBagList[j].transform.position = m_CurrentQueueCashierCounter.m_CustomerPlaceItemPos.position;
				}
				m_CardInBagList[j].m_Card3dUI.m_IgnoreCulling = true;
				m_CardInBagList[j].m_Card3dUI.m_CardUIAnimGrp.gameObject.SetActive(value: true);
			}
		}
	}

	public void OnItemScanned(Item item)
	{
		float num = item.GetCurrentPrice();
		m_ItemScannedCount++;
		if (num <= 0f)
		{
			num = CPlayerData.GetItemMarketPrice(item.GetItemType());
		}
		m_CurrentQueueCashierCounter.AddScannedItemCostTotal(num, item.GetItemType());
		m_TotalScannedItemCost += num;
		double num2 = Math.Round(m_TotalScannedItemCost, 2, MidpointRounding.AwayFromZero);
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			num2 = Math.Round(m_TotalScannedItemCost, 3, MidpointRounding.AwayFromZero);
		}
		m_TotalScannedItemCost = (float)num2;
		EvaluateFinishScanItem();
	}

	public void OnCardScanned(InteractableCard3d card)
	{
		float num = card.GetCurrentPrice();
		m_ItemScannedCount++;
		if (num <= 0f)
		{
			num = CPlayerData.GetCardMarketPrice(card.m_Card3dUI.m_CardUI.GetCardData());
		}
		m_CurrentQueueCashierCounter.AddScannedCardCostTotal(num, card.m_Card3dUI.m_CardUI.GetCardData());
		m_TotalScannedItemCost += num;
		double num2 = Math.Round(m_TotalScannedItemCost, 2, MidpointRounding.AwayFromZero);
		if (GameInstance.GetCurrencyConversionRate() > 1f)
		{
			num2 = Math.Round(m_TotalScannedItemCost, 3, MidpointRounding.AwayFromZero);
		}
		m_TotalScannedItemCost = (float)num2;
		EvaluateFinishScanItem();
	}

	private void EvaluateFinishScanItem()
	{
		if (m_ItemScannedCount >= m_ItemInBagList.Count + m_CardInBagList.Count)
		{
			bool flag = false;
			if (UnityEngine.Random.Range(0, 100) < 30 && m_TotalScannedItemCost >= 5f)
			{
				flag = true;
			}
			if (m_TotalScannedItemCost >= 500f)
			{
				flag = true;
			}
			m_CustomerCash.SetIsCard(flag);
			m_CustomerCash.gameObject.SetActive(value: true);
			m_Anim.SetBool("HandingOverCash", value: true);
			m_CurrentQueueCashierCounter.SetCustomerPaidAmount(flag, GetRandomPayAmount(m_TotalScannedItemCost));
			m_CurrentQueueCashierCounter.UpdateCashierCounterState(ECashierCounterState.TakingCash);
			m_IsCheckScanItemOutOfBound = false;
		}
	}

	private double GetRandomPayAmount(float limit)
	{
		double num = 0.0;
		List<int> list = new List<int>();
		list.Add(1);
		list.Add(5);
		list.Add(10);
		list.Add(20);
		list.Add(50);
		list.Add(100);
		list.Add(5);
		list.Add(10);
		list.Add(20);
		list.Add(5);
		list.Add(10);
		list.Add(-1);
		list.Add(5);
		list.Add(10);
		list.Add(-1);
		list.Add(5);
		list.Add(10);
		list.Add(-2);
		list.Add(1);
		list.Add(5);
		list.Add(1);
		list.Add(20);
		list.Add(50);
		list.Add(1);
		list.Add(1);
		list.Add(5);
		list.Add(10);
		list.Add(20);
		list.Add(50);
		list.Add(100);
		while (num < (double)limit)
		{
			float num2 = list[UnityEngine.Random.Range(0, list.Count)];
			num = ((num2 != -1f) ? ((num2 != -2f) ? (num + (double)num2) : ((double)m_TotalScannedItemCost)) : ((double)(Mathf.CeilToInt(m_TotalScannedItemCost / 5f) * 5)));
		}
		if (UnityEngine.Random.Range(0, 100) < CSingleton<CustomerManager>.Instance.GetCustomerExactChangeChance())
		{
			num = m_TotalScannedItemCost;
		}
		if (Mathf.RoundToInt((float)num * 100f) == Mathf.RoundToInt(m_TotalScannedItemCost * 100f))
		{
			CSingleton<CustomerManager>.Instance.ResetCustomerExactChangeChance();
		}
		else
		{
			CSingleton<CustomerManager>.Instance.AddCustomerExactChangeChance();
			if (num - (double)limit >= 50.0)
			{
				num = (float)Mathf.CeilToInt(limit / 50f) * 50f;
			}
			if (num > (double)limit)
			{
				if (num >= 100.0)
				{
					num = (float)Mathf.CeilToInt(limit / 50f) * 50f;
				}
				else if (num >= 50.0)
				{
					num = (float)Mathf.CeilToInt(limit / 20f) * 20f;
				}
				else if (num >= 20.0)
				{
					num = (float)Mathf.CeilToInt(limit / 10f) * 10f;
				}
				else if (num >= 10.0)
				{
					num = (float)Mathf.CeilToInt(limit / 5f) * 5f;
				}
				else if (num >= 2.0)
				{
					num = (float)Mathf.CeilToInt(limit / 5f) * 5f;
				}
				else if (num < 1.0)
				{
					num = UnityEngine.Random.Range(0, 5) switch
					{
						0 => 10.0, 
						1 => 5.0, 
						_ => 1.0, 
					};
				}
				if (UnityEngine.Random.Range(0, 100) < 10)
				{
					int num3 = Mathf.FloorToInt((float)num);
					float num4 = (float)num - (float)num3;
					num += (double)num4;
				}
			}
		}
		num = (float)Mathf.RoundToInt((float)num * 100f) / 100f;
		if (num < (double)m_TotalScannedItemCost)
		{
			num = m_TotalScannedItemCost;
		}
		return num;
	}

	public void OnCashTaken(bool isCard)
	{
		m_CustomerCash.gameObject.SetActive(value: false);
		m_Anim.SetBool("HandingOverCash", value: false);
		m_CurrentQueueCashierCounter.StartGivingChange();
		m_CurrentQueueCashierCounter.UpdateCashierCounterState(ECashierCounterState.GivingChange);
	}

	public void CounterGiveChangeCompleted(int coinAmount)
	{
		OnPayingDone();
		if (coinAmount > 10)
		{
			AddReviewData(ECustomerReviewType.GiveManyChangePennies, EItemType.None, 1, -2 * coinAmount);
		}
		if (coinAmount > 25)
		{
			AddReviewData(ECustomerReviewType.GiveManyChangePennies, EItemType.None, 1, -2 * coinAmount, addAsNew: true);
		}
		if (coinAmount > 60)
		{
			AddReviewData(ECustomerReviewType.GiveManyChangePennies, EItemType.None, 0, -2 * coinAmount, addAsNew: true);
		}
		if (coinAmount >= 100)
		{
			AddReviewData(ECustomerReviewType.GiveManyChangePennies, EItemType.None, 0, -2 * coinAmount, addAsNew: true);
			AddReviewData(ECustomerReviewType.GiveManyChangePennies, EItemType.None, 0, -2 * coinAmount, addAsNew: true);
			AddReviewData(ECustomerReviewType.GiveManyChangePennies, EItemType.None, 0, -2 * coinAmount, addAsNew: true);
		}
	}

	private void OnPayingDone()
	{
		m_IsAtPayingPosition = false;
		m_HasCheckedOut = true;
		m_Path = null;
		if (m_ItemInBagList.Count + m_CardInBagList.Count > 0)
		{
			m_ShoppingBagTransform.gameObject.SetActive(value: true);
		}
		if (!LightManager.IsLightSufficientToSee())
		{
			AddReviewData(ECustomerReviewType.LightTurnedOff, EItemType.None, UnityEngine.Random.Range(0, 2), -20);
		}
		m_Anim.SetBool("HoldingBag", value: true);
		m_CurrentQueueCashierCounter.SetPlsaticBagVisibility(isShow: false);
		m_CurrentQueueCashierCounter.UpdateCashierCounterState(ECashierCounterState.Idle);
		m_CurrentQueueCashierCounter.UpdateCurrentCustomer(null);
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < m_ItemInBagList.Count; i++)
		{
			m_ItemInBagList[i].transform.parent = m_ShoppingBagTransform;
			m_ItemInBagList[i].transform.position = m_ShoppingBagTransform.position;
			m_ItemInBagList[i].transform.rotation = m_ShoppingBagTransform.rotation;
			m_ItemInBagList[i].gameObject.SetActive(value: false);
			m_ItemInBagList[i].m_Collider.enabled = false;
			m_ItemInBagList[i].m_Rigidbody.isKinematic = true;
			m_ItemInBagList[i].m_InteractableScanItem.enabled = false;
			num += m_ItemInBagList[i].GetItemVolume();
			num2 += InventoryBase.GetUnlockItemLevelRequired(m_ItemInBagList[i].GetItemType());
		}
		for (int j = 0; j < m_CardInBagList.Count; j++)
		{
			m_CardInBagList[j].transform.parent = m_ShoppingBagTransform;
			m_CardInBagList[j].transform.position = m_ShoppingBagTransform.position;
			m_CardInBagList[j].transform.rotation = m_ShoppingBagTransform.rotation;
			m_CardInBagList[j].m_Card3dUI.gameObject.SetActive(value: false);
			m_CardInBagList[j].gameObject.SetActive(value: false);
			m_CardInBagList[j].m_Collider.enabled = false;
			m_CardInBagList[j].m_Rigidbody.isKinematic = true;
		}
		StartCoroutine(DelayRemoveCustomerFromQueue(UnityEngine.Random.Range(0.25f, 1f)));
		DetermineShopAction();
		CEventManager.QueueEvent(new CEventPlayer_AddShopExp(m_ItemInBagList.Count * 4 + Mathf.RoundToInt(num) + num2 / 2 + m_CardInBagList.Count * 10));
	}

	public void AddPlayerOpenPackNearby()
	{
		m_PlayerOpenPackNearbyCount++;
		if (m_PlayerOpenPackNearbyCount > 3)
		{
			AddReviewData(ECustomerReviewType.OwnerOpenPack, EItemType.None, 1, -15 * m_PlayerOpenPackNearbyCount);
		}
	}

	private void ExitShop()
	{
		m_TargetTransform = CustomerManager.GetRandomExitPoint();
		m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
		m_IsWaitingForPathCallback = true;
		SetState(ECustomerState.ExitingShop);
		bool flag = true;
		if (m_HasUpdatedCustomerCount)
		{
			flag = false;
		}
		if (!m_HasUpdatedCustomerCount)
		{
			m_HasUpdatedCustomerCount = true;
			CSingleton<CustomerManager>.Instance.UpdateCustomerCount(-1);
		}
		if ((bool)m_CurrentTradeCardCashierCounter)
		{
			m_CurrentTradeCardCashierCounter.CustomerFinishTradingCard();
			m_CurrentTradeCardCashierCounter = null;
		}
		if (!CPlayerData.m_IsShopOnceOpen || !flag)
		{
			return;
		}
		if (m_ItemInBagList.Count + m_CardInBagList.Count <= 0 && !m_HasPlayedGame)
		{
			CPlayerData.m_GameReportDataCollect.customerDisatisfied++;
			CPlayerData.m_GameReportDataCollectPermanent.customerDisatisfied++;
		}
		if (m_ItemInBagList.Count > 0)
		{
			CPlayerData.m_GameReportDataCollect.customerBoughtItem++;
			CPlayerData.m_GameReportDataCollectPermanent.customerBoughtItem++;
			CPlayerData.m_GameReportDataCollect.itemAmountSold += m_ItemInBagList.Count;
			CPlayerData.m_GameReportDataCollectPermanent.itemAmountSold += m_ItemInBagList.Count;
			for (int i = 0; i < m_ItemInBagList.Count; i++)
			{
				CPlayerData.m_GameReportDataCollect.totalItemEarning += m_ItemInBagList[i].GetCurrentPrice();
				CPlayerData.m_GameReportDataCollectPermanent.totalItemEarning += m_ItemInBagList[i].GetCurrentPrice();
			}
		}
		if (m_CardInBagList.Count > 0)
		{
			CPlayerData.m_GameReportDataCollect.customerBoughtCard++;
			CPlayerData.m_GameReportDataCollectPermanent.customerBoughtCard++;
			CPlayerData.m_GameReportDataCollect.cardAmountSold += m_CardInBagList.Count;
			CPlayerData.m_GameReportDataCollectPermanent.cardAmountSold += m_CardInBagList.Count;
			for (int j = 0; j < m_CardInBagList.Count; j++)
			{
				CPlayerData.m_GameReportDataCollect.totalCardEarning += m_CardInBagList[j].GetCurrentPrice();
				CPlayerData.m_GameReportDataCollectPermanent.totalCardEarning += m_CardInBagList[j].GetCurrentPrice();
				AchievementManager.OnSoldCardPrice(m_CardInBagList[j].GetCurrentPrice());
				AchievementManager.OnSoldGradedCard100K(m_CardInBagList[j].GetCurrentPrice(), m_CardInBagList[j].m_Card3dUI.m_CardUI.GetCardData().cardGrade);
			}
			TutorialManager.AddTaskValue(ETutorialTaskCondition.SellCard, m_CardInBagList.Count);
		}
		if (m_ItemInBagList.Count + m_CardInBagList.Count > 0)
		{
			CPlayerData.m_GameReportDataCollect.checkoutCount++;
			CPlayerData.m_GameReportDataCollectPermanent.checkoutCount++;
			if (UnityEngine.Random.Range(0, 100) < 75)
			{
				if (CSingleton<CustomerManager>.Instance.GetSmellyCustomerInsideShopCount() == 0)
				{
					AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 1, 30);
					AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 1, 30);
					AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 1, 30);
				}
				else if (CSingleton<CustomerManager>.Instance.GetSmellyCustomerInsideShopCount() == 1)
				{
					AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 1, -10);
					AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 1, -10);
				}
				else if (CSingleton<CustomerManager>.Instance.GetSmellyCustomerInsideShopCount() == 2)
				{
					AddReviewData(ECustomerReviewType.SmellyCustomer, EItemType.None, 1, -30);
				}
			}
		}
		for (int num = m_CustomerReviewGatherDataList.Count - 1; num >= 0; num--)
		{
			if (m_CustomerReviewGatherDataList[num].goodBadLevel < 2 && m_CustomerReviewGatherDataList[num].itemType != EItemType.None && !m_TargetBuyItemList.Contains(m_CustomerReviewGatherDataList[num].itemType))
			{
				m_CustomerReviewGatherDataList.RemoveAt(num);
			}
		}
		for (int num2 = m_CustomerReviewGatherDataList.Count - 1; num2 >= 0; num2--)
		{
			if (m_CustomerReviewGatherDataList[num2].goodBadLevel < 2 && m_CustomerReviewGatherDataList[num2].itemType != EItemType.None)
			{
				List<int> restockDataIndexList = InventoryBase.GetRestockDataIndexList(m_CustomerReviewGatherDataList[num2].itemType);
				if (restockDataIndexList.Count > 1)
				{
					if (!CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[0]) && !CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[1]))
					{
						m_CustomerReviewGatherDataList.RemoveAt(num2);
					}
				}
				else if (restockDataIndexList.Count > 0 && !CPlayerData.GetIsItemLicenseUnlocked(restockDataIndexList[0]))
				{
					m_CustomerReviewGatherDataList.RemoveAt(num2);
				}
			}
		}
		for (int num3 = m_CustomerReviewGatherDataList.Count - 1; num3 >= 0; num3--)
		{
			if (m_CustomerReviewGatherDataList[num3].goodBadLevel < 2 && m_CustomerReviewGatherDataList[num3].itemType != EItemType.None && (bool)ShelfManager.GetShelfWithItemType(m_CustomerReviewGatherDataList[num3].itemType))
			{
				m_CustomerReviewGatherDataList.RemoveAt(num3);
			}
		}
		if (LightManager.IsLightSufficientToSee())
		{
			for (int num4 = m_CustomerReviewGatherDataList.Count - 1; num4 >= 0; num4--)
			{
				if (m_CustomerReviewGatherDataList[num4].customerReviewType == ECustomerReviewType.LightTurnedOff)
				{
					m_CustomerReviewGatherDataList.RemoveAt(num4);
				}
			}
		}
		CustomerReviewManager.CustomerSendReviewList(m_CustomerReviewGatherDataList);
	}

	private void AddReviewData(ECustomerReviewType reviewType, EItemType itemType, int goodBadLevel, int higherStarChanceAdd, bool addAsNew = false)
	{
		if (goodBadLevel == 1)
		{
			goodBadLevel = 2;
		}
		if (CPlayerData.m_ShopLevel < 4 && reviewType == ECustomerReviewType.SmellyCustomer)
		{
			return;
		}
		if (!addAsNew)
		{
			bool flag = false;
			for (int i = 0; i < m_CustomerReviewGatherDataList.Count; i++)
			{
				if (m_CustomerReviewGatherDataList[i].customerReviewType == reviewType)
				{
					m_CustomerReviewGatherDataList[i].itemType = itemType;
					m_CustomerReviewGatherDataList[i].higherStarChanceAdd += higherStarChanceAdd;
					if (m_CustomerReviewGatherDataList[i].higherStarChanceAdd < -50)
					{
						m_CustomerReviewGatherDataList[i].higherStarChanceAdd = 0;
						m_CustomerReviewGatherDataList[i].goodBadLevel--;
					}
					else if (m_CustomerReviewGatherDataList[i].higherStarChanceAdd > 50)
					{
						m_CustomerReviewGatherDataList[i].higherStarChanceAdd = 0;
						m_CustomerReviewGatherDataList[i].goodBadLevel++;
					}
					if (m_CustomerReviewGatherDataList[i].goodBadLevel < 0)
					{
						m_CustomerReviewGatherDataList[i].goodBadLevel = 0;
					}
					else if (m_CustomerReviewGatherDataList[i].goodBadLevel > 2)
					{
						m_CustomerReviewGatherDataList[i].goodBadLevel = 2;
					}
					flag = true;
				}
			}
			if (flag)
			{
				return;
			}
		}
		CustomerReviewGatherData customerReviewGatherData = new CustomerReviewGatherData();
		customerReviewGatherData.customerReviewType = reviewType;
		customerReviewGatherData.itemType = itemType;
		customerReviewGatherData.goodBadLevel = goodBadLevel;
		customerReviewGatherData.higherStarChanceAdd = higherStarChanceAdd;
		m_CustomerReviewGatherDataList.Add(customerReviewGatherData);
	}

	private IEnumerator DelayExitShop(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		if (!m_HasCheckedOut && (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0))
		{
			ThinkWantToPay();
		}
		else
		{
			ExitShop();
		}
	}

	private IEnumerator DelayRemoveCustomerFromQueue(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		m_CurrentQueueCashierCounter.RemoveCustomerFromQueue(this);
		m_CurrentQueueCashierCounter.RemoveCurrentCustomerFromQueue();
	}

	public void OnCashierCounterQueueMoved(int index)
	{
		StartCoroutine(DelayQueueMoved(index));
	}

	private IEnumerator DelayQueueMoved(int index)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0.25f, 0.4f) * (float)index);
		m_ReachedEndOfPath = false;
		m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
		m_IsWaitingForPathCallback = true;
	}

	private void SetState(ECustomerState state)
	{
		m_CurrentState = state;
	}

	public void OnPathComplete(Path p)
	{
		p.Claim(this);
		if (!p.error)
		{
			if (m_Path != null)
			{
				m_Path.Release(this);
			}
			m_Path = p;
			m_CurrentWaypoint = 0;
		}
		else
		{
			p.Release(this);
		}
		m_IsWaitingForPathCallback = false;
		m_ReachedEndOfPath = false;
		if (p.error)
		{
			if (!m_IsInsideShop)
			{
				GoShopNotOpenState();
			}
			else if (m_CurrentState == ECustomerState.ExitingShop)
			{
				OnReachedPathEnd();
			}
			else
			{
				OnReachedPathEnd();
			}
		}
	}

	private void GoShopNotOpenState()
	{
		if (!m_HasCheckedOut && (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0))
		{
			ThinkWantToPay();
			return;
		}
		m_ReachedEndOfPath = false;
		m_TargetTransform = CustomerManager.GetRandomShopWindowOutsidePoint();
		m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
		m_IsWaitingForPathCallback = true;
		m_UnableToFindQueue = false;
		SetState(ECustomerState.ShopNotOpen);
	}

	public void InstantSnapToPlayTable(InteractablePlayTable playTable, int seatIndex, bool isGameStarted)
	{
		m_CurrentPlayTable = playTable;
		m_CurrentPlayTableSeatIndex = seatIndex;
		SetState(ECustomerState.PlayingAtTable);
		m_TargetTransform = m_CurrentPlayTable.GetSitLoc(m_CurrentPlayTableSeatIndex);
		m_LerpStartPos = m_TargetTransform.position;
		m_TargetLerpPos = m_TargetTransform.position;
		m_TargetLerpRotation = m_TargetTransform.rotation;
		base.transform.position = m_TargetLerpPos;
		base.transform.rotation = m_TargetLerpRotation;
		m_Timer = 0f;
		m_SecondaryTimer = 0f;
		m_SecondaryTimerMax = UnityEngine.Random.Range(5, 30);
		m_IsInsideShop = true;
		m_Anim.SetBool("IsSitting", value: true);
		if (isGameStarted)
		{
			PlayTableGameStarted();
		}
	}

	public void PlayTableGameStarted()
	{
		m_HasPlayedGame = true;
		m_Anim.SetBool("IsPlaying", value: true);
		m_GameCardFanOut.SetActive(value: true);
		m_GameCardSingle.SetActive(value: true);
	}

	public void PlayTableGameEnded(float totalPlayTime, float playTableFee)
	{
		m_HasPlayedGame = true;
		m_Anim.SetBool("IsPlaying", value: false);
		m_Anim.SetInteger("RandomPlayIndex", 0);
		m_GameCardFanOut.SetActive(value: false);
		m_GameCardSingle.SetActive(value: false);
		m_TargetTransform = m_CurrentPlayTable.GetStandLocB(m_CurrentPlayTableSeatIndex);
		if (m_TargetTransform == null)
		{
			m_TargetTransform = base.transform;
		}
		m_LerpStartPos = base.transform.position;
		m_TargetLerpPos = m_TargetTransform.position;
		m_TargetLerpRotation = m_TargetTransform.rotation;
		m_CurrentPlayTable = null;
		m_CurrentPlayTableSeatIndex = -1;
		m_Timer = 0f;
		m_TimerMax = UnityEngine.Random.Range(0.7f, 1.25f);
		SetState(ECustomerState.EndingPlayTableGame);
		float num = playTableFee * (totalPlayTime / 60f);
		if (num > 0f)
		{
			PriceChangeManager.AddTransaction(num, ETransactionType.PlayTableEarning, 0);
			CEventManager.QueueEvent(new CEventPlayer_AddCoin(num));
			CSingleton<PricePopupSpawner>.Instance.ShowPricePopup(num, 1.8f, base.transform);
		}
		if (!LightManager.IsLightSufficientToSee())
		{
			AddReviewData(ECustomerReviewType.LightTurnedOff, EItemType.None, UnityEngine.Random.Range(0, 2), -20);
		}
		if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
		{
			m_ShoppingBagTransform.gameObject.SetActive(value: true);
			m_Anim.SetBool("HoldingBag", value: true);
		}
		m_Anim.SetBool("IsSitting", value: false);
		if (totalPlayTime > 0f)
		{
			CPlayerData.m_GameReportDataCollect.customerPlayed++;
			CPlayerData.m_GameReportDataCollect.totalPlayTableTime += totalPlayTime;
			CPlayerData.m_GameReportDataCollect.totalPlayTableEarning += num;
			CPlayerData.m_GameReportDataCollectPermanent.customerPlayed++;
			CPlayerData.m_GameReportDataCollectPermanent.totalPlayTableTime += totalPlayTime;
			CPlayerData.m_GameReportDataCollectPermanent.totalPlayTableEarning += num;
			AchievementManager.OnCustomerFinishPlay(CPlayerData.m_GameReportDataCollectPermanent.customerPlayed);
			TutorialManager.AddTaskValue(ETutorialTaskCondition.CustomerPlay, 1f);
			float averageRating = CustomerReviewManager.GetAverageRating();
			CEventManager.QueueEvent(new CEventPlayer_AddShopExp(Mathf.RoundToInt(averageRating * averageRating * averageRating * averageRating * 0.05f * Mathf.Lerp(0f, 1f, (float)CPlayerData.m_ShopLevel / 80f) * 10f * (totalPlayTime / 60f))));
		}
	}

	public void Update()
	{
		if (!m_IsActive)
		{
			return;
		}
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, m_TargetLerpRotation, Time.fixedDeltaTime * m_RotationLerpSpeed);
		m_CurrentMoveSpeed = (m_LastFramePos - base.transform.position).magnitude * 50f;
		m_LastFramePos = base.transform.position;
		m_Anim.SetFloat("MoveSpeed", m_CurrentMoveSpeed);
		if (m_IsPausingAction)
		{
			return;
		}
		if (m_IsCheckScanItemOutOfBound)
		{
			m_CheckScanItemOutOfBoundTimer += Time.deltaTime;
			if (m_CheckScanItemOutOfBoundTimer > 3f)
			{
				m_CheckScanItemOutOfBoundTimer = 0f;
				CheckItemOutOfCashierBound();
			}
		}
		if (m_IsBeingSprayed)
		{
			m_BeingSprayedResetTimer += Time.deltaTime;
			if (m_BeingSprayedResetTimer > m_BeingSprayedResetTimeMax)
			{
				m_BeingSprayedResetTimer = 0f;
				m_IsBeingSprayed = false;
				m_Anim.SetBool("IsBeingSprayed", value: false);
			}
		}
		if (m_CurrentState == ECustomerState.Idle)
		{
			m_Timer += Time.deltaTime;
			if (!(m_Timer > 2f))
			{
				return;
			}
			m_Timer = 0f;
			if (CPlayerData.m_IsShopOpen)
			{
				DetermineShopAction();
			}
			else if (m_FailFindItemAttemptCount > UnityEngine.Random.Range(1, 5))
			{
				if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
				{
					ThinkWantToPay();
				}
				else
				{
					ExitShop();
				}
			}
			else
			{
				GoShopNotOpenState();
			}
			return;
		}
		if (m_CurrentState == ECustomerState.WantToBuyItem)
		{
			if (m_UnableToFindQueue)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 8f)
				{
					m_Timer = 0f;
					AttemptFindShelf();
				}
			}
		}
		else if (m_CurrentState == ECustomerState.WantToBuyCard)
		{
			if (m_UnableToFindQueue)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 8f)
				{
					m_Timer = 0f;
					AttemptFindCardShelf();
				}
			}
		}
		else if (m_CurrentState == ECustomerState.WantToTakeBulkDonationCard)
		{
			if (m_UnableToFindQueue)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 8f)
				{
					m_Timer = 0f;
					AttemptFindBulkDonationBox();
				}
			}
		}
		else if (m_CurrentState == ECustomerState.WantToPlayGame)
		{
			if (m_UnableToFindQueue)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 8f)
				{
					m_Timer = 0f;
					AttemptFindPlayTable();
				}
			}
		}
		else if (m_CurrentState == ECustomerState.WalkToShelf)
		{
			if (!m_IsInsideShop)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 1f)
				{
					m_Timer = 0f;
					if (!CPlayerData.m_IsShopOpen)
					{
						GoShopNotOpenState();
					}
					else if (LightManager.GetHasDayEnded())
					{
						m_FailFindItemAttemptCount = 5;
						GoShopNotOpenState();
					}
				}
			}
		}
		else if (m_CurrentState == ECustomerState.TakingItemFromShelf)
		{
			if (!m_HasTookItemFromShelf)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 1f)
				{
					m_HasTookItemFromShelf = true;
					m_Timer = 0f;
					TakeItemFromShelf();
				}
			}
			else
			{
				SetState(ECustomerState.Idle);
			}
		}
		else if (m_CurrentState == ECustomerState.TakingItemFromCardShelf)
		{
			if (!m_HasTookCardFromShelf)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 1f)
				{
					m_HasTookCardFromShelf = true;
					m_Timer = 0f;
					TakeCardFromShelf();
				}
			}
			else
			{
				SetState(ECustomerState.Idle);
			}
		}
		else if (m_CurrentState == ECustomerState.TakingItemFromBulkDonationBox)
		{
			if (!m_HasTookCardFromBulkDonationBox)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 1f)
				{
					m_HasTookCardFromBulkDonationBox = true;
					m_Timer = 0f;
					TakeCardFromBulkDonationBox();
				}
			}
			else
			{
				SetState(ECustomerState.Idle);
			}
		}
		else if (m_CurrentState == ECustomerState.WantToPay)
		{
			if (m_UnableToFindQueue)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > 8f)
				{
					m_Timer = 0f;
					AttemptFindQueue();
				}
			}
		}
		else if (m_CurrentState == ECustomerState.ReadyToPay)
		{
			if (!m_CurrentQueueCashierCounter)
			{
				ThinkWantToPay();
				return;
			}
			if (m_IsAtPayingPosition)
			{
				return;
			}
		}
		else if (m_CurrentState == ECustomerState.WalkToPlayTable)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 2f)
			{
				if (!m_IsInsideShop && !CPlayerData.m_IsShopOpen)
				{
					GoShopNotOpenState();
				}
				else if (!m_IsInsideShop && LightManager.GetHasDayEnded())
				{
					m_FailFindItemAttemptCount = 5;
					GoShopNotOpenState();
				}
				else if ((bool)m_CurrentPlayTable && !m_CurrentPlayTable.IsQueueEmpty(m_CurrentPlayTableSeatIndex))
				{
					AttemptFindPlayTable();
				}
			}
		}
		else
		{
			if (m_CurrentState == ECustomerState.PlayingAtTable)
			{
				m_Timer = Mathf.Clamp(m_Timer + Time.deltaTime, 0f, 1f);
				base.transform.position = Vector3.Lerp(m_LerpStartPos, m_TargetLerpPos, m_Timer * 3f);
				m_SecondaryTimer += Time.deltaTime;
				if (m_SecondaryTimer >= m_SecondaryTimerMax)
				{
					m_Anim.SetInteger("RandomPlayIndex", UnityEngine.Random.Range(0, 5));
					m_Anim.SetTrigger("PlayGameEmote");
					m_SecondaryTimerMax = UnityEngine.Random.Range(5, 30);
					m_SecondaryTimer = 0f;
				}
				return;
			}
			if (m_CurrentState == ECustomerState.QueueingPlayTable)
			{
				m_Timer += Time.deltaTime;
				bool flag = false;
				if (m_Timer > 2f)
				{
					m_Timer = 0f;
					m_TimerMax -= 2f;
					if (!LightManager.GetHasDayEnded())
					{
						OnReachedPathEnd();
					}
					else
					{
						flag = true;
					}
				}
				if (!(m_Timer > m_TimerMax || flag))
				{
					return;
				}
				m_CurrentPlayTable.CustomerUnbookQueueIndex(m_CurrentPlayTableSeatIndex);
				m_Timer = 0f;
				m_TimerMax = 0f;
				if (UnityEngine.Random.Range(0, 100) < 40)
				{
					if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
					{
						ThinkWantToPay();
					}
					else
					{
						ExitShop();
					}
				}
				else
				{
					SetState(ECustomerState.Idle);
					m_Timer = 1.5f;
				}
				return;
			}
			if (m_CurrentState == ECustomerState.EndingPlayTableGame)
			{
				m_Timer += Time.deltaTime;
				base.transform.position = Vector3.Lerp(m_LerpStartPos, m_TargetLerpPos, m_Timer * 3f);
				if (!(m_Timer > m_TimerMax))
				{
					return;
				}
				m_Timer = 0f;
				m_TimerMax = 0f;
				if (UnityEngine.Random.Range(0, 100) < 60)
				{
					if (m_ItemInBagList.Count > 0 || m_CardInBagList.Count > 0)
					{
						ThinkWantToPay();
					}
					else
					{
						ExitShop();
					}
				}
				else
				{
					SetState(ECustomerState.Idle);
					m_Timer = 2f;
				}
				return;
			}
			if (m_CurrentState == ECustomerState.WaitingToTradeCard)
			{
				if (!CSingleton<CustomerManager>.Instance.m_IsPlayerTrading)
				{
					m_Timer += Time.deltaTime;
				}
				if (m_Timer > 60f)
				{
					m_Timer = 0f;
					m_TimerMax = 0f;
					m_CustomerTradeData = null;
					m_IsPausingAction = false;
					m_ExclaimationMesh.SetActive(value: false);
					m_InteractCollider.SetActive(value: false);
					if ((bool)m_CurrentTradeCardCashierCounter)
					{
						m_HasTradedCard = true;
						m_CurrentTradeCardCashierCounter.CustomerFinishTradingCard();
						m_CurrentTradeCardCashierCounter = null;
					}
					DetermineShopAction();
				}
				return;
			}
		}
		if (!m_ReachedEndOfPath && Time.time > m_LastRepath + m_RepathRate && m_Seeker.IsDone() && (bool)m_TargetTransform)
		{
			m_LastRepath = Time.time;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
		}
		if (m_Path == null)
		{
			return;
		}
		while (Vector3.Distance(base.transform.position, m_Path.vectorPath[m_CurrentWaypoint]) < m_NextWaypointDistance)
		{
			if (!m_IsInsideShop && m_CurrentState != ECustomerState.ExitingShop)
			{
				m_IsInsideShop = CustomerManager.CheckIsInsideShop(base.transform.position);
				if (m_IsInsideShop)
				{
					OnCustomerReachInsideShop();
				}
			}
			else if (m_IsInsideShop && m_CurrentState == ECustomerState.ExitingShop && CustomerManager.CheckIsInsideShop(base.transform.position))
			{
				m_IsInsideShop = false;
				if (m_IsSmelly)
				{
					m_IsSmelly = false;
					CSingleton<CustomerManager>.Instance.RemoveFromSmellyCustomerList(this);
				}
			}
			if (m_CurrentWaypoint + 1 < m_Path.vectorPath.Count)
			{
				m_CurrentWaypoint++;
				continue;
			}
			if (!m_ReachedEndOfPath)
			{
				m_ReachedEndOfPath = true;
				OnReachedPathEnd();
			}
			WaypointEndUpdate();
			break;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, m_Path.vectorPath[m_CurrentWaypoint], m_ModifiedSpeed * m_ExtraSpeedMultiplier * Time.deltaTime);
		if (!m_ReachedEndOfPath)
		{
			Vector3 vector = m_Path.vectorPath[m_CurrentWaypoint] - base.transform.position;
			vector.y = 0f;
			if (vector != Vector3.zero)
			{
				m_TargetLerpRotation = Quaternion.LookRotation(vector, Vector3.up);
			}
		}
	}

	public List<Item> GetItemInBagList()
	{
		return m_ItemInBagList;
	}

	public List<InteractableCard3d> GetCardInBagList()
	{
		return m_CardInBagList;
	}

	public bool IsUnableToFindQueue()
	{
		if (!m_UnableToFindQueue)
		{
			return m_FailFindShelfAttemptCount > 0;
		}
		return true;
	}

	public bool IsActive()
	{
		return m_IsActive;
	}

	public bool IsInsideShop()
	{
		return m_IsInsideShop;
	}

	public bool IsSmelly()
	{
		return m_IsSmelly;
	}

	public void SetExtraSpeedMultiplier(float extraSpeedMultiplier)
	{
		m_ExtraSpeedMultiplier = extraSpeedMultiplier;
	}

	public void ResetExtraSpeedMultiplier()
	{
		m_ExtraSpeedMultiplier = 1f;
	}

	private bool PopupText(List<string> textList, int appearChance = 15)
	{
		int num = UnityEngine.Random.Range(0, 100);
		if (m_IsChattyCustomer)
		{
			num = 0;
		}
		if (num < appearChance)
		{
			CSingleton<PricePopupSpawner>.Instance.ShowTextPopup(textList[UnityEngine.Random.Range(0, textList.Count)], 1.8f, base.transform);
			return true;
		}
		return false;
	}

	private void CustomerConfused()
	{
		m_Anim.SetTrigger("ShakeHead");
	}

	public float GetCurrentPlayTableFee()
	{
		return m_CurrentPlayTableFee;
	}

	public bool HasCheckedOut()
	{
		return m_HasCheckedOut;
	}

	public CustomerSaveData GetCustomerSaveData()
	{
		CustomerSaveData customerSaveData = new CustomerSaveData();
		if (m_CurrentState == ECustomerState.WantToBuyItem || m_CurrentState == ECustomerState.WantToBuyItem || m_CurrentState == ECustomerState.WantToPay || m_CurrentState == ECustomerState.WantToPlayGame || m_CurrentState == ECustomerState.WantToBuyCard || m_CurrentState == ECustomerState.PlayingAtTable || m_CurrentState == ECustomerState.ExitingShop || m_CurrentState == ECustomerState.WantToTakeBulkDonationCard)
		{
			customerSaveData.currentState = m_CurrentState;
		}
		else if (m_CurrentState == ECustomerState.QueuingToPay || m_CurrentState == ECustomerState.ReadyToPay)
		{
			customerSaveData.currentState = ECustomerState.WantToPay;
		}
		else
		{
			customerSaveData.currentState = ECustomerState.Idle;
		}
		customerSaveData.hasCheckedOut = m_HasCheckedOut;
		customerSaveData.hasPlayedGame = m_HasPlayedGame;
		customerSaveData.hasTookItemFromShelf = m_HasTookItemFromShelf;
		customerSaveData.hasTookCardFromShelf = m_HasTookCardFromShelf;
		customerSaveData.isInsideShop = m_IsInsideShop;
		customerSaveData.isSmelly = m_IsSmelly;
		customerSaveData.smellyMeter = m_SmellyMeter;
		customerSaveData.pos.SetData(base.transform.position);
		customerSaveData.rot.SetData(base.transform.rotation);
		if (!m_HasCheckedOut)
		{
			List<EItemType> list = new List<EItemType>();
			List<float> list2 = new List<float>();
			for (int i = 0; i < m_ItemInBagList.Count; i++)
			{
				list.Add(m_ItemInBagList[i].GetItemType());
				list2.Add(m_ItemInBagList[i].GetCurrentPrice());
			}
			customerSaveData.itemInBagList = list;
			customerSaveData.itemInBagPriceList = list2;
			List<CardData> list3 = new List<CardData>();
			List<float> list4 = new List<float>();
			for (int j = 0; j < m_CardInBagList.Count; j++)
			{
				list3.Add(m_CardInBagList[j].m_Card3dUI.m_CardUI.GetCardData());
				list4.Add(m_CardInBagList[j].GetCurrentPrice());
			}
			customerSaveData.cardInBagList = list3;
			customerSaveData.cardInBagPriceList = list4;
		}
		customerSaveData.currentCostTotal = m_CurrentCostTotal;
		customerSaveData.maxMoney = m_MaxMoney;
		customerSaveData.totalScannedItemCost = m_TotalScannedItemCost;
		customerSaveData.currentPlayTableFee = m_CurrentPlayTableFee;
		customerSaveData.customerReviewGatherDataList = m_CustomerReviewGatherDataList;
		customerSaveData.hasUpdatedCustomerCount = m_HasUpdatedCustomerCount;
		return customerSaveData;
	}

	public void LoadCustomerSaveData(CustomerSaveData data)
	{
		m_Timer = 0f;
		m_CurrentState = data.currentState;
		m_HasCheckedOut = data.hasCheckedOut;
		m_HasPlayedGame = data.hasPlayedGame;
		m_HasTookCardFromShelf = data.hasTookCardFromShelf;
		m_IsInsideShop = data.isInsideShop;
		m_IsSmelly = data.isSmelly;
		m_SmellyMeter = data.smellyMeter;
		m_CurrentCostTotal = data.currentCostTotal;
		m_MaxMoney = data.maxMoney;
		m_TotalScannedItemCost = data.totalScannedItemCost;
		m_CurrentPlayTableFee = data.currentPlayTableFee;
		m_HasUpdatedCustomerCount = data.hasUpdatedCustomerCount;
		if (data.customerReviewGatherDataList != null)
		{
			m_CustomerReviewGatherDataList = data.customerReviewGatherDataList;
		}
		base.transform.position = data.pos.Data;
		base.transform.rotation = data.rot.Data;
		m_SmellyFX.SetActive(m_IsSmelly);
		if (m_IsSmelly)
		{
			CSingleton<CustomerManager>.Instance.AddToSmellyCustomerList(this);
		}
		else
		{
			CSingleton<CustomerManager>.Instance.RemoveFromSmellyCustomerList(this);
		}
		if (m_HasCheckedOut)
		{
			m_ShoppingBagTransform.gameObject.SetActive(value: true);
			return;
		}
		if (data.itemInBagList.Count + data.cardInBagList.Count > 0)
		{
			m_ShoppingBagTransform.gameObject.SetActive(value: true);
		}
		for (int i = 0; i < data.itemInBagList.Count; i++)
		{
			Item item = null;
			ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(data.itemInBagList[i]);
			item = ItemSpawnManager.GetItem(m_ShoppingBagTransform);
			item.SetMesh(itemMeshData.mesh, itemMeshData.material, data.itemInBagList[i], itemMeshData.meshSecondary, itemMeshData.materialSecondary, itemMeshData.materialList);
			item.SetCurrentPrice(data.itemInBagPriceList[i]);
			item.transform.position = m_ShoppingBagTransform.position;
			item.transform.rotation = m_ShoppingBagTransform.rotation;
			item.gameObject.SetActive(value: false);
			m_ItemInBagList.Add(item);
		}
		for (int j = 0; j < data.cardInBagList.Count; j++)
		{
			Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
			InteractableCard3d component = ShelfManager.SpawnInteractableObject(EObjectType.Card3d).GetComponent<InteractableCard3d>();
			cardUI.m_CardUI.SetCardUI(data.cardInBagList[j]);
			component.SetCardUIFollow(cardUI);
			component.SetEnableCollision(isEnable: false);
			component.SetCurrentPrice(data.cardInBagPriceList[j]);
			component.transform.position = m_ShoppingBagTransform.position;
			component.transform.rotation = m_ShoppingBagTransform.rotation;
			m_CardInBagList.Add(component);
			component.m_Card3dUI.gameObject.SetActive(value: false);
			component.gameObject.SetActive(value: false);
		}
		if (m_CurrentState == ECustomerState.WantToPay)
		{
			AttemptFindQueue();
		}
	}
}
