using System.Collections.Generic;
using UnityEngine;

public class ControllerSelectorUIGrp : MonoBehaviour
{
	public RectTransform m_Rect;

	public GameObject m_TopUIGrp;

	public List<Transform> m_SpriteGrp;

	public Animation m_PopupAnim;

	private void Start()
	{
		m_TopUIGrp.SetActive(value: false);
	}

	public void SetActive(bool isActive)
	{
		if (Time.timeScale <= 0f)
		{
			m_PopupAnim.enabled = false;
			m_TopUIGrp.transform.localScale = Vector3.one;
		}
		else
		{
			m_PopupAnim.enabled = true;
		}
		m_TopUIGrp.SetActive(isActive);
	}

	public void SetSpriteGrpScale(float scale)
	{
		if (Time.timeScale <= 0f)
		{
			m_PopupAnim.enabled = false;
			m_TopUIGrp.transform.localScale = Vector3.one;
		}
		else
		{
			m_PopupAnim.enabled = true;
		}
		for (int i = 0; i < m_SpriteGrp.Count; i++)
		{
			m_SpriteGrp[i].localScale = Vector3.one * scale;
		}
	}
}
