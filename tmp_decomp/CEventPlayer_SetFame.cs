public class CEventPlayer_SetFame : CEvent
{
	public int m_FameValue { get; private set; }

	public CEventPlayer_SetFame(int fameValue)
	{
		m_FameValue = fameValue;
	}
}
