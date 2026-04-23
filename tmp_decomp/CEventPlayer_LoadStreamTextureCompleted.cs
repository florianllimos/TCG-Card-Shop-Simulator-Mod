public class CEventPlayer_LoadStreamTextureCompleted : CEvent
{
	public bool m_IsSuccess { get; private set; }

	public string m_FileName { get; private set; }

	public CEventPlayer_LoadStreamTextureCompleted(bool isSuccess, string fileName)
	{
		m_IsSuccess = isSuccess;
		m_FileName = fileName;
	}
}
