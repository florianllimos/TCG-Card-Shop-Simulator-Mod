using System.Collections;
using UnityEngine;

public class SliderManager : MonoBehaviour
{
	public bool m_ScrollYAxis = true;

	private float m_ScrollItemOffsetX = 0.4f;

	public float m_ScrollButtonOffsetX = 1800f;

	private float m_ScrollItemOffsetY = 0.4f;

	public float m_ScrollButtonOffsetY = 800f;

	public float m_InitPosY = -50f;

	public int m_CurrentItemIndex;

	public int m_CurrentBtnSetIndex;

	private int m_PreviousBtnSetIndex;

	private int m_PreviousItemIndex;

	private float m_MaxScrollX;

	private float m_MaxScrollY;

	private bool m_IsMouseDown;

	private bool m_IsSwiping;

	private Vector2 m_MouseStartPos;

	private Vector2 m_MouseFirstStartPos;

	public GameObject m_Slider;

	private LerpSlider m_LerpSlider;

	private Vector3 m_SliderStartPos;

	public int m_MaxItemInABox = 12;

	private bool m_IsEnabled;

	private void Awake()
	{
		m_LerpSlider = m_Slider.GetComponent<LerpSlider>();
		m_PreviousBtnSetIndex = m_CurrentBtnSetIndex;
	}

	private void Start()
	{
		m_SliderStartPos = m_Slider.transform.localPosition;
		EvaluateScrollerPosition();
	}

	private IEnumerator DelayEnable()
	{
		m_IsEnabled = false;
		m_IsMouseDown = false;
		yield return new WaitForSeconds(0.1f);
		m_IsEnabled = true;
		m_IsMouseDown = false;
	}

	private void Update()
	{
		EvaluateMouseSlider();
	}

	private void EvaluateMouseSlider()
	{
		if (!m_Slider.activeSelf || !m_IsEnabled)
		{
			return;
		}
		if (InputManager.GetKeyDownAction(EGameAction.InteractLeft))
		{
			m_MouseStartPos = Input.mousePosition;
			m_MouseFirstStartPos = m_MouseStartPos;
			m_IsMouseDown = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			m_IsMouseDown = false;
			EvaluateScrollerPosition();
		}
		if (!m_IsMouseDown)
		{
			return;
		}
		float num = Input.mousePosition.x - m_MouseStartPos.x;
		float value = m_Slider.transform.localPosition.x + num * 1.5f;
		float num2 = Input.mousePosition.y - m_MouseStartPos.y;
		float value2 = m_Slider.transform.localPosition.y + num2 * 1.5f;
		if (m_ScrollYAxis)
		{
			m_MaxScrollY = (float)(m_MaxItemInABox - 1) * m_ScrollButtonOffsetY;
			value2 = Mathf.Clamp(value2, m_InitPosY, m_MaxScrollY);
			m_Slider.transform.localPosition = new Vector3(m_SliderStartPos.x, value2, 0f);
			m_LerpSlider.SetLerpPosY(value2);
			m_MouseStartPos = Input.mousePosition;
			if (num2 > 0f)
			{
				m_CurrentBtnSetIndex = Mathf.CeilToInt((value2 - m_ScrollButtonOffsetY * 0.1f) / m_ScrollButtonOffsetY);
			}
			else
			{
				m_CurrentBtnSetIndex = Mathf.CeilToInt((value2 - m_ScrollButtonOffsetY * 0.9f) / m_ScrollButtonOffsetY);
			}
		}
		else
		{
			m_MaxScrollX = (float)(m_MaxItemInABox - 1) * (0f - m_ScrollButtonOffsetX);
			value = Mathf.Clamp(value, m_MaxScrollX, 0f);
			m_Slider.transform.localPosition = new Vector3(value, m_SliderStartPos.y, 0f);
			m_LerpSlider.SetLerpPos(value);
			m_MouseStartPos = Input.mousePosition;
			if (num > 0f)
			{
				m_CurrentBtnSetIndex = Mathf.CeilToInt((value - m_ScrollButtonOffsetX * 0.1f) / m_ScrollButtonOffsetX) * -1;
			}
			else
			{
				m_CurrentBtnSetIndex = Mathf.CeilToInt((value - m_ScrollButtonOffsetX * 0.9f) / m_ScrollButtonOffsetX) * -1;
			}
		}
		if (m_CurrentBtnSetIndex != m_PreviousBtnSetIndex)
		{
			m_PreviousBtnSetIndex = m_CurrentBtnSetIndex;
		}
	}

	private void EvaluateScrollerPosition()
	{
		if (m_ScrollYAxis)
		{
			float num = (0f - m_ScrollItemOffsetY) * (float)m_CurrentBtnSetIndex;
			num = m_ScrollButtonOffsetY * (float)m_CurrentBtnSetIndex;
			m_LerpSlider.SetLerpPosY(num);
		}
		else
		{
			float num2 = (0f - m_ScrollItemOffsetX) * (float)m_CurrentBtnSetIndex;
			num2 = (0f - m_ScrollButtonOffsetX) * (float)m_CurrentBtnSetIndex;
			m_LerpSlider.SetLerpPos(num2);
		}
	}

	private void EvaluateItemSliderChanged()
	{
	}

	private void OnEnable()
	{
		StartCoroutine(DelayEnable());
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_TouchScreen>(CPlayer_OnTouchScreen);
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_TouchScreen>(CPlayer_OnTouchScreen);
		}
	}

	private void CPlayer_OnTouchScreen(CEventPlayer_TouchScreen evt)
	{
	}
}
