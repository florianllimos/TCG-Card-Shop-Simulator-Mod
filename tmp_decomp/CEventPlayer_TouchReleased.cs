using UnityEngine;

public class CEventPlayer_TouchReleased : CEvent
{
	public Vector3 m_ReleaseVector { get; private set; }

	public CEventPlayer_TouchReleased(Vector3 releaseVector)
	{
		m_ReleaseVector = releaseVector;
	}
}
