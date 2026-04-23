using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class WorkerData
{
	public string name;

	public Sprite icon;

	public float restockSpeed;

	public float checkoutSpeed;

	public float walkSpeedMultiplier;

	public float costPerDay;

	public float hiringCost;

	public float arriveEarlySpeedMin;

	public float arriveEarlySpeedMax;

	public int shopLevelRequired;

	public string description;

	public string bonusConversation;

	public bool goBackOnTime;

	public bool prologueShow;

	public string GetName()
	{
		return name;
	}

	public string GetDescription()
	{
		return LocalizationManager.GetTranslation(description);
	}

	public string GetBonusConversation()
	{
		return LocalizationManager.GetTranslation(bonusConversation);
	}

	public string GetRestockSpeedText()
	{
		if (restockSpeed < 1f)
		{
			return LocalizationManager.GetTranslation("Very Fast");
		}
		if (restockSpeed < 1.75f)
		{
			return LocalizationManager.GetTranslation("Fast");
		}
		if (restockSpeed < 2.5f)
		{
			return LocalizationManager.GetTranslation("Normal");
		}
		if (restockSpeed < 3f)
		{
			return LocalizationManager.GetTranslation("Slow");
		}
		return LocalizationManager.GetTranslation("Very Slow");
	}

	public string GetCheckoutSpeedText()
	{
		if (checkoutSpeed < 1f)
		{
			return LocalizationManager.GetTranslation("Very Fast");
		}
		if (checkoutSpeed < 1.75f)
		{
			return LocalizationManager.GetTranslation("Fast");
		}
		if (checkoutSpeed < 2.5f)
		{
			return LocalizationManager.GetTranslation("Normal");
		}
		if (checkoutSpeed < 3f)
		{
			return LocalizationManager.GetTranslation("Slow");
		}
		return LocalizationManager.GetTranslation("Very Slow");
	}

	public string GetSalaryCostText()
	{
		return GameInstance.GetPriceString(costPerDay) + "/" + LocalizationManager.GetTranslation("day");
	}
}
