using System;
using System.Collections.Generic;

[Serializable]
public class CustomerSaveData
{
	public ECustomerState currentState;

	public bool hasPlayedGame;

	public bool hasCheckedOut;

	public bool hasTookItemFromShelf;

	public bool hasTookCardFromShelf;

	public bool isInsideShop;

	public bool isSmelly;

	public bool hasUpdatedCustomerCount;

	public int smellyMeter;

	public float currentCostTotal;

	public float maxMoney;

	public float totalScannedItemCost;

	public float currentPlayTableFee;

	public Vector3Serializer pos;

	public QuaternionSerializer rot;

	public List<EItemType> itemInBagList;

	public List<float> itemInBagPriceList;

	public List<CardData> cardInBagList;

	public List<float> cardInBagPriceList;

	public List<CustomerReviewGatherData> customerReviewGatherDataList;
}
