using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedRoomBlocker : MonoBehaviour
{
	public bool m_IsFrontRow;

	public GameObject m_Wall;

	public GameObject m_Building;

	public Animation m_Anim;

	public int m_MainRoomIndex = -1;

	public int m_StoreRoomIndex = -1;

	public List<MeshRenderer> m_WallMeshRendererList;

	public List<MeshRenderer> m_WallMeshRendererList_NoBar;

	public GameObject m_ShopWall_WithBar;

	public GameObject m_ShopWall_NoBar;

	private bool m_IsShopLotB;

	public List<InteractableObject> m_VerticalDecoObjectList = new List<InteractableObject>();

	private void Awake()
	{
		m_Wall.SetActive(!m_IsFrontRow);
		m_Building.SetActive(m_IsFrontRow);
	}

	public void HideBlocker()
	{
		m_Anim.Play();
		if (m_VerticalDecoObjectList.Count > 0)
		{
			for (int num = m_VerticalDecoObjectList.Count - 1; num >= 0; num--)
			{
				m_VerticalDecoObjectList[num].OnLockedRoomHidden();
			}
		}
		m_VerticalDecoObjectList.Clear();
	}

	private IEnumerator DelayHide()
	{
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(value: false);
	}

	public void AddToVerticalDecoObjectList(InteractableObject decoObject)
	{
		if (!m_VerticalDecoObjectList.Contains(decoObject))
		{
			m_VerticalDecoObjectList.Add(decoObject);
		}
	}

	public void RemoveFromVerticalDecoObjectList(InteractableObject decoObject)
	{
		m_VerticalDecoObjectList.Remove(decoObject);
	}

	public void UpdateShopLotBWallMaterial(Material mat1, Material mat2)
	{
		m_IsShopLotB = true;
		for (int i = 0; i < m_WallMeshRendererList.Count; i++)
		{
			Material[] materials = new Material[3] { mat1, mat2, mat1 };
			m_WallMeshRendererList[i].materials = materials;
		}
		for (int j = 0; j < m_WallMeshRendererList_NoBar.Count; j++)
		{
			Material[] materials2 = new Material[1] { mat1 };
			m_WallMeshRendererList_NoBar[j].materials = materials2;
		}
	}

	public void SetWallBarVisibility(bool isVisible, bool isShopLotB)
	{
		if (m_IsShopLotB == isShopLotB)
		{
			m_ShopWall_WithBar.SetActive(isVisible);
			m_ShopWall_NoBar.SetActive(!isVisible);
		}
	}
}
