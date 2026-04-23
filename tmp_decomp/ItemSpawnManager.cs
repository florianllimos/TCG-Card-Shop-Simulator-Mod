using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : CSingleton<ItemSpawnManager>
{
	public static ItemSpawnManager m_Instance;

	public Item m_ItemPrefab;

	public Transform m_ItemParentGrp;

	private int m_SpawnedItemCount;

	private List<Item> m_ItemList = new List<Item>();

	private Stack<Item> m_FreeItems = new Stack<Item>();

	private void Start()
	{
		for (int i = 0; i < m_ItemParentGrp.childCount; i++)
		{
			Item component = m_ItemParentGrp.GetChild(i).GetComponent<Item>();
			if ((bool)component)
			{
				component.gameObject.SetActive(value: false);
				m_ItemList.Add(component);
				m_FreeItems.Push(component);
			}
		}
	}

	public static Item GetItem(Transform parent)
	{
		Item orCreateItem = CSingleton<ItemSpawnManager>.Instance.GetOrCreateItem();
		orCreateItem.transform.SetParent(parent, worldPositionStays: false);
		return orCreateItem;
	}

	private Item GetOrCreateItem()
	{
		Item item;
		if (m_FreeItems.Count > 0)
		{
			item = m_FreeItems.Pop();
		}
		else
		{
			item = Object.Instantiate(m_ItemPrefab, m_ItemParentGrp);
			item.name = $"Item_{m_SpawnedItemCount++}";
			m_ItemList.Add(item);
		}
		return item;
	}

	public static void DisableItem(Item item)
	{
		CSingleton<ItemSpawnManager>.Instance.ReturnToPool(item);
	}

	private void ReturnToPool(Item item)
	{
		item.enabled = false;
		item.transform.SetParent(m_ItemParentGrp, worldPositionStays: false);
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
		item.transform.localScale = Vector3.one;
		item.gameObject.SetActive(value: false);
		item.m_Mesh.enabled = true;
		m_FreeItems.Push(item);
	}
}
