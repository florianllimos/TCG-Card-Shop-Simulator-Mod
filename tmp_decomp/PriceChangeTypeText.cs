using System;
using I2.Loc;

[Serializable]
public class PriceChangeTypeText
{
	public string name;

	public EPriceChangeType priceChangeType;

	public string GetName(bool isIncrease)
	{
		if (isIncrease)
		{
			string translation = LocalizationManager.GetTranslation("increase");
			return LocalizationManager.GetTranslation(name).Replace("XXX", translation);
		}
		string translation2 = LocalizationManager.GetTranslation("decrease");
		return LocalizationManager.GetTranslation(name).Replace("XXX", translation2);
	}
}
