using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CC;

public class Grid_Icon_Generator : MonoBehaviour, ICustomizerUI
{
	public enum Type
	{
		Apparel,
		Hair
	}

	public Type CustomizationType;

	public int Slot;

	public int MinIcons = 9;

	public GameObject Prefab;

	public List<string> Objects = new List<string>();

	private CharacterCustomization customizer;

	private List<Grid_Icon> gridIcons = new List<Grid_Icon>();

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util parentUI)
	{
		customizer = customizerScript;
		switch (CustomizationType)
		{
		case Type.Apparel:
		{
			List<scrObj_Apparel.Apparel> items = customizer.ApparelTables[Slot].Items;
			for (int num = 0; num < items.Count; num++)
			{
				string name2 = items[num].Name;
				int index2 = num;
				Grid_Icon component2 = Object.Instantiate(Prefab, base.transform).GetComponent<Grid_Icon>();
				component2.GetComponent<Button>().onClick.AddListener(delegate
				{
					customizer.setApparelByName(name2, Slot, 0);
				});
				component2.GetComponent<Button>().onClick.AddListener(delegate
				{
					updateSelectedState(index2);
				});
				component2.name = name2;
				gridIcons.Add(component2);
				component2.setIcon(items[num].Icon);
			}
			break;
		}
		case Type.Hair:
		{
			List<scrObj_Hair.Hairstyle> hairstyles = customizer.HairTables[Slot].Hairstyles;
			for (int i = 0; i < hairstyles.Count; i++)
			{
				string name = hairstyles[i].Name;
				int index = i;
				Grid_Icon component = Object.Instantiate(Prefab, base.transform).GetComponent<Grid_Icon>();
				component.GetComponent<Button>().onClick.AddListener(delegate
				{
					customizer.setHairByName(name, Slot);
				});
				component.GetComponent<Button>().onClick.AddListener(delegate
				{
					updateSelectedState(index);
				});
				component.name = name;
				gridIcons.Add(component);
				component.setIcon(hairstyles[i].Icon);
			}
			break;
		}
		}
		RefreshUIElement();
		while (base.transform.childCount < 9)
		{
			Object.Instantiate(Prefab, base.transform).GetComponent<Grid_Icon>().GetComponent<Button>()
				.interactable = false;
		}
	}

	public void RefreshUIElement()
	{
		string text = "";
		switch (CustomizationType)
		{
		case Type.Apparel:
			text = customizer.StoredCharacterData.ApparelNames[Slot];
			break;
		case Type.Hair:
			text = customizer.StoredCharacterData.HairNames[Slot];
			break;
		}
		foreach (Grid_Icon gridIcon in gridIcons)
		{
			gridIcon.setSelected(text == gridIcon.name);
		}
	}

	public void updateSelectedState(int index)
	{
		for (int i = 0; i < gridIcons.Count; i++)
		{
			gridIcons[i].setSelected(i == index);
		}
	}
}
