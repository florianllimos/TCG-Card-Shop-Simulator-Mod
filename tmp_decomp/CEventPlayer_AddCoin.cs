public class CEventPlayer_AddCoin : CEvent
{
	public float m_CoinValue { get; private set; }

	public bool m_NoLerp { get; private set; }

	public CEventPlayer_AddCoin(float coinValue, bool noLerp = false)
	{
		m_CoinValue = coinValue;
		m_NoLerp = noLerp;
	}
}
