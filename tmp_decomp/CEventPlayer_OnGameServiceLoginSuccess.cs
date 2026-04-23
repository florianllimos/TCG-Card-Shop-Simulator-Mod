public class CEventPlayer_OnGameServiceLoginSuccess : CEvent
{
	public bool m_LoginSuccess { get; private set; }

	public CEventPlayer_OnGameServiceLoginSuccess(bool loginSuccess)
	{
		m_LoginSuccess = loginSuccess;
	}
}
