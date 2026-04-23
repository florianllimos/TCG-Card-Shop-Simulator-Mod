using TMPro;
using UnityEngine.UI;

public class GradeCardPanelUI : UIElementBase
{
	public bool m_ShowEmptySlotImage;

	public bool m_ShowPlusText;

	public CardUI m_CardUI;

	public Image m_EmptySlotImage;

	public TextMeshProUGUI m_ValueText;

	private GradedCardSubmitSelectScreen m_GradedCardSubmitSelectScreen;

	private int m_Index;

	public void Init(GradedCardSubmitSelectScreen gradedCardSubmitSelectScreen, int index)
	{
		m_GradedCardSubmitSelectScreen = gradedCardSubmitSelectScreen;
		m_Index = index;
	}

	public override void OnPressButton()
	{
		if ((bool)m_GradedCardSubmitSelectScreen)
		{
			m_GradedCardSubmitSelectScreen.OnPressSelectCardSlotIndex(m_Index);
		}
	}

	public void UpdateCardUI(CardData cardData)
	{
		if (cardData != null && cardData.monsterType != EMonsterType.None)
		{
			m_CardUI.SetCardUI(cardData);
			m_CardUI.gameObject.SetActive(value: true);
			m_ValueText.text = GameInstance.GetPriceString(CPlayerData.GetCardMarketPrice(cardData));
			m_EmptySlotImage.enabled = false;
		}
		else
		{
			m_CardUI.gameObject.SetActive(value: false);
			m_ValueText.text = "";
			m_EmptySlotImage.enabled = m_ShowEmptySlotImage;
		}
	}
}
