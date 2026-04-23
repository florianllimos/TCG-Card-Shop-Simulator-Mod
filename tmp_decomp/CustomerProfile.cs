using System;
using System.Collections.Generic;

[Serializable]
public class CustomerProfile
{
	public string name;

	public ECustomerType type;

	public float appearRate;

	public int maxMoneyRangeMin;

	public int maxMoneyRangeMax;

	public int maxMoney;

	public int itemWantRandomPercentMin;

	public int itemWantRandomPercentMax;

	public int maxItemCountRangeMin;

	public int maxItemCountRangeMax;

	public List<EItemType> ignoreItemList;

	public float buyCardPercentChance;
}
