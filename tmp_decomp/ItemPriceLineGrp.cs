using UnityEngine;
using UnityEngine.UI;

public class ItemPriceLineGrp : MonoBehaviour
{
	public Image m_Line;

	public Transform m_LineScaleTransform;

	public bool m_IsActive;

	public float m_Lerp;

	private Vector3 m_Scale;

	public void SetActive(bool isActive)
	{
		m_IsActive = isActive;
		base.gameObject.SetActive(isActive);
	}

	public void SetColor(Color color)
	{
		m_Line.color = color;
	}

	public void SetScaleLerp(float lerp)
	{
		m_Lerp = lerp;
		m_Scale = Vector3.one;
		m_Scale.z = m_Lerp;
		m_LineScaleTransform.localScale = m_Scale;
	}

	public void AddScaleLerp(float lerp)
	{
		m_Lerp += lerp;
		if (m_Lerp > 1f)
		{
			m_Lerp = 1f;
		}
		m_Scale = Vector3.one;
		m_Scale.z = m_Lerp;
		m_LineScaleTransform.localScale = m_Scale;
	}
}
