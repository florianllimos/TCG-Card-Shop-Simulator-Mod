using System;
using I2.Loc;

[Serializable]
public class GameEventData
{
	public string name;

	public EGameEventFormat format;

	public int unlockPlayCountRequired;

	public int hostEventCost;

	public float baseFee;

	public float marketPriceMinPercent;

	public float marketPriceMaxPercent;

	public EPriceChangeType positivePriceChangeType;

	public EPriceChangeType negativePriceChangeType;

	public string GetName()
	{
		return LocalizationManager.GetTranslation(name);
	}
}
