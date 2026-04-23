using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class CustomerReviewManager : CSingleton<CustomerReviewManager>
{
	public static CustomerReviewManager m_Instance;

	public Text_ScriptableObject m_TextSO;

	private int m_CustomerReviewCount;

	private float m_CustomerReviewScoreAverage;

	private float m_CustomerReviewScoreAverageLocalList;

	public List<CustomerReviewData> m_CustomerReviewDataList = new List<CustomerReviewData>();

	private void Update()
	{
		m_CustomerReviewDataList = CPlayerData.m_CustomerReviewDataList;
	}

	public static void CustomerSendReviewList(List<CustomerReviewGatherData> customerReviewGatherDataList)
	{
		if (customerReviewGatherDataList.Count > 0)
		{
			CustomerReviewGatherData customerReviewGatherData = customerReviewGatherDataList[Random.Range(0, customerReviewGatherDataList.Count)];
			AddCustomerReview(customerReviewGatherData.customerReviewType, customerReviewGatherData.itemType, customerReviewGatherData.goodBadLevel, customerReviewGatherData.higherStarChanceAdd);
		}
	}

	public static void AddCustomerReview(ECustomerReviewType reviewType, EItemType itemType, int goodBadLevel, int higherStarChanceAdd)
	{
		CustomerReviewData customerReviewData = new CustomerReviewData();
		customerReviewData.customerName = CSingleton<CustomerReviewManager>.Instance.m_TextSO.GetRandomName();
		customerReviewData.customerReviewType = reviewType;
		customerReviewData.day = CPlayerData.m_CurrentDay;
		customerReviewData.hour = LightManager.GetTimeHour();
		customerReviewData.minute = LightManager.GetTimeMinute();
		customerReviewData.itemType = itemType;
		customerReviewData.textSOGoodBadLevel = goodBadLevel;
		for (int i = 0; i < CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList.Count; i++)
		{
			if (CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].customerReviewType == reviewType)
			{
				if (goodBadLevel >= 2 && CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].good_TextList.Count == 0)
				{
					goodBadLevel = 1;
				}
				if (goodBadLevel == 1 && CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].normal_TextList.Count == 0)
				{
					goodBadLevel = 0;
				}
				break;
			}
		}
		customerReviewData.textSOGoodBadLevel = goodBadLevel;
		int num = 4;
		switch (goodBadLevel)
		{
		case 0:
			num = 1;
			break;
		case 1:
			num = 3;
			break;
		}
		if (Random.Range(0, 100) < 50 + higherStarChanceAdd)
		{
			num++;
		}
		customerReviewData.starLevel = num;
		int num2 = -1;
		for (int j = 0; j < CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList.Count; j++)
		{
			if (CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[j].customerReviewType == reviewType)
			{
				switch (goodBadLevel)
				{
				case 0:
					num2 = Random.Range(0, CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[j].bad_TextList.Count);
					break;
				case 1:
					num2 = Random.Range(0, CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[j].normal_TextList.Count);
					break;
				case 2:
					num2 = Random.Range(0, CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[j].good_TextList.Count);
					break;
				}
				if (num2 >= 0 && CSingleton<CustomerReviewManager>.Instance.HasSameReviewInList(reviewType, goodBadLevel, num2))
				{
					num2 = -1;
				}
				break;
			}
		}
		if (num2 < 0)
		{
			for (int k = 0; k < CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList.Count; k++)
			{
				if (CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[k].customerReviewType == ECustomerReviewType.StoreGeneric)
				{
					switch (goodBadLevel)
					{
					case 0:
						customerReviewData.customerReviewType = ECustomerReviewType.StoreGeneric;
						num2 = Random.Range(0, CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[k].bad_TextList.Count);
						break;
					case 1:
						customerReviewData.customerReviewType = ECustomerReviewType.StoreGeneric;
						num2 = Random.Range(0, CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[k].normal_TextList.Count);
						break;
					case 2:
						customerReviewData.customerReviewType = ECustomerReviewType.StoreGeneric;
						num2 = Random.Range(0, CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[k].good_TextList.Count);
						break;
					}
					if (num2 >= 0 && CSingleton<CustomerReviewManager>.Instance.HasSameReviewInList(reviewType, goodBadLevel, num2))
					{
						num2 = -1;
					}
					break;
				}
			}
		}
		customerReviewData.textSOIndex = num2;
		if (num2 >= 0)
		{
			float customerReviewScoreAverage = CPlayerData.m_CustomerReviewScoreAverage;
			customerReviewScoreAverage = ((float)customerReviewData.starLevel + customerReviewScoreAverage * (float)CPlayerData.m_CustomerReviewCount) / (float)(CPlayerData.m_CustomerReviewCount + 1);
			CPlayerData.m_CustomerReviewCount++;
			CPlayerData.m_CustomerReviewScoreAverage = customerReviewScoreAverage;
			CPlayerData.m_CustomerReviewDataList.Add(customerReviewData);
			if (CPlayerData.m_CustomerReviewDataList.Count > 50)
			{
				CPlayerData.m_CustomerReviewDataList.RemoveAt(0);
			}
		}
	}

	public string GetReviewTextString(CustomerReviewData reviewData)
	{
		if (reviewData.textSOIndex < 0)
		{
			return "";
		}
		string text = "";
		for (int i = 0; i < CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList.Count; i++)
		{
			if (CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].customerReviewType == reviewData.customerReviewType)
			{
				if (reviewData.textSOGoodBadLevel == 2 && CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].good_TextList.Count == 0)
				{
					reviewData.textSOGoodBadLevel = 1;
				}
				if (reviewData.textSOGoodBadLevel == 1 && CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].normal_TextList.Count == 0)
				{
					reviewData.textSOGoodBadLevel = 0;
				}
				if (reviewData.textSOGoodBadLevel == 0)
				{
					text = CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].bad_TextList[reviewData.textSOIndex];
				}
				else if (reviewData.textSOGoodBadLevel == 1)
				{
					text = CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].normal_TextList[reviewData.textSOIndex];
				}
				else if (reviewData.textSOGoodBadLevel == 2)
				{
					text = CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList[i].good_TextList[reviewData.textSOIndex];
				}
				break;
			}
		}
		text = LocalizationManager.GetTranslation("CustomerReview/" + text);
		text = text.Replace("XXX", CPlayerData.PlayerName);
		return text.Replace("YYY", InventoryBase.GetItemData(reviewData.itemType).GetName());
	}

	public bool HasSameReviewInList(ECustomerReviewType reviewType, int goodBadLevel, int textSOIndex)
	{
		int num = CPlayerData.m_CustomerReviewDataList.Count - 10;
		if (num < 0)
		{
			num = 0;
		}
		for (int num2 = CPlayerData.m_CustomerReviewDataList.Count - 1; num2 >= num; num2--)
		{
			if (CPlayerData.m_CustomerReviewDataList[num2].customerReviewType == reviewType && CPlayerData.m_CustomerReviewDataList[num2].textSOGoodBadLevel == goodBadLevel && CPlayerData.m_CustomerReviewDataList[num2].textSOIndex == textSOIndex)
			{
				return true;
			}
		}
		return false;
	}

	public static float GetAverageRating()
	{
		if (CPlayerData.m_CustomerReviewDataList.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 0; i < CPlayerData.m_CustomerReviewDataList.Count; i++)
		{
			num += (float)CPlayerData.m_CustomerReviewDataList[i].starLevel;
		}
		CSingleton<CustomerReviewManager>.Instance.m_CustomerReviewScoreAverageLocalList = num / (float)CPlayerData.m_CustomerReviewDataList.Count;
		return CSingleton<CustomerReviewManager>.Instance.m_CustomerReviewScoreAverageLocalList;
	}
}
