public class CEventPlayer_CardPriceChanged : CEvent
{
	public CardData m_CardData { get; private set; }

	public float m_Price { get; private set; }

	public CEventPlayer_CardPriceChanged(CardData cardData, float price)
	{
		m_CardData = cardData;
		m_Price = price;
	}
}
