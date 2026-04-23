using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;

public class InteractionPlayerController : CSingleton<InteractionPlayerController>
{
	public static InteractionPlayerController m_Instance;

	public EGameState m_CurrentGameState;

	public Collider m_PlayerCollider;

	public Rigidbody m_PlayerRigidbody;

	public Camera m_Cam;

	public float m_RayDistance = 5f;

	public float m_MouseHoldAutoFireRate = 0.15f;

	public float m_MoveObjectRotateSpeed = 5f;

	public Transform m_LookAtTransform;

	public Transform m_CamWorldPos;

	public Transform m_HoldItemPos;

	public Transform m_HoldBigItemPos;

	public Transform m_HoldBigItemPosUnscaled;

	public Transform m_HoldCardPackPos;

	public Transform m_HoldCardAlbumPos;

	public Transform m_HoldCardPos;

	public Transform m_HoldCardGrpRotation;

	public List<Transform> m_HoldCardPosList;

	public Transform m_HoldCardCloseUpPos;

	public Transform m_OpenCardPackPos;

	public Transform m_HoldDeodorantPos;

	public ParticleSystem m_DeodorantSprayVFX;

	public ParticleSystem m_SellShelfVFX;

	public List<Transform> m_HoldCardPackPosList;

	public List<Transform> m_OpenCardBoxSpawnCardPackPosList;

	public Animation m_OpenCardBoxInnerMesh;

	public MeshFilter m_OpenCardBoxMeshFilter;

	public MeshRenderer m_OpenCardBoxMesh;

	public AdvancedWalkerController m_WalkerCtrl;

	public CameraController m_CameraController;

	public CameraMouseInput m_CameraMouseInput;

	public CameraFOVControl m_CameraFOVController;

	public MaterialFadeInOut m_BlackBGWorldUIFade;

	public float m_HideCardAlbumTime = 0.6f;

	public CollectionBinderFlipAnimCtrl m_CollectionBinderFlipAnimCtrl;

	public InputTooltipListDisplay m_InputTooltipListDisplay;

	public ConfirmTrashScreen m_ConfirmTrashScreen;

	public ConfirmTrashCardScreen m_ConfirmTrashCardScreen;

	public ConfirmGoNextDayScreen m_ConfirmGoNextDayScreen;

	public ConfirmSellShelfScreen m_ConfirmSellShelfScreen;

	public SetCashierCounterSettingScreen m_SetCashierCounterSettingScreen;

	public SetCardStorageShelfSettingScreen m_SetCardStorageShelfSettingScreen;

	public PlaceDecoUIScreen m_PlaceDecoUIScreen;

	public ShowCardObtainedPage m_ShowCardObtainedPage;

	public GradedCardSubmitSelectScreen m_GradedCardSubmitSelectScreen;

	public UI_BarcodeScannerScreen m_UIBarcodeScannerScreen;

	public BulkDonationBoxUIScreen m_BulkDonationBoxUIScreen;

	private Vector3 m_OriginalCameraPos;

	private Vector3 m_CurrentCameraPos;

	private Quaternion m_OriginalCameraRot = Quaternion.identity;

	private Quaternion m_HoldCardGrpTargetRotation = Quaternion.identity;

	private bool m_IsInUIMode;

	private bool m_IsPhoneScreenMode;

	private bool m_IsDecoScreenMode;

	private bool m_IsCashCounterMode;

	private bool m_IsHoldBoxMode;

	private bool m_IsHoldItemMode;

	private bool m_IsHoldCardMode;

	private bool m_IsHoldSprayMode;

	private bool m_IsWorkerInteractMode;

	private bool m_IsViewCardAlbumMode;

	private bool m_IsExitingViewCardAlbumMode;

	private bool m_IsMovingObjectMode;

	private bool m_IsMovingObjectVerticalMode;

	private bool m_IsMovingBoxMode;

	private bool m_IsScanRestockMode;

	private Vector3 m_BoxPhysicsDimension;

	private bool m_IsPuttingCardOnDisplay;

	private bool m_IsSelectingCardForGradingScreen;

	private bool m_IsSelectingCardForEditDeckScreen;

	private bool m_IsSelectingCardForBulkDonationBoxScreen;

	private bool m_CanSelectCardForBulkDonationBoxScreen;

	private bool m_IsHoldingMouseDown;

	private bool m_IsHoldingRightMouseDown;

	private bool m_IsResetMousePress;

	private bool m_IsResetRightMousePress;

	private bool m_IsLerpingCameraRot;

	private bool m_IsOpeningCardBox;

	private bool m_CanOpenDecoUI;

	private bool m_IsStopSnapWhenMoving;

	private bool m_HasEvaluatedDpadQuickSelectionRaycastObj;

	private bool m_IsSprinting;

	private bool m_IsCrouching;

	private bool m_IsPlayTableGameMode;

	public bool m_IsPlayingTopDownGameMode;

	private Quaternion m_LerpCameraTargetRot;

	private Transform m_LookAtTarget;

	private Transform m_CameraWorldFollowTarget;

	private float m_LookAtDelayTimer;

	private float m_LookAtDelayTime;

	private float m_LerpCameraRotSpeed = 1f;

	public float m_MouseDownTime;

	private float m_RightMouseDownTime;

	private float m_MouseDownTimeTotal;

	private float m_TargetCameraPosX;

	private float m_TargetCameraPosY;

	private float m_TargetCameraPosZ;

	private float m_CurrentCameraPosX;

	private float m_CurrentCameraPosY;

	private float m_CurrentCameraPosZ;

	private float m_CameraBlendPosTimer;

	private InteractableObject m_CurrentRaycastObject;

	private InteractablePackagingBox m_CurrentHoldingBox;

	private InteractablePackagingBox_Item m_CurrentHoldingItemBox;

	private InteractablePackagingBox_Item m_CurrentRaycastedPackageBoxItem;

	private InteractablePackagingBox_Shelf m_CurrentHoldingBoxShelf;

	private InteractablePackagingBox_Card m_CurrentHoldingBoxCard;

	private ShelfCompartment m_CurrentItemCompartment;

	private WorkerCollider m_HitWorkerCollider;

	private Worker m_HitWorker;

	private InteractableStorageCompartment m_CurrentStorageCompartment;

	private InteractableCardCompartment m_CurrentCardCompartment;

	private InteractableCardCompartment m_CurrentPutOnDisplayCardCompartment;

	private InteractablePlayTable m_CurrentPlayTable;

	private InteractableTrashBin m_CurrentTrashBin;

	private InteractableCashierCounter m_CurrentCashierCounter;

	private InteractableAutoCleanser m_CurrentAutoCleanser;

	private InteractableAutoPackOpener m_CurrentAutoPackOpener;

	private InteractableEmptyBoxStorage m_CurrentEmptyBoxStorage;

	private InteractableWorkbench m_CurrentWorkbench;

	private InteractableCard3d m_CurrentRaycastedCard3d;

	private List<InteractableCard3d> m_CurrentHoldingCard3dList = new List<InteractableCard3d>();

	private List<Item> m_HoldItemList = new List<Item>();

	private Item m_CurrentHoldSprayItem;

	private CardData m_CurrentSelectedCardData;

	private Shelf m_GamepadCurrentHitShelf;

	private CardShelf m_GamepadCurrentHitCardShelf;

	private WarehouseShelf m_GamepadCurrentHitWarehouseShelf;

	private int m_GamepadCurrentQuickSelectIndex = -1;

	private int m_ItemCompartmentMask;

	private int m_interactionMask;

	private void Awake()
	{
		m_ItemCompartmentMask = LayerMask.GetMask("ShopModel", "Physics", "ItemCompartment", "Obstacles");
		m_interactionMask = LayerMask.GetMask("ShopModel", "Physics", "UI", "Obstacles");
		EvaluateInvertedMouse();
	}

	private void Start()
	{
		m_OpenCardBoxInnerMesh.gameObject.SetActive(value: false);
		m_DeodorantSprayVFX.Stop();
		m_SellShelfVFX.Stop();
		HideCursor();
	}

