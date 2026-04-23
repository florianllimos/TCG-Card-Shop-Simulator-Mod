public class CEventPlayer_OnSaveStatusUpdated : CEvent
{
	public bool m_IsSuccess { get; private set; }

	public bool m_IsAutosaving { get; private set; }

	public CEventPlayer_OnSaveStatusUpdated(bool isSuccess, bool isAutosaving)
	{
		m_IsSuccess = isSuccess;
		m_IsAutosaving = isAutosaving;
	}
}
