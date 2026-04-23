using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class DecoPurchaseData
{
	public string name;

	public string mainNameText;

	public string replaceNameXXXText;

	public string replaceNameYYYText;

	public int levelRequirement;

	public float price;

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
