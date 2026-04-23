using System.Collections.Generic;
using UnityEngine;

public class PlayCardPool : CSingleton<PlayCardPool>
{
	public static PlayCardPool m_Instance;

	public List<PlayCard> m_PlayCardPoolList;

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(this);
		for (int i = 0; i < m_PlayCardPoolList.Count; i++)
		{
			m_PlayCardPoolList[i].Init();
		}
	}

	public PlayCard FindPlayCard()
	{
		for (int i = 0; i < m_PlayCardPoolList.Count; i++)
		{
			if (!m_PlayCardPoolList[i].gameObject.activeSelf)
			{
				m_PlayCardPoolList[i].gameObject.SetActive(value: true);
				return m_PlayCardPoolList[i];
			}
		}
		return null;
	}
}
