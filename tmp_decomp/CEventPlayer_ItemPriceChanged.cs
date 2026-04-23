public class CEventPlayer_ItemPriceChanged : CEvent
{
	public EItemType m_ItemType { get; private set; }

	public float m_Price { get; private set; }

	public CEventPlayer_ItemPriceChanged(EItemType itemType, float price)
	{
		m_ItemType = itemType;
		m_Price = price;
	}
}
