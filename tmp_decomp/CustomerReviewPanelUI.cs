using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine.UI;

public class CustomerReviewPanelUI : UIElementBase
{
	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_ReviewText;

	public TextMeshProUGUI m_DayText;

	public TextMeshProUGUI m_TimeText;

	public List<Image> m_StarImageList;

	public void Init(CustomerReviewData reviewData)
	{
		m_DayText.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (reviewData.day + 1).ToString());
		string text = "AM";
		if (reviewData.hour >= 12)
		{
			text = "PM";
		}
		string text2 = reviewData.minute.ToString();
		if (reviewData.minute < 10)
		{
			text2 = "0" + reviewData.minute;
		}
		int num = reviewData.hour;
		if (reviewData.hour > 12)
		{
			num = reviewData.hour - 12;
		}
		string text3 = num.ToString();
		if (num < 10)
		{
			text3 = "0" + num;
		}
		m_NameText.text = reviewData.customerName;
		m_TimeText.text = text3 + ":" + text2 + text;
		m_ReviewText.text = CSingleton<CustomerReviewManager>.Instance.GetReviewTextString(reviewData);
		for (int i = 0; i < m_StarImageList.Count; i++)
		{
			m_StarImageList[i].enabled = false;
		}
		for (int j = 0; j < reviewData.starLevel; j++)
		{
			m_StarImageList[j].enabled = true;
		}
	}
}
