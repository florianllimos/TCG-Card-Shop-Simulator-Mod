using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class ShopDecoData
{
	public string name;

	public string mainNameText;

	public string replaceNameXXXText;

	public string replaceNameYYYText;

	public float price;

	public bool showBar;

	public Texture mainTexture;

	public Texture roughnessMap;

	public Texture normalMap;

	public Color color;

	public float smoothness = 1f;

	public Sprite icon;

	public string GetName()
	{
		if (mainNameText != null && mainNameText != "")
		{
			return LocalizationManager.GetTranslation(mainNameText).Replace("XXX", LocalizationManager.GetTranslation(replaceNameXXXText)).Replace("YYY", replaceNameYYYText);
		}
		return LocalizationManager.GetTranslation(name);
	}
}
