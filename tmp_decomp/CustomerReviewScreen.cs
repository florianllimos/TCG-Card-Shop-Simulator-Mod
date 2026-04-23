using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CustomerReviewScreen : GenericSliderScreen
{
	public List<CustomerReviewPanelUI> m_CustomerReviewPanelUIList;

	public Image m_StarFillImage;

	public TextMeshProUGUI m_ShopNameText;

	public TextMeshProUGUI m_AverageRatingText;

	protected override void Init()
	{
		base.Init();
		m_ShopNameText.text = CPlayerData.PlayerName;
		m_AverageRatingText.text = CustomerReviewManager.GetAverageRating().ToString("f2");
		m_StarFillImage.fillAmount = CustomerReviewManager.GetAverageRating() / 5f;
		for (int i = 0; i < m_CustomerReviewPanelUIList.Count; i++)
		{
			m_CustomerReviewPanelUIList[i].SetActive(isActive: false);
		}
		for (int j = 0; j < CPlayerData.m_CustomerReviewDataList.Count && j < m_CustomerReviewPanelUIList.Count; j++)
		{
			m_CustomerReviewPanelUIList[j].Init(CPlayerData.m_CustomerReviewDataList[CPlayerData.m_CustomerReviewDataList.Count - j - 1]);
			m_CustomerReviewPanelUIList[j].SetActive(isActive: true);
			m_ScrollEndPosParent = m_CustomerReviewPanelUIList[j].gameObject;
		}
	}

	protected override void OnOpenScreen()
	{
		Init();
		base.OnOpenScreen();
	}
}
