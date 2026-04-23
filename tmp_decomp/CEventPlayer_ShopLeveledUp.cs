public class CEventPlayer_ShopLeveledUp : CEvent
{
	public int m_ShopLevel { get; private set; }

	public CEventPlayer_ShopLeveledUp(int shopLevel)
	{
		m_ShopLevel = shopLevel;
	}
}
