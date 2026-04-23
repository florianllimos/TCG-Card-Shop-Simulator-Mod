using TMPro;
using UnityEngine;

public class EndDayReportTextUI : MonoBehaviour
{
	public TextMeshProUGUI m_Text;

	public bool m_IsInt;

	public bool m_IsPrice;

	public bool m_IsTime;

	public bool m_IsShowPlusSymbol;

	private bool m_IsLerpEnded;

	private float m_LerpNumber;

	private float m_LerpStart;

	private float m_LerpTarget;

	private float m_LerpTimer;

	private float m_LerpTime;

	public void SetNumber(float start, float end, float lerpTime, Color positiveColor, Color negativeColor, Color neutralColor)
	{
		m_IsLerpEnded = false;
		m_LerpTimer = 0f;
		m_LerpTime = lerpTime;
		if (m_LerpTime <= 0f)
		{
			m_LerpTime = 0.01f;
		}
		m_LerpStart = start;
		m_LerpTarget = end;
		if (m_LerpStart == m_LerpTarget)
		{
			m_LerpTime /= 2f;
		}
		base.gameObject.SetActive(value: false);
		if (end > 0f)
		{
			m_Text.color = positiveColor;
		}
		else if (end < 0f)
		{
			m_Text.color = negativeColor;
		}
		else
		{
			m_Text.color = neutralColor;
		}
	}

	public void UpdateLerp()
	{
		if (m_IsLerpEnded)
		{
			return;
		}
		if (m_LerpTimer == 0f)
		{
			base.gameObject.SetActive(value: true);
			if (m_LerpStart != m_LerpTarget)
			{
				SoundManager.SetEnableSound_CoinIncrease(isEnable: true);
			}
		}
		m_LerpTimer += Time.deltaTime / m_LerpTime;
		m_LerpNumber = Mathf.Lerp(m_LerpStart, m_LerpTarget, m_LerpTimer);
		if (m_IsTime)
		{
			m_Text.text = GameInstance.GetTimeString(Mathf.FloorToInt(m_LerpNumber), showDay: false, showHour: true, showMinutes: true, showSeconds: false, removeZero: false, convertDayToHour: true);
		}
		else if (m_IsInt)
		{
			if (m_IsShowPlusSymbol && m_LerpNumber > 0f)
			{
				m_Text.text = "+" + Mathf.FloorToInt(m_LerpNumber);
			}
			else
			{
				m_Text.text = Mathf.FloorToInt(m_LerpNumber).ToString();
			}
		}
		else if (m_IsPrice)
		{
			if (m_IsShowPlusSymbol && m_LerpNumber > 0f)
			{
				m_Text.text = "+" + GameInstance.GetPriceString(m_LerpNumber);
			}
			else
			{
				m_Text.text = GameInstance.GetPriceString(m_LerpNumber);
			}
		}
		else if (m_IsShowPlusSymbol && m_LerpNumber > 0f)
		{
			m_Text.text = "+" + m_LerpNumber.ToString("F2");
		}
		else
		{
			m_Text.text = m_LerpNumber.ToString("F2");
		}
		if (m_LerpTimer >= 1f)
		{
			m_IsLerpEnded = true;
			SoundManager.SetEnableSound_CoinIncrease(isEnable: false);
		}
	}

	public void EndLerpInstantly()
	{
		base.gameObject.SetActive(value: true);
		m_LerpTimer = 1f;
		UpdateLerp();
	}

	public bool IsLerpEnded()
	{
		return m_IsLerpEnded;
	}
}
