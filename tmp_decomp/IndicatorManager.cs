using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : CSingleton<IndicatorManager>
{
	public static IndicatorManager m_Instance;

	public List<ArrowIndicatorUI> m_ArrowIndicatorUIList;

	private List<GameObject> m_ParentList = new List<GameObject>();

	public void Start()
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
		for (int i = 0; i < m_ArrowIndicatorUIList.Count; i++)
		{
			m_ArrowIndicatorUIList[i].gameObject.SetActive(value: false);
			m_ParentList.Add(null);
		}
	}

	private void Update()
	{
		for (int i = 0; i < m_ParentList.Count; i++)
		{
			if ((bool)m_ParentList[i] && !m_ParentList[i].activeInHierarchy)
			{
				m_ArrowIndicatorUIList[i].gameObject.SetActive(value: false);
				m_ArrowIndicatorUIList[i].transform.parent = base.transform;
				m_ParentList[i] = null;
			}
		}
	}

	public static void HideAllIndicator()
	{
		for (int i = 0; i < CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList.Count; i++)
		{
			CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].gameObject.SetActive(value: false);
			CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].transform.parent = CSingleton<IndicatorManager>.Instance.transform;
			CSingleton<IndicatorManager>.Instance.m_ParentList[i] = null;
		}
	}

	public static void ShowArrowIndicator(GameObject parent, float posYOffset = 0f, float scale = 1f)
	{
		for (int i = 0; i < CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList.Count; i++)
		{
			if (!CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].gameObject.activeSelf)
			{
				CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].SetPosY(posYOffset);
				CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].SetScale(scale);
				CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].transform.parent = parent.transform;
				CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].transform.localPosition = Vector3.zero;
				CSingleton<IndicatorManager>.Instance.m_ArrowIndicatorUIList[i].gameObject.SetActive(value: true);
				CSingleton<IndicatorManager>.Instance.m_ParentList[i] = parent;
				break;
			}
		}
	}
}
