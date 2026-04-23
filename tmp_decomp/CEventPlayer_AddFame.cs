public class CEventPlayer_AddFame : CEvent
{
	public int m_FameValue { get; private set; }

	public bool m_NoLerp { get; private set; }

	public CEventPlayer_AddFame(int fameValue, bool noLerp = false)
	{
		m_FameValue = fameValue;
		m_NoLerp = noLerp;
	}
}
