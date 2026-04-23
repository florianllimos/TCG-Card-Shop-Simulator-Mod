using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 2)]
public class PoolFX_ScriptableObject : ScriptableObject
{
	public GameObject m_SlashHitFXPrefab;

	public GameObject m_ExplosionFXPrefab;

	public GameObject m_FlameHitFXPrefab;

	public GameObject m_RocketHitFXPrefab;

	public GameObject m_SonicBlasterFXPrefab;

	public GameObject m_BasicHitFXPrefab;

	public GameObject m_RobotDestroyedExplosionFXPrefab;

	public GameObject m_ShockBurstFXPrefab;

	public GameObject m_ShockHitFXPrefab;

	public GameObject m_FreezeHitFXPrefab;

	public GameObject m_FreezeExplosionFXPrefab;

	public GameObject m_ChillMissleHitFXPrefab;

	public GameObject m_EnergyExplodeFXPrefab;
}
