using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CC;

public class Apparel_Menu : MonoBehaviour, ICustomizerUI
{
	public GameObject ButtonPrefab;

	public GameObject Container;

	public TextMeshProUGUI OptionText;

	private CharacterCustomization customizer;

	private int navIndex;

	private int optionsCount;

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util parentUI)
	{
		customizer = customizerScript;
		RefreshUIElement();
	}

	public void setOption(int i)
	{
		navIndex = i;
		OptionText.text = customizer.ApparelTables[i].Label;
		foreach (Transform item in Container.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (scrObj_Apparel.Apparel item2 in customizer.ApparelTables[i].Items)
		{
			int Slot = i;
			for (int j = 0; j < item2.Materials.Count; j++)
			{
				string name = item2.Name;
				int matIndex = j;
				GameObject obj = Object.Instantiate(ButtonPrefab, Container.transform).gameObject;
				obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
				{
					customizer.setApparelByName(name, Slot, matIndex);
				});
				obj.GetComponentInChildren<TextMeshProUGUI>().text = item2.Materials[j].Label + " " + item2.DisplayName;
			}
			if (item2.Materials.Count == 0)
			{
				string name2 = item2.Name;
				GameObject obj2 = Object.Instantiate(ButtonPrefab, Container.transform).gameObject;
				obj2.GetComponentInChildren<Button>().onClick.AddListener(delegate
				{
					customizer.setApparelByName(name2, Slot, 0);
				});
				obj2.GetComponentInChildren<TextMeshProUGUI>().text = item2.DisplayName;
			}
		}
	}

	public void navLeft()
	{
		setOption((navIndex == 0) ? (optionsCount - 1) : (navIndex - 1));
	}

	public void navRight()
	{
		setOption((navIndex != optionsCount - 1) ? (navIndex + 1) : 0);
	}

	public void RefreshUIElement()
	{
		optionsCount = customizer.ApparelTables.Count;
		setOption(0);
	}
}
