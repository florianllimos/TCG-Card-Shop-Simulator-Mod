using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GenericSliderScreen : UIScreenBase
{
	public RectTransform m_AnimGrp;

	public Transform m_VerticalLayoutGrp;

	public VerticalLayoutGroup m_VerticalLayout;

	public bool m_ResetScrollPosOnOpen = true;

	public bool m_EvalSliderMaxOnInit = true;

	public bool m_EvalSliderMaxOnOpen;

	private bool m_IsMinimized;

	public float m_DefaultMinimizedY;

	public float m_MaximizePosY = -100f;

	public float m_ScrollSpeedMultiplier = 1f;

	public float m_GamepadJoystickSpeedMultiplier = 1f;

	public float m_MaxPosXClamp = 100000f;

	private float m_ScreenRatio = 1f;

	[Space(10f)]
	[Header("Slider")]
	public GameObject m_SliderGrp;

	public bool m_MouseWheelScrollInverted;

	protected bool m_CanEvaluateMaxScrollPos;

	protected bool m_MaxPosFound;

	protected bool m_UseAccurateMaxPos;

	protected bool m_MaxPosAccurateFound;

	public GameObject m_ScrollEndComparer;

	public GameObject m_ScrollEndPos;

	protected GameObject m_ScrollEndPosParent;

	public float m_ScrollEndPosOffsetYAmount;

	protected float m_MinPosX;

	protected float m_MaxPosX = 5000f;

	protected float m_MinHardLimit = 100f;

	protected float m_MaxHardLimit = 100f;

	private float m_ScrollSpeed = 1f;

	private float m_MouseWheelScrollSpeed = 100f;

	private float m_GamepadScrollSpeed = 50f;

	protected float m_LerpPosX;

	protected float m_PosX;

	private bool m_LastEnableOverrideCameraCanDrag;

	protected override void Awake()
	{
		base.Awake();
		m_DefaultMinimizedY = m_AnimGrp.offsetMax.y;
	}

	protected override void Start()
	{
		base.Start();
		m_ScreenRatio = 1920f / (float)Screen.height;
		m_ScrollSpeed = Screen.width;
		m_MouseWheelScrollSpeed = 96f;
	}

	protected override void Init()
	{
		base.Init();
		if (m_EvalSliderMaxOnInit)
		{
			StartCoroutine(EvaluateActiveRestockUIScroller());
		}
	}

	protected IEnumerator EvaluateActiveRestockUIScroller()
	{
		yield return new WaitForSeconds(0.01f);
		m_MaxPosX = 90000f;
		m_MaxPosFound = false;
		m_MaxPosAccurateFound = false;
		if (!m_ScrollEndPosParent)
		{
			m_ScrollEndPos.transform.SetParent(m_ScrollEndComparer.transform, worldPositionStays: false);
			m_ScrollEndPos.transform.localPosition = Vector3.zero;
			m_UseAccurateMaxPos = false;
		}
		else
		{
			m_ScrollEndPos.transform.SetParent(m_ScrollEndPosParent.transform, worldPositionStays: false);
			m_ScrollEndPos.transform.localPosition = new Vector3(0f, m_ScrollEndPosOffsetYAmount, 0f);
		}
	}

	protected override void RunUpdate()
	{
		base.RunUpdate();
		GameInstance.m_CanDragMainMenuSlider = false;
		EvaluateScrollMaxPos();
		EvaluateScreenDrag();
	}

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
		if (m_EvalSliderMaxOnOpen)
		{
			StartCoroutine(EvaluateActiveRestockUIScroller());
		}
		if (m_ResetScrollPosOnOpen)
		{
			m_PosX = 0f;
			m_LerpPosX = 0f;
		}
		UpdateSliderGroupPosition();
		StartCoroutine(DelayCanEvaluateMaxScrollPos());
		GameInstance.m_CanDragMainMenuSlider = false;
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
		m_IsScreenOpen = false;
		m_CanEvaluateMaxScrollPos = false;
		GameInstance.m_CanDragMainMenuSlider = true;
		m_ScreenGroup.SetActive(value: false);
	}

	protected IEnumerator DelayCanEvaluateMaxScrollPos()
	{
		yield return new WaitForSeconds(0.05f);
		if (m_ScrollEndPos.transform.position.y > m_ScrollEndComparer.transform.position.y)
		{
			m_UseAccurateMaxPos = false;
		}
		else
		{
			m_UseAccurateMaxPos = true;
		}
		m_CanEvaluateMaxScrollPos = true;
	}

	protected void EvaluateScreenDrag()
	{
		m_PosX += CSingleton<TouchManager>.Instance.m_DragRatioY * m_ScrollSpeed * m_ScrollSpeedMultiplier;
		if (m_MouseWheelScrollInverted)
		{
			m_PosX += Input.mouseScrollDelta.y * m_MouseWheelScrollSpeed * m_ScrollSpeedMultiplier;
			if (!CSingleton<CGameManager>.Instance.m_DisableController && CSingleton<InputManager>.Instance.m_IsControllerActive)
			{
				m_PosX += CSingleton<InputManager>.Instance.m_CurrentGamepad.rightStick.y.value * m_GamepadScrollSpeed * m_ScrollSpeedMultiplier;
			}
		}
		else
		{
			m_PosX -= Input.mouseScrollDelta.y * m_MouseWheelScrollSpeed * m_ScrollSpeedMultiplier;
			if (!CSingleton<CGameManager>.Instance.m_DisableController && CSingleton<InputManager>.Instance.m_IsControllerActive)
			{
				m_PosX -= CSingleton<InputManager>.Instance.m_CurrentGamepad.rightStick.y.value * m_GamepadScrollSpeed * m_ScrollSpeedMultiplier;
			}
		}
		if (!CSingleton<TouchManager>.Instance.m_IsPressed)
		{
			m_PosX = Mathf.Clamp(m_PosX, m_MinPosX, m_MaxPosX);
		}
		else
		{
			m_PosX = Mathf.Clamp(m_PosX, m_MinPosX - m_MinHardLimit * m_ScrollSpeedMultiplier, m_MaxPosX + m_MaxHardLimit * m_ScrollSpeedMultiplier);
		}
		if (TouchManager.IsSnappingToFinger())
		{
			m_PosX = m_LerpPosX;
		}
		m_LerpPosX = Mathf.Lerp(m_LerpPosX, m_PosX, Time.deltaTime * CSingleton<TouchManager>.Instance.m_LerpSpeed);
		UpdateSliderGroupPosition();
	}

	protected void UpdateSliderGroupPosition()
	{
		m_SliderGrp.transform.localPosition = new Vector3(0f, m_LerpPosX, 0f);
	}

	protected void EvaluateScrollMaxPos()
	{
		if (!m_CanEvaluateMaxScrollPos || (m_MaxPosFound && m_MaxPosAccurateFound))
		{
			return;
		}
		float num = m_ScrollEndComparer.transform.position.y - m_ScrollEndPos.transform.position.y;
		if (!m_MaxPosFound && num <= 0f)
		{
			m_MaxPosFound = true;
			m_MaxPosX = m_SliderGrp.transform.localPosition.y;
			if (m_MaxPosX < 0f)
			{
				m_MaxPosX = 0f;
			}
		}
		if (!m_UseAccurateMaxPos)
		{
			m_MaxPosAccurateFound = true;
		}
		int num2 = 0;
		while (m_MaxPosFound && !m_MaxPosAccurateFound && num2 < 1000)
		{
			num2++;
			if (num < 0f)
			{
				m_SliderGrp.transform.localPosition -= new Vector3(0f, 1f * m_ScrollSpeedMultiplier, 0f);
				m_MaxPosX -= 1f * m_ScrollSpeedMultiplier;
				if (Mathf.Abs(num) < 5f * m_ScrollSpeedMultiplier * m_GamepadJoystickSpeedMultiplier)
				{
					m_MaxPosAccurateFound = true;
					m_MaxPosX = m_SliderGrp.transform.localPosition.y;
					m_MaxPosX = Mathf.Clamp(m_MaxPosX, 0f, m_MaxPosXClamp);
				}
				num = m_ScrollEndComparer.transform.position.y - m_ScrollEndPos.transform.position.y;
			}
		}
	}

	public void OnPressMinMaximize()
	{
		if (m_IsMinimized)
		{
			m_IsMinimized = false;
			m_AnimGrp.offsetMax = new Vector2(m_AnimGrp.offsetMax.x, m_DefaultMinimizedY);
		}
		else
		{
			m_IsMinimized = true;
			m_AnimGrp.offsetMax = new Vector2(m_AnimGrp.offsetMax.x, m_MaximizePosY);
		}
		m_MaxPosFound = false;
		m_MaxPosAccurateFound = false;
		m_MaxPosX = 90000f;
	}

	public void ScrollToUIDelayed(GameObject targetUI, bool instantSnapToPos, float offsetY)
	{
		StartCoroutine(DelayScroll(targetUI, instantSnapToPos, offsetY));
	}

	private IEnumerator DelayScroll(GameObject targetUI, bool instantSnapToPos, float offsetY)
	{
		yield return new WaitForSeconds(0.02f);
		ScrollToUI(targetUI, instantSnapToPos, offsetY);
	}

	public void ScrollToUI(GameObject targetUI, bool instantSnapToPos, float offsetY)
	{
		bool flag = false;
		float lerpPosX = m_LerpPosX;
		int num = 0;
		float num2 = m_ScrollEndComparer.transform.position.y - targetUI.transform.position.y;
		while (!flag && num < 10000)
		{
			num++;
			if (num2 < 0f)
			{
				m_PosX -= Mathf.Clamp(Mathf.Abs(num2) * Time.deltaTime, 1f, 30f);
				m_LerpPosX = m_PosX;
				UpdateSliderGroupPosition();
				num2 = m_ScrollEndComparer.transform.position.y - targetUI.transform.position.y + offsetY;
				if (Mathf.Abs(num2) < 5f * m_ScrollSpeedMultiplier * m_GamepadJoystickSpeedMultiplier)
				{
					flag = true;
				}
			}
			else if (num2 > 0f)
			{
				m_PosX += Mathf.Clamp(Mathf.Abs(num2) * Time.deltaTime, 1f, 30f);
				m_LerpPosX = m_PosX;
				UpdateSliderGroupPosition();
				num2 = m_ScrollEndComparer.transform.position.y - targetUI.transform.position.y + offsetY;
				if (Mathf.Abs(num2) < 5f * m_ScrollSpeedMultiplier * m_GamepadJoystickSpeedMultiplier)
				{
					flag = true;
				}
			}
			if (m_PosX <= m_MinPosX || m_PosX >= m_MaxPosX)
			{
				flag = true;
			}
		}
		if (!instantSnapToPos)
		{
			m_LerpPosX = lerpPosX;
		}
		UpdateSliderGroupPosition();
	}
}
