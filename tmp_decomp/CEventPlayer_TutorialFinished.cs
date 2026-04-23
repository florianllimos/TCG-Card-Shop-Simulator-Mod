public class CEventPlayer_TutorialFinished : CEvent
{
	public bool m_IsFinished { get; private set; }

	public CEventPlayer_TutorialFinished(bool isFinished)
	{
		m_IsFinished = isFinished;
	}
}
