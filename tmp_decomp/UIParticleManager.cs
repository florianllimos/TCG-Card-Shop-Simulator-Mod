using System.Collections.Generic;
using UnityEngine;

public class UIParticleManager : CSingleton<UIParticleManager>
{
	public GameObject m_Camera;

	public List<ParticleSystem> m_ParticleList;

	public List<float> m_DeactivateTimeList;

	private bool m_HasActiveParticle;

	private float m_Timer;

	private float m_DeactivateTime = 3f;

	private bool m_IsLerp;

	private float m_LerpTimer;

	private float m_LerpSpeed = 1f;

	private Vector3 m_StartPos;

	private Vector3 m_TargetPos;

	private Transform m_CurrentParticleTransform;

	public void Awake()
	{
		m_Camera.SetActive(value: false);
	}

	public void Update()
	{
		if (m_HasActiveParticle)
		{
			if (m_IsLerp)
			{
				m_LerpTimer += Time.deltaTime * m_LerpSpeed;
				m_CurrentParticleTransform.position = Vector3.Lerp(m_StartPos, m_TargetPos, m_LerpTimer);
			}
			m_Timer += Time.deltaTime;
			if (m_Timer >= m_DeactivateTime)
			{
				m_CurrentParticleTransform.position = m_StartPos;
				m_IsLerp = true;
				m_HasActiveParticle = false;
				m_Camera.SetActive(value: false);
			}
		}
	}

	public static void SpawnParticle(EParticleType particleType)
	{
		CSingleton<UIParticleManager>.Instance.m_Camera.SetActive(value: true);
		CSingleton<UIParticleManager>.Instance.m_HasActiveParticle = true;
		CSingleton<UIParticleManager>.Instance.m_Timer = 0f;
		CSingleton<UIParticleManager>.Instance.m_DeactivateTime = CSingleton<UIParticleManager>.Instance.m_DeactivateTimeList[(int)particleType];
		CSingleton<UIParticleManager>.Instance.m_ParticleList[(int)particleType].gameObject.SetActive(value: true);
		CSingleton<UIParticleManager>.Instance.m_ParticleList[(int)particleType].Play();
	}

	public static void SpawnParticleMove(EParticleType particleType, Vector3 movePos, float time)
	{
		CSingleton<UIParticleManager>.Instance.m_LerpTimer = 0f;
		if ((bool)CSingleton<UIParticleManager>.Instance.m_CurrentParticleTransform)
		{
			CSingleton<UIParticleManager>.Instance.m_CurrentParticleTransform.position = CSingleton<UIParticleManager>.Instance.m_StartPos;
		}
		SpawnParticle(particleType);
		CSingleton<UIParticleManager>.Instance.m_CurrentParticleTransform = CSingleton<UIParticleManager>.Instance.m_ParticleList[(int)particleType].transform;
		CSingleton<UIParticleManager>.Instance.m_StartPos = CSingleton<UIParticleManager>.Instance.m_CurrentParticleTransform.position;
		CSingleton<UIParticleManager>.Instance.m_TargetPos = CSingleton<UIParticleManager>.Instance.m_StartPos + movePos;
		if (time <= 0f)
		{
			time = 0.01f;
		}
		CSingleton<UIParticleManager>.Instance.m_LerpSpeed = 1f / time;
		CSingleton<UIParticleManager>.Instance.m_IsLerp = true;
	}
}
