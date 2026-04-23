using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycasterManager : CSingleton<RaycasterManager>
{
	public List<GraphicRaycaster> m_RaycasterList = new List<GraphicRaycaster>();

	public List<GraphicRaycaster> m_CustomRaycasterList = new List<GraphicRaycaster>();

	public List<HorizontalLayoutGroup> m_HLayoutGrpList = new List<HorizontalLayoutGroup>();

	public List<VerticalLayoutGroup> m_VLayoutGrpList = new List<VerticalLayoutGroup>();

	public List<HorizontalLayoutGroup> m_HLayoutGrpExceptionList = new List<HorizontalLayoutGroup>();

	private void Start()
	{
		GraphicRaycaster[] componentsInChildren = base.gameObject.GetComponentsInChildren<GraphicRaycaster>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			m_RaycasterList.Add(componentsInChildren[i]);
		}
		for (int j = 0; j < m_CustomRaycasterList.Count; j++)
		{
			m_RaycasterList.Add(m_CustomRaycasterList[j]);
		}
		HorizontalLayoutGroup[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<HorizontalLayoutGroup>();
		for (int k = 0; k < componentsInChildren2.Length; k++)
		{
			if (!m_HLayoutGrpExceptionList.Contains(componentsInChildren2[k]))
			{
				m_HLayoutGrpList.Add(componentsInChildren2[k]);
			}
		}
		VerticalLayoutGroup[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<VerticalLayoutGroup>();
		for (int l = 0; l < componentsInChildren3.Length; l++)
		{
			m_VLayoutGrpList.Add(componentsInChildren3[l]);
		}
	}

	private IEnumerator DisableLayoutGrp()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < m_HLayoutGrpList.Count; i++)
		{
			m_HLayoutGrpList[i].enabled = false;
		}
		for (int j = 0; j < m_VLayoutGrpList.Count; j++)
		{
			m_VLayoutGrpList[j].enabled = false;
		}
	}

	public static void SetUIRaycastEnabled(bool isEnabled)
	{
		for (int i = 0; i < CSingleton<RaycasterManager>.Instance.m_RaycasterList.Count; i++)
		{
			CSingleton<RaycasterManager>.Instance.m_RaycasterList[i].enabled = isEnabled;
		}
	}
}
