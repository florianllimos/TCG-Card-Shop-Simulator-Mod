public class CEventPlayer_OnOpenCardPack : CEvent
{
	public int m_PackIndex { get; private set; }

	public int m_Amount { get; private set; }

	public CEventPlayer_OnOpenCardPack(int packIndex, int amount)
	{
		m_PackIndex = packIndex;
		m_Amount = amount;
	}
}
