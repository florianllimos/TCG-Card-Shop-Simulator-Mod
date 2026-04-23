public class CEventUI_GameVersionNotLatest : CEvent
{
	public float m_CurrentVersion { get; private set; }

	public float m_LatestVersion { get; private set; }

	public string m_LatestVersionString { get; private set; }

	public CEventUI_GameVersionNotLatest(float CurrentVersion, float LatestVersion, string LatestVersionString)
	{
		m_CurrentVersion = CurrentVersion;
		m_LatestVersion = LatestVersion;
		m_LatestVersionString = LatestVersionString;
	}
}
