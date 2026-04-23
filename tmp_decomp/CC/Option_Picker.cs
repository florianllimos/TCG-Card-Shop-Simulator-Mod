using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CC;

public class Option_Picker : MonoBehaviour, ICustomizerUI
{
	public enum Type
	{
		Blendshape,
		Texture,
		Hair
	}

	private CharacterCustomization customizer;

	public Type CustomizationType;

	public CC_Property Property;

	public List<CC_Property> Options = new List<CC_Property>();

	public int Slot;

	public TextMeshProUGUI OptionText;

	private int navIndex;

	private int optionsCount;

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
	{
		customizer = customizerScript;
		RefreshUIElement();
	}

	public void RefreshUIElement()
	{
		switch (CustomizationType)
		{
		case Type.Blendshape:
		{
			optionsCount = Options.Count;
			updateOptionText();
			int i;
			for (i = 0; i < customizer.StoredCharacterData.Blendshapes.Count; i++)
			{
				int num = Options.FindIndex((CC_Property t) => t.propertyName == customizer.StoredCharacterData.Blendshapes[i].propertyName);
				if (num != -1)
				{
					navIndex = num;
					updateOptionText();
					break;
				}
			}
			break;
		}
		case Type.Texture:
		{
			optionsCount = Options.Count;
			int savedIndex = customizer.StoredCharacterData.TextureProperties.FindIndex((CC_Property t) => t.propertyName == Property.propertyName && t.materialIndex == Property.materialIndex && t.meshTag == Property.meshTag);
			if (savedIndex != -1)
			{
				navIndex = Options.FindIndex((CC_Property t) => t.stringValue == customizer.StoredCharacterData.TextureProperties[savedIndex].stringValue);
			}
			else
			{
				navIndex = 0;
			}
			updateOptionText();
			break;
		}
		case Type.Hair:
			optionsCount = customizer.HairTables[Slot].Hairstyles.Count;
			navIndex = customizer.HairTables[Slot].Hairstyles.FindIndex((scrObj_Hair.Hairstyle t) => t.Name == customizer.StoredCharacterData.HairNames[Slot]);
			if (navIndex == -1)
			{
				navIndex = 0;
			}
			updateOptionText();
			break;
		}
	}

	public void updateOptionText()
	{
		OptionText.SetText(navIndex + 1 + "/" + optionsCount);
	}

	public void setOption(int i)
	{
		navIndex = i;
		updateOptionText();
		switch (CustomizationType)
		{
		case Type.Blendshape:
			foreach (CC_Property option in Options)
			{
				_ = option;
				customizer.setBlendshapeByName(base.name, 0f);
			}
			customizer.setBlendshapeByName(Options[i].propertyName, 1f);
			break;
		case Type.Hair:
			customizer.setHair(i, Slot);
			break;
		case Type.Texture:
			customizer.setTextureProperty(Options[i], save: true);
			break;
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
