using System;

[Serializable]
public struct GameReportDataCollect
{
	public int customerVisited;

	public int checkoutCount;

	public int customerDisatisfied;

	public int customerBoughtItem;

	public int customerBoughtCard;

	public int customerPlayed;

	public int storeExpGained;

	public int storeLevelGained;

	public int itemAmountSold;

	public int cardAmountSold;

	public float totalPlayTableTime;

	public float totalItemEarning;

	public float totalCardEarning;

	public float totalPlayTableEarning;

	public float supplyCost;

	public float upgradeCost;

	public float employeeCost;

	public float rentCost;

	public float billCost;

	public int cardPackOpened;

	public int smellyCustomerCleaned;

	public int manualCheckoutCount;

	public int gemMintCardObtained;

	public void CopyData(GameReportDataCollect data)
	{
		customerVisited = data.customerVisited;
		checkoutCount = data.checkoutCount;
		customerDisatisfied = data.customerDisatisfied;
		customerBoughtItem = data.customerBoughtItem;
		customerBoughtCard = data.customerBoughtCard;
		customerPlayed = data.customerPlayed;
		storeExpGained = data.storeExpGained;
		storeLevelGained = data.storeLevelGained;
		itemAmountSold = data.itemAmountSold;
		cardAmountSold = data.cardAmountSold;
		totalPlayTableTime = data.totalPlayTableTime;
		totalItemEarning = data.totalItemEarning;
		totalCardEarning = data.totalCardEarning;
		totalPlayTableEarning = data.totalPlayTableEarning;
		supplyCost = data.supplyCost;
		upgradeCost = data.upgradeCost;
		employeeCost = data.employeeCost;
		rentCost = data.rentCost;
		billCost = data.billCost;
		cardPackOpened = data.cardPackOpened;
		smellyCustomerCleaned = data.smellyCustomerCleaned;
		manualCheckoutCount = data.manualCheckoutCount;
	}

	public void ResetData()
	{
		customerVisited = 0;
		checkoutCount = 0;
		customerDisatisfied = 0;
		customerBoughtItem = 0;
		customerBoughtCard = 0;
		customerPlayed = 0;
		storeExpGained = 0;
		storeLevelGained = 0;
		itemAmountSold = 0;
		cardAmountSold = 0;
		totalPlayTableTime = 0f;
		totalItemEarning = 0f;
		totalCardEarning = 0f;
		totalPlayTableEarning = 0f;
		supplyCost = 0f;
		upgradeCost = 0f;
		employeeCost = 0f;
		rentCost = 0f;
		billCost = 0f;
		cardPackOpened = 0;
		smellyCustomerCleaned = 0;
		manualCheckoutCount = 0;
	}
}
