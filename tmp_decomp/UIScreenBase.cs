using UnityEngine;

public class UIScreenBase : MonoBehaviour
{
	public GameObject m_ScreenGroup;

	public bool m_CloseIfScreenOpened = true;

	public bool m_CheckVirtualKeyboardActive;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	protected UIScreenBase m_ParentScreen;

	protected bool m_IsScreenOpen;

	protected bool m_FinishLoading;

	protected bool m_FinishHideLoadingScreen;

	protected UIScreenBase m_CurrentChildScreen;

	protected virtual void Awake()
	{
		m_ControllerScreenUIExtension = GetComponent<ControllerScreenUIExtension>();
	}

	protected virtual void Start()
	{
		m_ScreenGroup.SetActive(value: false);
	}

	protected void Update()
	{
		if (m_FinishLoading && !m_CurrentChildScreen && m_IsScreenOpen)
		{
			RunUpdate();
		}
	}

	protected virtual void RunUpdate()
	{
	}

	protected virtual void Init()
	{
		m_FinishLoading = true;
	}

	public void SetParentScreen(UIScreenBase parentScreen)
	{
		m_ParentScreen = parentScreen;
	}

	protected void OpenChildScreen(UIScreenBase childScreen)
	{
		m_CurrentChildScreen = childScreen;
		m_CurrentChildScreen.SetParentScreen(this);
		childScreen.OpenScreen();
	}

	protected void CloseChildScreen(UIScreenBase childScreen)
	{
		if (m_CurrentChildScreen == childScreen)
		{
			m_CurrentChildScreen = null;
			OnChildScreenClosed(childScreen);
		}
	}

	protected virtual void OnChildScreenClosed(UIScreenBase childScreen)
	{
	}

	public void OpenScreen()
	{
		if (m_CloseIfScreenOpened && m_IsScreenOpen)
		{
			CloseScreen();
			return;
		}
		OnOpenScreen();
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
	}

	protected virtual void OnOpenScreen()
	{
		m_IsScreenOpen = true;
		m_ScreenGroup.SetActive(value: true);
	}

	public void CloseScreen()
	{
		if (m_IsScreenOpen && (!m_CheckVirtualKeyboardActive || !GameInstance.m_IsVirtualKeyboardActive))
		{
			OnCloseScreen();
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
		}
	}

	protected virtual void OnCloseScreen()
	{
		m_IsScreenOpen = false;
		m_ScreenGroup.SetActive(value: false);
		if ((bool)m_ParentScreen)
		{
			m_ParentScreen.CloseChildScreen(this);
			m_ParentScreen = null;
		}
	}

	public void OnPressBack()
	{
		if ((bool)m_CurrentChildScreen)
		{
			m_CurrentChildScreen.OnPressBack();
		}
		else
		{
			CloseScreen();
		}
	}

	public bool IsScreenOpened()
	{
		return m_IsScreenOpen;
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_FinishHideLoadingScreen>(OnFinishHideLoadingScreen);
		}
	}

	protected virtual void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
	}

	protected virtual void OnFinishHideLoadingScreen(CEventPlayer_FinishHideLoadingScreen evt)
	{
		m_FinishHideLoadingScreen = true;
		if (!m_FinishLoading)
		{
			Init();
		}
	}
}
