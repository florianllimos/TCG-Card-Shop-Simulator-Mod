using UnityEngine;
using UnityEngine.UI;

namespace CC;

public class Option_Color_Picker : MonoBehaviour, ICustomizerUI
{
	private CharacterCustomization customizer;

	private CC_UI_Util parentUI;

	public CC_Property Property;

	public CC_Property OpacityProperty;

	public bool Hair;

	public int HairSlot;

	public Image ColorPreview;

	public Slider HueSlider;

	public Slider SatSlider;

	public Slider ValSlider;

	public Slider OpacitySlider;

	private Color color;

	private float hue;

	private float sat = 0.5f;

	private float val = 0.5f;

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
	{
		customizer = customizerScript;
		parentUI = ParentUI;
		RefreshUIElement();
	}

	public void RefreshUIElement()
	{
		int num = customizer.StoredCharacterData.ColorProperties.FindIndex((CC_Property t) => t.propertyName == Property.propertyName && t.materialIndex == Property.materialIndex && t.meshTag == Property.meshTag);
		Color color;
		if (Hair)
		{
			if (ColorUtility.TryParseHtmlString("#" + customizer.StoredCharacterData.HairColor[(HairSlot != -1) ? HairSlot : 0].stringValue, out color))
			{
				this.color = color;
				ColorPreview.color = this.color;
				Color.RGBToHSV(this.color, out hue, out sat, out val);
				HueSlider.SetValueWithoutNotify(hue);
				SatSlider.SetValueWithoutNotify(sat);
				ValSlider.SetValueWithoutNotify(val);
			}
		}
		else if (num != -1 && ColorUtility.TryParseHtmlString("#" + customizer.StoredCharacterData.ColorProperties[num].stringValue, out color))
		{
			this.color = color;
			ColorPreview.color = this.color;
			Color.RGBToHSV(this.color, out hue, out sat, out val);
			HueSlider.SetValueWithoutNotify(hue);
			SatSlider.SetValueWithoutNotify(sat);
			ValSlider.SetValueWithoutNotify(val);
		}
		num = customizer.StoredCharacterData.FloatProperties.FindIndex((CC_Property t) => t.propertyName == OpacityProperty.propertyName && t.materialIndex == OpacityProperty.materialIndex && t.meshTag == OpacityProperty.meshTag);
		if (num != -1 && OpacitySlider != null)
		{
			OpacitySlider.SetValueWithoutNotify(customizer.StoredCharacterData.FloatProperties[num].floatValue);
		}
	}

	public void setColor()
	{
		color = Color.HSVToRGB(hue, sat, val);
		ColorPreview.color = color;
		if (Hair)
		{
			customizer.setHairColor(Property, color, HairSlot, save: true);
		}
		else
		{
			customizer.setColorProperty(Property, color, save: true);
		}
	}

	public void setHue(float value)
	{
		hue = value;
		setColor();
	}

	public void setSat(float value)
	{
		sat = value;
		setColor();
	}

	public void setVal(float value)
	{
		val = value;
		setColor();
	}

	public void setOpacity(float value)
	{
		OpacityProperty.floatValue = value;
		customizer.setFloatProperty(OpacityProperty, save: true);
	}

	public void randomize(bool RandomizeOpacity)
	{
		hue = Random.Range(0f, 1f);
		val = Random.Range(0f, 1f);
		sat = Random.Range(0f, 1f);
		setColor();
		HueSlider.value = hue;
		SatSlider.value = sat;
		ValSlider.value = val;
		if (RandomizeOpacity)
		{
			float num = Random.Range(0f, 1f);
			setOpacity(num);
			OpacitySlider.value = num;
		}
	}
}
