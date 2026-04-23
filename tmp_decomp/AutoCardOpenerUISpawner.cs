using System.Collections.Generic;
using UnityEngine;

public class AutoCardOpenerUISpawner : CSingleton<AutoCardOpenerUISpawner>
{
	public static AutoCardOpenerUISpawner m_Instance;

	public AutoCardOpenerUI m_AutoCardOpenerUIPrefab;

	private int m_SpawnedCardCount;

	private List<AutoCardOpenerUI> m_AutoCardOpenerUIList = new List<AutoCardOpenerUI>();

	private List<Card3dUIGroup> m_AllCard3dUIList = new List<Card3dUIGroup>();

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
		m_AutoCardOpenerUIList = new List<AutoCardOpenerUI>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			m_AutoCardOpenerUIList.Add(base.transform.GetChild(i).gameObject.GetComponent<AutoCardOpenerUI>());
		}
		for (int j = 0; j < 5; j++)
		{
			AddAutoCardOpenerUIPrefab();
		}
		for (int k = 0; k < m_AutoCardOpenerUIList.Count; k++)
		{
			m_AutoCardOpenerUIList[k].gameObject.SetActive(value: false);
		}
	}

	public AutoCardOpenerUI GetAutoCardOpenerUI()
	{
		for (int i = 0; i < m_AutoCardOpenerUIList.Count; i++)
		{
			if (!m_AutoCardOpenerUIList[i].gameObject.activeSelf)
			{
				m_AutoCardOpenerUIList[i].gameObject.SetActive(value: true);
				return m_AutoCardOpenerUIList[i];
			}
		}
		AutoCardOpenerUI autoCardOpenerUI = AddAutoCardOpenerUIPrefab();
		autoCardOpenerUI.gameObject.SetActive(value: true);
		return autoCardOpenerUI;
	}

	private AutoCardOpenerUI AddAutoCardOpenerUIPrefab()
	{
		AutoCardOpenerUI autoCardOpenerUI = Object.Instantiate(m_AutoCardOpenerUIPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform);
		autoCardOpenerUI.transform.localRotation = Quaternion.identity;
		autoCardOpenerUI.name = "AutoCardOpenerUIGrp_" + m_SpawnedCardCount;
		autoCardOpenerUI.gameObject.SetActive(value: false);
		m_AutoCardOpenerUIList.Add(autoCardOpenerUI);
		m_SpawnedCardCount++;
		return autoCardOpenerUI;
	}
}
