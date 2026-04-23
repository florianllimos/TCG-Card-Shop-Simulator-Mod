public class CEventPlayer_OnCardRaritySelectScreenUpdated : CEvent
{
	public int m_CardRarityIndex { get; private set; }

	public CEventPlayer_OnCardRaritySelectScreenUpdated(int cardRarityIndex)
	{
		m_CardRarityIndex = cardRarityIndex;
	}
}
