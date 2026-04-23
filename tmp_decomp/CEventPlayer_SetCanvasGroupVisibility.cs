public class CEventPlayer_SetCanvasGroupVisibility : CEvent
{
	public bool m_IsVisible { get; private set; }

	public CEventPlayer_SetCanvasGroupVisibility(bool isVisible)
	{
		m_IsVisible = isVisible;
	}
}
