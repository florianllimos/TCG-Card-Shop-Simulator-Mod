using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LightManager : CSingleton<LightManager>
{
	public bool m_LoadMaterialOriginalColor;

	public static LightManager m_Instance;

	public bool m_BlendoutSunShadow = true;

	public float m_TimerLerpSpeed = 1f;

	public float m_SkyboxRotateSpeed = 0.2f;

	public TextMeshProUGUI m_BillboardText;

	public GameObject m_SunlightGrp;

	public GameObject m_ShoplightGrp;

	public GameObject m_NightlightGrp;

	public ReflectionProbe m_ReflectionProbe;

	public PostProcessVolume m_PostProcessVolume;

	public SkyboxBlender m_SkyboxBlender;

	public Material m_CardBackMat;

	public Color m_CardBackMatOriginalEmissionColor;

	public List<Material> m_ItemMatList;

	public List<Color> m_ItemMatOriginalColorList;

	public List<float> m_TimeTillNextSkybox;

	public List<float> m_SkyboxBlendDuration;

	public Transform m_Sunlight;

	public Transform m_SunlightLerpStartPos;

	public Transform m_SunlightLerpEndPos;

	public List<Light> m_ItemLightList = new List<Light>();

	public List<Light> m_SunlightList = new List<Light>();

	public List<Light> m_AmbientLightList = new List<Light>();

	public List<float> m_TargetSunlightIntensityPercentList = new List<float>();

	public List<Light> m_SoftShadowLightList = new List<Light>();

	public string m_TimeString = "";

	public string m_TimeOfDayString = "";

	private bool m_FinishLoading;

	private bool m_IsLoadingSkyboxBlend;

	private bool m_IsLerpingSunIntensity;

	private bool m_IsBlendingSkybox;

	private bool m_IsStopBlendingSkybox;

	private bool m_HasDayEnded;

	private bool m_IsShopLightOn;

	private bool m_IsSunLightOn = true;

	private bool m_CanChangeBGM = true;

	private int m_TImeOfDayIndex;

	private int m_SkyboxIndex;

	private int m_TimeHour = 8;

	private int m_TimeMin;

	private int m_CurrentQualitySettingIndex;

	private float m_TimeMinFloat;

	private float m_Timer;

	private float m_SunlightLerpTimer;

	private float m_SunlightRotationLerpTimer;

	private float m_WorldUIEvaluateTimer;

	private float m_GlobalBrightness = 1f;

	private float m_LerpStartBrightness;

	private float m_ShopLightOnTimer;

	private Color m_BillboardTextOriginalColor;

	private Color m_BillboardTargetLerpColor;

	private List<float> m_OriginalSunlightGrpLightIntensityList = new List<float>();

	private List<float> m_OriginalShoplightGrpLightIntensityList = new List<float>();

	private List<float> m_OriginalItemLightIntensityList = new List<float>();

	private List<float> m_OriginalAmbientLightIntensityList = new List<float>();

	private LightTimeData m_LightTimeData;

	private void Awake()
	{
		for (int i = 0; i < m_SunlightList.Count; i++)
		{
			m_OriginalSunlightGrpLightIntensityList.Add(m_SunlightList[i].intensity);
		}
		for (int j = 0; j < m_ItemLightList.Count; j++)
		{
			m_OriginalItemLightIntensityList.Add(m_ItemLightList[j].intensity);
		}
		for (int k = 0; k < m_AmbientLightList.Count; k++)
		{
			m_OriginalAmbientLightIntensityList.Add(m_AmbientLightList[k].intensity);
		}
		if (m_LoadMaterialOriginalColor)
		{
			for (int l = 0; l < m_ItemMatList.Count; l++)
			{
				if ((bool)m_ItemMatList[l])
				{
					m_ItemMatOriginalColorList.Add(m_ItemMatList[l].GetColor("_Color"));
				}
				else
				{
					m_ItemMatOriginalColorList.Add(Color.white);
				}
			}
		}
		m_BillboardTextOriginalColor = m_BillboardText.color;
		m_LightTimeData = new LightTimeData();
	}

	private void Init()
	{
		if (m_FinishLoading)
		{
			return;
		}
		m_FinishLoading = true;
		if (CPlayerData.m_LightTimeData == null)
		{
			ResetSunlightIntensity();
			UpdateLightTimeData();
			CPlayerData.m_LightTimeData = m_LightTimeData;
			return;
		}
		m_LightTimeData = CPlayerData.m_LightTimeData;
		m_IsLerpingSunIntensity = m_LightTimeData.m_IsLerpingSunIntensity;
		m_IsBlendingSkybox = m_LightTimeData.m_IsBlendingSkybox;
		m_IsStopBlendingSkybox = m_LightTimeData.m_IsStopBlendingSkybox;
		m_HasDayEnded = m_LightTimeData.m_HasDayEnded;
		m_TImeOfDayIndex = m_LightTimeData.m_TImeOfDayIndex;
		m_SkyboxIndex = m_LightTimeData.m_SkyboxIndex;
		m_TimeHour = m_LightTimeData.m_TimeHour;
		m_TimeMin = m_LightTimeData.m_TimeMin;
		m_TimeMinFloat = m_LightTimeData.m_TimeMinFloat;
		m_Timer = m_LightTimeData.m_Timer;
		m_SunlightLerpTimer = m_LightTimeData.m_SunlightLerpTimer;
		m_SunlightRotationLerpTimer = m_LightTimeData.m_SunlightRotationLerpTimer;
		m_GlobalBrightness = m_LightTimeData.m_GlobalBrightness;
		m_LerpStartBrightness = m_LightTimeData.m_LerpStartBrightness;
		m_NightlightGrp.SetActive(m_LightTimeData.m_IsNightLightOn);
		m_ShoplightGrp.SetActive(m_LightTimeData.m_IsShopLightOn);
		m_SunlightGrp.SetActive(m_LightTimeData.m_IsSunlightOn);
		m_Sunlight.rotation = Quaternion.Lerp(m_SunlightLerpStartPos.rotation, m_SunlightLerpEndPos.rotation, m_SunlightRotationLerpTimer);
		m_ShopLightOnTimer = m_LightTimeData.m_ShopLightOnTimer;
		if (m_HasDayEnded)
		{
			CEventManager.QueueEvent(new CEventPlayer_OnDayEnded());
		}
		if (m_TImeOfDayIndex >= 2)
		{
			SoundManager.BlendToMusic("BGM_ShopNight", 1f, isLinearBlend: true);
		}
		else
		{
			SoundManager.BlendToMusic("BGM_ShopDay", 1f, isLinearBlend: true);
		}
		if (!m_IsLerpingSunIntensity)
		{
			for (int i = 0; i < m_AmbientLightList.Count; i++)
			{
				m_AmbientLightList[i].intensity = m_GlobalBrightness;
			}
			if (m_TImeOfDayIndex >= 3)
			{
				for (int j = 0; j < m_SunlightList.Count; j++)
				{
					float num = m_OriginalSunlightGrpLightIntensityList[j] * m_TargetSunlightIntensityPercentList[0];
					float b = num * m_TargetSunlightIntensityPercentList[1];
					m_SunlightList[j].intensity = Mathf.Lerp(num, b, 1f);
				}
				if (m_BlendoutSunShadow)
				{
					m_SunlightList[0].shadowStrength = 0f;
					m_SunlightList[0].shadows = LightShadows.None;
				}
			}
			else if (m_TImeOfDayIndex == 2)
			{
				for (int k = 0; k < m_SunlightList.Count; k++)
				{
					float num2 = m_OriginalSunlightGrpLightIntensityList[k];
					float b2 = num2 * m_TargetSunlightIntensityPercentList[0];
					m_SunlightList[k].intensity = Mathf.Lerp(num2, b2, 1f);
				}
				if (m_BlendoutSunShadow)
				{
					m_SunlightList[0].shadowStrength = 0f;
					m_SunlightList[0].shadows = LightShadows.None;
				}
			}
		}
		StartCoroutine(DelayUpdateSkyBox());
		EvaluateTimeClock();
		EvaluateWorldUIBrightness();
		CPlayerData.m_LightTimeData = m_LightTimeData;
	}

	private IEnumerator DelayUpdateSkyBox()
	{
		m_IsLoadingSkyboxBlend = true;
		if (m_IsBlendingSkybox)
		{
			m_SkyboxBlender.Blend(m_SkyboxIndex);
			m_SkyboxBlender.blendValue = 1f;
			yield return new WaitForSeconds(0.01f);
			m_SkyboxBlender.blendValue = m_LightTimeData.m_SkyboxBlendValue;
			m_SkyboxBlender.blendSpeed = m_LightTimeData.m_SkyboxBlendSpeed;
			m_SkyboxBlender.Blend(m_SkyboxIndex + 1);
		}
		else
		{
			m_SkyboxBlender.Blend(m_SkyboxIndex);
			m_SkyboxBlender.blendValue = 1f;
			yield return new WaitForSeconds(0.01f);
			m_SkyboxBlender.blendValue = m_LightTimeData.m_SkyboxBlendValue;
			m_SkyboxBlender.blendSpeed = m_LightTimeData.m_SkyboxBlendSpeed;
		}
		m_IsLoadingSkyboxBlend = false;
		DynamicGI.UpdateEnvironment();
	}

	private void UpdateLightTimeData()
	{
		m_LightTimeData.m_IsLerpingSunIntensity = m_IsLerpingSunIntensity;
		m_LightTimeData.m_IsBlendingSkybox = m_IsBlendingSkybox;
		m_LightTimeData.m_IsStopBlendingSkybox = m_IsStopBlendingSkybox;
		m_LightTimeData.m_HasDayEnded = m_HasDayEnded;
		m_LightTimeData.m_TImeOfDayIndex = m_TImeOfDayIndex;
		m_LightTimeData.m_SkyboxIndex = m_SkyboxIndex;
		m_LightTimeData.m_TimeHour = m_TimeHour;
		m_LightTimeData.m_TimeMin = m_TimeMin;
		m_LightTimeData.m_TimeMinFloat = m_TimeMinFloat;
		m_LightTimeData.m_Timer = m_Timer;
		m_LightTimeData.m_SunlightLerpTimer = m_SunlightLerpTimer;
		m_LightTimeData.m_SunlightRotationLerpTimer = m_SunlightRotationLerpTimer;
		m_LightTimeData.m_GlobalBrightness = m_GlobalBrightness;
		m_LightTimeData.m_LerpStartBrightness = m_LerpStartBrightness;
		m_LightTimeData.m_IsNightLightOn = m_NightlightGrp.activeSelf;
		m_LightTimeData.m_IsShopLightOn = m_ShoplightGrp.activeSelf;
		m_LightTimeData.m_IsSunlightOn = m_SunlightGrp.activeSelf;
		m_LightTimeData.m_ShopLightOnTimer = m_ShopLightOnTimer;
		if (!m_IsLoadingSkyboxBlend)
		{
			m_LightTimeData.m_SkyboxBlendValue = m_SkyboxBlender.blendValue;
			m_LightTimeData.m_SkyboxBlendSpeed = m_SkyboxBlender.blendSpeed;
		}
		CPlayerData.m_LightTimeData = m_LightTimeData;
	}

	private void Update()
	{
		if (!m_FinishLoading)
		{
			return;
		}
		UpdateLightTimeData();
		if (m_IsShopLightOn)
		{
			m_ShopLightOnTimer += Time.deltaTime;
		}
		if (!CPlayerData.m_IsShopOnceOpen || m_HasDayEnded)
		{
			return;
		}
		m_TimeMinFloat += Time.deltaTime * m_TimerLerpSpeed;
		m_TimeMin = Mathf.FloorToInt(m_TimeMinFloat);
		EvaluateTimeClock();
		EvaluateLerpSunIntensity();
		m_SunlightRotationLerpTimer += Time.deltaTime * 0.0013888889f * m_TimerLerpSpeed;
		m_Sunlight.rotation = Quaternion.Lerp(m_SunlightLerpStartPos.rotation, m_SunlightLerpEndPos.rotation, m_SunlightRotationLerpTimer);
		m_SkyboxBlender.rotationSpeed = m_SkyboxRotateSpeed * m_TimerLerpSpeed;
		if (!m_IsBlendingSkybox && !m_IsStopBlendingSkybox)
		{
			m_Timer += Time.deltaTime * m_TimerLerpSpeed;
			if (m_Timer > m_TimeTillNextSkybox[m_SkyboxIndex])
			{
				m_Timer = 0f;
				m_IsBlendingSkybox = true;
				m_SkyboxBlender.blendSpeed = 1f / m_SkyboxBlendDuration[m_SkyboxIndex] * m_TimerLerpSpeed;
				m_SkyboxBlender.Blend(m_SkyboxIndex + 1);
			}
		}
		else
		{
			if (!m_IsBlendingSkybox || m_IsStopBlendingSkybox)
			{
				return;
			}
			m_Timer += Time.deltaTime * m_TimerLerpSpeed;
			if (m_Timer >= m_SkyboxBlendDuration[m_SkyboxIndex])
			{
				m_Timer = 0f;
				m_SkyboxIndex++;
				m_IsBlendingSkybox = false;
				if (m_SkyboxIndex >= 2)
				{
					m_IsStopBlendingSkybox = true;
					m_SkyboxBlender.Stop();
				}
			}
		}
	}

	public static void GoNextDay()
	{
		CPlayerData.m_CurrentDay++;
		CSingleton<LightManager>.Instance.ResetSunlightIntensity();
	}

	private void ResetSunlightIntensity()
	{
		for (int i = 0; i < m_SunlightList.Count; i++)
		{
			m_SunlightList[i].intensity = m_OriginalSunlightGrpLightIntensityList[i];
		}
		for (int j = 0; j < m_ItemLightList.Count; j++)
		{
			m_ItemLightList[j].intensity = m_OriginalItemLightIntensityList[j];
		}
		for (int k = 0; k < m_AmbientLightList.Count; k++)
		{
			m_AmbientLightList[k].intensity = m_OriginalAmbientLightIntensityList[k];
		}
		m_SunlightList[0].shadowStrength = 1f;
		m_SunlightList[0].shadows = LightShadows.Soft;
		m_IsLerpingSunIntensity = false;
		m_IsBlendingSkybox = false;
		m_IsStopBlendingSkybox = false;
		m_SkyboxIndex = 0;
		m_Timer = 0f;
		m_SunlightLerpTimer = 0f;
		m_SunlightRotationLerpTimer = 0f;
		m_Sunlight.rotation = Quaternion.Lerp(m_SunlightLerpStartPos.rotation, m_SunlightLerpEndPos.rotation, 0f);
		m_SkyboxBlender.Blend(0);
		m_SkyboxBlender.blendValue = 1f;
		m_SunlightRotationLerpTimer = 0f;
		m_IsShopLightOn = false;
		m_IsSunLightOn = true;
		m_SunlightGrp.SetActive(value: true);
		m_ShoplightGrp.SetActive(value: false);
		m_NightlightGrp.SetActive(value: false);
		EvaluateWorldUIBrightness();
		StartCoroutine(DelayUpdateEnv());
	}

	private IEnumerator DelayUpdateEnv()
	{
		yield return new WaitForSeconds(0.01f);
		m_SkyboxBlender.Blend(0);
		m_SkyboxBlender.blendValue = 1f;
		yield return new WaitForSeconds(0.01f);
		DynamicGI.UpdateEnvironment();
		m_SkyboxBlender.blendValue = 0f;
		m_HasDayEnded = false;
		m_TimeHour = 8;
		m_TimeMin = 0;
		m_TimeMinFloat = 0f;
		EvaluateTimeClock();
		CEventManager.QueueEvent(new CEventPlayer_OnDayStarted());
		SoundManager.BlendToMusic("BGM_ShopDay", 1f, isLinearBlend: true);
	}

	private void EvaluateLerpSunIntensity()
	{
		if (!m_IsLerpingSunIntensity)
		{
			return;
		}
		if (m_TImeOfDayIndex >= 3)
		{
			m_SunlightLerpTimer += Time.deltaTime * (1f / m_SkyboxBlendDuration[1]) * m_TimerLerpSpeed;
			for (int i = 0; i < m_SunlightList.Count; i++)
			{
				float num = m_OriginalSunlightGrpLightIntensityList[i] * m_TargetSunlightIntensityPercentList[0];
				float b = num * m_TargetSunlightIntensityPercentList[1];
				m_SunlightList[i].intensity = Mathf.Lerp(num, b, m_SunlightLerpTimer);
			}
			m_WorldUIEvaluateTimer += Time.deltaTime;
			if (m_WorldUIEvaluateTimer > 0.1f)
			{
				EvaluateWorldUIBrightness();
				m_WorldUIEvaluateTimer = 0f;
			}
			if (m_SunlightLerpTimer >= 1f)
			{
				m_SunlightLerpTimer = 0f;
				m_SunlightGrp.SetActive(value: false);
				m_NightlightGrp.gameObject.SetActive(value: true);
				m_IsLerpingSunIntensity = false;
			}
		}
		else
		{
			if (m_TImeOfDayIndex != 2)
			{
				return;
			}
			m_SunlightLerpTimer += Time.deltaTime * (1f / m_SkyboxBlendDuration[0]) * m_TimerLerpSpeed;
			for (int j = 0; j < m_SunlightList.Count; j++)
			{
				float num2 = m_OriginalSunlightGrpLightIntensityList[j];
				float b2 = num2 * m_TargetSunlightIntensityPercentList[0];
				m_SunlightList[j].intensity = Mathf.Lerp(num2, b2, m_SunlightLerpTimer);
			}
			if (m_BlendoutSunShadow)
			{
				m_SunlightList[0].shadowStrength = Mathf.Lerp(1f, 0f, m_SunlightLerpTimer * 1.25f);
				if (m_SunlightList[0].shadowStrength <= 0f)
				{
					m_SunlightList[0].shadows = LightShadows.None;
				}
			}
			m_WorldUIEvaluateTimer += Time.deltaTime;
			if (m_WorldUIEvaluateTimer > 0.1f)
			{
				EvaluateWorldUIBrightness();
				m_WorldUIEvaluateTimer = 0f;
			}
			if (m_SunlightLerpTimer >= 1f)
			{
				m_SunlightLerpTimer = 0f;
				m_IsLerpingSunIntensity = false;
			}
		}
	}

	private void EvaluateTimeClock()
	{
		if (m_TimeMin >= 60)
		{
			m_TimeMinFloat = 0f;
			m_TimeMin = 0;
			m_TimeHour++;
			for (int i = 0; i < CPlayerData.m_GradeCardInProgressList.Count; i++)
			{
				CPlayerData.m_GradeCardInProgressList[i].m_MinutePassed += 60f;
			}
		}
		if (m_TimeHour >= 21 && m_TimeMin >= 0)
		{
			m_TimeHour = 21;
			m_TimeMin = 0;
		}
		string text = "AM";
		if (m_TimeHour >= 12)
		{
			text = "PM";
		}
		string text2 = m_TimeMin.ToString();
		if (m_TimeMin < 10)
		{
			text2 = "0" + m_TimeMin;
		}
		int num = m_TimeHour;
		if (m_TimeHour > 12)
		{
			num = m_TimeHour - 12;
		}
		string text3 = num.ToString();
		if (num < 10)
		{
			text3 = "0" + num;
		}
		m_TimeString = text3 + ":" + text2 + text;
		if (m_TimeHour == 18 && m_TimeMin == 30)
		{
			m_TimeOfDayString = "Night";
			m_TImeOfDayIndex = 3;
			m_IsLerpingSunIntensity = true;
			m_LerpStartBrightness = m_GlobalBrightness;
		}
		else if (m_TimeHour == 16 && m_TimeMin == 0)
		{
			m_TimeOfDayString = "Evening";
			m_TImeOfDayIndex = 2;
			m_IsLerpingSunIntensity = true;
			m_LerpStartBrightness = m_GlobalBrightness;
			if (m_CanChangeBGM)
			{
				SoundManager.BlendToMusic("BGM_ShopNight", 0.1f, isLinearBlend: true);
			}
		}
		else if (m_TimeHour == 12 && m_TimeMin == 0)
		{
			m_TimeOfDayString = "Afternoon";
			m_TImeOfDayIndex = 1;
			m_IsLerpingSunIntensity = false;
		}
		else if (m_TimeHour == 8 && m_TimeMin == 0)
		{
			m_TimeOfDayString = "Morning";
			m_TImeOfDayIndex = 0;
			m_IsLerpingSunIntensity = false;
			m_HasDayEnded = false;
		}
		else if (m_TimeHour == 21 && m_TimeMin == 0)
		{
			m_TimeOfDayString = "Day End";
			m_HasDayEnded = true;
			m_TImeOfDayIndex = 4;
			m_IsLerpingSunIntensity = false;
			m_SkyboxBlender.rotationSpeed = 0f;
			CEventManager.QueueEvent(new CEventPlayer_OnDayEnded());
		}
	}

	public float GetPercentTillDayEnd()
	{
		float num = 780f;
		return (float)((m_TimeHour - 8) * 60 + m_TimeMin) / num;
	}

	private void EvaluateWorldUIBrightness()
	{
		m_IsShopLightOn = m_ShoplightGrp.activeSelf;
		m_IsSunLightOn = m_SunlightGrp.activeSelf;
		if (m_IsSunLightOn)
		{
			if (m_TImeOfDayIndex == 3)
			{
				if (m_IsLerpingSunIntensity)
				{
					m_GlobalBrightness = Mathf.Lerp(m_LerpStartBrightness, 0.05f, m_SunlightLerpTimer);
				}
				else
				{
					m_GlobalBrightness = 0.05f;
				}
			}
			else if (m_TImeOfDayIndex == 2)
			{
				if (m_IsLerpingSunIntensity)
				{
					m_GlobalBrightness = Mathf.Lerp(m_LerpStartBrightness, 0.55f, m_SunlightLerpTimer);
				}
				else
				{
					m_GlobalBrightness = 0.55f;
				}
			}
			else
			{
				m_GlobalBrightness = 0.95f;
			}
		}
		else
		{
			m_GlobalBrightness = 0.05f;
		}
		if (m_IsShopLightOn)
		{
			m_GlobalBrightness += 0.9f;
		}
		m_GlobalBrightness = Mathf.Clamp(m_GlobalBrightness, 0f, 1f);
		for (int i = 0; i < m_ItemLightList.Count; i++)
		{
			m_ItemLightList[i].intensity = m_GlobalBrightness * m_OriginalItemLightIntensityList[i];
		}
		for (int j = 0; j < m_AmbientLightList.Count; j++)
		{
			m_AmbientLightList[j].intensity = m_GlobalBrightness * m_OriginalAmbientLightIntensityList[j];
		}
		PriceTagUISpawner.SetAllPriceTagUIBrightness(m_GlobalBrightness);
		Card3dUISpawner.SetAllCardUIBrightness(m_GlobalBrightness);
		m_BillboardTargetLerpColor = m_BillboardTextOriginalColor * m_GlobalBrightness;
		m_BillboardTargetLerpColor.a = 1f;
		m_BillboardText.color = m_BillboardTargetLerpColor;
		m_CardBackMat.SetColor("_EmissionColor", m_CardBackMatOriginalEmissionColor * Mathf.Lerp(0.2f, 1f, m_GlobalBrightness));
		for (int k = 0; k < m_ItemMatList.Count; k++)
		{
			if ((bool)m_ItemMatList[k])
			{
				m_ItemMatList[k].SetColor("_Color", m_ItemMatOriginalColorList[k] * Mathf.Lerp(0.2f, 1f, m_GlobalBrightness));
			}
		}
	}

	public void ToggleShopLight()
	{
		m_ShoplightGrp.SetActive(!m_ShoplightGrp.activeSelf);
		EvaluateWorldUIBrightness();
	}

	public static bool IsLightSufficientToSee()
	{
		if (CSingleton<LightManager>.Instance.m_TimeHour >= 18)
		{
			return CSingleton<LightManager>.Instance.m_IsShopLightOn;
		}
		return true;
	}

	public float GetBrightness()
	{
		return m_GlobalBrightness;
	}

	public static bool GetHasDayStarted()
	{
		return CSingleton<LightManager>.Instance.m_HasDayEnded;
	}

	public static bool GetHasDayEnded()
	{
		return CSingleton<LightManager>.Instance.m_HasDayEnded;
	}

	public override void OnApplicationQuit()
	{
		if (!m_LoadMaterialOriginalColor)
		{
			return;
		}
		for (int i = 0; i < m_ItemMatList.Count; i++)
		{
			if ((bool)m_ItemMatList[i])
			{
				m_ItemMatList[i].SetColor("_Color", m_ItemMatOriginalColorList[i]);
			}
		}
	}

	public static bool IsMorning()
	{
		return CSingleton<LightManager>.Instance.m_TImeOfDayIndex == 0;
	}

	public static bool IsAfternoon()
	{
		return CSingleton<LightManager>.Instance.m_TImeOfDayIndex == 1;
	}

	public static bool IsEvening()
	{
		return CSingleton<LightManager>.Instance.m_TImeOfDayIndex == 2;
	}

	public static bool IsNight()
	{
		return CSingleton<LightManager>.Instance.m_TImeOfDayIndex == 3;
	}

	public static float GetShopLightOnTime()
	{
		return CSingleton<LightManager>.Instance.m_ShopLightOnTimer;
	}

	public static void ResetShopLightOnTime()
	{
		CSingleton<LightManager>.Instance.m_ShopLightOnTimer = 0f;
	}

	public static int GetTimeHour()
	{
		return CSingleton<LightManager>.Instance.m_TimeHour;
	}

	public static int GetTimeMinute()
	{
		return CSingleton<LightManager>.Instance.m_TimeMin;
	}

	public static bool IsShopLightOn()
	{
		return CSingleton<LightManager>.Instance.m_IsShopLightOn;
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
		EvaluateQualitySetting();
	}

	protected void OnSettingUpdated(CEventPlayer_OnSettingUpdated evt)
	{
		EvaluateQualitySetting();
	}

	private void EvaluateQualitySetting()
	{
		if (m_CurrentQualitySettingIndex == CSingleton<CGameManager>.Instance.m_QualitySettingIndex)
		{
			return;
		}
		m_CurrentQualitySettingIndex = CSingleton<CGameManager>.Instance.m_QualitySettingIndex;
		if (m_CurrentQualitySettingIndex == 2)
		{
			m_PostProcessVolume.enabled = false;
			m_SunlightList[0].shadows = LightShadows.Hard;
			for (int i = 0; i < m_SoftShadowLightList.Count; i++)
			{
				m_SoftShadowLightList[i].shadows = LightShadows.None;
			}
		}
		else if (m_CurrentQualitySettingIndex == 1)
		{
			m_PostProcessVolume.enabled = false;
			m_SunlightList[0].shadows = LightShadows.Soft;
			for (int j = 0; j < m_SoftShadowLightList.Count; j++)
			{
				m_SoftShadowLightList[j].shadows = LightShadows.None;
			}
		}
		else
		{
			m_PostProcessVolume.enabled = true;
			m_SunlightList[0].shadows = LightShadows.Soft;
			for (int k = 0; k < m_SoftShadowLightList.Count; k++)
			{
				m_SoftShadowLightList[k].shadows = LightShadows.Soft;
			}
		}
	}

	public void RefreshBGM()
	{
		if (m_TImeOfDayIndex >= 2)
		{
			SoundManager.BlendToMusic("BGM_ShopNight", 1f, isLinearBlend: true);
		}
		else
		{
			SoundManager.BlendToMusic("BGM_ShopDay", 1f, isLinearBlend: true);
		}
	}

	public void SetCanChangeBGM(bool canChange)
	{
		m_CanChangeBGM = canChange;
	}
}
