using System;

[Serializable]
public class LightTimeData
{
	public bool m_IsLerpingSunIntensity;

	public bool m_IsBlendingSkybox;

	public bool m_IsStopBlendingSkybox;

	public bool m_HasDayEnded;

	public int m_TImeOfDayIndex;

	public int m_SkyboxIndex;

	public int m_TimeHour;

	public int m_TimeMin;

	public float m_TimeMinFloat;

	public float m_Timer;

	public float m_SunlightLerpTimer;

	public float m_SunlightRotationLerpTimer;

	public float m_GlobalBrightness;

	public float m_LerpStartBrightness;

	public float m_SkyboxBlendValue;

	public float m_SkyboxBlendSpeed;

	public float m_ShopLightOnTimer;

	public bool m_IsNightLightOn;

	public bool m_IsShopLightOn;

	public bool m_IsSunlightOn;
}
