using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CC;

public class Option_MultiPicker : MonoBehaviour, ICustomizerUI
{
	public enum Type
	{
		Texture
	}

	private CharacterCustomization customizer;

	private CC_UI_Util parentUI;

	public List<MultiPicker> Properties = new List<MultiPicker>();

	public Type CustomizationType;

	public int Slot;

	public TextMeshProUGUI OptionText;

	private int navIndex;

	private int optionsCount;

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
	{
		customizer = customizerScript;
		parentUI = ParentUI;
		RefreshUIElement();
	}

	public void RefreshUIElement()
	{
		if (CustomizationType == Type.Texture)
		{
			optionsCount = Properties[0].Objects.Count;
			getSavedOption(customizer.StoredCharacterData.TextureProperties);
		}
	}

	public void getSavedOption(List<CC_Property> savedProps)
	{
		CC_Property property0 = Properties[0].Property;
		int savedIndex = savedProps.FindIndex((CC_Property t) => t.propertyName == property0.propertyName && t.materialIndex == property0.materialIndex && t.meshTag == property0.meshTag);
		if (savedIndex != -1)
		{
			navIndex = Properties[0].Objects.FindIndex((string t) => t == savedProps[savedIndex].stringValue);
		}
		else
		{
			navIndex = 0;
		}
		updateOptionText();
	}

	public void updateOptionText()
	{
		OptionText.SetText(navIndex + 1 + "/" + optionsCount);
	}

	public void setOption(int j)
	{
		navIndex = j;
		updateOptionText();
		if (CustomizationType == Type.Texture)
		{
			for (int i = 0; i < Properties.Count; i++)
			{
				Properties[i].Property.stringValue = Properties[i].Objects[j];
				customizer.setTextureProperty(Properties[i].Property, save: true);
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

	public void randomize()
	{
		setOption(Random.Range(0, optionsCount));
	}
}
