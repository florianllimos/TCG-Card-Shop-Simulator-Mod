public class CEventPlayer_NotEnoughItem : CEvent
{
	public EItemType m_ItemType { get; private set; }

	public CEventPlayer_NotEnoughItem(EItemType itemType)
	{
		m_ItemType = itemType;
	}
}
