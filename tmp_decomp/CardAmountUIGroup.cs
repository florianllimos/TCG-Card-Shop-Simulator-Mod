using TMPro;
using UnityEngine;

public class CardAmountUIGroup : MonoBehaviour
{
	public CardUI m_CardUI;

	public TextMeshProUGUI m_AmountText;

	public GameObject m_IsNewIcon;

	public GameObject m_IsHighValueIcon;

	private int m_Index;

	private ShowCardObtainedPage m_ShowCardObtainedPage;

	public void InitShowCardObtainedPage(ShowCardObtainedPage showCardObtainedPage, int index)
	{
		m_Index = index;
		m_ShowCardObtainedPage = showCardObtainedPage;
	}

	public void SetActive(bool isActive)
	{
		m_CardUI.gameObject.SetActive(isActive);
		m_AmountText.gameObject.SetActive(isActive);
	}

	public void OnPressCardUI()
	{
		m_ShowCardObtainedPage.OnPressCardUI(m_Index);
	}

	public void OnHoverEnter()
	{
		m_ShowCardObtainedPage.OnHoverEnterCardUI(m_Index);
	}

	public void OnHoverExit()
	{
		m_ShowCardObtainedPage.OnHoverExitCardUI();
	}
}
