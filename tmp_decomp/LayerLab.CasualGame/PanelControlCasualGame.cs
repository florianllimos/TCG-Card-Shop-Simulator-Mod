using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LayerLab.CasualGame;

public class PanelControlCasualGame : MonoBehaviour
{
	private int page;

	private bool isReady;

	[SerializeField]
	private List<GameObject> panels = new List<GameObject>();

	private TextMeshProUGUI textTitle;

	[SerializeField]
	private Transform panelTransform;

	[SerializeField]
	private Button buttonPrev;

	[SerializeField]
	private Button buttonNext;

	private void Start()
	{
		textTitle = base.transform.GetComponentInChildren<TextMeshProUGUI>();
		buttonPrev.onClick.AddListener(Click_Prev);
		buttonNext.onClick.AddListener(Click_Next);
		foreach (Transform item in panelTransform)
		{
			panels.Add(item.gameObject);
			item.gameObject.SetActive(value: false);
		}
		panels[page].SetActive(value: true);
		isReady = true;
		CheckControl();
	}

	private void Update()
	{
		if (panels.Count > 0 && isReady)
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				Click_Prev();
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				Click_Next();
			}
		}
	}

	public void Click_Prev()
	{
		if (page > 0 && isReady)
		{
			panels[page].SetActive(value: false);
			panels[--page].SetActive(value: true);
			textTitle.text = panels[page].name;
			CheckControl();
		}
	}

	public void Click_Next()
	{
		if (page < panels.Count - 1)
		{
			panels[page].SetActive(value: false);
			panels[++page].SetActive(value: true);
			CheckControl();
		}
	}

	private void SetArrowActive()
	{
		buttonPrev.gameObject.SetActive(page > 0);
		buttonNext.gameObject.SetActive(page < panels.Count - 1);
	}

	private void CheckControl()
	{
		textTitle.text = panels[page].name.Replace("_", " ");
		SetArrowActive();
	}
}
