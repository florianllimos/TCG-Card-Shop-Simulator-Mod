using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CC;

public class Option_Proportional_Sliders : MonoBehaviour, ICustomizerUI
{
	private CharacterCustomization customizer;

	private CC_UI_Util parentUI;

	public List<string> Objects = new List<string>();

	public string MeshTag = "";

	public int MaterialIndex = -1;

	public float SliderHeight = 60f;

	public string TextPrefix = "";

	public bool RemoveText;

	private float sliderSum;

	public GameObject SliderObject;

	private List<Slider> sliders = new List<Slider>();

	private List<Option_Slider> sliderObjects = new List<Option_Slider>();

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
	{
		customizer = customizerScript;
		parentUI = ParentUI;
		for (int i = 0; i < Objects.Count; i++)
		{
			Option_Slider component = Object.Instantiate(SliderObject, base.transform).GetComponent<Option_Slider>();
			sliderObjects.Add(component);
			component.Property.propertyName = Objects[i];
			component.CustomizationType = Option_Slider.Type.Blendshape;
			component.InitializeUIElement(customizerScript, ParentUI);
			Slider slider = component.GetComponentInChildren<Slider>();
			sliders.Add(slider);
			slider.onValueChanged.AddListener(delegate
			{
				checkExcess(slider);
			});
			component.GetComponentInChildren<TextMeshProUGUI>().text = TextPrefix + (i + 1);
			component.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SliderHeight);
			if (RemoveText)
			{
				Object.Destroy(component.GetComponentInChildren<TextMeshProUGUI>().gameObject);
			}
		}
		base.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, base.gameObject.GetComponent<RectTransform>().rect.height + (SliderHeight + base.gameObject.GetComponent<VerticalLayoutGroup>().spacing) * (float)sliderObjects.Count);
	}

	public void RefreshUIElement()
	{
	}

	public void checkExcess(Slider mainSlider)
	{
		sliderSum = 0f;
		foreach (Slider slider in sliders)
		{
			sliderSum = slider.value + sliderSum;
		}
		if (!(sliderSum > 1f))
		{
			return;
		}
		for (int i = 0; i < sliders.Count; i++)
		{
			if (mainSlider != sliders[i])
			{
				distributeExcess(sliderSum - mainSlider.value, sliderSum - 1f, i);
			}
		}
	}

	public void distributeExcess(float sum, float excess, int index)
	{
		sliders[index].SetValueWithoutNotify(sliders[index].value - sliders[index].value / sum * excess);
		sliderObjects[index].setProperty(sliders[index].value);
	}
}
