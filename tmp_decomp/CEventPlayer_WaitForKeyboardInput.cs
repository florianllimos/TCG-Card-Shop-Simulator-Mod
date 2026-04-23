public class CEventPlayer_WaitForKeyboardInput : CEvent
{
	public string m_Text { get; private set; }

	public CEventPlayer_WaitForKeyboardInput(string Text)
	{
		m_Text = Text;
	}
}
