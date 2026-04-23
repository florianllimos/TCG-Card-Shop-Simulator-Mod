using TMPro;
using UnityEngine;

public class TutorialSubGroup : MonoBehaviour
{
	public GameObject m_ScreenGrp;

	public TextMeshProUGUI m_NumberText;

	public TutorialData m_TutorialData;

	public bool m_ShowValue = true;

	public bool m_IsInt;

	public bool m_IsPrice;

	private bool m_IsTaskFinish;

	private float m_CurrentValue;

	public float m_MaxValue;

	public void AddTaskValue(float currentValue, ETutorialTaskCondition tutorialTaskCondition)
	{
		if (m_TutorialData.tutorialTaskCondition == tutorialTaskCondition)
		{
			m_TutorialData.value += currentValue;
			m_CurrentValue += currentValue;
			EvaluateValueText();
			if (m_CurrentValue >= m_MaxValue)
			{
				m_IsTaskFinish = true;
			}
		}
	}

	public bool IsTaskFinish()
	{
		return m_IsTaskFinish;
	}

	public void OpenScreen()
	{
		EvaluateValueText();
		base.gameObject.SetActive(value: true);
	}

	public void CloseScreen()
	{
		base.gameObject.SetActive(value: false);
	}

	private void EvaluateValueText()
	{
		if (m_IsInt)
		{
			m_NumberText.text = Mathf.RoundToInt(m_CurrentValue) + "/" + Mathf.RoundToInt(m_MaxValue);
		}
		else
		{
			m_NumberText.text = m_CurrentValue + "/" + m_MaxValue;
		}
		m_NumberText.enabled = m_ShowValue;
	}
}
