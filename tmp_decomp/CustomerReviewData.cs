using System;

[Serializable]
public class CustomerReviewData
{
	public ECustomerReviewType customerReviewType;

	public int starLevel;

	public int textSOGoodBadLevel;

	public int textSOIndex;

	public int day;

	public int hour;

	public int minute;

	public EItemType itemType;

	public string customerName;
}
