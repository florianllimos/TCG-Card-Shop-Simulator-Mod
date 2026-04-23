using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CC;

public class Tab_Manager : MonoBehaviour
{
	[Header("Button Active Colors")]
	public ColorBlock TabColorActive;

	[Header("Button Inactive Colors")]
	public ColorBlock TabColorInactive;

	public GameObject TabParent;

	private List<GameObject> tabs = new List<GameObject>();

	private List<GameObject> tabMenus = new List<GameObject>();

	private void Start()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			int index = i;
			tabs.Add(gameObject);
			gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
			{
				switchTab(index);
			});
		}
		foreach (Transform item in TabParent.transform)
		{
			tabMenus.Add(item.gameObject);
		}
		switchTab(0);
	}

	public void switchTab(int tab)
	{
		for (int i = 0; i < tabs.Count; i++)
		{
			tabs[i].GetComponentInChildren<Button>().colors = ((tab == i) ? TabColorActive : TabColorInactive);
			if (tabMenus.Count > i)
			{
				tabMenus[i].SetActive(tab == i);
			}
		}
	}
}
