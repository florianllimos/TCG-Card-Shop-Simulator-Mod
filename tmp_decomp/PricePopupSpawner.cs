using System.Collections.Generic;
using UnityEngine;

public class PricePopupSpawner : CSingleton<PricePopupSpawner>
{
	public List<PricePopupUI> m_PricePopupList;

	public Transform m_Cam;

	public Color m_AddMoneyColor;

	public void ShowPricePopup(float price, float offsetUp, Transform followTransform)
	{
		if (CSingleton<InteractionPlayerController>.Instance.IsInUIMode())
		{
			return;
		}
		for (int i = 0; i < m_PricePopupList.Count; i++)
		{
			if (!m_PricePopupList[i].gameObject.activeSelf)
			{
				m_PricePopupList[i].SetFollowTransform(followTransform, offsetUp);
				m_PricePopupList[i].m_Transform.position = m_PricePopupList[i].m_FollowTransform.position + Vector3.up * m_PricePopupList[i].m_OffsetUp;
				m_PricePopupList[i].m_Transform.LookAt(new Vector3(m_Cam.position.x, m_Cam.position.y, m_Cam.position.z));
				m_PricePopupList[i].m_Text.color = m_AddMoneyColor;
				m_PricePopupList[i].m_Text.text = "+" + GameInstance.GetPriceString(price);
				m_PricePopupList[i].gameObject.SetActive(value: true);
				break;
			}
		}
	}

	public void ShowTextPopup(string text, float offsetUp, Transform followTransform)
	{
		if (CSingleton<InteractionPlayerController>.Instance.IsInUIMode())
		{
			return;
		}
		for (int i = 0; i < m_PricePopupList.Count; i++)
		{
			if (!m_PricePopupList[i].gameObject.activeSelf)
			{
				m_PricePopupList[i].SetFollowTransform(followTransform, offsetUp);
				m_PricePopupList[i].m_Transform.position = m_PricePopupList[i].m_FollowTransform.position + Vector3.up * m_PricePopupList[i].m_OffsetUp;
				m_PricePopupList[i].m_Transform.LookAt(new Vector3(m_Cam.position.x, m_Cam.position.y, m_Cam.position.z));
				m_PricePopupList[i].m_Text.color = Color.white;
				m_PricePopupList[i].m_Text.text = text;
				m_PricePopupList[i].gameObject.SetActive(value: true);
				break;
			}
		}
	}

	private void Awake()
	{
		for (int i = 0; i < m_PricePopupList.Count; i++)
		{
			m_PricePopupList[i].gameObject.SetActive(value: false);
			m_PricePopupList[i].Init();
		}
	}

	private void Update()
	{
		for (int i = 0; i < m_PricePopupList.Count; i++)
		{
			if (m_PricePopupList[i].gameObject.activeSelf)
			{
				m_PricePopupList[i].m_Transform.position = m_PricePopupList[i].m_FollowTransform.position + Vector3.up * m_PricePopupList[i].m_OffsetUp;
				m_PricePopupList[i].m_Transform.LookAt(new Vector3(m_Cam.position.x, m_Cam.position.y, m_Cam.position.z));
			}
		}
	}
}
