using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class FurniturePurchaseData
{
	public string name;

	public string description;

	public int levelRequirement;

	public float price;

	public EObjectType objectType;

	public Sprite icon;

	public string GetName()
	{
		return LocalizationManager.GetTranslation(name);
	}

	public string GetDescription()
	{
		return LocalizationManager.GetTranslation(description);
	}
}
