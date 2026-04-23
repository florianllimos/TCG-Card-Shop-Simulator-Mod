public class CEventPlayer_NotEnoughCurrency : CEvent
{
	public int m_ResourceIndex { get; private set; }

	public CEventPlayer_NotEnoughCurrency(int resourceIndex)
	{
		m_ResourceIndex = resourceIndex;
	}
}
