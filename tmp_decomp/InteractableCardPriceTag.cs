using UnityEngine;

public class InteractableCardPriceTag : InteractableObject
{
	public UI_PriceTag m_PriceTagUI;

	private bool m_IsPriceSet;

	private InteractableCardCompartment m_CardCompartment;

	public void Init(InteractableCardCompartment cardCompartment)
	{
		m_CardCompartment = cardCompartment;
	}

	public void SetPriceTagUI(UI_PriceTag PriceTagUI)
	{
		m_PriceTagUI = PriceTagUI;
		if ((bool)m_PriceTagUI)
		{
			m_PriceTagUI.InitCard(this);
		}
	}

	public UI_PriceTag GetPriceTagUI()
	{
		return m_PriceTagUI;
	}

	public void SetCardData(CardData cardData)
	{
		m_PriceTagUI.SetCardData(cardData);
	}

	public override void OnMouseButtonUp()
	{
		m_CardCompartment.OnStartSetPrice();
		CSingleton<SetItemPriceScreen>.Instance.OpenSetCardPriceScreen(m_PriceTagUI.GetCardData(), m_CardCompartment);
	}

	public void SetVisibility(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
		m_PriceTagUI.gameObject.SetActive(isVisible);
	}

	public void SetPriceText(float price)
	{
		m_PriceTagUI.SetPriceText(price);
	}

	public void RefreshPriceText()
	{
		m_PriceTagUI.RefreshPriceText();
	}

	public void SetPriceTagScale(float scale)
	{
		m_PriceTagUI.transform.localScale = Vector3.one * scale;
	}

	public void SetPriceChecked(bool isPriceSet)
	{
		m_IsPriceSet = isPriceSet;
	}

	public bool GetIsPriceSet()
	{
		return m_IsPriceSet;
	}

	public CardShelf GetCardShelf()
	{
		if ((bool)m_CardCompartment)
		{
			return m_CardCompartment.GetCardShelf();
		}
		return null;
	}
}
