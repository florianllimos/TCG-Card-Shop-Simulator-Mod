using System.Collections.Generic;
using UnityEngine;

public class CardFanRandomizer : MonoBehaviour
{
	public List<GameObject> m_CardFanGrpList;

	public List<Material> m_PlayCardMatList;

	public List<MeshRenderer> m_MeshRendererList;

	private void OnEnable()
	{
		RandomizePlayCard();
	}

	public void RandomizePlayCard()
	{
		for (int i = 0; i < m_CardFanGrpList.Count; i++)
		{
			m_CardFanGrpList[i].SetActive(value: false);
		}
		m_CardFanGrpList[Random.Range(0, m_CardFanGrpList.Count)].SetActive(value: true);
		Material material = m_PlayCardMatList[Random.Range(0, m_PlayCardMatList.Count)];
		Material material2 = m_PlayCardMatList[Random.Range(0, m_PlayCardMatList.Count)];
		for (int j = 0; j < m_MeshRendererList.Count; j++)
		{
			m_MeshRendererList[j].material = material;
		}
		m_MeshRendererList[0].material = material2;
	}
}
