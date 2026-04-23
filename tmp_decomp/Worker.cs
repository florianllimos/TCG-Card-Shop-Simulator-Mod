using System.Collections;
using System.Collections.Generic;
using CC;
using Pathfinding;
using UnityEngine;

public class Worker : MonoBehaviour
{
	public CharacterCustomization m_CharacterCustom;

	public Animator m_Anim;

	public WorkerCollider m_WorkerCollider;

	public int m_WorkerIndex;

	public EWorkerTask m_PrimaryTask;

	public EWorkerTask m_SecondaryTask;

	public EWorkerTask m_WorkerTask;

	public EWorkerTask m_LastWorkerTask;

	public EWorkerTask m_LastWorkerSecondaryTask;

	public Transform m_PlayerLookAtTarget;

	public Transform m_HoldBoxLoc;

	public Transform m_WristBoneL;

	public Transform m_WristBoneR;

	public GameObject m_ExclaimationMesh;

	public float m_RestockTime = 1.5f;

	public float m_ScanItemTime = 1.5f;

	public float m_GiveChangeTime = 1.5f;

	public float m_RestockCardWaitTime = 1f;

	public float m_CurrentMoveSpeed;

	private Vector3 m_LastFramePos;

	public bool m_IsFemale;

	public bool m_IsActive;

	public bool m_IsPausingAction;

	public Seeker m_Seeker;

	public float m_Speed = 1f;

	public float m_NextWaypointDistance = 0.1f;

	public float m_RepathRate = 2f;

	public float m_RotationLerpSpeed = 2f;

	private Path m_Path;

	public EWorkerState m_CurrentState;

	public Shelf m_CurrentShelf;

	public WarehouseShelf m_CurrentWarehouseShelf;

	public ShelfCompartment m_CurrentItemCompartment;

	public CardShelf m_CurrentCardShelf;

	public InteractableCardCompartment m_CurrentCardCompartment;

	private InteractableCashierCounter m_CurrentCashierCounter;

	private InteractablePackagingBox_Item m_CurrentItemBox;

	private InteractablePackagingBox_Item m_CurrentHoldItemBox;

	private InteractablePackagingBox_Item m_LastStoredItemBox;

	private InteractableTrashBin m_CurrentTrashBin;

	private InteractableEmptyBoxStorage m_CurrentEmptyBoxStorage;

	private InteractableAutoCleanser m_TargetRefillCleanser;

	private InteractableAutoPackOpener m_TargetRefillCardOpener;

	public EItemType m_TargetBoxItemType = EItemType.None;

	public int m_TargetBoxSize;

	private WorkerData m_WorkerData;

	private bool m_ReachedEndOfPath;

	private bool m_InstantStopRestockWhenDayEnd;

	private bool m_InstantStopCounterWhenDayEnd;

	private bool m_IsFillShelfWithoutLabel;

	private bool m_HasAttemptFindEmptyBoxStorage;

	private int m_CurrentWaypoint;

	private int m_FailFindShelfAttemptCount;

	private int m_FailFindItemAttemptCount;

	public int m_CardTakenForRestockCount;

	public InteractableCardStorageShelf m_CurrentCardStorageShelf;

	private float m_LastRepath = float.NegativeInfinity;

	private float m_ExtraSpeedMultiplier = 1f;

	private float m_OriginalSpeedMultiplier = 1f;

	public Transform m_TargetTransform;

	private Quaternion m_RotationBeforeInteract;

	private Quaternion m_TargetLerpRotation;

	private Vector3 m_LerpStartPos;

	private Vector3 m_TargetLerpPos;

	private bool m_IsInsideShop;

	private bool m_IsWaitingForPathCallback;

	private bool m_UnableToFindQueue;

	private bool m_HasUpdatedCustomerCount;

	private bool m_FirstTimeReachRestPosition;

	private bool m_IsExclaimationVisibleState;

	public bool m_CanFindStoredItem;

	public bool m_HaveValidItemToRestock;

	private bool m_IsRoundUpPrice;

	private bool m_IsRoundUpCardPrice;

	public bool m_CanDoSecondaryTask;

	private bool m_IsSetTaskSettingPrimarySecondary;

	private float m_Timer;

	private float m_TimerMax;

	private float m_SecondaryTimer;

	private float m_SecondaryTimerMax;

	private float m_SetPriceMultiplier = 1f;

	private float m_SetCardPriceMultiplier = 1f;

	private List<EItemType> m_CardPackItemTypeList = new List<EItemType>();

	private List<bool> m_CardPackItemTypeEnabledList = new List<bool>();

	private List<CardData> m_CardPriceChangedList = new List<CardData>();

	public List<int> m_ExpList = new List<int>();

	public List<int> m_TaskLevel = new List<int>();

	public bool m_IsBonusBoosted;

	public int m_BonusBoostedCount;

	public int m_FailFindTaskCount;

