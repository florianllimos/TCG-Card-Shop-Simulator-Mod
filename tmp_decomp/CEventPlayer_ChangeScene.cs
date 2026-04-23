public class CEventPlayer_ChangeScene : CEvent
{
	public ELevelName m_SceneName { get; private set; }

	public CEventPlayer_ChangeScene(ELevelName SceneName)
	{
		m_SceneName = SceneName;
	}
}
