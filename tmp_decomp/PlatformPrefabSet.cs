using System;
using GA;
using UnityEngine;

[CreateAssetMenu(fileName = "PlatformPrefabSet", menuName = "Multiplatform/Prefab Set", order = 1)]
public class PlatformPrefabSet : ScriptableObject
{
	[Serializable]
	public class PrefabEntry
	{
		public string m_PrefabName;

		public GameObject m_XboxPrefab;

		public GameObject m_PSPrefab;

		public GameObject m_MSStorePrefab;
	}

	public PrefabEntry[] m_Prefabs;

	public GameObject GetPrefabForPlatform(string prefabName, EPlatform platform)
	{
		PrefabEntry[] prefabs = m_Prefabs;
		foreach (PrefabEntry prefabEntry in prefabs)
		{
			if (prefabEntry.m_PrefabName == prefabName)
			{
				switch (platform)
				{
				case EPlatform.MSStore:
					return prefabEntry.m_MSStorePrefab;
				case EPlatform.XboxScarlett:
				case EPlatform.XboxOne:
					return prefabEntry.m_XboxPrefab;
				case EPlatform.PS4:
				case EPlatform.PS5:
					return prefabEntry.m_PSPrefab;
				}
			}
		}
		Debug.LogWarning("PlatformPrefabSet: prefab '" + prefabName + "' not found");
		return null;
	}
}
