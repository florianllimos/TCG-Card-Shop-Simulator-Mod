public class CEventPlayer_SetGamePaused : CEvent
{
	public bool m_IsPaused { get; private set; }

	public CEventPlayer_SetGamePaused(bool isPaused)
	{
		m_IsPaused = isPaused;
	}
}
