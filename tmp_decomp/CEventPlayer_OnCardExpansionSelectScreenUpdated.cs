public class CEventPlayer_OnCardExpansionSelectScreenUpdated : CEvent
{
	public int m_CardExpansionTypeIndex { get; private set; }

	public CEventPlayer_OnCardExpansionSelectScreenUpdated(int cardExpansionTypeIndex)
	{
		m_CardExpansionTypeIndex = cardExpansionTypeIndex;
	}
}
