public class CEventPlayer_AddShopExp : CEvent
{
	public int m_ExpValue { get; private set; }

	public bool m_NoLerp { get; private set; }

	public CEventPlayer_AddShopExp(int ExpValue, bool noLerp = false)
	{
		m_ExpValue = ExpValue;
		m_NoLerp = noLerp;
	}
}