	private void Update()
	{
		if (m_IsMovingObjectMode || m_IsMovingObjectVerticalMode)
		{
			if (InputManager.GetKeyDownAction(EGameAction.DisableSnap))
			{
				m_IsStopSnapWhenMoving = true;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
		{
			CSingleton<ShelfManager>.Instance.SaveInteractableObjectData();
		}
		if (InputManager.GetKeyUpAction(EGameAction.DisableSnap))
		{
			m_IsStopSnapWhenMoving = false;
		}
		if (!m_IsInUIMode && !m_IsPhoneScreenMode && !m_IsDecoScreenMode)
		{
			if (InputManager.GetKeyDownAction(EGameAction.PauseGame) && !CSingleton<SettingScreen>.Instance.m_ScreenGrp.activeSelf && !SettingScreen.IsChangingKeybind())
			{
				PauseScreen.OpenScreen();
				return;
			}
			CSingleton<SettingScreen>.Instance.ManualUpdate();
		}
		if (m_IsHoldCardMode || m_IsViewCardAlbumMode)
		{
			m_HoldCardGrpRotation.transform.localRotation = Quaternion.Lerp(m_HoldCardGrpRotation.transform.localRotation, CSingleton<InteractionPlayerController>.Instance.m_HoldCardGrpTargetRotation, Time.deltaTime * 10f);
		}
		EvaluateCameraLerp();
		if (m_PlayerCollider.transform.position.y <= -0.5f || m_PlayerCollider.transform.position.y >= 5f)
		{
			m_PlayerCollider.transform.position = base.transform.position;
		}
		if (m_IsInUIMode)
		{
			if (m_IsDecoScreenMode && (InputManager.GetKeyUpAction(EGameAction.Decorate) || Input.GetKeyUp(KeyCode.Escape)))
			{
				CloseDecoInventoryScreen();
			}
			return;
		}
		if (!m_IsInUIMode && !m_IsViewCardAlbumMode && !m_IsExitingViewCardAlbumMode && !m_IsCashCounterMode && !m_IsPhoneScreenMode && !m_IsPlayTableGameMode && !CSingleton<SettingScreen>.Instance.m_ScreenGrp.activeSelf && !CSingleton<PauseScreen>.Instance.m_ScreenGrp.activeSelf && !CSingleton<LoadingScreen>.Instance.m_ScreenGrp.activeSelf)
		{
			if (InputManager.GetKeyDownAction(EGameAction.Sprint))
			{
				m_IsSprinting = !m_IsSprinting;
				m_IsCrouching = false;
			}
			else if (InputManager.GetKeyDownAction(EGameAction.Crouch))
			{
				SetIsCrouching(!m_IsCrouching);
			}
			if (CSingleton<CGameManager>.Instance.m_IsHoldToSprint && m_IsSprinting && InputManager.GetKeyUpAction(EGameAction.Sprint))
			{
				m_IsSprinting = false;
				m_IsCrouching = false;
			}
			if (CSingleton<CGameManager>.Instance.m_IsHoldToCrouch && m_IsCrouching && InputManager.GetKeyUpAction(EGameAction.Crouch))
			{
				m_IsSprinting = false;
				m_IsCrouching = false;
			}
		}
		if (CSingleton<PauseScreen>.Instance.m_ScreenGrp.activeSelf || CSingleton<LoadingScreen>.Instance.m_ScreenGrp.activeSelf || (CPlayerData.m_TutorialIndex <= 0 && CPlayerData.m_ShopLevel < 1))
		{
			return;
		}
		if (m_IsPhoneScreenMode)
		{
			if ((InputManager.GetKeyUpAction(EGameAction.ClosePhone) || Input.GetKeyUp(KeyCode.Escape)) && !CSingleton<SettingScreen>.Instance.m_ScreenGrp.activeSelf)
			{
				PhoneManager.ExitPhoneMode();
			}
			else
			{
				CSingleton<SettingScreen>.Instance.ManualUpdate();
			}
		}
		else
		{
			if (CSingleton<SettingScreen>.Instance.m_ScreenGrp.activeSelf)
			{
				return;
			}
			if ((InputManager.GetKeyDownAction(EGameAction.GoNextDay) || Input.GetKeyDown(KeyCode.KeypadEnter)) && LightManager.GetHasDayEnded())
			{
				if (ShelfManager.HasCustomerInCashCounterQueue() && m_ConfirmGoNextDayScreen.CanOpenScreen())
				{
					m_ConfirmGoNextDayScreen.OpenScreen();
					return;
				}
				ShowGoNextDayScreen();
			}
			if (!m_IsResetMousePress && InputManager.GetKeyDownAction(EGameAction.InteractLeft))
			{
				m_IsHoldingMouseDown = true;
			}
			if (InputManager.GetKeyUpAction(EGameAction.InteractLeft))
			{
				m_IsHoldingMouseDown = false;
			}
			if (!m_IsResetRightMousePress && InputManager.GetKeyDownAction(EGameAction.InteractRight))
			{
				m_IsHoldingRightMouseDown = true;
			}
			if (InputManager.GetKeyUpAction(EGameAction.InteractRight))
			{
				m_IsHoldingRightMouseDown = false;
			}
			if (m_IsHoldBoxMode)
			{
				RaycastHoldBoxState();
			}
			else if (m_IsHoldItemMode)
			{
				RaycastHoldItemState();
			}
			else if (m_IsHoldSprayMode)
			{
				RaycastHoldSprayState();
			}
			else if (m_IsWorkerInteractMode)
			{
				RaycastWorkerInteractMode();
			}
			else if (m_IsHoldCardMode)
			{
				RaycastHoldCardState();
			}
			else if (m_IsMovingObjectMode)
			{
				RaycastMovingObjectState();
			}
			else if (m_IsMovingBoxMode)
			{
				RaycastMovingBoxState();
			}
			else if (m_IsCashCounterMode)
			{
				RaycastCashCounterState();
			}
			else if (m_IsViewCardAlbumMode)
			{
				RaycastViewCardAlbumState();
			}
			else if (m_IsScanRestockMode)
			{
				RaycastScannerRestockState();
			}
			else if (!m_IsPlayTableGameMode)
			{
				RaycastNormalState();
				if (InputManager.GetKeyDownAction(EGameAction.Decorate))
				{
					m_CanOpenDecoUI = true;
				}
				if (InputManager.GetKeyUpAction(EGameAction.OpenPhone))
				{
					PhoneManager.EnterPhoneMode();
				}
				else if (InputManager.GetKeyUpAction(EGameAction.Decorate) && m_CanOpenDecoUI)
				{
					m_CanOpenDecoUI = false;
					ShowDecoInventoryScreen();
				}
			}
			if (InputManager.GetKeyUpAction(EGameAction.InteractLeft))
			{
				m_IsResetMousePress = false;
			}
			if (InputManager.GetKeyUpAction(EGameAction.InteractRight))
			{
				m_IsResetRightMousePress = false;
			}
			if (m_IsResetMousePress && !InputManager.GetKeyHoldAction(EGameAction.InteractLeft))
			{
				m_IsResetMousePress = false;
			}
			if (m_IsResetRightMousePress && !InputManager.GetKeyHoldAction(EGameAction.InteractRight))
			{
				m_IsResetRightMousePress = false;
			}
		}
	}

	public void ShowDecoInventoryScreen()
	{
		m_IsDecoScreenMode = true;
		m_PlaceDecoUIScreen.OpenScreen();
	}

	public void CloseDecoInventoryScreen()
	{
		m_IsDecoScreenMode = false;
		m_PlaceDecoUIScreen.CloseScreen();
	}

	public void ShowGoNextDayScreen()
	{
		if (LightManager.GetHasDayEnded())
		{
			if (m_IsCashCounterMode)
			{
				OnExitCashCounterMode();
			}
			ShelfManager.OnPressGoNextDay();
			WorkerManager.OnPressGoNextDay();
			EndOfDayReportScreen.OpenScreen();
		}
	}

	public void ConfirmSellFurniture()
	{
		if ((bool)m_CurrentHoldingBoxShelf)
		{
			FurniturePurchaseData furniturePurchaseData = InventoryBase.GetFurniturePurchaseData(m_CurrentHoldingBoxShelf.GetBoxedObjectType());
			float price = furniturePurchaseData.price;
			furniturePurchaseData.GetName();
			PriceChangeManager.AddTransaction(price / 2f, ETransactionType.SellFurniture, (int)furniturePurchaseData.objectType);
			CEventManager.QueueEvent(new CEventPlayer_AddCoin(price / 2f));
			m_CurrentHoldingBoxShelf.OnDestroyed();
			m_CurrentHoldingBoxShelf = null;
			OnExitHoldBoxMode();
			SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			m_SellShelfVFX.Play();
		}
	}

	private void EvaluateDPadQuickSelectControl()
	{
		if ((m_GamepadCurrentQuickSelectIndex != -1 || m_HasEvaluatedDpadQuickSelectionRaycastObj) && (!CSingleton<InputManager>.Instance.m_IsControllerActive || InputManager.IsMovingLeftThumbstick() || InputManager.IsMovingRightThumbstick() || InputManager.GetKeyDownAction(EGameAction.Jump)))
		{
			m_GamepadCurrentQuickSelectIndex = -1;
			m_HasEvaluatedDpadQuickSelectionRaycastObj = false;
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
		}
		if (!InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Up) && !InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Down) && !InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_L) && !InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_R))
		{
			return;
		}
		RaycastDpadQuickSelectionObject();
		List<Transform> list = new List<Transform>();
		if ((bool)m_GamepadCurrentHitShelf)
		{
			list = m_GamepadCurrentHitShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode);
		}
		else if ((bool)m_GamepadCurrentHitCardShelf)
		{
			list = m_GamepadCurrentHitCardShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode);
		}
		else if ((bool)m_GamepadCurrentHitWarehouseShelf)
		{
			list = m_GamepadCurrentHitWarehouseShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode);
		}
		if (list.Count > 0)
		{
			if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Down) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_R))
			{
				CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
				if (m_GamepadCurrentQuickSelectIndex == -1 || m_HasEvaluatedDpadQuickSelectionRaycastObj)
				{
					m_GamepadCurrentQuickSelectIndex++;
				}
				if (m_GamepadCurrentQuickSelectIndex >= list.Count)
				{
					m_GamepadCurrentQuickSelectIndex = 0;
				}
				if (!list[m_GamepadCurrentQuickSelectIndex].gameObject.activeSelf)
				{
					m_GamepadCurrentQuickSelectIndex++;
					if (m_GamepadCurrentQuickSelectIndex >= list.Count)
					{
						m_GamepadCurrentQuickSelectIndex = 0;
					}
				}
				CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(list[m_GamepadCurrentQuickSelectIndex], 8f);
			}
			else if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Up) || InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_L))
			{
				CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
				if (m_GamepadCurrentQuickSelectIndex == -1)
				{
					m_GamepadCurrentQuickSelectIndex = 0;
				}
				else if (m_HasEvaluatedDpadQuickSelectionRaycastObj)
				{
					m_GamepadCurrentQuickSelectIndex--;
				}
				if (m_GamepadCurrentQuickSelectIndex < 0)
				{
					m_GamepadCurrentQuickSelectIndex = list.Count - 1;
				}
				if (!list[m_GamepadCurrentQuickSelectIndex].gameObject.activeSelf)
				{
					m_GamepadCurrentQuickSelectIndex--;
					if (m_GamepadCurrentQuickSelectIndex < 0)
					{
						m_GamepadCurrentQuickSelectIndex = list.Count - 1;
					}
				}
				CSingleton<InteractionPlayerController>.Instance.StartAimLookAt(list[m_GamepadCurrentQuickSelectIndex], 8f);
			}
		}
		m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
	}

	private void RaycastDpadQuickSelectionObject()
	{
		if (m_HasEvaluatedDpadQuickSelectionRaycastObj)
		{
			return;
		}
		m_GamepadCurrentHitShelf = null;
		m_GamepadCurrentHitWarehouseShelf = null;
		m_GamepadCurrentHitCardShelf = null;
		Ray ray = new Ray(m_Cam.transform.position, m_Cam.transform.forward);
		if (Physics.Raycast(ray, out var hitInfo, m_RayDistance, m_ItemCompartmentMask))
		{
			Transform obj = hitInfo.transform;
			InteractableStorageCompartment component = hitInfo.transform.GetComponent<InteractableStorageCompartment>();
			if ((bool)component)
			{
				m_GamepadCurrentHitWarehouseShelf = component.GetShelfCompartment().GetWarehouseShelf();
				if ((bool)m_GamepadCurrentHitWarehouseShelf)
				{
					m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitWarehouseShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component.GetShelfCompartment().m_GamepadQuickSelectAimLoc.transform);
				}
				m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
			}
			ShelfCompartment component2 = obj.transform.GetComponent<ShelfCompartment>();
			if ((bool)component2)
			{
				m_GamepadCurrentHitShelf = component2.GetShelf();
				if ((bool)m_GamepadCurrentHitShelf)
				{
					m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component2.m_GamepadQuickSelectAimLoc.transform);
				}
				m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
			}
			InteractableCardCompartment component3 = hitInfo.transform.GetComponent<InteractableCardCompartment>();
			if ((bool)component3)
			{
				m_GamepadCurrentHitCardShelf = component3.GetCardShelf();
				if ((bool)m_GamepadCurrentHitCardShelf)
				{
					m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitCardShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component3.m_GamepadQuickSelectAimLoc.transform);
				}
				m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
			}
		}
		if (!Physics.Raycast(ray, out hitInfo, m_RayDistance, m_interactionMask))
		{
			return;
		}
		_ = hitInfo.transform;
		Shelf component4 = hitInfo.transform.GetComponent<Shelf>();
		if ((bool)component4)
		{
			m_GamepadCurrentHitShelf = component4;
		}
		WarehouseShelf component5 = hitInfo.transform.GetComponent<WarehouseShelf>();
		if ((bool)component5)
		{
			m_GamepadCurrentHitWarehouseShelf = component5;
		}
		CardShelf component6 = hitInfo.transform.GetComponent<CardShelf>();
		if ((bool)component6)
		{
			m_GamepadCurrentHitCardShelf = component6;
		}
		InteractablePriceTag component7 = hitInfo.transform.GetComponent<InteractablePriceTag>();
		if ((bool)component7)
		{
			m_GamepadCurrentHitShelf = component7.GetShelf();
			m_GamepadCurrentHitWarehouseShelf = component7.GetWarehouseShelf();
			if ((bool)m_GamepadCurrentHitShelf)
			{
				m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component7.transform);
			}
			if ((bool)m_GamepadCurrentHitWarehouseShelf)
			{
				m_GamepadCurrentQuickSelectIndex = -1;
			}
			if (m_IsScanRestockMode && (bool)m_GamepadCurrentHitWarehouseShelf)
			{
				m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitWarehouseShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component7.transform);
			}
			m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
		}
		InteractableCardPriceTag component8 = hitInfo.transform.GetComponent<InteractableCardPriceTag>();
		if ((bool)component8)
		{
			m_GamepadCurrentHitCardShelf = component8.GetCardShelf();
			if ((bool)m_GamepadCurrentHitCardShelf)
			{
				m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitCardShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component8.transform);
			}
			m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
		}
	}

	private void RaycastNormalState()
	{
		EvaluateDPadQuickSelectControl();
		Ray ray = new Ray(m_Cam.transform.position, m_Cam.transform.forward);
		bool flag = false;
		if (Physics.Raycast(ray, out var hitInfo, m_RayDistance, m_ItemCompartmentMask))
		{
			Transform obj = hitInfo.transform;
			WorkerCollider component = hitInfo.transform.GetComponent<WorkerCollider>();
			if ((bool)component)
			{
				if (m_HitWorkerCollider != component)
				{
					m_HitWorkerCollider = component;
					m_HitWorker = component.m_Worker;
					RemoveToolTip(EGameAction.InteractLeft);
					AddToolTip(EGameAction.InteractLeft);
					m_HitWorkerCollider.OnRaycasted();
				}
			}
			else
			{
				if ((bool)m_HitWorkerCollider)
				{
					RemoveToolTip(EGameAction.InteractLeft);
					m_HitWorkerCollider.OnRaycastEnded();
				}
				m_HitWorkerCollider = null;
				m_HitWorker = null;
			}
			InteractableStorageCompartment component2 = hitInfo.transform.GetComponent<InteractableStorageCompartment>();
			if ((bool)component2)
			{
				ShowControllerQuickSelectTooltip();
				if (m_CurrentStorageCompartment != component2)
				{
					m_CurrentStorageCompartment = component2;
					if (m_CurrentStorageCompartment.GetShelfCompartment().GetItemCount() > 0)
					{
						RemoveToolTip(EGameAction.TakeBox);
						AddToolTip(EGameAction.TakeBox);
					}
				}
			}
			else
			{
				if ((bool)m_CurrentStorageCompartment)
				{
					RemoveToolTip(EGameAction.TakeBox);
				}
				m_CurrentStorageCompartment = null;
			}
			ShelfCompartment component3 = obj.transform.GetComponent<ShelfCompartment>();
			if ((bool)component3)
			{
				if (m_CurrentItemCompartment != component3)
				{
					m_CurrentItemCompartment = component3;
					if (m_CurrentItemCompartment.m_CanPutItem)
					{
						ShowControllerQuickSelectTooltip();
						if (m_CurrentItemCompartment.GetItemCount() > 0)
						{
							RemoveToolTip(EGameAction.TakeItem);
							AddToolTip(EGameAction.TakeItem, isHold: true);
						}
					}
				}
			}
			else if ((bool)m_CurrentItemCompartment)
			{
				RemoveToolTip(EGameAction.QuickSelect);
				RemoveToolTip(EGameAction.TakeItem);
				m_CurrentItemCompartment = null;
			}
			InteractableCardCompartment component4 = hitInfo.transform.GetComponent<InteractableCardCompartment>();
			if ((bool)component4)
			{
				if (m_CurrentCardCompartment != component4)
				{
					m_CurrentCardCompartment = component4;
					RemoveToolTip(EGameAction.PutCard);
					RemoveToolTip(EGameAction.TakeCard);
					ShowControllerQuickSelectTooltip();
					if (m_CurrentCardCompartment.m_StoredCardList.Count > 0)
					{
						AddToolTip(EGameAction.TakeCard);
					}
					else
					{
						AddToolTip(EGameAction.PutCard);
					}
				}
			}
			else if ((bool)m_CurrentCardCompartment)
			{
				RemoveToolTip(EGameAction.QuickSelect);
				RemoveToolTip(EGameAction.PutCard);
				RemoveToolTip(EGameAction.TakeCard);
				m_CurrentCardCompartment = null;
			}
			InteractablePlayTable component5 = hitInfo.transform.GetComponent<InteractablePlayTable>();
			if ((bool)component5)
			{
				if (m_CurrentPlayTable != component5)
				{
					m_CurrentPlayTable = component5;
					RemoveToolTip(EGameAction.ManageEvent);
					AddToolTip(EGameAction.ManageEvent);
				}
			}
			else if ((bool)m_CurrentPlayTable)
			{
				RemoveToolTip(EGameAction.ManageEvent);
				m_CurrentPlayTable = null;
			}
			InteractablePackagingBox_Item component6 = hitInfo.transform.GetComponent<InteractablePackagingBox_Item>();
			if ((bool)component6)
			{
				if (m_CurrentRaycastedPackageBoxItem != component6)
				{
					m_CurrentRaycastedPackageBoxItem = component6;
					RemoveToolTip(EGameAction.TakeBox);
					AddToolTip(EGameAction.TakeBox);
					if (m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetItemCount() > 0)
					{
						RemoveToolTip(EGameAction.TakeItem);
						AddToolTip(EGameAction.TakeItem);
					}
				}
			}
			else if ((bool)m_CurrentRaycastedPackageBoxItem)
			{
				RemoveToolTip(EGameAction.TakeBox);
				RemoveToolTip(EGameAction.TakeItem);
				m_CurrentRaycastedPackageBoxItem = null;
			}
			InteractableAutoCleanser component7 = hitInfo.transform.GetComponent<InteractableAutoCleanser>();
			if ((bool)component7)
			{
				if (m_CurrentAutoCleanser != component7)
				{
					if ((bool)m_CurrentAutoCleanser)
					{
						m_CurrentAutoCleanser.OnRaycastEnded();
					}
					m_CurrentAutoCleanser = component7;
					RemoveToolTip(EGameAction.TurnOff);
					RemoveToolTip(EGameAction.TurnOn);
					if (m_CurrentAutoCleanser.IsTurnedOn())
					{
						AddToolTip(EGameAction.TurnOff);
					}
					else
					{
						AddToolTip(EGameAction.TurnOn);
					}
				}
			}
			else if ((bool)m_CurrentAutoCleanser)
			{
				m_CurrentAutoCleanser.OnRaycastEnded();
				m_CurrentAutoCleanser = null;
				RemoveToolTip(EGameAction.TurnOff);
				RemoveToolTip(EGameAction.TurnOn);
			}
			InteractableAutoPackOpener component8 = hitInfo.transform.GetComponent<InteractableAutoPackOpener>();
			if ((bool)component8)
			{
				if (m_CurrentAutoPackOpener != component8)
				{
					if ((bool)m_CurrentAutoPackOpener)
					{
						m_CurrentAutoPackOpener.OnRaycastEnded();
					}
					m_CurrentAutoPackOpener = component8;
				}
			}
			else if ((bool)m_CurrentAutoPackOpener)
			{
				m_CurrentAutoPackOpener.OnRaycastEnded();
				m_CurrentAutoPackOpener = null;
			}
			InteractableEmptyBoxStorage component9 = hitInfo.transform.GetComponent<InteractableEmptyBoxStorage>();
			if ((bool)component9)
			{
				if (m_CurrentEmptyBoxStorage != component9)
				{
					if ((bool)m_CurrentEmptyBoxStorage)
					{
						m_CurrentEmptyBoxStorage.OnRaycastEnded();
					}
					m_CurrentEmptyBoxStorage = component9;
				}
			}
			else if ((bool)m_CurrentEmptyBoxStorage)
			{
				m_CurrentEmptyBoxStorage.OnRaycastEnded();
				m_CurrentEmptyBoxStorage = null;
			}
			InteractableWorkbench component10 = hitInfo.transform.GetComponent<InteractableWorkbench>();
			if ((bool)component10)
			{
				if (m_CurrentWorkbench != component10)
				{
					if ((bool)m_CurrentWorkbench)
					{
						m_CurrentWorkbench.OnRaycastEnded();
					}
					m_CurrentWorkbench = component10;
					if (m_CurrentWorkbench.m_StoredItemList.Count > 0)
					{
						AddToolTip(EGameAction.TakeItem);
					}
				}
			}
			else if ((bool)m_CurrentWorkbench)
			{
				m_CurrentWorkbench.OnRaycastEnded();
				m_CurrentWorkbench = null;
				RemoveToolTip(EGameAction.TakeItem);
			}
		}
		else
		{
			if ((bool)m_CurrentItemCompartment)
			{
				RemoveToolTip(EGameAction.TakeItem);
			}
			if ((bool)m_CurrentStorageCompartment)
			{
				RemoveToolTip(EGameAction.TakeBox);
			}
			if ((bool)m_CurrentCardCompartment)
			{
				RemoveToolTip(EGameAction.PutCard);
				RemoveToolTip(EGameAction.TakeCard);
			}
			if ((bool)m_CurrentPlayTable)
			{
				RemoveToolTip(EGameAction.ManageEvent);
			}
			if ((bool)m_CurrentRaycastedPackageBoxItem)
			{
				RemoveToolTip(EGameAction.TakeBox);
				RemoveToolTip(EGameAction.TakeItem);
			}
			if ((bool)m_HitWorkerCollider)
			{
				RemoveToolTip(EGameAction.InteractLeft);
				m_HitWorkerCollider.OnRaycastEnded();
			}
			if ((bool)m_CurrentAutoCleanser)
			{
				m_CurrentAutoCleanser.OnRaycastEnded();
				RemoveToolTip(EGameAction.TurnOff);
				RemoveToolTip(EGameAction.TurnOn);
			}
			if ((bool)m_CurrentAutoPackOpener)
			{
				m_CurrentAutoPackOpener.OnRaycastEnded();
			}
			if ((bool)m_CurrentEmptyBoxStorage)
			{
				m_CurrentEmptyBoxStorage.OnRaycastEnded();
				RemoveToolTip(EGameAction.TakeBox);
			}
			if ((bool)m_CurrentWorkbench)
			{
				m_CurrentWorkbench.OnRaycastEnded();
				RemoveToolTip(EGameAction.TakeItem);
			}
			m_CurrentItemCompartment = null;
			m_CurrentStorageCompartment = null;
			m_CurrentCardCompartment = null;
			m_CurrentPlayTable = null;
			m_CurrentRaycastedPackageBoxItem = null;
			m_HitWorkerCollider = null;
			m_HitWorker = null;
			m_CurrentAutoCleanser = null;
			m_CurrentAutoPackOpener = null;
			m_CurrentEmptyBoxStorage = null;
			m_CurrentWorkbench = null;
		}
		if (Physics.Raycast(ray, out hitInfo, m_RayDistance, m_interactionMask))
		{
			_ = hitInfo.transform;
			InteractableObject component11 = hitInfo.transform.GetComponent<InteractableObject>();
			if ((bool)component11)
			{
				if (m_CurrentRaycastObject != component11)
				{
					if ((bool)m_CurrentRaycastObject)
					{
						m_CurrentRaycastObject.OnRaycastEnded();
					}
					m_CurrentRaycastObject = component11;
					component11.OnRaycasted();
				}
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.OnRaycastEnded();
			m_CurrentRaycastObject = null;
		}
		if (!m_IsResetMousePress && InputManager.GetKeyUpAction(EGameAction.InteractLeft))
		{
			if ((bool)m_CurrentEmptyBoxStorage)
			{
				m_CurrentEmptyBoxStorage.OnMouseButtonUp();
				return;
			}
			if ((bool)m_CurrentStorageCompartment)
			{
				m_CurrentStorageCompartment.OnMouseButtonUp();
				return;
			}
			if ((bool)m_CurrentCardCompartment && m_CurrentCardCompartment.m_StoredCardList.Count <= 0)
			{
				m_CurrentPutOnDisplayCardCompartment = m_CurrentCardCompartment;
				SetIsPuttingCardOnDisplay(isPuttingCardOnDisplay: true);
				EnterViewCardAlbumMode();
				return;
			}
			if ((bool)m_CurrentPlayTable)
			{
				StartCoroutine(CSingleton<PhoneManager>.Instance.DelayOpenManageEventScreen());
				return;
			}
			if ((bool)m_HitWorkerCollider)
			{
				m_HitWorkerCollider.OnMousePress();
				return;
			}
			if ((bool)m_CurrentRaycastObject)
			{
				m_CurrentRaycastObject.OnMouseButtonUp();
				return;
			}
		}
		if (!m_IsResetRightMousePress && InputManager.GetKeyUpAction(EGameAction.InteractRight))
		{
			if ((bool)m_CurrentCardCompartment)
			{
				m_CurrentCardCompartment.OnRightMouseButtonUp();
				return;
			}
			if ((bool)m_CurrentRaycastObject && !m_CurrentWorkbench)
			{
				m_CurrentRaycastObject.OnRightMouseButtonUp();
				return;
			}
			if ((bool)m_CurrentPlayTable)
			{
				m_CurrentPlayTable.OnRightMouseButtonUp();
				return;
			}
		}
		if (m_IsHoldingRightMouseDown)
		{
			m_RightMouseDownTime += Time.deltaTime;
			if (m_RightMouseDownTime >= m_MouseHoldAutoFireRate)
			{
				m_RightMouseDownTime = 0f;
				EvaluateTakeItemFromShelf();
				return;
			}
		}
		else if (m_RightMouseDownTime > 0f)
		{
			m_RightMouseDownTime = 0f;
			EvaluateTakeItemFromShelf();
			return;
		}
		if (InputManager.GetKeyUpAction(EGameAction.OpenCardAlbum))
		{
			EnterViewCardAlbumMode();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.StartMoveObject) && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.StartMoveObject();
		}
	}

	public void ForceStartMoveObject(InteractableObject obj)
	{
		m_CurrentRaycastObject = obj;
		m_CurrentRaycastObject.StartMoveObject();
	}

	public void SetIsPuttingCardOnDisplay(bool isPuttingCardOnDisplay)
	{
		m_IsPuttingCardOnDisplay = isPuttingCardOnDisplay;
		if (!isPuttingCardOnDisplay)
		{
			m_CurrentPutOnDisplayCardCompartment = null;
		}
	}

	public bool IsPuttingCardOnDisplay()
	{
		return m_IsPuttingCardOnDisplay;
	}

	public void StartSelectingCardFromGradedCardUIScreen()
	{
		HideCursor();
		PhoneManager.SetPhoneMode(isPhoneMode: false);
		m_IsPhoneScreenMode = false;
		m_IsSelectingCardForGradingScreen = true;
		EnterViewCardAlbumMode();
		m_GradedCardSubmitSelectScreen.SetGameUIVisible(isVisible: false);
		m_OriginalCameraRot = m_CameraController.transform.localRotation;
	}

	public void ExitSelectingCardFromGradedCardUIScreen()
	{
		ShowCursor();
		SetCurrentGameState(EGameState.PhoneState);
		PhoneManager.SetPhoneMode(isPhoneMode: true);
		m_IsPhoneScreenMode = true;
		m_IsSelectingCardForGradingScreen = false;
		m_GradedCardSubmitSelectScreen.SetGameUIVisible(isVisible: true);
		m_GradedCardSubmitSelectScreen.UpdateSelectedCardData(m_CurrentSelectedCardData);
		m_CurrentSelectedCardData = null;
	}

	public void StartSelectingCardFromDeckEditUIScreen()
	{
		ExitUIMode();
		HideCursor();
		m_IsSelectingCardForEditDeckScreen = true;
		EnterViewCardAlbumMode();
		PlayCardGameManager.DeckEditScreenSetGameUIVisible(isVisible: false);
		m_OriginalCameraRot = m_CameraController.transform.localRotation;
	}

	public void ExitSelectingCardFromDeckEditUIScreen()
	{
		EnterUIMode();
		ShowCursor();
		m_IsSelectingCardForEditDeckScreen = false;
		PlayCardGameManager.DeckEditScreenSetGameUIVisible(isVisible: true);
		PlayCardGameManager.UpdateSelectedCardData(m_CurrentSelectedCardData);
		m_CurrentSelectedCardData = null;
	}

	public void StartSelectingCardFromDeckBulkDonationBoxUIScreen(bool canSelectGradedCard)
	{
		ExitUIMode();
		HideCursor();
		m_IsSelectingCardForBulkDonationBoxScreen = true;
		m_CanSelectCardForBulkDonationBoxScreen = canSelectGradedCard;
		EnterViewCardAlbumMode();
		m_BulkDonationBoxUIScreen.SetGameUIVisible(isVisible: false);
		m_OriginalCameraRot = m_CameraController.transform.localRotation;
	}

	public void ExitSelectingCardFromBulkDonationBoxUIScreen()
	{
		EnterUIMode();
		ShowCursor();
		m_IsSelectingCardForBulkDonationBoxScreen = false;
		m_CanSelectCardForBulkDonationBoxScreen = false;
		m_BulkDonationBoxUIScreen.SetGameUIVisible(isVisible: true);
		m_BulkDonationBoxUIScreen.UpdateSelectedCardData(m_CurrentSelectedCardData);
		m_CurrentSelectedCardData = null;
	}

	public void ExitSelectingCardFromGradedCardUIScreenResetCamera()
	{
		m_CameraController.transform.localRotation = m_OriginalCameraRot;
		float x = m_CameraController.transform.localRotation.eulerAngles.x;
		float y = m_CameraController.transform.localRotation.eulerAngles.y;
		x = ((x > 180f) ? (x - 360f) : x);
		y = ((y > 180f) ? (y - 360f) : y);
		m_CameraController.SetRotationAngles(x, y);
	}

	public void UpdateSelectedCardData(CardData cardData)
	{
		m_CurrentSelectedCardData = cardData;
	}

	public bool IsSelectingCardForGradingScreen()
	{
		return m_IsSelectingCardForGradingScreen;
	}

	public bool IsSelectingCardForEditDeckScreen()
	{
		return m_IsSelectingCardForEditDeckScreen;
	}

	public bool IsSelectingCardForBulkDonationBoxScreen()
	{
		return m_IsSelectingCardForBulkDonationBoxScreen;
	}

	public bool CanSelectGradedCardForBulkDonationBoxScreen()
	{
		return m_CanSelectCardForBulkDonationBoxScreen;
	}

	private void RaycastHoldBoxState()
	{
		EvaluateDPadQuickSelectControl();
		Ray ray = new Ray(m_Cam.transform.position, m_Cam.transform.forward);
		int mask = LayerMask.GetMask("ShopModel", "ItemCompartment", "Obstacles");
		if (Physics.Raycast(ray, out var hitInfo, m_RayDistance, mask))
		{
			ShelfCompartment component = hitInfo.transform.transform.GetComponent<ShelfCompartment>();
			if ((bool)component)
			{
				if (m_CurrentItemCompartment != component)
				{
					m_CurrentItemCompartment = component;
					ShowControllerQuickSelectTooltip();
					if (m_CurrentItemCompartment.m_CanPutBox)
					{
						RemoveToolTip(EGameAction.StoreBox);
						AddToolTip(EGameAction.StoreBox);
					}
					else
					{
						RemoveToolTip(EGameAction.PutItem);
						AddToolTip(EGameAction.PutItem, isHold: true);
						if (m_CurrentItemCompartment.m_CanPutItem && m_CurrentItemCompartment.GetItemCount() > 0)
						{
							RemoveToolTip(EGameAction.TakeItem);
							AddToolTip(EGameAction.TakeItem, isHold: true);
						}
					}
				}
			}
			else if ((bool)m_CurrentItemCompartment)
			{
				RemoveToolTip(EGameAction.QuickSelect);
				RemoveToolTip(EGameAction.StoreBox);
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
				m_CurrentItemCompartment = null;
			}
			InteractableAutoCleanser component2 = hitInfo.transform.GetComponent<InteractableAutoCleanser>();
			if ((bool)component2)
			{
				if (m_CurrentAutoCleanser != component2)
				{
					if ((bool)m_CurrentAutoCleanser)
					{
						m_CurrentAutoCleanser.OnRaycastEnded();
					}
					m_CurrentAutoCleanser = component2;
					m_CurrentAutoCleanser.OnRaycasted();
					AddToolTip(EGameAction.Refill);
				}
			}
			else if ((bool)m_CurrentAutoCleanser)
			{
				m_CurrentAutoCleanser.OnRaycastEnded();
				m_CurrentAutoCleanser = null;
				RemoveToolTip(EGameAction.Refill);
			}
			InteractableAutoPackOpener component3 = hitInfo.transform.GetComponent<InteractableAutoPackOpener>();
			if ((bool)component3)
			{
				if (m_CurrentAutoPackOpener != component3)
				{
					if ((bool)m_CurrentAutoPackOpener)
					{
						m_CurrentAutoPackOpener.OnRaycastEnded();
					}
					m_CurrentAutoPackOpener = component3;
					m_CurrentAutoPackOpener.OnRaycasted();
					RemoveToolTip(EGameAction.TurnOn);
					RemoveToolTip(EGameAction.Refill);
					AddToolTip(EGameAction.Refill);
				}
			}
			else if ((bool)m_CurrentAutoPackOpener)
			{
				m_CurrentAutoPackOpener.OnRaycastEnded();
				m_CurrentAutoPackOpener = null;
				RemoveToolTip(EGameAction.Refill);
			}
			InteractableEmptyBoxStorage component4 = hitInfo.transform.GetComponent<InteractableEmptyBoxStorage>();
			if ((bool)component4)
			{
				if (m_CurrentEmptyBoxStorage != component4)
				{
					if ((bool)m_CurrentEmptyBoxStorage)
					{
						m_CurrentEmptyBoxStorage.OnRaycastEnded();
					}
					m_CurrentEmptyBoxStorage = component4;
					m_CurrentEmptyBoxStorage.OnRaycasted();
					AddToolTip(EGameAction.StoreBox);
					RemoveToolTip(EGameAction.TakeBox);
					RemoveToolTip(EGameAction.StartMoveObject);
				}
			}
			else if ((bool)m_CurrentEmptyBoxStorage)
			{
				m_CurrentEmptyBoxStorage.OnRaycastEnded();
				m_CurrentEmptyBoxStorage = null;
				RemoveToolTip(EGameAction.StoreBox);
			}
			InteractableWorkbench component5 = hitInfo.transform.GetComponent<InteractableWorkbench>();
			if ((bool)component5)
			{
				if (m_CurrentWorkbench != component5)
				{
					if ((bool)m_CurrentWorkbench)
					{
						m_CurrentWorkbench.OnRaycastEnded();
					}
					m_CurrentWorkbench = component5;
					m_CurrentWorkbench.OnRaycasted();
					RemoveToolTip(EGameAction.StartMoveObject);
					RemoveToolTip(EGameAction.InteractLeft);
					RemoveToolTip(EGameAction.PutItem);
					RemoveToolTip(EGameAction.TakeItem);
					AddToolTip(EGameAction.PutItem);
					if (m_CurrentWorkbench.m_StoredItemList.Count > 0)
					{
						AddToolTip(EGameAction.TakeItem);
					}
				}
			}
			else if ((bool)m_CurrentWorkbench)
			{
				m_CurrentWorkbench.OnRaycastEnded();
				m_CurrentWorkbench = null;
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
			}
			InteractableTrashBin component6 = hitInfo.transform.GetComponent<InteractableTrashBin>();
			if ((bool)component6)
			{
				m_CurrentTrashBin = component6;
				component6.OnRaycasted();
			}
			else if ((bool)m_CurrentTrashBin)
			{
				m_CurrentTrashBin.OnRaycastEnded();
				m_CurrentTrashBin = null;
			}
		}
		else
		{
			if ((bool)m_CurrentTrashBin)
			{
				m_CurrentTrashBin.OnRaycastEnded();
				m_CurrentTrashBin = null;
			}
			if ((bool)m_CurrentItemCompartment)
			{
				RemoveToolTip(EGameAction.StoreBox);
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
				m_CurrentItemCompartment = null;
			}
			if ((bool)m_CurrentAutoCleanser)
			{
				m_CurrentAutoCleanser.OnRaycastEnded();
				m_CurrentAutoCleanser = null;
				RemoveToolTip(EGameAction.Refill);
			}
			if ((bool)m_CurrentAutoPackOpener)
			{
				m_CurrentAutoPackOpener.OnRaycastEnded();
				m_CurrentAutoPackOpener = null;
				RemoveToolTip(EGameAction.Refill);
			}
			if ((bool)m_CurrentEmptyBoxStorage)
			{
				m_CurrentEmptyBoxStorage.OnRaycastEnded();
				m_CurrentEmptyBoxStorage = null;
				RemoveToolTip(EGameAction.StoreBox);
			}
			if ((bool)m_CurrentWorkbench)
			{
				m_CurrentWorkbench.OnRaycastEnded();
				m_CurrentWorkbench = null;
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
			}
		}
		if (InputManager.GetKeyUpAction(EGameAction.SellBoxedUpShelf) && (bool)m_CurrentHoldingBoxShelf)
		{
			if (m_CurrentHoldingBoxShelf.GetBoxedObjectType() == EObjectType.CashCounter && ShelfManager.GetCashierCounterCount() <= 1)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CantSellLastCashierCounter);
				return;
			}
			m_ConfirmSellShelfScreen.SetFurnitureData(InventoryBase.GetFurniturePurchaseData(m_CurrentHoldingBoxShelf.GetBoxedObjectType()));
			m_ConfirmSellShelfScreen.OpenScreen();
			return;
		}
		if (InputManager.GetKeyUpAction(EGameAction.Throw))
		{
			m_CurrentHoldingBox.ThrowBox(isPlayer: true);
			return;
		}
		if (InputManager.GetKeyUpAction(EGameAction.PlaceBox) && (bool)m_CurrentHoldingBox)
		{
			m_CurrentRaycastObject = m_CurrentHoldingBox;
			m_CurrentHoldingBox.DropBox(isPlayer: true);
			m_CurrentRaycastObject.StartMoveObject();
			return;
		}
		if (m_CurrentHoldingBox.IsBoxOpened())
		{
			if (InputManager.GetKeyUpAction(EGameAction.CloseBox))
			{
				m_CurrentHoldingBox.OnPressOpenBox();
				return;
			}
		}
		else if (InputManager.GetKeyUpAction(EGameAction.OpenBox))
		{
			m_CurrentHoldingBox.OnPressOpenBox();
			return;
		}
		if (m_IsHoldingMouseDown)
		{
			m_MouseDownTime += Time.deltaTime;
			if (m_MouseDownTime >= m_MouseHoldAutoFireRate)
			{
				m_MouseDownTime = 0f;
				if ((bool)m_CurrentAutoCleanser)
				{
					m_CurrentAutoCleanser.DispenseItemFromBox(m_CurrentHoldingItemBox, isPlayer: true);
				}
				else if ((bool)m_CurrentAutoPackOpener)
				{
					m_CurrentAutoPackOpener.DispenseItemFromBox(m_CurrentHoldingItemBox, isPlayer: true);
				}
				else if ((bool)m_CurrentWorkbench)
				{
					m_CurrentWorkbench.DispenseItemFromBox(m_CurrentHoldingItemBox, isPlayer: true);
				}
				else
				{
					m_CurrentHoldingBox.OnHoldStateLeftMousePress(isPlayer: true, GetCurrentItemCompartment());
				}
				return;
			}
		}
		else if (m_MouseDownTime > 0f)
		{
			m_MouseDownTime = 0f;
			if ((bool)m_CurrentAutoCleanser)
			{
				m_CurrentAutoCleanser.DispenseItemFromBox(m_CurrentHoldingItemBox, isPlayer: true);
			}
			else if ((bool)m_CurrentAutoPackOpener)
			{
				m_CurrentAutoPackOpener.DispenseItemFromBox(m_CurrentHoldingItemBox, isPlayer: true);
			}
			else if ((bool)m_CurrentEmptyBoxStorage)
			{
				m_CurrentEmptyBoxStorage.StoreBox(m_CurrentHoldingItemBox, isPlayer: true);
			}
			else if ((bool)m_CurrentWorkbench)
			{
				m_CurrentWorkbench.DispenseItemFromBox(m_CurrentHoldingItemBox, isPlayer: true);
			}
			else
			{
				m_CurrentHoldingBox.OnHoldStateLeftMousePress(isPlayer: true, GetCurrentItemCompartment());
			}
			if (!m_CurrentTrashBin)
			{
				return;
			}
			InteractablePackagingBox_Shelf component7 = m_CurrentHoldingBox.GetComponent<InteractablePackagingBox_Shelf>();
			if ((bool)m_CurrentHoldingItemBox)
			{
				if (m_CurrentHoldingItemBox.m_ItemCompartment.GetItemCount() > 0)
				{
					m_ConfirmTrashScreen.OpenScreen();
				}
				else
				{
					m_CurrentTrashBin.DiscardBox(m_CurrentHoldingBox, isPlayer: true);
				}
			}
			else if ((bool)m_CurrentHoldingBoxCard)
			{
				m_ConfirmTrashScreen.OpenScreen();
			}
			else if ((bool)component7 && component7.GetBoxedObjectType() == EObjectType.CashCounter)
			{
				if (ShelfManager.GetCashierCounterCount() <= 1)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NeedAtLeastOneCashierCounter);
				}
				else
				{
					m_CurrentTrashBin.DiscardBox(m_CurrentHoldingBox, isPlayer: true);
				}
			}
			else
			{
				m_CurrentPlayTable = null;
				m_CurrentTrashBin.DiscardBox(m_CurrentHoldingBox, isPlayer: true);
			}
			return;
		}
		if (m_IsHoldingRightMouseDown)
		{
			m_RightMouseDownTime += Time.deltaTime;
			if (m_RightMouseDownTime >= m_MouseHoldAutoFireRate)
			{
				m_RightMouseDownTime = 0f;
				if ((bool)m_CurrentWorkbench && (bool)m_CurrentHoldingItemBox)
				{
					m_CurrentWorkbench.RemoveItemFromShelf(isPlayer: true, m_CurrentHoldingItemBox);
				}
				else
				{
					m_CurrentHoldingBox.OnHoldStateRightMousePress(isPlayer: true, GetCurrentItemCompartment());
				}
			}
		}
		else if (m_RightMouseDownTime > 0f)
		{
			m_RightMouseDownTime = 0f;
			if ((bool)m_CurrentWorkbench && (bool)m_CurrentHoldingItemBox)
			{
				m_CurrentWorkbench.RemoveItemFromShelf(isPlayer: true, m_CurrentHoldingItemBox);
			}
			else
			{
				m_CurrentHoldingBox.OnHoldStateRightMousePress(isPlayer: true, GetCurrentItemCompartment());
			}
		}
	}

	public void ConfirmDiscardBox()
	{
		if ((bool)m_CurrentTrashBin)
		{
			m_CurrentTrashBin.DiscardBox(m_CurrentHoldingBox, isPlayer: true);
		}
	}

	public void ConfirmDiscardCard()
	{
		if ((bool)m_CurrentTrashBin)
		{
			InteractableCard3d interactableCard3d = m_CurrentHoldingCard3dList[0];
			RemoveCurrentCard();
			interactableCard3d.OnDestroyed();
			SoundManager.PlayAudio("SFX_Dispose", 0.5f);
		}
	}

	private void RaycastHoldSprayState()
	{
		if (m_IsHoldingMouseDown)
		{
			m_MouseDownTime += Time.deltaTime;
			m_MouseDownTimeTotal += Time.deltaTime;
			if (m_MouseDownTime >= m_MouseHoldAutoFireRate)
			{
				m_MouseDownTime = 0f;
				if (m_CurrentHoldSprayItem.GetContentFill() > 0f)
				{
					m_DeodorantSprayVFX.Play();
					if (m_MouseDownTimeTotal > 0.2f)
					{
						m_CurrentHoldSprayItem.DepleteContent(Time.fixedDeltaTime / 2.5f);
						CSingleton<GameUIScreen>.Instance.m_DeodorantBar.fillAmount = m_CurrentHoldSprayItem.GetContentFill();
						for (int i = 0; i < CSingleton<CustomerManager>.Instance.GetCustomerList().Count; i++)
						{
							CSingleton<CustomerManager>.Instance.GetCustomerList()[i].DeodorantSprayCheck(m_CurrentHoldSprayItem.transform.position, 2.5f, 1);
						}
					}
					SoundManager.SetEnableSound_SprayLoop(isEnable: true);
				}
				else
				{
					m_DeodorantSprayVFX.Stop();
					SoundManager.SetEnableSound_SprayLoop(isEnable: false);
				}
			}
		}
		else
		{
			m_MouseDownTime = 0f;
			m_MouseDownTimeTotal = 0f;
			m_DeodorantSprayVFX.Stop();
			SoundManager.SetEnableSound_SprayLoop(isEnable: false);
		}
		if (InputManager.GetKeyUpAction(EGameAction.InitiateOpenPack))
		{
			m_IsResetRightMousePress = true;
			m_RightMouseDownTime = 0f;
			m_IsHoldingRightMouseDown = false;
			m_MouseDownTime = 0f;
			m_DeodorantSprayVFX.Stop();
			SoundManager.SetEnableSound_SprayLoop(isEnable: false);
			m_IsHoldItemMode = true;
			m_IsHoldSprayMode = false;
			m_HoldItemList.Add(m_CurrentHoldSprayItem);
			CPlayerData.m_HoldItemTypeList.Add(m_CurrentHoldSprayItem.GetItemType());
			m_CurrentHoldSprayItem.SmoothLerpToTransform(m_HoldCardPackPosList[m_HoldItemList.Count], m_HoldCardPackPosList[m_HoldItemList.Count], ignoreUpForce: true);
			m_CurrentHoldSprayItem = null;
			CSingleton<GameUIScreen>.Instance.m_DeodorantBarGrp.SetActive(value: false);
			SetCurrentGameState(EGameState.HoldingItemState);
		}
	}

	private void RaycastWorkerInteractMode()
	{
	}

	private void RaycastHoldItemState()
	{
		if (m_IsOpeningCardBox)
		{
			return;
		}
		EvaluateDPadQuickSelectControl();
		Ray ray = new Ray(m_Cam.transform.position, m_Cam.transform.forward);
		int mask = LayerMask.GetMask("ShopModel", "ItemCompartment", "Physics", "Obstacles");
		if (Physics.Raycast(ray, out var hitInfo, m_RayDistance, mask))
		{
			ShelfCompartment component = hitInfo.transform.transform.GetComponent<ShelfCompartment>();
			if ((bool)component)
			{
				if (m_CurrentItemCompartment != component)
				{
					m_CurrentItemCompartment = component;
					RemoveToolTip(EGameAction.PutItem);
					AddToolTip(EGameAction.PutItem, isHold: true);
					if (m_CurrentItemCompartment.m_CanPutItem && m_CurrentItemCompartment.GetItemCount() > 0)
					{
						RemoveToolTip(EGameAction.TakeItem);
						AddToolTip(EGameAction.TakeItem, isHold: true);
					}
				}
			}
			else if ((bool)m_CurrentItemCompartment)
			{
				RemoveToolTip(EGameAction.TakeItem);
				RemoveToolTip(EGameAction.PutItem);
				m_CurrentItemCompartment = null;
			}
			InteractablePackagingBox_Item component2 = hitInfo.transform.GetComponent<InteractablePackagingBox_Item>();
			if ((bool)component2)
			{
				if (m_CurrentRaycastedPackageBoxItem != component2)
				{
					if ((bool)m_CurrentRaycastedPackageBoxItem)
					{
						m_CurrentRaycastedPackageBoxItem.OnRaycastEnded();
					}
					m_CurrentRaycastedPackageBoxItem = component2;
					m_CurrentRaycastedPackageBoxItem.OnRaycasted();
					RemoveToolTip(EGameAction.TakeBox);
					RemoveToolTip(EGameAction.PutItem);
					AddToolTip(EGameAction.PutItem, isHold: true);
					if (m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetItemCount() > 0)
					{
						RemoveToolTip(EGameAction.TakeItem);
						AddToolTip(EGameAction.TakeItem, isHold: true);
					}
				}
			}
			else if ((bool)m_CurrentRaycastedPackageBoxItem)
			{
				m_CurrentRaycastedPackageBoxItem.OnRaycastEnded();
				RemoveToolTip(EGameAction.TakeBox);
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
				m_CurrentRaycastedPackageBoxItem = null;
			}
			InteractableAutoCleanser component3 = hitInfo.transform.GetComponent<InteractableAutoCleanser>();
			if ((bool)component3)
			{
				if (m_CurrentAutoCleanser != component3)
				{
					if ((bool)m_CurrentAutoCleanser)
					{
						m_CurrentAutoCleanser.OnRaycastEnded();
					}
					m_CurrentAutoCleanser = component3;
					m_CurrentAutoCleanser.OnRaycasted();
					AddToolTip(EGameAction.Refill);
				}
			}
			else if ((bool)m_CurrentAutoCleanser)
			{
				m_CurrentAutoCleanser.OnRaycastEnded();
				m_CurrentAutoCleanser = null;
				RemoveToolTip(EGameAction.Refill);
			}
			InteractableAutoPackOpener component4 = hitInfo.transform.GetComponent<InteractableAutoPackOpener>();
			if ((bool)component4)
			{
				if (m_CurrentAutoPackOpener != component4)
				{
					if ((bool)m_CurrentAutoPackOpener)
					{
						m_CurrentAutoPackOpener.OnRaycastEnded();
					}
					m_CurrentAutoPackOpener = component4;
					m_CurrentAutoPackOpener.OnRaycasted();
					RemoveToolTip(EGameAction.TurnOn);
					RemoveToolTip(EGameAction.Refill);
					AddToolTip(EGameAction.Refill);
				}
			}
			else if ((bool)m_CurrentAutoPackOpener)
			{
				m_CurrentAutoPackOpener.OnRaycastEnded();
				m_CurrentAutoPackOpener = null;
				RemoveToolTip(EGameAction.Refill);
			}
			InteractableWorkbench component5 = hitInfo.transform.GetComponent<InteractableWorkbench>();
			if ((bool)component5)
			{
				if (m_CurrentWorkbench != component5)
				{
					if ((bool)m_CurrentWorkbench)
					{
						m_CurrentWorkbench.OnRaycastEnded();
					}
					m_CurrentWorkbench = component5;
					m_CurrentWorkbench.OnRaycasted();
					RemoveToolTip(EGameAction.StartMoveObject);
					RemoveToolTip(EGameAction.InteractLeft);
					AddToolTip(EGameAction.PutItem);
					if (m_CurrentWorkbench.m_StoredItemList.Count > 0)
					{
						AddToolTip(EGameAction.TakeItem);
					}
				}
			}
			else if ((bool)m_CurrentWorkbench)
			{
				m_CurrentWorkbench.OnRaycastEnded();
				m_CurrentWorkbench = null;
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
			}
			InteractableTrashBin component6 = hitInfo.transform.GetComponent<InteractableTrashBin>();
			if ((bool)component6)
			{
				m_CurrentTrashBin = component6;
				component6.OnRaycasted();
			}
			else if ((bool)m_CurrentTrashBin)
			{
				m_CurrentTrashBin.OnRaycastEnded();
				m_CurrentTrashBin = null;
			}
		}
		else
		{
			if ((bool)m_CurrentRaycastObject)
			{
				m_CurrentRaycastObject.OnRaycastEnded();
				m_CurrentRaycastObject = null;
			}
			if ((bool)m_CurrentTrashBin)
			{
				m_CurrentTrashBin.OnRaycastEnded();
				m_CurrentTrashBin = null;
			}
			if ((bool)m_CurrentItemCompartment)
			{
				RemoveToolTip(EGameAction.TakeItem);
				RemoveToolTip(EGameAction.PutItem);
				m_CurrentItemCompartment = null;
			}
			if ((bool)m_CurrentRaycastedPackageBoxItem)
			{
				m_CurrentRaycastedPackageBoxItem.OnRaycastEnded();
				RemoveToolTip(EGameAction.TakeBox);
				RemoveToolTip(EGameAction.TakeItem);
				RemoveToolTip(EGameAction.PutItem);
				m_CurrentRaycastedPackageBoxItem = null;
			}
			if ((bool)m_CurrentAutoCleanser)
			{
				m_CurrentAutoCleanser.OnRaycastEnded();
				m_CurrentAutoCleanser = null;
				RemoveToolTip(EGameAction.Refill);
			}
			if ((bool)m_CurrentAutoPackOpener)
			{
				m_CurrentAutoPackOpener.OnRaycastEnded();
				m_CurrentAutoPackOpener = null;
				RemoveToolTip(EGameAction.Refill);
			}
			if ((bool)m_CurrentWorkbench)
			{
				m_CurrentWorkbench.OnRaycastEnded();
				m_CurrentWorkbench = null;
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
			}
		}
		if (InputManager.GetKeyUpAction(EGameAction.InitiateOpenPack))
		{
			m_IsResetRightMousePress = true;
			m_RightMouseDownTime = 0f;
			m_IsHoldingRightMouseDown = false;
			m_MouseDownTime = 0f;
			Item item = m_HoldItemList[0];
			if (IsHoldingSpray())
			{
				m_CurrentHoldSprayItem = item;
				RemoveHoldItem(item);
				m_CurrentHoldSprayItem.SmoothLerpToTransform(m_HoldDeodorantPos, m_HoldDeodorantPos, ignoreUpForce: true);
				m_IsHoldSprayMode = true;
				m_IsHoldItemMode = false;
				SetCurrentGameState(EGameState.HoldSprayState);
				CSingleton<GameUIScreen>.Instance.m_DeodorantBarGrp.SetActive(value: true);
				CSingleton<GameUIScreen>.Instance.m_DeodorantBar.fillAmount = m_CurrentHoldSprayItem.GetContentFill();
			}
			else
			{
				EvaluateOpenCardPack();
			}
			return;
		}
		if (m_IsHoldingMouseDown)
		{
			m_MouseDownTime += Time.deltaTime;
			if (m_MouseDownTime >= m_MouseHoldAutoFireRate)
			{
				m_MouseDownTime = 0f;
				EvaluatePutItemOnShelf();
				return;
			}
		}
		else if (m_MouseDownTime > 0f)
		{
			m_MouseDownTime = 0f;
			EvaluatePutItemOnShelf();
			if ((bool)m_CurrentTrashBin)
			{
				Item item2 = m_HoldItemList[0];
				RemoveHoldItem(item2);
				item2.DisableItem();
				SoundManager.PlayAudio("SFX_Dispose", 0.5f);
				return;
			}
		}
		if (m_IsHoldingRightMouseDown)
		{
			m_RightMouseDownTime += Time.deltaTime;
			if (m_RightMouseDownTime >= m_MouseHoldAutoFireRate)
			{
				m_RightMouseDownTime = 0f;
				EvaluateTakeItemFromShelf();
			}
		}
		else if (m_RightMouseDownTime > 0f)
		{
			m_RightMouseDownTime = 0f;
			EvaluateTakeItemFromShelf();
		}
	}

	private void RaycastScannerRestockState()
	{
		EvaluateDPadQuickSelectControl();
		Ray ray = new Ray(m_Cam.transform.position, m_Cam.transform.forward);
		int mask = LayerMask.GetMask("ShopModel", "Physics", "UI", "Obstacles");
		if (Physics.Raycast(ray, out var hitInfo, m_RayDistance, mask))
		{
			Transform transform = hitInfo.transform;
			if ((bool)m_CurrentRaycastObject && transform != m_CurrentRaycastObject.transform)
			{
				RemoveToolTip(EGameAction.ScanAddToCart);
				RemoveToolTip(EGameAction.ScanRemoveFromCart);
				m_CurrentRaycastObject.OnRaycastEnded();
				m_UIBarcodeScannerScreen.UpdateScannerUI(EItemType.None, isBigBox: false, isWarehouseShelf: false);
			}
			Shelf component = hitInfo.transform.GetComponent<Shelf>();
			if ((bool)component)
			{
				m_GamepadCurrentHitShelf = component;
				m_CurrentRaycastObject = null;
			}
			WarehouseShelf component2 = hitInfo.transform.GetComponent<WarehouseShelf>();
			if ((bool)component2)
			{
				m_GamepadCurrentHitWarehouseShelf = component2;
				m_CurrentRaycastObject = null;
			}
			CardShelf component3 = hitInfo.transform.GetComponent<CardShelf>();
			if ((bool)component3)
			{
				m_GamepadCurrentHitCardShelf = component3;
				m_CurrentRaycastObject = null;
			}
			InteractablePriceTag component4 = hitInfo.transform.GetComponent<InteractablePriceTag>();
			if ((bool)component4 && m_CurrentRaycastObject != component4)
			{
				component4.OnRaycasted();
				m_CurrentRaycastObject = component4;
				m_GamepadCurrentHitShelf = component4.GetShelf();
				m_GamepadCurrentHitWarehouseShelf = component4.GetWarehouseShelf();
				if ((bool)m_GamepadCurrentHitShelf)
				{
					m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component4.transform);
				}
				if ((bool)m_GamepadCurrentHitWarehouseShelf)
				{
					m_GamepadCurrentQuickSelectIndex = -1;
				}
				if (m_IsScanRestockMode && (bool)m_GamepadCurrentHitWarehouseShelf)
				{
					m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitWarehouseShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component4.transform);
				}
				m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
				RemoveToolTip(EGameAction.SetPrice);
				RemoveToolTip(EGameAction.RemoveLabel);
				RemoveToolTip(EGameAction.ScanAddToCart);
				RemoveToolTip(EGameAction.ScanRemoveFromCart);
				AddToolTip(EGameAction.ScanAddToCart);
				AddToolTip(EGameAction.ScanRemoveFromCart);
				m_UIBarcodeScannerScreen.UpdateScannerUI(component4.GetCompartmentItemType(), component4.IsBigBox(), m_GamepadCurrentHitWarehouseShelf != null);
			}
			InteractableCardPriceTag component5 = hitInfo.transform.GetComponent<InteractableCardPriceTag>();
			if ((bool)component5 && m_CurrentRaycastObject != component5)
			{
				component5.OnRaycasted();
				m_CurrentRaycastObject = component5;
				m_GamepadCurrentHitCardShelf = component5.GetCardShelf();
				if ((bool)m_GamepadCurrentHitCardShelf)
				{
					m_GamepadCurrentQuickSelectIndex = m_GamepadCurrentHitCardShelf.GetGamepadQuickSelectTransformList(m_IsScanRestockMode).IndexOf(component5.transform);
				}
				m_HasEvaluatedDpadQuickSelectionRaycastObj = true;
				RemoveToolTip(EGameAction.SetPrice);
				RemoveToolTip(EGameAction.RemoveLabel);
				RemoveToolTip(EGameAction.ScanAddToCart);
				RemoveToolTip(EGameAction.ScanRemoveFromCart);
				m_UIBarcodeScannerScreen.UpdateScannerUI(EItemType.None, isBigBox: false, isWarehouseShelf: false);
			}
		}
		else if ((bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.OnRaycastEnded();
			m_CurrentRaycastObject = null;
			RemoveToolTip(EGameAction.ScanAddToCart);
			RemoveToolTip(EGameAction.ScanRemoveFromCart);
			m_UIBarcodeScannerScreen.UpdateScannerUI(EItemType.None, isBigBox: false, isWarehouseShelf: false);
		}
		if (InputManager.GetKeyUpAction(EGameAction.OpenPhone))
		{
			CSingleton<InteractionPlayerController>.Instance.OnExitScannerRestockMode();
			CSingleton<InteractionPlayerController>.Instance.OnEnterPhoneScreenMode();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.InteractLeft))
		{
			if (m_IsResetMousePress)
			{
				m_IsResetMousePress = false;
				return;
			}
			m_IsHoldingMouseDown = false;
			m_RightMouseDownTime = 0f;
			m_MouseDownTime = 0f;
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerAnim.Rewind();
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerAnim.Play("BarcodeScannerScan");
			CSingleton<PhoneManager>.Instance.m_ScannerRestockScreen.AddToCartForCheckout(m_UIBarcodeScannerScreen.GetRestockIndex(), 1);
			if (m_UIBarcodeScannerScreen.GetRestockIndex() >= 0)
			{
				m_UIBarcodeScannerScreen.EvaluateShelfAndStorageCount();
			}
		}
		else
		{
			if (!InputManager.GetKeyUpAction(EGameAction.InteractRight))
			{
				return;
			}
			if (m_IsResetRightMousePress)
			{
				m_IsResetRightMousePress = false;
				return;
			}
			m_IsHoldingRightMouseDown = false;
			m_RightMouseDownTime = 0f;
			m_MouseDownTime = 0f;
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerAnim.Rewind();
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerAnim.Play("BarcodeScannerScan");
			CSingleton<PhoneManager>.Instance.m_ScannerRestockScreen.RemoveFromCartForCheckout(m_UIBarcodeScannerScreen.GetRestockIndex(), 1);
			if (m_UIBarcodeScannerScreen.GetRestockIndex() >= 0)
			{
				m_UIBarcodeScannerScreen.EvaluateShelfAndStorageCount();
			}
		}
	}

	public bool IsHoldingSpray()
	{
		if (m_HoldItemList.Count > 0 && (bool)m_HoldItemList[0] && m_HoldItemList[0].GetItemType() == EItemType.Deodorant)
		{
			return true;
		}
		return false;
	}

	public bool CanOpenPack()
	{
		bool result = false;
		if (m_HoldItemList.Count > 0)
		{
			Item item = m_HoldItemList[0];
			result = IsCardPackType(item.GetItemType());
		}
		return result;
	}

	public bool IsCardPackType(EItemType itemType)
	{
		if (itemType != EItemType.BasicCardPack && itemType != EItemType.RareCardPack && itemType != EItemType.EpicCardPack && itemType != EItemType.LegendaryCardPack && itemType != EItemType.DestinyBasicCardPack && itemType != EItemType.DestinyRareCardPack && itemType != EItemType.DestinyEpicCardPack && itemType != EItemType.DestinyLegendaryCardPack && itemType != EItemType.GhostPack && itemType != EItemType.MegabotPack && itemType != EItemType.FantasyRPGPack)
		{
			return itemType == EItemType.CatJobPack;
		}
		return true;
	}

	public bool CanOpenCardBox()
	{
		bool result = false;
		if (m_HoldItemList.Count > 0)
		{
			Item item = m_HoldItemList[0];
			result = item.GetItemType() == EItemType.BasicCardBox || item.GetItemType() == EItemType.RareCardBox || item.GetItemType() == EItemType.EpicCardBox || item.GetItemType() == EItemType.LegendaryCardBox || item.GetItemType() == EItemType.DestinyBasicCardBox || item.GetItemType() == EItemType.DestinyRareCardBox || item.GetItemType() == EItemType.DestinyEpicCardBox || item.GetItemType() == EItemType.DestinyLegendaryCardBox;
		}
		return result;
	}

	public void EvaluateOpenCardPack()
	{
		if (CanOpenPack())
		{
			m_GamepadCurrentQuickSelectIndex = -1;
			m_HasEvaluatedDpadQuickSelectionRaycastObj = false;
			CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
			Item item = m_HoldItemList[0];
			RemoveHoldItem(item);
			CSingleton<CardOpeningSequence>.Instance.ReadyingCardPack(item);
			m_IsHoldingMouseDown = false;
			m_IsHoldingRightMouseDown = false;
		}
		else
		{
			if (!CanOpenCardBox())
			{
				return;
			}
			m_IsOpeningCardBox = true;
			Item item2 = m_HoldItemList[0];
			EItemType eItemType = CardBoxToCardPack(item2.GetItemType());
			if (eItemType != EItemType.None)
			{
				ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(item2.GetItemType());
				m_OpenCardBoxMeshFilter.mesh = itemMeshData.mesh;
				m_OpenCardBoxMesh.material = itemMeshData.material;
				item2.gameObject.SetActive(value: false);
				m_HoldItemList.Clear();
				CPlayerData.m_HoldItemTypeList.Clear();
				RemoveToolTip(EGameAction.OpenCardBox);
				SoundManager.PlayAudio("SFX_OpenCardBox", 0.6f);
				m_OpenCardBoxInnerMesh.gameObject.SetActive(value: true);
				m_OpenCardBoxInnerMesh.Rewind();
				m_OpenCardBoxInnerMesh.Play();
				for (int i = 0; i < 8; i++)
				{
					Item item3 = null;
					ItemMeshData itemMeshData2 = InventoryBase.GetItemMeshData(eItemType);
					item3 = ItemSpawnManager.GetItem(m_OpenCardBoxSpawnCardPackPosList[i]);
					item3.SetMesh(itemMeshData2.mesh, itemMeshData2.material, eItemType, itemMeshData2.meshSecondary, itemMeshData2.materialSecondary, itemMeshData2.materialList);
					item3.transform.position = m_OpenCardBoxSpawnCardPackPosList[i].position;
					item3.transform.rotation = m_OpenCardBoxSpawnCardPackPosList[i].rotation;
					item3.transform.parent = m_OpenCardBoxSpawnCardPackPosList[i];
					item3.transform.localScale = m_OpenCardBoxSpawnCardPackPosList[i].localScale;
					item3.gameObject.SetActive(value: true);
					m_HoldItemList.Add(item3);
					CPlayerData.m_HoldItemTypeList.Add(item3.GetItemType());
					StartCoroutine(DelayLerpSpawnedCardPackToHand(i, 1.25f + 0.05f * (float)i, item3, m_HoldCardPackPosList[i], item2));
				}
			}
		}
	}

	private IEnumerator DelayLerpSpawnedCardPackToHand(int index, float waitTime, Item item, Transform targetTransform, Item cardBoxItem)
	{
		yield return new WaitForSeconds(waitTime);
		item.SmoothLerpToTransform(targetTransform, targetTransform);
		if (index == 7 && (bool)cardBoxItem)
		{
			cardBoxItem.DisableItem();
			m_OpenCardBoxInnerMesh.gameObject.SetActive(value: false);
			m_IsOpeningCardBox = false;
		}
	}

	private EItemType CardBoxToCardPack(EItemType cardBoxItemType)
	{
		return cardBoxItemType switch
		{
			EItemType.BasicCardBox => EItemType.BasicCardPack, 
			EItemType.RareCardBox => EItemType.RareCardPack, 
			EItemType.EpicCardBox => EItemType.EpicCardPack, 
			EItemType.LegendaryCardBox => EItemType.LegendaryCardPack, 
			EItemType.DestinyBasicCardBox => EItemType.DestinyBasicCardPack, 
			EItemType.DestinyRareCardBox => EItemType.DestinyRareCardPack, 
			EItemType.DestinyEpicCardBox => EItemType.DestinyEpicCardPack, 
			EItemType.DestinyLegendaryCardBox => EItemType.DestinyLegendaryCardPack, 
			_ => EItemType.None, 
		};
	}

	private void EvaluateTakeItemFromShelf()
	{
		if ((bool)m_CurrentWorkbench && m_CurrentWorkbench.GetItemCount() > 0)
		{
			if (m_HoldItemList.Count > 0)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.HandFull);
				return;
			}
			Item item = m_CurrentWorkbench.TakeItemToHand(getLastItem: false);
			if ((bool)item)
			{
				item.SmoothLerpToTransform(m_HoldCardPackPosList[m_HoldItemList.Count], m_HoldCardPackPosList[m_HoldItemList.Count]);
				m_HoldItemList.Add(item);
				CPlayerData.m_HoldItemTypeList.Add(item.GetItemType());
				SetCurrentGameState(EGameState.HoldingItemState);
				m_IsHoldItemMode = true;
				SoundManager.GenericPop(1f, 0.9f);
			}
		}
		else if (!m_CurrentStorageCompartment && (bool)m_CurrentRaycastedPackageBoxItem && m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetItemCount() > 0)
		{
			if (m_CurrentRaycastedPackageBoxItem.IsBoxOpened())
			{
				int num = 8;
				if (m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetItemCount() <= 0)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxNoItem);
					return;
				}
				Item firstItem = m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetFirstItem();
				if ((bool)firstItem && firstItem.GetItemVolume() != 1f)
				{
					num = 1;
				}
				if (m_HoldItemList.Count < num)
				{
					if (m_HoldItemList.Count > 0)
					{
						float itemVolume = m_HoldItemList[0].GetItemVolume();
						if ((bool)firstItem && firstItem.GetItemVolume() != itemVolume)
						{
							NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.HandFull);
							return;
						}
					}
					Item item2 = m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.TakeItemToHand(getLastItem: false);
					if ((bool)item2)
					{
						item2.SmoothLerpToTransform(m_HoldCardPackPosList[m_HoldItemList.Count], m_HoldCardPackPosList[m_HoldItemList.Count]);
						m_HoldItemList.Add(item2);
						CPlayerData.m_HoldItemTypeList.Add(item2.GetItemType());
						SetCurrentGameState(EGameState.HoldingItemState);
						m_IsHoldItemMode = true;
						SoundManager.GenericPop(1f, 0.9f);
					}
				}
				else
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.HandFull);
				}
			}
			else
			{
				m_CurrentRaycastedPackageBoxItem.OnPressOpenBox();
				RemoveToolTip(EGameAction.CloseBox);
			}
		}
		else
		{
			if ((bool)m_CurrentStorageCompartment || !m_CurrentItemCompartment || !m_CurrentItemCompartment.m_CanPutItem)
			{
				return;
			}
			int num2 = 8;
			if (m_CurrentItemCompartment.GetItemCount() <= 0)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfNoItem);
				return;
			}
			Item lastItem = m_CurrentItemCompartment.GetLastItem();
			if ((bool)lastItem && lastItem.GetItemVolume() != 1f)
			{
				num2 = 1;
			}
			if (m_HoldItemList.Count < num2)
			{
				if (m_HoldItemList.Count > 0)
				{
					float itemVolume2 = m_HoldItemList[0].GetItemVolume();
					if ((bool)lastItem && lastItem.GetItemVolume() != itemVolume2)
					{
						NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.HandFull);
						return;
					}
				}
				Item item3 = m_CurrentItemCompartment.TakeItemToHand();
				if ((bool)item3)
				{
					item3.SmoothLerpToTransform(m_HoldCardPackPosList[m_HoldItemList.Count], m_HoldCardPackPosList[m_HoldItemList.Count]);
					m_HoldItemList.Add(item3);
					CPlayerData.m_HoldItemTypeList.Add(item3.GetItemType());
					SetCurrentGameState(EGameState.HoldingItemState);
					m_IsHoldItemMode = true;
					SoundManager.GenericPop(1f, 0.9f);
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.HandFull);
			}
		}
	}

	private void EvaluatePutItemOnShelf()
	{
		if ((bool)m_CurrentAutoCleanser && m_HoldItemList.Count > 0)
		{
			Item item = m_HoldItemList[0];
			if (item.GetItemType() == EItemType.Deodorant)
			{
				if (m_CurrentAutoCleanser.HasEnoughSlot())
				{
					SoundManager.GenericPop();
					m_CurrentAutoCleanser.AddItem(item, addToFront: true);
					RemoveHoldItem(item);
				}
				else
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserNoSlot);
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserOnlyAllowCleanser);
			}
		}
		else if ((bool)m_CurrentAutoPackOpener && m_HoldItemList.Count > 0)
		{
			Item item2 = m_HoldItemList[0];
			if (CanOpenPack())
			{
				if (m_CurrentAutoPackOpener.GetIsProcessing())
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CannotAddMachineRunning);
				}
				else if (m_CurrentAutoPackOpener.HasEnoughSlot())
				{
					SoundManager.GenericPop();
					m_CurrentAutoPackOpener.AddItem(item2, addToFront: true, isPlayer: true);
					RemoveHoldItem(item2);
				}
				else
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserNoSlot);
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.MachineOnlyAcceptCardPacks);
			}
		}
		else if ((bool)m_CurrentWorkbench && m_HoldItemList.Count > 0)
		{
			Item item3 = m_HoldItemList[0];
			if (m_CurrentWorkbench.IsValidItemType(item3.GetItemType()))
			{
				if (m_CurrentWorkbench.HasEnoughSlot())
				{
					SoundManager.GenericPop();
					m_CurrentWorkbench.AddItem(item3, addToFront: true);
					RemoveHoldItem(item3);
				}
				else
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.AutoCleanserNoSlot);
				}
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CanOnlyPutBulkBox);
			}
		}
		else if (!m_CurrentStorageCompartment && (bool)m_CurrentRaycastedPackageBoxItem)
		{
			if (m_CurrentRaycastedPackageBoxItem.IsBoxOpened())
			{
				Item item4 = m_HoldItemList[0];
				if (!m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.m_CanPutItem)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CanOnlyStoreBoxOnThisShelf);
					return;
				}
				if (m_HoldItemList.Count <= 0)
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NoItemOnHand);
					return;
				}
				EItemType eItemType = m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.CheckItemType(item4.GetItemType());
				m_CurrentRaycastedPackageBoxItem.SetItemType(eItemType);
				if (!m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.HasEnoughSlot())
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxNoSlot);
					return;
				}
				if (eItemType != item4.GetItemType())
				{
					NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.BoxWrongItemType);
					return;
				}
				Transform lastEmptySlotTransform = m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetLastEmptySlotTransform();
				Transform emptySlotParent = m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetEmptySlotParent();
				item4.LerpToTransform(lastEmptySlotTransform, emptySlotParent);
				m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.AddItem(item4, addToFront: true);
				m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.SetPriceTagVisibility(isVisible: false);
				RemoveHoldItem(item4);
				SoundManager.GenericPop();
				if (m_HoldItemList.Count == 0)
				{
					m_IsResetMousePress = true;
					m_MouseDownTime = 0f;
					m_IsHoldingMouseDown = false;
				}
			}
			else
			{
				m_CurrentRaycastedPackageBoxItem.OnPressOpenBox();
				RemoveToolTip(EGameAction.CloseBox);
			}
		}
		else
		{
			if ((bool)m_CurrentStorageCompartment || !m_CurrentItemCompartment)
			{
				return;
			}
			Item item5 = m_HoldItemList[0];
			if (!m_CurrentItemCompartment.m_CanPutItem)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.CanOnlyStoreBoxOnThisShelf);
				return;
			}
			if (m_HoldItemList.Count <= 0)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.NoItemOnHand);
				return;
			}
			EItemType eItemType2 = m_CurrentItemCompartment.CheckItemType(item5.GetItemType());
			if (!m_CurrentItemCompartment.HasEnoughSlot())
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfNoSlot);
				return;
			}
			if (eItemType2 != item5.GetItemType())
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShelfWrongItemType);
				return;
			}
			Transform emptySlotTransform = m_CurrentItemCompartment.GetEmptySlotTransform();
			Transform emptySlotParent2 = m_CurrentItemCompartment.GetEmptySlotParent();
			item5.LerpToTransform(emptySlotTransform, emptySlotParent2);
			m_CurrentItemCompartment.AddItem(item5, addToFront: false);
			RemoveHoldItem(item5);
			SoundManager.GenericPop();
			TutorialManager.AddTaskValue(ETutorialTaskCondition.PutItemOnShelf, 1f);
		}
	}

	private void RemoveHoldItem(Item currentItem)
	{
		m_HoldItemList.Remove(currentItem);
		CPlayerData.m_HoldItemTypeList.RemoveAt(0);
		if (m_HoldItemList.Count <= 0)
		{
			SetCurrentGameState(EGameState.DefaultState);
			m_IsHoldItemMode = false;
			m_IsResetMousePress = true;
			m_MouseDownTime = 0f;
			m_IsHoldingMouseDown = false;
			m_IsResetRightMousePress = true;
			m_RightMouseDownTime = 0f;
			m_IsHoldingRightMouseDown = false;
		}
		else
		{
			for (int i = 0; i < m_HoldItemList.Count; i++)
			{
				m_HoldItemList[i].SmoothLerpToTransform(m_HoldCardPackPosList[i], m_HoldCardPackPosList[i], ignoreUpForce: true);
			}
		}
	}

	public void AddHoldItemToFront(Item currentItem)
	{
		m_HoldItemList.Insert(0, currentItem);
		CPlayerData.m_HoldItemTypeList.Insert(0, currentItem.GetItemType());
		SetCurrentGameState(EGameState.HoldingItemState);
		m_IsHoldItemMode = true;
		for (int i = 0; i < m_HoldItemList.Count; i++)
		{
			m_HoldItemList[i].SmoothLerpToTransform(m_HoldCardPackPosList[i], m_HoldCardPackPosList[i], ignoreUpForce: true);
		}
	}

	public int GetHoldItemCount()
	{
		return m_HoldItemList.Count;
	}

	public static void SetAllHoldItemVisibility(bool isVisible)
	{
		for (int i = 0; i < CSingleton<InteractionPlayerController>.Instance.m_HoldItemList.Count; i++)
		{
			CSingleton<InteractionPlayerController>.Instance.m_HoldItemList[i].gameObject.SetActive(isVisible);
		}
	}

	private void RaycastMovingObjectState()
	{
		if (InputManager.GetKeyUpAction(EGameAction.Flip) && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.Flip();
			return;
		}
		if (InputManager.GetKeyUpAction(EGameAction.PlaceMoveObject) && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.PlaceMovedObject();
			return;
		}
		if (InputManager.GetKeyUpAction(EGameAction.BoxUpShelf) && (bool)m_CurrentRaycastObject && m_CurrentRaycastObject.m_CanBoxUpObject && m_CurrentRaycastObject.m_CanPickupMoveObject)
		{
			m_CurrentRaycastObject.BoxUpObject(holdBox: true);
			return;
		}
		if ((bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.AddObjectRotation(Input.mouseScrollDelta.y * m_MoveObjectRotateSpeed);
		}
		if (InputManager.GetKeyUpAction(EGameAction.Rotate))
		{
			if ((bool)m_CurrentRaycastObject)
			{
				m_CurrentRaycastObject.AddObjectRotation(15f, 15f);
			}
		}
		else if (InputManager.GetKeyUpAction(EGameAction.RotateB) && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.AddObjectRotation(-15f, 15f);
		}
		int mask = LayerMask.GetMask("ShopModel", "Ground", "Glass", "Obstacles");
		if (m_IsMovingObjectVerticalMode)
		{
			mask = LayerMask.GetMask("DecorationBlocker", "DecorationBlockerRaycast");
		}
		if (Physics.Raycast(new Ray(m_Cam.transform.position, m_Cam.transform.forward), out var hitInfo, m_RayDistance * 1.5f, mask))
		{
			Transform objHit = hitInfo.transform;
			if ((bool)m_CurrentRaycastObject)
			{
				m_CurrentRaycastObject.SetTargetMovePosition(hitInfo.point, hitInfo.normal, objHit);
			}
		}
		else if ((bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.SetMovePositionToCamera();
		}
	}

	private void RaycastMovingBoxState()
	{
		if (InputManager.GetKeyUpAction(EGameAction.PlaceMoveObject) && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.PlaceMovedObject();
		}
		if ((bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.AddObjectRotation(Input.mouseScrollDelta.y * m_MoveObjectRotateSpeed);
		}
		if (InputManager.GetKeyUpAction(EGameAction.Rotate))
		{
			if ((bool)m_CurrentRaycastObject)
			{
				m_CurrentRaycastObject.AddObjectRotation(15f, 15f);
			}
		}
		else if (InputManager.GetKeyUpAction(EGameAction.RotateB) && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.AddObjectRotation(-15f, 15f);
		}
		int mask = LayerMask.GetMask("ShopModel", "Ground", "Glass", "Obstacles", "Physics");
		if (m_IsMovingObjectVerticalMode)
		{
			mask = LayerMask.GetMask("DecorationBlocker", "DecorationBlockerRaycast");
		}
		if (Physics.Raycast(new Ray(m_Cam.transform.position, m_Cam.transform.forward), out var hitInfo, m_RayDistance * 1f, mask))
		{
			Transform objHit = hitInfo.transform;
			float num = Mathf.Lerp(m_BoxPhysicsDimension.x, m_BoxPhysicsDimension.y, hitInfo.normal.y);
			if ((bool)m_CurrentRaycastObject)
			{
				m_CurrentRaycastObject.SetTargetMovePosition(hitInfo.point + hitInfo.normal * num, hitInfo.normal, objHit);
			}
		}
		else if ((bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.SetMovePositionToCamera(1f);
		}
	}

	private void RaycastCashCounterState()
	{
		bool flag = false;
		int mask = LayerMask.GetMask("Item", "UI");
		if (Physics.Raycast(new Ray(m_Cam.transform.position, m_Cam.transform.forward), out var hitInfo, m_RayDistance, mask))
		{
			_ = hitInfo.transform;
			InteractableObject component = hitInfo.transform.GetComponent<InteractableObject>();
			if ((bool)component && component.m_CanScanByCounter)
			{
				if (m_CurrentRaycastObject != component)
				{
					if ((bool)m_CurrentRaycastObject)
					{
						m_CurrentRaycastObject.OnRaycastEnded();
					}
					m_CurrentRaycastObject = component;
					component.OnRaycasted();
				}
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && (bool)m_CurrentRaycastObject)
		{
			m_CurrentRaycastObject.OnRaycastEnded();
			m_CurrentRaycastObject = null;
		}
		if (InputManager.GetKeyDownAction(EGameAction.ScanCounter) || InputManager.GetKeyDownAction(EGameAction.AddChange))
		{
			m_IsHoldingMouseDown = true;
		}
		else if (InputManager.GetKeyUpAction(EGameAction.ScanCounter) || InputManager.GetKeyUpAction(EGameAction.AddChange))
		{
			m_IsHoldingMouseDown = false;
		}
		if (InputManager.GetKeyDownAction(EGameAction.RemoveChange))
		{
			m_IsHoldingRightMouseDown = true;
		}
		else if (InputManager.GetKeyUpAction(EGameAction.RemoveChange))
		{
			m_IsHoldingRightMouseDown = false;
		}
		if (m_IsHoldingMouseDown && !InputManager.GetKeyHoldAction(EGameAction.ScanCounter) && !InputManager.GetKeyHoldAction(EGameAction.AddChange))
		{
			m_IsHoldingMouseDown = false;
		}
		if (m_IsHoldingRightMouseDown && !InputManager.GetKeyHoldAction(EGameAction.RemoveChange))
		{
			m_IsHoldingRightMouseDown = false;
		}
		if ((bool)m_CurrentRaycastObject)
		{
			if (m_IsHoldingMouseDown)
			{
				m_MouseDownTime += Time.deltaTime;
				if (m_MouseDownTime >= m_MouseHoldAutoFireRate)
				{
					m_MouseDownTime = 0f;
					m_CurrentRaycastObject.OnMouseButtonUp();
				}
			}
			else if (m_MouseDownTime > 0f)
			{
				m_MouseDownTime = 0f;
				m_CurrentRaycastObject.OnMouseButtonUp();
			}
			if (m_IsHoldingRightMouseDown)
			{
				m_RightMouseDownTime += Time.deltaTime;
				if (m_RightMouseDownTime >= m_MouseHoldAutoFireRate)
				{
					m_RightMouseDownTime = 0f;
					m_CurrentRaycastObject.OnRightMouseButtonUp();
				}
			}
			else if (m_RightMouseDownTime > 0f)
			{
				m_RightMouseDownTime = 0f;
				m_CurrentRaycastObject.OnRightMouseButtonUp();
			}
		}
		if (InputManager.GetKeyUpAction(EGameAction.ExitCounter) && (bool)m_CurrentCashierCounter)
		{
			m_CurrentCashierCounter.OnPressEsc();
		}
		if (InputManager.GetKeyUpAction(EGameAction.DoneCounter) && (bool)m_CurrentCashierCounter)
		{
			m_CurrentCashierCounter.OnPressSpaceBar();
		}
	}

	public static void SetCurrentInteractableObject(InteractableObject obj)
	{
		CSingleton<InteractionPlayerController>.Instance.m_CurrentRaycastObject = obj;
	}

	public static void SetPlayerPos(Vector3 pos)
	{
		CSingleton<InteractionPlayerController>.Instance.m_PlayerRigidbody.isKinematic = true;
		CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.transform.position = pos;
	}

	public static void CameraLerpToPosition(Vector3 pos, Quaternion rot)
	{
		CSingleton<InteractionPlayerController>.Instance.m_OriginalCameraPos = CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.position;
		CSingleton<InteractionPlayerController>.Instance.m_OriginalCameraRot = CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.rotation;
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.position = pos;
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.rotation = rot;
	}

	public static void CameraLerpToOriginalPos()
	{
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.position = CSingleton<InteractionPlayerController>.Instance.m_OriginalCameraPos;
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.rotation = CSingleton<InteractionPlayerController>.Instance.m_OriginalCameraRot;
	}

	private void EvaluateCameraLerp()
	{
		if (IsCrouching())
		{
			if (m_TargetCameraPosY != -1f)
			{
				m_TargetCameraPosY = -1f;
				m_CameraBlendPosTimer = 0f;
			}
		}
		else if (m_IsPlayTableGameMode)
		{
			if (m_TargetCameraPosY != -0.5f)
			{
				m_TargetCameraPosY = -0.5f;
				m_CameraBlendPosTimer = 0f;
			}
		}
		else
		{
			if (m_TargetCameraPosX != 0f)
			{
				m_TargetCameraPosX = 0f;
				m_CameraBlendPosTimer = 0f;
			}
			if (m_TargetCameraPosY != 0f)
			{
				m_TargetCameraPosY = 0f;
				m_CameraBlendPosTimer = 0f;
			}
			if (m_TargetCameraPosZ != 0f)
			{
				m_TargetCameraPosZ = 0f;
				m_CameraBlendPosTimer = 0f;
			}
		}
		if ((bool)m_CameraWorldFollowTarget)
		{
			m_CamWorldPos.transform.position = m_CameraWorldFollowTarget.transform.position;
			if (m_TargetCameraPosX != m_CamWorldPos.transform.localPosition.x)
			{
				m_TargetCameraPosX = m_CamWorldPos.transform.localPosition.x;
				m_CameraBlendPosTimer = 0f;
			}
			if (m_TargetCameraPosY != m_CamWorldPos.transform.localPosition.y)
			{
				m_TargetCameraPosY = m_CamWorldPos.transform.localPosition.y;
				m_CameraBlendPosTimer = 0f;
			}
			if (m_TargetCameraPosZ != m_CamWorldPos.transform.localPosition.z)
			{
				m_TargetCameraPosZ = m_CamWorldPos.transform.localPosition.z;
				m_CameraBlendPosTimer = 0f;
			}
		}
		if (m_CameraBlendPosTimer < 1f)
		{
			m_CameraBlendPosTimer += Time.deltaTime * 2f;
			m_CurrentCameraPosX = Mathf.Lerp(m_CurrentCameraPosX, m_TargetCameraPosX, m_CameraBlendPosTimer);
			m_CurrentCameraPosY = Mathf.Lerp(m_CurrentCameraPosY, m_TargetCameraPosY, m_CameraBlendPosTimer);
			m_CurrentCameraPosZ = Mathf.Lerp(m_CurrentCameraPosZ, m_TargetCameraPosZ, m_CameraBlendPosTimer);
			m_CurrentCameraPos = Vector3.zero;
			m_CurrentCameraPos.x = m_CurrentCameraPosX;
			m_CurrentCameraPos.y = m_CurrentCameraPosY;
			m_CurrentCameraPos.z = m_CurrentCameraPosZ;
			m_CameraController.transform.localPosition = m_CurrentCameraPos;
			m_LookAtTransform.transform.localPosition = m_CurrentCameraPos;
		}
		if (m_LookAtTarget != null)
		{
			if (m_LookAtDelayTimer >= m_LookAtDelayTime)
			{
				m_LookAtTransform.LookAt(m_LookAtTarget);
				ForceLookAt(m_LookAtTransform, m_LerpCameraRotSpeed);
			}
			else
			{
				m_LookAtDelayTimer += Time.deltaTime;
			}
		}
		if (m_IsLerpingCameraRot)
		{
			m_CameraController.transform.rotation = Quaternion.Lerp(m_CameraController.transform.rotation, m_LerpCameraTargetRot, Time.deltaTime * m_LerpCameraRotSpeed);
		}
	}

	public void ShowCursor()
	{
		if (!CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			Cursor.visible = true;
		}
		CenterDot.SetVisibility(isVisible: false);
		InputManager.SetCursorLockMode(CursorLockMode.Confined);
		m_CameraController.enabled = false;
	}

	public void HideCursor()
	{
		Cursor.visible = false;
		CenterDot.SetVisibility(isVisible: true);
		InputManager.SetCursorLockMode(CursorLockMode.Locked);
		m_CameraController.enabled = true;
	}

	public void EnterUIMode()
	{
		ShowCursor();
		m_IsInUIMode = true;
		m_IsResetMousePress = true;
		m_MouseDownTime = 0f;
		m_IsHoldingMouseDown = false;
	}

	public void SetCameraWorldPositionTarget(Transform followTarget)
	{
		m_CameraWorldFollowTarget = followTarget;
	}

	public void StartAimLookAt(Transform lookTarget, float lerpSpeed, float delay = 0f)
	{
		m_LookAtTarget = lookTarget;
		m_LerpCameraRotSpeed = lerpSpeed;
		m_LookAtDelayTimer = 0f;
		m_LookAtDelayTime = delay;
	}

	public void StopAimLookAt()
	{
		m_LookAtTarget = null;
	}

	public void StopCameraLerp()
	{
		m_LookAtTarget = null;
		m_CameraWorldFollowTarget = null;
		m_IsLerpingCameraRot = false;
		float x = m_CameraController.transform.localRotation.eulerAngles.x;
		float y = CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.localRotation.eulerAngles.y;
		x = ((x > 180f) ? (x - 360f) : x);
		y = ((y > 180f) ? (y - 360f) : y);
		m_CameraController.SetRotationAngles(x, y);
		m_CameraController.enabled = true;
	}

	public void ForceLookAt(Transform forceView, float lerpSpeed)
	{
		m_IsLerpingCameraRot = true;
		m_LerpCameraTargetRot = forceView.rotation;
		m_LerpCameraRotSpeed = lerpSpeed;
	}

	public void ExitUIMode()
	{
		if (m_IsLerpingCameraRot)
		{
			float x = m_CameraController.transform.localRotation.eulerAngles.x;
			float y = m_CameraController.transform.localRotation.eulerAngles.y;
			x = ((x > 180f) ? (x - 360f) : x);
			y = ((y > 180f) ? (y - 360f) : y);
			m_CameraController.SetRotationAngles(x, y);
		}
		HideCursor();
		m_IsLerpingCameraRot = false;
		StartCoroutine(DelaySetUIModeFalse());
		m_IsResetMousePress = true;
		m_MouseDownTime = 0f;
		m_IsHoldingMouseDown = false;
	}

	public void ResetMousePress()
	{
		m_IsResetMousePress = true;
		m_MouseDownTime = 0f;
		m_IsHoldingMouseDown = false;
	}

	public void EnterLockMoveMode()
	{
		m_WalkerCtrl.SetStopMovement(isStop: true);
		m_CameraController.enabled = false;
		m_IsInUIMode = true;
	}

	public void ExitLockMoveMode()
	{
		CSingleton<InteractionPlayerController>.Instance.m_PlayerRigidbody.isKinematic = false;
		m_WalkerCtrl.SetStopMovement(isStop: false);
		m_CameraController.enabled = true;
		m_IsInUIMode = false;
	}

	private IEnumerator DelaySetUIModeFalse()
	{
		yield return new WaitForSeconds(0.05f);
		m_IsInUIMode = false;
	}

	public void OnEnterOpenPackState()
	{
		SetCurrentGameState(EGameState.OpeningPackState);
	}

	public void OnExitOpenPackState()
	{
		if (m_HoldItemList.Count <= 0)
		{
			SetCurrentGameState(EGameState.DefaultState);
			m_IsResetMousePress = true;
			m_MouseDownTime = 0f;
			m_IsHoldingMouseDown = false;
			m_IsResetRightMousePress = true;
			m_RightMouseDownTime = 0f;
			m_IsHoldingRightMouseDown = false;
		}
		else
		{
			SetCurrentGameState(EGameState.HoldingItemState);
		}
	}

	public void OnEnterHoldBoxMode(InteractablePackagingBox packageBox)
	{
		SetCurrentGameState(EGameState.HoldingBoxState);
		m_IsHoldBoxMode = true;
		m_CurrentHoldingBox = packageBox;
		m_CurrentHoldingItemBox = m_CurrentHoldingBox.GetComponent<InteractablePackagingBox_Item>();
		m_CurrentHoldingBoxShelf = m_CurrentHoldingBox.GetComponent<InteractablePackagingBox_Shelf>();
		m_CurrentHoldingBoxCard = m_CurrentHoldingBox.GetComponent<InteractablePackagingBox_Card>();
		m_CurrentRaycastObject = null;
	}

	public void OnExitHoldBoxMode()
	{
		m_IsHoldBoxMode = false;
		m_CurrentHoldingBox = null;
		m_CurrentHoldingItemBox = null;
		m_CurrentHoldingBoxCard = null;
		m_CurrentRaycastedPackageBoxItem = null;
		SetCurrentGameState(EGameState.DefaultState);
	}

	public void OnEnterMoveObjectMode(bool isMovingObjectVerticalMode)
	{
		SetCurrentGameState(EGameState.MovingObjectState);
		m_IsMovingObjectMode = true;
		m_IsMovingObjectVerticalMode = isMovingObjectVerticalMode;
	}

	public void OnEnterMoveBoxMode(Vector3 boxPhysicsDimension)
	{
		SetCurrentGameState(EGameState.MovingBoxState);
		m_IsMovingBoxMode = true;
		m_BoxPhysicsDimension = boxPhysicsDimension;
	}

	public void OnExitMoveObjectMode()
	{
		if (m_IsMovingObjectMode || m_IsMovingBoxMode)
		{
			SetCurrentGameState(EGameState.DefaultState);
			m_IsMovingObjectMode = false;
			m_IsMovingBoxMode = false;
			m_IsMovingObjectVerticalMode = false;
		}
	}

	public void OnEnterCashCounterMode(InteractableCashierCounter cashierCounter)
	{
		m_PlayerCollider.isTrigger = true;
		m_CurrentCashierCounter = cashierCounter;
		m_WalkerCtrl.SetStopMovement(isStop: true);
		SetCurrentGameState(EGameState.CashCounterState);
		m_IsCashCounterMode = true;
		if (m_IsCrouching)
		{
			SetIsCrouching(isCrouching: false);
		}
	}

	public void OnExitCashCounterMode()
	{
		CSingleton<InteractionPlayerController>.Instance.m_PlayerRigidbody.isKinematic = false;
		m_PlayerCollider.isTrigger = false;
		m_CurrentCashierCounter = null;
		m_WalkerCtrl.SetStopMovement(isStop: false);
		m_CurrentRaycastObject = null;
		SetCurrentGameState(EGameState.DefaultState);
		m_IsCashCounterMode = false;
		GameUIScreen.SetGameUIVisible(isVisible: true);
	}

	public void OnEnterScannerRestockMode()
	{
		m_IsResetMousePress = true;
		m_CurrentRaycastObject = null;
		SetCurrentGameState(EGameState.ScannerRestockState);
		PhoneManager.SetScanRestockMode(isScanRestockMode: true);
		m_IsScanRestockMode = true;
		m_IsPhoneScreenMode = false;
		m_UIBarcodeScannerScreen.UpdateScannerUI(EItemType.None, isBigBox: false, isWarehouseShelf: false);
		RemoveToolTip(EGameAction.ExitScanMode);
		AddToolTip(EGameAction.ExitScanMode);
		SoundManager.PlayAudio("SFX_Throw", 0.1f);
	}

	public void OnExitScannerRestockMode()
	{
		SetCurrentGameState(EGameState.DefaultState);
		PhoneManager.SetScanRestockMode(isScanRestockMode: false);
		m_IsScanRestockMode = false;
		RemoveToolTip(EGameAction.ExitScanMode);
		SoundManager.PlayAudio("SFX_Throw", 0.1f, 0.8f);
	}

	public void OnEnterPlayTableGameMode(PlayTableGame playTableGame)
	{
		m_PlayerCollider.isTrigger = true;
		m_WalkerCtrl.SetStopMovement(isStop: true);
		SetCurrentGameState(EGameState.PlayTableGameState);
		m_IsPlayTableGameMode = true;
		if (m_IsCrouching)
		{
			SetIsCrouching(isCrouching: false);
		}
	}

	public void OnExitPlayTableGameMode()
	{
		CSingleton<InteractionPlayerController>.Instance.m_PlayerRigidbody.isKinematic = false;
		m_PlayerCollider.isTrigger = false;
		m_WalkerCtrl.SetStopMovement(isStop: false);
		m_CurrentRaycastObject = null;
		SetCurrentGameState(EGameState.DefaultState);
		m_IsPlayTableGameMode = false;
		m_IsPlayingTopDownGameMode = false;
		GameUIScreen.SetGameUIVisible(isVisible: true);
	}

	public void OnEnterWorkbenchMode()
	{
		m_PlayerCollider.isTrigger = true;
		m_WalkerCtrl.SetStopMovement(isStop: true);
		GameUIScreen.SetGameUIVisible(isVisible: false);
		RemoveToolTip(EGameAction.TakeItem);
	}

	public void OnExitWorkbenchMode()
	{
		CSingleton<InteractionPlayerController>.Instance.m_PlayerRigidbody.isKinematic = false;
		m_PlayerCollider.isTrigger = false;
		m_WalkerCtrl.SetStopMovement(isStop: false);
		m_CurrentRaycastObject = null;
		GameUIScreen.SetGameUIVisible(isVisible: true);
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		if ((bool)m_CurrentWorkbench && m_CurrentWorkbench.m_StoredItemList.Count > 0)
		{
			AddToolTip(EGameAction.TakeItem);
		}
	}

	public void OnEnterPhoneScreenMode()
	{
		m_GamepadCurrentQuickSelectIndex = -1;
		m_HasEvaluatedDpadQuickSelectionRaycastObj = false;
		CSingleton<InteractionPlayerController>.Instance.StopCameraLerp();
		ShowCursor();
		m_WalkerCtrl.SetStopMovement(isStop: true);
		m_IsPhoneScreenMode = true;
		GameUIScreen.HideEnterGoNextDayIndicatorVisible();
		SetCurrentGameState(EGameState.PhoneState);
	}

	public void OnExitPhoneScreenMode()
	{
		HideCursor();
		m_WalkerCtrl.SetStopMovement(isStop: false);
		StartCoroutine(DelayExitPhoneScreenMode());
		GameUIScreen.ResetEnterGoNextDayIndicatorVisible();
		SetCurrentGameState(EGameState.DefaultState);
	}

	private IEnumerator DelayExitPhoneScreenMode()
	{
		yield return new WaitForSeconds(0.1f);
		m_CurrentRaycastObject = null;
		m_IsPhoneScreenMode = false;
	}

	public void EnterWorkerInteractMode()
	{
		SetCurrentGameState(EGameState.WorkerInteractState);
		m_IsWorkerInteractMode = true;
	}

	public void ExitWorkerInteractMode()
	{
		SetCurrentGameState(EGameState.DefaultState);
		m_IsWorkerInteractMode = false;
	}

	public void EnterHoldCardMode()
	{
		SetCurrentGameState(EGameState.HoldingCardState);
		m_IsHoldCardMode = true;
		if (m_IsPuttingCardOnDisplay)
		{
			if ((bool)m_CurrentPutOnDisplayCardCompartment)
			{
				m_CurrentPutOnDisplayCardCompartment.OnMouseButtonUp();
			}
			SetIsPuttingCardOnDisplay(isPuttingCardOnDisplay: false);
		}
	}

	public void ExitHoldCardMode()
	{
		SetCurrentGameState(EGameState.DefaultState);
		m_IsHoldCardMode = false;
	}

	private void RaycastHoldCardState()
	{
		if (m_IsHoldingMouseDown)
		{
			m_MouseDownTime += Time.deltaTime;
		}
		EvaluateDPadQuickSelectControl();
		Ray ray = new Ray(m_Cam.transform.position, m_Cam.transform.forward);
		int mask = LayerMask.GetMask("ItemCompartment");
		if (Physics.Raycast(ray, out var hitInfo, m_RayDistance, mask))
		{
			_ = hitInfo.transform;
			InteractableCardCompartment component = hitInfo.transform.GetComponent<InteractableCardCompartment>();
			if ((bool)component)
			{
				if (m_CurrentCardCompartment != component)
				{
					ShowControllerQuickSelectTooltip();
					m_CurrentCardCompartment = component;
					RemoveToolTip(EGameAction.PutCard);
					AddToolTip(EGameAction.PutCard);
					RemoveToolTip(EGameAction.TakeCard);
					AddToolTip(EGameAction.TakeCard);
				}
			}
			else
			{
				RemoveToolTip(EGameAction.QuickSelect);
				RemoveToolTip(EGameAction.PutCard);
				RemoveToolTip(EGameAction.TakeCard);
				m_CurrentCardCompartment = null;
			}
		}
		else
		{
			RemoveToolTip(EGameAction.PutCard);
			RemoveToolTip(EGameAction.TakeCard);
			m_CurrentCardCompartment = null;
		}
		if (Physics.Raycast(new Ray(m_Cam.transform.position, m_Cam.transform.forward), out var hitInfo2, m_RayDistance, LayerMask.GetMask("ShopModel")))
		{
			_ = hitInfo2.transform;
			InteractableTrashBin component2 = hitInfo2.transform.GetComponent<InteractableTrashBin>();
			if ((bool)component2)
			{
				m_CurrentTrashBin = component2;
				component2.OnRaycasted();
			}
			else if ((bool)m_CurrentTrashBin)
			{
				m_CurrentTrashBin.OnRaycastEnded();
				m_CurrentTrashBin = null;
			}
		}
		else if ((bool)m_CurrentTrashBin)
		{
			m_CurrentTrashBin.OnRaycastEnded();
			m_CurrentTrashBin = null;
		}
		if (InputManager.GetKeyUpAction(EGameAction.PutCard) && m_MouseDownTime > 0f)
		{
			m_MouseDownTime = 0f;
			if ((bool)m_CurrentCardCompartment)
			{
				m_CurrentCardCompartment.OnMouseButtonUp();
				return;
			}
			if ((bool)m_CurrentTrashBin)
			{
				InteractableCard3d interactableCard3d = m_CurrentHoldingCard3dList[0];
				float cardMarketPrice = CPlayerData.GetCardMarketPrice(interactableCard3d.m_Card3dUI.m_CardUI.GetCardData());
				float num = 10f;
				num = ((CPlayerData.m_ShopLevel < 5) ? 10f : ((CPlayerData.m_ShopLevel < 10) ? 20f : ((CPlayerData.m_ShopLevel < 20) ? 40f : ((CPlayerData.m_ShopLevel < 30) ? 80f : ((CPlayerData.m_ShopLevel < 40) ? 120f : ((CPlayerData.m_ShopLevel >= 50) ? 500f : 250f))))));
				if (cardMarketPrice > num)
				{
					m_ConfirmTrashCardScreen.OpenScreen();
					return;
				}
				RemoveCurrentCard();
				interactableCard3d.OnDestroyed();
				SoundManager.PlayAudio("SFX_Dispose", 0.5f);
				return;
			}
		}
		if (InputManager.GetKeyUpAction(EGameAction.TakeCard) && (bool)m_CurrentCardCompartment)
		{
			m_CurrentCardCompartment.OnRightMouseButtonUp();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.OpenCardAlbum))
		{
			EnterViewCardAlbumMode();
		}
	}

	public void EnterViewCardAlbumMode()
	{
		if (!m_IsViewCardAlbumMode && !m_IsExitingViewCardAlbumMode)
		{
			m_IsExitingViewCardAlbumMode = true;
			m_IsViewCardAlbumMode = true;
			m_WalkerCtrl.SetStopMovement(isStop: true);
			m_CameraController.EnterViewCardAlbumMode();
			m_CollectionBinderFlipAnimCtrl.StartShowCardAlbum(m_HoldCardAlbumPos);
			StartCoroutine(DelayExitViewCardAlbumMode());
			SetCurrentGameState(EGameState.ViewAlbumState);
			GameUIScreen.SetGameUIVisible(isVisible: false);
			TutorialManager.SetGameUIVisible(isVisible: false);
		}
	}

	public void ExitViewCardAlbumMode()
	{
		if (m_IsViewCardAlbumMode && !m_IsExitingViewCardAlbumMode)
		{
			m_IsExitingViewCardAlbumMode = true;
			if (m_CurrentGameState != EGameState.HoldingCardState)
			{
				SetCurrentGameState(EGameState.DefaultState);
			}
			m_IsViewCardAlbumMode = false;
			if (!m_IsSelectingCardForGradingScreen && !m_IsSelectingCardForEditDeckScreen && !m_IsSelectingCardForBulkDonationBoxScreen)
			{
				m_WalkerCtrl.SetStopMovement(isStop: false);
			}
			m_CameraController.ExitViewCardAlbumMode();
			m_CollectionBinderFlipAnimCtrl.HideCardAlbum();
			StartCoroutine(DelayExitViewCardAlbumMode());
			GameUIScreen.SetGameUIVisible(isVisible: true);
			TutorialManager.SetGameUIVisible(isVisible: true);
			if (m_IsSelectingCardForGradingScreen)
			{
				ExitSelectingCardFromGradedCardUIScreen();
			}
			else if (m_IsSelectingCardForEditDeckScreen)
			{
				ExitSelectingCardFromDeckEditUIScreen();
			}
			else if (m_IsSelectingCardForBulkDonationBoxScreen)
			{
				ExitSelectingCardFromBulkDonationBoxUIScreen();
			}
		}
	}

	private IEnumerator DelayExitViewCardAlbumMode()
	{
		yield return new WaitForSeconds(m_HideCardAlbumTime);
		m_IsExitingViewCardAlbumMode = false;
	}

	private void RaycastViewCardAlbumState()
	{
		if (!m_CollectionBinderFlipAnimCtrl.m_IsHoldingCardCloseUp)
		{
			Ray ray = new Ray(m_Cam.transform.position, m_Cam.transform.forward);
			bool flag = false;
			int mask = LayerMask.GetMask("UI");
			if (Physics.Raycast(ray, out var hitInfo, m_RayDistance, mask))
			{
				_ = hitInfo.transform;
				InteractableCard3d component = hitInfo.transform.GetComponent<InteractableCard3d>();
				if ((bool)component)
				{
					if (m_CurrentRaycastedCard3d != component)
					{
						if ((bool)m_CurrentRaycastedCard3d)
						{
							m_CurrentRaycastedCard3d.OnRaycastEnded();
						}
						m_CurrentRaycastedCard3d = component;
						component.OnRaycasted();
					}
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag && (bool)m_CurrentRaycastedCard3d)
			{
				m_CurrentRaycastedCard3d.OnRaycastEnded();
				m_CurrentRaycastedCard3d = null;
			}
		}
		else
		{
			m_CurrentRaycastedCard3d = null;
		}
		if (InputManager.GetKeyUpAction(EGameAction.TakeAlbumCard))
		{
			m_CollectionBinderFlipAnimCtrl.OnMouseButtonUp();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.InteractRight))
		{
			m_CollectionBinderFlipAnimCtrl.OnRightMouseButtonUp();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.CloseCardAlbum))
		{
			ExitViewCardAlbumMode();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.FlipNextPage))
		{
			m_CollectionBinderFlipAnimCtrl.FlipNextPage();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.FlipPreviousPage))
		{
			m_CollectionBinderFlipAnimCtrl.FlipPreviousPage();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.FlipPreviousPage10))
		{
			m_CollectionBinderFlipAnimCtrl.FlipPrevious10Page();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.FlipNextPage10))
		{
			m_CollectionBinderFlipAnimCtrl.FlipNext10Page();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.SortAlbum))
		{
			m_CollectionBinderFlipAnimCtrl.OpenSortAlbumScreen();
		}
	}

	public static ShelfCompartment GetCurrentItemCompartment()
	{
		return CSingleton<InteractionPlayerController>.Instance.m_CurrentItemCompartment;
	}

	public static bool HasEnoughSlotToHoldCard()
	{
		return CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count < 8;
	}

	public static void AddHoldCard(InteractableCard3d card3d)
	{
		if ((bool)card3d)
		{
			card3d.m_Card3dUI.m_IgnoreCulling = true;
			card3d.m_Card3dUI.SetSimplifyCardDistanceCull(isCull: false);
			card3d.m_Card3dUI.m_CardUI.SetFoilCullListVisibility(isActive: true);
			card3d.m_Card3dUI.m_CardUI.ResetFarDistanceCull();
			card3d.m_Card3dUI.m_CardUIAnimGrp.gameObject.SetActive(value: true);
			card3d.m_Card3dUI.m_CardUI.SetFoilMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilMaterialWorldView);
			card3d.m_Card3dUI.m_CardUI.SetFoilBlendedMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilBlendedMaterialWorldView);
			card3d.LerpToTransform(CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count], CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count]);
			Quaternion holdCardGrpTargetRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 15f, (float)CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count / 7f));
			CSingleton<InteractionPlayerController>.Instance.m_HoldCardGrpTargetRotation = holdCardGrpTargetRotation;
			CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Add(card3d);
			CPlayerData.m_HoldCardDataList.Add(card3d.m_Card3dUI.m_CardUI.GetCardData());
		}
	}

	public static void RemoveCurrentCard()
	{
		CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList[0].m_Card3dUI.m_IgnoreCulling = false;
		CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList[0].m_Card3dUI.m_CardUI.SetFoilMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilMaterialTangentView);
		CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList[0].m_Card3dUI.m_CardUI.SetFoilBlendedMaterialList(CSingleton<Card3dUISpawner>.Instance.m_FoilBlendedMaterialTangentView);
		CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.RemoveAt(0);
		CPlayerData.m_HoldCardDataList.RemoveAt(0);
		if (CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count <= 0)
		{
			CSingleton<InteractionPlayerController>.Instance.ExitHoldCardMode();
			Quaternion holdCardGrpTargetRotation = Quaternion.Euler(0f, 0f, 0f);
			CSingleton<InteractionPlayerController>.Instance.m_HoldCardGrpTargetRotation = holdCardGrpTargetRotation;
			return;
		}
		for (int i = 0; i < CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count; i++)
		{
			CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList[i].LerpToTransform(CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[i], CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[i]);
		}
		Quaternion holdCardGrpTargetRotation2 = Quaternion.Euler(0f, 0f, Mathf.Lerp(-2.143f, 15f, (float)CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count / 8f));
		CSingleton<InteractionPlayerController>.Instance.m_HoldCardGrpTargetRotation = holdCardGrpTargetRotation2;
	}

	public static void RemoveAllCard()
	{
		for (int i = 0; i < CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count; i++)
		{
			CPlayerData.AddCard(CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList[i].m_Card3dUI.m_CardUI.GetCardData(), 1);
			CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList[i].OnDestroyed();
		}
		CSingleton<InteractionPlayerController>.Instance.ExitHoldCardMode();
		CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Clear();
		CPlayerData.m_HoldCardDataList.Clear();
		Quaternion holdCardGrpTargetRotation = Quaternion.Euler(0f, 0f, 0f);
		CSingleton<InteractionPlayerController>.Instance.m_HoldCardGrpTargetRotation = holdCardGrpTargetRotation;
	}

	public static InteractableCard3d GetCurrentHoldCard()
	{
		if (CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList.Count <= 0)
		{
			return null;
		}
		return CSingleton<InteractionPlayerController>.Instance.m_CurrentHoldingCard3dList[0];
	}

	public static void ClearToolTip()
	{
		CSingleton<InteractionPlayerController>.Instance.m_InputTooltipListDisplay.ClearTooltip();
	}

	public static void AddToolTip(EGameAction action, bool isHold = false, bool singleKeyOnly = false)
	{
		CSingleton<InteractionPlayerController>.Instance.m_InputTooltipListDisplay.ShowTooltip(action, isHold, singleKeyOnly);
	}

	public static void RemoveToolTip(EGameAction action)
	{
		CSingleton<InteractionPlayerController>.Instance.m_InputTooltipListDisplay.RemoveTooltip(action);
	}

	public static void TempHideToolTip()
	{
		CSingleton<InteractionPlayerController>.Instance.m_InputTooltipListDisplay.ClearTooltip();
	}

	public static void RestoreHiddenToolTip()
	{
		CSingleton<InteractionPlayerController>.Instance.SetCurrentGameState(CSingleton<InteractionPlayerController>.Instance.m_CurrentGameState);
	}

	private void SetCurrentGameState(EGameState state)
	{
		m_CurrentGameState = state;
		m_InputTooltipListDisplay.SetCurrentGameState(state);
		switch (state)
		{
		case EGameState.HoldingItemState:
			if ((bool)m_CurrentItemCompartment && m_CurrentItemCompartment.m_CanPutItem)
			{
				ShowControllerQuickSelectTooltip();
				RemoveToolTip(EGameAction.PutItem);
				RemoveToolTip(EGameAction.TakeItem);
				AddToolTip(EGameAction.PutItem, isHold: true);
				AddToolTip(EGameAction.TakeItem, isHold: true);
			}
			if ((bool)m_CurrentRaycastedPackageBoxItem)
			{
				RemoveToolTip(EGameAction.QuickSelect);
				RemoveToolTip(EGameAction.PutItem);
				AddToolTip(EGameAction.PutItem, isHold: true);
				if (m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetItemCount() > 0)
				{
					RemoveToolTip(EGameAction.TakeItem);
					AddToolTip(EGameAction.TakeItem, isHold: true);
				}
			}
			if ((bool)m_CurrentWorkbench)
			{
				RemoveToolTip(EGameAction.StartMoveObject);
				RemoveToolTip(EGameAction.InteractLeft);
				RemoveToolTip(EGameAction.PutItem);
				AddToolTip(EGameAction.PutItem);
			}
			break;
		case EGameState.HoldingCardState:
			if ((bool)m_CurrentCardCompartment)
			{
				ShowControllerQuickSelectTooltip();
				RemoveToolTip(EGameAction.PutCard);
				AddToolTip(EGameAction.PutCard);
				RemoveToolTip(EGameAction.TakeCard);
				AddToolTip(EGameAction.TakeCard);
			}
			break;
		case EGameState.HoldingBoxState:
			if ((bool)m_CurrentItemCompartment)
			{
				if (m_CurrentItemCompartment.m_CanPutBox)
				{
					ShowControllerQuickSelectTooltip();
					RemoveToolTip(EGameAction.StoreBox);
					AddToolTip(EGameAction.StoreBox);
				}
				else if (m_CurrentItemCompartment.m_CanPutItem)
				{
					ShowControllerQuickSelectTooltip();
					RemoveToolTip(EGameAction.PutItem);
					AddToolTip(EGameAction.PutItem, isHold: true);
					RemoveToolTip(EGameAction.TakeItem);
					AddToolTip(EGameAction.TakeItem, isHold: true);
				}
			}
			if ((bool)m_CurrentWorkbench)
			{
				RemoveToolTip(EGameAction.StartMoveObject);
				RemoveToolTip(EGameAction.InteractLeft);
				RemoveToolTip(EGameAction.PutItem);
				AddToolTip(EGameAction.PutItem);
			}
			break;
		case EGameState.DefaultState:
			if ((bool)m_CurrentRaycastObject)
			{
				ClearToolTip();
				for (int i = 0; i < m_CurrentRaycastObject.m_GameActionInputDisplayList.Count; i++)
				{
					AddToolTip(m_CurrentRaycastObject.m_GameActionInputDisplayList[i]);
				}
			}
			if ((bool)m_CurrentItemCompartment && m_CurrentItemCompartment.m_CanPutItem)
			{
				ShowControllerQuickSelectTooltip();
				if (m_CurrentItemCompartment.GetItemCount() > 0)
				{
					RemoveToolTip(EGameAction.TakeItem);
					AddToolTip(EGameAction.TakeItem, isHold: true);
				}
			}
			if ((bool)m_CurrentStorageCompartment)
			{
				ShowControllerQuickSelectTooltip();
				if (m_CurrentStorageCompartment.GetShelfCompartment().GetItemCount() > 0)
				{
					RemoveToolTip(EGameAction.TakeBox);
					AddToolTip(EGameAction.TakeBox);
				}
			}
			if ((bool)m_CurrentRaycastedPackageBoxItem)
			{
				RemoveToolTip(EGameAction.TakeBox);
				AddToolTip(EGameAction.TakeBox);
				if (m_CurrentRaycastedPackageBoxItem.m_ItemCompartment.GetItemCount() > 0)
				{
					RemoveToolTip(EGameAction.TakeItem);
					AddToolTip(EGameAction.TakeItem);
				}
			}
			if ((bool)m_CurrentCardCompartment)
			{
				ShowControllerQuickSelectTooltip();
				RemoveToolTip(EGameAction.PutCard);
				RemoveToolTip(EGameAction.TakeCard);
				if (m_CurrentCardCompartment.m_StoredCardList.Count > 0)
				{
					AddToolTip(EGameAction.TakeCard);
				}
				else
				{
					AddToolTip(EGameAction.PutCard);
				}
			}
			if ((bool)m_CurrentPlayTable)
			{
				RemoveToolTip(EGameAction.ManageEvent);
				AddToolTip(EGameAction.ManageEvent);
			}
			if ((bool)m_CurrentWorkbench && m_CurrentWorkbench.m_StoredItemList.Count > 0)
			{
				RemoveToolTip(EGameAction.TakeItem);
				AddToolTip(EGameAction.TakeItem);
			}
			break;
		}
	}

	public bool IsInUIMode()
	{
		if (!m_IsInUIMode && !m_IsPhoneScreenMode)
		{
			return m_IsViewCardAlbumMode;
		}
		return true;
	}

	public bool IsSprinting()
	{
		return m_IsSprinting;
	}

	public bool IsCrouching()
	{
		return m_IsCrouching;
	}

	public void SetIsCrouching(bool isCrouching)
	{
		m_IsCrouching = isCrouching;
		m_IsSprinting = false;
	}

	public void OnJump()
	{
		m_IsCrouching = false;
	}

	public bool CanJump()
	{
		if (m_CurrentGameState != EGameState.DefaultState && m_CurrentGameState != EGameState.HoldingBoxState && m_CurrentGameState != EGameState.HoldingCardState && m_CurrentGameState != EGameState.HoldingItemState && m_CurrentGameState != EGameState.HoldSprayState)
		{
			return m_CurrentGameState == EGameState.ScannerRestockState;
		}
		return true;
	}

	public bool IsStopSnapWhenMoving()
	{
		return m_IsStopSnapWhenMoving;
	}

	private void EvaluateInvertedMouse()
	{
		if ((bool)m_CameraMouseInput)
		{
			m_CameraMouseInput.invertHorizontalInput = CSingleton<CGameManager>.Instance.m_InvertedMouseX;
			m_CameraMouseInput.invertVerticalInput = CSingleton<CGameManager>.Instance.m_InvertedMouse;
		}
	}

	public void OpenCashierSettingScreen(InteractableCashierCounter cashCounter)
	{
		m_SetCashierCounterSettingScreen.SetCashierCounter(cashCounter);
		m_SetCashierCounterSettingScreen.OpenScreen();
	}

	public void OpenCardStorageShelfSettingScreen(InteractableCardStorageShelf cardStorageShelf)
	{
		m_SetCardStorageShelfSettingScreen.SetCardStorageShelf(cardStorageShelf);
		m_SetCardStorageShelfSettingScreen.OpenScreen();
	}

	public void ShowControllerQuickSelectTooltip()
	{
		RemoveToolTip(EGameAction.QuickSelect);
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			AddToolTip(EGameAction.QuickSelect);
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
			CEventManager.AddListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
			CEventManager.RemoveListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		AchievementManager.OnCheckAlbumCardCount(CPlayerData.GetTotalCardCollectedAmount());
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.SetRotationAngles(0f, -35f);
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = false;
		if (CPlayerData.m_HoldItemTypeList.Count > 7)
		{
			for (int num = CPlayerData.m_HoldItemTypeList.Count - 1; num >= 8; num--)
			{
				CPlayerData.m_HoldItemTypeList.RemoveAt(num);
			}
		}
		if (CPlayerData.m_HoldCardDataList.Count > 7)
		{
			for (int num2 = CPlayerData.m_HoldCardDataList.Count - 1; num2 >= 8; num2--)
			{
				CPlayerData.m_HoldCardDataList.RemoveAt(num2);
			}
		}
		if (CPlayerData.m_HoldCardDataList.Count > 0)
		{
			List<InteractableCard3d> list = new List<InteractableCard3d>();
			for (int i = 0; i < CPlayerData.m_HoldCardDataList.Count; i++)
			{
				Card3dUIGroup cardUI = CSingleton<Card3dUISpawner>.Instance.GetCardUI();
				InteractableCard3d component = ShelfManager.SpawnInteractableObject(EObjectType.Card3d).GetComponent<InteractableCard3d>();
				cardUI.m_CardUI.SetCardUI(CPlayerData.m_HoldCardDataList[i]);
				cardUI.transform.position = CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[i].position;
				cardUI.transform.rotation = CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[i].rotation;
				component.transform.position = CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[i].position;
				component.transform.rotation = CSingleton<InteractionPlayerController>.Instance.m_HoldCardPosList[i].rotation;
				component.SetCardUIFollow(cardUI);
				component.SetEnableCollision(isEnable: false);
				list.Add(component);
			}
			CPlayerData.m_HoldCardDataList.Clear();
			for (int j = 0; j < list.Count; j++)
			{
				AddHoldCard(list[j]);
			}
			EnterHoldCardMode();
		}
		else if (CPlayerData.m_HoldItemTypeList.Count > 0)
		{
			List<Item> list2 = new List<Item>();
			for (int k = 0; k < CPlayerData.m_HoldItemTypeList.Count; k++)
			{
				Item item = null;
				ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(CPlayerData.m_HoldItemTypeList[k]);
				item = ItemSpawnManager.GetItem(m_HoldCardPackPosList[k]);
				item.SetMesh(itemMeshData.mesh, itemMeshData.material, CPlayerData.m_HoldItemTypeList[k], itemMeshData.meshSecondary, itemMeshData.materialSecondary, itemMeshData.materialList);
				item.transform.position = m_HoldCardPackPosList[k].position;
				item.transform.rotation = m_HoldCardPackPosList[k].rotation;
				item.SmoothLerpToTransform(m_HoldCardPackPosList[k], m_HoldCardPackPosList[k]);
				item.gameObject.SetActive(value: true);
				list2.Add(item);
			}
			for (int l = 0; l < list2.Count; l++)
			{
				m_HoldItemList.Add(list2[l]);
			}
			SetCurrentGameState(EGameState.HoldingItemState);
			m_IsHoldItemMode = true;
		}
	}

	protected void OnFinishHideLoadingScreen(CEventPlayer_FinishHideLoadingScreen evt)
	{
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.SetRotationAngles(0f, -35f);
		CSingleton<InteractionPlayerController>.Instance.m_CameraController.enabled = true;
		if (!CPlayerData.m_HasGetGhostCard && (CPlayerData.GetCardCollectedAmount(ECardExpansionType.Ghost, isDimensionCard: false) > 0 || CPlayerData.GetCardCollectedAmount(ECardExpansionType.Ghost, isDimensionCard: true) > 0))
		{
			CPlayerData.m_HasGetGhostCard = true;
		}
	}

	protected void OnSettingUpdated(CEventPlayer_OnSettingUpdated evt)
	{
		EvaluateInvertedMouse();
	}
}
