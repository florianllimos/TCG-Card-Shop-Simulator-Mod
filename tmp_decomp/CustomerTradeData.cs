using System;

[Serializable]
public class CustomerTradeData
{
	public bool m_IsTrading;

	public float m_PriceSet;

	public float m_LastPriceSet;

	public float m_SellCardAskPrice;

	public float m_SellCardMarketPrice;

	public int m_MaxDeclineCount;

	public int m_DeclineCount;

	public CardData m_CardData_L;

	public CardData m_CardData_R;
}
