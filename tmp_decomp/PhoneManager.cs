using System.Collections;
using UnityEngine;

public class PhoneManager : CSingleton<PhoneManager>
{
	public static PhoneManager m_Instance;

	public Transform m_PhoneGrp;

	public Transform m_StartPos;

	public Transform m_EndPos;

	public Animation m_EndPosAnim;

	public Animation m_BarcodeScannerAnim;

	public GameObject m_BarcodeScannerMesh;

	public UI_PhoneScreen m_UI_PhoneScreen;

	public RestockItemScreen m_RestockItemScreen;

	public RestockItemBoardGameScreen m_RestockItemBoardGameScreen;

	public FurnitureShopUIScreen m_FurnitureShopUIScreen;

	public ExpansionShopUIScreen m_ExpandShopUIScreen;

	public SetGameEventScreen m_SetGameEventUIScreen;

	public CheckPriceScreen m_CheckPriceScreen;

	public HireWorkerScreen m_HireWorkerScreen;

	public RentBillScreen m_RentBillScreen;

	public CustomerReviewScreen m_CustomerReviewScreen;

	public ShopBuyDecoUIScreen m_ShopBuyDecoUIScreen;

	public GradeCardWebsiteUIScreen m_GradeCardWebsiteUIScreen;

	public ScannerRestockScreen m_ScannerRestockScreen;

	private bool m_IsPhoneMode;

	private bool m_IsScanRestockMode;

	private bool m_IsOpeningPhone;

	private bool m_CanClosePhone = true;

	private float m_Timer;

	private float m_LerpSpeed = 3f;

	private void Awake()
	{
		m_BarcodeScannerMesh.SetActive(value: false);
	}

	private void Update()
	{
		if (m_IsPhoneMode)
		{
			m_Timer = Mathf.Clamp(m_Timer + Time.deltaTime * m_LerpSpeed, 0f, 1f);
		}
		else
		{
			m_Timer = Mathf.Clamp(m_Timer - Time.deltaTime * m_LerpSpeed, 0f, 1f);
		}
		m_PhoneGrp.transform.position = Vector3.Lerp(m_StartPos.position, m_EndPos.position, m_Timer);
		m_PhoneGrp.transform.rotation = Quaternion.Lerp(m_StartPos.rotation, m_EndPos.rotation, m_Timer);
	}

	public static void EnterPhoneMode()
	{
		SoundManager.PlayAudio("SFX_Throw", 0.1f);
		CSingleton<InteractionPlayerController>.Instance.OnEnterPhoneScreenMode();
		CSingleton<PhoneManager>.Instance.m_IsPhoneMode = true;
		CSingleton<PhoneManager>.Instance.m_UI_PhoneScreen.OpenScreen();
	}

	public static void ExitPhoneMode()
	{
		if (CanClosePhone())
		{
			CSingleton<PhoneManager>.Instance.m_EndPosAnim.Play("PhoneDefaultEndPos");
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerAnim.Play("BarcodeScannerHidden");
			SoundManager.PlayAudio("SFX_Throw", 0.1f, 0.8f);
			CSingleton<InteractionPlayerController>.Instance.OnExitPhoneScreenMode();
			CSingleton<PhoneManager>.Instance.m_IsPhoneMode = false;
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerMesh.SetActive(value: false);
		}
		CSingleton<PhoneManager>.Instance.m_UI_PhoneScreen.OnPressBack();
	}

	public static void SetPhoneMode(bool isPhoneMode)
	{
		CSingleton<PhoneManager>.Instance.m_IsPhoneMode = isPhoneMode;
	}

	public static void SetScanRestockMode(bool isScanRestockMode)
	{
		CSingleton<PhoneManager>.Instance.m_IsScanRestockMode = isScanRestockMode;
		if (isScanRestockMode)
		{
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerMesh.SetActive(value: true);
			CSingleton<PhoneManager>.Instance.m_EndPosAnim.Play("EnterPhoneScanRestockMode");
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerAnim.Play("BarcodeScannerEnter");
			CSingleton<PhoneManager>.Instance.m_ScannerRestockScreen.m_ControllerScreenUIExtension.SetControllerUIActive(isActive: false);
			CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.gameObject.SetActive(value: false);
		}
		else
		{
			CSingleton<PhoneManager>.Instance.m_EndPosAnim.Play("ExitPhoneScanRestockMode");
			CSingleton<PhoneManager>.Instance.m_BarcodeScannerAnim.Play("BarcodeScannerExit");
			CSingleton<PhoneManager>.Instance.m_ScannerRestockScreen.m_ControllerScreenUIExtension.SetControllerUIActive(isActive: true);
			CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.gameObject.SetActive(value: true);
		}
	}

	public static void SetCanClosePhone(bool canClose)
	{
		if (canClose)
		{
			InteractionPlayerController.RemoveToolTip(EGameAction.ClosePhone);
			InteractionPlayerController.AddToolTip(EGameAction.ClosePhone);
		}
		else
		{
			InteractionPlayerController.RemoveToolTip(EGameAction.ClosePhone);
		}
		CSingleton<PhoneManager>.Instance.m_CanClosePhone = canClose;
	}

	public static bool CanClosePhone()
	{
		if (CSingleton<PhoneManager>.Instance.m_CanClosePhone)
		{
			return !CSingleton<PhoneManager>.Instance.m_IsOpeningPhone;
		}
		return false;
	}

	public IEnumerator DelayOpenManageEventScreen()
	{
		m_IsOpeningPhone = true;
		EnterPhoneMode();
		yield return new WaitForSeconds(0.1f);
		m_UI_PhoneScreen.OnPressManageEventBtn();
		m_IsOpeningPhone = false;
	}

	public void SetBillNotificationVisible(bool isVisible)
	{
		m_UI_PhoneScreen.SetBillNotificationVisible(isVisible);
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
			CEventManager.AddListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.AddListener<CEventPlayer_OnDayEnded>(OnDayEnded);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
			CEventManager.RemoveListener<CEventPlayer_OnDayStarted>(OnDayStarted);
			CEventManager.RemoveListener<CEventPlayer_OnDayEnded>(OnDayEnded);
		}
	}

	protected virtual void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		m_RentBillScreen.OnGameFinishLoaded();
	}

	protected virtual void OnFinishHideLoadingScreen(CEventPlayer_FinishHideLoadingScreen evt)
	{
	}

	protected void OnDayStarted(CEventPlayer_OnDayStarted evt)
	{
		if (CPlayerData.m_CurrentDay > 0)
		{
			m_RentBillScreen.EvaluateNewDayBill();
		}
	}

	protected void OnDayEnded(CEventPlayer_OnDayEnded evt)
	{
	}
}
