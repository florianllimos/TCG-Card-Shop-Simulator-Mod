using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SliderUI : MonoBehaviour
{
	public TextMeshProUGUI m_PercentText;

	public float m_PercentMultiplier = 255f;

	public float m_DecimalAmount;

	public RectTransform m_SliderIcon;

	public RectTransform m_StartPoint;

	public RectTransform m_EndPoint;

	private float m_Percent;

	public SliderPercentEvent m_SliderEvent;

	public UnityEvent m_OnReleaseEvent;

	private void Awake()
	{
	}

	public void OnSliderPress(int index)
	{
		EvaluateSlider();
	}

	public void OnSliderDrag(int index)
	{
		EvaluateSlider();
	}

	public void OnRelease()
	{
		m_OnReleaseEvent.Invoke();
	}

	private void EvaluateSlider()
	{
		float num = m_EndPoint.position.x - m_StartPoint.position.x;
		float x = Input.mousePosition.x;
		x = Mathf.Clamp(x, m_StartPoint.position.x, m_EndPoint.position.x);
		m_Percent = (x - m_StartPoint.position.x) / num;
		m_SliderIcon.position = new Vector3(x, m_SliderIcon.position.y, 0f);
		m_SliderEvent.Invoke(m_Percent);
		EvaluateText();
	}

	private void EvaluateText()
	{
		if (m_DecimalAmount <= 0f)
		{
			int num = Mathf.FloorToInt(m_Percent * 100f * m_PercentMultiplier / 100f);
			m_PercentText.text = num.ToString();
		}
		else
		{
			float num2 = (float)Mathf.FloorToInt(m_Percent * Mathf.Pow(10f, m_DecimalAmount) * m_PercentMultiplier) / Mathf.Pow(10f, m_DecimalAmount);
			m_PercentText.text = num2.ToString();
		}
	}

	public void SetSliderPosByPercent(float percent)
	{
		m_Percent = percent;
		float value = Mathf.Lerp(m_StartPoint.position.x, m_EndPoint.position.x, percent);
		value = Mathf.Clamp(value, m_StartPoint.position.x, m_EndPoint.position.x);
		m_SliderIcon.position = new Vector3(value, m_SliderIcon.position.y, 0f);
		EvaluateText();
	}

	private void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(CPlayer_OnStartGame);
			CEventManager.AddListener<CEventPlayer_ChangeScene>(OnOpenLoadingScreen);
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(CPlayer_OnStartGame);
			CEventManager.RemoveListener<CEventPlayer_ChangeScene>(OnOpenLoadingScreen);
		}
	}

	private void CPlayer_OnStartGame(CEventPlayer_GameDataFinishLoaded evt)
	{
	}

	private void OnOpenLoadingScreen(CEventPlayer_ChangeScene evt)
	{
	}
}