	public void InitializeCharacter()
	{
		if (!m_CharacterCustom.m_HasInit)
		{
			if (m_IsFemale)
			{
				m_CharacterCustom.CharacterName = "Worker" + m_WorkerIndex;
			}
			else
			{
				m_CharacterCustom.CharacterName = "Worker" + m_WorkerIndex;
			}
			m_CharacterCustom.Initialize();
			for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_CardPackItemTypeList.Count; i++)
			{
				m_CardPackItemTypeList.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_CardPackItemTypeList[i]);
				m_CardPackItemTypeEnabledList.Add(item: true);
			}
			for (int j = 0; j < 100; j++)
			{
				m_TaskLevel.Add(0);
				m_ExpList.Add(0);
			}
		}
	}

	public void OnRaycasted()
	{
	}

	public void OnRaycastEnded()
	{
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
		WorkerInteractUIScreen.OpenScreen(this);
		m_IsExclaimationVisibleState = m_ExclaimationMesh.activeSelf;
		m_ExclaimationMesh.SetActive(value: false);
	}

	public void OnPressStopInteract()
	{
		m_TargetLerpRotation = m_RotationBeforeInteract;
		m_IsPausingAction = false;
		CSingleton<InteractionPlayerController>.Instance.ExitWorkerInteractMode();
		CSingleton<InteractionPlayerController>.Instance.StopAimLookAt();
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(isStop: false);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		GameUIScreen.ResetToolTipVisibility();
		GameUIScreen.ResetEnterGoNextDayIndicatorVisible();
		TutorialManager.SetGameUIVisible(isVisible: true);
		m_ExclaimationMesh.SetActive(m_IsExclaimationVisibleState);
	}

	public void SetRestockShelfWithNoLabel(bool isFillShelfWithoutLabel)
	{
		m_IsFillShelfWithoutLabel = isFillShelfWithoutLabel;
	}

	public void UpdateSetPriceOption(bool isRoundUpPrice, float setPriceMultiplier)
	{
		m_IsRoundUpPrice = isRoundUpPrice;
		m_SetPriceMultiplier = setPriceMultiplier;
	}

	public void UpdateSetCardPriceOption(bool isRoundUpPrice, float setPriceMultiplier)
	{
		m_IsRoundUpCardPrice = isRoundUpPrice;
		m_SetCardPriceMultiplier = setPriceMultiplier;
	}

	public void SetLastTask(EWorkerTask task)
	{
		m_LastWorkerTask = task;
	}

	public void SetTask(EWorkerTask task)
	{
		if (task == EWorkerTask.GoBackHome && m_CanDoSecondaryTask)
		{
			m_CanDoSecondaryTask = false;
			DetermineShopAction();
		}
		else if (m_PrimaryTask != task)
		{
			m_PrimaryTask = task;
			m_WorkerTask = task;
			m_CurrentState = EWorkerState.Idle;
			m_Timer = 10f;
			m_HaveValidItemToRestock = false;
			if (m_PrimaryTask != EWorkerTask.Rest || m_SecondaryTask != EWorkerTask.Rest)
			{
				m_FirstTimeReachRestPosition = true;
				m_IsExclaimationVisibleState = false;
				m_ExclaimationMesh.SetActive(value: false);
			}
			if ((bool)m_CurrentCashierCounter && task != EWorkerTask.ManCounter)
			{
				m_CurrentCashierCounter.SetCurrentWorker(null);
				m_CurrentCashierCounter = null;
			}
		}
	}

	public void SetSecondaryTask(EWorkerTask task)
	{
		if (m_SecondaryTask != task)
		{
			m_SecondaryTask = task;
			if (m_WorkerTask != m_PrimaryTask)
			{
				m_WorkerTask = m_PrimaryTask;
				m_CurrentState = EWorkerState.Idle;
				m_Timer = 10f;
				m_HaveValidItemToRestock = false;
			}
			if (m_PrimaryTask != EWorkerTask.Rest || m_SecondaryTask != EWorkerTask.Rest)
			{
				m_FirstTimeReachRestPosition = true;
				m_IsExclaimationVisibleState = false;
				m_ExclaimationMesh.SetActive(value: false);
			}
			if ((bool)m_CurrentCashierCounter && task != EWorkerTask.ManCounter)
			{
				m_CurrentCashierCounter.SetCurrentWorker(null);
				m_CurrentCashierCounter = null;
			}
		}
	}

	public void FireWorker()
	{
		CPlayerData.SetIsWorkerHired(m_WorkerIndex, isHired: false);
		m_WorkerCollider.gameObject.SetActive(value: false);
		m_LastWorkerTask = EWorkerTask.Rest;
		SetSecondaryTask(EWorkerTask.Fired);
		SetTask(EWorkerTask.Fired);
		DetermineShopAction();
	}

	public void CounterNextCustomer(int customerInQueueCount)
	{
		if (LightManager.GetHasDayEnded() && (!CSingleton<CustomerManager>.Instance.HasCustomerInShop() || (m_InstantStopCounterWhenDayEnd && !m_IsBonusBoosted)))
		{
			if (customerInQueueCount <= 1)
			{
				SetTask(EWorkerTask.GoBackHome);
				DetermineShopAction();
			}
		}
		else if (!LightManager.GetHasDayEnded() && m_SecondaryTask == EWorkerTask.ManCounter && customerInQueueCount == 0)
		{
			EvaluateSwitchFromSecondaryToPrimaryTask();
			DetermineShopAction();
		}
	}

	public void OnDayEnded()
	{
		if (m_WorkerTask != EWorkerTask.ManCounter && m_WorkerTask != EWorkerTask.Fired)
		{
			if (m_InstantStopRestockWhenDayEnd && !m_IsBonusBoosted)
			{
				SetTask(EWorkerTask.GoBackHome);
			}
		}
		else
		{
			if (m_WorkerTask != EWorkerTask.ManCounter || (CSingleton<CustomerManager>.Instance.HasCustomerInShop() && (!m_InstantStopCounterWhenDayEnd || m_IsBonusBoosted)))
			{
				return;
			}
			if ((bool)m_CurrentCashierCounter)
			{
				if (m_CurrentCashierCounter.GetCustomerInQueueCount() <= 0)
				{
					SetTask(EWorkerTask.GoBackHome);
					DetermineShopAction();
				}
			}
			else
			{
				SetTask(EWorkerTask.GoBackHome);
				DetermineShopAction();
			}
		}
	}

	private void EvaluateWorkerAttribute()
	{
		m_WorkerData = WorkerManager.GetWorkerData(m_WorkerIndex);
		m_ScanItemTime = m_WorkerData.checkoutSpeed * 1.33f;
		m_GiveChangeTime = m_WorkerData.checkoutSpeed;
		m_RestockTime = m_WorkerData.restockSpeed;
		m_ExtraSpeedMultiplier = 1f + m_WorkerData.walkSpeedMultiplier;
		m_InstantStopCounterWhenDayEnd = m_WorkerData.goBackOnTime;
		m_InstantStopRestockWhenDayEnd = m_WorkerData.goBackOnTime;
	}

	public WorkerData GetWorkerData()
	{
		return m_WorkerData;
	}

	public void ActivateWorker(bool resetTask)
	{
		m_IsActive = true;
		m_WorkerCollider.gameObject.SetActive(value: true);
		m_ExclaimationMesh.SetActive(value: false);
		InitializeCharacter();
		EvaluateWorkerAttribute();
		EvaluateSkillLevel();
		m_Timer = 0f;
		m_FailFindShelfAttemptCount = 0;
		m_FailFindItemAttemptCount = 0;
		m_IsWaitingForPathCallback = false;
		m_UnableToFindQueue = false;
		m_FirstTimeReachRestPosition = false;
		m_HasAttemptFindEmptyBoxStorage = false;
		m_CanDoSecondaryTask = true;
		m_IsBonusBoosted = false;
		m_BonusBoostedCount = 0;
		m_CardPriceChangedList.Clear();
		m_TargetTransform = CustomerManager.GetRandomExitPoint();
		base.transform.position = m_TargetTransform.position;
		base.transform.rotation = m_TargetTransform.rotation;
		m_TargetLerpRotation = m_TargetTransform.rotation;
		m_CurrentCashierCounter = null;
		m_CurrentHoldItemBox = null;
		m_CurrentItemBox = null;
		if (resetTask)
		{
			m_PrimaryTask = EWorkerTask.Rest;
			m_SecondaryTask = EWorkerTask.Rest;
			m_WorkerTask = EWorkerTask.Rest;
			m_LastWorkerTask = EWorkerTask.Rest;
		}
		else
		{
			m_PrimaryTask = m_LastWorkerTask;
			m_WorkerTask = m_LastWorkerTask;
			if (m_WorkerTask != EWorkerTask.Rest)
			{
				m_FirstTimeReachRestPosition = true;
			}
		}
		SetState(EWorkerState.Idle);
		m_Timer = 10f;
		m_Anim.SetBool("HoldingBag", value: false);
		m_Anim.SetBool("HandingOverCash", value: false);
	}

	public void DeactivateWorker()
	{
		if ((bool)m_CurrentHoldItemBox)
		{
			DropBoxOnHand();
		}
		if (!m_HasUpdatedCustomerCount)
		{
			m_HasUpdatedCustomerCount = true;
			CSingleton<WorkerManager>.Instance.UpdateWorkerCount(-1);
		}
		SetOutOfScreen();
		base.gameObject.SetActive(value: false);
		m_IsActive = false;
		m_IsInsideShop = false;
		m_CurrentState = EWorkerState.Idle;
		if (!CPlayerData.GetIsWorkerHired(m_WorkerIndex))
		{
			m_PrimaryTask = EWorkerTask.Rest;
			m_SecondaryTask = EWorkerTask.Rest;
			m_WorkerTask = EWorkerTask.Rest;
			m_LastWorkerTask = EWorkerTask.Rest;
		}
		m_HasUpdatedCustomerCount = false;
		m_FirstTimeReachRestPosition = false;
		m_HasAttemptFindEmptyBoxStorage = false;
	}

	private void DetermineShopAction()
	{
		if (m_CurrentState != EWorkerState.WalkingToRestOutside)
		{
			m_ReachedEndOfPath = false;
			m_Path = null;
		}
		if (m_CanDoSecondaryTask)
		{
			if (m_SecondaryTask != m_PrimaryTask && m_PrimaryTask != EWorkerTask.Rest && m_SecondaryTask != EWorkerTask.Rest)
			{
				m_CanFindStoredItem = !m_CanFindStoredItem;
			}
			m_WorkerTask = m_SecondaryTask;
		}
		else
		{
			if (m_PrimaryTask != m_SecondaryTask && m_PrimaryTask != EWorkerTask.Rest && m_SecondaryTask != EWorkerTask.Rest)
			{
				m_CanFindStoredItem = !m_CanFindStoredItem;
			}
			m_WorkerTask = m_PrimaryTask;
		}
		EvaluateTaskLevelStat();
		if ((bool)m_CurrentCashierCounter && m_WorkerTask != EWorkerTask.ManCounter)
		{
			m_CurrentCashierCounter.StopCurrentWorker();
		}
		if ((bool)m_CurrentHoldItemBox && m_WorkerTask != EWorkerTask.RestockShelf && m_WorkerTask != EWorkerTask.RefillCleanser && m_WorkerTask != EWorkerTask.RefillCardOpener)
		{
			DropBoxOnHand();
		}
		if (m_WorkerTask == EWorkerTask.ManCounter)
		{
			AttemptFindCashierCounter();
		}
		else if (m_WorkerTask == EWorkerTask.SetPrice)
		{
			AttemptFindSetPriceShelf();
		}
		else if (m_WorkerTask == EWorkerTask.RefillCleanser)
		{
			AttemptRefillCleanser();
		}
		else if (m_WorkerTask == EWorkerTask.RefillCardOpener)
		{
			AttemptRefillCardOpener();
		}
		else if (m_WorkerTask == EWorkerTask.RestockCardDisplay)
		{
			if (m_CardTakenForRestockCount <= 0 || !m_CurrentCardStorageShelf)
			{
				AttemptFindCardStorageShelf();
			}
			else
			{
				AttemptRestockCardTable();
			}
		}
		else if (m_WorkerTask == EWorkerTask.RestockShelf)
		{
			if ((bool)m_CurrentHoldItemBox)
			{
				AttemptFindShelf();
				return;
			}
			List<InteractablePackagingBox_Item> itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: false);
			List<EItemType> list = new List<EItemType>();
			List<EItemType> list2 = new List<EItemType>();
			List<EItemType> list3 = new List<EItemType>();
			for (int num = itemPackagingBoxListWithItem.Count - 1; num >= 0; num--)
			{
				if (!itemPackagingBoxListWithItem[num].IsValidObject() || !itemPackagingBoxListWithItem[num].CanWorkerTakeBox() || (!itemPackagingBoxListWithItem[num].m_IsStored && itemPackagingBoxListWithItem[num].transform.position.y > 2f))
				{
					itemPackagingBoxListWithItem.RemoveAt(num);
				}
			}
			bool flag = false;
			if (m_CanFindStoredItem)
			{
				itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: true);
				flag = true;
			}
			for (int num2 = itemPackagingBoxListWithItem.Count - 1; num2 >= 0; num2--)
			{
				if (!itemPackagingBoxListWithItem[num2].IsValidObject() || !itemPackagingBoxListWithItem[num2].CanWorkerTakeBox())
				{
					itemPackagingBoxListWithItem.RemoveAt(num2);
				}
			}
			if (flag)
			{
				m_HaveValidItemToRestock = itemPackagingBoxListWithItem.Count > 0;
			}
			for (int i = 0; i < itemPackagingBoxListWithItem.Count; i++)
			{
				EItemType eItemType = itemPackagingBoxListWithItem[i].m_ItemCompartment.GetItemType();
				if (itemPackagingBoxListWithItem[i].m_ItemCompartment.GetItemCount() <= 0)
				{
					eItemType = EItemType.None;
				}
				if (eItemType == EItemType.None)
				{
					continue;
				}
				if (!list.Contains(eItemType) && eItemType != EItemType.None)
				{
					list.Add(eItemType);
				}
				if (itemPackagingBoxListWithItem[i].m_IsBigBox && !itemPackagingBoxListWithItem[i].m_IsStored)
				{
					if (!list3.Contains(eItemType))
					{
						list3.Add(eItemType);
					}
				}
				else if (!itemPackagingBoxListWithItem[i].m_IsBigBox && !itemPackagingBoxListWithItem[i].m_IsStored && !list2.Contains(eItemType))
				{
					list2.Add(eItemType);
				}
			}
			List<Shelf> list4 = new List<Shelf>();
			List<Shelf> list5 = new List<Shelf>();
			bool flag2 = false;
			if (!m_IsFillShelfWithoutLabel)
			{
				for (int j = 0; j < list.Count; j++)
				{
					list5 = ShelfManager.GetShelfListToRestockItem(list[j], ignoreNoneType: true);
					for (int k = 0; k < list5.Count; k++)
					{
						if (!list4.Contains(list5[k]))
						{
							if (WorkerManager.GetActiveWorkerCount() == 1)
							{
								list4.Add(list5[k]);
								flag2 = true;
							}
							else if (0 == 0)
							{
								list4.Add(list5[k]);
								flag2 = true;
							}
						}
					}
				}
			}
			if (m_IsFillShelfWithoutLabel && !flag2)
			{
				for (int l = 0; l < list.Count; l++)
				{
					list5 = ShelfManager.GetShelfListToRestockItem(list[l]);
					for (int m = 0; m < list5.Count; m++)
					{
						if (!list4.Contains(list5[m]))
						{
							list4.Add(list5[m]);
						}
					}
				}
			}
			bool flag3 = false;
			List<ShelfCompartment> list6 = new List<ShelfCompartment>();
			List<EItemType> list7 = new List<EItemType>();
			for (int n = 0; n < list4.Count; n++)
			{
				List<ShelfCompartment> list8 = new List<ShelfCompartment>();
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					list8 = list4[n].GetNonFullItemCompartmentList(list[num3], flag2);
					for (int num4 = 0; num4 < list8.Count; num4++)
					{
						if (!list6.Contains(list8[num4]))
						{
							list6.Add(list8[num4]);
						}
					}
				}
				for (int num5 = 0; num5 < list6.Count; num5++)
				{
					if (list6[num5].GetItemCount() >= list6[num5].GetMaxItemCount() && (flag2 || (list6[num5].GetItemType() != EItemType.None && list6[num5].GetItemCount() != 0)))
					{
						continue;
					}
					if (WorkerManager.GetActiveWorkerCount() == 1)
					{
						if (list.Contains(list6[num5].GetItemType()))
						{
							m_TargetBoxItemType = list6[num5].GetItemType();
							list7.Add(list6[num5].GetItemType());
							flag3 = true;
						}
						else if (list.Count > 0 && m_IsFillShelfWithoutLabel && (list6[num5].GetItemType() == EItemType.None || list6[num5].GetItemCount() == 0))
						{
							list7.Add(list[Random.Range(0, list.Count)]);
							flag3 = true;
						}
						continue;
					}
					bool flag4 = false;
					for (int num6 = 0; num6 < WorkerManager.GetWorkerList().Count; num6++)
					{
						if (WorkerManager.GetWorkerList()[num6] != this && WorkerManager.GetWorkerList()[num6].IsActive() && WorkerManager.GetWorkerList()[num6].CheckWorkerSameTarget(list6[num5].m_CustomerStandLoc))
						{
							flag4 = true;
							break;
						}
					}
					if (!flag4)
					{
						if (list.Contains(list6[num5].GetItemType()))
						{
							m_TargetBoxItemType = list6[num5].GetItemType();
							list7.Add(list6[num5].GetItemType());
							flag3 = true;
						}
						else if (list.Count > 0 && m_IsFillShelfWithoutLabel && (list6[num5].GetItemType() == EItemType.None || list6[num5].GetItemCount() == 0))
						{
							list7.Add(list[Random.Range(0, list.Count)]);
							flag3 = true;
						}
					}
				}
			}
			if (list7.Count > 0)
			{
				m_TargetBoxItemType = list7[Random.Range(0, list7.Count)];
			}
			if (itemPackagingBoxListWithItem.Count <= 0)
			{
				GoRestPoint();
				if (!flag)
				{
					m_CanFindStoredItem = true;
				}
				else
				{
					m_HaveValidItemToRestock = false;
				}
				return;
			}
			if (flag3)
			{
				m_TargetBoxSize = 0;
				AttemptFindBoxForRestock();
				m_HaveValidItemToRestock = true;
				return;
			}
			List<WarehouseShelf> list9 = new List<WarehouseShelf>();
			List<WarehouseShelf> list10 = new List<WarehouseShelf>();
			List<WarehouseShelf> list11 = new List<WarehouseShelf>();
			flag2 = false;
			for (int num7 = 0; num7 < list2.Count; num7++)
			{
				list11 = ShelfManager.GetWarehouseShelfListToStoreItem(list2[num7], isBigBox: false, ignoreNoneType: true);
				for (int num8 = 0; num8 < list11.Count; num8++)
				{
					list9.Add(list11[num8]);
					flag2 = true;
				}
			}
			for (int num9 = 0; num9 < list3.Count; num9++)
			{
				list11 = ShelfManager.GetWarehouseShelfListToStoreItem(list3[num9], isBigBox: true, ignoreNoneType: true);
				for (int num10 = 0; num10 < list11.Count; num10++)
				{
					if (!list10.Contains(list11[num10]))
					{
						list10.Add(list11[num10]);
						flag2 = true;
					}
				}
			}
			if (m_IsFillShelfWithoutLabel && !flag2)
			{
				for (int num11 = 0; num11 < list2.Count; num11++)
				{
					if (!list.Contains(list2[num11]))
					{
						continue;
					}
					list11 = ShelfManager.GetWarehouseShelfListToStoreItem(list2[num11], isBigBox: false);
					for (int num12 = 0; num12 < list11.Count; num12++)
					{
						if (!list9.Contains(list11[num12]))
						{
							list9.Add(list11[num12]);
						}
					}
				}
				for (int num13 = 0; num13 < list3.Count; num13++)
				{
					if (!list.Contains(list3[num13]))
					{
						continue;
					}
					list11 = ShelfManager.GetWarehouseShelfListToStoreItem(list3[num13], isBigBox: true);
					for (int num14 = 0; num14 < list11.Count; num14++)
					{
						if (!list10.Contains(list11[num14]))
						{
							list10.Add(list11[num14]);
						}
					}
				}
			}
			bool flag5 = false;
			List<ShelfCompartment> list12 = new List<ShelfCompartment>();
			for (int num15 = 0; num15 < list9.Count; num15++)
			{
				List<ShelfCompartment> list13 = new List<ShelfCompartment>();
				for (int num16 = 0; num16 < list2.Count; num16++)
				{
					if (!list.Contains(list2[num16]))
					{
						continue;
					}
					list13 = list9[num15].GetNonFullItemCompartmentList(list2[num16], isBigBox: false, flag2);
					for (int num17 = 0; num17 < list13.Count; num17++)
					{
						if (!list12.Contains(list13[num17]))
						{
							list12.Add(list13[num17]);
						}
					}
				}
				for (int num18 = 0; num18 < list12.Count; num18++)
				{
					if ((list12[num18].GetItemCount() < list12[num18].GetMaxItemCount() || (!flag2 && list12[num18].GetItemType() == EItemType.None) || list12[num18].GetItemCount() == 0) && !list12[num18].CheckBoxType(isBigBox: false))
					{
						if (WorkerManager.GetActiveWorkerCount() == 1)
						{
							m_TargetBoxItemType = list12[num18].GetItemType();
							if (!list2.Contains(m_TargetBoxItemType))
							{
								m_TargetBoxItemType = list2[Random.Range(0, list2.Count)];
							}
							m_TargetBoxSize = 1;
							flag5 = true;
							break;
						}
						if (0 == 0)
						{
							m_TargetBoxItemType = list12[num18].GetItemType();
							if (!list2.Contains(m_TargetBoxItemType))
							{
								m_TargetBoxItemType = list2[Random.Range(0, list2.Count)];
							}
							m_TargetBoxSize = 1;
							flag5 = true;
						}
					}
					if (flag5)
					{
						break;
					}
				}
				if (flag5)
				{
					break;
				}
			}
			if (flag5)
			{
				AttemptFindBoxToStore();
				m_HaveValidItemToRestock = true;
				return;
			}
			for (int num19 = 0; num19 < list10.Count; num19++)
			{
				List<ShelfCompartment> list14 = new List<ShelfCompartment>();
				for (int num20 = 0; num20 < list3.Count; num20++)
				{
					if (!list.Contains(list3[num20]))
					{
						continue;
					}
					list14 = list10[num19].GetNonFullItemCompartmentList(list3[num20], isBigBox: true, flag2);
					for (int num21 = 0; num21 < list14.Count; num21++)
					{
						if (!list12.Contains(list14[num21]))
						{
							list12.Add(list14[num21]);
						}
					}
				}
				for (int num22 = 0; num22 < list12.Count; num22++)
				{
					if ((list12[num22].GetItemCount() < list12[num22].GetMaxItemCount() || (!flag2 && (list12[num22].GetItemType() == EItemType.None || list12[num22].GetItemCount() == 0))) && list12[num22].CheckBoxType(isBigBox: true))
					{
						if (WorkerManager.GetActiveWorkerCount() == 1)
						{
							m_TargetBoxItemType = list12[num22].GetItemType();
							if (!list3.Contains(m_TargetBoxItemType))
							{
								m_TargetBoxItemType = list3[Random.Range(0, list3.Count)];
							}
							m_TargetBoxSize = 2;
							flag5 = true;
							break;
						}
						if (0 == 0)
						{
							m_TargetBoxItemType = list12[num22].GetItemType();
							if (!list3.Contains(m_TargetBoxItemType))
							{
								m_TargetBoxItemType = list3[Random.Range(0, list3.Count)];
							}
							m_TargetBoxSize = 2;
							flag5 = true;
						}
					}
					if (flag5)
					{
						break;
					}
				}
				if (flag5)
				{
					break;
				}
			}
			if (flag5)
			{
				AttemptFindBoxToStore();
				m_HaveValidItemToRestock = true;
				return;
			}
			GoRestPoint();
			if (!flag)
			{
				m_CanFindStoredItem = true;
			}
			else
			{
				m_HaveValidItemToRestock = false;
			}
		}
		else if (m_WorkerTask == EWorkerTask.Fired || m_WorkerTask == EWorkerTask.GoBackHome)
		{
			ExitShop();
		}
		else if (LightManager.GetHasDayEnded())
		{
			ExitShop();
		}
		else
		{
			GoRestPoint();
			m_CanFindStoredItem = false;
		}
	}

	private void AttemptRefillCleanser()
	{
		SetState(EWorkerState.SearchingForCleanserToRefill);
		m_ReachedEndOfPath = false;
		List<InteractableAutoCleanser> list = new List<InteractableAutoCleanser>();
		for (int i = 0; i < ShelfManager.GetAutoCleanserList().Count; i++)
		{
			if (!ShelfManager.GetAutoCleanserList()[i].IsValidObject() || !ShelfManager.GetAutoCleanserList()[i].HasEnoughSlot())
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < WorkerManager.GetWorkerList().Count; j++)
			{
				if (WorkerManager.GetWorkerList()[j] != this && WorkerManager.GetWorkerList()[j].IsActive() && WorkerManager.GetWorkerList()[j].CheckWorkerSameTarget(ShelfManager.GetAutoCleanserList()[i].m_StandLoc))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(ShelfManager.GetAutoCleanserList()[i]);
			}
		}
		if (list.Count > 0)
		{
			if ((bool)m_CurrentHoldItemBox && m_CurrentHoldItemBox.GetItemType() == EItemType.Deodorant)
			{
				m_TargetRefillCleanser = list[Random.Range(0, list.Count)];
				if ((bool)m_TargetRefillCleanser.m_StandLoc)
				{
					m_TargetTransform = m_TargetRefillCleanser.m_StandLoc.transform;
				}
				else
				{
					m_TargetTransform = m_TargetRefillCleanser.transform;
				}
				m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
				m_IsWaitingForPathCallback = true;
				m_UnableToFindQueue = false;
				m_ReachedEndOfPath = false;
				SetState(EWorkerState.WalkToCleanser);
			}
			else if ((bool)m_CurrentHoldItemBox && m_CurrentHoldItemBox.GetItemType() != EItemType.Deodorant)
			{
				AttemptFindWarehouseShelf();
			}
			else
			{
				m_TargetRefillCleanser = list[Random.Range(0, list.Count)];
				m_TargetBoxItemType = EItemType.Deodorant;
				AttemptFindBoxToRefillCleanser();
			}
		}
		else if ((bool)m_CurrentHoldItemBox)
		{
			AttemptFindWarehouseShelf();
		}
		else
		{
			m_TargetTransform = null;
			m_CanFindStoredItem = false;
			GoRestPoint();
		}
	}

	private void AttemptFindBoxToRefillCleanser()
	{
		m_ReachedEndOfPath = false;
		m_CurrentItemBox = null;
		List<InteractablePackagingBox_Item> itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: true);
		List<InteractablePackagingBox_Item> list = new List<InteractablePackagingBox_Item>();
		for (int num = itemPackagingBoxListWithItem.Count - 1; num >= 0; num--)
		{
			if (!itemPackagingBoxListWithItem[num].IsValidObject() || !itemPackagingBoxListWithItem[num].CanWorkerTakeBox() || (!itemPackagingBoxListWithItem[num].m_IsStored && itemPackagingBoxListWithItem[num].transform.position.y > 2f))
			{
				itemPackagingBoxListWithItem.RemoveAt(num);
			}
		}
		if (itemPackagingBoxListWithItem.Count == 0)
		{
			itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: false);
			for (int num2 = itemPackagingBoxListWithItem.Count - 1; num2 >= 0; num2--)
			{
				if (!itemPackagingBoxListWithItem[num2].IsValidObject() || !itemPackagingBoxListWithItem[num2].CanWorkerTakeBox() || (!itemPackagingBoxListWithItem[num2].m_IsStored && itemPackagingBoxListWithItem[num2].transform.position.y > 2f))
				{
					itemPackagingBoxListWithItem.RemoveAt(num2);
				}
			}
		}
		for (int i = 0; i < itemPackagingBoxListWithItem.Count; i++)
		{
			if (m_TargetBoxItemType != EItemType.None && m_TargetBoxItemType != itemPackagingBoxListWithItem[i].m_ItemCompartment.GetItemType())
			{
				continue;
			}
			if (WorkerManager.GetActiveWorkerCount() == 1)
			{
				list.Add(itemPackagingBoxListWithItem[i]);
				continue;
			}
			for (int j = 0; j < WorkerManager.GetWorkerList().Count; j++)
			{
				if (WorkerManager.GetWorkerList()[j] != this && WorkerManager.GetWorkerList()[j].IsActive() && WorkerManager.GetWorkerList()[j].GetCurrentItemBox() != itemPackagingBoxListWithItem[i] && m_LastStoredItemBox != itemPackagingBoxListWithItem[i])
				{
					list.Add(itemPackagingBoxListWithItem[i]);
				}
			}
		}
		m_HaveValidItemToRestock = list.Count > 0;
		if (list.Count > 0)
		{
			int num3 = 0;
			int index = 0;
			for (int k = 0; k < list.Count; k++)
			{
				if (Mathf.RoundToInt((float)list[k].m_ItemCompartment.GetItemCount() / (float)list[k].m_ItemCompartment.GetMaxItemCount() * 100f) > num3)
				{
					index = k;
				}
			}
			m_CurrentItemBox = list[index];
		}
		if ((bool)m_CurrentItemBox)
		{
			m_TargetTransform = m_CurrentItemBox.transform;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(EWorkerState.SearchingForBoxToRefillCleanser);
		}
		else
		{
			m_TargetTransform = null;
			m_CanFindStoredItem = false;
			GoRestPoint();
		}
	}

	private void AttemptRefillCardOpener()
	{
		SetState(EWorkerState.SearchingForBoxToRefillCardOpener);
		m_ReachedEndOfPath = false;
		List<InteractableAutoPackOpener> list = new List<InteractableAutoPackOpener>();
		for (int i = 0; i < ShelfManager.GetAutoPackOpenerList().Count; i++)
		{
			if (!ShelfManager.GetAutoPackOpenerList()[i].IsValidObject() || !ShelfManager.GetAutoPackOpenerList()[i].HasEnoughSlot() || ShelfManager.GetAutoPackOpenerList()[i].GetIsProcessing())
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < WorkerManager.GetWorkerList().Count; j++)
			{
				if (WorkerManager.GetWorkerList()[j] != this && WorkerManager.GetWorkerList()[j].IsActive() && WorkerManager.GetWorkerList()[j].CheckWorkerSameTarget(ShelfManager.GetAutoPackOpenerList()[i].m_StandLoc))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(ShelfManager.GetAutoPackOpenerList()[i]);
			}
		}
		if (list.Count > 0)
		{
			bool flag2 = false;
			if ((bool)m_CurrentHoldItemBox && m_CardPackItemTypeList.Contains(m_CurrentHoldItemBox.GetItemType()))
			{
				int index = m_CardPackItemTypeList.IndexOf(m_CurrentHoldItemBox.GetItemType());
				if (m_CardPackItemTypeEnabledList[index])
				{
					flag2 = true;
				}
			}
			if ((bool)m_CurrentHoldItemBox && flag2)
			{
				m_TargetRefillCardOpener = list[Random.Range(0, list.Count)];
				if ((bool)m_TargetRefillCardOpener.m_StandLoc)
				{
					m_TargetTransform = m_TargetRefillCardOpener.m_StandLoc.transform;
				}
				else
				{
					m_TargetTransform = m_TargetRefillCardOpener.transform;
				}
				m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
				m_IsWaitingForPathCallback = true;
				m_UnableToFindQueue = false;
				m_ReachedEndOfPath = false;
				SetState(EWorkerState.WalkToCardOpener);
				return;
			}
			if ((bool)m_CurrentHoldItemBox && !flag2)
			{
				AttemptFindWarehouseShelf();
				return;
			}
			List<EItemType> list2 = new List<EItemType>();
			for (int k = 0; k < m_CardPackItemTypeEnabledList.Count; k++)
			{
				if (m_CardPackItemTypeEnabledList[k])
				{
					list2.Add(m_CardPackItemTypeList[k]);
				}
			}
			if (list2.Count > 0)
			{
				m_TargetRefillCardOpener = list[Random.Range(0, list.Count)];
				m_TargetBoxItemType = list2[Random.Range(0, list2.Count)];
				AttemptFindBoxToRefillCardOpener();
			}
			else
			{
				m_TargetTransform = null;
				m_CanFindStoredItem = false;
				GoRestPoint();
			}
		}
		else if ((bool)m_CurrentHoldItemBox)
		{
			AttemptFindWarehouseShelf();
		}
		else
		{
			m_TargetTransform = null;
			m_CanFindStoredItem = false;
			GoRestPoint();
		}
	}

	private void AttemptFindBoxToRefillCardOpener()
	{
		m_ReachedEndOfPath = false;
		m_CurrentItemBox = null;
		List<InteractablePackagingBox_Item> itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: true);
		List<InteractablePackagingBox_Item> list = new List<InteractablePackagingBox_Item>();
		for (int num = itemPackagingBoxListWithItem.Count - 1; num >= 0; num--)
		{
			if (!itemPackagingBoxListWithItem[num].IsValidObject() || !itemPackagingBoxListWithItem[num].CanWorkerTakeBox() || (!itemPackagingBoxListWithItem[num].m_IsStored && itemPackagingBoxListWithItem[num].transform.position.y > 2f))
			{
				itemPackagingBoxListWithItem.RemoveAt(num);
			}
		}
		if (itemPackagingBoxListWithItem.Count == 0)
		{
			itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: false);
			for (int num2 = itemPackagingBoxListWithItem.Count - 1; num2 >= 0; num2--)
			{
				if (!itemPackagingBoxListWithItem[num2].IsValidObject() || !itemPackagingBoxListWithItem[num2].CanWorkerTakeBox() || (!itemPackagingBoxListWithItem[num2].m_IsStored && itemPackagingBoxListWithItem[num2].transform.position.y > 2f))
				{
					itemPackagingBoxListWithItem.RemoveAt(num2);
				}
			}
		}
		List<EItemType> list2 = new List<EItemType>();
		for (int i = 0; i < m_CardPackItemTypeEnabledList.Count; i++)
		{
			if (m_CardPackItemTypeEnabledList[i])
			{
				list2.Add(m_CardPackItemTypeList[i]);
			}
		}
		for (int j = 0; j < itemPackagingBoxListWithItem.Count; j++)
		{
			if (m_TargetBoxItemType != EItemType.None && !list2.Contains(itemPackagingBoxListWithItem[j].m_ItemCompartment.GetItemType()))
			{
				continue;
			}
			if (WorkerManager.GetActiveWorkerCount() == 1)
			{
				list.Add(itemPackagingBoxListWithItem[j]);
				continue;
			}
			for (int k = 0; k < WorkerManager.GetWorkerList().Count; k++)
			{
				if (WorkerManager.GetWorkerList()[k] != this && WorkerManager.GetWorkerList()[k].IsActive() && WorkerManager.GetWorkerList()[k].GetCurrentItemBox() != itemPackagingBoxListWithItem[j] && m_LastStoredItemBox != itemPackagingBoxListWithItem[j])
				{
					list.Add(itemPackagingBoxListWithItem[j]);
				}
			}
		}
		m_HaveValidItemToRestock = list.Count > 0;
		if (list.Count > 0)
		{
			int num3 = 0;
			int index = 0;
			for (int l = 0; l < list.Count; l++)
			{
				if (Mathf.RoundToInt((float)list[l].m_ItemCompartment.GetItemCount() / (float)list[l].m_ItemCompartment.GetMaxItemCount() * 100f) > num3)
				{
					index = l;
				}
			}
			m_CurrentItemBox = list[index];
		}
		if ((bool)m_CurrentItemBox)
		{
			m_TargetTransform = m_CurrentItemBox.transform;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(EWorkerState.SearchingForBoxToRefillCardOpener);
		}
		else
		{
			m_TargetTransform = null;
			m_CanFindStoredItem = false;
			GoRestPoint();
		}
	}

	private void AttemptFindCardStorageShelf()
	{
		m_ReachedEndOfPath = false;
		m_CurrentCardStorageShelf = ShelfManager.GetCardStorageShelf();
		bool flag = false;
		if ((bool)m_CurrentCardStorageShelf)
		{
			flag = true;
			m_TargetTransform = m_CurrentCardStorageShelf.m_CustomerStandLocList[Random.Range(0, m_CurrentCardStorageShelf.m_CustomerStandLocList.Count)];
		}
		if (!m_CurrentCardStorageShelf || !m_CurrentCardStorageShelf.IsValidObject() || m_CurrentCardStorageShelf.GetTotalCardAmount() == 0 || !m_CurrentCardStorageShelf.CanWorkerTake())
		{
			flag = false;
		}
		if ((bool)m_TargetTransform && flag)
		{
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(EWorkerState.WalkToCardStorageShelf);
		}
		else
		{
			m_UnableToFindQueue = true;
			GoRestPoint();
		}
	}

	private void AttemptRestockCardTable()
	{
		SetState(EWorkerState.SearchingForCardTableToRestock);
		m_ReachedEndOfPath = false;
		List<CardShelf> list = new List<CardShelf>();
		List<InteractableCardCompartment> list2 = new List<InteractableCardCompartment>();
		List<InteractableCardCompartment> list3 = new List<InteractableCardCompartment>();
		if ((bool)m_CurrentCardShelf && m_CurrentCardShelf.IsValidObject())
		{
			for (int i = 0; i < m_CurrentCardShelf.GetCardCompartmentList().Count; i++)
			{
				if (m_CurrentCardShelf.GetCardCompartmentList()[i].m_StoredCardList.Count != 0)
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < WorkerManager.GetWorkerList().Count; j++)
				{
					if (WorkerManager.GetWorkerList()[j] != this && WorkerManager.GetWorkerList()[j].IsActive() && WorkerManager.GetWorkerList()[j].CheckWorkerSameTarget(m_CurrentCardShelf.GetCardCompartmentList()[i].m_CustomerStandLoc))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(m_CurrentCardShelf);
					break;
				}
			}
		}
		if (list.Count == 0)
		{
			list = ShelfManager.GetCardShelfList();
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (!list[k].IsValidObject() || list[k].m_ItemNotForSale)
			{
				continue;
			}
			list2 = list[k].GetCardCompartmentList();
			for (int l = 0; l < list2.Count; l++)
			{
				if (list2[l].m_StoredCardList.Count != 0)
				{
					continue;
				}
				bool flag2 = false;
				for (int m = 0; m < WorkerManager.GetWorkerList().Count; m++)
				{
					if (WorkerManager.GetWorkerList()[m] != this && WorkerManager.GetWorkerList()[m].IsActive() && WorkerManager.GetWorkerList()[m].CheckWorkerSameTarget(list2[l].m_CustomerStandLoc))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					list3.Add(list2[l]);
				}
			}
		}
		if (list3.Count > 0)
		{
			m_CurrentCardCompartment = list3[Random.Range(0, list3.Count)];
			m_CurrentCardShelf = m_CurrentCardCompartment.GetCardShelf();
		}
		else
		{
			m_CurrentCardCompartment = null;
			m_CurrentCardShelf = null;
		}
		if ((bool)m_CurrentCardCompartment)
		{
			m_TargetTransform = m_CurrentCardCompartment.m_CustomerStandLoc;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			m_ReachedEndOfPath = false;
			SetState(EWorkerState.WalkToCardTableToRestock);
		}
		else
		{
			m_UnableToFindQueue = true;
			GoRestPoint();
		}
	}

	private void OnReachedPathEnd()
	{
		if (m_IsPausingAction)
		{
			return;
		}
		if ((bool)m_TargetTransform && m_CurrentState != EWorkerState.SearchingForBox && m_CurrentState != EWorkerState.SearchingForBoxToStore && m_CurrentState != EWorkerState.SearchingForBoxToRefillCleanser && m_CurrentState != EWorkerState.SearchingForBoxToRefillCardOpener)
		{
			m_TargetLerpRotation = m_TargetTransform.rotation;
		}
		if (m_CurrentState == EWorkerState.WalkingToCounter)
		{
			if (!m_CurrentCashierCounter)
			{
				AttemptFindCashierCounter();
				m_FailFindShelfAttemptCount++;
			}
			else if (m_CurrentCashierCounter.IsValidObject() && m_CurrentCashierCounter.CanCheckout())
			{
				m_FailFindTaskCount = 0;
				m_CurrentCashierCounter.NPCStartManCounter(this);
				SetState(EWorkerState.ManningCounter);
			}
			else
			{
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.SearchingForBox || m_CurrentState == EWorkerState.SearchingForBoxToStore || m_CurrentState == EWorkerState.SearchingForBoxToRefillCleanser || m_CurrentState == EWorkerState.SearchingForBoxToRefillCardOpener)
		{
			float num = 0f;
			if ((bool)m_TargetTransform)
			{
				num = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_CurrentItemBox || num > 3.5f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
				return;
			}
			bool flag = false;
			if (m_CurrentItemBox.IsValidObject() && m_CurrentItemBox.CanWorkerTakeBox() && m_CurrentItemBox.m_ItemCompartment.GetItemCount() > 0)
			{
				if (m_CurrentItemBox.m_IsStored)
				{
					if (m_CurrentItemBox.GetBoxStoredCompartment().GetInteractablePackagingBoxList().Count <= 0)
					{
						m_CurrentItemBox = null;
						DetermineShopAction();
						return;
					}
					InteractablePackagingBox_Item lastInteractablePackagingBox = m_CurrentItemBox.GetBoxStoredCompartment().GetLastInteractablePackagingBox();
					if (!lastInteractablePackagingBox.CanPickup() || !lastInteractablePackagingBox.IsValidObject() || !lastInteractablePackagingBox.CanWorkerTakeBox())
					{
						m_CurrentItemBox = null;
						DetermineShopAction();
						return;
					}
					m_FailFindTaskCount = 0;
					flag = true;
					m_CurrentItemBox = lastInteractablePackagingBox;
					m_CurrentItemBox.StartHoldBox(isPlayer: false, m_HoldBoxLoc);
					m_CurrentItemBox.GetBoxStoredCompartment().RemoveBox(lastInteractablePackagingBox);
				}
				else
				{
					m_FailFindTaskCount = 0;
					m_CurrentItemBox.StartHoldBox(isPlayer: false, m_HoldBoxLoc);
				}
				m_CurrentHoldItemBox = m_CurrentItemBox;
				m_CurrentItemBox = null;
				m_Anim.SetBool("IsHoldingBox", value: true);
				if (m_CurrentState == EWorkerState.SearchingForBoxToRefillCleanser)
				{
					AttemptRefillCleanser();
				}
				else if (m_CurrentState == EWorkerState.SearchingForBoxToRefillCardOpener)
				{
					AttemptRefillCardOpener();
				}
				else if (m_CurrentState == EWorkerState.SearchingForBox || flag)
				{
					AttemptFindShelf();
				}
				else
				{
					AttemptFindWarehouseShelf();
				}
			}
			else
			{
				m_CurrentItemBox = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToShelf)
		{
			float num2 = 0f;
			if ((bool)m_TargetTransform)
			{
				num2 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_CurrentShelf || num2 > 1.25f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if (m_CurrentShelf.IsValidObject())
			{
				m_FailFindTaskCount = 0;
				SetState(EWorkerState.RestockingShelf);
			}
			else
			{
				m_CurrentShelf = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToWarehouseShelf)
		{
			float num3 = 0f;
			if ((bool)m_TargetTransform)
			{
				num3 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_CurrentWarehouseShelf || num3 > 3f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if (m_CurrentWarehouseShelf.IsValidObject())
			{
				m_FailFindTaskCount = 0;
				SetState(EWorkerState.StoreBoxOnWarehouseShelf);
			}
			else
			{
				m_CurrentWarehouseShelf = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.TrashingBox)
		{
			m_FailFindTaskCount = 0;
			Vector3 forward = CSingleton<WorkerManager>.Instance.m_TrashBin.transform.position - base.transform.position;
			if ((bool)m_CurrentTrashBin && m_CurrentTrashBin.IsValidObject())
			{
				forward = m_CurrentTrashBin.transform.position - base.transform.position;
			}
			forward.y = 0f;
			m_TargetLerpRotation = Quaternion.LookRotation(forward, Vector3.up);
			SetState(EWorkerState.ReadyToTrashBox);
		}
		else if (m_CurrentState == EWorkerState.StoringEmptyBox)
		{
			m_FailFindTaskCount = 0;
			if ((bool)m_CurrentEmptyBoxStorage && m_CurrentEmptyBoxStorage.IsValidObject() && m_CurrentEmptyBoxStorage.HasStorageSpace())
			{
				Vector3 forward2 = (forward2 = m_CurrentEmptyBoxStorage.transform.position - base.transform.position);
				forward2.y = 0f;
				m_TargetLerpRotation = Quaternion.LookRotation(forward2, Vector3.up);
				SetState(EWorkerState.ReadyToStoreEmptyBox);
				m_HasAttemptFindEmptyBoxStorage = false;
			}
			else
			{
				AttemptFindTrashBin();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToCleanser)
		{
			float num4 = 0f;
			if ((bool)m_TargetTransform)
			{
				num4 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_TargetRefillCleanser || num4 > 1.25f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if (m_TargetRefillCleanser.IsValidObject())
			{
				Vector3 forward3 = m_TargetRefillCleanser.transform.position - base.transform.position;
				forward3.y = 0f;
				m_TargetLerpRotation = Quaternion.LookRotation(forward3, Vector3.up);
				SetState(EWorkerState.RefillingCleanser);
			}
			else
			{
				m_TargetRefillCleanser = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToCardOpener)
		{
			float num5 = 0f;
			if ((bool)m_TargetTransform)
			{
				num5 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_TargetRefillCardOpener || num5 > 1.25f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if (m_TargetRefillCardOpener.IsValidObject() && !m_TargetRefillCardOpener.GetIsProcessing())
			{
				Vector3 forward4 = m_TargetRefillCardOpener.transform.position - base.transform.position;
				forward4.y = 0f;
				m_TargetLerpRotation = Quaternion.LookRotation(forward4, Vector3.up);
				SetState(EWorkerState.RefillingCardOpener);
			}
			else
			{
				m_TargetRefillCardOpener = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToCardStorageShelf)
		{
			float num6 = 0f;
			if ((bool)m_TargetTransform)
			{
				num6 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_CurrentCardStorageShelf || num6 > 1.25f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if (m_CurrentCardStorageShelf.IsValidObject() && m_CurrentCardStorageShelf.GetTotalCardAmount() > 0 && m_CurrentCardStorageShelf.CanWorkerTake())
			{
				Vector3 forward5 = m_CurrentCardStorageShelf.transform.position - base.transform.position;
				forward5.y = 0f;
				m_TargetLerpRotation = Quaternion.LookRotation(forward5, Vector3.up);
				SetState(EWorkerState.TakingCardStorageShelfCard);
			}
			else
			{
				m_CurrentCardStorageShelf = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToCardTableToRestock)
		{
			float num7 = 0f;
			if ((bool)m_TargetTransform)
			{
				num7 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_CurrentCardShelf || num7 > 1.25f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if ((bool)m_CurrentCardCompartment && (bool)m_CurrentCardShelf && m_CurrentCardShelf.IsValidObject())
			{
				Vector3 forward6 = m_CurrentCardCompartment.transform.position - base.transform.position;
				forward6.y = 0f;
				m_TargetLerpRotation = Quaternion.LookRotation(forward6, Vector3.up);
				SetState(EWorkerState.RestockingCardTable);
			}
			else
			{
				m_CurrentCardShelf = null;
				m_CurrentCardCompartment = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToShelfSetPrice)
		{
			float num8 = 0f;
			if ((bool)m_TargetTransform)
			{
				num8 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_CurrentShelf || num8 > 1.25f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if (m_CurrentShelf.IsValidObject())
			{
				SetState(EWorkerState.SettingShelfPrice);
			}
			else
			{
				m_CurrentShelf = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkToCardShelfSetPrice)
		{
			float num9 = 0f;
			if ((bool)m_TargetTransform)
			{
				num9 = (m_TargetTransform.position - base.transform.position).magnitude;
			}
			if (!m_CurrentCardCompartment || num9 > 1.25f)
			{
				DetermineShopAction();
				m_FailFindShelfAttemptCount++;
			}
			else if ((bool)m_CurrentCardCompartment && m_CurrentCardCompartment.GetCardShelf().IsValidObject())
			{
				SetState(EWorkerState.SettingCardPrice);
			}
			else
			{
				m_CurrentCardCompartment = null;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkingToRestOutside)
		{
			SetState(EWorkerState.Idle);
			if (!m_FirstTimeReachRestPosition && m_PrimaryTask == EWorkerTask.Rest && m_SecondaryTask == EWorkerTask.Rest)
			{
				m_FirstTimeReachRestPosition = true;
				m_ExclaimationMesh.SetActive(value: true);
			}
		}
		else if (m_CurrentState == EWorkerState.ExitingShop)
		{
			DeactivateWorker();
		}
		else if (m_CurrentState == EWorkerState.Idle && !m_FirstTimeReachRestPosition && m_WorkerTask == EWorkerTask.Rest)
		{
			m_FirstTimeReachRestPosition = true;
			m_ExclaimationMesh.SetActive(value: true);
		}
	}

	private bool StateUpdate()
	{
		if (m_IsPausingAction)
		{
			return true;
		}
		if (m_CurrentState == EWorkerState.Idle)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 2f)
			{
				m_FailFindTaskCount = 0;
				m_CanDoSecondaryTask = !m_CanDoSecondaryTask;
				if (m_SecondaryTask == EWorkerTask.Rest)
				{
					m_CanDoSecondaryTask = false;
				}
				if (m_PrimaryTask == EWorkerTask.Rest)
				{
					m_CanDoSecondaryTask = true;
				}
				m_Timer = 0f;
				DetermineShopAction();
			}
			return true;
		}
		if (m_CurrentState == EWorkerState.WalkingToRestOutside && m_PrimaryTask == EWorkerTask.ManCounter && m_SecondaryTask != EWorkerTask.ManCounter && m_WorkerTask != EWorkerTask.ManCounter)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 0.01f)
			{
				m_Timer = 0f;
				m_CanDoSecondaryTask = !m_CanDoSecondaryTask;
				if (m_SecondaryTask == EWorkerTask.Rest)
				{
					m_CanDoSecondaryTask = false;
				}
				if (m_PrimaryTask == EWorkerTask.Rest)
				{
					m_CanDoSecondaryTask = true;
				}
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkingToRestOutside && m_PrimaryTask != EWorkerTask.ManCounter && m_SecondaryTask == EWorkerTask.ManCounter && m_WorkerTask != EWorkerTask.ManCounter)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 0.01f)
			{
				m_Timer = 0f;
				m_CanDoSecondaryTask = !m_CanDoSecondaryTask;
				if (m_SecondaryTask == EWorkerTask.Rest)
				{
					m_CanDoSecondaryTask = false;
				}
				if (m_PrimaryTask == EWorkerTask.Rest)
				{
					m_CanDoSecondaryTask = true;
				}
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkingToRestOutside && !m_CanDoSecondaryTask && m_SecondaryTask != EWorkerTask.Rest)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 0.5f && m_FailFindTaskCount < 2)
			{
				m_Timer = 0f;
				m_CanDoSecondaryTask = !m_CanDoSecondaryTask;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkingToRestOutside && m_CanDoSecondaryTask && m_PrimaryTask != EWorkerTask.Rest)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 0.5f && m_FailFindTaskCount < 2)
			{
				m_Timer = 0f;
				m_CanDoSecondaryTask = !m_CanDoSecondaryTask;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkingToRestOutside && !m_CanDoSecondaryTask && m_SecondaryTask == EWorkerTask.Rest && m_CanFindStoredItem)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 0.1f)
			{
				m_Timer = 0f;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.WalkingToRestOutside && m_CanDoSecondaryTask && m_PrimaryTask == EWorkerTask.Rest && m_CanFindStoredItem)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 0.1f)
			{
				m_Timer = 0f;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.SearchingForBox || m_CurrentState == EWorkerState.SearchingForBoxToStore || m_CurrentState == EWorkerState.SearchingForBoxToRefillCleanser || m_CurrentState == EWorkerState.SearchingForBoxToRefillCardOpener)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockTime)
			{
				m_Timer = 0f;
				if (!m_CurrentItemBox)
				{
					DetermineShopAction();
				}
				else if (!m_CurrentItemBox.IsValidObject() || !m_CurrentItemBox.CanWorkerTakeBox())
				{
					m_CurrentItemBox = null;
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.RestockingShelf)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentItemCompartment && (bool)m_CurrentHoldItemBox && (bool)m_CurrentShelf && m_CurrentShelf.IsValidObject())
				{
					if (m_CurrentHoldItemBox.IsBoxOpened())
					{
						m_CurrentHoldItemBox.DispenseItem(isPlayer: false, m_CurrentItemCompartment);
						AddExp(EWorkerTask.RestockShelf, 1);
						if (!m_CurrentItemCompartment.HasEnoughSlot() || m_CurrentHoldItemBox.m_ItemCompartment.GetItemType() != m_CurrentItemCompartment.GetItemType())
						{
							if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() > 0)
							{
								AttemptFindShelf();
							}
							else
							{
								AttemptFindTrashBin();
							}
						}
						else if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() == 0)
						{
							AttemptFindTrashBin();
						}
					}
					else
					{
						m_CurrentHoldItemBox.SetOpenCloseBox(isOpen: true, isPlayer: false);
					}
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.RefillingCleanser)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentHoldItemBox && (bool)m_TargetRefillCleanser && m_TargetRefillCleanser.IsValidObject())
				{
					if (m_CurrentHoldItemBox.IsBoxOpened())
					{
						if (!m_TargetRefillCleanser.HasEnoughSlot() || m_CurrentHoldItemBox.m_ItemCompartment.GetItemType() != EItemType.Deodorant)
						{
							if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() > 0)
							{
								AttemptRefillCleanser();
							}
							else
							{
								AttemptFindTrashBin();
							}
						}
						else if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() == 0)
						{
							AttemptFindTrashBin();
						}
						else
						{
							m_TargetRefillCleanser.DispenseItemFromBox(m_CurrentHoldItemBox, isPlayer: false);
							AddExp(EWorkerTask.RefillCleanser, 20);
						}
					}
					else
					{
						m_CurrentHoldItemBox.SetOpenCloseBox(isOpen: true, isPlayer: false);
					}
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.RefillingCardOpener)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentHoldItemBox && (bool)m_TargetRefillCardOpener && m_TargetRefillCardOpener.IsValidObject() && !m_TargetRefillCardOpener.GetIsProcessing())
				{
					if (m_CurrentHoldItemBox.IsBoxOpened())
					{
						if (!m_TargetRefillCardOpener.HasEnoughSlot() || !m_CardPackItemTypeList.Contains(m_CurrentHoldItemBox.GetItemType()))
						{
							if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() > 0)
							{
								AttemptRefillCardOpener();
							}
							else
							{
								AttemptFindTrashBin();
							}
						}
						else if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() == 0)
						{
							AttemptFindTrashBin();
						}
						else
						{
							m_TargetRefillCardOpener.DispenseItemFromBox(m_CurrentHoldItemBox, isPlayer: false);
							AddExp(EWorkerTask.RefillCardOpener, 1);
						}
					}
					else
					{
						m_CurrentHoldItemBox.SetOpenCloseBox(isOpen: true, isPlayer: false);
					}
				}
				else if ((bool)m_CurrentHoldItemBox && m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() == 0)
				{
					AttemptFindTrashBin();
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.RestockCardWaiting)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockCardWaitTime)
			{
				m_Timer = 0f;
				DetermineShopAction();
			}
		}
		else if (m_CurrentState == EWorkerState.TakingCardStorageShelfCard)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentCardStorageShelf && m_CurrentCardStorageShelf.IsValidObject() && m_CurrentCardStorageShelf.GetTotalCardAmount() > 0 && m_CurrentCardStorageShelf.CanWorkerTake())
				{
					PlayWorkerActionAnim();
					m_CardTakenForRestockCount = 8;
					SetState(EWorkerState.RestockCardWaiting);
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.RestockingCardTable)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentCardStorageShelf && (!m_CurrentCardStorageShelf.IsValidObject() || m_CurrentCardStorageShelf.GetTotalCardAmount() == 0 || !m_CurrentCardStorageShelf.CanWorkerTake()))
				{
					m_CardTakenForRestockCount = 0;
				}
				if (m_CardTakenForRestockCount > 0 && (bool)m_CurrentCardShelf && (bool)m_CurrentCardCompartment && m_CurrentCardShelf.IsValidObject())
				{
					if ((bool)m_CurrentCardStorageShelf && m_CurrentCardCompartment.m_StoredCardList.Count <= 0)
					{
						InteractableCard3d randomCard = m_CurrentCardStorageShelf.GetRandomCard();
						if ((bool)randomCard)
						{
							m_FailFindTaskCount = 0;
							PlayWorkerActionAnim();
							randomCard.transform.position = m_WristBoneL.transform.position;
							randomCard.transform.rotation = m_WristBoneL.transform.rotation;
							m_CurrentCardCompartment.SetCardOnShelf(randomCard);
							m_CardTakenForRestockCount--;
							AddExp(EWorkerTask.RestockCardDisplay, 10);
							if (m_CurrentCardStorageShelf.GetTotalCardAmount() == 0)
							{
								m_CardTakenForRestockCount = 0;
							}
							if (m_PrimaryTask != EWorkerTask.Rest && m_SecondaryTask != EWorkerTask.Rest)
							{
								EvaluateSwitchFromSecondaryToPrimaryTask();
								if (!m_CanDoSecondaryTask)
								{
									m_CanFindStoredItem = true;
								}
							}
							bool flag = false;
							CardData cardData = randomCard.m_Card3dUI.m_CardUI.GetCardData();
							if (cardData.cardGrade <= 0)
							{
								for (int i = 0; i < m_CardPriceChangedList.Count; i++)
								{
									if (m_CardPriceChangedList[i].IsSameCardDataType(cardData))
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								SetState(EWorkerState.SettingCardPrice);
							}
							else
							{
								SetState(EWorkerState.RestockCardWaiting);
							}
						}
						else
						{
							m_UnableToFindQueue = true;
							GoRestPoint();
						}
					}
					else
					{
						DetermineShopAction();
					}
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.SettingShelfPrice)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_ScanItemTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentItemCompartment && (bool)m_CurrentShelf && m_CurrentShelf.IsValidObject())
				{
					if (!m_CurrentItemCompartment.HasSetPrice() && m_CurrentItemCompartment.GetItemType() != EItemType.None)
					{
						float num = CPlayerData.GetItemMarketPrice(m_CurrentItemCompartment.GetItemType()) * m_SetPriceMultiplier;
						if (m_IsRoundUpPrice)
						{
							num = ((CSingleton<CGameManager>.Instance.m_CurrencyType != EMoneyCurrencyType.Yen) ? ((float)Mathf.RoundToInt(num * GameInstance.GetCurrencyConversionRate())) : ((float)(Mathf.RoundToInt(num * GameInstance.GetCurrencyConversionRate() / 100f) * 100)));
							num /= GameInstance.GetCurrencyConversionRate();
							num = (float)Mathf.RoundToInt(num * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
						}
						CPlayerData.SetItemPrice(m_CurrentItemCompartment.GetItemType(), num);
					}
					PlayWorkerActionAnim();
					AddExp(EWorkerTask.SetPrice, 10);
					SetState(EWorkerState.LookForItemPriceToSet);
					if (m_PrimaryTask != EWorkerTask.Rest && m_SecondaryTask != EWorkerTask.Rest)
					{
						EvaluateSwitchFromSecondaryToPrimaryTask();
						if (!m_CanDoSecondaryTask)
						{
							DetermineShopAction();
						}
					}
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.SettingCardPrice)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_ScanItemTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentCardCompartment && (bool)m_CurrentCardCompartment.GetCardShelf() && m_CurrentCardCompartment.GetCardShelf().IsValidObject())
				{
					if (!m_CurrentCardCompartment.HasSetPrice() && m_CurrentCardCompartment.m_StoredCardList.Count > 0)
					{
						CardData cardData2 = m_CurrentCardCompartment.m_StoredCardList[0].m_Card3dUI.m_CardUI.GetCardData();
						float num2 = CPlayerData.GetCardMarketPrice(cardData2) * m_SetCardPriceMultiplier;
						if (m_IsRoundUpCardPrice)
						{
							num2 = ((CSingleton<CGameManager>.Instance.m_CurrencyType != EMoneyCurrencyType.Yen) ? ((float)Mathf.RoundToInt(num2 * GameInstance.GetCurrencyConversionRate())) : ((float)(Mathf.RoundToInt(num2 * GameInstance.GetCurrencyConversionRate() / 100f) * 100)));
							num2 /= GameInstance.GetCurrencyConversionRate();
							num2 = (float)Mathf.RoundToInt(num2 * GameInstance.GetCurrencyRoundDivideAmount()) / GameInstance.GetCurrencyRoundDivideAmount();
						}
						if (m_CardPriceChangedList.Count > 100)
						{
							m_CardPriceChangedList.RemoveAt(0);
						}
						m_CardPriceChangedList.Add(cardData2);
						CPlayerData.SetCardPrice(cardData2, num2);
					}
					PlayWorkerActionAnim();
					AddExp(EWorkerTask.SetPrice, 10);
					SetState(EWorkerState.RestockCardWaiting);
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.LookForItemPriceToSet)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 1f)
			{
				m_Timer = 0f;
				bool flag2 = false;
				for (int j = 0; j < m_CurrentShelf.GetItemCompartmentList().Count; j++)
				{
					if (m_CurrentShelf.IsValidObject() && !m_CurrentShelf.GetItemCompartmentList()[j].HasSetPrice() && m_CurrentShelf.GetItemCompartmentList()[j].GetItemType() != EItemType.None)
					{
						m_CurrentItemCompartment = m_CurrentShelf.GetItemCompartmentList()[j];
						m_TargetTransform = m_CurrentItemCompartment.m_CustomerStandLoc;
						m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
						m_IsWaitingForPathCallback = true;
						m_UnableToFindQueue = false;
						m_ReachedEndOfPath = false;
						SetState(EWorkerState.WalkToShelfSetPrice);
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					EvaluateSwitchFromSecondaryToPrimaryTask();
					SetState(EWorkerState.Idle);
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.LookForCardPriceToSet)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > 1f)
			{
				m_Timer = 0f;
				bool flag3 = false;
				CardShelf cardShelf = m_CurrentCardCompartment.GetCardShelf();
				if (cardShelf.IsValidObject())
				{
					for (int k = 0; k < cardShelf.GetCardCompartmentList().Count; k++)
					{
						if (!cardShelf.GetCardCompartmentList()[k].HasSetPrice() && cardShelf.GetCardCompartmentList()[k].m_StoredCardList.Count > 0)
						{
							m_CurrentCardCompartment = cardShelf.GetCardCompartmentList()[k];
							m_TargetTransform = m_CurrentCardCompartment.m_CustomerStandLoc;
							m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
							m_IsWaitingForPathCallback = true;
							m_UnableToFindQueue = false;
							m_ReachedEndOfPath = false;
							SetState(EWorkerState.WalkToCardShelfSetPrice);
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					EvaluateSwitchFromSecondaryToPrimaryTask();
					SetState(EWorkerState.Idle);
					DetermineShopAction();
				}
			}
		}
		else if (m_CurrentState == EWorkerState.StoreBoxOnWarehouseShelf)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_RestockTime)
			{
				m_Timer = 0f;
				if ((bool)m_CurrentItemCompartment && (bool)m_CurrentHoldItemBox && (bool)m_CurrentWarehouseShelf && m_CurrentWarehouseShelf.IsValidObject())
				{
					m_CurrentHoldItemBox.DispenseItem(isPlayer: false, m_CurrentItemCompartment);
					if (m_CurrentHoldItemBox.m_IsStored)
					{
						AddExp(m_WorkerTask, 10);
						m_CurrentHoldItemBox = null;
						DropBoxOnHand();
						if (LightManager.GetHasDayEnded())
						{
							SetTask(EWorkerTask.GoBackHome);
						}
						EvaluateSwitchFromSecondaryToPrimaryTask();
						DetermineShopAction();
					}
					else if (!m_CurrentItemCompartment.HasEnoughSlot() || m_CurrentHoldItemBox.m_ItemCompartment.GetItemType() != m_CurrentItemCompartment.GetItemType())
					{
						if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() > 0)
						{
							AttemptFindWarehouseShelf();
						}
						else if (m_CurrentHoldItemBox.m_ItemCompartment.GetItemCount() == 0)
						{
							AttemptFindTrashBin();
						}
					}
				}
				else
				{
					DetermineShopAction();
				}
			}
		}
		else
		{
			if (m_CurrentState == EWorkerState.ReadyToTrashBox)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > Mathf.Clamp(m_RestockTime, 0.5f, 2f))
				{
					m_Timer = 0f;
					if ((bool)m_CurrentTrashBin && m_CurrentTrashBin.IsValidObject())
					{
						if ((bool)m_CurrentHoldItemBox)
						{
							m_CurrentTrashBin.DiscardBox(m_CurrentHoldItemBox, isPlayer: false);
							m_CurrentTrashBin = null;
							m_CurrentHoldItemBox = null;
							DropBoxOnHand();
						}
						if (LightManager.GetHasDayEnded())
						{
							SetTask(EWorkerTask.GoBackHome);
						}
						EvaluateSwitchFromSecondaryToPrimaryTask();
						DetermineShopAction();
					}
					else
					{
						AttemptFindTrashBin();
					}
				}
				return true;
			}
			if (m_CurrentState == EWorkerState.ReadyToStoreEmptyBox)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer > Mathf.Clamp(m_RestockTime, 0.5f, 2f))
				{
					m_Timer = 0f;
					if ((bool)m_CurrentEmptyBoxStorage && m_CurrentEmptyBoxStorage.IsValidObject() && m_CurrentEmptyBoxStorage.HasStorageSpace())
					{
						if ((bool)m_CurrentHoldItemBox)
						{
							m_HasAttemptFindEmptyBoxStorage = false;
							m_CurrentEmptyBoxStorage.StoreBox(m_CurrentHoldItemBox, isPlayer: false);
							m_CurrentEmptyBoxStorage = null;
							m_CurrentHoldItemBox = null;
							DropBoxOnHand();
						}
						if (LightManager.GetHasDayEnded())
						{
							SetTask(EWorkerTask.GoBackHome);
						}
						EvaluateSwitchFromSecondaryToPrimaryTask();
						DetermineShopAction();
					}
					else
					{
						AttemptFindTrashBin();
					}
				}
				return true;
			}
			if (m_CurrentState == EWorkerState.ManningCounter)
			{
				if (LightManager.GetHasDayEnded())
				{
					m_Timer += Time.deltaTime;
					if (m_Timer > 10f)
					{
						m_Timer = 0f;
						if (!CSingleton<CustomerManager>.Instance.HasCustomerInShop())
						{
							SetTask(EWorkerTask.GoBackHome);
							DetermineShopAction();
						}
					}
				}
				else if (m_PrimaryTask == EWorkerTask.ManCounter && m_SecondaryTask != EWorkerTask.Rest && m_SecondaryTask != EWorkerTask.ManCounter)
				{
					m_Timer += Time.deltaTime;
					if (m_Timer > 10f)
					{
						m_Timer = 0f;
						if ((bool)m_CurrentCashierCounter && m_CurrentCashierCounter.GetCustomerInQueueCount() == 0)
						{
							m_Timer = 0f;
							m_CanDoSecondaryTask = true;
							m_CurrentCashierCounter.StopCurrentWorker();
						}
					}
				}
				else if (m_SecondaryTask == EWorkerTask.ManCounter && m_PrimaryTask != EWorkerTask.Rest && m_PrimaryTask != EWorkerTask.ManCounter)
				{
					m_Timer += Time.deltaTime;
					if (m_Timer > 10f)
					{
						m_Timer = 0f;
						if ((bool)m_CurrentCashierCounter && m_CurrentCashierCounter.GetCustomerInQueueCount() == 0)
						{
							m_Timer = 0f;
							EvaluateSwitchFromSecondaryToPrimaryTask();
							m_CurrentCashierCounter.StopCurrentWorker();
						}
					}
				}
			}
		}
		return false;
	}

	private void AttemptFindBoxForRestock()
	{
		m_ReachedEndOfPath = false;
		m_CurrentItemBox = null;
		List<InteractablePackagingBox_Item> itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: false);
		List<InteractablePackagingBox_Item> list = new List<InteractablePackagingBox_Item>();
		if (itemPackagingBoxListWithItem.Count == 0 || m_CanFindStoredItem)
		{
			itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: true);
			m_CanFindStoredItem = false;
		}
		for (int num = itemPackagingBoxListWithItem.Count - 1; num >= 0; num--)
		{
			if (!itemPackagingBoxListWithItem[num].IsValidObject() || !itemPackagingBoxListWithItem[num].CanWorkerTakeBox() || (!itemPackagingBoxListWithItem[num].m_IsStored && itemPackagingBoxListWithItem[num].transform.position.y > 2f))
			{
				itemPackagingBoxListWithItem.RemoveAt(num);
			}
		}
		for (int i = 0; i < itemPackagingBoxListWithItem.Count; i++)
		{
			if ((m_TargetBoxItemType != EItemType.None && m_TargetBoxItemType != itemPackagingBoxListWithItem[i].m_ItemCompartment.GetItemType()) || (m_TargetBoxSize == 1 && itemPackagingBoxListWithItem[i].m_IsBigBox) || (m_TargetBoxSize == 2 && !itemPackagingBoxListWithItem[i].m_IsBigBox))
			{
				continue;
			}
			if (WorkerManager.GetActiveWorkerCount() == 1)
			{
				list.Add(itemPackagingBoxListWithItem[i]);
				continue;
			}
			for (int j = 0; j < WorkerManager.GetWorkerList().Count; j++)
			{
				if (WorkerManager.GetWorkerList()[j] != this && WorkerManager.GetWorkerList()[j].IsActive() && WorkerManager.GetWorkerList()[j].GetCurrentItemBox() != itemPackagingBoxListWithItem[i] && m_LastStoredItemBox != itemPackagingBoxListWithItem[i])
				{
					list.Add(itemPackagingBoxListWithItem[i]);
				}
			}
		}
		if (list.Count > 0)
		{
			int num2 = 1000;
			int index = 0;
			for (int k = 0; k < list.Count; k++)
			{
				if (Mathf.RoundToInt((float)list[k].m_ItemCompartment.GetItemCount() / (float)list[k].m_ItemCompartment.GetMaxItemCount() * 100f) < num2)
				{
					index = k;
				}
			}
			m_CurrentItemBox = list[index];
		}
		if ((bool)m_CurrentItemBox)
		{
			m_TargetTransform = m_CurrentItemBox.transform;
			if ((bool)m_CurrentItemBox.GetBoxStoredCompartment() && (bool)m_CurrentItemBox.GetBoxStoredCompartment().m_CustomerStandLoc)
			{
				m_TargetTransform = m_CurrentItemBox.GetBoxStoredCompartment().m_CustomerStandLoc.transform;
			}
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(EWorkerState.SearchingForBox);
		}
		else
		{
			m_TargetTransform = null;
			GoRestPoint();
			m_CanFindStoredItem = true;
		}
	}

	private void AttemptFindBoxToStore()
	{
		m_ReachedEndOfPath = false;
		m_CurrentItemBox = null;
		List<InteractablePackagingBox_Item> itemPackagingBoxListWithItem = RestockManager.GetItemPackagingBoxListWithItem(includeStoredItem: false);
		List<InteractablePackagingBox_Item> list = new List<InteractablePackagingBox_Item>();
		for (int num = itemPackagingBoxListWithItem.Count - 1; num >= 0; num--)
		{
			if (!itemPackagingBoxListWithItem[num].IsValidObject() || !itemPackagingBoxListWithItem[num].CanWorkerTakeBox() || (!itemPackagingBoxListWithItem[num].m_IsStored && itemPackagingBoxListWithItem[num].transform.position.y > 2f))
			{
				itemPackagingBoxListWithItem.RemoveAt(num);
			}
		}
		for (int i = 0; i < itemPackagingBoxListWithItem.Count; i++)
		{
			if ((m_TargetBoxItemType != EItemType.None && m_TargetBoxItemType != itemPackagingBoxListWithItem[i].m_ItemCompartment.GetItemType()) || (m_TargetBoxSize == 1 && itemPackagingBoxListWithItem[i].m_IsBigBox) || (m_TargetBoxSize == 2 && !itemPackagingBoxListWithItem[i].m_IsBigBox))
			{
				continue;
			}
			if (WorkerManager.GetActiveWorkerCount() == 1)
			{
				list.Add(itemPackagingBoxListWithItem[i]);
				continue;
			}
			for (int j = 0; j < WorkerManager.GetWorkerList().Count; j++)
			{
				if (WorkerManager.GetWorkerList()[j] != this && WorkerManager.GetWorkerList()[j].IsActive() && WorkerManager.GetWorkerList()[j].GetCurrentItemBox() != itemPackagingBoxListWithItem[i] && m_LastStoredItemBox != itemPackagingBoxListWithItem[i])
				{
					list.Add(itemPackagingBoxListWithItem[i]);
				}
			}
		}
		if (list.Count > 0)
		{
			int num2 = 0;
			int index = 0;
			for (int k = 0; k < list.Count; k++)
			{
				if (Mathf.RoundToInt((float)list[k].m_ItemCompartment.GetItemCount() / (float)list[k].m_ItemCompartment.GetMaxItemCount() * 100f) > num2)
				{
					index = k;
				}
			}
			m_CurrentItemBox = list[index];
		}
		if ((bool)m_CurrentItemBox)
		{
			m_TargetTransform = m_CurrentItemBox.transform;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			SetState(EWorkerState.SearchingForBoxToStore);
			return;
		}
		m_TargetTransform = null;
		if (list.Count == 0)
		{
			if (m_HaveValidItemToRestock)
			{
				m_CanFindStoredItem = false;
				m_HaveValidItemToRestock = false;
			}
		}
		else
		{
			m_CanFindStoredItem = true;
		}
		GoRestPoint();
	}

	private void AttemptFindCashierCounter()
	{
		if (!m_CurrentCashierCounter)
		{
			m_ReachedEndOfPath = false;
			m_CurrentCashierCounter = ShelfManager.GetUnmannedCashierCounter();
			if ((bool)m_CurrentCashierCounter)
			{
				m_TargetTransform = m_CurrentCashierCounter.m_LockPlayerPos;
				m_CurrentCashierCounter.SetCurrentWorker(this);
				m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
				m_IsWaitingForPathCallback = true;
				m_UnableToFindQueue = false;
				SetState(EWorkerState.WalkingToCounter);
			}
			else
			{
				m_TargetTransform = null;
				GoRestPoint();
				m_CanFindStoredItem = false;
			}
		}
	}

	private void AttemptFindTrashBin()
	{
		if (!m_HasAttemptFindEmptyBoxStorage && (bool)m_CurrentHoldItemBox && m_CurrentHoldItemBox.m_IsBigBox)
		{
			m_HasAttemptFindEmptyBoxStorage = true;
			m_CurrentEmptyBoxStorage = ShelfManager.GetEmptyBoxStorage(base.transform.position);
			if (m_CurrentEmptyBoxStorage != null)
			{
				m_Path = null;
				SetState(EWorkerState.StoringEmptyBox);
				m_ReachedEndOfPath = false;
				m_TargetTransform = m_CurrentEmptyBoxStorage.transform;
				m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
				m_IsWaitingForPathCallback = true;
				m_UnableToFindQueue = false;
				return;
			}
			m_HasAttemptFindEmptyBoxStorage = false;
		}
		m_Path = null;
		SetState(EWorkerState.TrashingBox);
		m_ReachedEndOfPath = false;
		m_CurrentTrashBin = ShelfManager.GetClosestTrashBin(base.transform.position);
		if (m_CurrentTrashBin != null)
		{
			m_TargetTransform = m_CurrentTrashBin.transform;
		}
		else
		{
			m_TargetTransform = CSingleton<WorkerManager>.Instance.m_TrashBin.transform;
		}
		m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
		m_IsWaitingForPathCallback = true;
		m_UnableToFindQueue = false;
	}

	public bool CheckWorkerSameTarget(Transform targetTransform)
	{
		if (m_TargetTransform == null || targetTransform == null)
		{
			return false;
		}
		if (m_TargetTransform == targetTransform)
		{
			return true;
		}
		if ((m_TargetTransform.position - targetTransform.position).magnitude < 0.1f)
		{
			return true;
		}
		return false;
	}

	private void DropBoxOnHand()
	{
		if ((bool)m_CurrentHoldItemBox)
		{
			m_CurrentHoldItemBox.DropBox(isPlayer: false);
		}
		m_CurrentHoldItemBox = null;
		m_Anim.SetBool("IsHoldingBox", value: false);
	}

	private void AttemptFindShelf()
	{
		SetState(EWorkerState.FindingShelfToRestock);
		m_ReachedEndOfPath = false;
		m_CurrentShelf = null;
		m_CurrentItemCompartment = null;
		bool flag = false;
		bool ignoreNoneType = true;
		List<Shelf> shelfListToRestockItem = ShelfManager.GetShelfListToRestockItem(m_CurrentHoldItemBox.m_ItemCompartment.GetItemType(), ignoreNoneType);
		for (int i = 0; i < WorkerManager.GetWorkerList().Count; i++)
		{
			if (WorkerManager.GetWorkerList()[i] != this)
			{
				WorkerManager.GetWorkerList()[i].IsActive();
			}
		}
		if (m_IsFillShelfWithoutLabel)
		{
			ignoreNoneType = false;
			shelfListToRestockItem = ShelfManager.GetShelfListToRestockItem(m_CurrentHoldItemBox.m_ItemCompartment.GetItemType(), ignoreNoneType);
		}
		List<ShelfCompartment> list = new List<ShelfCompartment>();
		List<ShelfCompartment> list2 = new List<ShelfCompartment>();
		for (int j = 0; j < shelfListToRestockItem.Count; j++)
		{
			list = shelfListToRestockItem[j].GetNonFullItemCompartmentList(m_CurrentHoldItemBox.m_ItemCompartment.GetItemType(), ignoreNoneType);
			for (int k = 0; k < list.Count; k++)
			{
				if (WorkerManager.GetActiveWorkerCount() == 1)
				{
					flag = true;
					list2.Add(list[k]);
				}
				else
				{
					bool flag2 = false;
					for (int l = 0; l < WorkerManager.GetWorkerList().Count; l++)
					{
						if (WorkerManager.GetWorkerList()[l] != this && WorkerManager.GetWorkerList()[l].IsActive() && WorkerManager.GetWorkerList()[l].CheckWorkerSameTarget(list[k].m_CustomerStandLoc))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						list2.Add(list[k]);
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (list2.Count > 0)
		{
			m_CurrentItemCompartment = list2[Random.Range(0, list2.Count)];
			m_CurrentShelf = m_CurrentItemCompartment.GetShelf();
		}
		if ((bool)m_CurrentShelf && (bool)m_CurrentItemCompartment)
		{
			m_TargetTransform = m_CurrentItemCompartment.m_CustomerStandLoc;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			m_ReachedEndOfPath = false;
			SetState(EWorkerState.WalkToShelf);
		}
		else if ((bool)m_CurrentHoldItemBox)
		{
			AttemptFindWarehouseShelf();
		}
		else
		{
			m_UnableToFindQueue = true;
			GoRestPoint();
			m_CanFindStoredItem = false;
		}
	}

	private void AttemptFindWarehouseShelf()
	{
		SetState(EWorkerState.FindingWarehouseShelf);
		m_ReachedEndOfPath = false;
		m_CurrentWarehouseShelf = null;
		m_CurrentItemCompartment = null;
		bool flag = false;
		bool ignoreNoneType = true;
		List<WarehouseShelf> warehouseShelfListToStoreItem = ShelfManager.GetWarehouseShelfListToStoreItem(m_CurrentHoldItemBox.m_ItemCompartment.GetItemType(), m_CurrentHoldItemBox.m_IsBigBox, ignoreNoneType);
		if (m_IsFillShelfWithoutLabel && m_WorkerTask != EWorkerTask.RefillCleanser && m_WorkerTask != EWorkerTask.RefillCardOpener)
		{
			ignoreNoneType = false;
			warehouseShelfListToStoreItem = ShelfManager.GetWarehouseShelfListToStoreItem(m_CurrentHoldItemBox.m_ItemCompartment.GetItemType(), m_CurrentHoldItemBox.m_IsBigBox, ignoreNoneType);
		}
		List<ShelfCompartment> list = new List<ShelfCompartment>();
		List<int> list2 = new List<int>();
		List<ShelfCompartment> list3 = new List<ShelfCompartment>();
		for (int i = 0; i < warehouseShelfListToStoreItem.Count; i++)
		{
			list = warehouseShelfListToStoreItem[i].GetNonFullItemCompartmentList(m_CurrentHoldItemBox.m_ItemCompartment.GetItemType(), m_CurrentHoldItemBox.m_IsBigBox, ignoreNoneType);
			for (int j = 0; j < list.Count; j++)
			{
				if (WorkerManager.GetActiveWorkerCount() == 1)
				{
					flag = true;
					list2.Add(i);
					list3.Add(list[j]);
				}
				else if (0 == 0)
				{
					list2.Add(i);
					list3.Add(list[j]);
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (list3.Count > 0)
		{
			int index = Random.Range(0, list3.Count);
			m_CurrentWarehouseShelf = warehouseShelfListToStoreItem[list2[index]];
			m_CurrentItemCompartment = list3[index];
		}
		if ((bool)m_CurrentWarehouseShelf && (bool)m_CurrentItemCompartment)
		{
			m_TargetTransform = m_CurrentItemCompartment.m_CustomerStandLoc;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			m_ReachedEndOfPath = false;
			SetState(EWorkerState.WalkToWarehouseShelf);
			if ((bool)m_CurrentHoldItemBox && m_CurrentHoldItemBox.IsBoxOpened())
			{
				m_CurrentHoldItemBox.SetOpenCloseBox(isOpen: false, isPlayer: false);
			}
		}
		else
		{
			m_UnableToFindQueue = true;
			DropBoxOnHand();
			GoRestPoint();
			m_CanFindStoredItem = false;
		}
	}

	private void AttemptFindSetPriceShelf()
	{
		m_ReachedEndOfPath = false;
		m_CurrentShelf = null;
		m_CurrentItemCompartment = null;
		List<Shelf> shelfList = ShelfManager.GetShelfList();
		List<ShelfCompartment> list = new List<ShelfCompartment>();
		List<ShelfCompartment> list2 = new List<ShelfCompartment>();
		for (int i = 0; i < shelfList.Count; i++)
		{
			if (!shelfList[i].IsValidObject() || shelfList[i].m_ItemNotForSale)
			{
				continue;
			}
			list = shelfList[i].GetItemCompartmentList();
			for (int j = 0; j < list.Count; j++)
			{
				if (!list[j].HasSetPrice() && list[j].GetItemType() != EItemType.None)
				{
					list2.Add(list[j]);
				}
			}
		}
		if (list2.Count > 0)
		{
			m_CurrentItemCompartment = list2[Random.Range(0, list2.Count)];
			m_CurrentShelf = m_CurrentItemCompartment.GetShelf();
		}
		if ((bool)m_CurrentShelf && (bool)m_CurrentItemCompartment)
		{
			m_TargetTransform = m_CurrentItemCompartment.m_CustomerStandLoc;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			m_ReachedEndOfPath = false;
			SetState(EWorkerState.WalkToShelfSetPrice);
		}
		else
		{
			m_UnableToFindQueue = true;
			GoRestPoint();
		}
	}

	private void AttemptFindSetPriceCardShelf()
	{
		m_ReachedEndOfPath = false;
		m_CurrentCardCompartment = null;
		List<CardShelf> cardShelfList = ShelfManager.GetCardShelfList();
		List<InteractableCardCompartment> list = new List<InteractableCardCompartment>();
		List<InteractableCardCompartment> list2 = new List<InteractableCardCompartment>();
		for (int i = 0; i < cardShelfList.Count; i++)
		{
			if (!cardShelfList[i].IsValidObject() || cardShelfList[i].m_ItemNotForSale)
			{
				continue;
			}
			list = cardShelfList[i].GetCardCompartmentList();
			for (int j = 0; j < list.Count; j++)
			{
				if (!list[j].HasSetPrice() && list[j].m_StoredCardList.Count > 0)
				{
					list2.Add(list[j]);
				}
			}
		}
		if (list2.Count > 0)
		{
			m_CurrentCardCompartment = list2[Random.Range(0, list2.Count)];
		}
		if ((bool)m_CurrentCardCompartment)
		{
			m_TargetTransform = m_CurrentCardCompartment.m_CustomerStandLoc;
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
			m_ReachedEndOfPath = false;
			SetState(EWorkerState.WalkToCardShelfSetPrice);
		}
		else
		{
			m_UnableToFindQueue = true;
			GoRestPoint();
		}
	}

	private void EvaluateSwitchFromSecondaryToPrimaryTask()
	{
		if (m_PrimaryTask != EWorkerTask.Rest && m_SecondaryTask != EWorkerTask.Rest && m_WorkerTask == m_SecondaryTask && m_CanDoSecondaryTask)
		{
			m_CanDoSecondaryTask = false;
		}
	}

	public void PlayWorkerActionAnim()
	{
		m_Anim.SetTrigger("ScanItem");
	}

	public void StopManningCounter()
	{
		m_CurrentCashierCounter = null;
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(DelayStopManningCounter());
		}
	}

	private IEnumerator DelayStopManningCounter()
	{
		yield return new WaitForSeconds(0.1f);
		DetermineShopAction();
	}

	public void GoRestPoint()
	{
		m_FailFindTaskCount++;
		if (m_CurrentState != EWorkerState.WalkingToRestOutside)
		{
			m_Path = null;
			m_ReachedEndOfPath = false;
			if (LightManager.GetHasDayEnded())
			{
				SetTask(EWorkerTask.GoBackHome);
				ExitShop();
				return;
			}
			SetState(EWorkerState.WalkingToRestOutside);
			m_TargetTransform = WorkerManager.GetWorkerRestPoint(m_WorkerIndex);
			m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
			m_IsWaitingForPathCallback = true;
			m_UnableToFindQueue = false;
		}
	}

	public void SetOutOfScreen()
	{
		base.transform.position = Vector3.one * 10000f;
	}

	private void WaypointEndUpdate()
	{
	}

	private void ExitShop()
	{
		m_TargetTransform = CustomerManager.GetRandomExitPoint();
		m_Seeker.StartPath(base.transform.position, m_TargetTransform.position, OnPathComplete);
		m_IsWaitingForPathCallback = true;
		SetState(EWorkerState.ExitingShop);
		m_WorkerCollider.gameObject.SetActive(value: false);
		if (!m_HasUpdatedCustomerCount)
		{
			m_HasUpdatedCustomerCount = true;
			CSingleton<WorkerManager>.Instance.UpdateWorkerCount(-1);
		}
	}

	private IEnumerator DelayExitShop(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		ExitShop();
	}

	private void SetState(EWorkerState state)
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
		m_HoldBoxLoc.position = Vector3.Lerp(m_WristBoneL.position, m_WristBoneR.position, 0.5f);
		m_HoldBoxLoc.position += m_HoldBoxLoc.up * 0.04f;
		if (StateUpdate())
		{
			return;
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
			if (!m_IsInsideShop && m_CurrentState != EWorkerState.ExitingShop)
			{
				m_IsInsideShop = CustomerManager.CheckIsInsideShop(base.transform.position);
				if (!m_IsInsideShop)
				{
				}
			}
			else if (m_IsInsideShop && m_CurrentState == EWorkerState.ExitingShop && CustomerManager.CheckIsInsideShop(base.transform.position))
			{
				m_IsInsideShop = false;
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
		if (m_Path == null)
		{
			return;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, m_Path.vectorPath[m_CurrentWaypoint], m_Speed * m_ExtraSpeedMultiplier * Time.deltaTime);
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

	public void ForceGoHome()
	{
		SetTask(EWorkerTask.GoBackHome);
		DetermineShopAction();
	}

	private void EvaluateTaskLevelStat()
	{
		if (m_WorkerTask != EWorkerTask.Rest && m_WorkerTask != EWorkerTask.Fired && m_WorkerTask != EWorkerTask.GoBackHome)
		{
			int num = 0;
			if (m_IsBonusBoosted)
			{
				num = m_BonusBoostedCount;
			}
			float num2 = 1f - (float)(m_TaskLevel[(int)m_WorkerTask] + num) * 0.08f;
			m_ScanItemTime = m_WorkerData.checkoutSpeed * 1.33f * num2;
			m_GiveChangeTime = m_WorkerData.checkoutSpeed * num2;
			m_RestockTime = m_WorkerData.restockSpeed * num2;
			float num3 = 0f;
			int num4 = 0;
			int num5 = 0;
			for (int i = 1; i < 7; i++)
			{
				num4 += m_TaskLevel[i];
				num5++;
			}
			num3 = (float)num4 / (float)num5;
			m_ExtraSpeedMultiplier = 1f + m_WorkerData.walkSpeedMultiplier + (num3 + (float)num) * 0.03f;
		}
	}

	public void AddExp(EWorkerTask task, int expAddAmount)
	{
		m_ExpList[(int)task] += expAddAmount;
		if (m_IsBonusBoosted)
		{
			m_ExpList[(int)task] += expAddAmount;
		}
		if (m_ExpList[(int)task] > 100000000)
		{
			m_ExpList[(int)task] = 100000000;
		}
		EvaluateSkillLevel();
	}

	public void GiveSalaryBonus()
	{
		m_IsBonusBoosted = true;
		m_BonusBoostedCount++;
		if (m_BonusBoostedCount > 3)
		{
			m_BonusBoostedCount = 3;
		}
		EvaluateTaskLevelStat();
	}

	public int GetBonusBoostedCount()
	{
		return m_BonusBoostedCount;
	}

	private void EvaluateSkillLevel()
	{
		for (int i = 0; i < m_ExpList.Count; i++)
		{
			int num = m_ExpList[i];
			int value = 0;
			if (num >= 70000)
			{
				value = 10;
			}
			else if (num >= 45000)
			{
				value = 9;
			}
			else if (num >= 30000)
			{
				value = 8;
			}
			else if (num >= 20000)
			{
				value = 7;
			}
			else if (num >= 12500)
			{
				value = 6;
			}
			else if (num >= 7000)
			{
				value = 5;
			}
			else if (num >= 3500)
			{
				value = 4;
			}
			else if (num >= 1750)
			{
				value = 3;
			}
			else if (num >= 750)
			{
				value = 2;
			}
			else if (num >= 250)
			{
				value = 1;
			}
			m_TaskLevel[i] = value;
		}
	}

	public int GetTaskSkillLevel(EWorkerTask workerTask)
	{
		return m_TaskLevel[(int)workerTask];
	}

	public bool IsActive()
	{
		return m_IsActive;
	}

	public void SetExtraSpeedMultiplier(float extraSpeedMultiplier)
	{
		m_OriginalSpeedMultiplier = m_ExtraSpeedMultiplier;
		m_ExtraSpeedMultiplier = extraSpeedMultiplier;
	}

	public void ResetExtraSpeedMultiplier()
	{
		m_ExtraSpeedMultiplier = m_OriginalSpeedMultiplier;
	}

	public WorkerSaveData GetWorkerSaveData()
	{
		WorkerSaveData workerSaveData = new WorkerSaveData();
		if (m_CurrentState == EWorkerState.ExitingShop)
		{
			workerSaveData.currentState = m_CurrentState;
		}
		else
		{
			workerSaveData.currentState = EWorkerState.Idle;
		}
		workerSaveData.primaryTask = m_PrimaryTask;
		workerSaveData.secondaryTask = m_SecondaryTask;
		workerSaveData.workerTask = m_WorkerTask;
		workerSaveData.isGoingHome = false;
		if (m_PrimaryTask == EWorkerTask.GoBackHome && LightManager.GetHasDayEnded())
		{
			workerSaveData.isGoingHome = true;
			workerSaveData.primaryTask = m_LastWorkerTask;
			workerSaveData.workerTask = m_LastWorkerTask;
		}
		workerSaveData.pos.SetData(base.transform.position);
		workerSaveData.rot.SetData(base.transform.rotation);
		workerSaveData.isFillShelfWithoutLabel = m_IsFillShelfWithoutLabel;
		workerSaveData.isRoundUpPrice = m_IsRoundUpPrice;
		workerSaveData.isRoundUpCardPrice = m_IsRoundUpCardPrice;
		workerSaveData.setPriceMultiplier = m_SetPriceMultiplier;
		workerSaveData.setCardPriceMultiplier = m_SetCardPriceMultiplier;
		workerSaveData.isBonusBoosted = m_IsBonusBoosted;
		workerSaveData.bonusBoostedCount = m_BonusBoostedCount;
		workerSaveData.cardPackItemTypeEnabledList = m_CardPackItemTypeEnabledList;
		workerSaveData.expList = m_ExpList;
		return workerSaveData;
	}

	public void LoadWorkerSaveData(WorkerSaveData data)
	{
		m_Timer = 0f;
		m_IsFillShelfWithoutLabel = data.isFillShelfWithoutLabel;
		m_IsRoundUpPrice = data.isRoundUpPrice;
		m_SetPriceMultiplier = data.setPriceMultiplier;
		m_IsRoundUpCardPrice = data.isRoundUpCardPrice;
		m_SetCardPriceMultiplier = data.setCardPriceMultiplier;
		m_IsBonusBoosted = data.isBonusBoosted;
		m_BonusBoostedCount = data.bonusBoostedCount;
		if (!m_IsRoundUpPrice && m_SetPriceMultiplier == 0f)
		{
			m_IsRoundUpPrice = true;
			m_SetPriceMultiplier = 1f;
		}
		if (!m_IsRoundUpCardPrice && m_SetCardPriceMultiplier == 0f)
		{
			m_IsRoundUpCardPrice = true;
			m_SetCardPriceMultiplier = 1f;
		}
		if (data.cardPackItemTypeEnabledList != null && data.cardPackItemTypeEnabledList.Count > 0)
		{
			m_CardPackItemTypeEnabledList = data.cardPackItemTypeEnabledList;
		}
		if (data.expList != null && data.expList.Count > 0)
		{
			m_ExpList = data.expList;
		}
		EvaluateSkillLevel();
		m_CurrentState = data.currentState;
		m_PrimaryTask = data.primaryTask;
		m_SecondaryTask = data.secondaryTask;
		m_WorkerTask = data.workerTask;
		if (m_PrimaryTask != data.workerTask && m_SecondaryTask != data.workerTask)
		{
			m_PrimaryTask = data.workerTask;
		}
		if (m_WorkerTask == EWorkerTask.GoBackHome && !LightManager.GetHasDayEnded())
		{
			m_WorkerTask = EWorkerTask.Rest;
		}
		if (m_PrimaryTask == EWorkerTask.GoBackHome && !LightManager.GetHasDayEnded())
		{
			m_PrimaryTask = EWorkerTask.Rest;
		}
		if (m_CurrentState == EWorkerState.ExitingShop && !LightManager.GetHasDayEnded())
		{
			m_CurrentState = EWorkerState.Idle;
		}
		SetLastTask(m_PrimaryTask);
		base.transform.position = data.pos.Data;
		base.transform.rotation = data.rot.Data;
		if (base.transform.position.x > 5000f || base.transform.position.y > 5000f)
		{
			m_TargetTransform = CustomerManager.GetRandomExitPoint();
			base.transform.position = m_TargetTransform.position;
			base.transform.rotation = m_TargetTransform.rotation;
			m_TargetLerpRotation = m_TargetTransform.rotation;
		}
	}

	public InteractablePackagingBox_Item GetCurrentItemBox()
	{
		return m_CurrentItemBox;
	}

	public bool GetIsRoundUpPrice()
	{
		return m_IsRoundUpPrice;
	}

	public float GetPriceMultiplier()
	{
		return m_SetPriceMultiplier;
	}

	public bool GetIsRoundUpCardPrice()
	{
		return m_IsRoundUpCardPrice;
	}

	public float GetCardPriceMultiplier()
	{
		return m_SetCardPriceMultiplier;
	}

	public void SetTaskSettingPrimarySecondary(bool isPrimary)
	{
		m_IsSetTaskSettingPrimarySecondary = isPrimary;
	}

	public bool GetIsSetTaskSettingPrimarySecondary()
	{
		return m_IsSetTaskSettingPrimarySecondary;
	}

	public List<bool> GetCardPackItemTypeEnabledList()
	{
		return m_CardPackItemTypeEnabledList;
	}

	public void SetCardPackItemTypeEnabled(int index, bool cardPackItemTypeEnabledList)
	{
		m_CardPackItemTypeEnabledList[index] = cardPackItemTypeEnabledList;
	}
}
