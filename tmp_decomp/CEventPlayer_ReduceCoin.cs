public class CEventPlayer_ReduceCoin : CEvent
{
	public float m_CoinValue { get; private set; }

	public bool m_NoLerp { get; private set; }

	public CEventPlayer_ReduceCoin(float coinValue, bool noLerp = false)
	{
		m_CoinValue = coinValue;
		m_NoLerp = noLerp;
	}
}
