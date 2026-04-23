using UnityEngine;

namespace CC;

public class Option_Grid_Picker : MonoBehaviour, ICustomizerUI
{
	public enum Type
	{
		Blendshape,
		Color
	}

	private CharacterCustomization customizer;

	public string[] ShapesYNeg;

	public string[] ShapesYPos;

	public string[] ShapesXNeg;

	public string[] ShapesXPos;

	public bool save;

	public Texture2D colorTexture;

	public Type CustomizationType;

	public CC_Property ColorProperty;

	public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util parentUI)
	{
		customizer = customizerScript;
		if (CustomizationType != Type.Blendshape)
		{
			_ = 1;
		}
	}

	public void RefreshUIElement()
	{
	}

	public void updateValue(Vector2 coords)
	{
		switch (CustomizationType)
		{
		case Type.Blendshape:
		{
			Vector2 vector = new Vector2(coords.x * 2f - 1f, coords.y * 2f - 1f);
			string[] shapesYNeg = ShapesYNeg;
			foreach (string text in shapesYNeg)
			{
				customizer.setBlendshapeByName(text, Mathf.Abs(Mathf.Clamp(vector.y, -1f, 0f)), save);
			}
			shapesYNeg = ShapesYPos;
			foreach (string text2 in shapesYNeg)
			{
				customizer.setBlendshapeByName(text2, Mathf.Clamp(vector.y, 0f, 1f), save);
			}
			shapesYNeg = ShapesXPos;
			foreach (string text3 in shapesYNeg)
			{
				customizer.setBlendshapeByName(text3, Mathf.Clamp(vector.x, 0f, 1f), save);
			}
			shapesYNeg = ShapesXNeg;
			foreach (string text4 in shapesYNeg)
			{
				customizer.setBlendshapeByName(text4, Mathf.Abs(Mathf.Clamp(vector.x, -1f, 0f)), save);
			}
			break;
		}
		case Type.Color:
		{
			int x = Mathf.RoundToInt((float)colorTexture.width * coords.x);
			int y = Mathf.RoundToInt((float)colorTexture.height * coords.y);
			Color pixel = colorTexture.GetPixel(x, y);
			customizer.setColorProperty(ColorProperty, pixel, save: true);
			break;
		}
		}
	}
}
