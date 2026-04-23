using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : CSingleton<PoolingManager>
{
	public PoolFX_ScriptableObject m_PoolFXScriptableObject;

	private static Vector3 m_DefaultPos;

	private GameObject m_PoolingObjectGroup;

	private static List<GameObject> m_SlashHitFXList;

	private static List<GameObject> m_ExplosionFXList;

	private GameObject objectInst;

	private int m_SlashHitFXInitCount;

	private int m_ExplosionFXInitCount = 10;

	private GameObject m_SlashHitFXPrefab;

	private GameObject m_ExplosionFXPrefab;

	protected PoolingManager()
	{
	}

	private void Start()
	{
		m_SlashHitFXList = new List<GameObject>();
		m_ExplosionFXList = new List<GameObject>();
		m_SlashHitFXPrefab = m_PoolFXScriptableObject.m_SlashHitFXPrefab;
		m_ExplosionFXPrefab = m_PoolFXScriptableObject.m_ExplosionFXPrefab;
		m_DefaultPos = new Vector3(9999f, 9999f, 0f);
		StartCoroutine(SpawnInitialPool());
	}

	private void ParentObjectToGroup(GameObject poolObject)
	{
		poolObject.transform.SetParent(m_PoolingObjectGroup.transform);
	}

	private IEnumerator SpawnInitialPool()
	{
		float waitTime = 0.02f;
		m_PoolingObjectGroup = new GameObject("PoolingObject_Grp");
		Object.DontDestroyOnLoad(m_PoolingObjectGroup);
		yield return new WaitForSeconds(waitTime);
		for (int i = 0; i < m_SlashHitFXInitCount; i++)
		{
			objectInst = Object.Instantiate(m_SlashHitFXPrefab, m_DefaultPos, base.transform.rotation);
			objectInst.SetActive(value: false);
			ParentObjectToGroup(objectInst);
			m_SlashHitFXList.Add(objectInst);
		}
		yield return new WaitForSeconds(waitTime);
		for (int j = 0; j < m_ExplosionFXInitCount; j++)
		{
			objectInst = Object.Instantiate(m_ExplosionFXPrefab, m_DefaultPos, base.transform.rotation);
			objectInst.SetActive(value: false);
			ParentObjectToGroup(objectInst);
			m_ExplosionFXList.Add(objectInst);
		}
	}

	public static GameObject ActivateObject(PoolObjectType objectType, Vector3 position, Quaternion rotation, Transform followTarget = null)
	{
		List<GameObject> list = new List<GameObject>();
		List<FollowObject> list2 = new List<FollowObject>();
		switch (objectType)
		{
		case PoolObjectType.slashHitFX:
			list = m_SlashHitFXList;
			break;
		case PoolObjectType.explosionFX:
			list = m_ExplosionFXList;
			break;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[i].activeSelf)
			{
				list[i].transform.position = position;
				list[i].transform.rotation = rotation;
				list[i].SetActive(value: true);
				if ((bool)followTarget)
				{
					list2[i].SetFollowTarget(followTarget);
				}
				return list[i];
			}
		}
		return null;
	}

	public static void DeactivateObject(GameObject gameobject)
	{
		gameobject.transform.position = m_DefaultPos;
		gameobject.SetActive(value: false);
	}
}
