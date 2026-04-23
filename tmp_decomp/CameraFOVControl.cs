using System.Collections.Generic;
using UnityEngine;

public class CameraFOVControl : MonoBehaviour
{
	public List<Camera> m_CamList;

	public Transform m_PosLocScalerGrp;

	public float m_DefaultFOV = 40f;

	private bool m_IsLerping;

	private float m_CurrentFOV = 40f;

	private float m_TargetLerpFOV = 40f;

	private float m_FOVMultiplier = 60f;

	private float m_LerpTimer;

	public float m_LerpSpeed = 5f;

	private float m_CurrentFOVSlider;

	private void Init()
	{
		m_CurrentFOVSlider = CSingleton<CGameManager>.Instance.m_CameraFOVSlider;
		m_CurrentFOV = m_DefaultFOV + m_CurrentFOVSlider * m_FOVMultiplier;
		UpdateFOV(m_CurrentFOV);
		UpdateHoldPositionLoc(m_CurrentFOV);
	}

	public void UpdateFOV(float fov)
	{
		for (int i = 0; i < m_CamList.Count; i++)
		{
			m_CamList[i].fieldOfView = fov;
		}
		UpdateHoldPositionLoc(fov);
	}

	private void UpdateHoldPositionLoc(float fov)
	{
		float num = (fov - 40f) / 10f * -0.2f;
		if (fov > 90f)
		{
			num = 0f - Mathf.Lerp(0.64f, 0.7f, 1f - (100f - fov) / 10f);
		}
		else if (fov > 80f)
		{
			num = 0f - Mathf.Lerp(0.58f, 0.64f, 1f - (90f - fov) / 10f);
		}
		else if (fov > 70f)
		{
			num = 0f - Mathf.Lerp(0.5f, 0.58f, 1f - (80f - fov) / 10f);
		}
		else if (fov > 60f)
		{
			num = 0f - Mathf.Lerp(0.4f, 0.5f, 1f - (70f - fov) / 10f);
		}
		Vector3 one = Vector3.one;
		one.z = 1f + num;
		m_PosLocScalerGrp.localScale = one;
	}

	public void StartLerpToFOV(float targetFOV)
	{
		m_IsLerping = true;
		m_TargetLerpFOV = targetFOV;
	}

	public void StopLerpFOV()
	{
		m_IsLerping = false;
	}

	private void Update()
	{
		if (m_CurrentFOVSlider != CSingleton<CGameManager>.Instance.m_CameraFOVSlider)
		{
			m_CurrentFOVSlider = CSingleton<CGameManager>.Instance.m_CameraFOVSlider;
			m_CurrentFOV = m_DefaultFOV + m_CurrentFOVSlider * m_FOVMultiplier;
			if (!m_IsLerping)
			{
				UpdateFOV(m_CurrentFOV);
				UpdateHoldPositionLoc(m_CurrentFOV);
			}
		}
		if (m_IsLerping && m_LerpTimer < 1f)
		{
			m_LerpTimer += Time.deltaTime * m_LerpSpeed;
			if (m_LerpTimer > 1f)
			{
				m_LerpTimer = 1f;
			}
			UpdateFOV(Mathf.Lerp(m_CurrentFOV, m_TargetLerpFOV, m_LerpTimer));
		}
		else if (!m_IsLerping && m_LerpTimer > 0f)
		{
			m_LerpTimer -= Time.deltaTime * m_LerpSpeed;
			if (m_LerpTimer < 0f)
			{
				m_LerpTimer = 0f;
			}
			UpdateFOV(Mathf.Lerp(m_CurrentFOV, m_TargetLerpFOV, m_LerpTimer));
		}
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
	}
}
