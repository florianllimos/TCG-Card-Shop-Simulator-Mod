public class CEventPlayer_SetCoin : CEvent
{
	public float m_CoinValue { get; private set; }

	public double m_CoinValueDouble { get; private set; }

	public CEventPlayer_SetCoin(float coinValue, double coinValueDouble)
	{
		m_CoinValue = coinValue;
		m_CoinValueDouble = coinValueDouble;
	}
}
