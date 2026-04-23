public class CEventPlayer_SetShopExp : CEvent
{
	public int m_ExpValue { get; private set; }

	public CEventPlayer_SetShopExp(int ExpValue)
	{
		m_ExpValue = ExpValue;
	}
}
