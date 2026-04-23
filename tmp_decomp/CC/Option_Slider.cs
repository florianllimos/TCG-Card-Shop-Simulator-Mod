using UnityEngine;
using UnityEngine.UI;

namespace CC;

public class Option_Slider : MonoBehaviour, ICustomizerUI
{
	public enum Type
	{
		Blendshape,
		Scalar
	}

	public Type CustomizationType;

	public CC_Property Property;

	public Slider slider;

	private float defaultValue;

	private CharacterCustomization customizer;

	private CC_UI_Util parentUI;

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
	{
		defaultValue = slider.value;
		customizer = customizerScript;
		parentUI = ParentUI;
		slider = GetComponentInChildren<Slider>();
		RefreshUIElement();
	}

	public void RefreshUIElement()
	{
		switch (CustomizationType)
		{
		case Type.Blendshape:
		{
			int num2 = customizer.StoredCharacterData.Blendshapes.FindIndex((CC_Property t) => t.propertyName == Property.propertyName);
			if (num2 != -1)
			{
				slider.SetValueWithoutNotify(customizer.StoredCharacterData.Blendshapes[num2].floatValue);
			}
			else
			{
				slider.SetValueWithoutNotify(defaultValue);
			}
			break;
		}
		case Type.Scalar:
		{
			int num = customizer.StoredCharacterData.FloatProperties.FindIndex((CC_Property t) => t.propertyName == Property.propertyName && t.materialIndex == Property.materialIndex && t.meshTag == Property.meshTag);
			if (num != -1)
			{
				slider.SetValueWithoutNotify(customizer.StoredCharacterData.FloatProperties[num].floatValue);
			}
			else
			{
				slider.SetValueWithoutNotify(defaultValue);
			}
			break;
		}
		}
	}

	public void setProperty(float value)
	{
		Property.floatValue = value;
		switch (CustomizationType)
		{
		case Type.Blendshape:
			customizer.setBlendshapeByName(Property.propertyName, value);
			break;
		case Type.Scalar:
			Debug.Log(value);
			customizer.setFloatProperty(Property, save: true);
			break;
		}
	}

	public void randomize()
	{
		slider.value = Random.Range(slider.minValue, slider.maxValue);
	}
}
