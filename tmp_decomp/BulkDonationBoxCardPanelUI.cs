using TMPro;
using UnityEngine;

public class BulkDonationBoxCardPanelUI : UIElementBase
{
	public GameObject m_EmptyAddNewCardSlot;

	public CardUI m_CardUI;

	public GameObject m_CardCountGrp;

	public TextMeshProUGUI m_CountText;

	private BulkDonationBoxUIScreen m_BulkDonationBoxUIScreen;

	private int m_Index;

	private bool m_IsEmptyAddNewCardSlot;

	public void Init(BulkDonationBoxUIScreen bulkDonationBoxUIScreen, int index)
	{
		m_BulkDonationBoxUIScreen = bulkDonationBoxUIScreen;
		m_Index = index;
	}

	public void SetEmptyCardSlot()
	{
		m_IsEmptyAddNewCardSlot = true;
		m_EmptyAddNewCardSlot.SetActive(value: true);
		m_CardUI.gameObject.SetActive(value: false);
		m_CountText.text = "";
		m_CardCountGrp.SetActive(value: false);
	}

	public void SetCardUI(CardData cardData, int amount)
	{
		m_IsEmptyAddNewCardSlot = false;
		m_EmptyAddNewCardSlot.SetActive(value: false);
		m_CardUI.SetCardUI(cardData);
		if (cardData.cardGrade > 0)
		{
			m_CountText.enabled = false;
		}
		else
		{
			m_CountText.text = "X " + amount;
			m_CountText.enabled = true;
		}
		m_CardCountGrp.SetActive(value: true);
		m_CardUI.gameObject.SetActive(value: true);
	}

	public override void OnPressButton()
	{
		if (CSingleton<InputManager>.Instance.m_IsControllerActive || CSingleton<TouchManager>.Instance.m_AllowButtonPress)
		{
			SoundManager.GenericConfirm();
			if (m_IsEmptyAddNewCardSlot)
			{
				m_BulkDonationBoxUIScreen.OnPressEmptySlot(m_Index);
			}
			else
			{
				m_BulkDonationBoxUIScreen.OnPressOpenEditCard(m_CardUI.GetCardData(), m_Index);
			}
		}
	}

	public void OnHoverEnter()
	{
	}

	public void OnHoverExit()
	{
	}
}
