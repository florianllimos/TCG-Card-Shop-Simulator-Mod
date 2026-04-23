public class CCheatEvent : CEvent
{
	public enum ECheatType
	{
		IncreaseShopLevel,
		IncreaseMoney
	}

	public ECheatType m_cheatType;

	public CCheatEvent(ECheatType type)
	{
		m_cheatType = type;
	}
}
