public class CEventPlayer_ParticleSpawnerReachedDestination : CEvent
{
	public ECurrencyType m_CurrencyType { get; private set; }

	public CEventPlayer_ParticleSpawnerReachedDestination(ECurrencyType CurrencyType)
	{
		m_CurrencyType = CurrencyType;
	}
}
